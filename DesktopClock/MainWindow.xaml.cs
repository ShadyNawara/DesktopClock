using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using WpfScreenHelper;

namespace DesktopClock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Finds main screen scalling
        /// starts a new form in the correct position for all screens
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            object? screenScalingRegistry = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ThemeManager", "LastLoadedDPI", "96");

            float screenScalling = (screenScalingRegistry != null ? int.Parse((string)screenScalingRegistry) : 96.0f) / 96.0f;

            foreach (Screen s in Screen.AllScreens)
            {
                if (s.Primary)
                {
                    continue;
                }

                Rect viewport = s.WorkingArea;

                MainWindow mainWindow = new(false)
                {
                    Top = (viewport.Top / screenScalling) + (viewport.Height / screenScalling) - this.Height,
                    Left = (viewport.Left / screenScalling) + (viewport.Width / screenScalling) - this.Width
                };
                mainWindow.Show();
            }

            Hide();
        }

        /// <summary>
        /// secondary windows start clock tick every second
        /// </summary>
        /// <param name="firstWindow">whether or not this is the first window,
        ///     not really used just to be able to overload the constructor</param>
        public MainWindow(bool firstWindow)
        {
            if (!firstWindow)
            {
                InitializeComponent();
                DispatcherTimer clockTicker = new();
                clockTicker.Tick += new EventHandler(ClockTick);
                clockTicker.Interval = new TimeSpan(0, 0, 1);
                clockTicker.Start();
            }
        }

        /// <summary>
        /// Update clock text based on current time / data
        /// adjust to format if needed
        /// </summary>
        public void ClockTick(object? sender, EventArgs? e)
        {
            timeTextBlock.Text = DateTime.Now.ToString("h:mm:ss tt");
            dateTextBlock.Text = DateTime.Now.ToString("dddd dd, MMMM yyyy");
        }

        /// <summary>
        /// Exit the application
        /// </summary>
        private void ExitButtonClicked_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
