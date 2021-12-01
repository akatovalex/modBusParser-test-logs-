using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modBusParse {
     class MBParse {

        public static string Parse(string input, MBExceptions mBExceptions, Commands commands, string outputFormat = ".json") {
            SourceList sourceList = new SourceList();
            List<string> inputString = new List<string>();
            inputString.AddRange(input.Split('\n'));

            string output = "";
            Source source = null;
            foreach (var line in inputString) {

                string Delimiter = "\t";        // В исходном примере большая часть данных отделена друг от друга табуляциями (всегда)

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
                var words = line.Trim().Split(new[] { Delimiter }, 7, StringSplitOptions.RemoveEmptyEntries);

                /*string lengthString;          // Можно использовать более сложные конструкции для парсинга данных, 
                                                // однако для полноценных логов всё равно придётся вносить правки
                string rwString;
                string serial;
                foreach (var word in words) {
                    if (word.Trim().StartsWith("IRP_")) {
                        rwString = word.Trim();
                    } else if (word.Trim().StartsWith("Serial")) {
                        serial = word;
                    }
                     else if (word.StartsWith("Length")) {
                        lengthString = word;
                    }
                }*/
                Line newLine = new Line();

                if (words.Length == 7 && words[6].StartsWith("Length ")) {    //простейшая проверка структуры вводимой строки
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
                                                                            // Если применить на всю строку, можно случайно обрезать последнее значение вроде 0xEE (0xee)
                    ParsingLine(ref newLine, length, words, mBExceptions, commands);

                    source.LineList.Add(newLine);
                }

            }


            if (outputFormat == ".json") {
                output = CreateJSON(sourceList);
            }
            else {
                output = CreateXML(sourceList);
            }

            return output;
        }

        private static void ParsingLine(ref Line newLine, int length, string[] words, MBExceptions mBExceptions, Commands commands) {
            if (length > 0) {
                var byteArrayString = words[6].Split(':')[1].Trim();
                newLine.Direction = words[3];


                newLine.RawFrame = new byte[length];
                var byteArrayStringSplit = byteArrayString.Split(' ');
                for (int i = 0; i < length; i++) {
                    newLine.RawFrame[i] = Convert.ToByte(byteArrayStringSplit[i], 16);
                }
                if (length >= 4) {
                    newLine.RawData = new byte[length - 4];
                    for (int i = 0; i < newLine.RawFrame.Count() - 4; i++) {
                        newLine.RawData[i] = newLine.RawFrame[i + 2];           // починить вывод в логах с Base64 на побайтовый HEX
                    }

                    // Выводит сначала старший бит CRC, затем младший
                    // в ModBus пакете выводится в обратном порядке
                    string crc = CheckSum.CRC16(newLine.RawFrame, length - 2).ToString("X4");
                    // Разворот строки (в примере используется такой формат)
                    newLine.CRC = crc.Substring(2, 2) + crc.Substring(0, 2);
                    if ( newLine.CRC != (newLine.RawFrame[length-2].ToString("X2")+newLine.RawFrame[length-1].ToString("X2")) ) {
                        newLine.Error = "Wrong CRC";
                    }

                    newLine.Address = newLine.RawFrame[0].ToString("X2");
                    newLine.Command =  newLine.RawFrame[1].ToString("X2");       // временный вывод байтов - поменять на команду
                    if (newLine.RawFrame[1] >= 128) {
                        if (length == 5) {
                            //newLine.Exception = newLine.RawFrame[2].ToString("X2"); // временный вывод байтов - поменять на сообщение об ошибке
                            string keyException = newLine.RawFrame[2].ToString("X2");
                            string exception = "";
                            if (!mBExceptions.MBException.TryGetValue(keyException, out exception)) {
                                exception = "Unknown";
                            }
                            newLine.Exception = keyException + ":" + exception;
                        } else {
                            newLine.Error = "Wrong exception packet's length (length cannot be more or less than 5 bytes): " + length + " bytes";
                        }
                    }
                    string keyCommand = (newLine.RawFrame[1] % 128).ToString("X2");
                    string command = "";
                    if (!commands.Command.TryGetValue(keyCommand, out command)) {
                        command = "Unknown";
                    }
                    newLine.Command = keyCommand + ":" + command;
                }
                else {
                    // пакет битый
                    newLine.Error = "Wrong data packet's length (length cannot be less than 4 bytes): " + length + " bytes ";
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


