using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.NET.Core.Helpers.Cryptography
{
    [Serializable]
    public class RSAParametersSerializable : ISerializable
    {
        private RSAParameters _rsaParameters;

        public RSAParameters RSAParameters
        {
            get
            {
                return _rsaParameters;
            }
        }

        public RSAParametersSerializable(RSAParameters rsaParameters)
        {
            _rsaParameters = rsaParameters;
        }

        private RSAParametersSerializable()
        {
        }

        public byte[] D { get { return _rsaParameters.D; } set { _rsaParameters.D = value; } }

        public byte[] DP { get { return _rsaParameters.DP; } set { _rsaParameters.DP = value; } }

        public byte[] DQ { get { return _rsaParameters.DQ; } set { _rsaParameters.DQ = value; } }

        public byte[] Exponent { get { return _rsaParameters.Exponent; } set { _rsaParameters.Exponent = value; } }

        public byte[] InverseQ { get { return _rsaParameters.InverseQ; } set { _rsaParameters.InverseQ = value; } }

        public byte[] Modulus { get { return _rsaParameters.Modulus; } set { _rsaParameters.Modulus = value; } }

        public byte[] P { get { return _rsaParameters.P; } set { _rsaParameters.P = value; } }

        public byte[] Q { get { return _rsaParameters.Q; } set { _rsaParameters.Q = value; } }

        public RSAParametersSerializable(SerializationInfo information, StreamingContext context)
        {
            _rsaParameters = new RSAParameters();

            foreach (SerializationEntry entry in information)
            {
                switch (entry.Name)
                {
                    case "D":
                        D = (byte[])information.GetValue("D", typeof(byte[])); break;
                    case "DP":
                        DP = (byte[])information.GetValue("DP", typeof(byte[])); break;
                    case "DQ":
                        DQ = (byte[])information.GetValue("DQ", typeof(byte[])); break;
                    case "Exponent":
                        Exponent = (byte[])information.GetValue("Exponent", typeof(byte[])); break;
                    case "InverseQ":
                        InverseQ = (byte[])information.GetValue("InverseQ", typeof(byte[])); break;
                    case "Modulus":
                        Modulus = (byte[])information.GetValue("Modulus", typeof(byte[]));  break;
                    case "P":
                        P = (byte[])information.GetValue("P", typeof(byte[]));  break;
                    case "Q":
                        Q = (byte[])information.GetValue("Q", typeof(byte[])); break;
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("D", _rsaParameters.D);
            info.AddValue("DP", _rsaParameters.DP);
            info.AddValue("DQ", _rsaParameters.DQ);
            info.AddValue("Exponent", _rsaParameters.Exponent);
            info.AddValue("InverseQ", _rsaParameters.InverseQ);
            info.AddValue("Modulus", _rsaParameters.Modulus);
            info.AddValue("P", _rsaParameters.P);
            info.AddValue("Q", _rsaParameters.Q);
        }
    }
}
