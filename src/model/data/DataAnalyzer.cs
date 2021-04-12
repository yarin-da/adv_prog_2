using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Adv_Prog_2.model.data
{
    class DataAnalyzer
    {
        #region dll_imports
        [DllImport(@"AnalysisUtil.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Analysis_CreateTimeSeries(
            [MarshalAs(UnmanagedType.LPStr)] string fileName,
            [MarshalAs(UnmanagedType.LPArray)] string[] titles,
            int numOfTitles);

        [DllImport(@"AnalysisUtil.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Analysis_DestroyTimeSeries(IntPtr ts);

        [DllImport(@"AnalysisUtil.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Analysis_GetCorrelatedColumn(
            StringBuilder buf,
            IntPtr ts,
            [MarshalAs(UnmanagedType.LPStr)] string title);
        
        [DllImport(@"AnalysisUtil.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float Analysis_GetValue(
            IntPtr ts,
            [MarshalAs(UnmanagedType.LPStr)] string title,
            int lineNumber);

        [DllImport(@"AnalysisUtil.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float Analysis_GetMin(
            IntPtr ts,
            [MarshalAs(UnmanagedType.LPStr)] string title);

        [DllImport(@"AnalysisUtil.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float Analysis_GetMax(
            IntPtr ts,
            [MarshalAs(UnmanagedType.LPStr)] string title);
        #endregion

        private string flightFileName = null;
        private string learnFileName = null;
        private string[] titles = null;
        private IntPtr flightTs = IntPtr.Zero;
        private IntPtr learnTs = IntPtr.Zero;
        private StringBuilder buffer;

        #region ctor
        public DataAnalyzer()
        {
            // create a buffer large enough to contain every possible feature title
            const int BUFFER_SIZE = 1024;
            buffer = new StringBuilder(BUFFER_SIZE);
        }
        #endregion

        #region dll_wrappers
        // return an array which contains the values of 'title'
        // in the last 'amount' lines sent to the server
        // with the option to skip a few lines everytime
        // e.g. if skip=4, we'll get the values at lines: X, X+5, X+10, etc.
        public float[] GetLastValues(string title, int currLine, int amount, int skip)
        {
            // insert values from the range:
            // [currLine - amount * (skip + 1) + 1, currLine]
            float[] arr = new float[amount];
            // set lineNum to the beginning of the range
            int lineNum = currLine - amount * (skip + 1) + 1;
            for (int i = 0; i < amount; i++)
            {
                // if lineNum is too small - enter a default value (0)
                if (lineNum < 0)
                {
                    arr[i] = 0;
                }
                else
                {
                    arr[i] = GetValue(title, lineNum);
                }
                // advance by skipping 'skip' amount of lines
                lineNum = lineNum + skip + 1;
            }
            return arr;
        }

        // return the minimum value of 'feature'
        public float GetMin(string feature)
        {
            if (flightTs != IntPtr.Zero && !String.IsNullOrEmpty(feature))
            {
                return Analysis_GetMin(flightTs, feature);
            }
            return 0;
        }

        // return the maximum value of 'feature'
        public float GetMax(string feature)
        {
            if (flightTs != IntPtr.Zero && !String.IsNullOrEmpty(feature))
            {
                return Analysis_GetMax(flightTs, feature);
            }
            return 0;
        }

        public float GetValue(string title, int index)
        {
            if (flightTs != IntPtr.Zero && !String.IsNullOrEmpty(title)) {
                return Analysis_GetValue(flightTs, title, index);
            }
            // return a default value (0) if we're missing data
            return 0;
        }

        public string GetCorrelatedColumn(string title)
        {
            if (learnTs != IntPtr.Zero && !String.IsNullOrEmpty(title))
            {
                // write the feature's title into buffer
                Analysis_GetCorrelatedColumn(buffer, learnTs, title);
                return buffer.ToString();
            }
            return null;
        }
        #endregion

        #region data_handling
        // convert a List<string> to string[]
        public string[] StringListToArr(List<string> list)
        {
            string[] arr = new string[list.Count];
            int index = 0;
            foreach (string str in list)
            {
                arr[index] = str;
                index++;
            }
            return arr;
        }
        public void SetMetaData(List<string> titlesList)
        {
            if (titlesList == null) { return; }
            titles = StringListToArr(titlesList);
            // load timeseries object (or do nothing if we're missing data)
            SetLearningData(learnFileName);
            SetFlightData(flightFileName);
        }

        public void SetLearningData(string learnFileName)
        {
            // load timeseries object (or do nothing if we're missing data)
            if (String.IsNullOrEmpty(learnFileName)) { return; }
            this.learnFileName = learnFileName;
            // if we have enough data to load learnTs
            if (titles != null)
            {
                // if a learnTs object has been allocated - deallocate it
                if (learnTs != IntPtr.Zero)
                {
                    Analysis_DestroyTimeSeries(learnTs);
                    learnTs = IntPtr.Zero;
                }
                learnTs = Analysis_CreateTimeSeries(learnFileName, titles, titles.Length);
            }
        }

        public void SetFlightData(string flightFileName)
        {
            // load timeseries object (or do nothing if we're missing data)
            if (String.IsNullOrEmpty(flightFileName)) { return; }
            this.flightFileName = flightFileName;
            // if we have enough data to load flightTs
            if (titles != null)
            {
                // if a flightTs object has been allocated - deallocate it
                if (flightTs != IntPtr.Zero)
                {
                    Analysis_DestroyTimeSeries(flightTs);
                    flightTs = IntPtr.Zero;
                }
                flightTs = Analysis_CreateTimeSeries(flightFileName, titles, titles.Length);
            }
        }

        public void DestroyResources()
        {
            // destroy an object if it's not null, and set it to null after
            if (flightTs != IntPtr.Zero)
            {
                Analysis_DestroyTimeSeries(flightTs);
                flightTs = IntPtr.Zero;
            }
            if (learnTs != IntPtr.Zero)
            {
                Analysis_DestroyTimeSeries(learnTs);
                learnTs = IntPtr.Zero;
            }
        }
        #endregion
    }
}
