using System;

namespace Game
{
	// Token: 0x02000293 RID: 659
	public interface IEditableItemData
	{
		// Token: 0x0600133B RID: 4923
		IEditableItemData Copy();

		// Token: 0x0600133C RID: 4924
		void LoadString(string data);

		// Token: 0x0600133D RID: 4925
		string SaveString();
	}
}
