using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modBusParse {
    public class SourceList {

        [Newtonsoft.Json.JsonProperty(PropertyName = "SourceType")]
        public string sourceType;

        [Newtonsoft.Json.JsonProperty(PropertyName = "Sources")]
        public List<Source> sources;
        public SourceList() {
            sourceType = "COM";     //в логах отсутствует (подразумевается из address = serial...)
            sources = new List<Source>();
        }
    }

    public class Source {
        private string address;
        private string speed;
        List<Line> lineList;

        public Source() {
            lineList = new List<Line>();
            Speed = "Unknown";  //в логах отсутствует
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "address")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "address")]
        public string Address {
            get { return address; }
            set {
                address = value;
            }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "speed")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "speed")]
        public string Speed {
            get { return speed; }
            set {
                speed = value;
            }
        }


        //При включении этой строки логи xml больше похоже на исходный пример xml, т.к. 
        [System.Xml.Serialization.XmlElement(ElementName ="Line")]         //  LineList исчезает совсем

        [Newtonsoft.Json.JsonProperty(PropertyName = "Line")]
        public List<Line> LineList {
            get { return lineList; }
            set { lineList = value; }


        }
    }


    public class Line {


        private string direction;
        private string address;
        private string command;
        private string exception;
        private string error;
        private string crc;

        private byte[] rawFrame;
        private byte[] rawData;

        public Line() {
        }


        [System.Xml.Serialization.XmlAttribute]
        public string Direction {
            get { return direction; }
            set {
                if (value == "IRP_MJ_WRITE") { direction = "Request"; }
                else if (value == "IRP_MJ_READ") { direction = "Response"; }
                else {
                    direction = "Unknown";
                }
            }
        }

        [System.Xml.Serialization.XmlAttribute]
        public string Address {
            get { return address; }
            set {
                address = value;
            }
        }


        [System.Xml.Serialization.XmlAttribute]
        public string Command {
            get { return command; }
            set {
                command = value;
            }
        }

        [System.Xml.Serialization.XmlAttribute]
        public string Exception {
            get { return exception; }
            set {
                exception = value;
            }
        }
        [System.Xml.Serialization.XmlAttribute]
        public string Error {
            get { return error; }
            set {
                if (value == "TIMEOUT") {
                    error = "Timeout";
                    direction = "Response";
                }
                else if (value != "SUCCESS") {
                    error = value;
                } else {
                    error = null;
                }
            }
        }

        [System.Xml.Serialization.XmlAttribute]
        public string CRC {
            get { return crc; }
            set {
                crc = value;
            }
        }


        [System.Xml.Serialization.XmlElement(ElementName = "raw_frame", DataType = "hexBinary")]
        [Newtonsoft.Json.JsonConverter(typeof(ByteArrayConverter))]
        //[Newtonsoft.Json.JsonIgnore]
        public byte[] RawFrame {
            get { return rawFrame; }
            set {
                rawFrame = value;
            }
        }


        [System.Xml.Serialization.XmlElement(ElementName = "raw_data", DataType = "hexBinary")]
        [Newtonsoft.Json.JsonConverter(typeof(ByteArrayConverter))]
        public byte[] RawData {
            get { return rawData; }
            set {
                rawData = value;
            }
        }


    }
}