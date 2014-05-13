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
        DispatcherTimer timer;

        MediaPlayer mp;

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

        //public List<KeyValuePair<long, long>> valueList;
        private ChartInput data;

        public MainWindow()
        {
            data = new ChartInput();
            InitializeComponent();
            this.DataContext = data;
        }

        private void enableStartAnalysisButtonIfPathsAreSet()
        {
            if (!String.IsNullOrEmpty(CsvFilePathTextBox.Text) && !String.IsNullOrEmpty(MediaFilePathTextBox.Text))
            {
                ExecuteButton.IsEnabled = true;
                SetControlButtonsEnabled(true);
            }
            else
            {
                ExecuteButton.IsEnabled = false;
                SetControlButtonsEnabled(false);
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
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Slider.Value = mp.Position.TotalSeconds;
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
            Slider.Maximum = ts.TotalSeconds;
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
