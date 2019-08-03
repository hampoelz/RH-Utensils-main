using MaterialDesignThemes.Wpf;
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
                cbData = msg.Length * 2 //Number of bytes required for marshalling this string as a series of unicode characters
            };
            IntPtr _copyDataBuff = IntPtrAlloc(_copyData);

            //Send message to the other process
            SendMessage(targetProcess.MainWindowHandle, WM_COPYDATA, IntPtr.Zero, _copyDataBuff);

            Marshal.FreeHGlobal(_copyDataBuff);
            Marshal.FreeHGlobal(_stringMessageBuffer);
        }

        public static void SendDataBroadcastMessage(string msg)
        {
            foreach ((Process proc, int id) in MainWindow.backgroundProcesses)
            {
                SendDataMessage(proc, msg);
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

        private static bool _isHandling;

        private static async void Handle(string msg)
        {
            while (_isHandling) await Task.Delay(100);

            _isHandling = true;

            if (msg.StartsWith("set Color"))
            {
                await XmlHelper.SetString(Config.File, "config/color", ExtractQuote(msg));
                Config.Informations.Extension.Color = ExtractQuote(msg);
            }
            else if (msg.StartsWith("set tmp Color"))
            {
                Config.Informations.Extension.Color = ExtractQuote(msg);
            }
            else if (msg.StartsWith("set Favicon"))
            {
                await XmlHelper.SetString(Config.File, "config/favicon", ExtractQuote(msg));
                Config.Informations.Extension.Favicon = ExtractQuote(msg);
            }
            else if (msg.StartsWith("set tmp Favicon"))
            {
                Config.Informations.Extension.Favicon = ExtractQuote(msg);
            }
            else if (msg.StartsWith("set WindowHeight"))
            {
                await XmlHelper.SetString(Config.File, "config/height", ExtractQuote(msg));
                Config.Informations.Extension.WindowHeight = int.Parse(ExtractQuote(msg));
            }
            else if (msg.StartsWith("set tmp WindowHeight"))
            {
                Config.Informations.Extension.WindowHeight = int.Parse(ExtractQuote(msg));
            }
            else if (msg.StartsWith("set WindowWidth"))
            {
                await XmlHelper.SetString(Config.File, "config/width", ExtractQuote(msg));
                Config.Informations.Extension.WindowWidth = int.Parse(ExtractQuote(msg));
            }
            else if (msg.StartsWith("set tmp WindowWidth"))
            {
                Config.Informations.Extension.WindowWidth = int.Parse(ExtractQuote(msg));
            }
            else if (msg.StartsWith("set selectionIndex"))
            {
                if (int.TryParse(ExtractQuote(msg), out var index) && index - 1 >= 0 && index <= Config.Menu.Sites.Count)
                {
                    if (Config.Menu.Sites[index - 1].Space) index = 1;

                    await MenuHelper.SelectMenuItemAsync(index - 1);
                    await XmlHelper.SetString(Config.File, "config/selectionIndex", (index).ToString());
                }
                else
                {
                    await MenuHelper.SelectMenuItemAsync(0);
                }
            }
            else if (msg.StartsWith("set tmp selectionIndex"))
            {
                if (int.TryParse(ExtractQuote(msg), out var index) && index - 1 >= 0 && index <= Config.Menu.Sites.Count)
                {
                    if (Config.Menu.Sites[index - 1].Space) index = 1;

                    await MenuHelper.SelectMenuItemAsync(index - 1);
                }
                else
                {
                    await MenuHelper.SelectMenuItemAsync(0);
                }
            }
            else if (msg.StartsWith("add tmp Site"))
            {
                var title = ExtractQuotes(msg)[0];
                var icon = ExtractQuotes(msg)[1];
                var path = ExtractQuotes(msg)[2];
                var startArguments = ExtractQuotes(msg)[3];

                var sites = Config.Menu.Sites;

                if (title == "null" || path == "null") sites.Insert(sites.Count - 3, new MenuItem() { Space = true });
                else sites.Insert(sites.Count - 3, new MenuItem { Title = title, Icon = Enum.TryParse(icon, out PackIconKind Icon) ? Icon : PackIconKind.Application, Path = path, StartArguments = startArguments });

                await Config.Menu.SetSites(sites);
            }
            else if (msg.StartsWith("update tmp Site"))
            {
                var sites = Config.Menu.Sites;
                var title = ExtractQuotes(msg)[1];
                var icon = ExtractQuotes(msg)[2];

                var _currentIndex = Pages.Menu.ListViewMenu.SelectedIndex;

                if (int.TryParse(ExtractQuotes(msg)[0], out var index) && index - 1 >= 0 && index - 1 <= Config.Menu.Sites.Count - 3)
                {
                    sites[index - 1].Title = title;
                    sites[index - 1].Icon = Enum.TryParse(icon, out PackIconKind Icon) ? Icon : PackIconKind.Application;

                    if (index - 1 != _currentIndex)
                    {
                        var path = ExtractQuotes(msg)[3];
                        var startArguments = ExtractQuotes(msg)[4];

                        sites[index - 1].Path = path;
                        sites[index - 1].StartArguments = startArguments;
                    }
                }

                await Config.Menu.SetSites(sites);
            }
            else if (msg.StartsWith("remove tmp Site"))
            {
                var sites = Config.Menu.Sites;

                var _currentIndex = Pages.Menu.ListViewMenu.SelectedIndex;

                if (int.TryParse(ExtractQuote(msg), out var index) && index - 1 >= 0 && index - 1 <= Config.Menu.Sites.Count - 3 && index - 1 != _currentIndex)
                {
                    sites.RemoveAt(index - 1);
                }

                await Config.Menu.SetSites(sites);

                if (index - 1 == 0)
                {
                    _currentIndex = _currentIndex - index - 1;
                }
                else if (index - 1 < _currentIndex)
                {
                    _currentIndex--;
                }

                MenuHelper.MoveCursorMenu(_currentIndex);
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
                ExtensionsManager.FileToOpen = ExtractQuote(msg);
                SendDataBroadcastMessage(msg);
            }

            _isHandling = false;
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