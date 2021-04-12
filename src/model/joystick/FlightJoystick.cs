using System.ComponentModel;
using Adv_Prog_2.model.data;

namespace Adv_Prog_2.model.joystick
{
    class FlightJoystick : IJoystick
    {
        // joystick pad has a constant width in the view
        private const int JOYSTICK_SIZE = 120;
        private float padRadius;
        private float knobRadius;
        private float center;

        private IFileIterator fileIterator;
        private DataAnalyzer dataAnalyzer;

        public float KnobX { get; private set; }
        public float KnobY { get; private set; }
        public float Throttle1 { get; private set; } = 0;
        public float Throttle2 { get; private set; } = 0;
        public float Rudder { get; private set; } = -1;

        #region property_changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion

        #region ctor
        public FlightJoystick(IFileIterator fileIterator, DataAnalyzer dataAnalyzer)
        {
            this.fileIterator = fileIterator;
            this.dataAnalyzer = dataAnalyzer;

            // reposition the knob when we read new data
            fileIterator.OnLineChanged += Update;

            // init size and position
            // radius is half the width (diameter)
            padRadius = JOYSTICK_SIZE / 2;
            knobRadius = padRadius / 2;
            center = padRadius - knobRadius;
            // set the initial position to the center
            KnobX = KnobY = center;
        }
        #endregion

        private void UpdateKnobPosition()
        {
            int lineNumber = fileIterator.LineNumber;
            float aileron = dataAnalyzer.GetValue("aileron", lineNumber);
            float elevator = dataAnalyzer.GetValue("elevator", lineNumber);

            float valueRange = padRadius - knobRadius; 

            KnobX = (valueRange * aileron) + center;
            // multiply by -1 because we want the knob to be drawn higher as y grows
            KnobY = (-1 * valueRange * elevator) + center;
            NotifyPropertyChanged("KnobX");
            NotifyPropertyChanged("KnobY");
        }

        public void UpdateSliders()
        {
            // get the current line we're reading
            int lineNumber = fileIterator.LineNumber;
            // fetch the relevant values
            Throttle1 = dataAnalyzer.GetValue("throttle1", lineNumber);
            Throttle2 = dataAnalyzer.GetValue("throttle2", lineNumber);
            Rudder = dataAnalyzer.GetValue("rudder", lineNumber);
            // notify the view
            NotifyPropertyChanged("Throttle1");
            NotifyPropertyChanged("Throttle2");
            NotifyPropertyChanged("Rudder");
        }

        private void Update()
        {
            UpdateKnobPosition();
            UpdateSliders();
        }
    }
}
