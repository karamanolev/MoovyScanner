using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoovyCopy
{
    public partial class MainWindow : Form
    {
        private long totalBytes = 0;
        private Task task = null;
        private byte[] buffer = new byte[4 * 1024 * 1024];
        private char[] directorySeparator = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        private DateTime copyStartDate;

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

        public void AddSourceFile(string item)
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
            if (string.IsNullOrWhiteSpace(this.textTarget.Text) || !Directory.Exists(this.textTarget.Text))
            {
                MessageBox.Show("Please select a valid target directory.");
                return;
            }
            if (this.listFiles.Items.Count == 0)
            {
                MessageBox.Show("Please select files to copy.");
                return;
            }

            this.btnClear.Enabled = false;
            this.btnStart.Enabled = false;
            this.btnClose.Enabled = false;

            string targetDir = this.textTarget.Text;
            string[] files = this.listFiles.Items.Cast<string>().ToArray();

            this.task = new Task(() =>
            {
                try
                {
                    this.copyStartDate = DateTime.Now;

                    long copied = 0;

                    foreach (string file in files)
                    {
                        this.CopyFile(targetDir, file, ref copied);
                    }

                    this.OnComplete();
                }
                catch (Exception ex)
                {
                    this.ReportError(ex.Message);
                }
            });
            this.task.Start();
        }

        private void OnComplete()
        {
            this.Invoke(new Action(() =>
            {
                TimeSpan time = DateTime.Now.Subtract(this.copyStartDate);

                this.btnClose.Enabled = true;
                MessageBox.Show("Copy complete in " + time + ".");
                this.task = null;
                this.Close();
            }));
        }

        private void ReportError(string errorMessage)
        {
            this.Invoke(new Action(() =>
            {
                MessageBox.Show("Error while copying: " + errorMessage);
                this.task = null;
                this.Close();
            }));
        }

        private void CopyFile(string targetDir, string file, ref long copied)
        {
            string targetFile = this.GetOutputPath(targetDir, file);
            Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
            using (Stream inputStream = File.OpenRead(file))
            using (Stream outputStream = File.OpenWrite(targetFile))
            {
                int read;
                while ((read = inputStream.Read(this.buffer, 0, this.buffer.Length)) != 0)
                {
                    outputStream.Write(this.buffer, 0, read);
                    copied += read;

                    this.UpdateProgressBar(copied);
                }
            }
        }

        private void UpdateProgressBar(long copied)
        {
            this.BeginInvoke(new Action(() =>
            {
                this.progressBar1.Minimum = 0;
                this.progressBar1.Maximum = 10000;
                this.progressBar1.Value = (int)(copied * 10000 / this.totalBytes);
            }));
        }

        private string GetOutputPath(string targetDir, string sourceFile)
        {
            string targetFile = targetDir;
            foreach (string _part in sourceFile.Split(this.directorySeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                string part = Utility.FixFilename(_part);
                if (!string.IsNullOrWhiteSpace(part))
                {
                    targetFile = Path.Combine(targetFile, part);
                }
            }
            return targetFile;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.task != null && !this.task.IsCompleted)
            {
                if (MessageBox.Show("Are you sure you want to cancel the copy?", "MoovyCopy", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
