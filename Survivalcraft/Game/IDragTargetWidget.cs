using System;

namespace Game
{
	// Token: 0x02000389 RID: 905
	public interface IDragTargetWidget
	{
		// Token: 0x060019EA RID: 6634
		void DragOver(Widget dragWidget, object data);

		// Token: 0x060019EB RID: 6635
		void DragDrop(Widget dragWidget, object data);
	}
}
