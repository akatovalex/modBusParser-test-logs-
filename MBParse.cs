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
            string delimiterNewLine = "\n";
            inputString.AddRange(input.Split(new[] { delimiterNewLine }, StringSplitOptions.RemoveEmptyEntries));

            string output = "";
            Source source = null;

            string rwLast = "";
            Line newLine = null;
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
                                                // однако для полноценных логов всё равно придётся вносить правки,
                                                // для входных примеров из папки input logs работает
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


                if (words.Length == 7 && words[6].StartsWith("Length ")) {    //простейшая проверка структуры вводимой строки
                    if (rwLast != words[3]) {               // Новый пакет, не продолжение предыдущего

                        newLine = new Line();

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

                        int length = Convert.ToInt32(words[6].Split(':')[0].Trim('L', 'e', 'n', 'g', 't', 'h', ' '));     // ¯\_(ツ)_/¯
                                                                                                                          // Если применить на всю строку, можно случайно обрезать последнее значение вроде 0xEE (0xee)
                        ParsingNewLine(ref newLine, length, words, mBExceptions, commands);

                        source.LineList.Add(newLine);
                    } else {                // продолжение старого пакета

                        /*
                        foreach (var element in sourceList.sources) {
                            if (element.Address == words[4]) {
                                source = element;
                                break;
                            }
                            else {
                                source = null;
                            }
                        }*/
                        if (words[5] == "TIMEOUT") {
                            newLine.Address = null;
                            newLine.Command = null;
                            newLine.CRC = null;
                            newLine.RawFrame = null;
                            newLine.RawData = null;
                            newLine.Exception = null;

                            newLine.Direction = "IRP_MJ_READ";
                            newLine.Error = "TIMEOUT";
                        }
                        else {
                            if (source == null) {
                                source = new Source();
                                sourceList.sources.Add(source);
                            }
                            //source.Address = words[4];

                            if (newLine == null) {
                                newLine = new Line();
                                System.Windows.MessageBox.Show("New line wasn't created ", "ERROR!", new System.Windows.MessageBoxButton());
                            }

                            //newLine.Error = words[5];

                            int length = Convert.ToInt32(words[6].Split(':')[0].Trim('L', 'e', 'n', 'g', 't', 'h', ' '));     
                                                                                                                              // Если применить на всю строку, можно случайно обрезать последнее значение вроде 0xEE (0xee)
                            ParsingOldLine(ref newLine, length, words, mBExceptions, commands);
                        }
                    }

                    rwLast = words[3];
                }

            }


            if (outputFormat == ".json") {
                output = CreateJSON(sourceList);
            }
            else if (outputFormat == ".xml") {
                output = CreateXML(sourceList);
            } else {
                output = CreateTXT(sourceList);
            }

            return output;
        }

        private static bool ParsingNewLine(ref Line newLine, int length, string[] words, MBExceptions mBExceptions, Commands commands) {
            if (length > 0) {
                var byteArrayString = words[6].Split(':')[1].Trim();
                newLine.Direction = words[3];


                newLine.RawFrame = new byte[length];
                var byteArrayStringSplit = byteArrayString.Split(' ');
                for (int i = 0; i < length; i++) {
                    newLine.RawFrame[i] = Convert.ToByte(byteArrayStringSplit[i], 16);
                }
                if (newLine.RawFrame.Length >= 4) {
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

                    string keyCommand = (newLine.RawFrame[1] % 128).ToString("X2");
                    string command = "";
                    if (!commands.Command.TryGetValue(keyCommand, out command)) {
                        command = "Unknown";
                    }
                    newLine.Command = keyCommand + ":" + command;

                    if (newLine.RawFrame[1] >= 128) {
                        if (length == 5) {
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
                }
                else {
                    // пакет битый
                    newLine.Error = "Wrong data packet's length (length cannot be less than 4 bytes): " + length + " bytes ";
                }
                return true;
            }
            return false;

        }

        private static bool ParsingOldLine(ref Line newLine, int length, string[] words, MBExceptions mBExceptions, Commands commands) {
            if (length > 0) {
                var byteArrayString = words[6].Split(':')[1].Trim();

                byte[] oldByteArray = newLine.RawFrame;
                int oldLength = oldByteArray.Length;
                int newDataLength = oldLength + length;

                newLine.RawFrame = new byte[newDataLength];

                for (int i = 0; i < oldLength; i++) {
                    newLine.RawFrame[i] = oldByteArray[i];
                }

                var byteArrayStringSplit = byteArrayString.Split(' ');
                for (int i = 0; i < length; i++) {
                    newLine.RawFrame[i+oldLength] = Convert.ToByte(byteArrayStringSplit[i], 16);
                }

                if (newDataLength >= 4) {
                    newLine.RawData = new byte[newDataLength - 4];
                    for (int i = 0; i < newLine.RawFrame.Count() - 4; i++) {
                        newLine.RawData[i] = newLine.RawFrame[i + 2];           // починить вывод в логах с Base64 на побайтовый HEX
                    }

                    // Выводит сначала старший бит CRC, затем младший
                    // в ModBus пакете выводится в обратном порядке
                    string crc = CheckSum.CRC16(newLine.RawFrame, newDataLength - 2).ToString("X4");
                    // Разворот строки (в примере используется такой формат)
                    newLine.CRC = crc.Substring(2, 2) + crc.Substring(0, 2);
                    if (newLine.CRC != (newLine.RawFrame[newDataLength - 2].ToString("X2") + newLine.RawFrame[newDataLength - 1].ToString("X2"))) {
                        newLine.Error = "Wrong CRC";
                    } else {
                        newLine.Error = null;
                    }

                    newLine.Address = newLine.RawFrame[0].ToString("X2");

                    string keyCommand = (newLine.RawFrame[1] % 128).ToString("X2");
                    string command = "";
                    if (!commands.Command.TryGetValue(keyCommand, out command)) {
                        command = "Unknown";
                    }
                    newLine.Command = keyCommand + ":" + command;

                    if (newLine.RawFrame[1] >= 128) {
                        if (newDataLength == 5) {
                            string keyException = newLine.RawFrame[2].ToString("X2");
                            string exception = "";
                            if (!mBExceptions.MBException.TryGetValue(keyException, out exception)) {
                                exception = "Unknown";
                            }
                            newLine.Exception = keyException + ":" + exception;
                        }
                        else {
                            newLine.Error = "Wrong exception packet's length (length cannot be more or less than 5 bytes): " + newDataLength + " bytes";
                        }
                    }
                }
                else {
                    // пакет битый
                    newLine.Error = "Wrong data packet's length (length cannot be less than 4 bytes): " + newDataLength + " bytes ";
                }
                return true;
            }
            return false;

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
                serializer.Serialize(stringwriter, sourceList);
                return stringwriter.ToString();
            }
            catch {
                throw;
            }
        }

        private static string CreateTXT(SourceList sourceList) {
            string output = "";
            string endl = System.Environment.NewLine;
            output += "SourceType: "+ sourceList.sourceType + endl;
            foreach (Source source in sourceList.sources) {
                output += Tabs() + "Source: Address=" + source.Address + " Speed=" + source.Speed + endl;
                foreach (Line line in source.LineList) {
                    output += Tabs(2) + "Line: Direction=" + line.Direction;
                    if (line.Address != null) { output += " Address=" + line.Address; }
                    if (line.Command != null) { output += " Command=\"" + line.Command + "\""; }
                    if (line.Exception != null) { output += " Exception=\"" + line.Exception + "\""; }
                    if (line.Error != null) { output += " Error=\"" + line.Error + "\""; }
                    if (line.CRC != null) { output += " CRC=" + line.CRC; }

                    if (line.RawFrame != null) {
                        output += "\n" + Tabs(3);
                        output += "RawFrame:";
                        foreach (var element in line.RawFrame) {
                            output += " " + element.ToString("X2");
                        }
                    }

                    if (line.RawData != null) {
                        output += "\n" + Tabs(3);
                        output += "RawData:";
                        foreach (var element in line.RawData) {
                            output += " " + element.ToString("X2");
                        }
                    }
                    output += "\n";
                }
                output += "\n";
            }

            return output;
        }

        private static string Tabs(int N = 1) {
            string result = "";
            for (int i = 0; i < N; i++) {
                result += "\t";
            }
            return result;
        }

    }

}


