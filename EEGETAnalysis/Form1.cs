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

// Furiour Library http://www.phon.ucl.ac.uk/home/mark/basicdsp/

namespace EEGETAnalysis
{
    public partial class Form1 : Form
    {
        string csvFilePath = null;
        string mediaFilePath = null;

        // Duration of current media
        double mediaDuration = 0;

        // BeGaze CSV data
        List<List<string>> csvData = null;

        // timestamps
        List<string> time = null;

        // EEG data from T7
        List<string> eegt7 = null;

        // eye tracking coordinates
        List<string> LPORX = null;
        List<string> LPORY = null;

        public Double Duration(String file)
        {
            WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayerClass();
            WMPLib.IWMPMedia mediainfo = wmp.newMedia(file);
            return mediainfo.duration;
        }

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

            mediaDuration = Duration(mediaFilePath);

            try
            {
                CsvParser parser = new CsvParser(csvFilePath);
                csvData = parser.Parse();

                EEGChart.Series.Clear(); //ensure that the chart is empty
                EEGChart.Series.Add("T7");
                EEGChart.Series[0].ChartType = SeriesChartType.Line;
                EEGChart.Legends.Clear();

                foreach (List<string> item in csvData)
                {
                    if (item[0] == "Time")
                    {
                        time = item;
                    }

                    if (item[0] == "EEG_RAW_T7")
                    {
                        eegt7 = item;
                    }

                    if (item[0] == "L POR X [px]")
                    {
                        LPORX = item;
                    }

                    if (item[0] == "L POR Y [px]")
                    {
                        LPORY = item;
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

                playButton.Enabled = true;
                stopButton.Enabled = true;
                pauseButton.Enabled = true;
                rewindButton.Enabled = true;

                selectCsvFileButton.Enabled = false;
                selectMediaFileButton.Enabled = false;

                startAnalysisButton.Enabled = false;

                trackBar.Enabled = true;
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            windowsMediaPlayer.Ctlcontrols.play();
            videoPlayTimer.Enabled = true;
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            windowsMediaPlayer.Ctlcontrols.pause();
            videoPlayTimer.Enabled = false;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            windowsMediaPlayer.Ctlcontrols.stop();
            videoPlayTimer.Enabled = false;
        }

        private void rewindButton_Click(object sender, EventArgs e)
        {
            windowsMediaPlayer.Ctlcontrols.currentPosition = 0;
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            csvFilePath = null;
            mediaFilePath = null;
            playButton.Enabled = false;
            stopButton.Enabled = false;
            pauseButton.Enabled = false;
            rewindButton.Enabled = false;

            selectCsvFileButton.Enabled = true;
            selectMediaFileButton.Enabled = true;

            mediaFilePathTextBox.Text = mediaFilePath;
            csvFilePathTextBox.Text = csvFilePath;

            windowsMediaPlayer.Ctlcontrols.stop();
            windowsMediaPlayer.URL = null;

            trackBar.Enabled = false;

            EEGChart.Series.Clear();
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            double newPosition = mediaDuration * ((double)trackBar.Value/100);
            windowsMediaPlayer.Ctlcontrols.currentPosition = newPosition;
        }

        private void videoPlayTimer_Tick(object sender, EventArgs e)
        {
            double currentVideoPositionInPercent = ((windowsMediaPlayer.Ctlcontrols.currentPosition / mediaDuration) * 100);

            // protection, do not go higher then 100%
            if(currentVideoPositionInPercent > 100)
            {
                currentVideoPositionInPercent = 100;
            }

            int newTrackBarValue = (int)currentVideoPositionInPercent;

            if (newTrackBarValue <= trackBar.Maximum)
            {
                trackBar.Value = newTrackBarValue;
            }

            int LPORXindex = (int)(LPORX.Count * (currentVideoPositionInPercent/100));
            int LPORYindex = (int)(LPORY.Count * (currentVideoPositionInPercent/100));

            // first row contains identifier, skip it
            if(LPORXindex < 1)
            {
                LPORXindex = 1;
            }

            if(LPORYindex < 1)
            {
                LPORYindex = 1;
            }

            // on 100% it will be out of range, because counting index begins at 0 -> prevent
            if(LPORXindex >= LPORX.Count)
            {
                LPORXindex = LPORX.Count - 1;
            }

            if (LPORYindex >= LPORY.Count)
            {
                LPORYindex = LPORY.Count - 1;
            }

            // get only the the value before the dot (for int)
            var splitValues = LPORX[LPORXindex].Split('.');

            int eyeX = Convert.ToInt32(splitValues[0]);

            splitValues = LPORY[LPORYindex].Split('.');

            int eyeY = Convert.ToInt32(splitValues[0]);

            // draw point
            Pen pen = new Pen(Color.Red, 2);
            Brush brush = new SolidBrush(Color.Red);

            Graphics g = windowsMediaPlayer.CreateGraphics();
            g.DrawRectangle(pen, eyeX, eyeY, 5, 5);

            g.DrawLine(pen, 0, 0, 2000, 2000);


            //TransparentControl transparentImage = new TransparentControl();
            //transparentImage.Height = 100;
            //transparentImage.Width = 100;
            //Image image = Image.FromFile(@"C:\Work\TKL\EEGETAnalysis\EEGETAnalysis\eye.png");
            //transparentImage.Image = image;
            ////this.Controls.Add(transparentImage);

            //windowsMediaPlayer.Controls.Add(transparentImage);

            //transparentImage.BringToFront();
            //transparentImage.Left = 200;
            //transparentImage.Top = 100;
        }

        private void windowsMediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            // media ended, disable timer
            if (e.newState == 8)
            {
                videoPlayTimer.Enabled = false;
            }
        }
    }
}
