using System.ComponentModel;
using System.Diagnostics;
using Adv_Prog_2.model.joystick;

namespace Adv_Prog_2
{
    class FlightJoystick : IJoystick
    {
        private const int JOYSTICK_SIZE = 120;
        private float padRadius;
        private float knobRadius;
        private float knobX;
        private float knobY;
        private float center;
        private IFileIterator fileIterator;
        private DataAnalyzer dataAnalyzer;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public FlightJoystick(IFileIterator fileIterator, DataAnalyzer dataAnalyzer)
        {
            this.fileIterator = fileIterator;
            this.dataAnalyzer = dataAnalyzer;

            // reposition the knob when we read new data
            fileIterator.OnLineChanged += Update;

            // init size and position
            padRadius = JOYSTICK_SIZE / 2;
            knobRadius = padRadius / 2;
            center = padRadius - knobRadius;
            knobX = knobY = center;
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

        private void UpdateKnobPosition()
        {
            int lineNumber = fileIterator.LineNumber;
            float aileron = dataAnalyzer.GetValue("aileron", lineNumber);
            float elevator = dataAnalyzer.GetValue("elevator", lineNumber);

            float valueRange = padRadius - knobRadius; 

            KnobX = (valueRange * aileron) + center;
            // multiply by -1 because we want the knob to be drawn higher as y grows
            KnobY = (-1 * valueRange * elevator) + center;
        }

        public void UpdateSliders()
        {
            int lineNumber = fileIterator.LineNumber;
            Throttle1 = dataAnalyzer.GetValue("throttle1", lineNumber);
            Throttle2 = dataAnalyzer.GetValue("throttle2", lineNumber);
            Rudder = dataAnalyzer.GetValue("rudder", lineNumber);

            NotifyPropertyChanged("Throttle1");
            NotifyPropertyChanged("Throttle2");
            NotifyPropertyChanged("Rudder");
        }

        public float MaxThrottle { get; private set; } = 1;
        public float MinThrottle { get; private set; } = 0;
        public float Throttle1 { get; private set; } = 0;
        public float Throttle2 { get; private set; } = 0;
        public float MaxRudder { get; private set; } = 1;
        public float MinRudder { get; private set; } = -1;
        public float Rudder { get; private set; } = -1;

        private void Update()
        {
            UpdateKnobPosition();
            UpdateSliders();
        }
    }
}
