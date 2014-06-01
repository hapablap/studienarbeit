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

        Sampler sampler = null;

        /// <summary>
        /// Sample data from CSV
        /// </summary>
        List<EEGSample> samples = null;

        /// <summary>
        /// Video size in percent. Value is used to calculate video size on window resize.
        /// </summary>
        private double videoSizeInPercent = 100;

        /// <summary>
        /// The current video position in percent
        /// </summary>
        private double currentVideoPositionInPercent = 0;

        /// <summary>
        /// Red point to show eye focus on canvas
        /// </summary>
        Ellipse eyePoint;

        /// <summary>
        /// Font size for chart content
        /// </summary>
        float chartFontSize = 26f;

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

        int sampleRate;

        /// <summary>
        /// Graph which is used to paint line chart on the ZedGraphControl
        /// </summary>
        BasicDSP.Graph graph;

        /// <summary>
        /// ZedGraphControl is used to paint charts. This control is a WinForms control and
        /// implemented by using WindowsFormsIntegration
        /// </summary>
        ZedGraph.ZedGraphControl zedGraph;

        /// <summary>
        /// EEGLine object. The EEGLine is used to point the current position on the chart.
        /// </summary>
        ZedGraph.LineObj EEGLine;

        /// <summary>
        /// ZedGraph Pane
        /// </summary>
        ZedGraph.GraphPane zedGraphPane;

        /// <summary>
        /// Position of the EEG line.
        /// </summary>
        double EEGLineXPosition = 0;

        Electrode CurrentElectrode;

        /// <summary>
        /// Window construct. Initialize componentes and events.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            MediaCanvas.SizeChanged += MediaCanvas_SizeChanged;
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
        /// Process if input data is set (CSV and media)
        /// </summary>
        private void enableStartAnalysisButtonIfPathsAreSet()
        {
            if (!String.IsNullOrEmpty(CsvFilePathTextBox.Text) && !String.IsNullOrEmpty(MediaFilePathTextBox.Text))
            {
                ProcessCSVData();
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
            CurrentWaveComboBox.IsEnabled = status;
            AlphaWaveCheckBox.IsEnabled = status;
            BetaWaveCheckBox.IsEnabled = status;
            ThetaWaveCheckBox.IsEnabled = status;
            DeltaWaveCheckBox.IsEnabled = status;
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

            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
            zedGraph = new ZedGraph.ZedGraphControl();
            zedGraph.IsEnableZoom = false;
            host.Child = zedGraph;
            EEGGrid.Children.Add(host);

            graph = new BasicDSP.Graph(zedGraph.CreateGraphics(), zedGraph);

            zedGraphPane = zedGraph.GraphPane;
            EEGLine = new ZedGraph.LineObj(System.Drawing.Color.Red, EEGLineXPosition, zedGraphPane.YAxis.Scale.Min, EEGLineXPosition, zedGraphPane.YAxis.Scale.Max);
            EEGLine.Line.Width = 1f;
            zedGraphPane.GraphObjList.Add(EEGLine);

            ZedGraphRefresh();

            foreach (var item in Enum.GetValues(typeof(Electrode)))
            {
                CurrentWaveComboBox.Items.Add(item);
            }
        }

        void CurrentWaveComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentElectrode = (Electrode)((sender as ComboBox).SelectedItem);
            DrawWaveforms();
        }

        /// <summary>
        /// Draw the EEGData. Refresh is called when CheckBox are checked or unchecked.
        /// </summary>
        private void ZedGraphRefresh()
        {
            ZedGraph.GraphPane myPane = zedGraph.GraphPane;
            System.Drawing.Color tmpColor = System.Drawing.Color.Blue;

            for (int i = 0; i < myPane.CurveList.Count; i++)
            {
                switch (i)
                {
                    case 1:
                        tmpColor = System.Drawing.Color.Red;
                        break;
                    case 2:
                        tmpColor = System.Drawing.Color.Green;
                        break;
                    case 3:
                        tmpColor = System.Drawing.Color.DarkMagenta;
                        break;
                    case 4:
                        tmpColor = System.Drawing.Color.Orange;
                        break;
                    case 5:
                        tmpColor = System.Drawing.Color.Brown;
                        break;
                    default:
                        break;
                }

                myPane.CurveList[i].Color = tmpColor;
            }

            myPane.XAxis.Scale.FontSpec.Size = chartFontSize;
            myPane.XAxis.Title.FontSpec.Size = chartFontSize;
            myPane.YAxis.Scale.FontSpec.Size = chartFontSize;
            myPane.YAxis.Title.FontSpec.Size = chartFontSize;
            myPane.Title.Text = " ";
            zedGraph.Refresh();
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

            currentDataIndex = (int)(samples.Count * currentVideoPositionInPercent);

            // first row contains identifier, skip it
            if (currentDataIndex < 1)
            {
                currentDataIndex = 1;
            }

            // on 100% it will be out of range, because counting index begins at 0 -> prevent
            if (currentDataIndex >= samples.Count)
            {
                currentDataIndex = samples.Count - 1;
            }

            // Eye tracking (BEGIN)
            eyeX = samples[currentDataIndex].eyeX * videoSizeInPercent;
            eyeY = samples[currentDataIndex].eyeY * videoSizeInPercent;

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
            DrawEEGLine();
            // EEG data (END)
        }

        /// <summary>
        /// Draw the EEGLine on the right position depending on the video position
        /// </summary>
        private void DrawEEGLine()
        {
            EEGLineXPosition = (Convert.ToDouble(mp.Position.Milliseconds) * 0.001) + Convert.ToDouble(mp.Position.Seconds);

            zedGraphPane.GraphObjList.Remove(EEGLine);
            EEGLine = new ZedGraph.LineObj(System.Drawing.Color.Red, EEGLineXPosition, zedGraphPane.YAxis.Scale.Min, EEGLineXPosition, zedGraphPane.YAxis.Scale.Max);
            EEGLine.Line.Width = 1f;
            zedGraphPane.GraphObjList.Add(EEGLine);
            zedGraph.Refresh();
        }

        /// <summary>
        /// Move video position to slider position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mp.Position = TimeSpan.FromSeconds(Slider.Value);
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
        /// Process / parse the CSV file, open the video, add data to chart, enable controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessCSVData()
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

                sampleRate = Convert.ToInt32(parser.GetMetaDataDictionary().GetValue("Sample Rate"));

                sampler = new Sampler(csvData, sampleRate);
                samples = sampler.GetAllGoodSamples();

                SetControlButtonsEnabled(true);

                SelectCsvFileButton.IsEnabled = false;
                SelectMediaFileButton.IsEnabled = false;

                ResetButton.IsEnabled = true;

                graph.PlotClear(1);

                ZedGraphRefresh();

                CurrentWaveComboBox.SelectedIndex = 0;
                //OriginalWaveCheckBox.IsChecked = true;
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Draw waveforms on ZedGraph depending on which CheckBoxes are selected.
        /// </summary>
        private void DrawWaveforms()
        {
            graph.PlotClear(1);

            BasicDSP.Waveform waveform = sampler.GetEEGWaveform(CurrentElectrode);
            BasicDSP.Signal signal = waveform.Quantise();
            EEGAnalyzer analyzer = new EEGAnalyzer(waveform);

            if (OriginalWaveCheckBox.IsChecked == true)
            {
                graph.PlotSignal(1, ref signal, "");
            }

            if(AlphaWaveCheckBox.IsChecked == true)
            {
                BasicDSP.Signal waveformAlpha = analyzer.FilterBand(EEGBand.ALPHA).Quantise();
                graph.PlotSignal(1, ref waveformAlpha, "");
                //BasicDSP.Waveform spectrum = analyzer.GetAmplitudeSpectrum(50, 128);
                //graph.PlotWaveform(1, ref spectrum, "");
            }

            if (BetaWaveCheckBox.IsChecked == true)
            {
                BasicDSP.Signal waveformBeta = analyzer.FilterBand(EEGBand.BETA).Quantise();
                graph.PlotSignal(1, ref waveformBeta, "");
            }

            if (ThetaWaveCheckBox.IsChecked == true)
            {
                BasicDSP.Signal waveformTheta = analyzer.FilterBand(EEGBand.THETA).Quantise();
                graph.PlotSignal(1, ref waveformTheta, "");
            }

            if (DeltaWaveCheckBox.IsChecked == true)
            {
                BasicDSP.Signal waveformDelta = analyzer.FilterBand(EEGBand.DELTA).Quantise();
                graph.PlotSignal(1, ref waveformDelta, "");
            }

            ZedGraphRefresh();
        }

        /// <summary>
        /// Media ended method. Stop media and reset media position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            graph.PlotClear(1);
            zedGraph.Refresh();
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
            PlayButton.IsEnabled = false;
            mp.Play();
            timer.Start();
            eyePoint.Visibility = Visibility.Visible;
            PauseButton.IsEnabled = true;
        }

        /// <summary>
        /// Stop the video and stop the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mp.Stop();
            mp.Position = new TimeSpan(0);
            timer.Stop();
            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
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
            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
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

        /// <summary>
        /// CheckBox check or unchecked event method. Draw the waveforms.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EEGWaveCheckBox_CheckedOrUnchecked(object sender, RoutedEventArgs e)
        {
            DrawWaveforms();
        }
    }
}
