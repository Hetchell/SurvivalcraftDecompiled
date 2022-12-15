using System;
using System.Collections.Generic;
using System.IO;
using Engine;

namespace Game
{
	// Token: 0x02000283 RID: 643
	public class GameLogSink : ILogSink
	{
		// Token: 0x0600130D RID: 4877 RVA: 0x00094528 File Offset: 0x00092728
		public GameLogSink()
		{
			try
			{
				if (GameLogSink.m_stream != null)
				{
					throw new InvalidOperationException("GameLogSink already created.");
				}
				string text = "app:/Logs";
				string path = Storage.CombinePaths(new string[]
				{
					text,
					"Game.log"
				});
				Storage.CreateDirectory(text);
				GameLogSink.m_stream = Storage.OpenFile(path, OpenFileMode.CreateOrOpen);
				if (GameLogSink.m_stream.Length > 10485760L)
				{
					GameLogSink.m_stream.Dispose();
					GameLogSink.m_stream = Storage.OpenFile(path, OpenFileMode.Create);
				}
				GameLogSink.m_stream.Position = GameLogSink.m_stream.Length;
				GameLogSink.m_writer = new StreamWriter(GameLogSink.m_stream);
			}
			catch (Exception ex)
			{
				Engine.Log.Error("Error creating GameLogSink. Reason: {0}", new object[]
				{
					ex.Message
				});
			}
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x000945F8 File Offset: 0x000927F8
		public static string GetRecentLog(int bytesCount)
		{
			if (GameLogSink.m_stream == null)
			{
				return string.Empty;
			}
			Stream stream = GameLogSink.m_stream;
			string result;
			lock (stream)
			{
				try
				{
					GameLogSink.m_stream.Position = MathUtils.Max(GameLogSink.m_stream.Position - (long)bytesCount, 0L);
					result = new StreamReader(GameLogSink.m_stream).ReadToEnd();
				}
				finally
				{
					GameLogSink.m_stream.Position = GameLogSink.m_stream.Length;
				}
			}
			return result;
		}

		// Token: 0x0600130F RID: 4879 RVA: 0x00094690 File Offset: 0x00092890
		public static List<string> GetRecentLogLines(int bytesCount)
		{
			if (GameLogSink.m_stream == null)
			{
				return new List<string>();
			}
			Stream stream = GameLogSink.m_stream;
			List<string> result;
			lock (stream)
			{
				try
				{
					GameLogSink.m_stream.Position = MathUtils.Max(GameLogSink.m_stream.Position - (long)bytesCount, 0L);
					StreamReader streamReader = new StreamReader(GameLogSink.m_stream);
					List<string> list = new List<string>();
					for (;;)
					{
						string text = streamReader.ReadLine();
						if (text == null)
						{
							break;
						}
						list.Add(text);
					}
					result = list;
				}
				finally
				{
					GameLogSink.m_stream.Position = GameLogSink.m_stream.Length;
				}
			}
			return result;
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x00094744 File Offset: 0x00092944
		public void Log(LogType type, string message)
		{
			if (GameLogSink.m_stream != null)
			{
				Stream stream = GameLogSink.m_stream;
				lock (stream)
				{
					string value;
					switch (type)
					{
					case LogType.Debug:
						value = "DEBUG: ";
						break;
					case LogType.Verbose:
						value = "INFO: ";
						break;
					case LogType.Information:
						value = "INFO: ";
						break;
					case LogType.Warning:
						value = "WARNING: ";
						break;
					case LogType.Error:
						value = "ERROR: ";
						break;
					default:
						value = string.Empty;
						break;
					}
					GameLogSink.m_writer.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
					GameLogSink.m_writer.Write(" ");
					GameLogSink.m_writer.Write(value);
					GameLogSink.m_writer.WriteLine(message);
					GameLogSink.m_writer.Flush();
				}
			}
		}

		// Token: 0x04000D12 RID: 3346
		public static Stream m_stream;

		// Token: 0x04000D13 RID: 3347
		public static StreamWriter m_writer;
	}
}
