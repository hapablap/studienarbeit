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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.EEGChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.windowsMediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            this.settingsGroupBox = new System.Windows.Forms.GroupBox();
            this.resetButton = new System.Windows.Forms.Button();
            this.startAnalysisButton = new System.Windows.Forms.Button();
            this.selectMediaFileButton = new System.Windows.Forms.Button();
            this.selectCsvFileButton = new System.Windows.Forms.Button();
            this.mediaFilePathTextBox = new System.Windows.Forms.TextBox();
            this.csvFilePathTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.rewindButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.pauseButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.videoPlayTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.EEGChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.windowsMediaPlayer)).BeginInit();
            this.settingsGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // EEGChart
            // 
            this.EEGChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea5.Name = "ChartArea1";
            this.EEGChart.ChartAreas.Add(chartArea5);
            this.EEGChart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            legend5.Name = "Legend1";
            this.EEGChart.Legends.Add(legend5);
            this.EEGChart.Location = new System.Drawing.Point(12, 547);
            this.EEGChart.Name = "EEGChart";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Legend = "Legend1";
            series5.Name = "Series1";
            this.EEGChart.Series.Add(series5);
            this.EEGChart.Size = new System.Drawing.Size(843, 81);
            this.EEGChart.TabIndex = 0;
            this.EEGChart.Text = "EEGChart";
            // 
            // windowsMediaPlayer
            // 
            this.windowsMediaPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.windowsMediaPlayer.Enabled = true;
            this.windowsMediaPlayer.Location = new System.Drawing.Point(12, 130);
            this.windowsMediaPlayer.Name = "windowsMediaPlayer";
            this.windowsMediaPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("windowsMediaPlayer.OcxState")));
            this.windowsMediaPlayer.Size = new System.Drawing.Size(843, 411);
            this.windowsMediaPlayer.TabIndex = 1;
            this.windowsMediaPlayer.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(this.windowsMediaPlayer_PlayStateChange);
            // 
            // settingsGroupBox
            // 
            this.settingsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsGroupBox.Controls.Add(this.resetButton);
            this.settingsGroupBox.Controls.Add(this.startAnalysisButton);
            this.settingsGroupBox.Controls.Add(this.selectMediaFileButton);
            this.settingsGroupBox.Controls.Add(this.selectCsvFileButton);
            this.settingsGroupBox.Controls.Add(this.mediaFilePathTextBox);
            this.settingsGroupBox.Controls.Add(this.csvFilePathTextBox);
            this.settingsGroupBox.Controls.Add(this.label2);
            this.settingsGroupBox.Controls.Add(this.label1);
            this.settingsGroupBox.Location = new System.Drawing.Point(12, 3);
            this.settingsGroupBox.Name = "settingsGroupBox";
            this.settingsGroupBox.Size = new System.Drawing.Size(843, 121);
            this.settingsGroupBox.TabIndex = 2;
            this.settingsGroupBox.TabStop = false;
            this.settingsGroupBox.Text = "Settings";
            // 
            // resetButton
            // 
            this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resetButton.Location = new System.Drawing.Point(9, 90);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(828, 23);
            this.resetButton.TabIndex = 7;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Media file:";
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
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.trackBar);
            this.groupBox1.Controls.Add(this.rewindButton);
            this.groupBox1.Controls.Add(this.stopButton);
            this.groupBox1.Controls.Add(this.pauseButton);
            this.groupBox1.Controls.Add(this.playButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 634);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(843, 65);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // trackBar
            // 
            this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar.Enabled = false;
            this.trackBar.Location = new System.Drawing.Point(333, 14);
            this.trackBar.Maximum = 100;
            this.trackBar.Name = "trackBar";
            this.trackBar.Size = new System.Drawing.Size(504, 45);
            this.trackBar.TabIndex = 4;
            this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
            // 
            // rewindButton
            // 
            this.rewindButton.Enabled = false;
            this.rewindButton.Location = new System.Drawing.Point(252, 16);
            this.rewindButton.Name = "rewindButton";
            this.rewindButton.Size = new System.Drawing.Size(75, 39);
            this.rewindButton.TabIndex = 3;
            this.rewindButton.Text = "Rewind";
            this.rewindButton.UseVisualStyleBackColor = true;
            this.rewindButton.Click += new System.EventHandler(this.rewindButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(171, 16);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 39);
            this.stopButton.TabIndex = 2;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // pauseButton
            // 
            this.pauseButton.Enabled = false;
            this.pauseButton.Location = new System.Drawing.Point(90, 16);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(75, 39);
            this.pauseButton.TabIndex = 1;
            this.pauseButton.Text = "Pause";
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
            // 
            // playButton
            // 
            this.playButton.Enabled = false;
            this.playButton.Location = new System.Drawing.Point(9, 16);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(75, 39);
            this.playButton.TabIndex = 0;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // videoPlayTimer
            // 
            this.videoPlayTimer.Interval = 16;
            this.videoPlayTimer.Tick += new System.EventHandler(this.videoPlayTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 701);
            this.Controls.Add(this.groupBox1);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button rewindButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.TrackBar trackBar;
        private System.Windows.Forms.Timer videoPlayTimer;
    }
}

