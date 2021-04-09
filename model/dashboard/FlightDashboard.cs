using System.ComponentModel;

namespace Adv_Prog_2.model.dashboard
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
        public float Airspeed { get; private set; } = 0;
        public float Direction { get; private set; } = 0;
        public float Yaw { get; private set; } = 0;
        public float Pitch { get; private set; } = 0;
        public float Roll { get; private set; } = 0;


        private void Update()
        {
            int line = fileIterator.LineNumber;
            Altimeter = dataAnalyzer.GetValue("altitude-ft", line);
            Airspeed = dataAnalyzer.GetValue("airspeed-kt", line);
            Direction = ToPositiveDegree(dataAnalyzer.GetValue("heading-deg", line));
            Yaw = ToPositiveDegree(dataAnalyzer.GetValue("side-slip-deg", line));
            Pitch = ToPositiveDegree(dataAnalyzer.GetValue("pitch-deg", line));
            Roll = ToPositiveDegree(dataAnalyzer.GetValue("roll-deg", line));
            
            NotifyPropertyChanged("Altimeter");
            NotifyPropertyChanged("Airspeed");
            NotifyPropertyChanged("Direction");
            NotifyPropertyChanged("Yaw");
            NotifyPropertyChanged("Pitch");
            NotifyPropertyChanged("Roll");
        }

        public float ToPositiveDegree(float value)
        {
            if (value < 0)
            {
                int k = (int)(value / 360) + 1;
                value += k * 360;
            }
            return value;
        }

    }
}
