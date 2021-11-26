using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace modBusParse {
    class Commands {

        public Commands(string commandsFilePath) {
            Dictionary<string, string> b = new Dictionary<string, string>();

            string[] fs = File.ReadAllLines(commandsFilePath);
            foreach(string inputLine in fs) {
                var formatInput = inputLine.Trim().Split('-');
                b.Add(formatInput[0].Trim(), formatInput[1].Trim());
            }
        }
    }
}
