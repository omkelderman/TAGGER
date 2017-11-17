using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TAGGER.Utils
{
    class Logger
    {
        public static async Task Exception(string message)
        {
            StreamWriter log;

            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            log = new StreamWriter("Logs/Exceptions.txt");
            await log.WriteLineAsync(DateTime.Now.ToString() + message);
        }

        public static async Task Warning(string message)
        {
            StreamWriter log;

            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            log = new StreamWriter("Logs/Warnings.txt");
            await log.WriteLineAsync(DateTime.Now.ToString() + message);
        }

        public static async Task Verbose(string message)
        {
            StreamWriter log;

            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            log = new StreamWriter("Logs/Verbose.txt");
            await log.WriteLineAsync(DateTime.Now.ToString() + message);
        }
    }
}
