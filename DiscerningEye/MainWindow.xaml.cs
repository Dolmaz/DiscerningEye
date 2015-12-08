using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
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
using System.Xml.Linq;


namespace DiscerningEye
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {

        //================================================================
        //
        //  Members
        //
        //================================================================
        #region Members
        public string currentEorzeaTime;
        public string NodeFilePath = Environment.CurrentDirectory + "\\nodes.xml";
        public List<string> columnNameList;



        private EorzeaTimeExtension eorzeaTimeExtension;
        private ObservableCollection<Node> nodeList;
        private bool _IsEditingAlarms;
        private bool _PlayNotification;
        private bool _UseTextToSpeech;
        private int _DataGridSearchIndex;
        private int _EasterEggTextToSpeech;
        private bool _EnableEasterEggTextToSpeech;
        private double _AlarmPretrigger;
        private double _EorzeanOffset;

        private ObservableCollection<AlarmItem> alarmCollection;
        private System.Timers.Timer updateTimer;
        private SpeechSynthesizer speaker = new SpeechSynthesizer();
        #endregion Members


        //================================================================
        //
        //  Properties
        //
        //================================================================
        #region Properties
        /// <summary>
        /// Gets or Sets the current eorzea time string
        /// </summary>
        public string CurrentEorzeaTime
        {
            get { return this.currentEorzeaTime; }
            set
            {
                this.currentEorzeaTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentEorzeaTime"));
            }
        }

        /// <summary>
        /// Gets or Sets the value for the alarm pretrigger
        /// </summary>
        public double AlarmPretrigger
        {
            get { return this._AlarmPretrigger; }
            set
            {
                this._AlarmPretrigger = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AlarmPretrigger"));
            }
        }

        /// <summary>
        /// Gets or Sets the value for the offset of the eorzean clock time
        /// </summary>
        public double EorzeanOffset
        {
            get { return this._EorzeanOffset; }
            set
            {
                this._EorzeanOffset = value;
                OnPropertyChanged(new PropertyChangedEventArgs("EorzeanOffset"));
            }
        }

        /// <summary>
        /// Gets or sets the boolean value for if notifications sound should be played.
        /// </summary>
        public bool PlayNotification
        {
            get { return this._PlayNotification; }
            set
            {
                this._PlayNotification = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PlayNotification"));
            }
        }

        /// <summary>
        /// Gets of Sets the boolean value for if text-to-speech should be used
        /// </summary>
        public bool UseTextToSpeech
        {
            get { return this._UseTextToSpeech; }
            set
            {
                this._UseTextToSpeech = value;
                if (value)
                {
                    this._EasterEggTextToSpeech++;
                    if (this._EasterEggTextToSpeech >= 10 && this._EnableEasterEggTextToSpeech == false)
                    {
                        this._EnableEasterEggTextToSpeech = true;
                        this.lblEasterEggTextToSpeech.Visibility = Visibility.Visible;
                    }
                }
                OnPropertyChanged(new PropertyChangedEventArgs("UseTextToSpeech"));
            }
        }


        /// <summary>
        /// Gets or Sets boolean value indicating that the alarms on datagrid are being edited
        /// </summary>
        public bool IsEditingAlarms
        {
            get { return this._IsEditingAlarms; }
            set
            {
                this._IsEditingAlarms = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsEditingAlarms"));
            }
        }

        private int DataGridSearchIndex
        {
            get { return this._DataGridSearchIndex; }
            set
            {
                this._DataGridSearchIndex = value;
            }
        }
        #endregion Properties


        //================================================================
        //
        //  Constructor
        //
        //================================================================
        #region Constructor
        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.ToString());
            }
  
            this.DataContext = this;
            this.eorzeaTimeExtension = new EorzeaTimeExtension();
            this.eorzeaTimeExtension.OnEorzeaTimeUpdate += EorzeaTimeExtension_OnEorzeaTimeUpdate;

            

            this.IsEditingAlarms = false;
            this.AlarmPretrigger = 1;
            this.EorzeanOffset = 0;
            this.PlayNotification = false;
            this.UseTextToSpeech = false;
            this.DataGridSearchIndex = 0;
            this.chkAlarmPreTrigger.IsChecked = true;

            nodeList = new ObservableCollection<Node>();
            alarmCollection = new ObservableCollection<AlarmItem>();
            this.LoadNodes();


            CollectionViewSource itemCollectionViewSource;
            itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
            itemCollectionViewSource.Source = nodeList;

            this.updateTimer = new System.Timers.Timer();
            this.updateTimer.Interval = 1;
            this.updateTimer.Elapsed += UpdateTimer_Elapsed;
            this.updateTimer.Start();



        }
        #endregion Constructor


        //================================================================
        //
        //  Delegates
        //
        //================================================================
        #region Delegates
        #endregion Delegates


        //================================================================
        //
        //  Events
        //
        //================================================================
        #region Events

        #region PropertyChanged
        
        //
        //  Property Changed
        //
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        #endregion PropertyChanged
        #endregion Events



        //================================================================
        //
        //  Event Handlers of this window
        //
        //================================================================
        #region WindowEvents
        /// <summary>
        /// Occurs when the window has finished loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //  Present message about Eorzea Time being off sometimes and how to resolve.
            this.ShowMessageAsync("Reminder about Eorzean Time", "The time shown on the app for Current Eorzean Time may be off.  This is becuase it syncs based on your computers system time.  If it is off there are two things you can do:" +
                                   "\n\n1) Perform a sync with your current set Internet Time Server" +
                                   "\n\n2) Adjust the Eorzean Time Offset value found in settings by clicking the cog wheel in the top right");
        }

        /// <summary>
        /// Occurs when the window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainWindow_Closed(object sender, EventArgs e)
        {
            //  Ensure the trayicon object is disposed so we don't get ghost icons in the system tray
            this.trayIcon.Dispose();
        }

        /// <summary>
        /// Occurs when the windows state is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainWindow_StateChanged(object sender, EventArgs e)
        {
            //  If the window is now minimized, make it hidden (minimze to tray effect)
            if (this.WindowState == WindowState.Minimized)
                this.Visibility = Visibility.Hidden;
        }
        #endregion WindowEvents


        //================================================================
        //
        //  Event handlers of objects in this window
        //
        //================================================================
        #region OtherEventHandlers

        #region updateTImer
        //
        //  updatTimer
        //

        /// <summary>
        /// occurs when the updateTimer Elapsed event is called.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {



            //  Create a list of strings.  This is used to compile all alarms that are triggered
            List<string> Nodes = new List<string>();


            //  Check if any alarms are within 5 minutes
            foreach (AlarmItem alarm in this.alarmCollection)
            {
                //  Get the time difference between the alarm item and eorzea time
                TimeSpan timeDiff = alarm.Time.Subtract(eorzeaTimeExtension.EorzeaTimeSpan());

                //  If the time difference is 5 minnutes (+ offset) then add alert and disarm the alert.
                //  Alert will automatically realarm itself after 30 secs
                //if (timeDiff <= new TimeSpan(0, Convert.ToInt32(this.nudAlarmPreTrigger.Value), 0) && timeDiff > new TimeSpan(0, 0, 0) && alarm.Armed)
                if (timeDiff <= new TimeSpan(0, Convert.ToInt32(this.AlarmPretrigger), 0) && timeDiff > new TimeSpan(0, 0, 0) && alarm.Armed)
                {
                    alarm.Armed = false;
                    Nodes.Add(alarm.Item + " in " + alarm.Location + " in slot " + alarm.Slot + "\n");
                }
            }

            //  Check to see if any alerts were added to the nodes list.  If there are, then compile the alerts and push
            //  notifications
            if (Nodes.Count > 0)
            {
                //  Compile the alert string
                StringBuilder sb = new StringBuilder();
                foreach (string item in Nodes)
                {
                    sb.Append(item);
                }

                //  Play tone if enabled
                if (this.PlayNotification)
                {
                    SoundPlayer ding = new SoundPlayer(Properties.Resources.ding);
                    ding.Play();
                }

                //  Compile final string
                string text = string.Empty;
                if (this._EnableEasterEggTextToSpeech)
                    text = "The Following nodes are avaliable\n" + sb.ToString();
                else
                    text = "The Following nodes are available\n" + sb.ToString();

                //  Use text-to-speech if enabled
                if (this.UseTextToSpeech)
                {
                    speaker.SpeakAsync(text);
                }

                //  Push notification popup

                this.trayIcon.ShowBalloonTip("Nodes Avaliable", "The following node is avaliable\n" + sb.ToString(), Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
            }
        }
        #endregion updateTimer

        #region dataGrid
        //
        //  dataGrid
        //

        /// <summary>
        /// Occurs when the checkbox in the checkbox column is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //  Add the item to the alarm collection object
            this.alarmCollection.Add(new AlarmItem(this.dataGrid.SelectedItem as Node));
        }
        #endregion dataGrid

        #region EorzeaTimeExtension
        //
        //  EorzeaTimeExtension
        //

        /// <summary>
        /// Occurs when the EorzeaTimeExtension object triggers an update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EorzeaTimeExtension_OnEorzeaTimeUpdate(object sender, EorzeaTimeEventArgs args)
        {
            //  Update the current eorzea time string
            this.CurrentEorzeaTime = args.Time.ToShortTimeString();
        }
        #endregion EorzeaTimeExtension

        #region btnFilter
        //
        //  btnFilter
        //

        /// <summary>
        /// Occurs when btnFilter is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            var flyout = this.Flyouts.Items[0] as Flyout;
            if (flyout != null)
            {
                flyout.IsOpen = true;
            }
        }
        #endregion btnFilter

        #region btnApplyFilter
        //
        //  btnApplyFilter
        //

        /// <summary>
        /// Occurs when btnApplyFilter is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            this.ApplyFilter();

        }
        #endregion btnApplyFilter

        #region btnClearFilter
        //
        //  btnClearFilter
        //

        /// <summary>
        /// Occurs when btnClearFilter is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            this.cmbAlarmSet.SelectedIndex = 0;
            this.cmbClassFilter.SelectedIndex = 0;
            this.cmbTime.SelectedIndex = 0;
            this.txtItemFilter.Clear();
            this.txtLocationFilter.Clear();

            ICollectionView cv = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);

            cv.Filter = o =>
            {
                Node node = o as Node;
                return true;
            };

        }
        #endregion btnClearFilter


        #region btnSettings
        //
        //  btnSettings
        //

        /// <summary>
        /// Occurs when btnSettings is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSettigns_Click(object sender, RoutedEventArgs e)
        {
            var flyout = this.Flyouts.Items[1] as Flyout;
            if (flyout != null)
            {
                flyout.IsOpen = true;
            }
        }
        #endregion btnSettings

        #region chkEnableEorzeaOffset
        //
        //  chkEnableEorzeaOffset
        //
        
        /// <summary>
        /// Occurs when chkEnableEorzeaOffset is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkEnableEorzeaOffset_Checked(object sender, RoutedEventArgs e)
        {
            this.nudEorzeaOffset.IsEnabled = true;
        }

        /// <summary>
        /// Occurs when chkEnableEorzeaOffset is unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkEnableEorzeaOffset_Unchecked(object sender, RoutedEventArgs e)
        {
            this.nudEorzeaOffset.IsEnabled = false;
        }
        #endregion chkEnableEorzeaOffset

        #region chkAlarmPretrigger
        //
        //  chkAlarmPreTrigger
        //

        /// <summary>
        /// Occurs when chkAparmPreTrigger is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAlarmPreTrigger_Checked(object sender, RoutedEventArgs e)
        {
            this.nudAlarmPreTrigger.IsEnabled = true;
        }

        /// <summary>
        /// Occurs when chkAparmPreTrigger is unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAlarmPreTrigger_Unchecked(object sender, RoutedEventArgs e)
        {
            this.nudAlarmPreTrigger.IsEnabled = false;
        }
        #endregion chkAlarmPretrigger

        #region trayIcon
        //
        //  trayIcon
        //

        /// <summary>
        /// Occurs when the tray icon is double clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trayIcon_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            //  Restore the window state and make the window visible
            this.WindowState = WindowState.Normal;
            this.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Occurs whent he close menu item is clicked on the trayIcon context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion trayIcon

        #region btnViewThirdParty
        //
        //  btnViewThirdParty
        //

        /// <summary>
        /// Occurs when btnViewThirdParty is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewThirdParty_Click(object sender, RoutedEventArgs e)
        {
            string folderLocation = Environment.CurrentDirectory + "\\ThirdPartyLicenses";
            if (Directory.Exists(folderLocation))
                Process.Start(folderLocation);
            else
                this.ShowMessageAsync("Folder Not Found", "The following folder could not be located:\n\n" + folderLocation);

        }
        #endregion btnViewThirdParty
        #endregion OtherEventHandlers


        //================================================================
        //
        //  Helper Methods/Functions
        //
        //================================================================
        #region HelperMethods
        /// <summary>
        /// Loads the XML document nodes.xml and parses the information into the datagrid
        /// </summary>
        private void LoadNodes()
        {
            if (!File.Exists(NodeFilePath))
            {
                //  Show file missing dialog
                this.ShowMessageAsync("File nodes.xml Missing", "Could not locate the file nodes.xml at the following location\n\n" + NodeFilePath);
            }
            else
            {
                //  load xml into list
                XDocument doc = XDocument.Load(NodeFilePath);

                var nodes = from node in doc.Root.Descendants("Node")
                            select new
                            {
                                SetAlarm = node.Element("SetAlarm").Value,
                                Time = node.Element("Time").Value,
                                Item = node.Element("Item").Value,
                                Slot = node.Element("Slot").Value,
                                Location = node.Element("Location").Value,
                                Star = node.Element("Star").Value,
                                ClassJob = node.Element("ClassJob").Value,
                                NodeType = node.Element("Nodetype").Value,
                                BlueScripts = node.Element("BlueScripts").Value,
                                RedScripts = node.Element("RedScripts").Value,
                                Notes = node.Element("Notes").Value
                            };

                foreach (var node in nodes)
                {
                    Node nodeItem = new Node(
                       Boolean.Parse(node.SetAlarm),
                        node.Time,
                        node.Item,
                        node.Location,
                        node.Slot,
                        node.Star,
                        node.ClassJob,
                        node.NodeType,
                        node.BlueScripts,
                        node.RedScripts,
                        node.Notes);
                    this.nodeList.Add(nodeItem);

                }
            }
        }

        /// <summary>
        /// Applys the filter settings to the datagrid
        /// </summary>
        private void ApplyFilter()
        {
            //  Retreive filter settings
            bool? setAlarm;
            if (this.cmbAlarmSet.Text == "No Filter")
                setAlarm = null;
            else if (this.cmbAlarmSet.Text == "True")
                setAlarm = true;
            else
                setAlarm = false;

            string time = (this.cmbTime.Text == "All Times") ? "" : this.cmbTime.Text;
            string item = this.txtItemFilter.Text;
            string location = this.txtLocationFilter.Text;
            string classjob = (this.cmbClassFilter.Text == "All Classes") ? "" : this.cmbClassFilter.Text;


            //  Create collection view
            ICollectionView cv = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);

            cv.Filter = i =>
            {
                Node node = i as Node;
                if (setAlarm == null)
                {
                    return (
                           node.Time.Contains(time) && node.Item.ToLower().Contains(item.ToLower()) && node.Location.ToLower().Contains(location.ToLower()) && node.ClassJob.ToLower().Contains(classjob.ToLower())
                    );
                }
                else
                {
                    return (
                           node.SetAlarm == setAlarm && node.Time.Contains(time) && node.Item.ToLower().Contains(item.ToLower()) && node.Location.ToLower().Contains(location.ToLower()) && node.ClassJob.ToLower().Contains(classjob.ToLower())
                    );
                }
            };

            var flyout = this.Flyouts.Items[0] as Flyout;
            if (flyout != null)
            {
                flyout.IsOpen = false;
            }
        }
        #endregion HelperMethods


    }
}
