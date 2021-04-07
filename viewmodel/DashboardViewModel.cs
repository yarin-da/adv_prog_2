namespace Adv_Prog_2.viewmodel
{
    class DashboardViewModel : BaseViewModel
    {
        public float VM_Altimeter { get { return model.Altimeter; } }
        public float VM_Airspeed { get { return model.Airspeed; } }
        public float VM_Direction { get { return model.Direction; } }
        public float VM_Yaw { get { return model.Yaw; } }
        public float VM_Pitch { get { return model.Pitch; } }
        public float VM_Roll { get { return model.Roll; } }
        public float VM_AltimeterMax { get { return model.AltimeterMax; } }
        public float VM_AirspeedMax { get { return model.AirspeedMax; } }
        public float VM_DirectionMax { get { return model.DirectionMax; } }
        public float VM_YawMax { get { return model.YawMax; } }
        public float VM_PitchMax { get { return model.PitchMax; } }
        public float VM_RollMax { get { return model.RollMax; } }
        public float VM_AltimeterMin { get { return model.AltimeterMin; } }
        public float VM_AirspeedMin { get { return model.AirspeedMin; } }
        public float VM_DirectionMin { get { return model.DirectionMin; } }
        public float VM_YawMin { get { return model.YawMin; } }
        public float VM_PitchMin { get { return model.PitchMin; } }
        public float VM_RollMin { get { return model.RollMin; } }
    }
}
