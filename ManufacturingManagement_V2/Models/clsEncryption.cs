using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace ManufacturingManagement_V2.Models
{
    /// <summary>
    /// Summary description for clsEncryption
    /// </summary>
    /// 
    internal class clsEncryption
    {
        //
        #region variables
        // define the triple des provider
        private TripleDESCryptoServiceProvider m_des =new TripleDESCryptoServiceProvider();
        // define the string handler
        private UTF8Encoding m_utf8 = new UTF8Encoding();
        // define the local property arrays
        private byte[] m_key;
        private byte[] m_iv;
        //
        #endregion variables
        //
        internal clsEncryption()
        {
            byte[] key = { 32, 78, 13, 4, 54, 31, 99, 74, 53, 69, 94, 52, 17, 124, 15, 106, 64, 99, 37, 37, 89, 102, 83, 60 };
            byte[] iv = { 35, 213, 127, 191, 165, 212, 201, 147 };
            this.m_key = key;
            this.m_iv = iv;
        }
        //
        internal string Encrypt(string text)
        {
            byte[] input = m_utf8.GetBytes(text);
            byte[] output = Transform(input, m_des.CreateEncryptor(m_key, m_iv));
            return Convert.ToBase64String(output);
        }
        //
        internal string Decrypt(string text)
        {
            byte[] input = Convert.FromBase64String(text);
            byte[] output = Transform(input, m_des.CreateDecryptor(m_key, m_iv));
            return m_utf8.GetString(output);
        }
        //
        private byte[] Transform(byte[] input,ICryptoTransform CryptoTransform)
        {
            // create the necessary streams
            MemoryStream memStream = new MemoryStream();
            CryptoStream cryptStream = new CryptoStream(memStream, CryptoTransform, CryptoStreamMode.Write);
            // transform the bytes as requested
            cryptStream.Write(input, 0, input.Length);
            cryptStream.FlushFinalBlock();
            // Read the memory stream and
            // convert it back into byte array
            memStream.Position = 0;
            byte[] result = memStream.ToArray();
            // close and release the streams
            memStream.Close();
            cryptStream.Close();
            // hand back the encrypted buffer
            return result;
        }
        //
    }
}