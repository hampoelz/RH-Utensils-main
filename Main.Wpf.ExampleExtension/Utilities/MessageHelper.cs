using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Main.Wpf.ExampleExtension.Utilities
{
    public static class MessageHelper
    {
        private const int WM_COPYDATA = 0x004A;

        [DllImport("user32", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(IntPtr Hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public static void SendDataMessage(Process targetProcess, string msg)
        {
            IntPtr _stringMessageBuffer = Marshal.StringToHGlobalUni(msg);

            COPYDATASTRUCT _copyData = new COPYDATASTRUCT
            {
                dwData = IntPtr.Zero,
                lpData = _stringMessageBuffer,
                cbData = msg.Length * 2
            };
            IntPtr _copyDataBuff = IntPtrAlloc(_copyData);

            SendMessage(targetProcess.MainWindowHandle, WM_COPYDATA, IntPtr.Zero, _copyDataBuff);

            Marshal.FreeHGlobal(_copyDataBuff);
            Marshal.FreeHGlobal(_stringMessageBuffer);
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

                HandleMessages(_msg);
            }

            return IntPtr.Zero;
        }

        private static async void HandleMessages(string msg)
        {
            try
            {
                await Task.Delay(50);

                if (msg.StartsWith("set Name"))
                {
                    Config.Name = ExtractQuote(msg);
                }
                else if (msg.StartsWith("set Color"))
                {
                    Config.Color = ExtractQuote(msg);
                }
                else if (msg.StartsWith("set SettingProperty"))
                {
                    var parameter = ExtractQuotes(msg)[0];
                    var newValue = ExtractQuotes(msg)[1];

                    foreach (var prop in typeof(Settings).GetProperties())
                    {
                        if (prop.Name.Equals(parameter, StringComparison.InvariantCultureIgnoreCase))
                        {
                            prop.SetValue(null, newValue);
                        }
                    }
                }
                else if (msg.StartsWith("open File"))
                {
                    Pages.Home.File.Text = ExtractQuote(msg);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
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
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }
}