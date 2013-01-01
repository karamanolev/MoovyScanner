using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MoovyScanner
{
    public class ExtractorConfig
    {
        public static readonly string DefualtConfigPath = "Config.txt";

        private static readonly List<string> defaultInputPaths = new List<string>()
        {
            "nfo","xml","desktop","url","txt", "webloc"
        };
        private static readonly string defaultRegex = @"imdb\.(.*)\/title\/tt(?<imdb_id>[0-9]*)";
        private static readonly string defaultOutput = "http://moovy.at/";

        public List<string> InputPaths { get; set; }
        public List<string> Extensions { get; set; }
        public string Output { get; set; }
        public string Regex { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public ExtractorConfig()
        {
            this.InputPaths = new List<string>();
            this.Extensions = new List<string>(defaultInputPaths);
            this.Output = defaultOutput;
            this.Regex = defaultRegex;
            this.Username = "";
            this.Password = "";
        }

        public ExtractorConfig(string filename)
        {
            this.InputPaths = new List<string>();
            this.Extensions = new List<string>();
            this.Output = "";
            this.Regex = defaultRegex;
            this.Username = "";
            this.Password = "";

            string[] lines = File.ReadAllLines(filename, Encoding.UTF8);
            foreach (string line in lines)
            {
                string[] parts = line.Split(new char[] { '=' }, 2, StringSplitOptions.None);
                string key = parts[0].Trim();
                string value = parts[1].Trim();
                switch (key)
                {
                    case "Input":
                        this.InputPaths.Add(value);
                        break;
                    case "Extension":
                        this.Extensions.Add(value);
                        break;
                    case "Output":
                        this.Output = value;
                        break;
                    case "Regex":
                        this.Regex = value;
                        break;
                    case "Username":
                        this.Username = value;
                        break;
                    case "Password":
                        this.Password = value;
                        break;
                    default:
                        Logger.Log("Error: unknown settings key: " + key);
                        break;
                }
            }
        }

        public void Save(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
            {
                foreach (string inputPath in this.InputPaths)
                {
                    writer.WriteLine("Input=" + inputPath);
                }
                foreach (string extension in this.Extensions)
                {
                    writer.WriteLine("Extension=" + extension);
                }
                writer.WriteLine("Output=" + this.Output);
                writer.WriteLine("Regex=" + this.Regex);
                writer.WriteLine("Username=" + this.Username);
                writer.WriteLine("Password=" + this.Password);
            }
        }
    }
}
