using System;
using Engine;

namespace Game
{
	// Token: 0x02000082 RID: 130
	public interface IMovingBlockSet
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060002BF RID: 703
		Vector3 Position { get; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060002C0 RID: 704
		string Id { get; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060002C1 RID: 705
		object Tag { get; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060002C2 RID: 706
		Vector3 CurrentVelocity { get; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060002C3 RID: 707
		ReadOnlyList<MovingBlock> Blocks { get; }

		// Token: 0x060002C4 RID: 708
		BoundingBox BoundingBox(bool extendToFillCells);

		// Token: 0x060002C5 RID: 709
		void SetBlock(Point3 offset, int value);

		// Token: 0x060002C6 RID: 710
		void Stop();
	}
}
