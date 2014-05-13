using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EEGETAnalysis.Library;
using System.Windows.Threading;

namespace EEGETAnalysis.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Timer is used to synchronize UI elements and eye tracking data to the video
        /// </summary>
        DispatcherTimer timer;

        /// <summary>
        /// Media player object
        /// </summary>
        MediaPlayer mp;

        /// <summary>
        /// Duration of current media
        /// </summary>
        double mediaDuration = 0;

        /// <summary>
        /// BeGaze CSV data
        /// </summary>
        List<List<string>> csvData = null;

        /// <summary>
        /// timestamps
        /// </summary>
        List<string> time = null;

        /// <summary>
        /// EEG data from T7
        /// </summary>
        List<string> eegt7 = null;

        /// <summary>
        /// Eye tracking coordinates (X)
        /// </summary>
        List<string> LPORX = null;

        /// <summary>
        /// Eye tracking coordinates (Y)
        /// </summary>
        List<string> LPORY = null;

        /// <summary>
        /// Data for chart.
        /// </summary>
        private ChartInput data;

        /// <summary>
        /// Video size in percent. Value is used to calculate video size on window resize.
        /// </summary>
        private double videoSizeInPercent = 100;

        /// <summary>
        /// The current video position in percent
        /// </summary>
        private double currentVideoPositionInPercent;

        /// <summary>
        /// Red point to show eye focus on canvas
        /// </summary>
        Ellipse eyePoint;

        public MainWindow()
        {
            data = new ChartInput();
            InitializeComponent();
            this.DataContext = data;

            MediaCanvas.SizeChanged += MediaCanvas_SizeChanged;
        }

        void MediaCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (mp != null && mp.NaturalVideoHeight > 0)
            {
                videoSizeInPercent = (e.NewSize.Height / mp.NaturalVideoHeight);
                MediaCanvas.Width = mp.NaturalVideoWidth * videoSizeInPercent;
            }
        }

        private void enableStartAnalysisButtonIfPathsAreSet()
        {
            if (!String.IsNullOrEmpty(CsvFilePathTextBox.Text) && !String.IsNullOrEmpty(MediaFilePathTextBox.Text))
            {
                ExecuteButton.IsEnabled = true;
            }
            else
            {
                ExecuteButton.IsEnabled = false;
            }
        }

        private void SetControlButtonsEnabled(bool status)
        {
            PlayButton.IsEnabled = status;
            PauseButton.IsEnabled = status;
            StopButton.IsEnabled = status;
            RewindButton.IsEnabled = status;
            Slider.IsEnabled = status;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += timer_Tick;

            ExecuteButton.IsEnabled = false;
            CsvFilePathTextBox.IsEnabled = false;
            MediaFilePathTextBox.IsEnabled = false;

            SetControlButtonsEnabled(false);

            eyePoint = new Ellipse();
            eyePoint.Stroke = Brushes.Red;
            eyePoint.StrokeThickness = 4;
            MediaCanvas.Children.Add(eyePoint);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Slider.Value = mp.Position.TotalSeconds;

            currentVideoPositionInPercent = mp.Position.Seconds / mediaDuration;
            
            // protection, do not go higher then 100%
            if (currentVideoPositionInPercent > 100)
            {
                currentVideoPositionInPercent = 100;
            }

            int LPORXindex = (int)(LPORX.Count * (currentVideoPositionInPercent / 100));
            int LPORYindex = (int)(LPORY.Count * (currentVideoPositionInPercent / 100));
            
            // first row contains identifier, skip it
            if (LPORXindex < 1)
            {
                LPORXindex = 1;
            }

            if (LPORYindex < 1)
            {
                LPORYindex = 1;
            }

            // on 100% it will be out of range, because counting index begins at 0 -> prevent
            if (LPORXindex >= LPORX.Count)
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

            eyePoint.Margin = new Thickness(eyeX * videoSizeInPercent, eyeY * videoSizeInPercent, 0, 0);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mp.Position = TimeSpan.FromSeconds(Slider.Value);
        }

        private void SelectCsvFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".csv";
            dlg.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                CsvFilePathTextBox.Text = filename;
            }

            enableStartAnalysisButtonIfPathsAreSet();
        }

        private void SelectMediaFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".mp4";
            dlg.Filter = "mp4 files (*.mp4)|*.mp4|All files (*.*)|*.*";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                MediaFilePathTextBox.Text = filename;
            }

            enableStartAnalysisButtonIfPathsAreSet();
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            mp = new MediaPlayer();
            mp.MediaOpened += mp_MediaOpened;

            mp.Open(new Uri(MediaFilePathTextBox.Text));

            VideoDrawing vd = new VideoDrawing();
            vd.Player = mp;
            vd.Rect = new Rect(0, 0, 100, 100);

            DrawingBrush db = new DrawingBrush(vd);

            MediaCanvas.Background = db;

            //mediaDuration = Duration(mediaFilePath);

            try
            {
                CsvParser parser = new CsvParser(CsvFilePathTextBox.Text);
                csvData = parser.Parse();

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

                time.RemoveRange(0, 2); // remove head line and first (useless data)
                eegt7.RemoveRange(0, 2);

                time.RemoveAt(time.Count - 1); // remove last line (useless data)
                eegt7.RemoveAt(eegt7.Count - 1);

                for (int j = 0; j < eegt7.Count; j++)
                {
                    //this.data.Add(new KeyValuePair<long, long>(Convert.ToInt64(time[j]), Convert.ToInt64(eegt7[j].Substring(0, eegt7[j].Length - 3))));
                }

                SetControlButtonsEnabled(true);

                SelectCsvFileButton.IsEnabled = false;
                SelectMediaFileButton.IsEnabled = false;

                ExecuteButton.IsEnabled = false;
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void mp_MediaOpened(object sender, EventArgs e)
        {
            TimeSpan ts = mp.NaturalDuration.TimeSpan;
            mediaDuration = ts.TotalSeconds;
            Slider.Maximum = mediaDuration;

            videoSizeInPercent = (MediaCanvas.ActualHeight / mp.NaturalVideoHeight);

            MediaCanvas.Width = mp.NaturalVideoWidth * videoSizeInPercent;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            mp.Play();
            timer.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mp.Stop();
            timer.Stop();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            mp.Pause();
            timer.Stop();
        }

        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {
            mp.Position = new TimeSpan(0);
        }
    }
}
