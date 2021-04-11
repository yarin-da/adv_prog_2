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

        private string csvFileName = null;
        private string[] titles = null;
        private IntPtr timeseries = IntPtr.Zero;
        private StringBuilder buffer;

        // determines if we have enough data to create TimeSeries/Detector objects
        private bool HasData { get { return titles != null && csvFileName != null; } }

        #region ctor
        public DataAnalyzer()
        {
            // create a buffer large enough to contain every possible feature title
            const int BUFFER_SIZE = 1024;
            buffer = new StringBuilder(BUFFER_SIZE);
        }
        #endregion

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
            if (timeseries != IntPtr.Zero)
            {
                return Analysis_GetMin(timeseries, feature);
            }
            return 0;
        }

        // return the maximum value of 'feature'
        public float GetMax(string feature)
        {
            if (timeseries != IntPtr.Zero)
            {
                return Analysis_GetMax(timeseries, feature);
            }
            return 0;
        }

        public float GetValue(string title, int index)
        {
            // return a default value (0) if we're missing data
            if (timeseries == IntPtr.Zero || String.IsNullOrEmpty(title)) { return 0; }
            return Analysis_GetValue(timeseries, title, index);
        }

        public string GetCorrelatedColumn(string title)
        {
            if (timeseries != IntPtr.Zero)
            {
                // write the feature's title into buffer
                Analysis_GetCorrelatedColumn(buffer, timeseries, title);
                return buffer.ToString();
            }
            return null;
        }

        public void SetMetaData(List<string> titlesList)
        {
            titles = StringListToArr(titlesList);
            // load timeseries object (or do nothing if we're missing data)
            LoadTimeSeries();
        }

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

        public void SetFlightData(string csvFileName)
        {
            this.csvFileName = csvFileName;
            // load timeseries object (or do nothing if we're missing data)
            LoadTimeSeries();
        }

        private void LoadTimeSeries()
        {
            if (HasData)
            {
                // destroy resources if they're already loaded
                DestroyResources();
                // load new resources
                timeseries = Analysis_CreateTimeSeries(csvFileName, titles, titles.Length);
            }
        }

        public void DestroyResources()
        {
            // destroy an object if it's not null, and set it to null after
            if (timeseries != IntPtr.Zero)
            {
                Analysis_DestroyTimeSeries(timeseries);
                timeseries = IntPtr.Zero;
            }
        }
    }
}
