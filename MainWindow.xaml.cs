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
        SolidColorBrush green = new SolidColorBrush(Colors.LightGreen);
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
                    ButtonConnect.Content = "Disconnect";
                    LabelConnection.Content = "Connection Established";
                    LabelConnection.Foreground = green;
                }
                catch (Exception ex) { LabelConnection.Content = "Connection Failed!"; }
            }
            else {
                // update the GUI to notify the user that we're disconnected
                vm.Disconnect();
                ButtonConnect.Content = "Connect";
                LabelConnection.Content = "Not Connected";
                LabelConnection.Foreground = red;
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
            string filePath = UploadFile("csv", LabelCSV);
            if (!String.IsNullOrEmpty(filePath))
            {
                vm.SetFlightData(filePath);
            }
        }

        public string UploadFile(string type, Label label)
        {
            string filePath = "";
            // ask the user for a file (of a specific type)
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = String.Format("Text files (*.{0})|*.{0}|All files (*.*)|*.*", type);
            if (openFileDialog.ShowDialog() == true)
            {
                // get the path and extract the filename to update our label
                filePath = openFileDialog.FileName;
                string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                // update the GUI to notify the user that the file was accepted
                label.Content = fileName;
                label.Foreground = green;
            }
            return filePath;
        }

        public void UploadColumnData(object sender, RoutedEventArgs e)
        {
            // ask the user for an XML file
            string filePath = UploadFile("xml", LabelXML);
            if (!String.IsNullOrEmpty(filePath))
            {
                vm.SetColumnData(filePath);
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
