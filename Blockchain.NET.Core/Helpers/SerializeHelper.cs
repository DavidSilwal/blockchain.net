using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Core.Helpers
{
    public static class SerializeHelper
    {
        public static string Serialize<T>(T toSerialize, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(toSerialize, formatting);
        }

        public static T Deserialize<T>(string toDeserialize)
        {
            return JsonConvert.DeserializeObject<T>(toDeserialize);
        }
    }
}
