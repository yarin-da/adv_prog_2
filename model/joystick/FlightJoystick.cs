using System.ComponentModel;
using Adv_Prog_2.model.joystick;

namespace Adv_Prog_2
{
    class FlightJoystick : IJoystick
    {
        private const int JOYSTICK_RADIUS = 60;
        private const int KNOB_RADIUS = 30;
        private float knobX = 30;
        private float knobY = 30;
        private IFileIterator fileIterator;
        private DataAnalyzer dataAnalyzer;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }

        public FlightJoystick(IFileIterator fileIterator, DataAnalyzer dataAnalyzer)
        {
            this.fileIterator = fileIterator;
            this.dataAnalyzer = dataAnalyzer;

            fileIterator.OnLineChanged += UpdateKnobPosition;
        }

        public float KnobX { 
            get { return knobX; } 
            set 
            { 
                knobX = value;
                NotifyPropertyChanged("KnobX");
            }
        }
        public float KnobY { 
            get { return knobY; }
            set 
            { 
                knobY = value;
                NotifyPropertyChanged("KnobY");
            }
        }

        public void UpdateKnobPosition()
        {
            int lineNumber = fileIterator.LineNumber;
            float aileron = dataAnalyzer.GetValue("aileron", lineNumber);
            float elevator = dataAnalyzer.GetValue("elevator", lineNumber);

            // float throttle = statistics.GetValue("throttle", lineNumber); // [0,1]
            // float rudder = statistics.GetValue("rudder", lineNumber); // [0,1]?
            const float VALUE_RANGE = JOYSTICK_RADIUS - KNOB_RADIUS; 

            KnobX = TranslateX(VALUE_RANGE * aileron);
            // multiply by -1 because we want the knob to be drawn higher as y grows
            KnobY = TranslateY(-1 * VALUE_RANGE * elevator);
        }

        private float TranslateX(float x)
        {
            const int CENTER_X = JOYSTICK_RADIUS - KNOB_RADIUS;
            return x + CENTER_X;
        }
        private float TranslateY(float y)
        {
            const int CENTER_Y = JOYSTICK_RADIUS - KNOB_RADIUS;
            return y + CENTER_Y;
        }
    }
}
