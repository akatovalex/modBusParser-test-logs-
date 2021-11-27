using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modBusParse {
     class MBParse {
        public MBParse() {

        }

        public static string Parse(string input) {
            Source source = new Source();
            List<string> inputString = new List<string>();
            inputString.AddRange(input.Split('\n'));

            string output = "";
            foreach (var line in inputString) {
                string Delimiter = "\t";
                //int[] skippedInfoIndexes = {0,1,2,3,7 };
                //foreach (var word in line.Split(new[] { Delimiter }, StringSplitOptions.None).Select((value, i) => new { i, value })) {
                var words = line.Split(new[] { Delimiter }, StringSplitOptions.None);
                //for (int i = 0; i < words.Count(); i++) { 

                Line newLine = new Line();
                foreach (var word in words) {
                    //output += word + "\n";
                }
                if (words.Length > 4) {
                    source.Address = words[4];
                    newLine.Error = words[5];
                    output += words[4] + ' ' + words[5] + ' ' + words[6] + "\n";
                    source.LineList.Add(newLine);
                }
                //}
            }

            return output;
        }


    }
}
