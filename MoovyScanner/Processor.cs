using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace MoovyScanner
{
    class Processor
    {
        private ExtractorConfig config;
        private Regex regex;

        public Processor(ExtractorConfig config)
        {
            this.config = config;
            this.regex = new Regex(config.Regex, RegexOptions.None);
        }

        public void Process()
        {
            using (OutputWriter writer = new OutputWriter(this.config.Output, this.config.Username, this.config.Password))
            {
                foreach (string inputFile in this.EnumerateInputFiles())
                {
                    try
                    {
                        string imdbId = "";
                        if (this.ShouldScan(inputFile))
                        {
                            imdbId = this.EnumerateImdbIds(inputFile).FirstOrDefault() ?? "";
                        }

                        long fileSize = new FileInfo(inputFile).Length;
                        string ext = Path.GetExtension(inputFile).Trim('.').ToLower();

                        string line = "\"" + fileSize + "\";\"" + ext + "\";\"" + inputFile + "\";\"" + imdbId + "\"";
                        writer.WriteLine(line);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("File skipped (" + inputFile + "): " + ex);
                    }
                }
                Console.WriteLine("Uploading...");
            }
        }

        private IEnumerable<string> EnumerateImdbIds(string inputFile)
        {
            string contents = File.ReadAllText(inputFile, Encoding.UTF8);
            Match match = this.regex.Match(contents);
            Group group = match.Groups["imdb_id"];
            foreach (Capture capture in group.Captures)
            {
                yield return capture.Value;
            }
        }

        private bool ShouldScan(string fileName)
        {
            foreach (string extension in this.config.Extensions)
            {
                if (fileName.ToLower().EndsWith(extension.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerable<string> EnumerateInputFiles()
        {
            foreach (string inputPath in this.config.InputPaths)
            {
                Console.WriteLine("Scanning directory " + inputPath);
                Console.WriteLine("Listing files...");
                string[] files = Directory.GetFiles(inputPath, "*", SearchOption.AllDirectories);

                int fileNumber = 1;
                DateTime lastLog = DateTime.MinValue;
                foreach (string file in files)
                {
                    if (DateTime.Now.Subtract(lastLog).TotalSeconds > 1)
                    {
                        lastLog = DateTime.Now;
                        Console.WriteLine("Scanning file " + fileNumber + " / " + files.Length);
                    }
                    yield return file;
                    ++fileNumber;
                }
            }
        }
    }
}
