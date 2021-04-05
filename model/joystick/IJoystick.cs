using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Adv_Prog_2.model.joystick
{
    interface IJoystick : INotifyPropertyChanged
    {
        public float KnobX { get; }
        public float KnobY { get; }
    }
}
