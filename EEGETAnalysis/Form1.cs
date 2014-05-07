using EEGETAnalysis.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace EEGETAnalysis
{
    public partial class Form1 : Form
    {
        string csvFilePath = null;
        string mediaFilePath = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            startAnalysisButton.Enabled = false;
            windowsMediaPlayer.uiMode = "none";
            windowsMediaPlayer.Ctlcontrols.stop();
        }

        private void enableStartAnalysisButtonIfPathsAreSet()
        {
            if(!String.IsNullOrEmpty(mediaFilePath) && !String.IsNullOrEmpty(csvFilePath))
            {
                startAnalysisButton.Enabled = true;
            }
            else
            {
                startAnalysisButton.Enabled = false;
            }
        }

        private void selectCsvFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                csvFilePath = openFileDialog1.FileName;
                csvFilePathTextBox.Text = csvFilePath;
            }

            enableStartAnalysisButtonIfPathsAreSet();
        }

        private void selectMediaFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "mp4 files (*.mp4)|*.mp4|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                mediaFilePath = openFileDialog1.FileName;
                mediaFilePathTextBox.Text = mediaFilePath;
            }

            enableStartAnalysisButtonIfPathsAreSet();
        }

        private void startAnalysisButton_Click(object sender, EventArgs e)
        {
            windowsMediaPlayer.URL = mediaFilePath;
            windowsMediaPlayer.Ctlcontrols.stop();

            CsvParser parser = new CsvParser(csvFilePath);
            List<List<string>> result = parser.Parse();

            EEGChart.Series.Clear(); //ensure that the chart is empty
            EEGChart.Series.Add("T7");
            EEGChart.Series[0].ChartType = SeriesChartType.Line;
            EEGChart.Legends.Clear();

            List<string> time = null;
            List<string> eegt7 = null;

            foreach (List<string> item in result)
            {
                if (item[0] == "Time")
                {
                    time = item;
                }

                if (item[0] == "EEG_RAW_T7")
                {
                    eegt7 = item;
                }
            }

            //time.RemoveRange(0, 2); // remove head line and first (useless data)
            //eegt7.RemoveRange(0, 2);

            //time.RemoveAt(time.Count-1); // remove last line (useless data)
            //eegt7.RemoveAt(eegt7.Count-1);

            for (int j = 0; j < eegt7.Count; j++)
            {
                EEGChart.Series[0].Points.AddXY(time[j], eegt7[j]);
            }
        }
    }
}
