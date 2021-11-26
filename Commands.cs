using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace modBusParse {
    class Commands {

        private Dictionary <string, string> command;

        public Commands(string commandsFilePath) {
            LoadCommands(commandsFilePath);
        }

        private void LoadCommands(string commandsFilePath) {
            command = new Dictionary<string, string>();

            string[] fs = File.ReadAllLines(commandsFilePath);
            foreach (string inputLine in fs) {
                var formatInput = inputLine.Trim().Split('-');
                command.Add(formatInput[0].Trim(), formatInput[1].Trim());
            }
        }
        public Dictionary<string, string> Command {
            get {
                return command;
            }
        }
    }
}
