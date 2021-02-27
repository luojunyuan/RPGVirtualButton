﻿using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace RPGHelper
{
    class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        public const int PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool QueryFullProcessImageName(IntPtr hprocess, int dwFlags,
                                                         StringBuilder lpExeName, out int size);

        #region Enums
        public const int KEYEVENTF_KEYDOWN = 0x0000; // New definition
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        public const int VK_LCONTROL = 0xA2; //Left Control key code
        public const byte vbKeyV = 86;
        public const byte vbKeyC = 67;
        public const byte vbKeyControl = 0x11;   // CTRL 键
        public const byte vbKeyShift = 0x10;     // SHIFT 键
        public const int WS_EX_NOACTIVATE = 0x08000000;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int GWL_EXSTYLE = -20;

        public static long SWEH_CHILDID_SELF = 0;

        //uCmd 可选值:
        public enum GW : uint
        {
            /// <summary>
            /// 同级别第一个
            /// </summary>
            HWNDFIRST = 0,
            /// <summary>
            /// 同级别最后一个
            /// </summary>
            HWNDLAST = 1,
            /// <summary>
            /// 同级别下一个
            /// </summary>
            HWNDNEXT = 2,
            /// <summary>
            /// 同级别上一个
            /// </summary>
            HWNDPREV = 3,
            /// <summary>
            /// 属主窗口
            /// </summary>
            OWNER = 4,
            /// <summary>
            /// 子窗口
            /// </summary>
            CHILD = 5,
        }

        public enum WM : uint
        {
            INACTIVE = 0x0000,
            SETFOCUS = 0x0007,
            KILLFOCUS = 0x0008,
            ACTIVATE = 0x0006,

            SYSCOMMAND = 0x0112,
        }

        public enum SC : uint
        {
            MAXIMIZE = 0xF030,
            MINIMIZE = 0xF020,
            RESTORE = 0xF120,
        }

        //SetWinEventHook() flags
        public enum SWEH_dwFlags : uint
        {
            WINEVENT_OUTOFCONTEXT = 0x0000,     // Events are ASYNC
            WINEVENT_SKIPOWNTHREAD = 0x0001,    // Don't call back for events on installer's thread
            WINEVENT_SKIPOWNPROCESS = 0x0002,   // Don't call back for events on installer's process
            WINEVENT_INCONTEXT = 0x0004         // Events are SYNC, this causes your dll to be injected into every process
        }

        //SetWinEventHook() events
        public enum SWEH_Events : uint
        {
            EVENT_MIN = 0x00000001,
            EVENT_MAX = 0x7FFFFFFF,
            EVENT_SYSTEM_ALERT = 0x0002,
            EVENT_SYSTEM_FOREGROUND = 0x0003,
            EVENT_SYSTEM_MENUSTART = 0x0004,
            EVENT_SYSTEM_MENUEND = 0x0005,
            EVENT_SYSTEM_MENUPOPUPSTART = 0x0006,
            EVENT_SYSTEM_MENUPOPUPEND = 0x0007,
            EVENT_SYSTEM_CAPTURESTART = 0x0008,
            EVENT_SYSTEM_CAPTUREEND = 0x0009,
            EVENT_SYSTEM_MOVESIZESTART = 0x000A,
            EVENT_SYSTEM_MOVESIZEEND = 0x000B,
            EVENT_SYSTEM_CONTEXTHELPSTART = 0x000C,
            EVENT_SYSTEM_CONTEXTHELPEND = 0x000D,
            EVENT_SYSTEM_DRAGDROPSTART = 0x000E,
            EVENT_SYSTEM_DRAGDROPEND = 0x000F,
            EVENT_SYSTEM_DIALOGSTART = 0x0010,
            EVENT_SYSTEM_DIALOGEND = 0x0011,
            EVENT_SYSTEM_SCROLLINGSTART = 0x0012,
            EVENT_SYSTEM_SCROLLINGEND = 0x0013,
            EVENT_SYSTEM_SWITCHSTART = 0x0014,
            EVENT_SYSTEM_SWITCHEND = 0x0015,
            EVENT_SYSTEM_MINIMIZESTART = 0x0016,
            EVENT_SYSTEM_MINIMIZEEND = 0x0017,
            EVENT_SYSTEM_DESKTOPSWITCH = 0x0020,
            EVENT_SYSTEM_END = 0x00FF,
            EVENT_OEM_DEFINED_START = 0x0101,
            EVENT_OEM_DEFINED_END = 0x01FF,
            EVENT_UIA_EVENTID_START = 0x4E00,
            EVENT_UIA_EVENTID_END = 0x4EFF,
            EVENT_UIA_PROPID_START = 0x7500,
            EVENT_UIA_PROPID_END = 0x75FF,
            EVENT_CONSOLE_CARET = 0x4001,
            EVENT_CONSOLE_UPDATE_REGION = 0x4002,
            EVENT_CONSOLE_UPDATE_SIMPLE = 0x4003,
            EVENT_CONSOLE_UPDATE_SCROLL = 0x4004,
            EVENT_CONSOLE_LAYOUT = 0x4005,
            EVENT_CONSOLE_START_APPLICATION = 0x4006,
            EVENT_CONSOLE_END_APPLICATION = 0x4007,
            EVENT_CONSOLE_END = 0x40FF,
            EVENT_OBJECT_CREATE = 0x8000,               // hwnd ID idChild is created item
            EVENT_OBJECT_DESTROY = 0x8001,              // hwnd ID idChild is destroyed item
            EVENT_OBJECT_SHOW = 0x8002,                 // hwnd ID idChild is shown item
            EVENT_OBJECT_HIDE = 0x8003,                 // hwnd ID idChild is hidden item
            EVENT_OBJECT_REORDER = 0x8004,              // hwnd ID idChild is parent of zordering children
            EVENT_OBJECT_FOCUS = 0x8005,                // * hwnd ID idChild is focused item
            EVENT_OBJECT_SELECTION = 0x8006,            // hwnd ID idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex
            EVENT_OBJECT_SELECTIONADD = 0x8007,         // hwnd ID idChild is item added
            EVENT_OBJECT_SELECTIONREMOVE = 0x8008,      // hwnd ID idChild is item removed
            EVENT_OBJECT_SELECTIONWITHIN = 0x8009,      // hwnd ID idChild is parent of changed selected items
            EVENT_OBJECT_STATECHANGE = 0x800A,          // hwnd ID idChild is item w/ state change
            EVENT_OBJECT_LOCATIONCHANGE = 0x800B,       // * hwnd ID idChild is moved/sized item
            EVENT_OBJECT_NAMECHANGE = 0x800C,           // hwnd ID idChild is item w/ name change
            EVENT_OBJECT_DESCRIPTIONCHANGE = 0x800D,    // hwnd ID idChild is item w/ desc change
            EVENT_OBJECT_VALUECHANGE = 0x800E,          // hwnd ID idChild is item w/ value change
            EVENT_OBJECT_PARENTCHANGE = 0x800F,         // hwnd ID idChild is item w/ new parent
            EVENT_OBJECT_HELPCHANGE = 0x8010,           // hwnd ID idChild is item w/ help change
            EVENT_OBJECT_DEFACTIONCHANGE = 0x8011,      // hwnd ID idChild is item w/ def action change
            EVENT_OBJECT_ACCELERATORCHANGE = 0x8012,    // hwnd ID idChild is item w/ keybd accel change
            EVENT_OBJECT_INVOKED = 0x8013,              // hwnd ID idChild is item invoked
            EVENT_OBJECT_TEXTSELECTIONCHANGED = 0x8014, // hwnd ID idChild is item w? test selection change
            EVENT_OBJECT_CONTENTSCROLLED = 0x8015,
            EVENT_SYSTEM_ARRANGMENTPREVIEW = 0x8016,
            EVENT_OBJECT_END = 0x80FF,
            EVENT_AIA_START = 0xA000,
            EVENT_AIA_END = 0xAFFF
        }

        //SetWinEventHook() Object Ids
        public enum SWEH_ObjectId : long
        {
            OBJID_WINDOW = 0x00000000,
            OBJID_SYSMENU = 0xFFFFFFFF,
            OBJID_TITLEBAR = 0xFFFFFFFE,
            OBJID_MENU = 0xFFFFFFFD,
            OBJID_CLIENT = 0xFFFFFFFC,
            OBJID_VSCROLL = 0xFFFFFFFB,
            OBJID_HSCROLL = 0xFFFFFFFA,
            OBJID_SIZEGRIP = 0xFFFFFFF9,
            OBJID_CARET = 0xFFFFFFF8,
            OBJID_CURSOR = 0xFFFFFFF7,
            OBJID_ALERT = 0xFFFFFFF6,
            OBJID_SOUND = 0xFFFFFFF5,
            OBJID_QUERYCLASSNAMEIDX = 0xFFFFFFF4,
            OBJID_NATIVEOM = 0xFFFFFFF0
        }

        public enum ABMsg : int
        {
            ABM_NEW = 0,
            ABM_REMOVE,
            ABM_QUERYPOS,
            ABM_SETPOS,
            ABM_GETSTATE,
            ABM_GETTASKBARPOS,
            ABM_ACTIVATE,
            ABM_GETAUTOHIDEBAR,
            ABM_SETAUTOHIDEBAR,
            ABM_WINDOWPOSCHANGED,
            ABM_SETSTATE
        }

        public enum ABNotify : int
        {
            ABN_STATECHANGE = 0,
            ABN_POSCHANGED,
            ABN_FULLSCREENAPP,
            ABN_WINDOWARRANGE
        }

        public enum ABEdge : int
        {
            ABE_LEFT = 0,
            ABE_TOP,
            ABE_RIGHT,
            ABE_BOTTOM
        }
        #endregion

        private static readonly SWEH_dwFlags WinEventHookInternalFlags = SWEH_dwFlags.WINEVENT_OUTOFCONTEXT |
                                                                SWEH_dwFlags.WINEVENT_SKIPOWNPROCESS |
                                                                SWEH_dwFlags.WINEVENT_SKIPOWNTHREAD;

        public delegate void WinEventDelegate(IntPtr hWinEventHook,
                                              SWEH_Events eventType,
                                              IntPtr hwnd,
                                              SWEH_ObjectId idObject,
                                              long idChild,
                                              uint dwEventThread,
                                              uint dwmsEventTime);

        public static IntPtr WinEventHookRange(SWEH_Events eventFrom,
                                               SWEH_Events eventTo,
                                               WinEventDelegate _delegate,
                                               uint idProcess, uint idThread)
        {
            return UnsafeNativeMethods.SetWinEventHook(eventFrom, eventTo,
                                                       IntPtr.Zero, _delegate,
                                                       idProcess, idThread,
                                                       WinEventHookInternalFlags);
        }

        public static IntPtr WinEventHookOne(SWEH_Events _event, WinEventDelegate _delegate, uint idProcess, uint idThread)
        {
            return UnsafeNativeMethods.SetWinEventHook(_event, _event,
                                                       IntPtr.Zero, _delegate,
                                                       idProcess, idThread,
                                                       WinEventHookInternalFlags);
        }

        public static bool WinEventUnhook(IntPtr hWinEventHook)
        {
            return UnsafeNativeMethods.UnhookWinEvent(hWinEventHook);
        }

        /// <summary>
        /// 返回 hWnd 窗口的线程标识
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static uint GetWindowThread(IntPtr hWnd)
        {
            return UnsafeNativeMethods.GetWindowThreadProcessId(hWnd, IntPtr.Zero);
        }

        /// <summary>
        /// 传入 out 参数，通过 hWnd 获取 PID
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        public static uint GetWindowThread(IntPtr hWnd, out uint processId)
        {
            return UnsafeNativeMethods.GetWindowThreadProcessId(hWnd, out processId);
        }

        /// <summary>
        /// 返回窗口左上和右下的坐标(left, top, right, bottom)
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static RECT GetWindowRect(IntPtr hWnd)
        {
            RECT rect = new RECT();
            _ = SafeNativeMethods.GetWindowRect(hWnd, ref rect);
            return rect;
        }

        /// <summary>
        /// 返回窗口客户区大小, Right as Width, Bottom as Height 其余两项只会是0
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static RECT GetClientRect(IntPtr hWnd)
        {
            RECT rect = new RECT();
            _ = SafeNativeMethods.GetClientRect(hWnd, ref rect);
            return rect;
        }

        /// <summary>
        /// 取得前台窗口句柄函数
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetForegroundWindow()
        {
            return SafeNativeMethods.GetForegroundWindow();
        }

        public static bool DestroyWindow(IntPtr hWnd)
        {
            return SafeNativeMethods.DestroyWindow(hWnd);
        }

        public static int GetWindowLong(IntPtr hWnd, int index)
        {
            return SafeNativeMethods.GetWindowLong(hWnd, index);
        }
        public static int SetWindowLong(IntPtr hWnd, int index, int newStyle)
        {
            return SafeNativeMethods.SetWindowLong(hWnd, index, newStyle);
        }
        public static bool SetForegroundWindow(IntPtr hWnd)
        {
            return SafeNativeMethods.SetForegroundWindow(hWnd);
        }
        public static bool BringWindowToTop(IntPtr hWnd)
        {
            return SafeNativeMethods.BringWindowToTop(hWnd);
        }
        public static IntPtr GetWindow(IntPtr parentHWnd, GW uCmd)
        {
            return SafeNativeMethods.GetWindow(parentHWnd, uCmd);
        }
        public static int GetWindowText(IntPtr hWnd, StringBuilder title, int length)
        {
            return SafeNativeMethods.GetWindowText(hWnd, title, length);
        }
        public static void SwitchToThisWindow(IntPtr hWnd, bool fAltTab = true)
        {
            SafeNativeMethods.SwitchToThisWindow(hWnd, fAltTab);
        }

        // ---

        public static uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData)
        {
            return SafeNativeMethods.SHAppBarMessage(dwMessage, ref pData);
        }

        public static int RegisterWindowMessage(string msg)
        {
            return SafeNativeMethods.RegisterWindowMessage(msg);
        }

        /// <summary>
        /// 取得Shell窗口句柄函数
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetShellWindow()
        {
            return SafeNativeMethods.GetShellWindow();
        }

        /// <summary>
        /// 取得桌面窗口句柄函数
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetDesktopWindow()
        {
            return SafeNativeMethods.GetDesktopWindow();
        }
    }

    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        // k1mlka
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int PID);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        internal static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr parentHWnd, NativeMethods.GW uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        // ---

        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int RegisterWindowMessage(string msg);

        [DllImport("user32.dll")]
        public static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
    }

    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr voidProcessId);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr SetWinEventHook(NativeMethods.SWEH_Events eventMin, NativeMethods.SWEH_Events eventMax,
                                                    IntPtr hmodWinEventProc, NativeMethods.WinEventDelegate lpfnWinEventProc,
                                                    uint idProcess, uint idThread, NativeMethods.SWEH_dwFlags dwFlags);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct APPBARDATA
    {
        public int cbSize;
        public IntPtr hWnd;
        public int uCallbackMessage;
        public int uEdge;
        public RECT rc;
        public IntPtr lParam;
    }
}
