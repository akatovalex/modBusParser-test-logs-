using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modBusParse {
     class MBParse {
        public MBParse() {

        }

        public static string Parse(string input, MBExceptions mBExceptions, Commands commands, string outputFormat = ".json") {
            SourceList sourceList = new SourceList();
            //Source source = new Source();
            List<string> inputString = new List<string>();
            inputString.AddRange(input.Split('\n'));

            string output = "";
            Source source = null;
            foreach (var line in inputString) {

                string Delimiter = "\t";
                //int[] skippedInfoIndexes = {0,1,2,3,7 };  //"слова" из исходного пакета, которые не нужны в итоговом логе
                //foreach (var word in line.Split(new[] { Delimiter }, StringSplitOptions.None).Select((value, i) => new { i, value })) {
                var words = line.Split(new[] { Delimiter }, StringSplitOptions.None);
                //for (int i = 0; i < words.Count(); i++) { 

                Line newLine = new Line();
                foreach (var word in words) {
                    //output += word + "\n";
                }



                foreach (var element in sourceList.sources) {
                    if (element.Address == words[4]) {
                        source = element;
                        break;
                    } else {
                        source = null;
                    }
                }
                if (source == null) {
                    source = new Source();
                    sourceList.sources.Add(source);
                }
                


                if (words.Length > 4) {
                    source.Address = words[4];
                    newLine.Error = words[5];
                    output += words[4] + ' ' + words[5] + "\n";
                    int length = Convert.ToInt32(words[6].Split(':')[0].Trim('L','e','n','g','t','h',' '));     // ¯\_(ツ)_/¯
                    var byteArrayString = words[6].Split(':')[1].Trim();
                    //var byteArrayString = Convert.ToByte(words[6].Split(':')[1].Trim().Split(' '));
                    output += length + "\n";
                    if (length > 0) {
                        output += byteArrayString.ToString() + "\n";

                        if (length > 4) {
                            newLine.RawFrame = new byte[length];

                            var byteArrayStringSplit = byteArrayString.Split(' ');
                            for (int i = 0; i < length; i++) {
                                newLine.RawFrame[i] = Convert.ToByte(byteArrayStringSplit[i], 16);
                                output += newLine.RawFrame[i] + " ";
                            }
                            output += '\n';
                            newLine.RawData = new byte[length - 4];
                            for (int i = 0; i < newLine.RawFrame.Count() - 4; i++) {
                                newLine.RawData[i] = newLine.RawFrame[i + 2];
                                output += newLine.RawData[i] + " ";
                            }
                            output += '\n';
                            newLine.CRC = CheckSum.CRC16(newLine.RawFrame,length-2).ToString("X2");
                        }
                    }



                    source.LineList.Add(newLine);
                }
                if (outputFormat == ".json") {
                    output = Newtonsoft.Json.JsonConvert.SerializeObject(sourceList,
                            Newtonsoft.Json.Formatting.Indented
                            , new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore}  //Отображение NULL полей
                            );
                }
                else {
                    /*
                    System.Xml.XmlDocument xNode = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(Newtonsoft.Json.JsonConvert.SerializeObject(sourceList
                            , new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore }
                            )
                        ,"root");
                        output = xNode.OuterXml;
                        */
                    try {
                        //  xml
                        var stringwriter = new System.IO.StringWriter();
                        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SourceList));
                        serializer.Serialize(stringwriter, sourceList);
                        output = stringwriter.ToString();
                    }
                    catch {
                        throw;
                    }
                    //output = xNode.OuterXml;
                }

            }


            return output;
        }


    }
}
