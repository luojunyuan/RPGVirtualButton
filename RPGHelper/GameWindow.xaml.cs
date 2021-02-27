using ModernWpf.Controls;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using WindowsInput.Events;

namespace RPGHelper
{
    /// <summary>
    /// GameWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GameWindow : Window
    {
        readonly GameWindowViewModel ViewModel = new();
        private Process GameProcess;

        internal GameWindow(Process process)
        {
            InitializeComponent();
            DataContext = ViewModel;

            GameProcess = process;
            dpi = VisualTreeHelper.GetDpi(this).DpiScaleX;
            SetGameWindowHook();
        }

        #region Game Window Follow Hook
        private static NativeMethods.WinEventDelegate? WinEventDelegate;
        private static GCHandle GCSafetyHandle;
        private static IntPtr hWinEventHook;
        private static IntPtr gameHWnd;

        private void SetGameWindowHook()
        {
            WinEventDelegate = new NativeMethods.WinEventDelegate(WinEventCallback);
            GCSafetyHandle = GCHandle.Alloc(WinEventDelegate);

            // Register game exit event
            GameProcess.EnableRaisingEvents = true;
            GameProcess.Exited += new EventHandler(ApplicationExit);

            gameHWnd = GameProcess.MainWindowHandle;

            CheckWindowHandler();
            // For the first time
            if (gameHWnd == GameProcess.MainWindowHandle)
            {
                SetWindowHandler();
            }
        }

        private void ApplicationExit(object? sender, EventArgs e)
        {
            Debug.WriteLine("game quit");
            GCSafetyHandle.Free();
            NativeMethods.WinEventUnhook(hWinEventHook);

            Application.Current.Dispatcher.InvokeAsync(() => Close());
        }

        public void CheckWindowHandler()
        {
            var clientRect = NativeMethods.GetClientRect(gameHWnd);
            IntPtr realHandle = IntPtr.Zero;

            if (400 > clientRect.Bottom && 400 > clientRect.Right)
            {
                // Tip: This would active even when game window get minimize
                var windowRect = NativeMethods.GetWindowRect(gameHWnd);
                if ($"{windowRect.Left} {windowRect.Top} {windowRect.Right} {windowRect.Bottom}"
                    .Equals("-32000 -32000 -31840 -31972"))
                {
                    // Ignore minimize window situation
                    return;
                }

                // Start search handle
                realHandle = FindRealHandle();
            }

            if (realHandle != IntPtr.Zero)
            {
                gameHWnd = realHandle;
                SetWindowHandler();
            }
            else
            {
                // gameHwnd still be last handle
            }
        }

        private IntPtr FindRealHandle()
        {
            int textLength = GameProcess.MainWindowTitle.Length;
            StringBuilder title = new StringBuilder(textLength + 1);
            NativeMethods.GetWindowText(GameProcess.MainWindowHandle, title, title.Capacity);

            Debug.WriteLine($"Can't find standard window in MainWindowHandle! Start search title 「{title}」");

            // Must use original gameProc.MainWindowHandle
            IntPtr first = NativeMethods.GetWindow(GameProcess.MainWindowHandle, NativeMethods.GW.HWNDFIRST);
            IntPtr last = NativeMethods.GetWindow(GameProcess.MainWindowHandle, NativeMethods.GW.HWNDLAST);

            IntPtr cur = first;
            while (cur != last)
            {
                StringBuilder outText = new StringBuilder(textLength + 1);
                NativeMethods.GetWindowText(cur, outText, title.Capacity);
                if (outText.Equals(title))
                {
                    var rectClient = NativeMethods.GetClientRect(cur);
                    if (rectClient.Right != 0 && rectClient.Bottom != 0)
                    {
                        // check pid
                        _ = NativeMethods.GetWindowThread(cur, out uint pid);
                        //foreach (var proc in DataRepository.GameProcesses)
                        //{
                        if (GameProcess.Id == pid)
                        {
                            Debug.WriteLine($"Find handle at 0x{Convert.ToString(cur.ToInt64(), 16).ToUpper()}");
                            // Search over, believe handle is found
                            return cur;
                        }
                        //}
                    }
                }

                cur = NativeMethods.GetWindow(cur, NativeMethods.GW.HWNDNEXT);
            }

            Debug.WriteLine("Find failed, use last handle");
            return IntPtr.Zero;
        }

        private void SetWindowHandler()
        {
            Debug.WriteLine("Set handle");
            uint targetThreadId = NativeMethods.GetWindowThread(gameHWnd);

            // 调用 SetWinEventHook 传入 WinEventDelegate 回调函数，必须在UI线程上执行启用
            Application.Current.Dispatcher.InvokeAsync(
                () => hWinEventHook = NativeMethods.WinEventHookOne(
                    NativeMethods.SWEH_Events.EVENT_OBJECT_LOCATIONCHANGE,
                    WinEventDelegate!,
                    (uint)GameProcess.Id,
                    targetThreadId)
            );
            // Send game position first time
            UpdateLocation();
        }

        private void WinEventCallback(IntPtr hWinEventHook,
                                    NativeMethods.SWEH_Events eventType,
                                    IntPtr hWnd,
                                    NativeMethods.SWEH_ObjectId idObject,
                                    long idChild,
                                    uint dwEventThread,
                                    uint dwmsEventTime)
        {
            // 游戏窗口获取焦点时会调用
            //if (hWnd == GameInfo.Instance.hWnd &&
            //    eventType == Hook.SWEH_Events.EVENT_OBJECT_FOCUS)
            //{
            //    log.Info("Game window get foucus");
            //}

            // Update game's position infomation
            if (hWnd == gameHWnd &&
                eventType == NativeMethods.SWEH_Events.EVENT_OBJECT_LOCATIONCHANGE &&
                idObject == (NativeMethods.SWEH_ObjectId)NativeMethods.SWEH_CHILDID_SELF)
            {
                UpdateLocation();
            }
        }

        private void UpdateLocation()
        {
            var rect = NativeMethods.GetWindowRect(gameHWnd);
            var rectClient = NativeMethods.GetClientRect(gameHWnd);

            var width = rect.Right - rect.Left;  // equal rectClient.Right + shadow*2
            var height = rect.Bottom - rect.Top; // equal rectClient.Bottom + shadow + title

            var winShadow = (width - rectClient.Right) / 2;

            var wholeHeight = rect.Bottom - rect.Top;
            var winTitleHeight = wholeHeight - rectClient.Bottom - winShadow;

            var clientArea = new Thickness(winShadow / dpi, winTitleHeight / dpi, winShadow / dpi, winShadow / dpi);

            try
            {
                Height = height / dpi;
                Width = width / dpi;
                Left = rect.Left / dpi;
                Top = rect.Top / dpi;
                GameView.Padding = clientArea;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Hide Window In AltTab And No Focus
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HideAltTab();

            int exStyle = NativeMethods.GetWindowLong(gameWindowHwnd, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(
                                        gameWindowHwnd,
                                        NativeMethods.GWL_EXSTYLE,
                                        exStyle | NativeMethods.WS_EX_NOACTIVATE);
        }

        private void HideAltTab()
        {
            var windowInterop = new WindowInteropHelper(this);
            var exStyle = NativeMethods.GetWindowLong(windowInterop.Handle, NativeMethods.GWL_EXSTYLE);
            exStyle |= NativeMethods.WS_EX_TOOLWINDOW;
            NativeMethods.SetWindowLong(windowInterop.Handle, NativeMethods.GWL_EXSTYLE, exStyle);
        }
        #endregion

        #region DPI Aware
        private double dpi;

        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            base.OnDpiChanged(oldDpi, newDpi);

            dpi = VisualTreeHelper.GetDpi(this).DpiScaleX;
        }
        #endregion

        #region Topmost And FullScreen Hook Setup
        IntPtr gameWindowHwnd;
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            HwndSource? source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(WndProc);

            var interopHelper = new WindowInteropHelper(this);
            gameWindowHwnd = interopHelper.Handle;

            RegisterAppBar(false);

            // Alaways make window front
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (sender, _) =>
            {
                if (gameWindowHwnd == IntPtr.Zero)
                {
                    timer.Stop();
                }
                if (gameHWnd == NativeMethods.GetForegroundWindow())
                {
                    NativeMethods.BringWindowToTop(gameWindowHwnd);
                }
            };

            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Start();
        }
        #endregion

        #region FullScreen Button Transition
        private IntPtr desktopHandle;
        private IntPtr shellHandle;
        int uCallBackMsg;

        private void RegisterAppBar(bool registered)
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = gameWindowHwnd;

            desktopHandle = NativeMethods.GetDesktopWindow();
            shellHandle = NativeMethods.GetShellWindow();
            if (!registered)
            {
                //register
                uCallBackMsg = NativeMethods.RegisterWindowMessage("APPBARMSG_CSDN_HELPER");
                abd.uCallbackMessage = uCallBackMsg;
                _ = NativeMethods.SHAppBarMessage((int)NativeMethods.ABMsg.ABM_NEW, ref abd);
            }
            else
            {
                _ = NativeMethods.SHAppBarMessage((int)NativeMethods.ABMsg.ABM_REMOVE, ref abd);
            }
        }

        private Button? fullScreenButton;
        private void FullScreenButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            fullScreenButton = button;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == uCallBackMsg)
            {
                switch (wParam.ToInt32())
                {
                    case (int)NativeMethods.ABNotify.ABN_FULLSCREENAPP:
                        IntPtr hWnd = NativeMethods.GetForegroundWindow();
                        //判断当前全屏的应用是否是桌面
                        if (hWnd.Equals(desktopHandle) || hWnd.Equals(shellHandle))
                        {
                            break;
                        }
                        //判断是否全屏
                        if ((int)lParam == 1)
                        {
                            if (fullScreenButton is not null)
                            {
                                fullScreenButton.Content = new SymbolIcon { Symbol = Symbol.BackToWindow };
                            }
                        }
                        else
                        {
                            if (fullScreenButton is not null)
                            {
                                fullScreenButton.Content = new SymbolIcon { Symbol = Symbol.FullScreen };
                            }
                        }
                        CheckWindowHandler();
                        break;
                    default:
                        break;
                }
            }

            return IntPtr.Zero;
        }
        #endregion

        #region 左下
        private async void CtrlButton_PreviewTouchDown(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Hold(KeyCode.Control).Invoke();
        private async void CtrlButton_PreviewRelease(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Release(KeyCode.Control).Invoke();

        private async void ShiftButton_PreviewTouchDown(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Hold(KeyCode.Shift).Invoke();
        private async void ShiftButton_PreviewTouchUp(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Release(KeyCode.Shift).Invoke();
       
        private async void ZButton_PreviewTouchDown(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Click(KeyCode.Z).Invoke();
        private async void XButton_PreviewTouchDown(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Click(KeyCode.X).Invoke();
        #endregion

        #region 右下
        private async void LeftButton_PreviewTouchDown(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Hold(KeyCode.Left).Invoke();
        private async void LeftButton_PreviewRelease(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Release(KeyCode.Left).Invoke();

        private async void TopButton_PreviewTouchDown(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Hold(KeyCode.Up).Invoke();
        private async void TopButton_PreviewRelease(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Release(KeyCode.Up).Invoke();

        private async void RightButton_PreviewTouchDown(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Hold(KeyCode.Right).Invoke();
        private async void RightButton_PreviewRelease(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Release(KeyCode.Right).Invoke();

        private async void BottomButton_PreviewTouchDown(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Hold(KeyCode.Down).Invoke();
        private async void BottomButton_PreviewRelease(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Release(KeyCode.Down).Invoke();

        private async void EnterButton_PreviewTouchDown(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Click(KeyCode.Enter).Invoke();
        #endregion

        #region 左上
        private async void ESCButton_PreviewTouchDown(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().Click(KeyCode.Escape).Invoke();
        private async void FullScreenButton_PreviewTouchDown(object sender, TouchEventArgs e) => await WindowsInput.Simulate.Events().ClickChord(KeyCode.Alt, KeyCode.Enter).Invoke();
        #endregion

        private void Window_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (NativeMethods.GetForegroundWindow() != gameHWnd)
            {
                //NativeMethods.BringWindowToTop(GameProcess.MainWindowHandle);
                NativeMethods.SwitchToThisWindow(gameHWnd);
            }
        }
    }
}
