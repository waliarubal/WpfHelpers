using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace NullVoidCreations.WpfHelpers
{

    public class PlatformInvoke
    {

        #region pinvokes

        const uint INFINITE = 0xffffffff;

        [Flags]
        enum LogonFlags
        {
            LOGON_WITH_PROFILE = 0x00000001,
            LOGON_NETCREDENTIALS_ONLY = 0x00000002
        }

        [Flags]
        enum CreationFlags
        {
            CREATE_SUSPENDED = 0x00000004,
            CREATE_NEW_CONSOLE = 0x00000010,
            CREATE_NEW_PROCESS_GROUP = 0x00000200,
            CREATE_UNICODE_ENVIRONMENT = 0x00000400,
            CREATE_SEPARATE_WOW_VDM = 0x00000800,
            CREATE_DEFAULT_ERROR_MODE = 0x04000000,
        }

        [StructLayout(LayoutKind.Sequential)]
        struct ProcessInfo
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct StartupInfo
        {
            public int cb;
            public string reserved1;
            public string desktop;
            public string title;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public ushort wShowWindow;
            public short reserved2;
            public int reserved3;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        /// <summary>Enumeration of the different ways of showing a window using ShowWindow</summary>
        [Flags]
        enum WindowShowStyle : int
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,

            /// <summary>Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,

            /// <summary>Activates the window and displays it as a minimized window.</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,

            /// <summary>Activates the window and displays it as a maximized window.</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,

            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,

            /// <summary>Displays a window in its most recent size and position. This value is similar to "ShowNormal", except the window is not actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,

            /// <summary>Activates the window and displays it in its current size and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,

            /// <summary>Minimizes the specified window and activates the next top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,

            /// <summary>Displays the window as a minimized window. This value is similar to "ShowMinimized", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,

            /// <summary>Displays the window in its current size and position. This value is similar to "Show", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,

            /// <summary>Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,

            /// <summary>Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,

            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread that owns the window is hung. This flag should only be used when minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }

        [Flags]
        enum CSIDL
        {
            CSIDL_DESKTOP = 0x0000,    // <desktop>
            CSIDL_INTERNET = 0x0001,    // Internet Explorer (icon on desktop)
            CSIDL_PROGRAMS = 0x0002,    // Start Menu\Programs
            CSIDL_CONTROLS = 0x0003,    // My Computer\Control Panel
            CSIDL_PRINTERS = 0x0004,    // My Computer\Printers
            CSIDL_PERSONAL = 0x0005,    // My Documents
            CSIDL_FAVORITES = 0x0006,    // <user name>\Favorites
            CSIDL_STARTUP = 0x0007,    // Start Menu\Programs\Startup
            CSIDL_RECENT = 0x0008,    // <user name>\Recent
            CSIDL_SENDTO = 0x0009,    // <user name>\SendTo
            CSIDL_BITBUCKET = 0x000a,    // <desktop>\Recycle Bin
            CSIDL_STARTMENU = 0x000b,    // <user name>\Start Menu
            CSIDL_MYDOCUMENTS = 0x000c,    // logical "My Documents" desktop icon
            CSIDL_MYMUSIC = 0x000d,    // "My Music" folder
            CSIDL_MYVIDEO = 0x000e,    // "My Videos" folder
            CSIDL_DESKTOPDIRECTORY = 0x0010,    // <user name>\Desktop
            CSIDL_DRIVES = 0x0011,    // My Computer
            CSIDL_NETWORK = 0x0012,    // Network Neighborhood (My Network Places)
            CSIDL_NETHOOD = 0x0013,    // <user name>\nethood
            CSIDL_FONTS = 0x0014,    // windows\fonts
            CSIDL_TEMPLATES = 0x0015,
            CSIDL_COMMON_STARTMENU = 0x0016,    // All Users\Start Menu
            CSIDL_COMMON_PROGRAMS = 0X0017,    // All Users\Start Menu\Programs
            CSIDL_COMMON_STARTUP = 0x0018,    // All Users\Startup
            CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019,    // All Users\Desktop
            CSIDL_APPDATA = 0x001a,    // <user name>\Application Data
            CSIDL_PRINTHOOD = 0x001b,    // <user name>\PrintHood
            CSIDL_LOCAL_APPDATA = 0x001c,    // <user name>\Local Settings\Applicaiton Data (non roaming)
            CSIDL_ALTSTARTUP = 0x001d,    // non localized startup
            CSIDL_COMMON_ALTSTARTUP = 0x001e,    // non localized common startup
            CSIDL_COMMON_FAVORITES = 0x001f,
            CSIDL_INTERNET_CACHE = 0x0020,
            CSIDL_COOKIES = 0x0021,
            CSIDL_HISTORY = 0x0022,
            CSIDL_COMMON_APPDATA = 0x0023,    // All Users\Application Data
            CSIDL_WINDOWS = 0x0024,    // GetWindowsDirectory()
            CSIDL_SYSTEM = 0x0025,    // GetSystemDirectory()
            CSIDL_PROGRAM_FILES = 0x0026,    // C:\Program Files
            CSIDL_MYPICTURES = 0x0027,    // C:\Program Files\My Pictures
            CSIDL_PROFILE = 0x0028,    // USERPROFILE
            CSIDL_SYSTEMX86 = 0x0029,    // x86 system directory on RISC
            CSIDL_PROGRAM_FILESX86 = 0x002a,    // x86 C:\Program Files on RISC
            CSIDL_PROGRAM_FILES_COMMON = 0x002b,    // C:\Program Files\Common
            CSIDL_PROGRAM_FILES_COMMONX86 = 0x002c,    // x86 Program Files\Common on RISC
            CSIDL_COMMON_TEMPLATES = 0x002d,    // All Users\Templates
            CSIDL_COMMON_DOCUMENTS = 0x002e,    // All Users\Documents
            CSIDL_COMMON_ADMINTOOLS = 0x002f,    // All Users\Start Menu\Programs\Administrative Tools
            CSIDL_ADMINTOOLS = 0x0030,    // <user name>\Start Menu\Programs\Administrative Tools
            CSIDL_CONNECTIONS = 0x0031,    // Network and Dial-up Connections
            CSIDL_COMMON_MUSIC = 0x0035,    // All Users\My Music
            CSIDL_COMMON_PICTURES = 0x0036,    // All Users\My Pictures
            CSIDL_COMMON_VIDEO = 0x0037,    // All Users\My Video
            CSIDL_CDBURN_AREA = 0x003b    // USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning
        }

        [Flags]
        enum ConnectionStates
        {
            Modem = 0x1,
            LAN = 0x2,
            Proxy = 0x4,
            RasInstalled = 0x10,
            Offline = 0x20,
            Configured = 0x40,
        }

        [DllImport("wininet", SetLastError = true)]
        static extern bool InternetGetConnectedState(out int lpdwFlags, int dwReserved);

        [DllImport("shell32")]
        static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindow(string cls, string win);

        [DllImport("user32")]
        static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32")]
        static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32")]
        static extern bool OpenIcon(IntPtr hWnd);

        [DllImport("user32")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32")]
        static extern bool HideCaret(IntPtr hWnd);

        [DllImport("user32")]
        static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        static extern bool CreateProcessWithLogonW(
            string principal,
            string authority,
            string password,
            LogonFlags logonFlags,
            string appName,
            string cmdLine,
            CreationFlags creationFlags,
            IntPtr environmentBlock,
            string currentDirectory,
            ref StartupInfo startupInfo,
            out ProcessInfo processInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint WaitForSingleObject(IntPtr handle, uint milliseconds);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr h);

        #endregion

        #region properties

        public bool IsInternetAvailable
        {
            get
            {
                int flags;
                return InternetGetConnectedState(out flags, 0);
            }
        }

        #endregion

        #region private methods

        string GetSpecialDirectory(CSIDL specialFolder)
        {
            var path = new StringBuilder(260);
            SHGetSpecialFolderPath(IntPtr.Zero, path, (int)specialFolder, false);
            return path.ToString();
        }

        #endregion

        public bool RunProgram(string fileName, string arguments, string userName, string password)
        {
            var userParts = userName.Split('\\');
            if (userParts.Length != 2)
                return false;

            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists)
                return false;

            var startupInfo = new StartupInfo();
            startupInfo.cb = Marshal.SizeOf(typeof(StartupInfo));
            startupInfo.title = "NFS Workspace Addin Manager";

            var processInfo = new ProcessInfo();

            if (CreateProcessWithLogonW(
                userParts[1], // user name
                userParts[0], // domain name 
                password,
                LogonFlags.LOGON_WITH_PROFILE,
                fileInfo.FullName,
                arguments,
                0,
                IntPtr.Zero,
                fileInfo.DirectoryName,
                ref startupInfo,
                out processInfo))
            {
                WaitForSingleObject(processInfo.hProcess, INFINITE);

                CloseHandle(processInfo.hProcess);
                CloseHandle(processInfo.hThread);
                return true;
            }
            else
                return false;
        }

        public void ActivateOtherWindow(string title)
        {
            var other = FindWindow(null, title);
            if (other != IntPtr.Zero)
            {
                Show(other);
                if (IsIconic(other))
                    OpenIcon(other);
            }
        }

        public IntPtr GetHandle(Visual element)
        {
            var source = (HwndSource)HwndSource.FromVisual(element);
            return source.Handle;
        }

        public bool ToggleCaret(TextBox textBox, bool showCaret)
        {
            var handle = GetHandle(textBox);
            if (showCaret)
                return ShowCaret(handle);
            else
                return HideCaret(handle);
        }

        public IntPtr GetWindowHandle(Window window)
        {
            return new WindowInteropHelper(window).Handle;
        }

        public void Hide(IntPtr hWnd)
        {
            ShowWindow(hWnd, (int)WindowShowStyle.Hide);
        }

        public void Minimize(IntPtr hWnd)
        {
            ShowWindow(hWnd, (int)WindowShowStyle.Minimize);
        }

        public void Show(IntPtr hWnd)
        {
            ShowWindow(hWnd, (int)WindowShowStyle.Restore);
        }

        public string GetPublicDesktopDirectory()
        {
            return GetSpecialDirectory(CSIDL.CSIDL_COMMON_DESKTOPDIRECTORY);
        }

        public string GetStartMenuDirectory()
        {
            return GetSpecialDirectory(CSIDL.CSIDL_PROGRAMS);
        }

        public string SelectFolder(string description = "", Environment.SpecialFolder rootFolder = Environment.SpecialFolder.MyComputer)
        {
            using (var folderBrowser = new System.Windows.Forms.FolderBrowserDialog())
            {
                folderBrowser.Description = description;
                folderBrowser.RootFolder = rootFolder;
                if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return folderBrowser.SelectedPath;
            }

            return null;
        }

        public void CreateShortcut(string lnkPath, string executable, string arguments, string workingDirectory, string iconPath, bool isMinimized)
        {
            // delete existing link
            if (File.Exists(lnkPath))
                File.Delete(lnkPath);

            // Windows Script Host Shell Object
            var type = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")); 
            var shell = Activator.CreateInstance(type);
            try
            {
                var link = type.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell, new object[] { lnkPath });
                try
                {
                    type.InvokeMember("TargetPath", BindingFlags.SetProperty, null, link, new object[] { executable });
                    if (!string.IsNullOrEmpty(arguments))
                        type.InvokeMember("Arguments", BindingFlags.SetProperty, null, link, new object[] { arguments });
                    type.InvokeMember("IconLocation", BindingFlags.SetProperty, null, link, new object[] { iconPath });
                    type.InvokeMember("WindowStyle", BindingFlags.SetProperty, null, link, new object[] { isMinimized ? 7 : 1 });
                    type.InvokeMember("Save", BindingFlags.InvokeMethod, null, link, null);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    Marshal.FinalReleaseComObject(link);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Marshal.FinalReleaseComObject(shell);
            }
        }
    }
}
