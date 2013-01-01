using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;

namespace MoovyScanner
{
    public class OutputWriter : IDisposable
    {
        private Random random;
        private string outputPath;
        private string username, password;
        private StringBuilder builder;

        public OutputWriter(string outputPath, string username, string password)
        {
            this.random = new Random();
            this.builder = new StringBuilder();
            this.outputPath = outputPath;
            this.username = username;
            this.password = password;
        }

        public void WriteLine(string line)
        {
            this.builder.AppendLine(line);
        }

        private string GetContent(string content, string boundary)
        {
            StringBuilder requestString = new StringBuilder();

            requestString.AppendLine("--" + boundary);
            requestString.AppendLine("Content-Disposition: file; name=\"csv_file\"; filename=\"MoovyScannerResult.csv\"");
            requestString.AppendLine("Content-Type: text/plain");
            requestString.AppendLine();
            requestString.AppendLine(content);

            requestString.AppendLine("--" + boundary);
            requestString.AppendLine("Content-Disposition: form-data; name=\"eID\"");
            requestString.AppendLine();
            requestString.AppendLine("moovyremotecsv");

            requestString.AppendLine("--" + boundary);
            requestString.AppendLine("Content-Disposition: form-data; name=\"username\"");
            requestString.AppendLine();
            requestString.AppendLine(this.username);

            requestString.AppendLine("--" + boundary);
            requestString.AppendLine("Content-Disposition: form-data; name=\"password\"");
            requestString.AppendLine();
            requestString.AppendLine(this.password);

            requestString.Append("--" + boundary + "--");

            return requestString.ToString();
        }

        public void Dispose()
        {
            string content = builder.ToString();

            string boundary = "--" + this.random.Next(1000000000, 2000000000);
            string requestString = this.GetContent(content, boundary);
            byte[] bytes = Encoding.UTF8.GetBytes(requestString.ToString());

            File.WriteAllText("MoovyScannerResult.csv", content, Encoding.UTF8);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(this.outputPath);
            request.ServicePoint.Expect100Continue = false;
            request.KeepAlive = false;
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responseString = reader.ReadToEnd();
            }
        }
    }
}
