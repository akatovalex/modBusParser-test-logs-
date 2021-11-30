using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modBusParse {
     class MBParse {

        public static string Parse(string input, MBExceptions mBExceptions, Commands commands, string outputFormat = ".json") {
            SourceList sourceList = new SourceList();
            //Source source = new Source();
            List<string> inputString = new List<string>();
            inputString.AddRange(input.Split('\n'));

            string output = "";
            Source source = null;
            foreach (var line in inputString) {

                string Delimiter = "\t";
                /*int[] skippedInfoIndexes = {0,1,2 };  //"слова" из исходного пакета, которые не нужны в итоговом логе
                 * 
                 * 184	11:26:50	AppName	IRP_MJ_WRITE	Serial0	SUCCESS	Length 7: FE 46 00 3B 01 CA 6C 	
                 * 
                 * words:
                 * [0]  184
                 * [1]  11:26:50
                 * [2]  AppName
                 * [3]  IRP_MJ_WRITE
                 * [4]  Serial0
                 * [5]  SUCCESS
                 * [6]  Length 7: FE 46 00 3B 01 CA 6C 	
                 * 
                 */
                //foreach (var word in line.Split(new[] { Delimiter }, StringSplitOptions.None).Select((value, i) => new { i, value })) {
                var words = line.Split(new[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries);
                Line newLine = new Line();

                if (words.Length > 4) {

                    foreach (var element in sourceList.sources) {
                        if (element.Address == words[4]) {
                            source = element;
                            break;
                        }
                        else {
                            source = null;
                        }
                    }
                    if (source == null) {
                        source = new Source();
                        sourceList.sources.Add(source);
                    }
                    source.Address = words[4];
                    newLine.Error = words[5];

                    int length = Convert.ToInt32(words[6].Split(':')[0].Trim('L','e','n','g','t','h',' '));     // ¯\_(ツ)_/¯
                    ParsingLine(ref newLine, length, words);

                    source.LineList.Add(newLine);
                }

                if (outputFormat == ".json") {
                    output = CreateJSON(sourceList);
                }
                else {
                    output = CreateXML(sourceList);
                }
            }


            return output;
        }

        private static void ParsingLine(ref Line newLine, int length, string[] words) {
            if (length > 0) {
                var byteArrayString = words[6].Split(':')[1].Trim();
                newLine.Direction = words[3];
                //var byteArrayString = Convert.ToByte(words[6].Split(':')[1].Trim().Split(' '));
                if (length >= 4) {
                    newLine.RawFrame = new byte[length];

                    var byteArrayStringSplit = byteArrayString.Split(' ');
                    for (int i = 0; i < length; i++) {
                        newLine.RawFrame[i] = Convert.ToByte(byteArrayStringSplit[i], 16);
                    }
                    newLine.RawData = new byte[length - 4];
                    for (int i = 0; i < newLine.RawFrame.Count() - 4; i++) {
                        newLine.RawData[i] = newLine.RawFrame[i + 2];           // починить вывод в логах с Base64 на побайтовый HEX
                    }
                    // Выводит сначала старший бит CRC, затем младший
                    // в ModBus пакете выводится в обратном порядке
                    newLine.CRC = CheckSum.CRC16(newLine.RawFrame, length - 2).ToString("X2");

                    newLine.Address = newLine.RawFrame[0].ToString("X2");
                    newLine.Command = newLine.RawFrame[1].ToString("X2");       // временный вывод байтов - поменять на команду
                    if (newLine.RawFrame[1] >= 80 && length == 5) {
                        newLine.Exception = newLine.RawFrame[2].ToString("X2"); // временный вывод байтов - поменять на сообщение об ошибке
                    }
                }
                else {
                    // пакет битый
                }
            }

        }

        private static string CreateJSON(SourceList sourceList) {
            return Newtonsoft.Json.JsonConvert.SerializeObject(sourceList,
                            Newtonsoft.Json.Formatting.Indented
                            , new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore }  //Отображение NULL полей
                            );
        }

        private static string CreateXML(SourceList sourceList) {
            try {
                var stringwriter = new System.IO.StringWriter();
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SourceList));
                //XmlWriterSettings settings = new XmlWriterSettings();
                //settings.Indent = true;
                //settings.IndentChars = "    ";
                serializer.Serialize(stringwriter, sourceList);
                return stringwriter.ToString();
            }
            catch {
                throw;
            }
            //return null;
        }

    }

}


