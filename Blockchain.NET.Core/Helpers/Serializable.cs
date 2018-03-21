using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Core.Helpers
{
    public class Serializable<T> : Encryptable
    {
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public byte[] Serialize(string password)
        {
            var frontLockHash = GetFrontLockSha(password, password);
            return AESEncryptString(JsonConvert.SerializeObject(this), password, frontLockHash);
        }

        public static T Deserialize(string toDeserialize)
        {
            return JsonConvert.DeserializeObject<T>(toDeserialize);
        }

        public static T Deserialize(byte[] toDeserialize, string password)
        {
            var frontLockHash = GetFrontLockSha(password, password);
            return JsonConvert.DeserializeObject<T>(AESDecryptBytes(toDeserialize, password, frontLockHash));
        }
    }
}
