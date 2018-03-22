using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Core.Helpers
{
    public static class SerializeHelper
    {
        public static string Serialize<T>(T toSerialize)
        {
            return JsonConvert.SerializeObject(toSerialize);
        }

        public static T Deserialize<T>(string toDeserialize)
        {
            return JsonConvert.DeserializeObject<T>(toDeserialize);
        }
    }
}
