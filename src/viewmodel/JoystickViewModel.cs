using System;
using System.Collections.Generic;
using System.Text;

namespace Adv_Prog_2.viewmodel
{
    class JoystickViewModel : BaseViewModel
    {
        #region properties
        public float VM_Throttle1 { get { return model.Throttle1; } }
        public float VM_Throttle2 { get { return model.Throttle2; } }
        public float VM_Rudder { get { return model.Rudder; } }
        public float VM_KnobX { get { return model.KnobX; } }
        public float VM_KnobY { get { return model.KnobY; } }
        #endregion
    }
}
