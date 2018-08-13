using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TWParse
{
    //кастомный Json конвертер 
    public class CustomJsonConverter : JsonConverter
    {
        private readonly Type[] _types;

        public CustomJsonConverter(params Type[] types)
        {
            _types = types;
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            //убираем имена свойств класса TweetsInfo, и символ из значения делаем именем свойства
            JObject o = new JObject();
            o.Add(new JProperty(((TweetsInfo)value).Letter.ToString(), ((TweetsInfo)value).Freq));
            o.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return _types.Any(t => t == objectType);
        }
    }

}



