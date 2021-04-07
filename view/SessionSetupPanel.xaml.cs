using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Adv_Prog_2.viewmodel;
using Microsoft.Win32;

namespace Adv_Prog_2
{
    public partial class SessionSetupPanel : UserControl
    {
        private SessionSetupViewModel vm;
        SolidColorBrush green = new SolidColorBrush(Colors.LightGreen);

        public SessionSetupPanel()
        {
            InitializeComponent();
            vm = new SessionSetupViewModel();
            DataContext = vm;
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
            string message = "Copy your small_playback.xml file into {{FLIGHTGEAR_FOLDER}}/data/Protocol/\n";
            // message += ;
            MessageBox.Show(message);
        }
    }
}
