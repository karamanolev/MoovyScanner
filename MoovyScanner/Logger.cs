using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MoovyScanner
{
    class Logger
    {
        private static StreamWriter writer;

        public static void Setup(string logPath)
        {
            writer = new StreamWriter(logPath, false, Encoding.UTF8);
        }

        public static void Teardown()
        {
            writer.Close();
        }

        public static void Log(string message)
        {
            writer.WriteLine(message);
        }
    }
}
