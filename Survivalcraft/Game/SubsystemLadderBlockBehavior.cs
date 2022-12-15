using System;
using Engine;

namespace Game
{
	// Token: 0x0200018B RID: 395
	public class SubsystemLadderBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000909 RID: 2313 RVA: 0x0003E2A9 File Offset: 0x0003C4A9
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					59,
					213
				};
			}
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x0003E2C0 File Offset: 0x0003C4C0
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int face = LadderBlock.GetFace(Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z)));
			Point3 point = CellFace.FaceToPoint3(face);
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x - point.X, y - point.Y, z - point.Z);
			int num = Terrain.ExtractContents(cellValue);
			if (BlocksManager.Blocks[num].IsFaceTransparent(base.SubsystemTerrain, face, cellValue))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}
	}
}
