using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Diagnostics.Eventing.Reader;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace prolock
{

    public partial class MainForm : Form
    {
        private StringBuilder msg_out = new StringBuilder();

        private Process ps = null;
        private bool log = false;
        private bool mem = false;
        private bool defend = false;

        private Thread log_det;
        private Thread mem_det;
        private Thread def;

        Thread dyn;
        public MainForm()
        {
            InitializeComponent();
        }

        private void Main_form(object sender, EventArgs e)
        {

        }
        private void Static_checkbox(object sender, EventArgs e)
        {

        }

        private void Dynamic_checkbox(object sender, EventArgs e)
        {

        }

        private void Start_Click(object sender, EventArgs e)
        {
           
            if (Static.Checked && !Dynamic.Checked)
            {
                bool r = false;
                bool c = false;
                msg_out.AppendLine("Statick Detection Start");
                textBox1.Text = msg_out.ToString();
                textBox1.ScrollToCaret();

                string[] bat = new string[] { "run.bat", "clean.bat" }; //7글자, 9글자
                string[] bat_hash = new string[] { "0", "0" };

                ProcessStartInfo cmd = new ProcessStartInfo();  //https://redreans.tistory.com/58
                Process process = new Process();
                cmd.FileName = @"cmd";
                cmd.WindowStyle = ProcessWindowStyle.Hidden;
                cmd.CreateNoWindow = true;

                cmd.UseShellExecute = false;
                cmd.RedirectStandardOutput = true;
                cmd.RedirectStandardInput = true;
                cmd.RedirectStandardError = true;

                process.EnableRaisingEvents = false;
                process.StartInfo = cmd;
                
                for (int i = 0; i < 2; i++)
                {
                    process.Start();
                    process.StandardInput.Write(@"cd C:\ProgramData" + Environment.NewLine);
                    process.StandardInput.Write(@"certutil -hashfile " + bat[i] + " SHA256" + Environment.NewLine);
                    process.StandardInput.Close();

                    string result = process.StandardOutput.ReadToEnd(); //내용확인하기 위해 있는 부분
                    StringBuilder sb = new StringBuilder();

                    sb.Append(result);
                    if (sb.ToString().Length > 334)
                    {
                        sb.Remove(0, 215 + i * 4);
                        sb.Remove(64, 59);
                    }
                    if (sb.ToString().CompareTo("18661f8c245d26be1ec4df48a9e186569a77237f424f322f00ef94652b9d5f35") == 0)
                    {
                        msg_out.Append("run.bat File Detection");
                        textBox1.Text = msg_out.ToString();
                        textBox1.ScrollToCaret();

                        process.Start();
                        process.StandardInput.Write(@"cd C:\ProgramData" + Environment.NewLine);
                        process.StandardInput.Write(@"del run.bat" + Environment.NewLine);
                        process.StandardInput.Close();
                        r = true;

                        msg_out.Append("Delete run.bat");
                        textBox1.Text = msg_out.ToString();
                        textBox1.ScrollToCaret();
                    }
                    if (sb.ToString().CompareTo("b262b1b82e5db337d367ea1d4119cadb928963896f1aff940be763a00d45f305") == 0)
                    {
                        msg_out.Append("clean.bat File Detection");
                        textBox1.Text = msg_out.ToString();
                        textBox1.ScrollToCaret();

                        process.Start();
                        process.StandardInput.Write(@"cd C:\ProgramData" + Environment.NewLine);
                        process.StandardInput.Write(@"del clean.bat" + Environment.NewLine);
                        process.StandardInput.Close();
                        c = true;
                        
                        msg_out.Append("Delete clean.bat");
                        textBox1.Text = msg_out.ToString();
                        textBox1.ScrollToCaret();
                    }
                    process.WaitForExit();
                    process.Close();
                }
                if (r && c)
                {
                    msg_out.AppendLine("Defend Complete");
                    textBox1.Text = msg_out.ToString();
                    textBox1.ScrollToCaret();
                }
                else if (r)
                {
                    msg_out.AppendLine("Only run.bat exists and Delete");
                    textBox1.Text = msg_out.ToString();
                    textBox1.ScrollToCaret();
                }
                else if (c)
                {
                    msg_out.AppendLine("Only clean.bat exists and Delete");
                    textBox1.Text = msg_out.ToString();
                    textBox1.ScrollToCaret();
                }
                else
                {
                    msg_out.AppendLine("Nothing here");
                    textBox1.Text = msg_out.ToString();
                    textBox1.ScrollToCaret();
                }
            }
            else if (!Static.Checked && Dynamic.Checked)
            {
                msg_out.AppendLine("Dynamic Detection Start");
                textBox1.Text = msg_out.ToString();
                textBox1.ScrollToCaret();

                log_det = new Thread(new ThreadStart(log_search));
                 mem_det = new Thread(new ThreadStart(mem_search));
                 def = new Thread(new ThreadStart(defend_));

                 mem_det.Start();
                 log_det.Start(); 

                 dyn = new Thread(new ThreadStart(dyn_det));
                 dyn.Start();

            }
            else if (Static.Checked && Dynamic.Checked)
            {
                MessageBox.Show("Please check only one box");
                Static.Checked = false;
                Dynamic.Checked = false;
            }
            else
            {
                MessageBox.Show("Please check only one box");
            }
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            if (log_det.IsAlive && mem_det.IsAlive)
            {
                log_det.Abort();
                mem_det.Abort();
            }
            Static.Checked = false;
            Dynamic.Checked = false;

            msg_out.AppendLine("Stop Detection");
            textBox1.Text = msg_out.ToString();
            textBox1.ScrollToCaret();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Application.OpenForms["MainForm"].Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void mem_search()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            int i = 1;

            while (true)
            {
                Process[] allProc = Process.GetProcesses();
                foreach (Process p in allProc)
                {
                    StringBuilder proc = new StringBuilder();
                    if (p.ProcessName.Equals("powershell"))
                    {
                        if (parent.Parent(p).ProcessName.Equals("cmd") && parent.Parent(parent.Parent(p)).ProcessName.Equals("svchost"))
                        {
                            ps = p;
                            break;
                        }
                    }
                }
                if (ps != null)
                {
                    break; //powershell 찾음
                }
                else
                {
                    i++;
                }
            }
            msg_out.AppendLine("Malware Powershell Detect");
            textBox1.Text = msg_out.ToString();
            textBox1.ScrollToCaret();

            while (true)
            {
                if (Process.GetProcessById(ps.Id).PrivateMemorySize64 > 104857600)
                {
                    //100mb 넘게 할당되는  곳
                    msg_out.AppendLine("Memory, Detection");
                    textBox1.Text = msg_out.ToString();
                    textBox1.ScrollToCaret();

                    mem = true;
                    break;
                }
                else
                {
                    i++;
                }
            }
        }
        private void log_search()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            
            while (true)
            {
                string sBackup = "PowerShell";
                EventLog log_powershell = new EventLog();
                log_powershell.Source = sBackup;

                var query = from EventLogEntry entry in log_powershell.Entries orderby entry.TimeGenerated descending select entry;
                const char weirdChar = (char)3;
                string sBackupName = sBackup + "Log";
                var xml = new XDocument(
                    new XElement(sBackupName,
                    from EventLogEntry entry in log_powershell.Entries
                    orderby entry.TimeGenerated descending
                    select new XElement("Log",
                    new XElement("Message", entry.Message.Replace(weirdChar.ToString(), "")),
                    new XElement("TimeGenerared", entry.TimeGenerated),
                    new XElement("Source", entry.Source),
                    new XElement("EntryType", entry.EntryType.ToString())
                        )
                    )
                );
                string fileName = String.Format("{0}.txt", sBackupName);
                xml.Save(Path.Combine(Environment.CurrentDirectory, fileName));

                string path_file = String.Format("{0}\\{1}", Environment.CurrentDirectory, fileName);
                string[] text_line = System.IO.File.ReadAllLines(path_file);
                string the_line = null;

                for (int i = 0; i < text_line.Length; i++)
                {
                    if (text_line[i].Length > 16)
                    {
                        if (text_line[i].Substring(0, 16).Equals("\tHostApplication"))
                        {
                            the_line = text_line[i];
                            break;
                        }
                    }
                }
                the_line = the_line.Remove(0, 17);

                bool v = false;
                bool c = false;
                bool m = false;
                bool r = false;

                bool v_b = false;
                bool c_b = false;
                bool m_b = false;
                bool r_b = false;

                string temp = null;
                if (the_line.Length > 33) temp = the_line.Remove(0, 33);

                if (Base64.IsBase64String(temp))
                {
                    byte[] Decoding_64_arr = Convert.FromBase64String(temp);
                    string Decoding_64 = Encoding.Unicode.GetString(Decoding_64_arr);
                    v_b = Decoding_64.Contains("VirtualAlloc");
                    c_b = Decoding_64.Contains("CreateThread");
                    m_b = Decoding_64.Contains("memset");
                    r_b = Decoding_64.Contains("ReadAllBytes");
                }
                else
                {
                    v = the_line.Contains("VirtualAlloc");
                    c = the_line.Contains("CreateThread");
                    m = the_line.Contains("memeset");
                    r = the_line.Contains("ReadAllBytes");
                }

                if ((v && c && m && r) || (v_b && c_b && m_b && r_b)) //탐지 -> 차단 조건문
                {
                    msg_out.AppendLine("APIs, Detection");
                    textBox1.Text = msg_out.ToString();
                    textBox1.ScrollToCaret();

                    log = true;
                    break;
                }
            }
        }
       
        private void dyn_det()
        {
            while (true)
            {
                if (mem == true && log == true) // || or &&
                {
                    if (log_det.IsAlive) log_det.Abort();
                    if (mem_det.IsAlive) mem_det.Abort();
                    
                    defend = true;
                    msg_out.AppendLine("Warning! Must need Defend!");
                    textBox1.Text = msg_out.ToString();
                    textBox1.ScrollToCaret();
                    MessageBox.Show(new Form() { WindowState = FormWindowState.Maximized, TopMost = true }, "Warning! Must need Defend!");

                    def.Start();
                    break;
                }
            }
        }
        private void defend_()
        {
            msg_out.AppendLine("Kill Malware Process");
            textBox1.Text = msg_out.ToString();
            textBox1.ScrollToCaret();
            ps.Kill();

            string task_path = @"C:\Windows\System32\Tasks";
            System.IO.DirectoryInfo task_dir = new System.IO.DirectoryInfo(task_path);
            foreach(var item in task_dir.GetFiles())
            {
                MessageBox.Show(item.CreationTime.ToString());
                if(item.CreationTime.ToString().Contains(DateTime.Now.ToString()))
                {
                    MessageBox.Show(new Form() { WindowState = FormWindowState.Maximized, TopMost = true }, "hihi");

                }
            }

            msg_out.AppendLine("Defend Complete");
            textBox1.Text = msg_out.ToString();
            textBox1.ScrollToCaret();
            MessageBox.Show(new Form() { WindowState = FormWindowState.Maximized, TopMost = true }, "Defend Complete");

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
    public static class Base64
    {
        public static bool IsBase64String(this string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }
    }
    public static class parent
    {
        private static string FindIndexedProcessName(int pid)
        {
            var processName = Process.GetProcessById(pid).ProcessName;
            var processesByName = Process.GetProcessesByName(processName);
            string processIndexdName = null;

            for (var index = 0; index < processesByName.Length; index++)
            {
                processIndexdName = index == 0 ? processName : processName + "#" + index;
                var processId = new PerformanceCounter("Process", "ID Process", processIndexdName);
                if ((int)processId.NextValue() == pid)
                {
                    return processIndexdName;
                }
            }
            return processIndexdName;
        }

        private static Process FindPidFromIndexedProcessName(string indexedProcessName)
        {
            var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
            return Process.GetProcessById((int)parentId.NextValue());
        }

        public static Process Parent(this Process process)
        {
            return FindPidFromIndexedProcessName(FindIndexedProcessName(process.Id));
        }
    }
    class AutoClosingMessageBox
    {

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        System.Threading.Timer _timeoutTimer; //쓰레드 타이머
        string _caption;

        const int WM_CLOSE = 0x0010; //close 명령 

        AutoClosingMessageBox(string text, string caption, int timeout)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            MessageBox.Show(text, caption);
        }

        //생성자 함수
        public static void Show(string text, string caption, int timeout)
        {
            new AutoClosingMessageBox(text, caption, timeout);
        }

        //시간이 다되면 close 메세지를 보냄
        void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow(null, _caption);
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
        }
    }
}