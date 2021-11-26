using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modBusParse {
    public class Source {
        private string address;
        private string speed;
        List<Line> line;

        public Source() {
            Speed = "Unknown";
            line = new List<Line>();
        }

        public string Address {
            get { return address;  }
            set {
                address = value;
            }
        }

        public string Speed {
            get { return speed; }
            set {
                speed = value;
            }
        }


    }

    public class Line {
        private byte[] rawFrame;
        private byte[] rawData;

        private string direction;
        private string address;
        private string command;
        private string crc;
        private string error;
        private string exception;

        public Line() {

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

        public string Direction {
            get { return direction; }
            set {
                direction = value;
            }
        }
        public string Address {
            get { return address; }
            set {
                address = value;
            }
        }
        public string Command {
            get { return command; }
            set {
                command = value;
            }
        }
        public string CRC {
            get { return crc; }
            set {
                crc = value;
            }
        }
        public string Error {
            get { return error; }
            set {
                error = value;
            }
        }
        public string Exception {
            get { return exception; }
            set {
                exception = value;
            }
        }


    }
}
