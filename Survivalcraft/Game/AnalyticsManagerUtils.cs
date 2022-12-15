using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000217 RID: 535
	public static class AnalyticsManagerUtils
	{
		// Token: 0x0600107E RID: 4222 RVA: 0x0007DD98 File Offset: 0x0007BF98
		public static string AbbreviateStackTrace(string stackTrace)
		{
			stackTrace = stackTrace.Replace("System.Collections.Generic.", "");
			stackTrace = stackTrace.Replace("System.Collections.", "");
			stackTrace = stackTrace.Replace("System.IO.", "");
			stackTrace = stackTrace.Replace("Engine.Audio.", "");
			stackTrace = stackTrace.Replace("Engine.Input.", "");
			stackTrace = stackTrace.Replace("Engine.Graphics.", "");
			stackTrace = stackTrace.Replace("Engine.", "");
			if (stackTrace.StartsWith("Engine."))
			{
				stackTrace = stackTrace.Substring("Engine.".Length);
			}
			if (stackTrace.StartsWith("Game."))
			{
				stackTrace = stackTrace.Substring("Game.".Length);
			}
			if (stackTrace.StartsWith("System."))
			{
				stackTrace = stackTrace.Substring("System.".Length);
			}
			return stackTrace;
		}

		// Token: 0x0600107F RID: 4223 RVA: 0x0007DE84 File Offset: 0x0007C084
		public static string[] SplitStackTrace(string stackTrace)
		{
			List<string> list = new List<string>();
			do
			{
				string text = stackTrace.Substring(0, MathUtils.Min(stackTrace.Length, 254));
				list.Add(text);
				stackTrace = stackTrace.Remove(0, text.Length);
			}
			while (stackTrace.Length > 0 && list.Count < 4);
			return list.ToArray();
		}

		// Token: 0x06001080 RID: 4224 RVA: 0x0007DEE0 File Offset: 0x0007C0E0
		public static AnalyticsParameter[] CreateAnalyticsParametersForError(string message, Exception error)
		{
			string text = ExceptionManager.MakeFullErrorMessage(message, error);
			if (text.Length > 254)
			{
				text = text.Substring(0, 254);
			}
			string[] array = AnalyticsManagerUtils.SplitStackTrace(AnalyticsManagerUtils.AbbreviateStackTrace(error.StackTrace));
			return new AnalyticsParameter[]
			{
				new AnalyticsParameter("FullMessage", text),
				new AnalyticsParameter("StackTrace1", (array.Length >= 1) ? array[0] : string.Empty),
				new AnalyticsParameter("StackTrace2", (array.Length >= 2) ? array[1] : string.Empty),
				new AnalyticsParameter("StackTrace3", (array.Length >= 3) ? array[2] : string.Empty),
				new AnalyticsParameter("StackTrace4", (array.Length >= 4) ? array[3] : string.Empty),
				new AnalyticsParameter("Time", DateTime.Now.ToString("HH:mm:ss.fff"))
			};
		}
	}
}
