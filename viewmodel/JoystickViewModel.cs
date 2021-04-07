using System;
using System.Collections.Generic;
using System.Text;

namespace Adv_Prog_2.viewmodel
{
    class JoystickViewModel : BaseViewModel
    {
        public float VM_MaxThrottle { get { return model.MaxThrottle; } }
        public float VM_MinThrottle { get { return model.MinThrottle; } }
        public float VM_MaxRudder { get { return model.MaxRudder; } }
        public float VM_MinRudder { get { return model.MinRudder; } }
        public float VM_Throttle1 { get { return model.Throttle1; } }
        public float VM_Throttle2 { get { return model.Throttle2; } }
        public float VM_Rudder { get { return model.Rudder; } }
        public float VM_KnobX { get { return model.KnobX; } }
        public float VM_KnobY { get { return model.KnobY; } }

    }
}
