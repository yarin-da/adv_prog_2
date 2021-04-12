using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Adv_Prog_2.viewmodel.relaycmd;
using Microsoft.Win32;

namespace Adv_Prog_2.viewmodel
{
    class SessionSetupViewModel : BaseViewModel
    {
        #region constants
        public static SolidColorBrush green = new SolidColorBrush(Colors.LightGreen);
        public static SolidColorBrush red = new SolidColorBrush(Color.FromRgb(255, 128, 128));
        public const string notConnectedString = "Not Connected";
        public const string connectionFailedString = "Connection Failed!";
        public const string connectedString = "Connection Established";
        public const string learningFileMissingString = "Missing Learning Data File (csv)";
        public const string flightFileMissingString = "Missing Flight Data File (csv)";
        public const string metaFileMissingString = "Missing Flight Data File (xml)";
        public const string algoFileMissingString = "Missing Anomaly Algorithm (dll)";
        public const string connectButtonConnect = "Connect";
        public const string connectButtonDisconnect = "Disconnect";
        #endregion

        #region ctor
        public SessionSetupViewModel()
        {
            VM_ConnectCommand = new RelayCommand(null, param => ToggleConnection());
            VM_UploadFlightData = new RelayCommand(null, param => UploadFlightData());
            VM_UploadMetaData = new RelayCommand(null, param => UploadMetaData());
            VM_UploadLearningData = new RelayCommand(null, param => UploadLearningData());
            VM_UploadAlgorithmData = new RelayCommand(null, param => UploadAlgorithmData());
            VM_Help = new RelayCommand(null, param => Help());
        }
        #endregion

        #region help_button
        public ICommand VM_Help { get; private set; }
        public void Help()
        {
            string message = "Copy your metadata xml file into {{FLIGHTGEAR_FOLDER}}/data/Protocol/\n\n";
            message += "Open FlightGear, Go to Settings and enter these commands at the bottom:\n";
            message += "--generic=socket,in,10,{IP_ADDRESS},{PORT},udp,{METADATA_FILE.xml} --fdm = null\n\n";
            message += "Under the buttons, there are labels in red/green colors - make sure they are all green before you start the simulation\n\n";
            message += "The buttons are your friends - they will help you turn all of the labels green\n";
            MessageBox.Show(message);
        }
        #endregion

        #region setup_files
        public string VM_LearningDataFileName { get; private set; } = learningFileMissingString;
        public SolidColorBrush VM_LearningDataFileNameColor { get; private set; } = red;
        public string VM_FlightDataFileName { get; private set; } = flightFileMissingString;
        public SolidColorBrush VM_FlightDataFileNameColor { get; private set; } = red;
        public string VM_MetaDataFileName { get; private set; } = metaFileMissingString;
        public SolidColorBrush VM_MetaDataFileNameColor { get; private set; } = red;
        public string VM_AlgorithmDataFileName { get; private set; } = algoFileMissingString;
        public SolidColorBrush VM_AlgorithmDataFileNameColor { get; private set; } = red;

        public ICommand VM_UploadFlightData { get; private set; }
        public ICommand VM_UploadMetaData { get; private set; }
        public ICommand VM_UploadLearningData { get; private set; }
        public ICommand VM_UploadAlgorithmData { get; private set; }

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
        public void UploadLearningData()
        {
            // ask the user for a CSV file
            string filePath = UploadFile("csv");
            if (!String.IsNullOrEmpty(filePath))
            {
                // upload the file
                model.SetLearningData(filePath);
                // update the GUI to notify the user that the file was accepted
                VM_LearningDataFileName = GetFilename(filePath);
                VM_LearningDataFileNameColor = green;
                NotifyPropertyChanged("VM_LearningDataFileName");
                NotifyPropertyChanged("VM_LearningDataFileNameColor");
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
        public void UploadAlgorithmData()
        {
            // ask the user for a DLL file
            string filePath = UploadFile("dll");
            if (!String.IsNullOrEmpty(filePath))
            {
                // upload the file
                model.SetAlgorithmData(filePath);
                // update the GUI to notify the user that the file was accepted
                VM_AlgorithmDataFileName = GetFilename(filePath);
                VM_AlgorithmDataFileNameColor = green;
                NotifyPropertyChanged("VM_AlgorithmDataFileName");
                NotifyPropertyChanged("VM_AlgorithmDataFileNameColor");
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
