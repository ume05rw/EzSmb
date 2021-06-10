using Newtonsoft.Json;
using System;

namespace EzSmbTest.Models
{
    [JsonConverter(typeof(ItemTypeConverter))]
    public enum ItemType
    {
        Server = 1,
        Share = 2,
        Foder = 3,
        File = 4
    }

    public class ItemTypeConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ItemType);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer
        )
        {
            var stringValue = (string)reader.Value;

            return stringValue switch
            {
                "server" => ItemType.Server,
                "share" => ItemType.Share,
                "folder" => ItemType.Foder,
                "file" => ItemType.File,
                _ => throw new InvalidCastException($"Unexpected ItemType: {stringValue}"),
            };
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer
        )
        {
            throw new NotImplementedException();
        }
    }

}
