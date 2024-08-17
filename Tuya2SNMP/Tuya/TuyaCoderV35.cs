using Tuya2SNMP.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP.Tuya
{
    internal class TuyaCoderV35
    {

        public int SeqNo { get; protected set; } = 0;

        private readonly byte[] localKey;
        public byte[] SessionKey;
        private byte[] UsedKey => SessionKey ?? localKey;

        public TuyaCoderV35(byte[] localKey)
            => this.localKey = localKey;

        private static readonly byte[] VERSION_HEADER = Encoding.UTF8.GetBytes("3.5");
        private static readonly TuyaCommand[] COMMANDS_WITHOUT_VERSION = new TuyaCommand[]
        {
            TuyaCommand.SESS_KEY_NEG_START,
            TuyaCommand.SESS_KEY_NEG_RES,
            TuyaCommand.SESS_KEY_NEG_FINISH,
            TuyaCommand.HEART_BEAT,
            TuyaCommand.DP_QUERY_NEW,
            TuyaCommand.UPDATE_DPS
        };

        public static readonly byte[] PREFIX = new byte[] { 0, 0, 0x66, 0x99 };
        public static readonly byte[] SUFFIX = { 0, 0, 0x99, 0x66 };
        private const int LENGTH_NONCE = 12;
        private const int LENGTH_TAG = 16;

        public TuyaLocalResponse DecodeResponse(byte[] data)
        {

            // Check length and prefix
            if (data.Length < 22 || !data.Take(PREFIX.Length).SequenceEqual(PREFIX))
                throw new InvalidDataException("Invalid header/prefix");

            // Check length
            int length = data.ToInt(14);
            if (data.Length != 22 + length)
                throw new InvalidDataException("Invalid length");

            // Check suffix
            if (!data.Part(18 + length, SUFFIX.Length).SequenceEqual(SUFFIX))
                throw new InvalidDataException("Invalid suffix");

            // Packet number
            uint seqNo = data.ToUInt(6);

            // Command
            var command = (TuyaCommand)data.ToUInt(10);
            if (command == TuyaCommand.UDP)
                return null;

            // Payload
            byte[] payload = data.Part(18 + LENGTH_NONCE, length - LENGTH_NONCE - LENGTH_TAG);
            byte[] nonce = data.Part(18, LENGTH_NONCE);
            byte[] tag = data.Part(18 + length - LENGTH_TAG, LENGTH_TAG);
            byte[] associated = data.Part(4, 14);

            byte[] decryptedPayload = Decrypt(payload, associated, nonce, tag);

            if (!COMMANDS_WITHOUT_VERSION.Contains(command))
                decryptedPayload = decryptedPayload.Part(15);

            int returnCode = data.Part(0, 4).ToInt();
            decryptedPayload = decryptedPayload.Part(4);

            return new TuyaLocalResponse(command, seqNo, returnCode, decryptedPayload);

        }

        internal byte[] EncodeRequest(TuyaCommand command, object content, out byte[] nonce)
        {

            byte[] payload;
            if (content is string contentStr)
                payload = contentStr.UTF8toBytes();
            else if (content is byte[] contentBytes)
                payload = contentBytes;
            else
                throw new ArgumentException("Content must be a JSON string or a byte array.", nameof(content));

            if (!COMMANDS_WITHOUT_VERSION.Contains(command))
            {
                byte[] bufferForHeader = new byte[payload.Length + 15];
                VERSION_HEADER.CopyTo(bufferForHeader, 0);
                payload.CopyTo(bufferForHeader, 15);
                payload = bufferForHeader;
            }

            byte[] unknown = new byte[] { 0, 0 };

            using MemoryStream memoryStream = new();
            memoryStream.Write(PREFIX, 0, 4);
            memoryStream.Write(unknown, 0, 2);
            memoryStream.WriteBytesBigEndian(++SeqNo);
            memoryStream.WriteBytesBigEndian((uint)command);
            memoryStream.WriteBytesBigEndian(payload.Length + LENGTH_NONCE + LENGTH_TAG); // 12: nonce, 16: tag
            byte[] assoc = memoryStream.ToArray().Part(4);
            payload = Encrypt(payload, assoc, out nonce, out byte[] tag);
            memoryStream.Write(nonce);
            memoryStream.Write(payload);
            memoryStream.Write(tag);
            memoryStream.Write(SUFFIX, 0, 4);
            return memoryStream.ToArray();

        }

        internal static byte[] Pad(byte[] payload)
        {
            byte padding = (byte)(0x10 - (payload.Length & 0x0F));
            byte[] buffer = new byte[payload.Length + padding];
            Array.Fill(buffer, padding);
            payload.CopyTo(buffer, 0);
            return buffer;
        }

        internal byte[] Decrypt(byte[] dataEncrypted, byte[] dataAssociated, byte[] nonce, byte[] tag)
        {
            AesGcm aes = new(UsedKey);
            byte[] data = new byte[dataEncrypted.Length];
            aes.Decrypt(nonce, dataEncrypted, tag, data, dataAssociated);
            return data;
        }
        
        internal byte[] Encrypt(byte[] data, byte[] dataAssociated, out byte[] nonce, out byte[] tag, byte[] givenNonce = null)
        {
            AesGcm aes = new(UsedKey);
            if (givenNonce != null)
            {
                nonce = givenNonce;
            }
            else
            {
                nonce = new byte[LENGTH_NONCE];
                RandomNumberGenerator.Fill(nonce);
            }
            tag = new byte[LENGTH_TAG];
            byte[] dataEncrypted = new byte[data.Length];
            aes.Encrypt(nonce, data, dataEncrypted, tag, dataAssociated);
            return dataEncrypted;
        }

        internal byte[] MAC(byte[] data)
        {
            using HMACSHA256 hmac = new(UsedKey);
            return hmac.ComputeHash(data);
        }


    }
}
