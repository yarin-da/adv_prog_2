using System.ComponentModel;

namespace Adv_Prog_2.model.dashboard
{
    interface IDashboard : INotifyPropertyChanged
    {
        public float Altimeter { get; }
        public float Airspeed { get; }
        public float Direction { get; }
        public float Yaw { get; }
        public float Pitch { get; }
        public float Roll { get; }
    }
}
