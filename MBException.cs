using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace modBusParse {
    class MBExceptions {
        private Dictionary<string, string> mbException;

        public MBExceptions(string exceptionsFilePath) {
            LoadCommands(exceptionsFilePath);
        }

        private void LoadCommands(string exceptionsFilePath) {
            mbException = new Dictionary<string, string>();

            string[] fs = File.ReadAllLines(exceptionsFilePath);
            foreach (string inputLine in fs) {
                var formatInput = inputLine.Trim().Split('-');
                mbException.Add(formatInput[0].Trim(), formatInput[1].Trim());
            }
        }
        public Dictionary<string, string> MBException {
            get {
                return mbException;
            }
        }
    }
}
