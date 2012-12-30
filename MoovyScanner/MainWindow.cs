using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MoovyScanner
{
    public partial class MainWindow : Form
    {
        private static string[] newLineSplit = new string[] { Environment.NewLine };

        public ExtractorConfig Config { get; private set; }

        public MainWindow(ExtractorConfig config)
        {
            this.Config = config;

            InitializeComponent();

            this.textInputs.Text = string.Join(Environment.NewLine, this.Config.InputPaths);
            this.textExtensions.Text = string.Join(Environment.NewLine, this.Config.Extensions);
            this.textOutput.Text = this.Config.Output;
            this.textRegex.Text = this.Config.Regex;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Config.InputPaths = new List<string>(this.textInputs.Text.Split(newLineSplit, StringSplitOptions.RemoveEmptyEntries));
            this.Config.Extensions = new List<string>(this.textExtensions.Text.Split(newLineSplit, StringSplitOptions.RemoveEmptyEntries).Select(ext => this.FixExtension(ext)));
            this.Config.Regex = this.textRegex.Text;
            this.Config.Output = this.textOutput.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private string FixExtension(string e)
        {
            e = e.Trim();
            if (!e.StartsWith("."))
            {
                e = "." + e;
            }
            return e;
        }
    }
}
