using System.Windows.Input;
using System.Windows.Media;

namespace Adv_Prog_2.viewmodel
{
    class SessionSetupViewModel : BaseViewModel
    {
        public SessionSetupViewModel()
        {
            VM_ConnectCommand = new RelayCommand(null, param => model.ToggleConnection());
        }

        public string VM_ConnectButtonText { get { return model.ConnectButtonText; } }
        public string VM_ConnectStatus { get { return model.ConnectStatus; } }
        public SolidColorBrush VM_ConnectStatusColor { get { return model.ConnectStatusColor; } }
        public string VM_FlightDataFileName { get { return model.FlightDataFileName; } }
        public SolidColorBrush VM_FlightDataFileNameColor { get { return model.FlightDataFileNameColor; } }
        public string VM_MetaDataFileName { get { return model.MetaDataFileName; } }
        public SolidColorBrush VM_MetaDataFileNameColor { get { return model.MetaDataFileNameColor; } }
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

        public void SetFlightData(string filePath)
        {
            model.SetFlightDataFile(filePath);
        }

        public void SetColumnData(string filePath)
        {
            model.SetColumnData(filePath);
        }
    }
}
