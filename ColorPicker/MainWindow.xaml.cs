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
using WColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;
using WPoint = System.Windows.Point;
using DPoint = System.Drawing.Point;


namespace ColorPicker
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow
    {
        Thread RunMainThread = null;
        private bool isThreadEnding = false;
        private bool isThreadRun = false;

        [DllImport("user32.dll")]
        public static extern UInt16 GetAsyncKeyState(Int32 vKey);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public struct POINT
        {
            public int X;
            public int Y;
        }

        public struct HSV
        {
            public double H;
            public double S;
            public double V;
        }

        


        public MainWindow()
        {
            InitializeComponent();
        }

        void RunAct()
        {
            try
            {
                int prevKeyState = 0;
                POINT p;

                while (true)
                {
                    GetCursorPos(out p);
                    DColor c = GetPixel(p);

                    HSV hsv = RGB2HSV(c);

                    if (IsKeyPressed(ref prevKeyState, 120))
                    {
                        this.Invoke(new Action(() => RunAddListBox(p, c, hsv)));
                    }
                    this.Invoke(new Action(() => RunUIUpdate(p, c, hsv)));

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

            prev = crnt;

            return result;
        }

        void RunUIUpdate(POINT p, DColor c, HSV hsv)
        {
            tbxX.Text = p.X.ToString();
            tbxY.Text = p.Y.ToString();

            tbxR.Text = c.R.ToString();
            tbxG.Text = c.G.ToString();
            tbxB.Text = c.B.ToString();

            tbxH.Text = hsv.H.ToString("F2");
            tbxS.Text = hsv.S.ToString("F2");
            tbxV.Text = hsv.V.ToString("F2");

            tbxHexCode.Text = c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");

            bdrRGB.Background = new SolidColorBrush(WColor.FromArgb(c.A, c.R, c.G, c.B));
        }

        void RunAddListBox(POINT p, DColor c, HSV hsv)
        {
            string str = "";
            str += "X:" + p.X.ToString() + ",\t";
            str += "Y:" + p.Y.ToString() + ",\t";
            str += "R:" + c.R.ToString() + ",\t";
            str += "G:" + c.G.ToString() + ",\t";
            str += "B:" + c.B.ToString() + ",\t";
            str += "H:" + hsv.H.ToString("F2") + ",\t";
            str += "S:" + hsv.S.ToString("F2") + ",\t";
            str += "V:" + hsv.V.ToString("F2");

            //str = Convert.ToString(c.R, 16);


            listbox.Items.Add(str);
            listbox.SelectedIndex = listbox.Items.Count - 1;
        }


        public DColor GetPixel(POINT p)
        {
            using (Bitmap bmp = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.CopyFromScreen(p.X, p.Y, 0, 0, new System.Drawing.Size(1,1));
                }
                DColor result = bmp.GetPixel(0, 0);

                return result;
            }
        }

        public HSV RGB2HSV(DColor c)
        {
            HSV result_hsv = new HSV();

            double delta, min;

            min = Math.Min(Math.Min(c.R, c.G), c.B);
            result_hsv.V = Math.Max(Math.Max(c.R, c.G), c.B);
            delta = result_hsv.V - min;

            result_hsv.S = (result_hsv.V == 0.0)? 0.0: delta / result_hsv.V;

            if(result_hsv.S == 0.0)
            {
                result_hsv.H = 0.0;
            }
            else
            {
                if (c.R == result_hsv.V)
                {
                    result_hsv.H = (c.G - c.B) / delta;
                }
                else if (c.G == result_hsv.V)
                {
                    result_hsv.H = 2 + (c.B - c.R) / delta;
                }
                else if (c.B == result_hsv.V)
                {
                    result_hsv.H = 4 + (c.R - c.G) / delta;
                }

                result_hsv.H *= 60;

                if(result_hsv.H < 0.0)
                {
                    result_hsv.H += 360.0;
                }
            }

            result_hsv.H = Math.Round(result_hsv.H, 2);
            result_hsv.S = Math.Round(result_hsv.S * 100, 2);
            result_hsv.V = Math.Round(result_hsv.V * 100 / 255, 2);

            return result_hsv;
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

    }
}
