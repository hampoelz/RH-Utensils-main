using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Main.Wpf.ExampleExtension.Pages;

namespace Main.Wpf.ExampleExtension.Utilities
{
    public static class MessageHelper
    {
        private const int WmCopydata = 0x004A;

        [DllImport("user32", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(IntPtr Hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public static void SendDataMessage(Process targetProcess, string msg)
        {
            var stringMessageBuffer = Marshal.StringToHGlobalUni(msg);

            var copyData = new Copydatastruct
            {
                dwData = IntPtr.Zero,
                lpData = stringMessageBuffer,
                cbData = msg.Length * 2
            };
            var copyDataBuff = IntPtrAlloc(copyData);

            SendMessage(targetProcess.MainWindowHandle, WmCopydata, IntPtr.Zero, copyDataBuff);

            Marshal.FreeHGlobal(copyDataBuff);
            Marshal.FreeHGlobal(stringMessageBuffer);
        }

        private static IntPtr IntPtrAlloc<T>(T param)
        {
            var retval = Marshal.AllocHGlobal(Marshal.SizeOf(param));
            Marshal.StructureToPtr(param, retval, false);
            return retval;
        }

        public static void ReceiveDataMessages()
        {
            if (!(Application.Current.MainWindow is MainWindow mw)) return;

            var hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(mw).Handle);
            hwndSource?.AddHook(WndProc);
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != WmCopydata) return IntPtr.Zero;
            var dataStruct = Marshal.PtrToStructure<Copydatastruct>(lParam);

            var _msg = Marshal.PtrToStringUni(dataStruct.lpData, dataStruct.cbData / 2);

            HandleMessages(_msg);

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
                        if (prop.Name.Equals(parameter, StringComparison.InvariantCultureIgnoreCase))
                            prop.SetValue(null, newValue);
                }
                else if (msg.StartsWith("open File"))
                {
                    Home.File.Text = ExtractQuote(msg);
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
            foreach (var item in match) return item.ToString().Trim('"');

            return "";
        }

        private static List<string> ExtractQuotes(string str)
        {
            var quotes = new List<string>();

            var reg = new Regex("\".*?\"");
            var matches = reg.Matches(str);
            foreach (var item in matches) quotes.Add(item.ToString().Trim('"'));

            return quotes;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Copydatastruct
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }
}