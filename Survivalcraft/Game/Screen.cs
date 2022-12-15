using System;

namespace Game
{
	// Token: 0x02000146 RID: 326
	public class Screen : CanvasWidget
	{
		// Token: 0x06000624 RID: 1572 RVA: 0x00024D6C File Offset: 0x00022F6C
		public Screen()
		{
			if (Screen.Init != null)
			{
				Screen.Init();
			}
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x00024D85 File Offset: 0x00022F85
		public virtual void Enter(object[] parameters)
		{
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x00024D87 File Offset: 0x00022F87
		public virtual void Leave()
		{
		}

		// Token: 0x04000312 RID: 786
		public static Action Init;
	}
}
