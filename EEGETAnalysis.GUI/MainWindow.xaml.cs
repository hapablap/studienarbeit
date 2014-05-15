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
using System.Windows.Controls.DataVisualization.Charting;
using System.Threading;

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
        List<double> time = null;

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

        Line eegLine;

        double eegLineNullPoint = 12;

        double eegLineCurrentPosition = 0;

        /// <summary>
        /// Current data list index depending on current video position.
        /// This value is used to pick list values on the correct position
        /// depending on the video position. It is calculted on the timer tick.
        /// </summary>
        int currentDataIndex = 0;

        /// <summary>
        /// Eye x coordinate
        /// </summary>
        double eyeX = 0;

        /// <summary>
        /// Eye y coordinate
        /// </summary>
        double eyeY = 0;

        public MainWindow()
        {
            data = new ChartInput();
            InitializeComponent();
            this.DataContext = data;

            MediaCanvas.SizeChanged += MediaCanvas_SizeChanged;
            EEGDataCanvas.SizeChanged += EEGDataCanvas_SizeChanged;

            Worker wObject = new Worker();
            //Thread t = new Thread()
        }

        void EEGDataCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LineChart.Height = EEGDataCanvas.ActualHeight;
            LineChart.Width = EEGDataCanvas.ActualWidth;
        }

        /// <summary>
        /// Calculate the canvas width by height to ensure correct video format
        /// when window size is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MediaCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (mp != null && mp.NaturalVideoHeight > 0)
            {
                videoSizeInPercent = (e.NewSize.Height / mp.NaturalVideoHeight);
                MediaCanvas.Width = mp.NaturalVideoWidth * videoSizeInPercent;
            }
        }

        /// <summary>
        /// Enable the start analysis button only if input data is set
        /// </summary>
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

        /// <summary>
        /// Method to set all controls at the same time enabled / disabled
        /// </summary>
        /// <param name="status"></param>
        private void SetControlButtonsEnabled(bool status)
        {
            PlayButton.IsEnabled = status;
            PauseButton.IsEnabled = status;
            StopButton.IsEnabled = status;
            RewindButton.IsEnabled = status;
            Slider.IsEnabled = status;
        }

        /// <summary>
        /// Window Loaded event: Set up timer, set controls disabled on startup
        /// and prepare eye tracking point.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += timer_Tick;

            ExecuteButton.IsEnabled = false;
            CsvFilePathTextBox.IsEnabled = false;
            MediaFilePathTextBox.IsEnabled = false;

            SetControlButtonsEnabled(false);
            ResetButton.IsEnabled = false;

            eyePoint = new Ellipse();
            eyePoint.Stroke = Brushes.Red;
            eyePoint.StrokeThickness = 4;
            eyePoint.Width = 10;
            eyePoint.Height = 10;
            MediaCanvas.Children.Add(eyePoint);

            eyePoint.Visibility = Visibility.Hidden;

            eegLine = new Line();
            eegLine.Stroke = Brushes.Red;
            eegLine.StrokeThickness = 1;
            eegLine.Height = 130;
            eegLine.Width = 1000000;
            eegLine.X1 = eegLineNullPoint;
            eegLine.Y1 = 47;
            eegLine.X2 = eegLineNullPoint;
            eegLine.Y2 = 300;

            EEGDataCanvas.Children.Add(eegLine);

            Style dataPointStyle = GetNewDataPointStyle();
            series.DataPointStyle = dataPointStyle;
        }

        /// <summary>
        /// Timer tick: Get the eye tracking data depending on video position
        /// and draw the eye point on this position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e)
        {
            // Synchronize slider position with video
            Slider.Value = mp.Position.TotalSeconds;

            // Calculate the current video position in percent. This value is used
            // for different calculations
            currentVideoPositionInPercent = mp.Position.Seconds / mediaDuration;
            
            // protection, do not go higher then 100%
            if (currentVideoPositionInPercent > 100)
            {
                currentVideoPositionInPercent = 100;
            }

            currentDataIndex = (int)(LPORX.Count * currentVideoPositionInPercent);

            // first row contains identifier, skip it
            if (currentDataIndex < 1)
            {
                currentDataIndex = 1;
            }

            // on 100% it will be out of range, because counting index begins at 0 -> prevent
            if (currentDataIndex >= LPORX.Count)
            {
                currentDataIndex = LPORX.Count - 1;
            }

            // Eye tracking (BEGIN)

            // get only the the value before the dot (for int)
            var splitValues = LPORX[currentDataIndex].Split('.');

            eyeX = Convert.ToDouble(splitValues[0]) * videoSizeInPercent;

            splitValues = LPORY[currentDataIndex].Split('.');

            eyeY = Convert.ToDouble(splitValues[0]) * videoSizeInPercent;

            if(eyeX < 0)
            {
                eyeX = eyePoint.ActualWidth;
            }

            if(eyeY < 0)
            {
                eyeY = eyePoint.ActualHeight;
            }

            if(eyeX > MediaCanvas.ActualWidth)
            {
                eyeX = MediaCanvas.ActualWidth - eyePoint.ActualWidth;
            }

            if(eyeY > MediaCanvas.ActualHeight)
            {
                eyeY = MediaCanvas.ActualHeight - eyePoint.ActualHeight;
            }

            eyePoint.Margin = new Thickness(eyeX, eyeY, 0, 0);
            // Eye tracking (END)

            // EEG data (BEGIN)
            //this.data.Add(new KeyValuePair<long, long>(mp.Position.Seconds, Convert.ToInt64(eegt7[currentDataIndex].Substring(0, eegt7[currentDataIndex].Length - 3))));
            DrawEEGLine();
            // EEG data (END)
        }

        private void DrawEEGLine()
        {
            eegLineCurrentPosition = series.ActualWidth * currentVideoPositionInPercent;
            eegLine.X1 = eegLineCurrentPosition + eegLineNullPoint;
            eegLine.X2 = eegLineCurrentPosition + eegLineNullPoint;
        }

        /// <summary>
        /// Move video position to slider position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mp.Position = TimeSpan.FromSeconds(Slider.Value);

            //if (!videoIsPlaying)
            //{
            //    this.data.Clear();
            //}
        }

        /// <summary>
        /// Select CSV input file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Select media input file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Disable dots on the line chart for better performance.
        /// </summary>
        /// <returns></returns>
        private static Style GetNewDataPointStyle()
        {
            Color background = Color.FromRgb(0, 0, 0);
            Style style = new Style(typeof(DataPoint));
            Setter st1 = new Setter(DataPoint.BackgroundProperty, 
                                        new SolidColorBrush(background));
            Setter st2 = new Setter(DataPoint.BorderBrushProperty, 
                                        new SolidColorBrush(Colors.White));
            Setter st3 = new Setter(DataPoint.BorderThicknessProperty, new Thickness(0.1));

            Setter st4 = new Setter(DataPoint.TemplateProperty, null);
            style.Setters.Add(st1);
            style.Setters.Add(st2);
            style.Setters.Add(st3);
            style.Setters.Add(st4);
            return style;
        }

        /// <summary>
        /// Process / parse the CSV file, open the video, add data to chart, enable controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            mp = new MediaPlayer();
            mp.MediaOpened += mp_MediaOpened;
            mp.MediaEnded += mp_MediaEnded;

            mp.Open(new Uri(MediaFilePathTextBox.Text));

            VideoDrawing vd = new VideoDrawing();
            vd.Player = mp;
            vd.Rect = new Rect(0, 0, 100, 100);

            DrawingBrush db = new DrawingBrush(vd);

            MediaCanvas.Background = db;

            try
            {
                CsvParser parser = new CsvParser(CsvFilePathTextBox.Text);
                csvData = parser.Parse();

                List<string> timeTemp = new List<string>();
                time = new List<double>();

                foreach (List<string> item in csvData)
                {
                    if (item[0] == "Time")
                    {
                        timeTemp = item;
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

                if (timeTemp.Count > 0)
                {
                    timeTemp.RemoveRange(0, 2); // remove head line and first (useless data)
                    eegt7.RemoveRange(0, 2);

                    timeTemp.RemoveAt(timeTemp.Count - 1); // remove last line (useless data)
                    eegt7.RemoveAt(eegt7.Count - 1);

                    double sampleRate = Convert.ToDouble(parser.GetMetaDataDictionary().GetValue("Sample Rate"));

                    for (int i = 0; i < timeTemp.Count; i++)
                    {
                        //time.Add(i * (1 / sampleRate));
                        time.Add(Convert.ToDouble(timeTemp[i]));
                    }

                    for (int j = 0; j < eegt7.Count; j = j + 10)
                    {
                        this.data.Add(new KeyValuePair<long, long>(Convert.ToInt64(time[j]), Convert.ToInt64(eegt7[j].Substring(0, eegt7[j].Length - 3))));
                    }
                }

                SetControlButtonsEnabled(true);

                SelectCsvFileButton.IsEnabled = false;
                SelectMediaFileButton.IsEnabled = false;

                ExecuteButton.IsEnabled = false;
                ResetButton.IsEnabled = true;

                Point relativeLocation = series.TranslatePoint(new Point(0, 0), LineChart);
                eegLineNullPoint = relativeLocation.Y;
                DrawEEGLine();
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void mp_MediaEnded(object sender, EventArgs e)
        {
            mp.Stop();
            mp.Position = TimeSpan.FromSeconds(0);
        }

        /// <summary>
        /// Get some video values when media is opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mp_MediaOpened(object sender, EventArgs e)
        {
            TimeSpan ts = mp.NaturalDuration.TimeSpan;
            mediaDuration = ts.TotalSeconds;
            Slider.Maximum = mediaDuration;

            videoSizeInPercent = (MediaCanvas.ActualHeight / mp.NaturalVideoHeight);

            MediaCanvas.Width = mp.NaturalVideoWidth * videoSizeInPercent;
        }

        /// <summary>
        /// Reset all data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            mp.Close();
            this.data.Clear();
            Slider.Value = 0;
            SetControlButtonsEnabled(false);
            CsvFilePathTextBox.Text = null;
            MediaFilePathTextBox.Text = null;
            ResetButton.IsEnabled = false;
            eyePoint.Visibility = Visibility.Hidden;
            SelectCsvFileButton.IsEnabled = true;
            SelectMediaFileButton.IsEnabled = true;
        }

        /// <summary>
        /// Play the video and start the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            mp.Play();
            timer.Start();
            eyePoint.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Stop the video and stop the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mp.Stop();
            timer.Stop();
            PlayButton.IsEnabled = true;
        }

        /// <summary>
        /// Pause the video and stop the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            mp.Pause();
            timer.Stop();
        }

        /// <summary>
        /// Set video position to 0 (rewind)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {
            mp.Position = new TimeSpan(0);
            PlayButton.IsEnabled = true;
        }
    }
}
