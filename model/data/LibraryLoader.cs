using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Adv_Prog_2.model.data
{
    class LibraryLoader
    {
        #region native_ms_functions
        static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr LoadLibrary(string libname);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern int GetLastError();
        }
        #endregion

        // pointer to the DLL library we loaded
        private IntPtr handle;
        
        // an event to notify observers when a new library has been loaded
        public delegate void UpdateFunc();
        public event UpdateFunc OnLibraryLoad;

        public bool LibraryLoaded { get; private set; } = false;

        public void LoadLibrary(string filePath)
        {
            // save the current handle (it can be null)
            IntPtr oldHandle = handle;
            // load the library into the new handle
            handle = NativeMethods.LoadLibrary(filePath);
            if (handle != IntPtr.Zero)
            {
                // save old LibraryLoaded value
                // because we want to know if we must free an old library
                bool libraryAlreadyLoaded = LibraryLoaded;
                LibraryLoaded = true;
                // notify the classes that a new library has been loaded
                OnLibraryLoad?.Invoke();
                if (libraryAlreadyLoaded)
                {
                    // if a library was already loaded before - we must free it
                    FreeLibrary(oldHandle);
                }
                LibraryLoaded = true;
            }
            else
            {
                // if load failed - set the handle back into its old value
                handle = oldHandle;
            }
        }

        // get the address of a function called 'funcName' in our 'handle'
        public IntPtr GetFuncAddr(string funcName)
        {
            if (LibraryLoaded)
            {
                return NativeMethods.GetProcAddress(handle, funcName);
            }
            return IntPtr.Zero;
        }

        public void FreeLibrary(IntPtr handle)
        {
            // free the library (only if it's loaded)
            if (LibraryLoaded)
            {
                bool res = NativeMethods.FreeLibrary(handle);
                LibraryLoaded = false;
                if (!res)
                {
                    // failed to free the library
                }
            }
        }
    }
}
