using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace Adv_Prog_2.viewmodel
{
    class SessionSetupViewModel : BaseViewModel
    {
        public static SolidColorBrush green = new SolidColorBrush(Colors.LightGreen);
        public static SolidColorBrush red = new SolidColorBrush(Colors.Red);
        public const string notConnectedString = "Not Connected";
        public const string connectionFailedString = "Connection Failed!";
        public const string connectedString = "Connection Established";
        public const string flightFileMissingString = "Missing Flight Data File (csv)";
        public const string anomalyFileMissingString = "Missing Anomaly Data File (csv)";
        public const string metaFileMissingString = "Missing Flight Data File (xml)";
        public const string connectButtonConnect = "Connect";
        public const string connectButtonDisconnect = "Disconnect";

        public SessionSetupViewModel()
        {
            VM_ConnectCommand = new RelayCommand(null, param => ToggleConnection());
            VM_UploadFlightData = new RelayCommand(null, param => UploadFlightData());
            VM_UploadMetaData = new RelayCommand(null, param => UploadMetaData());
            VM_UploadAnomalyData = new RelayCommand(null, param => UploadAnomalyData());
            VM_Help = new RelayCommand(null, param => Help());
        }

        public ICommand VM_Help { get; private set; }

        public void Help()
        {
            string message = "Copy your metadata xml file into {{FLIGHTGEAR_FOLDER}}/data/Protocol/\n\n";
            message += "Open FlightGear, Go to Settings and enter these commands at the bottom:\n";
            message += "--generic=socket,in,10,{IP_ADDRESS},{PORT},udp,{METADATA_FILE.xml} --fdm = null\n\n";
            message += "Make sure to connect, upload flight data and the xml meta data before you start the simulation\n";
            MessageBox.Show(message);
        }

        #region setup_files
        public string VM_FlightDataFileName { get; private set; } = flightFileMissingString;
        public SolidColorBrush VM_FlightDataFileNameColor { get; private set; } = red;
        public string VM_AnomalyDataFileName { get; private set; } = anomalyFileMissingString;
        public SolidColorBrush VM_AnomalyDataFileNameColor { get; private set; } = red;
        public string VM_MetaDataFileName { get; private set; } = metaFileMissingString;
        public SolidColorBrush VM_MetaDataFileNameColor { get; private set; } = red;
        
        public ICommand VM_UploadFlightData { get; private set; }
        public ICommand VM_UploadMetaData { get; private set; }
        public ICommand VM_UploadAnomalyData { get; private set; }

        private string GetFilename(string filePath)
        {
            return filePath.Substring(filePath.LastIndexOf("\\") + 1);
        }

        public string UploadFile(string type)
        {
            string filePath = "";
            // ask the user for a file (of a specific type)
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = String.Format("Text files (*.{0})|*.{0}|All files (*.*)|*.*", type);
            if (openFileDialog.ShowDialog() == true)
            {
                // get the path and extract the filename to update our label
                filePath = openFileDialog.FileName;
            }
            return filePath;
        }
        public void UploadFlightData()
        {
            // ask the user for a CSV file
            string filePath = UploadFile("csv");
            if (!String.IsNullOrEmpty(filePath))
            {
                // upload the file
                model.SetFlightData(filePath);
                // update the GUI to notify the user that the file was accepted
                VM_FlightDataFileName = GetFilename(filePath);
                VM_FlightDataFileNameColor = green;
                NotifyPropertyChanged("VM_FlightDataFileName");
                NotifyPropertyChanged("VM_FlightDataFileNameColor");
            }
        }

        public void UploadMetaData()
        {
            // ask the user for an XML file
            string filePath = UploadFile("xml");
            if (!String.IsNullOrEmpty(filePath))
            {
                // upload the file
                model.SetMetaData(filePath);
                // update the GUI to notify the user that the file was accepted
                VM_MetaDataFileName = GetFilename(filePath);
                VM_MetaDataFileNameColor = green;
                NotifyPropertyChanged("VM_MetaDataFileName");
                NotifyPropertyChanged("VM_MetaDataFileNameColor");
            }
        }

        public void UploadAnomalyData()
        {
            // ask the user for a CSV file
            string filePath = UploadFile("csv");
            if (!String.IsNullOrEmpty(filePath))
            {
                // upload the file
                model.SetAnomalyData(filePath);
                // update the GUI to notify the user that the file was accepted
                VM_AnomalyDataFileName = GetFilename(filePath);
                VM_AnomalyDataFileNameColor = green;
                NotifyPropertyChanged("VM_AnomalyDataFileName");
                NotifyPropertyChanged("VM_AnomalyDataFileNameColor");
            }
        }
        #endregion

        #region setup_connection
        public string VM_ServerPort
        {
            get { return model.ServerPort; }
            set { model.ServerPort = value; }
        }
        public string VM_ServerIP
        {
            get { return model.ServerIP; }
            set { model.ServerIP = value; }
        }

        public ICommand VM_ConnectCommand { get; private set; }
        public string VM_ConnectButtonText { get; private set; } = connectButtonConnect;
        public string VM_ConnectStatus { get; private set; } = notConnectedString;
        public SolidColorBrush VM_ConnectStatusColor { get; private set; } = red;

        private void ToggleConnection()
        {
            if (model.IsConnected)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }
        }

        private void Connect()
        {
            bool connectSuccess = model.Connect();
            VM_ConnectStatus = connectSuccess ? connectedString : connectionFailedString;
            VM_ConnectStatusColor = connectSuccess ? green : red;
            VM_ConnectButtonText = connectSuccess ? connectButtonDisconnect : connectButtonConnect;
            NotifyPropertyChanged("VM_ConnectStatus");
            NotifyPropertyChanged("VM_ConnectStatusColor");
            NotifyPropertyChanged("VM_ConnectButtonText");
        }

        private void Disconnect()
        {
            model.Disconnect();
            VM_ConnectStatus = notConnectedString;
            VM_ConnectStatusColor = red;
            VM_ConnectButtonText = connectButtonConnect;
            NotifyPropertyChanged("VM_ConnectStatus");
            NotifyPropertyChanged("VM_ConnectStatusColor");
            NotifyPropertyChanged("VM_ConnectButtonText");
        }
        #endregion
    }
}
