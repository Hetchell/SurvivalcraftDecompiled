using System;
using Engine;
using Engine.Input;

namespace Game
{
	// Token: 0x0200026A RID: 618
	public static class ExceptionManager
	{
		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x0600125F RID: 4703 RVA: 0x0008DF50 File Offset: 0x0008C150
		public static Exception Error
		{
			get
			{
				return ExceptionManager.m_error;
			}
		}

		// Token: 0x06001260 RID: 4704 RVA: 0x0008DF57 File Offset: 0x0008C157
		public static void ReportExceptionToUser(string additionalMessage, Exception e)
		{
			Log.Error(ExceptionManager.MakeFullErrorMessage(additionalMessage, e) + "\n" + e.StackTrace);
			AnalyticsManager.LogError(additionalMessage, e);
		}

		// Token: 0x06001261 RID: 4705 RVA: 0x0008DF7C File Offset: 0x0008C17C
		public static void DrawExceptionScreen()
		{
		}

		// Token: 0x06001262 RID: 4706 RVA: 0x0008DF7E File Offset: 0x0008C17E
		public static void UpdateExceptionScreen()
		{
		}

		// Token: 0x06001263 RID: 4707 RVA: 0x0008DF80 File Offset: 0x0008C180
		public static string MakeFullErrorMessage(Exception e)
		{
			return ExceptionManager.MakeFullErrorMessage(null, e);
		}

		// Token: 0x06001264 RID: 4708 RVA: 0x0008DF8C File Offset: 0x0008C18C
		public static string MakeFullErrorMessage(string additionalMessage, Exception e)
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(additionalMessage))
			{
				text = additionalMessage;
			}
			for (Exception ex = e; ex != null; ex = ex.InnerException)
			{
				text = text + ((text.Length > 0) ? Environment.NewLine : string.Empty) + ex.Message;
			}
			return text;
		}

		// Token: 0x06001265 RID: 4709 RVA: 0x0008DFDA File Offset: 0x0008C1DA
		public static bool CheckContinueKey()
		{
			return Keyboard.IsKeyDown(Key.F11) || Keyboard.IsKeyDown(Key.Back);
		}

		// Token: 0x04000C9B RID: 3227
		public static Exception m_error;
	}
}
