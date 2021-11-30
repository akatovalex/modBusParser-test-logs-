using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modBusParse {
    public class ByteArrayConverter : Newtonsoft.Json.JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(byte[]);
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer) {
            if (reader.TokenType == Newtonsoft.Json.JsonToken.String) {
                var hex = serializer.Deserialize<string>(reader);
                if (!string.IsNullOrEmpty(hex)) {
                    return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
                }
            }
            return Enumerable.Empty<byte>();
        }


        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer) {
            var bytes = value as byte[];
            string s = BitConverter.ToString(bytes).Replace("-", " 0x");
            s = s.Insert(0, "0x");
            serializer.Serialize(writer, s);
        }

        /*
         при сериализации byte[] преобразуется в строку (и в XML и в JSON)

        Если обработать как массив int, то значения будут десятичными и по умолчанию пишутся на отдельной строке
        При попытке "искусственно" перевести массив (решение по ссылке №3) в hex, массив чисел превращается в массив строк
        и также пишется на отдельных строках
                
        идеальное решение - создать новый byteArrayConverter и использовать его, но это всё ещё будет строка (!) с HEX-значениями (без 0x..). 
        И данное решение не будет распространяться на XML
                /// https://stackoverflow.com/questions/15226921/how-to-serialize-byte-as-simple-json-array-and-not-as-base64-in-json-net *
                /// https://stackoverflow.com/questions/11829035/newton-soft-json-jsonserializersettings-for-object-with-property-as-byte-array
                /// https://stackoverflow.com/questions/15226921/how-to-serialize-byte-as-simple-json-array-and-not-as-base64-in-json-net
        Для XML достаточно использовать [System.Xml.Serialization.XmlElement(DataType = "hexBinary")]
        
        Другое решение:
        если заменить byte[] на uint[], то помимо увеличенного выделения памяти числа парсятся в десятичной системе счисления - также надо обрабатывать
        однако это уже парсится не как строка, а как массив элементов и распространяется и на JSON, и на XML 
        (в XML выглядит неудобно, т.к. каждое число пишется на отдельной строке с указанием типа данных)
         */
    }
}
