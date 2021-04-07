using System.ComponentModel;
using System.Diagnostics;

namespace Adv_Prog_2.model.joystick_and_dashboard
{
    class FlightDashboard : IDashboard
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private IFileIterator fileIterator;
        private DataAnalyzer dataAnalyzer;

        public FlightDashboard(IFileIterator fileIterator, DataAnalyzer dataAnalyzer)
        {
            this.fileIterator = fileIterator;
            this.dataAnalyzer = dataAnalyzer;

            this.fileIterator.OnLineChanged += Update;
        }

        public float Altimeter { get; private set; } = 0;
        public float AltimeterMin { get; private set; } = -14;
        public float AltimeterMax { get; private set; } = 700;
        public float Airspeed { get; private set; } = 0;
        public float AirspeedMin { get; private set; } = 0;
        public float AirspeedMax { get; private set; } = 100;
        public float Direction { get; private set; } = 0;
        public float DirectionMin { get; private set; } = 0;
        public float DirectionMax { get; private set; } = 360;
        public float Yaw { get; private set; } = 0;
        public float YawMin { get; private set; } = 0;
        public float YawMax { get; private set; } = 360;
        public float Pitch { get; private set; } = 0;
        public float PitchMin { get; private set; } = 0;
        public float PitchMax { get; private set; } = 360;
        public float Roll { get; private set; } = 0;
        public float RollMin { get; private set; } = 0;
        public float RollMax { get; private set; } = 360;


        private void Update()
        {
            int lineNumber = fileIterator.LineNumber;
            Altimeter = dataAnalyzer.GetValue("altitude-ft", lineNumber);
            Airspeed = dataAnalyzer.GetValue("airspeed-kt", lineNumber);
            Direction = ((int)dataAnalyzer.GetValue("heading-deg", lineNumber)) % 360;
            Yaw = ((int)dataAnalyzer.GetValue("side-slip-deg", lineNumber)) % 360;
            Pitch = ((int)dataAnalyzer.GetValue("pitch-deg", lineNumber)) % 360;
            Roll = ((int)dataAnalyzer.GetValue("roll-deg", lineNumber)) % 360;
            
            NotifyPropertyChanged("Altimeter");
            NotifyPropertyChanged("Airspeed");
            NotifyPropertyChanged("Direction");
            NotifyPropertyChanged("Yaw");
            NotifyPropertyChanged("Pitch");
            NotifyPropertyChanged("Roll");
        }

    }
}
