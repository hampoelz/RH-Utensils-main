using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Main.Wpf.Utilities
{
    public static class MessageHelper
    {
        public const int WM_COPYDATA = 0x004A;

        [DllImport("user32", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(IntPtr Hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public static void SendDataMessage(Process targetProcess, string msg)
        {
            //Copy the string message to a global memory area in unicode format
            IntPtr _stringMessageBuffer = Marshal.StringToHGlobalUni(msg);

            //Prepare copy data structure
            COPYDATASTRUCT _copyData = new COPYDATASTRUCT
            {
                dwData = IntPtr.Zero,
                lpData = _stringMessageBuffer,
                cbData = msg.Length * 2//Number of bytes required for marshalling this string as a series of unicode characters
            };
            IntPtr _copyDataBuff = IntPtrAlloc(_copyData);

            //Send message to the other process
            SendMessage(targetProcess.MainWindowHandle, WM_COPYDATA, IntPtr.Zero, _copyDataBuff);

            Marshal.FreeHGlobal(_copyDataBuff);
            Marshal.FreeHGlobal(_stringMessageBuffer);
        }

        public static void SendDataBroadcastMessage(string msg)
        {
            foreach ((Process proc, int id) p in MainWindow.backgroundProcesses)
            {
                SendDataMessage(p.proc, msg);
            }
        }

        private static IntPtr IntPtrAlloc<T>(T param)
        {
            IntPtr retval = Marshal.AllocHGlobal(Marshal.SizeOf(param));
            Marshal.StructureToPtr(param, retval, false);
            return retval;
        }

        public static void ReceiveDataMessages()
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            var hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(mw).Handle);
            hwndSource.AddHook(new HwndSourceHook(WndProc));
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == MessageHelper.WM_COPYDATA)
            {
                COPYDATASTRUCT _dataStruct = Marshal.PtrToStructure<COPYDATASTRUCT>(lParam);

                string _msg = Marshal.PtrToStringUni(_dataStruct.lpData, _dataStruct.cbData / 2);

                MessageHelper.Handle(_msg);
            }

            return IntPtr.Zero;
        }

        private static async void Handle(string msg)
        {
            await Task.Delay(50);

            if (msg.StartsWith("set Color"))
            {
                await XmlHelper.SetString(Config.File, "config/color", ExtractQuote(msg));
                Config.Informations.Extension.Color = ExtractQuote(msg);
            }
            else if (msg.StartsWith("set tmpColor"))
            {
                Config.Informations.Extension.Color = ExtractQuote(msg);
            }
            else if (msg.StartsWith("set Favicon"))
            {
                await XmlHelper.SetString(Config.File, "config/favicon", ExtractQuote(msg));
                Config.Informations.Extension.Favicon = ExtractQuote(msg);
            }
            else if (msg.StartsWith("set WindowHeight"))
            {
                await XmlHelper.SetString(Config.File, "config/height", ExtractQuote(msg));
                Config.Informations.Extension.WindowHeight = int.Parse(ExtractQuote(msg));
            }
            else if (msg.StartsWith("set tmpWindowHeight"))
            {
                Config.Informations.Extension.WindowHeight = int.Parse(ExtractQuote(msg));
            }
            else if (msg.StartsWith("set WindowWidth"))
            {
                await XmlHelper.SetString(Config.File, "config/width", ExtractQuote(msg));
                Config.Informations.Extension.WindowWidth = int.Parse(ExtractQuote(msg));
            }
            else if (msg.StartsWith("set tmpWindowWidth"))
            {
                Config.Informations.Extension.WindowWidth = int.Parse(ExtractQuote(msg));
            }
            else if (msg.StartsWith("set selectionIndex"))
            {
                var index = int.Parse(ExtractQuote(msg));

                await XmlHelper.SetString(Config.File, "config/selectionIndex", (index).ToString());

                await MenuHelper.SelectMenuItemAsync(index - 1).ConfigureAwait(false);
            }
            else if (msg.StartsWith("add Site"))
            {
                var title = ExtractQuotes(msg)[0];
                var icon = ExtractQuotes(msg)[1];
                var path = ExtractQuotes(msg)[2];
                var startArguments = ExtractQuotes(msg)[3];

                //To-Do
            }
            else if (msg.StartsWith("update Site"))
            {
                var index = int.Parse(ExtractQuotes(msg)[0]);
                var title = ExtractQuotes(msg)[1];
                var icon = ExtractQuotes(msg)[2];

                //To-Do
            }
            else if (msg.StartsWith("remove Site"))
            {
                var index = int.Parse(ExtractQuotes(msg)[0]);

                //To-Do
            }
            else if (msg.StartsWith("change SettingProperty"))
            {
                var parameter = ExtractQuotes(msg)[0];
                var newValue = ExtractQuotes(msg)[1];

                Config.Settings.Json = JsonHelper.ChangeValue(Config.Settings.Json, parameter, newValue);
            }
            else if (msg.StartsWith("write LogFile string"))
            {
                LogFile.WriteLog(ExtractQuote(msg));
            }
            else if (msg.StartsWith("write LogFile exception"))
            {
                var exception = new Exception(ExtractQuote(msg));

                LogFile.WriteLog(exception);
            }
            else if (msg.StartsWith("open File"))
            {
                if (!(Application.Current.MainWindow is MainWindow mw)) return;

                ExtensionsManager.FileToOpen = ExtractQuote(msg);
                SendDataBroadcastMessage(msg);
            }
        }

        private static string ExtractQuote(string str)
        {
            var reg = new Regex("\".*\"");
            var match = reg.Matches(str);
            foreach (var item in match)
            {
                return item.ToString().Trim('"');
            }

            return "";
        }

        private static List<string> ExtractQuotes(string str)
        {
            List<string> Quotes = new List<string>();

            var reg = new Regex("\".*?\"");
            var matches = reg.Matches(str);
            foreach (var item in matches)
            {
                Quotes.Add(item.ToString().Trim('"'));
            }

            return Quotes;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct COPYDATASTRUCT
    {
        public IntPtr dwData;    // Any value the sender chooses.  Perhaps its main window handle?
        public int cbData;       // The count of bytes in the message.
        public IntPtr lpData;    // The address of the message.
    }
}
