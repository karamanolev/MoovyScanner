using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MoovyScanner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Logger.Setup("Error.log");

            try
            {
                ExtractorConfig config;
                bool showUI;

                if (File.Exists(ExtractorConfig.DefualtConfigPath))
                {
                    config = new ExtractorConfig(ExtractorConfig.DefualtConfigPath);
                    showUI = !args.Contains("-q");
                }
                else
                {
                    config = new ExtractorConfig();
                    showUI = true;
                }

                if (showUI)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    MainWindow window = new MainWindow(config);
                    DialogResult result = window.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    config.Save(ExtractorConfig.DefualtConfigPath);
                }

                Processor processor = new Processor(config);
                processor.Process();

                Console.WriteLine("Complete.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            finally
            {
                Logger.Teardown();
            }
        }
    }
}
