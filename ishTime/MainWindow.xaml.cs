using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ishTime
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isPlay;
        private static Mutex _mutex = new Mutex(true, "{ishTime}");

        public MainWindow()
        {
            InitializeComponent();
            OnStartup();
            timeText.Text = "00:00:00";
            UpdateTodayText();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            string image = "";
            if (isPlay)
            {
                image = "/resources/play.png";
                isPlay = false;
                StopTimer();
            }
            else
            {
                image = "/resources/stop.png";
                isPlay = true;
                StartTimer();
            }
            Uri uri = new Uri(image, UriKind.Relative);
            ((Image)playButton).Source = new BitmapImage(uri);
        }

        private async void StartTimer()
        {
            Models db = new Models();
            db.StartTimer();
            while (isPlay)
            {
                TimeSpan currentTime = db.GetActiveTime();
                timeText.Text = currentTime.ToString(@"hh\:mm\:ss");
                await Task.Delay(1000);
            }
        }

        private void StopTimer()
        {
            Models db = new Models();
            db.StopTimer();
            UpdateTodayText();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
     
            if (isPlay)
            {
                StopTimer();
            }
            _mutex.ReleaseMutex();
            _mutex.Dispose();
        }

        private void UpdateTodayText()
        {
            Models db = new Models();
            TimeSpan todayStat = TimeSpan.FromSeconds(db.GetTodayStat());
            if (todayStat.TotalHours >= 1)
            {
                todayText.Text = $"Today: {Math.Round(todayStat.TotalHours, 1)}h";
            }else if (todayStat.TotalMinutes >= 1)
            {
                todayText.Text = $"Today: {Math.Round(todayStat.TotalMinutes, 1)}m";
            }else if (todayStat.TotalSeconds >= 1)
            {
                todayText.Text = $"Today: {Math.Round(todayStat.TotalSeconds)}s";
            }else { todayText.Text = "Today: ISHLA ESHAK"; }
            
        }


        protected void OnStartup()
        {
            if (!_mutex.WaitOne(TimeSpan.Zero, true))
            {
                MessageBox.Show("Another instance of the application is already running.");
                Application.Current.Shutdown();
            }

        }
    }
}
