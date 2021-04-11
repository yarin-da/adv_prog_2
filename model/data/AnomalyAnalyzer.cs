using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Adv_Prog_2.model.data
{
    class AnomalyAnalyzer
    {
        #region delegate_functions
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr CreateTimeSeriesFunc(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            [MarshalAs(UnmanagedType.LPArray)] string[] titles,
            int titlesAmount);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr CreateDetectorFunc(IntPtr learnTs);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DestroyFunc(IntPtr obj);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr GetTimeStepsFunc(
            IntPtr detector,
            IntPtr detectTs,
            [MarshalAs(UnmanagedType.LPStr)] string feature,
            ref int size);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GetTimeStepAtFunc(
            IntPtr timeSteps,
            int index);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr GetFuncDataFunc(
            IntPtr detector,
            IntPtr detectTs,
            [MarshalAs(UnmanagedType.LPStr)] string feature,
            ref int size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate float GetFuncDataAtFunc(
            IntPtr funcData,
            int index);
        #endregion

        #region loaded_functions
        private CreateTimeSeriesFunc CreateTimeSeries = null;
        private DestroyFunc DestroyTimeSeries = null;
        private CreateDetectorFunc CreateDetector = null;
        private DestroyFunc DestroyDetector = null;
        private GetTimeStepsFunc GetTimeSteps = null;
        private GetTimeStepAtFunc GetTimeStepAt = null;
        private DestroyFunc DestroyTimeSteps = null; 
        private GetFuncDataFunc GetFuncData = null;
        private GetFuncDataAtFunc GetFuncDataAt = null;
        private DestroyFunc DestroyFuncData = null;
        #endregion

        #region data_caching
        private IntPtr learnTs = IntPtr.Zero;
        private IntPtr detectTs = IntPtr.Zero;
        private IntPtr detector = IntPtr.Zero;
        private string[] titles = null;
        private string learnFilePath = null;
        private string anomalyFilePath = null;
        #endregion

        private LibraryLoader libLoader;

        #region ctor
        public AnomalyAnalyzer(LibraryLoader libLoader)
        {
            this.libLoader = libLoader;
            // we want libLoader to notify us when a new DLL library is loaded
            libLoader.OnLibraryLoad += LoadFunctions;
        }
        #endregion

        // returns true if all of the relevant data has been loaded
        public bool HasData
        {
            get
            {
                return CreateTimeSeries != null &&
                       DestroyTimeSeries != null &&
                       CreateDetector != null &&
                       DestroyDetector != null &&
                       GetTimeSteps != null &&
                       GetTimeStepAt != null &&
                       DestroyTimeSteps != null &&
                       GetFuncData != null &&
                       GetFuncDataAt != null &&
                       DestroyFuncData != null &&
                       titles != null &&
                       anomalyFilePath != null &&
                       learnFilePath != null;
    }
        }

        // creates timeseries objects and a detector
        private void LoadDetectorAndAnomaliesVector()
        {
            // make sure everything that is required has already been loaded
            if (HasData)
            {
                // destroy previously allocated resources (if there are any)
                DestroyResources();
                // allocate new resources
                learnTs = CreateTimeSeries(learnFilePath, titles, titles.Length);
                detectTs = CreateTimeSeries(anomalyFilePath, titles, titles.Length);
                detector = CreateDetector(learnTs);
            }
        }

        public void SetFlightData(string filePath)
        {
            anomalyFilePath = filePath;
            // initialize the detector (or do nothing if data is still missing)
            LoadDetectorAndAnomaliesVector();
        }

        public void SetLearningData(string filePath)
        {
            learnFilePath = filePath;
            // initialize the detector (or do nothing if data is still missing)
            LoadDetectorAndAnomaliesVector();
        }

        public void SetMetaData(string[] titles)
        {
            this.titles = titles;
            // initialize the detector (or do nothing if data is still missing)
            LoadDetectorAndAnomaliesVector();
        }

        public float[] GetDataFunction(string feature)
        {
            if (detector != IntPtr.Zero && HasData)
            {
                int size = 0;
                // load data related to the algorithm's function
                // and convert the data to float[]
                IntPtr dataFunc = GetFuncData(detector, detectTs, feature, ref size);
                float[] arr = new float[size];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = GetFuncDataAt(dataFunc, i);
                }
                DestroyFuncData(dataFunc);
                return arr;
            }
            return null;
        }

        public int[] GetAnomalyTimeSteps(string feature)
        {
            if (HasData)
            {
                int size = 0;
                // load the anomaly timesteps of 'feature'
                // convert the data to int[]
                IntPtr timeSteps =
                    GetTimeSteps(detector, detectTs, feature, ref size);
                int[] arr = new int[size];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = GetTimeStepAt(timeSteps, i);
                }
                DestroyTimeSteps(timeSteps);
                return arr;
            }

            return null;
        }

        public void LoadFunctions()
        {
            IntPtr funcAddr;
            if (libLoader.LibraryLoaded)
            {
                // fetch an address of a function
                // set the corresponding field if funcAddr is not null
                funcAddr = libLoader.GetFuncAddr("Analysis_CreateTimeSeries");
                if (funcAddr != IntPtr.Zero)
                {
                    CreateTimeSeries = Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(CreateTimeSeriesFunc)) as CreateTimeSeriesFunc;
                }
                funcAddr = libLoader.GetFuncAddr("Analysis_DestroyTimeSeries");
                if (funcAddr != IntPtr.Zero)
                {
                    DestroyTimeSeries = Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(DestroyFunc)) as DestroyFunc;
                }
                funcAddr = libLoader.GetFuncAddr("Analysis_CreateDetector");
                if (funcAddr != IntPtr.Zero)
                {
                    CreateDetector = Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(CreateDetectorFunc)) as CreateDetectorFunc;
                }
                funcAddr = libLoader.GetFuncAddr("Analysis_DestroyDetector");
                if (funcAddr != IntPtr.Zero)
                {
                    DestroyDetector = Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(DestroyFunc)) as DestroyFunc;
                }
                funcAddr = libLoader.GetFuncAddr("Analysis_GetTimeSteps");
                if (funcAddr != IntPtr.Zero)
                {
                    GetTimeSteps = Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(GetTimeStepsFunc)) as GetTimeStepsFunc;
                }
                funcAddr = libLoader.GetFuncAddr("Analysis_GetTimeStepAt");
                if (funcAddr != IntPtr.Zero)
                {
                    GetTimeStepAt = Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(GetTimeStepAtFunc)) as GetTimeStepAtFunc;
                }
                funcAddr = libLoader.GetFuncAddr("Analysis_DestroyTimeSteps");
                if (funcAddr != IntPtr.Zero)
                {
                    DestroyTimeSteps = Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(DestroyFunc)) as DestroyFunc;
                }
                funcAddr = libLoader.GetFuncAddr("Analysis_GetFuncData");
                if (funcAddr != IntPtr.Zero)
                {
                    GetFuncData = Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(GetFuncDataFunc)) as GetFuncDataFunc;
                }
                funcAddr = libLoader.GetFuncAddr("Analysis_GetFuncDataAt");
                if (funcAddr != IntPtr.Zero)
                {
                    GetFuncDataAt = Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(GetFuncDataAtFunc)) as GetFuncDataAtFunc;
                }
                funcAddr = libLoader.GetFuncAddr("Analysis_DestroyFuncData");
                if (funcAddr != IntPtr.Zero)
                {
                    DestroyFuncData = Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(DestroyFunc)) as DestroyFunc;
                }
            }
            // initialize the detector (or do nothing if data is still missing)
            LoadDetectorAndAnomaliesVector();
        }

        public void DestroyResources()
        {
            // free any resource that we have allocated
            if (detector != IntPtr.Zero && DestroyDetector != null)
            {
                DestroyDetector(detector);
            }
            detector = IntPtr.Zero;
            if (learnTs != IntPtr.Zero && DestroyTimeSeries != null)
            {
                DestroyTimeSeries(learnTs);
            }
            learnTs = IntPtr.Zero;
            if (detectTs != IntPtr.Zero && DestroyTimeSeries != null)
            {
                DestroyTimeSeries(detectTs);
            }
            detectTs = IntPtr.Zero;
        }
    }
}
