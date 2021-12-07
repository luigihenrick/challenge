using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infraestructure.Common
{
    sealed class UnixEpochDateTimeConverter : JsonConverter<DateTime>
    {
        static readonly DateTime s_epoch = new DateTime(1970, 1, 1, 0, 0, 0);

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            long unixTime = reader.GetInt64();

            return s_epoch.AddSeconds(unixTime);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // long unixTime = Convert.ToInt64((value - s_epoch).TotalSeconds);

            writer.WriteStringValue(value);
        }
    }
}