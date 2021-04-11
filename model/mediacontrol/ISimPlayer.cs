using System.ComponentModel;

namespace Adv_Prog_2.model.mediacontrol
{
    // simulation player
    internal interface ISimPlayer : INotifyPropertyChanged
    {
        public bool IsPlaying { get; }
        public int FrameCount { get; set; }
        public string MaxTimerString { get; }
        public string TimerString { get; set; }
        public string SpeedString { get; set; }
        public float Speed { get; set; }
        public int Frame { get; set; }

        public void SendControl(string control);
        public void LoadFile(string filePath);
    }
}
