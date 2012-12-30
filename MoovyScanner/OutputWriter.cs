using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;

namespace MoovyScanner
{
    public class OutputWriter : IDisposable
    {
        private string outputPath;
        private StringBuilder builder;

        public OutputWriter(string outputPath)
        {
            this.builder = new StringBuilder();
            this.outputPath = outputPath;
        }

        public void WriteLine(string line)
        {
            this.builder.AppendLine(line);
        }

        public void Dispose()
        {
            string content = builder.ToString();

            if (outputPath.StartsWith("http://"))
            {
                WebClient client = new WebClient();
                client.UploadData(new Uri(this.outputPath), "POST", Encoding.UTF8.GetBytes(content));
            }
            else
            {
                File.WriteAllText(this.outputPath, content, Encoding.UTF8);
            }
        }
    }
}
