using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BattlenetAccountSwitcher
{
    class Steam
    {
        string _installDir;

        private List<Process> _openWindows = new List<Process>();

        public Steam(string installDir)
        {
            _installDir = installDir;
        }

        public string InstallDir
        {
            get { return _installDir; }
            set { _installDir = value; }
        }

        public bool IsSteamRunning()
        {
            Process[] pname = Process.GetProcessesByName("Battle.net.exe");
            if (pname.Length == 0)
                return false;
            else
                return true;
        }

        public void KillSteam()
        {
            Process [] proc = Process.GetProcessesByName("Battle.net.exe");
	        proc[0].Kill();
        }

        public bool StartSteamAccount(SteamAccount a)
        {
            LoginBnet(a);
            return false;
        }

        public void ClearProcesses(object sender, EventArgs ev)
        {
            foreach (var window in _openWindows)
            {
                try
                {
                    window.Kill();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            _openWindows.Clear();
        }

        public void ScheduleClearProcesses()
        {
            var myTimer = new System.Windows.Forms.Timer
            {
                Interval = 5000
            };
            myTimer.Tick += ClearProcesses;
            myTimer.Start();
        }


        public bool LogoutSteam()
        {
            //Process.Start(installDir, " --exec = \"logout\"");
            ProcessStartInfo hiddenInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                Arguments = "/k \"\"" + InstallDir + "\" \"--exec=\"logout\"\"\" && exit",
                FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe"),
                WindowStyle = ProcessWindowStyle.Hidden
            };
            Process proc = new Process();
            proc.StartInfo = hiddenInfo;
            proc.Start();
            _openWindows.Add(proc);
            ScheduleClearProcesses();
            return true;

        }

        public bool LoginBnet(SteamAccount b)
        {
            if (!IsSteamRunning())
            {
                LogoutSteam();
            }
            UpdateManager.Updater();
            Process ahk = new Process {StartInfo = new ProcessStartInfo(AppDomain.CurrentDomain.BaseDirectory + @"\main.ahk", b.Username + " " + b.Password)};
            ahk.Start();
            return true;
        }
        /*public bool StartAhk()
        {
            if (DownloadMissing.CheckIfMissing())
            {
                DownloadMissing.Download();
            }
            AutoHotkeyEngine ahk = AutoHotkeyEngine.Instance;
            ahk.LoadFile(AppDomain.CurrentDomain.BaseDirectory + @"\main.ahk");

            return true;
        }
        */
        
    }
}
