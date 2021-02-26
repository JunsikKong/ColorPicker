using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorPicker
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow
    {
        int i = 0;
        Thread RunMainThread = null;
        private bool isThreadEnding = false;
        private bool isThreadRun = false;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator System.Windows.Point(POINT point)
            {
                return new System.Windows.Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll")]
        public static extern UInt16 GetAsyncKeyState(Int32 vKey);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);



        public MainWindow()
        {
            InitializeComponent();


            string str = "da";
            listbox.Items.Add(str);
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            if (RunMainThread == null)
            {
                RunMainThread = new Thread(() => RunAct());
                RunMainThread.IsBackground = true;

                tbxTmp.Text = $"Thread START";

                RunMainThread.Start();

                isThreadRun = true;
            }
            else
            {
                tbxTmp.Text = $"[경고] 이미 실행중입니다. - START";
            }
        }


        void RunAct()
        {
            try
            {
                int prevKeyState = 0;

                while (true)
                {
                    if(IsKeyPressed(ref prevKeyState, 120))
                    {

                    }

                    this.Invoke(new Action(() => RunUIUpdate()));

                    Thread.Sleep(1);
                }
            }
            catch (ThreadInterruptedException)
            {
                isThreadEnding = true;
            }
            finally
            {

            }
        }


        bool IsKeyPressed(ref int prev, int vKey)
        {
            bool result;

            int crnt = GetAsyncKeyState(vKey);

            if (crnt - prev > 1)
            {
                result = true;
            }
            else
            {
                result = false;

            }
            
            return result;
        }

        void RunUIUpdate()
        {
            POINT p;
            GetCursorPos(out p);

            tbxX.Text = p.X.ToString();
            tbxY.Text = p.Y.ToString();

            System.Drawing.Color c = GetPixel(p);

            tbxR.Text = c.R.ToString();
            tbxG.Text = c.G.ToString();
            tbxB.Text = c.B.ToString();

            bdrRGB.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B));
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            if (RunMainThread != null)
            {
                isThreadRun = false;
                RunMainThread = null;
            }
            else
            {
                tbxTmp.Text = "[경고] 실행중이 아닙니다. - STOP";
            }
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {

        }

        public System.Drawing.Color GetPixel(POINT p)
        {
            //int width = (int)SystemParameters.PrimaryScreenWidth*2;
            //int height = (int)SystemParameters.PrimaryScreenHeight;
            //using (Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))

            using (Bitmap bmp = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.CopyFromScreen(p.X, p.Y, 0, 0, new System.Drawing.Size(500,500));
                }

                System.Drawing.Color c = bmp.GetPixel(0, 0);

                return c;

                //bmp.Save("test.png", ImageFormat.Png);
            }
        }

    }
}
