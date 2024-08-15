using Tuya2SNMP.Helpers;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tuya2SNMP.Tuya
{

    public class TuyaDeviceV34 : ITuyaDevice, IDisposable
    {

        public string IP { get; private set; }
        public byte[] LocalKey { get; set; }

        private byte[] _sessionKey;
        public byte[] SessionKey
        {
            get => _sessionKey;
            set
            {
                _sessionKey = value;
                coder.SessionKey = value;
            }
        }

        public int Port { get; private set; } = 6668;
        public int ConnectionTimeout { get; set; } = 500;
        public int HeartbeatInterval { get; set; } = 5000;
        public int SecondsBeforeDisconnect { get; set; } = 15;

        public TuyaDeviceV34(string ip, byte[] localKey, int port = 6668)
        {
            _lastSeen = DateTime.Now;
            IP = ip;
            LocalKey = localKey;
            Port = port;
            coder = new(localKey);
            _ = Task.Run(HeartbeatTaskAsync);
        }

        public TuyaDeviceV34(string ip, string localKey, int port = 6668)
            : this(ip, localKey.UTF8toBytes(), port)
        { }

        private TcpClient client = null;
        private readonly SemaphoreSlim sem = new(1);
        internal readonly TuyaCoderV34 coder;

        public byte[] EncodeRequest(TuyaCommandV34 command, object content)
            => coder.EncodeRequest(command, content);

        public async Task SendRawAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            using SemaphoreLock semaphore = await sem.WaitDisposableAsync(cancellationToken);
            await _networkStream.WriteAsync(data, 0, data.Length, cancellationToken).ConfigureAwait(false);
        }

        public async Task SendAsync(TuyaCommandV34 command, object content, CancellationToken cancellationToken = default)
            => await SendRawAsync(EncodeRequest(command, content), cancellationToken);

        public async Task SendWithinConnectionAsync(TuyaCommandV34 command, object content, CancellationToken cancellationToken = default)
        {
            if (!_connectedMRS.IsSet)
            {
                await ConnectAsync(cancellationToken);
                await _connectedMRS.WaitAsync(cancellationToken);
            }
            await SendAsync(command, content, cancellationToken);
            if (!PermanentConnection)
            {
                Disconnect();
            }
        }

        public bool PermanentConnection { get; set; }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (client == null)
            {
                _keyExchangeStarted = false;
                _connectedMRS.Reset();
                client = new TcpClient();
                if (!await client.ConnectAsync(IP, Port).WithTimeout(ConnectionTimeout, cancellationToken))
                    throw new IOException("Connection timeout");
                _networkStream = client.GetStream();
                _receiveTaskCancellationTokenSource = new();
                CancellationTokenSource receiveTaskCombinedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_receiveTaskCancellationTokenSource.Token, cancellationToken);
                _receiveTask = Task.Run(ReceiveTaskAsync, receiveTaskCombinedCancellationTokenSource.Token);
                await SendSessKeyNegStartAsync(cancellationToken);
            }
        }

        public void Disconnect()
        {
            if (!Connected)
                return;
            Connected = false;
            _connectedMRS.Reset();
            SessionKey = null;
            _receiveTaskCancellationTokenSource.Cancel();
            try
            {
                client.Close();
            }
            catch { }
            client = null;
        }

        private volatile bool _connected;
        public bool Connected
        {
            get => _connected;
            set
            {
                if (value == _connected)
                    return;
                _connected = value;
                if (_connected)
                    ConnectionEstablished?.Invoke(this, true);
                else
                    ConnectionLost?.Invoke(this, false);
            }
        }

        public event TuyaDeviceConnectionStateChangedHandler ConnectionEstablished;
        public event TuyaDeviceConnectionStateChangedHandler ConnectionLost;

        private CancellationTokenSource _receiveTaskCancellationTokenSource;

        private const int KEY_LENGTH = 16;
        private const int CRC_LENGTH = 32;

        private bool _keyExchangeStarted = false;
        private byte[] _tempKeyLocal;
        private NetworkStream _networkStream;
        private AsyncManualResetEvent _connectedMRS = new(false);
        private Task _receiveTask;
        private int _unrespondedHeartbeats = 0;

        private async Task ReceiveTaskAsync()
        {
            byte[] buffer = new byte[1024];
            CancellationToken cancellationToken = _receiveTaskCancellationTokenSource.Token;
            while (true)
            {
                using MemoryStream currentPacketMemoryStream = new();
                int bytes = await _networkStream.ReadAsync(buffer, 0, 16, cancellationToken);
                int seekOffset = 0;
                while (seekOffset + 4 >= bytes && !buffer.Part(seekOffset, 4).SequenceEqual(TuyaConstants.PREFIX))
                    seekOffset += 4;
                await currentPacketMemoryStream.WriteAsync(buffer, seekOffset, bytes - seekOffset);
                int length = buffer.ToInt(seekOffset + 12);
                bytes = await _networkStream.ReadAsync(buffer, 0, length, cancellationToken);
                await currentPacketMemoryStream.WriteAsync(buffer, 0, length);
                byte[] packet = currentPacketMemoryStream.ToArray();
                TuyaLocalResponse response = coder.DecodeResponse(packet);
                await HandleResponseAsync(response);
            }
        }

        private async Task HeartbeatTaskAsync()
        {
            while (true)
            {
                if ((DateTime.Now - _lastSeen).TotalSeconds > SecondsBeforeDisconnect)
                    Disconnect();
                if (PermanentConnection && _connectedMRS.IsSet)
                    await SendHeartbeatAsync();
                await Task.Delay(HeartbeatInterval);
            }
        }

        private DateTime _lastSeen;

        private async Task HandleResponseAsync(TuyaLocalResponse response)
        {

            _lastSeen = DateTime.Now;

            if (response.Command == TuyaCommandV34.HEART_BEAT)
            {
                return;
            }

            if (response.Command == TuyaCommandV34.STATUS)
            {
                if (string.IsNullOrEmpty(response.JSON))
                    return;
                var root = JObject.Parse(response.JSON);
                if (root.GetValue("protocol").ToString() != "4")
                    return;
                var dps = JsonConvert.DeserializeObject<Dictionary<string, object>>(root["data"]["dps"].ToString());
                Dictionary<int, object> dpsDict = dps.ToDictionary(kv => int.Parse(kv.Key), kv => kv.Value);
                DpsUpdated?.Invoke(this, dpsDict);
                return;
            }

            if (response.Command == TuyaCommandV34.DP_QUERY_NEW)
            {
                if (string.IsNullOrEmpty(response.JSON))
                    return;
                var root = JObject.Parse(response.JSON);
                var dps = JsonConvert.DeserializeObject<Dictionary<string, object>>(root.GetValue("dps").ToString());
                Dictionary<int, object> dpsDict = dps.ToDictionary(kv => int.Parse(kv.Key), kv => kv.Value);
                DpsUpdated?.Invoke(this, dpsDict);
                return;
            }

            if (response.Command == TuyaCommandV34.SESS_KEY_NEG_RES)
            {
                if (!_keyExchangeStarted)
                    return;
                if (!CheckSessKeyNegResKeys(response.ByteArray, out byte[] tempKeyRemote))
                    return;
                await SendSessKeyNegFinishAsync(tempKeyRemote);
                SessionKey = CalculateSessionKey(tempKeyRemote);
                _keyExchangeStarted = false;
                _unrespondedHeartbeats = 0;
                _connectedMRS.Set();
                Connected = true;
                return;
            }

        }

        public event TuyaDeviceDpsUpdatedHandler DpsUpdated;

        private async Task SendSessKeyNegStartAsync(CancellationToken cancellationToken = default)
        {
            _tempKeyLocal = RandomBytes(KEY_LENGTH);
            _keyExchangeStarted = true;
            await SendAsync(TuyaCommandV34.SESS_KEY_NEG_START, _tempKeyLocal, cancellationToken);
        }

        private bool CheckSessKeyNegResKeys(byte[] payload, out byte[] tempKeyRemote)
        {
            tempKeyRemote = payload.Part(0, KEY_LENGTH);
            byte[] tempKeyLocalCrc = coder.MAC(_tempKeyLocal);
            byte[] expectedTempKeyLocalCrc = payload.Part(KEY_LENGTH, CRC_LENGTH);
            return tempKeyLocalCrc.SequenceEqual(expectedTempKeyLocalCrc);
        }

        private byte[] CalculateSessionKey(byte[] tempKeyRemote)
        {
            byte[] sessionKeyNotEncoded = new byte[KEY_LENGTH];
            for (int i = 0; i < KEY_LENGTH; i++)
                sessionKeyNotEncoded[i] = (byte)(_tempKeyLocal[i] ^ tempKeyRemote[i]);
            return coder.Encrypt(sessionKeyNotEncoded);
        }

        private async Task SendSessKeyNegFinishAsync(byte[] tempKeyRemote, CancellationToken cancellationToken = default)
        {
            byte[] tempKeyremoteCrc = coder.MAC(tempKeyRemote);
            await SendAsync(TuyaCommandV34.SESS_KEY_NEG_FINISH, tempKeyremoteCrc, cancellationToken);
        }

        private byte[] RandomBytes(int count)
        {
            byte[] bytes = new byte[count];
            new Random().NextBytes(bytes);
            return bytes;
        }

        public async Task SendHeartbeatAsync(CancellationToken cancellationToken = default)
        {
            _unrespondedHeartbeats++;
            await SendWithinConnectionAsync(TuyaCommandV34.HEART_BEAT, FillJson(null).UTF8toBytes(), cancellationToken);
        }

        public async Task QueryDpsAsync(CancellationToken cancellationToken = default)
            => await SendWithinConnectionAsync(TuyaCommandV34.DP_QUERY_NEW, FillJson(null).UTF8toBytes(), cancellationToken);

        public async Task SetDpAsync(int dp, object value, CancellationToken cancellationToken = default)
            => await SetDpsAsync(new Dictionary<int, object> { { dp, value } }, cancellationToken);

        public async Task SetDpsAsync(Dictionary<int, object> dps, CancellationToken cancellationToken = default)
            => await SendWithinConnectionAsync(TuyaCommandV34.CONTROL_NEW, FillJson(new Dictionary<string, object> { { "dps", dps } }).UTF8toBytes(), cancellationToken);

        public string FillJson(string dataJson)
        {
            var dataObj = string.IsNullOrEmpty(dataJson) ? new JObject() : JObject.Parse(dataJson);
            dataObj.AddFirst(new JProperty("ctype", 0));
            var root = new JObject
            {
                new JProperty("data", dataObj),
                new JProperty("protocol", 5),
                new JProperty("t", TimeHelpers.UnixSeconds.ToString("0"))
            };
            return root.ToString(Formatting.None);
        }

        public string FillJson(object data)
            => FillJson(JsonConvert.SerializeObject(data, Formatting.None));

        public void Dispose()
        {
            client?.Close();
            client?.Dispose();
            client = null;
        }

    }
}
