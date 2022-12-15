using System.Windows.Forms;

namespace Game
{
	// Token: 0x0200023C RID: 572
	public static class ClipboardManager
	{
		// Token: 0x1700028B RID: 651
		// (get) Token: 0x060011A1 RID: 4513 RVA: 0x000883E6 File Offset: 0x000865E6
		// (set) Token: 0x060011A2 RID: 4514 RVA: 0x000883ED File Offset: 0x000865ED
		public static string ClipboardString
		{
			get
			{
				return Clipboard.GetText();
			}
			set
			{
				Clipboard.SetText(value);
			}
		}
	}
}
