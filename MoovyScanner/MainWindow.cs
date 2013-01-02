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
            this.textRegexes.Text = string.Join(Environment.NewLine, this.Config.Regexes);
            this.textUsername.Text = this.Config.Username;
            this.textPassword.Text = this.Config.Password;
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
            this.Config.Regexes = new List<string>(this.textRegexes.Text.Split(newLineSplit, StringSplitOptions.RemoveEmptyEntries));
            this.Config.Output = this.textOutput.Text;
            this.Config.Username = this.textUsername.Text;
            this.Config.Password = this.textPassword.Text;

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

        private void btnAddInput_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                this.textInputs.Text += folderBrowser.SelectedPath + Environment.NewLine;
            }
        }
    }
}
