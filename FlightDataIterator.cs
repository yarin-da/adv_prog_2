using static Adv_Prog_2.IFileIterator;

namespace Adv_Prog_2
{
    class FlightDataIterator : IFileIterator
    {
        // mutex because LineNumber can be accessed from multiple threads
        private readonly object LineNumberLocker = new object();

        private int lineNum = 0;
        private string[] lines;
        
        // notify all listeners when line was changed
        public event CallbackFunc OnLineChanged;

        public void LoadFile(string filePath)
        {
            // read the CSV file into lines (each cell is a whole line)
            lines = System.IO.File.ReadAllLines(filePath);
        }

        public bool HasNext { 
            get 
            { 
                if (lines == null)
                {
                    return false;
                }
                return LineNumber < lines.Length - 1; 
            } 
        }
        
        public string CurrentLine { 
            get 
            { 
                if (lines == null)
                {
                    return "";
                }
                return lines[LineNumber]; 
            } 
        }

        public int LineCount { 
            get 
            {
                if (lines == null)
                {
                    return 0;
                }
                return lines.Length;
            }
        }

        public int LineNumber {
            get 
            {
                return lineNum;
            }
            set 
            {
                lock (LineNumberLocker)
                {
                    if (value >= 0 && lines != null && value < LineCount)
                    {
                        lineNum = value;
                    }
                }
                OnLineChangedUpdate();
            }
        }

        public void Next()
        {
            if (LineNumber < LineCount)
            {
                LineNumber++;
            }
        }

        private void OnLineChangedUpdate()
        {
            if (OnLineChanged != null)
            {
                OnLineChanged.Invoke();
            }
        }
    }
}
