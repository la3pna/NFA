using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Numerics;
using System.IO;
using System.Globalization;



namespace vnwa_ctrl
{
    public partial class Form1 : Form
    {

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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // For the example
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

        private void button2_Click(object sender, EventArgs e)
        {
           //ready for code 
            SendMessage(hwnd, WM_rem, new IntPtr(5), new IntPtr(1));
  
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SendMessage(hwnd, WM_rem , new IntPtr(0), new IntPtr(0));
            Application.Exit();
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

                  textBox2.Text = m.WParam.ToString();
                  textBox1.Text = m.LParam.ToString();
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
         
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
 
        }

        private void button5_Click(object sender, EventArgs e)
        {
            

            // gets the start and stop frequency, sends it to the VNA, does a sweep of S11
            SendMessage(hwnd, WM_rem, new IntPtr(8), new IntPtr(Convert.ToInt32(textBox3.Text)));
            SendMessage(hwnd, WM_rem, new IntPtr(9), new IntPtr(Convert.ToInt32(textBox3.Text)));
            SendMessage(hwnd, WM_rem, new IntPtr(10), new IntPtr(1));
            SendMessage(hwnd, WM_rem, new IntPtr(1), new IntPtr(2));
        }

        List<Complex> s11a = new List<Complex>();

        private void read_testfile(object sender, EventArgs e) 
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
                     textBox5.Text = Convert.ToString(rho.Real);  
                     textBox6.Text = Convert.ToString(rho.Imaginary); 
                 }
             }
             sr.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            read_testfile(sender,e);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SendMessage(hwnd, WM_rem, new IntPtr(0), new IntPtr(0));
        }

    }
}
