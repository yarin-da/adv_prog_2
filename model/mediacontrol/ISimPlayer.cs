using System.ComponentModel;

namespace Adv_Prog_2
{
    // simulation player
    internal interface ISimPlayer : INotifyPropertyChanged
    {
        public bool IsPlaying { get; }
        public int FrameCount { get; set; }
        public string TimerString { get; set; }
        public string SpeedString { get; set; }
        public float Speed { get; set; }
        public int Frame { get; set; }

        public void Play();
        public void Pause();
        public void Stop();
        public void FastForward();
        public void FastBackwards();
        public void FrameForward();
        public void FrameBackwards();

        public void LoadFile(string filePath);
    }
}
