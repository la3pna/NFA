using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;   // for Debug
using System.Numerics;  // for Complex
using System.IO;
using System.Globalization;
using System.IO.Ports;
using System.Threading;
using GPIBlibrary;

namespace NFA
{
    public partial class Form1 : Form
    {
        GPIB bus = new GPIB();
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, String lpWindowName);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint RegisterWindowMessage(string lpString);
        private readonly static int WM_USER = 0x0400;
        int WM_rem;
        IntPtr hwnd;
        bool firststart = true;
        Form2 secondForm = new Form2();
        public static bool debug = false;
        int addrNF;
        int addrSW;


        public Form1()
        {
            InitializeComponent();


            List<String> tList = new List<String>();

            comboBox1.Items.Clear();

            foreach (string s in SerialPort.GetPortNames())
            {
                tList.Add(s);
            }

            tList.Sort();
            comboBox1.Items.Add("Select COM port for tuner...");
            comboBox1.Items.AddRange(tList.ToArray());
            comboBox1.SelectedIndex = 0;


            List<String> tList2 = bus.portlist();
            comboBox2.Items.Add("Select COM port for GPIB...");
            comboBox2.Items.AddRange(tList.ToArray());
            comboBox2.SelectedIndex = 0;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.SmithPictureBox.Invalidate();
            Textboxadd("Program initiated, Smith chart drawn, VNWA program started.");
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;

            startInfo.FileName = "C:\\VNWA\\VNWA.exe";
            // startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = "-remote -callback " + System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.ToString() + " " + WM_USER.ToString() + " -debug";

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {


                }
            }
            catch
            {
                // Log error.
            }

        }

        private void Textboxadd(string s)
        {
            textBox1.Text = textBox1.Text + System.Environment.NewLine + s;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        // Create an array that defines the values of the R' and X' circles we want to display
        float[] circles = new float[] {0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f, 1.2f,
                                             1.4f, 1.6f, 1.8f, 2.0f, 3.0f, 4.0f, 5.0f, 10.0f, 20.0f};
        // Create array of magic numbers to prevent drawing arc outside the unity circle - this should be done in code!
        float[] Magic = new float[] { 11.4f, 22.6f, 33.5f, 43.5f, 53.2f, 62, 70, 77, 84, 90, 100, 109, 116, 122, 127, 143, 152, 158, 170, 179 };
        // Create array of magic numbers that position the X' text - this should be done in code!
        float[] Magic_2 = new float[] { 0.0f, 0.02f, 0.07f, 0.12f, 0.20f, 0.26f, 0.34f, 0.4f, 0.46f, 0.51f, 0.6f, 0.68f, 0.73f, 0.78f, 0.82f, 0.936f, 0.97f, 0.99f, 1.02f, 1.025f };
        float[] Magic_3 = new float[] { 0.41f, 0.31f, 0.23f, 0.16f, 0.09f, 0.05f, 0.02f, 0.0f, -0.01f, -0.01f, 0.0f, 0.02f, 0.04f, 0.07f, 0.096f, 0.2f, 0.27f, 0.314f, 0.41f, 0.46f };

        private int unity_circle_dia = 0;

        Pen p = new Pen(Brushes.Black);
        List<Complex> s11a = new List<Complex>();
        List<decimal> nf = new List<decimal>();
        List<decimal> gain_dut = new List<decimal>();
        private Form1 form;
        public void SetForm(Form1 myForm)
        {
            form = myForm;
        }

        private void DrawSmithChart(Graphics g)
        {
            // former parameters origin_x and origin_y become 0, as they are the upper left bound of the picture box.
            // likewise frame_size_x is 'Width' and frame_size_y is 'Height'
            // so this means origin_unity_x and origin_unity_y are also 0


            // Pen p = new Pen(Brushes.Black); 

            Rectangle r = new Rectangle(0, 0, SmithPictureBox.Width, SmithPictureBox.Height);  // outline round chart
            g.DrawRectangle(p, r);                // draw outline
            // fill chart rectangle with white
            g.FillRectangle(Brushes.White, r);


            // set the origin of the rectangle that contains the unity circle to be 2 * origin_x and origin_y for now
            int origin_unity_x = offsetX;
            int origin_unity_y = offsetY;

            // Draw unity circle
            Rectangle r_unity = new Rectangle(origin_unity_x, origin_unity_y, unity_circle_dia, unity_circle_dia);
            g.DrawEllipse(p, r_unity);

            // save x location of RHS  of unity circle
            int unity_RHS_x = r_unity.Right;

            // save y location of RHS of unity circle
            int unity_RHS_y = (r_unity.Height / 2 + origin_unity_x);

            // save unity circle radius
            int unity_circle_radius = unity_circle_dia / 2;

            // Draw pure Resistance line
            g.DrawLine(p, r_unity.Left, unity_RHS_y, r_unity.Right, unity_RHS_y);


            // Draw Real and Imaginary circles
            float[] diameter_R = new float[21];
            float[] diameter_X = new float[21];


            // calculate diameter of R' and X' circles
            for (int x = 0; x < circles.Length; x++)
            {
                diameter_R[x] = 1.0f / (1.0f + circles[x]);
                diameter_X[x] = (float)Math.Pow(1.0f / (float)Math.Sqrt(circles[x]), 2);   // (X'^-0.5)^2
            }

            Pen p1 = new Pen(Color.Black);
            Pen p2 = new Pen(Color.Red);

            // draw  Resistive and Reactive circles and text
            for (int x = 0; x < 20; x++)
            {
                Rectangle r1 = new Rectangle(unity_RHS_x - (int)(unity_circle_dia * diameter_R[x]), unity_RHS_y - (int)(unity_circle_radius * diameter_R[x]),
                                            (int)(unity_circle_dia * diameter_R[x]), (int)(unity_circle_dia * diameter_R[x]));
                g.DrawEllipse(p1, r1);


                Rectangle r2 = new Rectangle(unity_RHS_x - (int)(unity_circle_dia * diameter_X[x] / 2), unity_RHS_y - (int)(unity_circle_dia * diameter_X[x]),
                                             (int)(unity_circle_dia * diameter_X[x]), (int)(unity_circle_dia * diameter_X[x]));

                g.DrawArc(p2, r2, 90, Magic[x]);

                // Draw mirror image X', move rectangle down by diameter of unity circle, correct angles and draw
                Rectangle r3 = new Rectangle(unity_RHS_x - (int)(unity_circle_dia * diameter_X[x] / 2), unity_RHS_y, (int)(unity_circle_dia * diameter_X[x]),
                                             (int)(unity_circle_dia * diameter_X[x]));

                g.DrawArc(p2, r3, 270 - Magic[x], Magic[x]);

                // display R' & X' text
                display_text(g, MyFont, r_unity, r1, circles[x], Magic_2[x], Magic_3[x]);
            }
        }

        private void display_text(Graphics g, Font font, Rectangle r_unity, Rectangle r1, float f_text, float x, float y)
        {
            // Convert value to display to a string
            string text = f_text.ToString();  // use ToString("Nx")  where x is number of decimal places to display

            Font tempFont = new Font("Calibri", 12);

            // display R' values

            SizeF sz = g.VisibleClipBounds.Size;
            //Offset the coordinate system so that point (0, 0) is at the top left hand corner of the rectangle
            g.TranslateTransform(r1.Left, r1.Top);
            //Rotate the Graphics object so that the R text reads from bottom to top
            g.RotateTransform(270);
            sz = g.MeasureString(text, tempFont);
            //Offset the Drawstring method so that the center of the string is on the center line of the rectangle
            g.DrawString(text, font, Brushes.Black, -((sz.Width / 2) + r1.Width / 2), -(sz.Height / 2));
            //Reset the graphics object Transformations.
            g.ResetTransform();

            // display X' values above the Resistance line
            g.TranslateTransform(r_unity.Left, r_unity.Top);

            g.DrawString(text, font, Brushes.Black, -(sz.Width / 2 - (r_unity.Width * x)), -(sz.Height / 2 - (r_unity.Width * y)));

            float delta = (r_unity.Width / 2) - (r_unity.Width * y);

            // display X' values below the Resistance line
            g.DrawString(text, font, Brushes.Black, -(sz.Width / 2 - (r_unity.Width * x)), -(0 - ((r_unity.Width * y) + 2 * delta)));

            //Reset the graphics object Transformations.
            g.ResetTransform();
        }

        private Complex[] RhoAPlot;
        private Complex[] ZPlot;
        private Font MyFont;
        private int Steps = -1;
        private decimal[] noisefigure;
        private decimal[] gain;


        public void SetSteps(int steps, Font font)
        {
            Steps = steps - 1;
            RhoAPlot = new Complex[steps];
            ZPlot = new Complex[steps];
            if (Font == null)
                Font = this.Font;
            MyFont = font;
        }

        // compute complex impedance and store data for display by paint event 
        public Complex Set_Z(Complex rhoA, int index)
        {
            if (index < 0 || index > Steps)
            {
                throw new ArgumentOutOfRangeException("index is less than 0 or greater than Steps");
            }

            RhoAPlot[index] = rhoA;
            // Convert G into a complex impedance Zx
            // Complex Zo = new Complex(50, 0);
            Complex Z = new Complex();
            // VNA.ComputeComplexImpedance(rhoA, Zo, ref Z);
            ZPlot[index] = Z;

            return Z;
        }

        private int offsetX = 20;
        private int offsetY = 20;

        // Plot a complex Impedance on the Smith Chart
        private void PlotPoints(Graphics g)
        {
            int origin_unity_x = offsetX;
            int origin_unity_y = offsetY;

            // define the coordinates of the centre of the unity circle
            int middle_unity_x = origin_unity_x + unity_circle_dia / 2;
            int middle_unity_y = origin_unity_y + unity_circle_dia / 2;

            // define the pixel size of dot to display, this will need to scale when display is zoomed
            int dot = 6;

            Brush dotColour;

            // translate the coordinates to the centre of the unity circle
            g.TranslateTransform(middle_unity_x, middle_unity_y);

            for (int i = 0; i < RhoAPlot.Length; ++i)
            {
                Complex rhoA = RhoAPlot[i];

                Debug.WriteLine("G.Magnitude = {0}  G.Phase = {1}", rhoA.Magnitude, rhoA.Phase * 180 / Math.PI);
                Debug.WriteLine("G.Real = {0}  G.Imaginary = {1}", rhoA.Real, rhoA.Imaginary);

                // draw a dot on the chart at the location of Z', scale for diameter of the dot :)
                dotColour = Brushes.Blue;
                try
                {
                    if (noisefigure[i] > 20)
                        dotColour = Brushes.Red;      // first dot is green so we can see starting frequency
                    else if ((noisefigure[i] < 20) && (noisefigure[i] > 6))
                        dotColour = Brushes.DarkSalmon;
                    else if ((noisefigure[i] < 6) && (noisefigure[i] > 3))
                        dotColour = Brushes.Orange;
                    else if ((noisefigure[i] < 3) && (noisefigure[i] > 1))
                        dotColour = Brushes.Cyan;

                    else if (noisefigure[i] < 1)
                        dotColour = Brushes.Green;
                    else
                        dotColour = Brushes.Blue;
                }
                catch (Exception ex) { }


                g.FillEllipse(dotColour, (int)(unity_circle_dia / 2 * rhoA.Real) - dot / 2, -(int)(unity_circle_dia / 2 * rhoA.Imaginary) - dot / 2, dot, dot);
            }

            //Reset the graphics object Transformations.
            g.ResetTransform();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_USER)
            {
                if (firststart == true)
                {

                    WM_rem = m.WParam.ToInt32();
                    hwnd = new IntPtr(m.LParam.ToInt32());
                    firststart = false;
                }
                if (debug == true)
                {
                    Textboxadd(m.WParam.ToString() + "    " + m.LParam.ToString());
                }


            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private void PlotCurve(Graphics g)
        {
            int origin_unity_x = offsetX;
            int origin_unity_y = offsetY;

            // define the coordinates of the centre of the unity circle
            int middle_unity_x = origin_unity_x + unity_circle_dia / 2;
            int middle_unity_y = origin_unity_y + unity_circle_dia / 2;

            // translate the coordinates to the centre of the unity circle
            g.TranslateTransform(middle_unity_x, middle_unity_y);

            Point[] Points = new Point[RhoAPlot.Length];        // create a points array ready to drawing a curve

            for (int i = 0; i < RhoAPlot.Length; ++i)
            {
                Points[i] = new Point((int)(unity_circle_dia / 2 * RhoAPlot[i].Real), -(int)(unity_circle_dia / 2 * RhoAPlot[i].Imaginary));
            }

            p.Width = 3;                // need a fatter pen here
            g.DrawCurve(p, Points);     // draw a curve through the points

            //Reset the graphics object Transformations.
            g.ResetTransform();
            // reset pen width
            p.Width = 1;


        }

        public void clearSmithChart()
        {
            Graphics g = CreateGraphics();

            Rectangle r = new Rectangle(0, 0, SmithPictureBox.Width, SmithPictureBox.Height);  // outline round chart
            // fill chart rectangle with white
            g.FillRectangle(Brushes.White, r);

            g.Dispose();
        }

        private void SmithPictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (MyFont == null)
                MyFont = this.Font;

            // unity_circle_dia can be determined to be the min of width and height, so the circle is 'round'!
            unity_circle_dia = Math.Min(SmithPictureBox.Height, SmithPictureBox.Width) - 40;

            DrawSmithChart(g);

            if (Steps != -1)
            {
                if ((Steps == 0))
                {
                    PlotPoints(g);
                }
                else
                {
                    PlotPoints(g);
                    //    PlotCurve(g);
                }
            }
        }

        private void SmithPictureBox_Click(object sender, EventArgs e)
        {
            this.SmithPictureBox.Invalidate();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;  //here we add the autoscroll
            textBox1.ScrollToCaret();
        }



        private void Zpoint(Complex x, int step)
        {
            Complex z0 = new Complex(50, 0);
            Complex Z = new Complex();
            Z = Complex.Divide(Complex.Subtract(x, z0), Complex.Add(x, z0));
            // SetSteps(step, MyFont);
            Set_Z(Z, step);
        }

        int freqMHz;
        private void button2_Click(object sender, EventArgs e)
        {
            // setup instrument button
            //VNA stuff
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(0));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(99));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(58));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(92));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(116));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(101));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(115));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(116));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(92));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(116));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(101));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(115));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(116));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(46));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(115));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(49));
            SendMessage(hwnd, WM_rem, new IntPtr(7), new IntPtr(112));
            SendMessage(hwnd, WM_rem, new IntPtr(8), new IntPtr(Convert.ToInt32(textBox3.Text)));
            SendMessage(hwnd, WM_rem, new IntPtr(9), new IntPtr(Convert.ToInt32(textBox3.Text)));
            //tuner stuff
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }

            serialPort1.PortName = comboBox1.Text;
            serialPort1.BaudRate = 9600;
            serialPort1.DataBits = 8;
            serialPort1.StopBits = StopBits.One;
            serialPort1.Parity = Parity.None;
            serialPort1.ReadBufferSize = 4096;
            serialPort1.NewLine = "\r\n";
            serialPort1.Handshake = Handshake.None;
            serialPort1.ReadTimeout = 500;

            try
            {
                serialPort1.Open();
                serialPort1.Write("M");
                serialPort1.Write("L");
            }
            catch (Exception ex)
            {
                if (debug == true)
                {
                    //   Textboxadd(ex.ToString());
                }
            }
            //NF meter stuff
            bus.start(comboBox2.Text, 2000);
            addrNF = Convert.ToInt32(textBox2.Text);
            addrSW = Convert.ToInt32(textBox4.Text);
            freqMHz = Convert.ToInt32(textBox3.Text) / 1000000;
            bus.write(addrNF, "FR" + freqMHz.ToString() + "MZ");
            bus.write(addrNF, "H1");
            bus.write(addrNF, "M2");
            bus.write(addrNF, "N0");
            bus.write(addrNF, "X0");

        }

        private void read_testfile()
        {
            // read testfile, then add to the frequency list. 

            string line;
            System.IO.StreamReader sr = new
            System.IO.StreamReader("C:\\test\\test.s1p");
            while ((line = sr.ReadLine()) != null)
            {
                if (line.ToLowerInvariant().Contains('!'))
                {
                }
                else if (line.ToLowerInvariant().Contains('#'))
                {
                }
                else
                {

                    Complex s11b;
                    string[] servalspl = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string freqStrn = servalspl[0];
                    string s11reStrn = servalspl[1];
                    string s11imagStrn = servalspl[2];

                    float s11re = Convert.ToSingle(s11reStrn, CultureInfo.InvariantCulture);
                    float s11imag = Convert.ToSingle(s11imagStrn, CultureInfo.InvariantCulture);
                    s11b = new Complex(s11re, s11imag);
                    s11a.Add(s11b);

                    Complex rho = 50 * ((1 + s11b) / (1 - s11b));
                   // textBox5.Text = Convert.ToString(rho.Real);
                   // textBox6.Text = Convert.ToString(rho.Imaginary);
                }
            }
            sr.Close();
        }

        Thread t;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SendMessage(hwnd, WM_rem, new IntPtr(0), new IntPtr(0));
            try
            {
                t.Abort();
               // SendMessage(hwnd, WM_rem, new IntPtr(0), new IntPtr(0));
            }
            catch (Exception ex)
            {

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // do measure button!!!
            t = new Thread(measure);          // Kick off a new thread
            t.Start();
        }

        public void measure()
        {
            for (int i = 0; i <= 12; i++)
            {
                for (int j = 0; j <= 90; j++)
                {
                    try
                    {
                        serialPort1.Write("J");
                        if (debug == true)
                        {
                            // Textboxadd("J" + j.ToString()); 
                        }

                    }
                    catch (Exception ex)
                    {
                        // Textboxadd(ex.ToString());
                    }
                    System.Threading.Thread.Sleep(2200);
                    // here we need somthing that measures stuff
                    VNAsweep();
                    //delay some
                   // System.Threading.Thread.Sleep(2000);
                    //switch to noise measure
                    bus.write(addrSW, "A1A2A3");
                    System.Threading.Thread.Sleep(1000);

                    //measure noise parameter
                   NFmeasure();
                   System.Threading.Thread.Sleep(500);
                    //switch back

                    bus.write(addrSW, "B1B2B3");
                }

                try
                {
                    serialPort1.Write("L");
                    serialPort1.Write("K");
                    if (debug == true)
                    {
                        Textboxadd("K" + i.ToString());
                    }

                }
                catch (Exception ex)
                {
                    Textboxadd(ex.ToString());
                }
            }
        }


        public virtual void NFmeasure()
        {
          //  Textboxadd("NF start");
            string a = bus.writeread(addrNF, "T2");
            a = bus.writeread(addrNF, "T2");
            if (Regex.IsMatch(a, @"\d"))
            {
                try
                {
                    string[] servalspl = a.Split(',');

                    decimal atemp = Decimal.Parse(servalspl[0], System.Globalization.NumberStyles.Any);
                    decimal btemp = Decimal.Parse(servalspl[1], System.Globalization.NumberStyles.Any);
                    decimal ctemp = Decimal.Parse(servalspl[2], System.Globalization.NumberStyles.Any);
                    nf.Add(ctemp);
                    gain_dut.Add(btemp);

                //    textBox1.Text = textBox1.Text + System.Environment.NewLine + " freq = " + atemp.ToString() + " gain = " + btemp.ToString() + " NF = " + ctemp.ToString();
                }
                catch (Exception ex)
                {
                 //   textBox1.Text = " error ";
                    nf.Add(9999);
                    gain_dut.Add(0);
                }
               
                noisefigure = nf.ToArray();
                gain = gain_dut.ToArray();
            }
            this.SmithPictureBox.Invalidate();
        }

        private void VNAsweep()
        {

            SendMessage(hwnd, WM_rem, new IntPtr(10), new IntPtr(1));
            SendMessage(hwnd, WM_rem, new IntPtr(1), new IntPtr(2));
            System.Threading.Thread.Sleep(200);
            SendMessage(hwnd, WM_rem, new IntPtr(5), new IntPtr(1));
            System.Threading.Thread.Sleep(3000);
            read_testfile();
            System.Threading.Thread.Sleep(500);
            Complex[] x = s11a.ToArray();

            int j = x.Length;
            SetSteps(j + 1, MyFont);
            for (int i = 0; i < x.Length; i++)
            {
                Set_Z(x[i], i);
                //  Zpoint(x[i], i);
            }
            System.Threading.Thread.Sleep(100);
            this.SmithPictureBox.Invalidate();
            if (debug == true)
            {
                Textboxadd("added point");
            }

        }
        private void button5_Click(object sender, EventArgs e)
        {
            // plot button
            Complex[] x = s11a.ToArray();

            int j = x.Length;
            SetSteps(j + 1, MyFont);
            for (int i = 0; i < x.Length; i++)
            {
                Set_Z(x[i], i);
                //  Zpoint(x[i], i);
            }
            this.SmithPictureBox.Invalidate();
            Textboxadd("added point");
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            secondForm.Show();
        }

        private SaveFileDialog saveFileDialog1;
       
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

            saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "Comma separated (*.csv)|*.csv|Graph picture (*.jpg)|*.jpg";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileOk += saveFileDialog1_FileOk;
            saveFileDialog1.ShowDialog();
        }
        void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            

            if (e.Cancel == true)
                return;

            var extension = Path.GetExtension(saveFileDialog1.FileName);

            switch (extension.ToLower())
            {
                case ".jpg":


                    // Hide the form so that it does not appear in the screenshot

                    try
                    {

                        PrintScreen(saveFileDialog1.FileName);
                        Textboxadd("screenprint saved");

                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                    }

                    break;
                case ".csv":
                    
                    StreamWriter wText = new StreamWriter(saveFileDialog1.FileName);
                    wText.WriteLine("! Noise parameter");
                    wText.WriteLine("! real, imag, mag, phase, NF, gain");
                    wText.WriteLine("# " + freqMHz + " MHz");
                    Complex[] dataArray = s11a.ToArray(); 
                    int length = dataArray.Length;
                    for (int i = 0; i <= length - 1; i++)
                    {
                        wText.WriteLine(Convert.ToString(dataArray[i].Real, CultureInfo.InvariantCulture) + ',' + Convert.ToString(dataArray[i].Imaginary, CultureInfo.InvariantCulture) + "," + Convert.ToString(RhoAPlot[i].Magnitude, CultureInfo.InvariantCulture) + ',' + Convert.ToString(RhoAPlot[i].Phase, CultureInfo.InvariantCulture) + "," + Convert.ToString(noisefigure[i],CultureInfo.InvariantCulture)+ "," + Convert.ToString(gain[i],CultureInfo.InvariantCulture));
                    }
                    wText.Flush(); 
                    wText.Close();
                    break;
               
            }



        }

        private void PrintScreen(string file)
        {

            string initialDirectory = file;
            using (BackgroundWorker worker = new BackgroundWorker())
            {
                Thread.Sleep(0);
                this.Refresh();
                worker.DoWork += delegate(object sender, DoWorkEventArgs e)
                {
                    BackgroundWorker wkr = sender as BackgroundWorker;
                    Rectangle bounds = new Rectangle(Location, Size);
                    Thread.Sleep(300);
                    Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Location, Point.Empty, bounds.Size);
                    }
                    e.Result = bitmap;
                };
                worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
                {
                    if (e.Error != null)
                    {
                        Exception err = e.Error;
                        while (err.InnerException != null)
                        {
                            err = err.InnerException;
                        }
                        MessageBox.Show(err.Message, "Screen Capture",
                                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else if (e.Cancelled == true)
                    {
                    }
                    else if (e.Result != null)
                    {
                        if (e.Result is Bitmap)
                        {
                            Bitmap bitmap = (Bitmap)e.Result;

                            bitmap.Save(file);

                        }
                    }



                };
                worker.RunWorkerAsync();

            }


        }
    }
}