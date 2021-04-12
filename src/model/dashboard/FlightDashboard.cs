using System.ComponentModel;
using Adv_Prog_2.model.data;

namespace Adv_Prog_2.model.dashboard
{
    class FlightDashboard : IDashboard
    {
        #region property_changed
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion

        private IFileIterator fileIterator;
        private DataAnalyzer dataAnalyzer;

        #region ctor
        public FlightDashboard(IFileIterator fileIterator, DataAnalyzer dataAnalyzer)
        {
            this.fileIterator = fileIterator;
            this.dataAnalyzer = dataAnalyzer;

            // update the properties' values everytime we read new data
            this.fileIterator.OnLineChanged += Update;
        }
        #endregion

        public float Altimeter { get; private set; } = 0;
        public float Airspeed { get; private set; } = 0;
        public float Direction { get; private set; } = 0;
        public float Yaw { get; private set; } = 0;
        public float Pitch { get; private set; } = 0;
        public float Roll { get; private set; } = 0;

        private void Update()
        {
            // fetch the current values of the features
            // update the properties
            int line = fileIterator.LineNumber;
            Altimeter = dataAnalyzer.GetValue("altitude-ft", line);
            Airspeed = dataAnalyzer.GetValue("airspeed-kt", line);
            Direction = NormalizeDegree(dataAnalyzer.GetValue("heading-deg", line));
            Yaw = NormalizeDegree(dataAnalyzer.GetValue("side-slip-deg", line));
            Pitch = NormalizeDegree(dataAnalyzer.GetValue("pitch-deg", line));
            Roll = NormalizeDegree(dataAnalyzer.GetValue("roll-deg", line));
            
            // notify the view that the properties have been updated
            NotifyPropertyChanged("Altimeter");
            NotifyPropertyChanged("Airspeed");
            NotifyPropertyChanged("Direction");
            NotifyPropertyChanged("Yaw");
            NotifyPropertyChanged("Pitch");
            NotifyPropertyChanged("Roll");
        }

        // takes any degree ('value')
        // returns the corresponding degree in the range [0,360)
        public float NormalizeDegree(float value)
        {
            if (value < 0)
            {
                int k = (int)(value / 360) + 1;
                value += k * 360;
            }
            return value % 360;
        }

    }
}
