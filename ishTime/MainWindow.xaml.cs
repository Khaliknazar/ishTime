using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace ishTime
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isPlay;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string image = "";
            if (isPlay)
            {
                image = "/resources/play.png";
                isPlay = false;
            }
            else
            {
                image = "/resources/stop.png";
                isPlay = true;
            }
            Uri uri = new Uri(image, UriKind.Relative);
            ((Image)playButton).Source = new BitmapImage(uri);
        }
    }
}
