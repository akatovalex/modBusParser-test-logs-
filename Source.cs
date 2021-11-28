using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modBusParse {
    public class SourceList {
        public string sourceType;
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

        [System.Xml.Serialization.XmlAttribute]
        public string Address {
            get { return address; }
            set {
                address = value;
            }
        }

        [System.Xml.Serialization.XmlAttribute]
        public string Speed {
            get { return speed; }
            set {
                speed = value;
            }
        }


        //При наличии этой строки логи xml больше похоже на xml в примере, т.к. LineList исчезает совсем (субъективно неудобней)
        //[System.Xml.Serialization.XmlElement(ElementName ="Line")]
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
            direction = "Unknown";
        }


        [System.Xml.Serialization.XmlAttribute]
        public string Direction {
            get { return direction; }
            set {
                direction = value;
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

        public byte[] RawFrame {
            get { return rawFrame; }
            set {
                rawFrame = value;
            }
        }

        public byte[] RawData {
            get { return rawData; }
            set {
                rawData = value;
            }
        }





    }
}