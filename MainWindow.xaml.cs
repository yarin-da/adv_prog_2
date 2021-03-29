using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;

namespace Adv_Prog_2
{
    /// <summary>
    /// View Code
    /// </summary>
    public partial class MainWindow : Window
    {
        // constants
        public const int FRAME_SKIP = 50;
        public const float SPEED_OFFSET = 0.1f;

        IViewModel vm = new FlightViewModel();
        
        // colors for status labels
        SolidColorBrush darkGreen = new SolidColorBrush(Colors.DarkGreen);
        SolidColorBrush red = new SolidColorBrush(Colors.Red);

        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
        }

        public void ConnectToServer(object sender, RoutedEventArgs e)
        {
            if (!vm.VM_Connected)
            {
                try
                {
                    // parse the port and the server
                    string portText = PortTextBox.Text;
                    int port = Int32.Parse(portText);
                    string server = ServerTextBox.Text;
                    vm.Connect(port, server);
                    // update the GUI to notify the user that we're connected
                    ConnectButton.Content = "Disconnect";
                    ConnectionLabel.Content = "Connection Established";
                    ConnectionLabel.Foreground = darkGreen;
                }
                catch (Exception ex) { Console.WriteLine(ex); }
            }
            else {
                // update the GUI to notify the user that we're disconnected
                vm.Disconnect();
                ConnectButton.Content = "Connect";
                ConnectionLabel.Content = "Not Connected";
                ConnectionLabel.Foreground = red;
            }
        }

        public void UpdateSpeed(object sender, RoutedEventArgs e)
        {
            try
            {
                // update the speed when the user changes it from the textbox
                float speed = float.Parse(SpeedTextBox.Text);
                vm.VM_Speed = speed;
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }

        public void UploadFlightData(object sender, RoutedEventArgs e)
        {
            // ask the user for a CSV file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.csv)|*.csv|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                // get the filename from the path
                string filePath = openFileDialog.FileName;
                string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                // update the GUI to notify the user that a CSV file is chosen
                FileChosenLabel.Content = fileName;
                FileChosenLabel.Foreground = darkGreen;
                // make sure the model sends frames from the new file
                vm.SetFlightData(filePath);
            }
        }

        public void Help(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Copy your small_playback.xml file into {FlightGear_Installation_Folder}/data/Protocol/");
        }

        public void SkipBackwards(object sender, RoutedEventArgs e)
        {
            vm.VM_Frame -= FRAME_SKIP;
        }

        public void SkipForward(object sender, RoutedEventArgs e)
        {
            vm.VM_Frame += FRAME_SKIP;
        }
        public void ReducePlaybackSpeed(object sender, RoutedEventArgs e)
        {
            vm.VM_Speed -= SPEED_OFFSET;
        }
        public void IncreasePlaybackSpeed(object sender, RoutedEventArgs e)
        {
            vm.VM_Speed += SPEED_OFFSET;
        }

        public void Play(object sender, RoutedEventArgs e)
        {
            vm.Start();
        }
        public void Pause(object sender, RoutedEventArgs e)
        {
            vm.Pause();
        }
        public void Stop(object sender, RoutedEventArgs e)
        {
            vm.Stop();
        }
    }
}
