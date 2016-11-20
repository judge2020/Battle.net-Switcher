using System;
using System.Diagnostics;
using System.IO;
using AutoHotkey.Interop;

namespace SteamAccountSwitcher
{
    class Steam
    {
        string _installDir;

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

            //LogoutSteam();
            LoginBnet(a);

            return false;
        }




        public bool LogoutSteam()
        {
            //Process.Start(installDir, " --exec = \"logout\"");
            
            if (File.Exists(_installDir))
            {
                Process proc = Process.Start(Environment.SystemDirectory + @"\cmd.exe", "cmd /k \"\"" + InstallDir + "\" \"--exec=\"logout\"\"\" && exit");
                if (proc != null)
                {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();
                }

                return true;
            }
            return false;
            
        }

        public bool LoginBnet(SteamAccount b)
        {
            if (!IsSteamRunning())
            {
                LogoutSteam();
            }
            if (DownloadMissing.CheckIfMissing())
            {
                DownloadMissing.Download();
            }
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
