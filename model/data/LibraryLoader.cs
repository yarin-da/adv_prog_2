using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Adv_Prog_2.model.data
{
    class LibraryLoader
    {
        static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr LoadLibrary(string libname);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern int GetLastError();
        }

        private IntPtr handle;

        public bool LibraryLoaded { get; private set; } = false;

        public void LoadLibrary(string filePath)
        {
            if (!LibraryLoaded)
            {
                handle = NativeMethods.LoadLibrary(filePath);
                LibraryLoaded = true;
            }
        }

        public IntPtr GetFuncAddr(IntPtr handle, string funcName)
        {
            if (LibraryLoaded)
            {
                return NativeMethods.GetProcAddress(handle, funcName);
            }
            return IntPtr.Zero;
        }

        public void FreeLibrary(IntPtr handle)
        {
            if (LibraryLoaded)
            {
                bool res = NativeMethods.FreeLibrary(handle);
                LibraryLoaded = false;
                if (!res)
                {
                    Trace.WriteLine("Failed to free DLL");
                }
            }
        }
    }
}
