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
using System.ComponentModel;

namespace EEGETAnalysis.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Media members

        /// <summary>
        /// Media player object
        /// </summary>
        MediaPlayer mp;

        /// <summary>
        /// Duration of current media
        /// </summary>
        double mediaDuration = 0;

        /// <summary>
        /// Video size in percent. Value is used to calculate video size on window resize.
        /// </summary>
        private double videoSizeInPercent = 100;

        /// <summary>
        /// The current video position in percent
        /// </summary>
        private double currentVideoPositionInPercent = 0;

        #endregion

        #region Eye Tracking members

        /// <summary>
        /// Red point to show eye focus on canvas
        /// </summary>
        Ellipse eyePoint;

        /// <summary>
        /// Eye x coordinate
        /// </summary>
        double eyeX = 0;

        /// <summary>
        /// Eye y coordinate
        /// </summary>
        double eyeY = 0;

        #endregion

        #region EEG Graph members

        /// <summary>
        /// Graph which is used to paint line chart of the EEG
        /// </summary>
        BasicDSP.Graph eegGraph;

        /// <summary>
        /// ZedGraphControl is used to paint EEG. This control is a WinForms control and
        /// implemented by using WindowsFormsIntegration.
        /// </summary>
        ZedGraph.ZedGraphControl eegZedGraph;

        /// <summary>
        /// EEGLine object. The EEGLine is used to point the current position on the chart.
        /// </summary>
        ZedGraph.LineObj eegLine;

        /// <summary>
        /// ZedGraph Pane for EEG
        /// </summary>
        ZedGraph.GraphPane zedGraphPane;

        /// <summary>
        /// List with waveform colors for the EEG wave forms
        /// </summary>
        List<System.Drawing.Color> waveformColors;

        /// <summary>
        /// Current electrode which is selected for the EEG.
        /// </summary>
        Electrode currentElectrode;

        #endregion

        #region Emotion Graph members

        /// <summary>
        /// Graph which is used for emotions
        /// </summary>
        BasicDSP.Graph emotionGraph;

        /// <summary>
        /// ZedGraphControl for emotion graph
        /// </summary>
        ZedGraph.ZedGraphControl emotionZedGraph;

        /// <summary>
        /// Line for current position on emotion graph
        /// </summary>
        ZedGraph.LineObj emotionLine;

        /// <summary>
        /// GraphPane for emotion graph
        /// </summary>
        ZedGraph.GraphPane emotionGraphPane;

        #endregion

        #region Spectrum Graph members

        /// <summary>
        /// ZedGraphControl is used to paint spectrum. This control is a WinForms control and
        /// implemented by using WindowsFormsIntegration.
        /// </summary>
        ZedGraph.ZedGraphControl spectrumZedGraph;

        /// <summary>
        /// ZedGraph Pane for spectrum
        /// </summary>
        ZedGraph.GraphPane spectrumGraphPane;

        /// <summary>
        /// Graph which is used to paint line chart of the spectrum
        /// </summary>
        BasicDSP.Graph spectrumGraph;

        /// <summary>
        /// List of available spectrum sizes
        /// </summary>
        int[] spectrumSizes = { 64, 128, 256 };

        /// <summary>
        /// Current spectrum size
        /// </summary>
        int currentSpectrumSize = 0;

        /// <summary>
        /// Current electrode which is selected for the spectrum.
        /// </summary>
        Electrode currentSpectrumElectrode;

        #endregion

        #region Activity Graph members

        /// <summary>
        /// Graph which is used for activities
        /// </summary>
        BasicDSP.Graph activityGraph;

        /// <summary>
        /// ZedGraphControl for activity graph
        /// </summary>
        ZedGraph.ZedGraphControl activityZedGraph;

        /// <summary>
        /// Line for current position on activity graph
        /// </summary>
        ZedGraph.LineObj activityLine;

        /// <summary>
        /// GraphPane for activity graph
        /// </summary>
        ZedGraph.GraphPane activityGraphPane;

        /// <summary>
        /// List with waveform colors for the Activity wave forms
        /// </summary>
        List<System.Drawing.Color> activityWaveformColors;

        #endregion

        /// <summary>
        /// Timer is used to synchronize UI elements and eye tracking data to the video
        /// </summary>
        DispatcherTimer timer;

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
        /// Position of the graph lines (all graph lines share the same position).
        /// </summary>
        double graphLineXPosition = 0;

        /// <summary>
        /// Emotionizer object.
        /// </summary>
        EEGEmotionizer emotionizer = null;

        /// <summary>
        /// Worker to load CSV data in background
        /// </summary>
        private readonly BackgroundWorker worker = new BackgroundWorker();

        /// <summary>
        /// Bool value indicates if CSV data was processed. This value can be used
        /// to provide calls of functions if CSV data is not loaded (otherwise exceptions
        /// could be thrown - e.g. NullReferenceException).
        /// </summary>
        bool csvDataProcessed = false;

        /// <summary>
        /// BusyBar - the BusyBar is added in code because otherwise the XAML view shows an error
        /// </summary>
        Microsoft.Windows.Controls.BusyIndicator BusyBar = null;

        /// <summary>
        /// Window construct. Initialize componentes and events.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            MediaCanvas.SizeChanged += MediaCanvas_SizeChanged;
            this.SizeChanged += MainWindow_SizeChanged;
        }

        /// <summary>
        /// Process if input data is set (CSV and media)
        /// </summary>
        private void enableStartAnalysisButtonIfPathsAreSet()
        {
            if (!String.IsNullOrEmpty(CsvFilePathTextBox.Text) && !String.IsNullOrEmpty(MediaFilePathTextBox.Text))
            {
                BusyBar.IsBusy = true;
                CSVWorkerArgs args = new CSVWorkerArgs() { CsvFilePath = CsvFilePathTextBox.Text };
                worker.RunWorkerAsync(args);
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
            CurrentSpectrumComboBox.IsEnabled = status;
            OriginalWaveCheckBox.IsEnabled = status;
            AlphaWaveCheckBox.IsEnabled = status;
            BetaWaveCheckBox.IsEnabled = status;
            ThetaWaveCheckBox.IsEnabled = status;
            DeltaWaveCheckBox.IsEnabled = status;
            AlphaActivityCheckBox.IsEnabled = status;
            BetaActivityCheckBox.IsEnabled = status;
            ThetaActivityCheckBox.IsEnabled = status;
            DeltaActivityCheckBox.IsEnabled = status;
            NormalizeEEGWaveformsCheckBox.IsEnabled = status;
            NormalizeActivityCheckBox.IsEnabled = status;
            CurrentSpectrumSizeComboBox.IsEnabled = status;
            Slider.IsEnabled = status;
            SaveWAVButton.IsEnabled = status;

            if(!status)
            {
                OriginalWaveCheckBox.IsChecked = false;
                AlphaWaveCheckBox.IsChecked = false;
                BetaWaveCheckBox.IsChecked = false;
                DeltaWaveCheckBox.IsChecked = false;
                ThetaWaveCheckBox.IsChecked = false;
                AlphaActivityCheckBox.IsChecked = false;
                BetaActivityCheckBox.IsChecked = false;
                DeltaActivityCheckBox.IsChecked = false;
                ThetaActivityCheckBox.IsChecked = false;
                NormalizeEEGWaveformsCheckBox.IsChecked = false;
                NormalizeActivityCheckBox.IsChecked = false;
            }
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

            EyePointInit();

            waveformColors = new List<System.Drawing.Color>();
            activityWaveformColors = new List<System.Drawing.Color>();

            EEGGraphInit();
            
            EmotionGraphInit();

            SpectrumGraphInit();

            ActivityGraphInit();

            // Add electrodes to combo box (EEG and spectrum)
            foreach (var item in Enum.GetValues(typeof(Electrode)))
            {
                CurrentWaveComboBox.Items.Add(item);
                CurrentSpectrumComboBox.Items.Add(item);
            }

            // Add spectrum sizes to combobox
            foreach (var value in spectrumSizes)
            {
                CurrentSpectrumSizeComboBox.Items.Add(value);
            }

            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            BusyBar = new Microsoft.Windows.Controls.BusyIndicator();
            BusyBar.BusyContent = "Loading CSV data...";
            BusyBar.Margin = new Thickness((MediaCanvas.ActualWidth / 2) - 75, MediaCanvas.ActualHeight / 2, 0, 0);

            MediaCanvas.Children.Add(BusyBar);
        }

        /// <summary>
        /// Init EyePoint circle graph
        /// </summary>
        private void EyePointInit()
        {
            eyePoint = new Ellipse();
            eyePoint.Stroke = Brushes.Red;
            eyePoint.StrokeThickness = 4;
            eyePoint.Width = 10;
            eyePoint.Height = 10;
            MediaCanvas.Children.Add(eyePoint);

            eyePoint.Visibility = Visibility.Hidden;
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
            SynchronizeComponents();
        }

        /// <summary>
        /// Synchronize all components (Eye Tracking, media etc).
        /// </summary>
        private void SynchronizeComponents()
        {
            List<EEGSample> samples = emotionizer.Sampler.GetAllGoodSamples();

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

            if (eyeX < 0)
            {
                eyeX = eyePoint.ActualWidth;
            }

            if (eyeY < 0)
            {
                eyeY = eyePoint.ActualHeight;
            }

            if (eyeX > MediaCanvas.ActualWidth)
            {
                eyeX = MediaCanvas.ActualWidth - eyePoint.ActualWidth;
            }

            if (eyeY > MediaCanvas.ActualHeight)
            {
                eyeY = MediaCanvas.ActualHeight - eyePoint.ActualHeight;
            }

            eyePoint.Margin = new Thickness(eyeX, eyeY, 0, 0);
            // Eye tracking (END)

            graphLineXPosition = (Convert.ToDouble(mp.Position.Milliseconds) * 0.001) + Convert.ToDouble(mp.Position.Seconds);
            DrawEEGLine();
            DrawEmotionLine();
            DrawActivityLine();
            DrawSpectrum();
        }

        #region Worker

        /// <summary>
        /// Load CSV data in background task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            CSVWorkerArgs args = (CSVWorkerArgs)e.Argument;
            CSVWorkerResult result = new CSVWorkerResult();
            result.Success = false;

            try
            {
                CsvParser parser = new CsvParser(args.CsvFilePath);
                List<List<string>> csvData = parser.Parse();

                int sampleRate = Convert.ToInt32(parser.GetMetaDataDictionary().GetValue("Sample Rate"));

                Sampler sampler = new Sampler(csvData, sampleRate);
                result.Emotionizer = new EEGEmotionizer(sampler);
                result.Success = true;
            }
            catch (System.IO.IOException ex)
            {
                result.ErrorMessage = ex.Message;
                result.Success = false;
            }

            e.Result = result;
        }

        /// <summary>
        /// CSV data load completed, adjust GUI if data was loaded successfully
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CSVWorkerResult workerResult = (CSVWorkerResult)e.Result;
            if (workerResult.Success)
            {
                try
                {
                    emotionizer = workerResult.Emotionizer;

                    mp = new MediaPlayer();
                    mp.ScrubbingEnabled = true;
                    mp.MediaOpened += mp_MediaOpened;
                    mp.MediaEnded += mp_MediaEnded;

                    mp.Open(new Uri(MediaFilePathTextBox.Text));

                    VideoDrawing vd = new VideoDrawing();
                    vd.Player = mp;
                    vd.Rect = new Rect(0, 0, 100, 100);

                    DrawingBrush db = new DrawingBrush(vd);

                    MediaCanvas.Background = db;

                    SetControlButtonsEnabled(true);

                    SelectCsvFileButton.IsEnabled = false;
                    SelectMediaFileButton.IsEnabled = false;

                    ResetButton.IsEnabled = true;

                    csvDataProcessed = true;

                    eegGraph.PlotClear(1);
                    emotionGraph.PlotClear(1);
                    activityGraph.PlotClear(1);
                    spectrumGraph.PlotClear(1);

                    EEGZedGraphRefresh();

                    CurrentWaveComboBox.SelectedIndex = 0;
                    CurrentSpectrumComboBox.SelectedIndex = 0;
                    CurrentSpectrumSizeComboBox.SelectedIndex = 1;
                    OriginalWaveCheckBox.IsChecked = true;
                    AlphaActivityCheckBox.IsChecked = true;

                    DrawEEGLine();
                    DrawSpectrum();
                    DrawEmotions();
                    DrawActivities();
                }
                catch (System.IO.IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show(workerResult.ErrorMessage);
            }

            BusyBar.IsBusy = false;
        }

        #endregion

        #region Emotion Graph functions

        /// <summary>
        /// Emotion ZedGraph init
        /// </summary>
        private void EmotionGraphInit()
        {
            System.Windows.Forms.Integration.WindowsFormsHost emotionHost = new System.Windows.Forms.Integration.WindowsFormsHost();
            emotionZedGraph = new ZedGraph.ZedGraphControl();
            emotionZedGraph.IsEnableZoom = false;
            emotionHost.Child = emotionZedGraph;
            EmotionGrid.Children.Add(emotionHost);

            emotionGraph = new BasicDSP.Graph(emotionZedGraph.CreateGraphics(), emotionZedGraph);

            emotionGraphPane = emotionZedGraph.GraphPane;
            emotionLine = new ZedGraph.LineObj(System.Drawing.Color.Red, graphLineXPosition, emotionGraphPane.YAxis.Scale.Min, graphLineXPosition, emotionGraphPane.YAxis.Scale.Max);
            emotionLine.Line.Width = 1f;
            emotionGraphPane.GraphObjList.Add(emotionLine);

            EmotionZedGraphRefresh();
        }

        /// <summary>
        /// Draw emotion line graph with different line colors.
        /// </summary>
        private void EmotionZedGraphRefresh()
        {
            ZedGraph.GraphPane myPane = emotionZedGraph.GraphPane;
            System.Drawing.Color tmpColor = System.Drawing.Color.Blue;
            Brush tmpBrush = Brushes.Blue;
            string text = "";
            Thickness margin = new Thickness() { Left = 5, Right = 5 };

            if (emotionizer != null)
            {
                int i = 0;
                foreach (KeyValuePair<Emotion, BasicDSP.Waveform> emotion in emotionizer.Emotions)
                {
                    switch (emotion.Key)
                    {
                        case Emotion.RAGE:
                            tmpColor = System.Drawing.Color.Red;
                            tmpBrush = Brushes.Red;
                            text = "Rage";
                            break;
                        case Emotion.JOY:
                            tmpColor = System.Drawing.Color.Green;
                            tmpBrush = Brushes.Green;
                            text = "Joy";
                            break;
                        case Emotion.SORROW:
                            tmpColor = System.Drawing.Color.DarkMagenta;
                            tmpBrush = Brushes.DarkMagenta;
                            text = "Sorrow";
                            break;
                        case Emotion.FEAR:
                            tmpColor = System.Drawing.Color.Orange;
                            tmpBrush = Brushes.Orange;
                            text = "Fear";
                            break;
                        default:
                            break;
                    }

                    TextBlock textBlock = new TextBlock() { Text = text, Foreground = tmpBrush, Margin = margin };
                    EmotionStackPanel.Children.Add(textBlock);

                    myPane.CurveList[i].Color = tmpColor;
                    i++;
                }
            }

            foreach (ZedGraph.LineItem lineItem in myPane.CurveList)
            {
                lineItem.Line.IsSmooth = true;
                lineItem.Line.Width = 2f;
            }

            myPane.XAxis.Scale.FontSpec.Size = chartFontSize;
            myPane.XAxis.Title.FontSpec.Size = chartFontSize;
            myPane.YAxis.Scale.FontSpec.Size = chartFontSize;
            myPane.YAxis.Title.FontSpec.Size = chartFontSize;
            myPane.Title.Text = " ";
            emotionZedGraph.Refresh();
        }

        /// <summary>
        /// Draw the Emotion Line on the right position depending on the video position
        /// </summary>
        private void DrawEmotionLine()
        {
            if (csvDataProcessed)
            {
                emotionGraphPane.GraphObjList.Remove(emotionLine);
                emotionLine = new ZedGraph.LineObj(System.Drawing.Color.Red, graphLineXPosition, emotionGraphPane.YAxis.Scale.Min, graphLineXPosition, emotionGraphPane.YAxis.Scale.Max);
                emotionLine.Line.Width = 1f;
                emotionGraphPane.GraphObjList.Add(emotionLine);
                emotionZedGraph.Refresh();
            }
        }

        /// <summary>
        /// Draw emotions on ZedGraph
        /// </summary>
        private void DrawEmotions()
        {
            if (csvDataProcessed)
            {
                emotionGraph.PlotClear(1);

                BasicDSP.Waveform signal;

                foreach (KeyValuePair<Emotion, BasicDSP.Waveform> emotion in emotionizer.Emotions)
                {
                    signal = emotion.Value;
                    emotionGraph.PlotWaveform(1, ref signal, emotion.Key.ToString());
                }

                EmotionZedGraphRefresh();
            }
        }

        #endregion

        #region EEG Graph functions

        /// <summary>
        /// EEG ZedGraph init
        /// </summary>
        private void EEGGraphInit()
        {
            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
            eegZedGraph = new ZedGraph.ZedGraphControl();
            eegZedGraph.IsEnableZoom = false;
            // http://goorman.free.fr/ZedGraph/zedgraph.org/wiki/indexa9f9.html?title=Edit_Points_by_Dragging_the_Mouse
            //zedGraph.MouseDownEvent += zedGraph_MouseDownEvent;
            host.Child = eegZedGraph;
            EEGGrid.Children.Add(host);

            eegGraph = new BasicDSP.Graph(eegZedGraph.CreateGraphics(), eegZedGraph);

            zedGraphPane = eegZedGraph.GraphPane;
            eegLine = new ZedGraph.LineObj(System.Drawing.Color.Red, graphLineXPosition, zedGraphPane.YAxis.Scale.Min, graphLineXPosition, zedGraphPane.YAxis.Scale.Max);
            eegLine.Line.Width = 1f;
            zedGraphPane.GraphObjList.Add(eegLine);

            EEGZedGraphRefresh();
        }

        /// <summary>
        /// Draw the EEGData. Refresh is called when current electrode is changed.
        /// </summary>
        private void EEGZedGraphRefresh()
        {
            ZedGraph.GraphPane myPane = eegZedGraph.GraphPane;

            int i = 0;
            foreach (System.Drawing.Color color in waveformColors)
            {
                myPane.CurveList[i].Color = color;
                if (myPane.CurveList[i].IsLine)
                {
                    ((ZedGraph.LineItem)myPane.CurveList[i]).Line.IsSmooth = true;
                }

                i++;
            }

            myPane.XAxis.Scale.FontSpec.Size = chartFontSize;
            myPane.XAxis.Title.FontSpec.Size = chartFontSize;
            myPane.YAxis.Scale.FontSpec.Size = chartFontSize;
            myPane.YAxis.Title.FontSpec.Size = chartFontSize;
            myPane.Title.Text = " ";
            eegZedGraph.Refresh();
        }

        /// <summary>
        /// Draw the EEGLine on the right position depending on the video position
        /// </summary>
        private void DrawEEGLine()
        {
            if (csvDataProcessed)
            {
                zedGraphPane.GraphObjList.Remove(eegLine);
                eegLine = new ZedGraph.LineObj(System.Drawing.Color.Red, graphLineXPosition, zedGraphPane.YAxis.Scale.Min, graphLineXPosition, zedGraphPane.YAxis.Scale.Max);
                eegLine.Line.Width = 1f;
                zedGraphPane.GraphObjList.Add(eegLine);
                eegZedGraph.Refresh();
            }
        }

        /// <summary>
        /// Draw waveforms on ZedGraph depending on which one is selected.
        /// </summary>
        private void DrawWaveforms()
        {
            if (csvDataProcessed)
            {
                eegGraph.PlotClear(1);
                waveformColors.Clear();

                BasicDSP.Waveform waveform = emotionizer.Sampler.GetEEGWaveform(currentElectrode);
                EEGAnalyzer analyzer = new EEGAnalyzer(waveform);

                bool normalize = NormalizeEEGWaveformsCheckBox.IsChecked == true ? true : false;

                if (OriginalWaveCheckBox.IsChecked == true)
                {
                    if (normalize)
                    {
                        waveform = EEGUtils.Normalize(waveform);
                    }

                    waveformColors.Add(System.Drawing.Color.Blue);
                    eegGraph.PlotWaveform(1, ref waveform, "Original Wave");
                }

                if (AlphaWaveCheckBox.IsChecked == true)
                {
                    waveformColors.Add(System.Drawing.Color.Red);
                    BasicDSP.Waveform waveformAlpha = analyzer.FilterBand(EEGBand.ALPHA, normalize);
                    eegGraph.PlotWaveform(1, ref waveformAlpha, "Alpha");
                }

                if (BetaWaveCheckBox.IsChecked == true)
                {
                    waveformColors.Add(System.Drawing.Color.Green);
                    BasicDSP.Waveform waveformBeta = analyzer.FilterBand(EEGBand.BETA, normalize);
                    eegGraph.PlotWaveform(1, ref waveformBeta, "Beta");
                }

                if (ThetaWaveCheckBox.IsChecked == true)
                {
                    waveformColors.Add(System.Drawing.Color.DarkMagenta);
                    BasicDSP.Waveform waveformTheta = analyzer.FilterBand(EEGBand.THETA, normalize);
                    eegGraph.PlotWaveform(1, ref waveformTheta, "Theta");
                }

                if (DeltaWaveCheckBox.IsChecked == true)
                {
                    waveformColors.Add(System.Drawing.Color.Orange);
                    BasicDSP.Waveform waveformDelta = analyzer.FilterBand(EEGBand.DELTA, normalize);
                    eegGraph.PlotWaveform(1, ref waveformDelta, "Delta");
                }

                EEGZedGraphRefresh();
            }
        }

        #endregion

        #region Spectrum Graph functions

        /// <summary>
        /// Init spectrum ZedGraph
        /// </summary>
        private void SpectrumGraphInit()
        {
            System.Windows.Forms.Integration.WindowsFormsHost spectrumZedGraphHost = new System.Windows.Forms.Integration.WindowsFormsHost();
            spectrumZedGraph = new ZedGraph.ZedGraphControl();
            spectrumZedGraph.IsEnableZoom = false;
            spectrumZedGraphHost.Child = spectrumZedGraph;
            SpectrumGrid.Children.Add(spectrumZedGraphHost);

            spectrumGraph = new BasicDSP.Graph(spectrumZedGraph.CreateGraphics(), spectrumZedGraph);

            spectrumGraphPane = spectrumZedGraph.GraphPane;

            SpectrumZedGraphRefresh();
        }

        /// <summary>
        /// Set font size for the spectrum zed graph
        /// </summary>
        private void SpectrumZedGraphRefresh()
        {
            ZedGraph.GraphPane myPane = spectrumZedGraph.GraphPane;

            foreach (ZedGraph.LineItem lineItem in myPane.CurveList)
            {
                lineItem.Line.IsSmooth = true;
            }

            myPane.XAxis.Scale.FontSpec.Size = chartFontSize;
            myPane.XAxis.Title.FontSpec.Size = chartFontSize;
            myPane.YAxis.Scale.FontSpec.Size = chartFontSize;
            myPane.YAxis.Title.FontSpec.Size = chartFontSize;
            myPane.Title.Text = " ";
            spectrumZedGraph.Refresh();
        }

        /// <summary>
        /// Draw spectrum on ZedGraph
        /// </summary>
        private void DrawSpectrum()
        {
            if (csvDataProcessed)
            {
                spectrumGraph.PlotClear(1);

                BasicDSP.Waveform waveform = emotionizer.Sampler.GetEEGWaveform(currentSpectrumElectrode);
                EEGAnalyzer analyzer = new EEGAnalyzer(waveform);

                int spectrumIndex = Convert.ToInt32(Convert.ToDouble(analyzer.Waveform.Count) * currentVideoPositionInPercent);

                BasicDSP.Spectrum spectrum = analyzer.GetSpectrum(spectrumIndex, currentSpectrumSize);
                spectrumGraph.PlotDbSpectrum(1, ref spectrum, "");

                SpectrumZedGraphRefresh();
            }
        }

        #endregion

        #region Activity Graph functions

        /// <summary>
        /// Activity ZedGraph init
        /// </summary>
        private void ActivityGraphInit()
        {
            System.Windows.Forms.Integration.WindowsFormsHost activityHost = new System.Windows.Forms.Integration.WindowsFormsHost();
            activityZedGraph = new ZedGraph.ZedGraphControl();
            activityZedGraph.IsEnableZoom = false;
            activityHost.Child = activityZedGraph;
            ActivityGrid.Children.Add(activityHost);

            activityGraph = new BasicDSP.Graph(activityZedGraph.CreateGraphics(), activityZedGraph);

            activityGraphPane = activityZedGraph.GraphPane;
            activityLine = new ZedGraph.LineObj(System.Drawing.Color.Red, graphLineXPosition, activityGraphPane.YAxis.Scale.Min, graphLineXPosition, activityGraphPane.YAxis.Scale.Max);
            activityLine.Line.Width = 1f;
            activityGraphPane.GraphObjList.Add(activityLine);

            ActivityZedGraphRefresh();
        }

        /// <summary>
        /// Draw activity line graph with different line colors.
        /// </summary>
        private void ActivityZedGraphRefresh()
        {
            ZedGraph.GraphPane myPane = activityZedGraph.GraphPane;
            System.Drawing.Color tmpColor = System.Drawing.Color.Blue;
            Brush tmpBrush = Brushes.Blue;
            Thickness margin = new Thickness() { Left = 5, Right = 5 };

            int i = 0;
            foreach (System.Drawing.Color color in activityWaveformColors)
            {
                myPane.CurveList[i].Color = color;
                if (myPane.CurveList[i].IsLine)
                {
                    ((ZedGraph.LineItem)myPane.CurveList[i]).Line.IsSmooth = true;
                    ((ZedGraph.LineItem)myPane.CurveList[i]).Line.Width = 2f;
                }

                i++;
            }

            myPane.XAxis.Scale.FontSpec.Size = chartFontSize;
            myPane.XAxis.Title.FontSpec.Size = chartFontSize;
            myPane.YAxis.Scale.FontSpec.Size = chartFontSize;
            myPane.YAxis.Title.FontSpec.Size = chartFontSize;
            myPane.Title.Text = " ";
            activityZedGraph.Refresh();
        }

        /// <summary>
        /// Draw the Emotion Line on the right position depending on the video position
        /// </summary>
        private void DrawActivityLine()
        {
            if (csvDataProcessed)
            {
                activityGraphPane.GraphObjList.Remove(activityLine);
                activityLine = new ZedGraph.LineObj(System.Drawing.Color.Red, graphLineXPosition, activityGraphPane.YAxis.Scale.Min, graphLineXPosition, activityGraphPane.YAxis.Scale.Max);
                activityLine.Line.Width = 1f;
                activityGraphPane.GraphObjList.Add(activityLine);
                activityZedGraph.Refresh();
            }
        }

        /// <summary>
        /// Draw emotions on ZedGraph
        /// </summary>
        private void DrawActivities()
        {
            if (csvDataProcessed)
            {
                activityGraph.PlotClear(1);
                activityWaveformColors.Clear();

                bool normalize = NormalizeActivityCheckBox.IsChecked == true ? true : false;

                if (AlphaActivityCheckBox.IsChecked == true)
                {
                    BasicDSP.Waveform waveformAlpha;

                    if (emotionizer.AverageActivity.TryGetValue(EEGBand.ALPHA, out waveformAlpha))
                    {
                        if (normalize)
                        {
                            waveformAlpha = EEGUtils.Normalize(waveformAlpha);
                        }

                        activityWaveformColors.Add(System.Drawing.Color.Red);
                        activityGraph.PlotWaveform(1, ref waveformAlpha, "Alpha");
                    }
                }

                if (BetaActivityCheckBox.IsChecked == true)
                {
                    BasicDSP.Waveform waveformBeta;

                    if (emotionizer.AverageActivity.TryGetValue(EEGBand.BETA, out waveformBeta))
                    {
                        if (normalize)
                        {
                            waveformBeta = EEGUtils.Normalize(waveformBeta);
                        }

                        activityWaveformColors.Add(System.Drawing.Color.Green);
                        activityGraph.PlotWaveform(1, ref waveformBeta, "Beta");
                    }
                }

                if (ThetaActivityCheckBox.IsChecked == true)
                {
                    BasicDSP.Waveform waveformTheta;

                    if (emotionizer.AverageActivity.TryGetValue(EEGBand.THETA, out waveformTheta))
                    {
                        if (normalize)
                        {
                            waveformTheta = EEGUtils.Normalize(waveformTheta);
                        }

                        activityWaveformColors.Add(System.Drawing.Color.DarkMagenta);
                        activityGraph.PlotWaveform(1, ref waveformTheta, "Theta");
                    }
                }

                if (DeltaActivityCheckBox.IsChecked == true)
                {
                    BasicDSP.Waveform waveformDelta;

                    if (emotionizer.AverageActivity.TryGetValue(EEGBand.DELTA, out waveformDelta))
                    {
                        if (normalize)
                        {
                            waveformDelta = EEGUtils.Normalize(waveformDelta);
                        }

                        activityWaveformColors.Add(System.Drawing.Color.Orange);
                        activityGraph.PlotWaveform(1, ref waveformDelta, "Delta");
                    }
                }

                ActivityZedGraphRefresh();
            }
        }

        #endregion

        #region GUI events

        /// <summary>
        /// ComboBox selection changed for the current EEG electrode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentWaveComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentElectrode = (Electrode)((sender as ComboBox).SelectedItem);
            DrawWaveforms();
        }

        /// <summary>
        /// Size of the main window changed. Position BusyBar on center.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (BusyBar != null)
            {
                BusyBar.Margin = new Thickness((MediaCanvas.ActualWidth / 2) - 75, MediaCanvas.ActualHeight / 2, 0, 0);
            }
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
        /// Move video position to slider position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mp.Position = TimeSpan.FromSeconds(Slider.Value);

            // Calculate the current video position in percent. This value is used
            // for different calculations
            currentVideoPositionInPercent = mp.Position.Seconds / mediaDuration;

            // protection, do not go higher then 100%
            if (currentVideoPositionInPercent > 100)
            {
                currentVideoPositionInPercent = 100;
            }

            TimeTextBlock.Text = String.Format("{0:00}", mp.Position.Hours) + ":" + String.Format("{0:00}", mp.Position.Minutes) + ":" + String.Format("{0:00}", mp.Position.Seconds);

            if(!timer.IsEnabled)
            {
                SynchronizeComponents();
            }
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
        /// Reset all data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            mp.Close();

            EmotionStackPanel.Children.Clear();

            Slider.Value = 0;

            SetControlButtonsEnabled(false);

            CsvFilePathTextBox.Text = null;
            MediaFilePathTextBox.Text = null;

            ResetButton.IsEnabled = false;

            eyePoint.Visibility = Visibility.Hidden;

            SelectCsvFileButton.IsEnabled = true;
            SelectMediaFileButton.IsEnabled = true;

            csvDataProcessed = false;

            waveformColors.Clear();
            activityWaveformColors.Clear();

            mp = null;
            MediaCanvas.Width = this.ActualWidth;
            DrawingBrush myDrawingBrush = new DrawingBrush();
            MediaCanvas.Background = myDrawingBrush;

            eegGraph.PlotClear(1);
            eegZedGraph.Refresh();

            spectrumGraph.PlotClear(1);
            spectrumZedGraph.Refresh();

            emotionGraph.PlotClear(1);
            emotionZedGraph.Refresh();

            activityGraph.PlotClear(1);
            activityZedGraph.Refresh();
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
        /// EEG Waveform CheckBox check or unchecked event method. Draw the waveforms.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EEGWaveCheckBox_CheckedOrUnchecked(object sender, RoutedEventArgs e)
        {
            DrawWaveforms();
        }

        /// <summary>
        /// Activites Waveform CheckBox check or unchecked event method. Draw the waveforms.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActivityCheckBox_CheckedOrUnchecked(object sender, RoutedEventArgs e)
        {
            DrawActivities();
        }

        /// <summary>
        /// Set the current selected electrode for spectrum
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentSpectrumComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentSpectrumElectrode = (Electrode)((sender as ComboBox).SelectedItem);
            DrawSpectrum();
        }

        /// <summary>
        /// Set current selected size for spectrum
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentSpectrumSizeComboBox_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            currentSpectrumSize = (int)((sender as ComboBox).SelectedItem);
            DrawSpectrum();
        }

        /// <summary>
        /// Save EEG data to .wav file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveWAVButton_Click(object sender, RoutedEventArgs e)
        {
            string defaultFileName = "EEG Waveforms";

            if (!string.IsNullOrEmpty(CsvFilePathTextBox.Text))
            {
                defaultFileName = System.IO.Path.GetFileNameWithoutExtension(CsvFilePathTextBox.Text) + "_Electrode_" + currentElectrode.ToString();
            }
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = defaultFileName; // Default file name
            dlg.DefaultExt = ".wav"; // Default file extension
            dlg.Filter = "Waveform Audio File (.wav)|*.wav"; // Filter files by extension 

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results 
            if (result == true)
            {
                // Save document 
                string filename = dlg.FileName;

                try
                {
                    EEGAnalyzer analyzer = null;

                    if (emotionizer.Analyzers.TryGetValue(currentElectrode, out analyzer))
                    {
                        analyzer.SaveWavFile(filename);
                    }
                    else
                    {
                        MessageBox.Show("Could not load EEGAnalyzer. Save failed.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        #endregion

        #region Media functions

        /// <summary>
        /// Media ended method. Stop media and reset media position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mp_MediaEnded(object sender, EventArgs e)
        {
            mp.Stop();
            mp.Position = TimeSpan.FromSeconds(0);
            eyePoint.Visibility = Visibility.Hidden;
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

        #endregion
    }
}
