using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Adv_Prog_2
{
    class DataAnalyzer
    {
        // create a timeseries object
        [DllImport("StatisticAnalysis.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Analysis_TimeSeries(
            [MarshalAs(UnmanagedType.LPStr)] string fileName,
            [MarshalAs(UnmanagedType.LPArray)] string[] titles,
            int numOfTitles);
        
        // create a detector object
        [DllImport("StatisticAnalysis.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Analysis_Detector(IntPtr ts);

        // get the linear regression of column 'title'
        [DllImport("StatisticAnalysis.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Analysis_LinearReg(
            IntPtr detector,
            [MarshalAs(UnmanagedType.LPStr)] string title,
            ref float a,
            ref float b);

        // get the most correlated column to column 'title'
        [DllImport("StatisticAnalysis.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Analysis_GetCorrelatedColumn(
            StringBuilder buf,
            IntPtr ts,
            [MarshalAs(UnmanagedType.LPStr)] string title);
        
        // get the value in column 'title' and row 'lineNumber'
        [DllImport("StatisticAnalysis.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float Analysis_GetValue(
            IntPtr ts,
            [MarshalAs(UnmanagedType.LPStr)] string title,
            int lineNumber);

        [DllImport("StatisticAnalysis.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float Analysis_GetMin(
            IntPtr ts,
            [MarshalAs(UnmanagedType.LPStr)] string title);

        [DllImport("StatisticAnalysis.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float Analysis_GetMax(
            IntPtr ts,
            [MarshalAs(UnmanagedType.LPStr)] string title);

        [DllImport("StatisticAnalysis.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Analysis_DestroyTimeSeries(IntPtr ts);
        
        [DllImport("StatisticAnalysis.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Analysis_DestroyDetector(IntPtr detector);


        private string csvFileName = null;
        private string[] titles = null;
        private IntPtr timeseries = IntPtr.Zero;
        private IntPtr detector = IntPtr.Zero;
        private StringBuilder buffer;

        // determines if we have enough data to create TimeSeries/Detector objects
        private bool HasData { get { return titles != null && csvFileName != null; } }

        public DataAnalyzer()
        {
            // create a buffer large enough to contain every possible feature title
            const int BUFFER_SIZE = 1024;
            buffer = new StringBuilder(BUFFER_SIZE);
        }

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

        public float GetMin(string feature)
        {
            if (timeseries != IntPtr.Zero)
            {
                return Analysis_GetMin(timeseries, feature);
            }
            return 0;
        }

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

        public void GetLinearReg(string title, out float a, out float b)
        {
            if (timeseries != IntPtr.Zero)
            {
                // read a, b values into x, y
                float x = 0;
                float y = 0;
                Analysis_LinearReg(detector, title, ref x, ref y);
                // update the parameters
                a = x;
                b = y;
            }
            else 
            {
                // if timeseries is null simply set a, b to a default value (0)
                a = b = 0;
            }
        }

        public void SetMetaData(List<string> titlesList)
        {
            this.titles = StringListToArr(titlesList);
            if (HasData)
            {
                LoadTimeSeries();
            }
        }

        private string[] StringListToArr(List<string> list)
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
            if (HasData)
            {
                LoadTimeSeries();
            }
        }

        private void LoadTimeSeries()
        {
            // destroy resources if they're already loaded
            DestroyResources();
            // load new resources
            timeseries = Analysis_TimeSeries(csvFileName, titles, titles.Length);
            detector = Analysis_Detector(timeseries);
        }

        public void DestroyResources()
        {
            // destroy an object if it's not null, and set it to null after
            if (detector != IntPtr.Zero)
            {
                Analysis_DestroyDetector(detector);
                detector = IntPtr.Zero;
            }
            if (timeseries != IntPtr.Zero)
            {
                Analysis_DestroyTimeSeries(timeseries);
                timeseries = IntPtr.Zero;
            }
        }
    }
}
