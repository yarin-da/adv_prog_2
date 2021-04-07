using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Adv_Prog_2.viewmodel
{
    class MediaControlViewModel : BaseViewModel
    {
        public MediaControlViewModel ()
        {
            VM_PlayCommand = new RelayCommand(null, param => model.Play());
            VM_PauseCommand = new RelayCommand(null, param => model.Pause());
            VM_StopCommand = new RelayCommand(null, param => model.Stop());
            VM_FastForwardCommand = new RelayCommand(null, param => model.FastForward());
            VM_FastBackwardsCommand = new RelayCommand(null, param => model.FastBackwards());
            VM_FrameForwardCommand = new RelayCommand(null, param => model.FrameForward());
            VM_FrameBackwardsCommand = new RelayCommand(null, param => model.FrameBackwards());
        }

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
            set { model.FrameCount = value; }
        }
        public string VM_TimerString
        {
            get { return model.TimerString; }
            set { model.TimerString = value; }
        }
        public string VM_SpeedString
        {
            get { return model.SpeedString; }
            set { model.SpeedString = value; }
        }
        public float VM_Speed
        {
            get { return model.Speed; }
            set { model.Speed = value; }
        }
        public bool VM_Connected
        {
            get { return model.IsConnected; }
        }
    }
}
