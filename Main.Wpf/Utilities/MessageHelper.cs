using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Main.Wpf.Utilities
{
    static class MessageHelper
    {
        public const int WM_COPYDATA = 0x004A;

        [DllImport("user32", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(IntPtr Hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public static void SendDataMessage(Process targetProcess, string msg)
        {
            //Copy the string message to a global memory area in unicode format
            IntPtr _stringMessageBuffer = Marshal.StringToHGlobalUni(msg);

            //Prepare copy data structure
            COPYDATASTRUCT _copyData = new COPYDATASTRUCT();
            _copyData.dwData = IntPtr.Zero;
            _copyData.lpData = _stringMessageBuffer;
            _copyData.cbData = msg.Length * 2;//Number of bytes required for marshalling this string as a series of unicode characters
            IntPtr _copyDataBuff = IntPtrAlloc(_copyData);

            //Send message to the other process
            SendMessage(targetProcess.MainWindowHandle, WM_COPYDATA, IntPtr.Zero, _copyDataBuff);

            Marshal.FreeHGlobal(_copyDataBuff);
            Marshal.FreeHGlobal(_stringMessageBuffer);
        }

        // Allocate a pointer to an arbitrary structure on the global heap.
        private static IntPtr IntPtrAlloc<T>(T param)
        {
            IntPtr retval = Marshal.AllocHGlobal(Marshal.SizeOf(param));
            Marshal.StructureToPtr(param, retval, false);
            return retval;
        }

        public static void Handle(string msg)
        {
            //Tp-Do
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct COPYDATASTRUCT
    {
        public IntPtr dwData;    // Any value the sender chooses.  Perhaps its main window handle?
        public int cbData;       // The count of bytes in the message.
        public IntPtr lpData;    // The address of the message.
    }
}
