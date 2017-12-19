using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BattlenetAccountSwitcher
{
    class Steam
    {
        string _installDir;

        private List<Process> _openWindows = new List<Process>();

        public Steam(string installDir)
        {
            _installDir = installDir;
            _autoStartOccurancess = 0;
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

        private System.Windows.Forms.Timer _autoStarTimer;
        private int _autoStartOccurancess;
        private SteamAccount _recentSteamAccount;
        public void HandleAutoStart(object sender, EventArgs ev)
        {
            _autoStartOccurancess += 1;
            if (_autoStartOccurancess >= 15)
            {
                // Handle if bnet never opens for 15 seconds
                _autoStartOccurancess = 0;
                _autoStarTimer.Stop();
                return;
            }
            // look for the battle.net main window

            Process[] processlist = Process.GetProcesses();
            foreach (var process in processlist)
            {
                if (process.MainWindowTitle != "Blizzard Battle.net") continue;
                _autoStartOccurancess = 16;
                switch (_recentSteamAccount.AutoStart)
                {
                    case AutoStart.None:
                        break;
                    case AutoStart.Warcraft:
                        Process.Start("battlenet://WoW");
                        break;
                    case AutoStart.Diablo3:
                        Process.Start("battlenet://D3");
                        break;
                    case AutoStart.Starcraft2:
                        Process.Start("battlenet://SC2");
                        break;
                    case AutoStart.Hearthstone:
                        Process.Start("battlenet://WTCG");
                        break;
                    case AutoStart.HOTS:
                        Process.Start("battlenet://Hero");
                        break;
                    case AutoStart.Overwatch:
                        Process.Start("battlenet://Pro");
                        break;
                    case AutoStart.Starcraft1:
                        Process.Start("battlenet://SC1");
                        break;
                    case AutoStart.Destiny2:
                        Process.Start("battlenet://dst2");
                        break;
                }
            }
        }

        public void ScheduleHandleAutoStart()
        {
            _autoStarTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            _autoStarTimer.Tick += HandleAutoStart;
            _autoStarTimer.Start();
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
            FileUpdateManager.Updater();
            Process ahk = new Process {StartInfo = new ProcessStartInfo(@"main.ahk", b.Username + " " + b.Password)};
            ahk.Start();
            _recentSteamAccount = b;
            ScheduleHandleAutoStart();
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
