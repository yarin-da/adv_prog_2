using System;
using System.Collections.Generic;
using System.Text;

namespace Adv_Prog_2
{
    class FlightDataIterator : IFileIterator
    {
        private readonly object LineNumberLocker = new object();
        private int lineNum = 0;
        private string[] lines;

        public void LoadFile(string filePath)
        {
            // read the CSV file into lines (each cell is a whole line)
            lines = System.IO.File.ReadAllLines(filePath);
        }

        public bool HasNext { 
            get { 
                if (lines == null)
                {
                    return false;
                }
                return LineNumber < lines.Length; 
            } 
        }
        
        public string CurrentLine { 
            get { 
                if (lines == null)
                {
                    return "";
                }
                return lines[LineNumber]; 
            } 
        }

        public int LineCount { 
            get {
                if (lines == null)
                {
                    return 0;
                }
                return lines.Length;
            }
        }

        public int LineNumber {
            get {
                lock (LineNumberLocker)
                {
                    return lineNum;
                }
            }
            set {
                lock (LineNumberLocker)
                {
                    if (value >= 0 && lines != null && value < LineCount)
                    {
                        lineNum = value;
                    }
                }
            }
        }

        public void Next()
        {
            if (LineNumber < LineCount)
            {
                LineNumber++;
            }
        }
    }
}
