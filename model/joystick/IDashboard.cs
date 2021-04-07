using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Adv_Prog_2.model.joystick_and_dashboard
{
    interface IDashboard : INotifyPropertyChanged
    {
        public float Altimeter { get; }
        public float Airspeed { get; }
        public float Direction { get; }
        public float Yaw { get; }
        public float Pitch { get; }
        public float Roll { get; }
        public float AltimeterMax { get; }
        public float AirspeedMax { get; }
        public float DirectionMax { get; }
        public float YawMax { get; }
        public float PitchMax { get; }
        public float RollMax { get; }
        public float AltimeterMin { get; }
        public float AirspeedMin { get; }
        public float DirectionMin { get; }
        public float YawMin { get; }
        public float PitchMin { get; }
        public float RollMin { get; }
    }
}
