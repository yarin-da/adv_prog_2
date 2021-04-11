using System;
using System.Diagnostics;

namespace Adv_Prog_2.viewmodel
{
    class DashboardViewModel : BaseViewModel
    {
        #region trigo_calculations
        public const int MAX_DEG = 360;
        public const int MIN_DEG = 0;
        public const int OFFSET_DEG = 135;
        public const int RADIUS = 40;

        // the data we get is in degrees
        // thus we must convert everything to radians
        private double ToRadians(double angleDeg)
        {
            return angleDeg * Math.PI / 180;
        }

        // calculate the x value using 'angleDeg'
        private double GetX(double angleDeg)
        {
            double angleRad = ToRadians(angleDeg + OFFSET_DEG);
            return RADIUS * Math.Cos(angleRad);
        }

        // calculate the y value using 'angleDeg'
        private double GetY(double angleDeg)
        {
            double angleRad = ToRadians(angleDeg + OFFSET_DEG);
            return RADIUS * Math.Sin(angleRad);
        }
        #endregion

        // format the values into representable strings
        private string ValueToString(float val, string ext)
        {
            return String.Format("{0:0.000}{1}", val, ext);
        }
        
        #region properties
        public string VM_Altimeter
        {
            get { return ValueToString(model.Altimeter, " ft."); } 
        }

        public string VM_Airspeed 
        { 
            get { return ValueToString(model.Airspeed, " kt."); } 
        }

        public string VM_Direction
        {
            get {
                NotifyPropertyChanged("VM_DirectionX");
                NotifyPropertyChanged("VM_DirectionY");
                return ValueToString(model.Direction, "º"); 
            }
        }

        public double VM_DirectionX { get { return GetX(model.Direction); } }
        public double VM_DirectionY { get { return GetY(model.Direction); } }

        public string VM_Pitch
        {
            get
            {
                NotifyPropertyChanged("VM_PitchX");
                NotifyPropertyChanged("VM_PitchY");
                return ValueToString(model.Pitch, "º");
            }
        }
        
        public double VM_PitchX { get { return GetX(model.Pitch); } }
        public double VM_PitchY { get { return GetY(model.Pitch); } }

        public string VM_Yaw 
        {
            get
            {
                NotifyPropertyChanged("VM_YawX");
                NotifyPropertyChanged("VM_YawY");
                return ValueToString(model.Yaw, "º");
            }
        }
        
        public double VM_YawX { get { return GetX(model.Yaw); } }
        public double VM_YawY { get { return GetY(model.Yaw); } }

        public string VM_Roll
        {
            get
            {
                NotifyPropertyChanged("VM_RollX");
                NotifyPropertyChanged("VM_RollY");
                return ValueToString(model.Roll, "º");
            }
        }
        
        public double VM_RollX { get { return GetX(model.Roll); } }
        public double VM_RollY { get { return GetY(model.Roll); } }
        #endregion
    }
}
