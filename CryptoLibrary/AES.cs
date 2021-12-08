using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CryptoLibrary
{
    public class AES
    {
        private static string _privateKey;
        private static string _publicKey;
        private const int _aesKeySize = 256;

        public void SetPublicKey(string key) 
        {
            _publicKey = key;
        }

        public void SetPrivateKey(string key)
        {
            _privateKey = key;
        }

        public string Encrypt(string data)
        {
            byte[] init_vector_bytes = Encoding.UTF8.GetBytes(_privateKey);
            byte[] plain_text_bytes = Encoding.UTF8.GetBytes(data);
            PasswordDeriveBytes password = new PasswordDeriveBytes(_publicKey, null);
            byte[] key_bytes = password.GetBytes(_aesKeySize / 8);
            RijndaelManaged symmetric_key = new RijndaelManaged();
            symmetric_key.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetric_key.CreateEncryptor(key_bytes, init_vector_bytes);
            MemoryStream memory_stream = new MemoryStream();
            CryptoStream crypto_Stream = new CryptoStream(memory_stream, encryptor, CryptoStreamMode.Write);
            crypto_Stream.Write(plain_text_bytes, 0, plain_text_bytes.Length);
            crypto_Stream.FlushFinalBlock();
            byte[] encrypted = memory_stream.ToArray();
            memory_stream.Close();
            crypto_Stream.Close();
            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string data)
        {
            byte[] init_vector_bytes = Encoding.ASCII.GetBytes(_privateKey);
            byte[] decrypted_text = Convert.FromBase64String(data);
            PasswordDeriveBytes password = new PasswordDeriveBytes(_publicKey, null);
            byte[] key_bytes = password.GetBytes(_aesKeySize / 8);
            RijndaelManaged symmetric_key = new RijndaelManaged();
            symmetric_key.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetric_key.CreateDecryptor(key_bytes, init_vector_bytes);
            MemoryStream memory_stream = new MemoryStream(decrypted_text);
            CryptoStream crypto_stream = new CryptoStream(memory_stream, decryptor, CryptoStreamMode.Read);
            byte[] plain_text_bytes = new byte[decrypted_text.Length];
            int decrypted_byte_count = crypto_stream.Read(plain_text_bytes, 0, plain_text_bytes.Length);
            memory_stream.Close();
            crypto_stream.Close();
            return Encoding.UTF8.GetString(plain_text_bytes, 0, decrypted_byte_count);
        }

    }
}
