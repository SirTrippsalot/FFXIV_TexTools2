// FFXIV TexTools
// Copyright © 2017 Rafael Gonzalez - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using FFXIV_TexTools2.Helpers;
using FFXIV_TexTools2.Resources;
using FFXIV_TexTools2.ViewModel;
using FFXIV_TexTools2.Views;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Diagnostics;
using FFXIV_TexTools2.Properties;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;
using System.Xml.XPath;
using System.Xaml;


namespace FFXIV_TexTools2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool Maximized = false;
        private bool mRestoreIfMove = false;
        MainViewModel mViewModel;
        GridLength RCWidth;
        double RCMinWidth;
        int tabgap;
        

        public MainWindow()
        {
            if (Properties.Settings.Default.TT_Width > 0)
            {
                this.Top = Properties.Settings.Default.TT_Top;
                this.Left = Properties.Settings.Default.TT_Left;
                if (this.Top < 0) this.Top = 0;
            }
            InitializeComponent();
            mViewModel = new MainViewModel();
      
                DataContext = mViewModel;
            if (Properties.Settings.Default.TT_Height < System.Windows.SystemParameters.MaximizedPrimaryScreenHeight)
            {
                if (Properties.Settings.Default.TT_Width > 0 && Properties.Settings.Default.TT_Height > 0)
                {
                    MainWin.Width = Properties.Settings.Default.TT_Width;
                    MainWin.Height = Properties.Settings.Default.TT_Height;
                }
            }
            if (Properties.Settings.Default.TT_SplitWidth.Value > 0)
            {
                RightColumn.Width = Properties.Settings.Default.TT_SplitWidth;
            }
            if (Properties.Settings.Default.TT_SplitCollapse == true)
            {
                CollapseSpit();
            }
            if (Properties.Settings.Default.TT_Win_State == WindowState.Maximized)
            {
                SwitchWindowState();

            }
            BG.Visibility = System.Windows.Visibility.Visible;
            if (Properties.Settings.Default.DX_Ver == "DX11")
            {
                DXVerStatus.ToolTip = "Click Here to set DirectX9";
            }
            else
            {
                DXVerStatus.ToolTip = "Click Here to set DirectX11";
            }
            DXVerStatus.Content = "DirectX" + Properties.Settings.Default.DX_Ver.Substring(2);
            RespaceTabs();
            //HavokInterop.InitializeSTA();
        }
        void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr mWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(mWindowHandle).AddHook(new HwndSourceHook(WindowProc));
        }
        void ThumbBottomRightCorner_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width + e.HorizontalChange > 10)
                this.Width += e.HorizontalChange;
            if (this.Height + e.VerticalChange > 10)
                this.Height += e.VerticalChange;
        }
        void ThumbTopRightCorner_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width + e.HorizontalChange > 10)
                this.Width += e.HorizontalChange;

            if (this.Top + e.VerticalChange > 10 && this.Height - e.VerticalChange >= this.MinHeight)
            {
                this.Top += e.VerticalChange;
                this.Height -= e.VerticalChange;
            }
        }
        void ThumbTopLeftCorner_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Left + e.HorizontalChange > 10 && this.Width - e.HorizontalChange >= this.MinWidth)
            {
                this.Left += e.HorizontalChange;
                this.Width -= e.HorizontalChange;
            }
            if (this.Top + e.VerticalChange > 10 && this.Height - e.VerticalChange >= this.MinHeight)
            {
                this.Top += e.VerticalChange;
                this.Height -= e.VerticalChange;
            }
        }
        void ThumbBottomLeftCorner_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Left + e.HorizontalChange > 10 && this.Width - e.HorizontalChange >= this.MinWidth)
            {
                this.Left += e.HorizontalChange;
                this.Width -= e.HorizontalChange;
            }
            if (this.Height + e.VerticalChange > 10)
                this.Height += e.VerticalChange;
        }
        void ThumbRight_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width + e.HorizontalChange > 10)
                this.Width += e.HorizontalChange;
        }
        void ThumbLeft_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Left + e.HorizontalChange > 10 && this.Width - e.HorizontalChange >= this.MinWidth)
            {
                    this.Left += e.HorizontalChange;
                    this.Width -= e.HorizontalChange;
                            }
        }
        void ThumbBottom_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Height + e.VerticalChange > 10)
                this.Height += e.VerticalChange;
        }
        void ThumbTop_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Top + e.VerticalChange > 10 && this.Height - e.VerticalChange >= this.MinHeight)
            {
                this.Top += e.VerticalChange;
                this.Height -= e.VerticalChange;
            }
        }
        private static System.IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }
            return IntPtr.Zero;
        }
        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            POINT lMousePosition;
            GetCursorPos(out lMousePosition);
            IntPtr lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
            MONITORINFO lPrimaryScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
            {
                return;
            }
            IntPtr lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);
            MINMAXINFO lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            if (lPrimaryScreen.Equals(lCurrentScreen) == true)
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
            }
            else
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right - lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom - lPrimaryScreenInfo.rcMonitor.Top;
            }
            Marshal.StructureToPtr(lMmi, lParam, true);
        }
        private void SwitchWindowState()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    {
                        Properties.Settings.Default.TT_Width = MainWin.Width;
                        Properties.Settings.Default.TT_Height = MainWin.Height;
                        Properties.Settings.Default.TT_Top = Top;
                        Properties.Settings.Default.TT_Left = Left;
                        WindowState = WindowState.Maximized;
                        Maximized = true;
                        Thumbs();
                        RespaceTabs();
                        break;
                    }
                case WindowState.Maximized:
                    {
                        WindowState = WindowState.Normal;
                        Maximized = false;
                        Thumbs();
                        RespaceTabs();
                        break;
                    }
            }
        }
        public void RespaceTabs()
        {
            tabgap = (int)MainWin.Height - 540;
            if (tabgap < 0)
                tabgap = 0;
            Thickness tabmargin = new Thickness(0, tabgap, 0, -tabgap);
            SettingsTab.Margin = tabmargin;
            ChatTab.Margin = tabmargin;
        }
        void Thumbs()
        {
            if (Maximized == true)
            {
                ThumbBottom.Visibility = Visibility.Collapsed;
                ThumbLeft.Visibility = Visibility.Collapsed;
                ThumbTop.Visibility = Visibility.Collapsed;
                ThumbRight.Visibility = Visibility.Collapsed;
                ThumbTopLeftCorner.Visibility = Visibility.Collapsed;
                ThumbTopRightCorner.Visibility = Visibility.Collapsed;
                ThumbBottomLeftCorner.Visibility = Visibility.Collapsed;
                ThumbBottomRightCorner.Visibility = Visibility.Collapsed;
            }
            else
            {
                ThumbBottom.Visibility = Visibility.Visible;
                ThumbLeft.Visibility = Visibility.Visible;
                ThumbTop.Visibility = Visibility.Visible;
                ThumbRight.Visibility = Visibility.Visible;
                ThumbTopLeftCorner.Visibility = Visibility.Visible;
                ThumbTopRightCorner.Visibility = Visibility.Visible;
                ThumbBottomLeftCorner.Visibility = Visibility.Visible;
                ThumbBottomRightCorner.Visibility = Visibility.Visible;
            }
        }
        private void NewBar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mRestoreIfMove = false; 
            if (WindowState != WindowState.Maximized) { 
                Properties.Settings.Default.TT_Top = Top;
                Properties.Settings.Default.TT_Left = Left;
            }

        }
        private void NewBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (mRestoreIfMove)
            {
                mRestoreIfMove = false;
                double percentHorizontal = e.GetPosition(this).X / ActualWidth;
                double targetHorizontal = RestoreBounds.Width * percentHorizontal;
                double percentVertical = e.GetPosition(this).Y / ActualHeight;
                double targetVertical = RestoreBounds.Height * percentVertical;
                WindowState = WindowState.Normal;
                POINT lMousePosition;
                GetCursorPos(out lMousePosition);
                Left = lMousePosition.X - targetHorizontal;
                Top = lMousePosition.Y - targetVertical;
                DragMove();
            }
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);
        enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }
        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }

        private void DXVerStatus_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.DX_Ver == "DX11")
            {
                Properties.Settings.Default.DX_Ver = "DX9";
                Properties.Settings.Default.Save();
                DXVerStatus.ToolTip = "Click Here to set DirectX11";
            }
            else
            {
                Properties.Settings.Default.DX_Ver = "DX11";
                Properties.Settings.Default.Save();
                DXVerStatus.ToolTip = "Click Here to set DirectX9";
            }
            DXVerStatus.Content = "DirectX" + Properties.Settings.Default.DX_Ver.Substring(2);


        }

      
        private void Window_Closed(object sender, EventArgs e)
        {
            if (MainWin.WindowState == WindowState.Normal)
            {
                Properties.Settings.Default.TT_Width = MainWin.Width;
                Properties.Settings.Default.TT_Height = MainWin.Height;
                Properties.Settings.Default.TT_Top = Top;
                Properties.Settings.Default.TT_Left = Left;
            }
            if (MainWin.WindowState != WindowState.Minimized)
            {
                Properties.Settings.Default.TT_Win_State = MainWin.WindowState;
            }
            Properties.Settings.Default.TT_SplitWidth = RightColumn.Width;
            Properties.Settings.Default.Save();
            App.Current.Shutdown();
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("SplitterClick");
            if (MainSplitter.IsEnabled == true)
            {
                CollapseSpit();
                Properties.Settings.Default.TT_SplitCollapse = true;
            }
            else
            {
                IMGCollapse1.RenderTransformOrigin = new Point(0.5, 0.5);
                IMGCollapse2.RenderTransformOrigin = new Point(0.5, 0.5);
                ScaleTransform flipTrans = new ScaleTransform();
                flipTrans.ScaleX = 1;
                IMGCollapse1.RenderTransform = flipTrans;
                IMGCollapse2.RenderTransform = flipTrans;
                RightColumn.MaxWidth = 100000;
                RightColumn.MinWidth = RCMinWidth;
                RightColumn.Width = RCWidth;
                MainSplitter.IsEnabled = true;
                Properties.Settings.Default.TT_SplitCollapse = false;
            }
        }
        public void CollapseSpit()
        {
            IMGCollapse1.RenderTransformOrigin = new Point(0.5, 0.5);
            IMGCollapse2.RenderTransformOrigin = new Point(0.5, 0.5);
            ScaleTransform flipTrans = new ScaleTransform();
            flipTrans.ScaleX = -1;
            IMGCollapse1.RenderTransform = flipTrans;
            IMGCollapse2.RenderTransform = flipTrans;
            RCMinWidth = RightColumn.MinWidth;
            RCWidth = RightColumn.Width;
            RightColumn.MinWidth = 5;
            RightColumn.MaxWidth = 5;
            MainSplitter.IsEnabled = false;
        }

        private void textureTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = e.NewValue as CategoryViewModel;
            CategoryViewModel topLevel = null;
            if (item != null)
            {

                var itemParent = item.Parent;
                if (item.Children.Count > 0)
                {
                    Debug.WriteLine("Selected");
                    if (item.IsExpanded != true)
                    {
                        item.IsExpanded = true;
                        item.IsSelected = false;
                    }
                    else
                    {
                        item.IsExpanded = false;
                        item.IsSelected = false;
                    }
                }
                while (itemParent != null)
                {
                    topLevel = itemParent;
                    itemParent = itemParent.Parent;
                }
                if (item.ItemData != null)
                {
                    if (!topLevel.Name.Equals("UI"))
                    {
                        mViewModel.TextureVM.UpdateTexture(item.ItemData, item.Parent.Name);
                        if (item.Name.Equals(Strings.Face_Paint) || item.Name.Equals(Strings.Equipment_Decals))
                        {
                            tabControl.SelectedIndex = 0;
                            if (mViewModel.ModelVM != null)
                            {
                                mViewModel.ModelVM.ModelTabEnabled = false;
                            }
                        }
                        else
                        {
                            mViewModel.ModelVM.UpdateModel(item.ItemData, item.Parent.Name);
                            mViewModel.ModelVM.ModelTabEnabled = true;
                        }
                    }
                    else
                    {
                        tabControl.SelectedIndex = 0;
                        mViewModel.TextureVM.UpdateTexture(item.ItemData, "UI");
                        mViewModel.ModelVM.ModelTabEnabled = false;
                    }
                }
            }

        }
        private void NewBar_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {

                    SwitchWindowState();
                    Properties.Settings.Default.TT_Win_State = MainWin.WindowState;
                }
                else if (WindowState == WindowState.Maximized)
                {
                    mRestoreIfMove = true;
                    return;
                }

                DragMove();

            }
        }
        private void MaximizeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SwitchWindowState();
            Properties.Settings.Default.TT_Win_State = MainWin.WindowState;
        }
        private void MinimizeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }
        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MainWin.WindowState == WindowState.Normal)
            {
                Properties.Settings.Default.TT_Width = MainWin.Width;
                Properties.Settings.Default.TT_Height = MainWin.Height;
                Properties.Settings.Default.TT_Top = Top;
                Properties.Settings.Default.TT_Left = Left;
            }

            Properties.Settings.Default.TT_Win_State = MainWin.WindowState;
            Properties.Settings.Default.TT_SplitWidth = RightColumn.Width;
            Properties.Settings.Default.Save();
            App.Current.Shutdown();
        }

        private void MainWin_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Thumbs();
            RespaceTabs();
        }

        private void MainWin_StateChanged(object sender, EventArgs e)
        {
            Thumbs();
        }
    }

}
