using System.Windows.Input;
using Adv_Prog_2.viewmodel.relaycmd;

namespace Adv_Prog_2.viewmodel
{
    class MediaControlViewModel : BaseViewModel
    {
        #region ctor
        public MediaControlViewModel ()
        {
            VM_PlayCommand = new RelayCommand(null, param => model.Play());
            VM_PauseCommand = new RelayCommand(null, param => model.Pause());
            VM_StopCommand = new RelayCommand(null, param => model.Stop());
            VM_FastForwardCommand = new RelayCommand(null, param => model.SendMediaControl("FastForward"));
            VM_FastBackwardsCommand = new RelayCommand(null, param => model.SendMediaControl("FastBackwards"));
            VM_FrameForwardCommand = new RelayCommand(null, param => model.SendMediaControl("FrameForward"));
            VM_FrameBackwardsCommand = new RelayCommand(null, param => model.SendMediaControl("FrameBackwards"));
        }
        #endregion

        #region properties
        public ICommand VM_PlayCommand { get; private set; }
        public ICommand VM_PauseCommand { get; private set; }
        public ICommand VM_StopCommand { get; private set; }
        public ICommand VM_FastForwardCommand { get; private set; }
        public ICommand VM_FastBackwardsCommand { get; private set; }
        public ICommand VM_FrameForwardCommand { get; private set; }
        public ICommand VM_FrameBackwardsCommand { get; private set; }

        public int VM_Frame
        {
            get { return model.Frame; }
            set { model.Frame = value; }
        }
        public int VM_FrameCount
        {
            get { return model.FrameCount; }
        }
        public string VM_MaxTimerString { 
            get { return model.MaxTimerString; }
        }
        public string VM_TimerString
        {
            get { return model.TimerString; }
        }
        public string VM_SpeedString
        {
            get { return model.SpeedString; }
            set { model.SpeedString = value; }
        }
        public float VM_Speed
        {
            get { return model.Speed; }
        }
        public bool VM_Connected
        {
            get { return model.IsConnected; }
        }
        #endregion
    }
}
