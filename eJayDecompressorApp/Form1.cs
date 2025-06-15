using System;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eJayDecompressorApp
{
    public partial class Form1 : Form
    {
        private List<string> filesToConvert = new List<string>();
        private Label lblDropZone;
        private Label lblStatus;
        private Button btnConvert;
        private ProgressBar progressBar;
        private TextBox txtLog;
        private Button btnClearLog;
        private Decompressor decompressor;
        private CheckBox chkSameDirectory;
        private Button btnChooseDirectory;
        private CheckBox chkDeletePxd;
        private Label lblExtractTo;
        private ComboBox cmbOutputDirectory;
        private ComboBox cmbFileExists;
        private Label lblIfFileExists;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
            this.AllowDrop = true;
            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;
            decompressor = new Decompressor();
        }

        private void InitializeCustomComponents()
        {
            // Configure the form
            this.Text = "eJay PXD Batch Converter";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create TableLayoutPanel for robust layout
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 7,
                ColumnCount = 1,
                Padding = new Padding(10),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Drop zone
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Log
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));  // Clear log
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));  // Progress bar
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));  // Status label
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));  // Options
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));  // Convert button (increased height for padding)

            // Create drop zone label
            lblDropZone = new Label
            {
                Text = "Drag and drop one or more .pxd files here to batch convert to .wav in the same folder",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular),
                BackColor = System.Drawing.Color.WhiteSmoke
            };
            lblDropZone.AllowDrop = true;
            lblDropZone.DragEnter += Form1_DragEnter;
            lblDropZone.DragDrop += Form1_DragDrop;

            // Create log textbox
            txtLog = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Consolas", 9F),
                ReadOnly = true
            };

            // Create clear log button
            btnClearLog = new Button
            {
                Text = "Clear Log",
                Dock = DockStyle.Fill,
                Height = 30
            };
            btnClearLog.Click += (s, e) => txtLog.Clear();

            // Create progress bar
            progressBar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Height = 20,
                Visible = false
            };

            // Create status label
            lblStatus = new Label
            {
                Text = "",
                Dock = DockStyle.Fill,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            // Create options panel
            var optionsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Height = 30,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0)
            };

            // Create extract to label and combo
            lblExtractTo = new Label
            {
                Text = "Extract To:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(10, 7, 0, 0)
            };

            cmbOutputDirectory = new ComboBox
            {
                Width = 150,
                Height = 30,
                Items = { "Same Directory as PXD", "User Directory" },
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbOutputDirectory.SelectedIndex = 0;
            cmbOutputDirectory.SelectedIndexChanged += (s, e) => btnChooseDirectory.Visible = cmbOutputDirectory.SelectedIndex == 1;

            btnChooseDirectory = new Button
            {
                Text = "Choose Directory",
                Width = 120,
                Height = 30,
                Visible = false
            };
            btnChooseDirectory.Click += (s, e) =>
            {
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Store the selected directory path for later use
                    }
                }
            };

            // Create if file exists label and combo
            lblIfFileExists = new Label
            {
                Text = "If File Exists:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(10, 7, 0, 0)
            };

            cmbFileExists = new ComboBox
            {
                Width = 100,
                Height = 30,
                Items = { "Overwrite", "Rename" },
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFileExists.SelectedIndex = 0;

            // Create delete PXD option
            chkDeletePxd = new CheckBox
            {
                Text = "Delete PXD file after conversion",
                AutoSize = true,
                Height = 30,
                Padding = new Padding(10, 7, 0, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Add controls to options panel
            optionsPanel.Controls.Add(lblExtractTo);
            optionsPanel.Controls.Add(cmbOutputDirectory);
            optionsPanel.Controls.Add(btnChooseDirectory);
            optionsPanel.Controls.Add(lblIfFileExists);
            optionsPanel.Controls.Add(cmbFileExists);
            optionsPanel.Controls.Add(chkDeletePxd);

            // Create convert button
            btnConvert = new Button
            {
                Text = "Convert Files",
                Dock = DockStyle.Fill,
                Height = 40,
                Enabled = false,
                BackColor = System.Drawing.Color.LightSkyBlue,
                Margin = new Padding(0, 10, 0, 0)
            };
            btnConvert.Click += BtnConvert_Click;

            // Add controls to table
            table.Controls.Add(lblDropZone, 0, 0);
            table.Controls.Add(txtLog, 0, 1);
            table.Controls.Add(btnClearLog, 0, 2);
            table.Controls.Add(progressBar, 0, 3);
            table.Controls.Add(lblStatus, 0, 4);
            table.Controls.Add(optionsPanel, 0, 5);
            table.Controls.Add(btnConvert, 0, 6);

            // Add table to form
            this.Controls.Add(table);
        }

        private void LogMessage(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => LogMessage(message)));
                return;
            }

            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            txtLog.ScrollToCaret();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files)
            {
                filesToConvert.Clear();
                foreach (string file in files)
                {
                    if (Path.GetExtension(file).Equals(".pxd", StringComparison.OrdinalIgnoreCase))
                    {
                        filesToConvert.Add(file);
                        LogMessage($"Added file for conversion: {file}");
                    }
                }

                if (filesToConvert.Count > 0)
                {
                    lblStatus.Text = $"{filesToConvert.Count} .pxd files ready to convert";
                    btnConvert.Enabled = true;
                    LogMessage($"Ready to convert {filesToConvert.Count} files");
                }
                else
                {
                    lblStatus.Text = "No .pxd files found. Please drag and drop .pxd files.";
                    btnConvert.Enabled = false;
                    LogMessage("No .pxd files found in dropped files");
                }
            }
        }

        private async void BtnConvert_Click(object sender, EventArgs e)
        {
            if (filesToConvert.Count == 0) return;

            btnConvert.Enabled = false;
            progressBar.Visible = true;
            progressBar.Maximum = filesToConvert.Count;
            progressBar.Value = 0;

            try
            {
                foreach (string file in filesToConvert)
                {
                    string outputPath;
                    if (cmbOutputDirectory.SelectedIndex == 0)
                    {
                        // Same directory as PXD
                        outputPath = Path.ChangeExtension(file, ".wav");
                    }
                    else
                    {
                        // User specified directory
                        using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                        {
                            if (folderDialog.ShowDialog() == DialogResult.OK)
                            {
                                outputPath = Path.Combine(folderDialog.SelectedPath, Path.GetFileNameWithoutExtension(file) + ".wav");
                            }
                            else
                            {
                                continue; // Skip if user cancels
                            }
                        }
                    }

                    lblStatus.Text = $"Converting: {Path.GetFileName(file)}";
                    LogMessage($"Starting conversion of {file} to {outputPath}");
                    
                    await Task.Run(() => ConvertFile(file, outputPath));
                    
                    if (chkDeletePxd.Checked)
                    {
                        File.Delete(file);
                        LogMessage($"Deleted PXD file: {file}");
                    }

                    progressBar.Value++;
                    Application.DoEvents();
                }

                lblStatus.Text = $"Successfully converted {filesToConvert.Count} files";
                LogMessage($"Batch conversion completed successfully");
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error during conversion";
                LogMessage($"Error: {ex.Message}");
            }
            finally
            {
                btnConvert.Enabled = true;
                progressBar.Visible = false;
            }
        }

        private void ConvertFile(string inputPath, string outputPath)
        {
            try
            {
                decompressor.DecompressSinglePxd(inputPath, outputPath);
                LogMessage($"Conversion completed: {outputPath}");
            }
            catch (Exception ex)
            {
                LogMessage($"Error converting {inputPath}: {ex.Message}");
                throw;
            }
        }
    }
}
