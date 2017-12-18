using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Squirrel;

namespace BattlenetAccountSwitcher
{
	static class FileUpdateManager
	{
		private static DispatcherTimer _dispatcher;
		public static void PerformStartupCheck()
		{
			_dispatcher = new DispatcherTimer();
			_dispatcher.Tick += CheckForUpdates;
			_dispatcher.Interval = new TimeSpan(0, 0, 0, 3);
			_dispatcher.Start();
		}
		public static async void CheckForUpdates(object sender, EventArgs eventArgs)
		{
			_dispatcher.Stop();
			try
			{
				using (var mgr = await GetUpdateManager())
					await SquirrelUpdate(mgr);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static async Task<UpdateManager> GetUpdateManager()
		{
			return await UpdateManager.GitHubUpdateManager("https://github.com/judge2020/bnet-switcher-releases");
		}

		private static async Task<bool> SquirrelUpdate(UpdateManager mgr, bool ignoreDelta = false)
		{
			try
			{
				Console.WriteLine($"Checking for updates (ignoreDelta={ignoreDelta})");
				var updateInfo = await mgr.CheckForUpdate(ignoreDelta);
				if (!updateInfo.ReleasesToApply.Any())
				{
					Console.WriteLine("No new updates available");
					return false;
				}
				var latest = updateInfo.ReleasesToApply.LastOrDefault()?.Version;
				var current = mgr.CurrentlyInstalledVersion();
				if (latest <= current)
				{
					Console.WriteLine($"Installed version ({current}) is greater than latest release found ({latest}). Not downloading updates.");
					return false;
				}
				if (IsRevisionIncrement(current?.Version, latest?.Version))
				{
					Console.WriteLine($"Latest update ({latest}) is revision increment. Updating in background.");

				}
				Console.WriteLine($"Downloading {updateInfo.ReleasesToApply.Count} {(ignoreDelta ? "" : "delta ")}releases, latest={latest?.Version}");
				await mgr.DownloadReleases(updateInfo.ReleasesToApply);
				Console.WriteLine("Applying releases");
				await mgr.ApplyReleases(updateInfo);
				await mgr.CreateUninstallerRegistryEntry();
				Console.WriteLine("Done");
				return true;
			}
			catch (Exception ex)
			{
				if (ignoreDelta)
					return false;
				if (ex is Win32Exception)
					Console.WriteLine("Not able to apply deltas, downloading full release");
				return await SquirrelUpdate(mgr, true);
			}
		}
		internal static void StartUpdate()
		{
			Console.WriteLine("Restarting...");
			UpdateManager.RestartApp();
		}

		private static bool IsRevisionIncrement(Version current, Version latest)
		{
			if (current == null || latest == null)
				return false;
			return current.Major == latest.Major && current.Minor == latest.Minor && current.Build == latest.Build
				   && current.Revision < latest.Revision;
		}

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
