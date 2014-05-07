namespace EEGETAnalysis
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.EEGChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.windowsMediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            this.settingsGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.csvFilePathTextBox = new System.Windows.Forms.TextBox();
            this.mediaFilePathTextBox = new System.Windows.Forms.TextBox();
            this.selectCsvFileButton = new System.Windows.Forms.Button();
            this.selectMediaFileButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.startAnalysisButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.EEGChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.windowsMediaPlayer)).BeginInit();
            this.settingsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // EEGChart
            // 
            this.EEGChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea3.Name = "ChartArea1";
            this.EEGChart.ChartAreas.Add(chartArea3);
            this.EEGChart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            legend3.Name = "Legend1";
            this.EEGChart.Legends.Add(legend3);
            this.EEGChart.Location = new System.Drawing.Point(12, 504);
            this.EEGChart.Name = "EEGChart";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.EEGChart.Series.Add(series3);
            this.EEGChart.Size = new System.Drawing.Size(843, 142);
            this.EEGChart.TabIndex = 0;
            this.EEGChart.Text = "EEGChart";
            // 
            // windowsMediaPlayer
            // 
            this.windowsMediaPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.windowsMediaPlayer.Enabled = true;
            this.windowsMediaPlayer.Location = new System.Drawing.Point(12, 100);
            this.windowsMediaPlayer.Name = "windowsMediaPlayer";
            this.windowsMediaPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("windowsMediaPlayer.OcxState")));
            this.windowsMediaPlayer.Size = new System.Drawing.Size(843, 398);
            this.windowsMediaPlayer.TabIndex = 1;
            // 
            // settingsGroupBox
            // 
            this.settingsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsGroupBox.Controls.Add(this.startAnalysisButton);
            this.settingsGroupBox.Controls.Add(this.selectMediaFileButton);
            this.settingsGroupBox.Controls.Add(this.selectCsvFileButton);
            this.settingsGroupBox.Controls.Add(this.mediaFilePathTextBox);
            this.settingsGroupBox.Controls.Add(this.csvFilePathTextBox);
            this.settingsGroupBox.Controls.Add(this.label2);
            this.settingsGroupBox.Controls.Add(this.label1);
            this.settingsGroupBox.Location = new System.Drawing.Point(12, 3);
            this.settingsGroupBox.Name = "settingsGroupBox";
            this.settingsGroupBox.Size = new System.Drawing.Size(843, 91);
            this.settingsGroupBox.TabIndex = 2;
            this.settingsGroupBox.TabStop = false;
            this.settingsGroupBox.Text = "Settings";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "CSV file:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Media file:";
            // 
            // csvFilePathTextBox
            // 
            this.csvFilePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.csvFilePathTextBox.Enabled = false;
            this.csvFilePathTextBox.Location = new System.Drawing.Point(73, 13);
            this.csvFilePathTextBox.Name = "csvFilePathTextBox";
            this.csvFilePathTextBox.Size = new System.Drawing.Size(683, 20);
            this.csvFilePathTextBox.TabIndex = 2;
            // 
            // mediaFilePathTextBox
            // 
            this.mediaFilePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mediaFilePathTextBox.Enabled = false;
            this.mediaFilePathTextBox.Location = new System.Drawing.Point(73, 35);
            this.mediaFilePathTextBox.Name = "mediaFilePathTextBox";
            this.mediaFilePathTextBox.Size = new System.Drawing.Size(683, 20);
            this.mediaFilePathTextBox.TabIndex = 3;
            // 
            // selectCsvFileButton
            // 
            this.selectCsvFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectCsvFileButton.Location = new System.Drawing.Point(762, 11);
            this.selectCsvFileButton.Name = "selectCsvFileButton";
            this.selectCsvFileButton.Size = new System.Drawing.Size(75, 23);
            this.selectCsvFileButton.TabIndex = 4;
            this.selectCsvFileButton.Text = "Select";
            this.selectCsvFileButton.UseVisualStyleBackColor = true;
            this.selectCsvFileButton.Click += new System.EventHandler(this.selectCsvFileButton_Click);
            // 
            // selectMediaFileButton
            // 
            this.selectMediaFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectMediaFileButton.Location = new System.Drawing.Point(762, 35);
            this.selectMediaFileButton.Name = "selectMediaFileButton";
            this.selectMediaFileButton.Size = new System.Drawing.Size(75, 23);
            this.selectMediaFileButton.TabIndex = 5;
            this.selectMediaFileButton.Text = "Select";
            this.selectMediaFileButton.UseVisualStyleBackColor = true;
            this.selectMediaFileButton.Click += new System.EventHandler(this.selectMediaFileButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // startAnalysisButton
            // 
            this.startAnalysisButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.startAnalysisButton.Location = new System.Drawing.Point(9, 61);
            this.startAnalysisButton.Name = "startAnalysisButton";
            this.startAnalysisButton.Size = new System.Drawing.Size(828, 23);
            this.startAnalysisButton.TabIndex = 6;
            this.startAnalysisButton.Text = "OK";
            this.startAnalysisButton.UseVisualStyleBackColor = true;
            this.startAnalysisButton.Click += new System.EventHandler(this.startAnalysisButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 658);
            this.Controls.Add(this.settingsGroupBox);
            this.Controls.Add(this.windowsMediaPlayer);
            this.Controls.Add(this.EEGChart);
            this.Name = "Form1";
            this.Text = "EEGET Analysis";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.EEGChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.windowsMediaPlayer)).EndInit();
            this.settingsGroupBox.ResumeLayout(false);
            this.settingsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart EEGChart;
        private AxWMPLib.AxWindowsMediaPlayer windowsMediaPlayer;
        private System.Windows.Forms.GroupBox settingsGroupBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button selectMediaFileButton;
        private System.Windows.Forms.Button selectCsvFileButton;
        private System.Windows.Forms.TextBox mediaFilePathTextBox;
        private System.Windows.Forms.TextBox csvFilePathTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button startAnalysisButton;
    }
}

