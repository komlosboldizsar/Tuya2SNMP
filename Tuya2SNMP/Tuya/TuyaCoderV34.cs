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
    internal class TuyaCoderV34
    {

        public int SeqNo { get; protected set; } = 0;

        private readonly byte[] localKey;
        public byte[] SessionKey;
        private byte[] UsedKey => SessionKey ?? localKey;

        public TuyaCoderV34(byte[] localKey)
            => this.localKey = localKey;

        private static readonly byte[] VERSION_HEADER = Encoding.UTF8.GetBytes("3.4");
        private static readonly TuyaCommandV34[] COMMANDS_WITHOUT_VERSION = new TuyaCommandV34[]
        {
            TuyaCommandV34.SESS_KEY_NEG_START,
            TuyaCommandV34.SESS_KEY_NEG_RES,
            TuyaCommandV34.SESS_KEY_NEG_FINISH,
            TuyaCommandV34.HEART_BEAT,
            TuyaCommandV34.DP_QUERY_NEW,
            TuyaCommandV34.UPDATE_DPS
        };

        public TuyaLocalResponse DecodeResponse(byte[] data)
        {

            // Check length and prefix
            if (data.Length < 20 || !data.Take(TuyaConstants.PREFIX.Length).SequenceEqual(TuyaConstants.PREFIX))
                throw new InvalidDataException("Invalid header/prefix");

            // Check length
            int length = data.ToInt(12);
            if (data.Length != 16 + length)
                throw new InvalidDataException("Invalid length");

            // Check suffix
            if (!data.Skip(16 + length - TuyaConstants.SUFFIX.Length).Take(TuyaConstants.SUFFIX.Length).SequenceEqual(TuyaConstants.SUFFIX))
                throw new InvalidDataException("Invalid suffix");

            // Packet number
            uint seqNo = data.ToUInt(4);

            // Command
            var command = (TuyaCommandV34)data.ToUInt(8);
            if (command == TuyaCommandV34.UDP)
                return null;

            // Return code
            int returnCode = data.ToInt(16);

            // Payload
            int payloadOffset = 16 + ((returnCode & 0xFFFFFF00) == 0x00000000 ? 4 : 0);
            byte[] payload = data.Part(payloadOffset, length - 36);

            // CRC
            byte[] expectedCrc = data.Part(data.Length - 36, 32);
            byte[] computedCrc = MAC(data.Part(0, -36));
            if (!expectedCrc.SequenceEqual(computedCrc))
                throw new InvalidDataException("Expected and received CRC don't match");

            byte[] decryptedPayload = Decrypt(payload.Part(0, -4));
            if (!COMMANDS_WITHOUT_VERSION.Contains(command))
                decryptedPayload = decryptedPayload.Part(15);
            if (decryptedPayload.Length > 0) // De-pad
            {
                byte last = decryptedPayload[^1];
                decryptedPayload = decryptedPayload.Part(0, -last);
            }

            return new TuyaLocalResponse(command, seqNo, returnCode, decryptedPayload);

        }

        internal byte[] EncodeRequest(TuyaCommandV34 command, object content)
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

            payload = Pad(payload);
            payload = Encrypt(payload);

            using MemoryStream memoryStream = new();
            memoryStream.Write(TuyaConstants.PREFIX, 0, 4);
            memoryStream.WriteBytesBigEndian(++SeqNo);
            memoryStream.WriteBytesBigEndian((uint)command);
            memoryStream.WriteBytesBigEndian(payload.Length + 36);
            memoryStream.Write(payload);
            memoryStream.Write(MAC(memoryStream.ToArray()));
            memoryStream.Write(TuyaConstants.SUFFIX, 0, 4);
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

        internal byte[] Decrypt(byte[] data) => Crypt(data, aes => aes.CreateDecryptor());
        internal byte[] Encrypt(byte[] data) => Crypt(data, aes => aes.CreateEncryptor());
        internal byte[] Crypt(byte[] data, Func<Aes, ICryptoTransform> createCryptor)
        {
            if (data.Length == 0)
                return data;
            Aes aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.Key = UsedKey;
            aes.Padding = PaddingMode.None;
            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new(memoryStream, createCryptor(aes), CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.Close();
            data = memoryStream.ToArray();
            aes.Dispose();
            return data;
        }

        internal byte[] MAC(byte[] data)
        {
            using HMACSHA256 hmac = new(UsedKey);
            return hmac.ComputeHash(data);
        }


    }
}
