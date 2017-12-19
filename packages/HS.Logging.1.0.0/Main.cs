using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HS.Logging
{
	[DebuggerStepThrough]
	public class Log
	{
		private static readonly Queue<string> LogQueue = new Queue<string>();
		private static bool Initialized { get; set; }

		public static bool Initialize(string logDir, string logName)
		{
			if(Initialized)
				return false;
			Trace.AutoFlush = true;
			if(!logName.Contains('.'))
				logName = logName + ".txt";
			var logFile = Path.Combine(logDir, logName);
			if(!Directory.Exists(logDir))
				Directory.CreateDirectory(logDir);
			else
			{
				try
				{
					var fileInfo = new FileInfo(logFile);
					if(fileInfo.Exists)
					{
						using (new FileStream(logFile, FileMode.Open, FileAccess.Read, FileShare.None))
						{
							//can access log file => no other instance of same installation running
						}
						var fileExtension = logName.Split('.').Last();
						File.Move(logFile, logFile.Replace(fileExtension, "_" + DateTime.Now.ToUnixTime() + fileExtension));
						
					}
					else
						File.Create(logFile).Dispose();
				}
				catch(Exception)
				{
					return false;
				}
			}
			try
			{
				Trace.Listeners.Add(new TextWriterTraceListener(new StreamWriter(logFile, false)));
			}
			catch(Exception)
			{
				return false;
			}
			Initialized = true;
			foreach(var line in LogQueue)
				Trace.WriteLine(line);
			return true;
		}

		public static void WriteLine(string msg, LogType type, [CallerMemberName] string memberName = "",
									 [CallerFilePath] string sourceFilePath = "")
		{
#if(!DEBUG)
			if(type == LogType.Debug && Config.Instance.LogLevel == 0)
				return;
#endif
			var file = sourceFilePath?.Split('/', '\\').LastOrDefault()?.Split('.').FirstOrDefault();
			var line = $"{DateTime.Now.ToLongTimeString()}|{type}|{file}.{memberName} >> {msg}";
			if(Initialized)
				Trace.WriteLine(line);
			else
				LogQueue.Enqueue(line);
		}

		public static void Debug(string msg, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
			=> WriteLine(msg, LogType.Debug, memberName, sourceFilePath);

		public static void Info(string msg, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
			=> WriteLine(msg, LogType.Info, memberName, sourceFilePath);

		public static void Warn(string msg, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
			=> WriteLine(msg, LogType.Warning, memberName, sourceFilePath);

		public static void Error(string msg, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
			=> WriteLine(msg, LogType.Error, memberName, sourceFilePath);

		public static void Error(Exception ex, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
			=> WriteLine(ex.ToString(), LogType.Error, memberName, sourceFilePath);
	}
}
