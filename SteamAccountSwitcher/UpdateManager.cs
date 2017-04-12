using System.IO;
using System.Net;

namespace SteamAccountSwitcher
{
	static class UpdateManager
	{
		static bool RequiresUpdate()
		{
			if(!File.Exists("version.txt") || !File.Exists("main.ahk"))
				return true;
			using(var wc = new WebClient())
			{
				if(File.ReadAllText("version.txt") != wc.DownloadString("https://judge2020.com/bnet/version.txt"))
					return true;
			}
			return false;
		}

		static void PerformUpdate()
		{
			if(File.Exists("version.txt"))
				File.Delete("version.txt");
			if(File.Exists("main.ahk"))
				File.Delete("main.ahk");
			using(var wc = new WebClient())
			{
				wc.DownloadFile("https://judge2020.com/bnet/version.txt", "version.txt");
				wc.DownloadFile("https://judge2020.com/bnet/main.ahk", "main.ahk");
			}
		}

		public static void Updater()
		{
			if(!RequiresUpdate())
				return;
			PerformUpdate();
		}
	}
}
