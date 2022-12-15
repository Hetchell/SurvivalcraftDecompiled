using System;
using Engine;

namespace Game
{
	// Token: 0x02000164 RID: 356
	public class SubsystemBottomSuckerBlockBehavior : SubsystemInWaterBlockBehavior
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000706 RID: 1798 RVA: 0x0002C6B7 File Offset: 0x0002A8B7
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x0002C6C0 File Offset: 0x0002A8C0
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
			int face = BottomSuckerBlock.GetFace(Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z)));
			Point3 point = CellFace.FaceToPoint3(CellFace.OppositeFace(face));
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x + point.X, y + point.Y, z + point.Z);
			if (!this.IsSupport(cellValue, face))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x0002C74C File Offset: 0x0002A94C
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			if (Terrain.ExtractContents(base.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z)) == 226)
			{
				ComponentCreature componentCreature = componentBody.Entity.FindComponent<ComponentCreature>();
				if (componentCreature == null)
				{
					return;
				}
				componentCreature.ComponentHealth.Injure(0.01f * MathUtils.Abs(velocity), null, false, "Spiked by a sea creature");
			}
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x0002C7B4 File Offset: 0x0002A9B4
		public bool IsSupport(int value, int face)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
			return block.IsCollidable && !block.IsFaceTransparent(base.SubsystemTerrain, CellFace.OppositeFace(face), value);
		}
	}
}
