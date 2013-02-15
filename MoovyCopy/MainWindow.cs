using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MoovyCopy
{
    public partial class MainWindow : Form
    {
        private long totalBytes = 0;

        public MainWindow()
        {
            InitializeComponent();

            this.UpdateTotalSize();
        }

        private void UpdateTotalSize()
        {
            this.labelTotalSize.Text = "Total size: " + Utility.BytesToString(totalBytes);
        }

        private void btnBrowseTarget_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                this.textTarget.Text = folderBrowser.SelectedPath;
            }
        }

        private void listInputFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void listInputFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    this.AddSourceFile(file);
                }
            }
        }

        private void AddSourceFile(string item)
        {
            if (!this.listInputFiles.Items.Contains(item))
            {
                this.listInputFiles.Items.Add(item);

                foreach (string line in File.ReadAllLines(item))
                {
                    if (File.Exists(line))
                    {
                        this.AddInputFile(line);
                    }
                    else if (Directory.Exists(line))
                    {
                        foreach (string innerFile in Directory.GetFiles(line, "*", SearchOption.AllDirectories))
                        {
                            this.AddInputFile(innerFile);
                        }
                    }
                }
            }

            this.UpdateTotalSize();
        }

        private void AddInputFile(string file)
        {
            this.listFiles.Items.Add(file);
            FileInfo fileInfo = new FileInfo(file);
            this.totalBytes += fileInfo.Length;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.listInputFiles.Items.Clear();
            this.listFiles.Items.Clear();
            this.totalBytes = 0;
            this.textTarget.Text = "";
            this.UpdateTotalSize();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.textTarget.Text) && Directory.Exists(this.textTarget.Text))
            {
                MessageBox.Show("Please select a valid target directory.");
                return;
            }
            if (this.listFiles.Items.Count == 0)
            {
                MessageBox.Show("Please select files to copy.");
                return;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
