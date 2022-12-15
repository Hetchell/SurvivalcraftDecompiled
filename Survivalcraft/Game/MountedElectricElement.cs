using System;
using Engine;

namespace Game
{
	// Token: 0x020002B4 RID: 692
	public abstract class MountedElectricElement : ElectricElement
	{
		// Token: 0x060013D8 RID: 5080 RVA: 0x00099C0A File Offset: 0x00097E0A
		public MountedElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x00099C14 File Offset: 0x00097E14
		public override void OnNeighborBlockChanged(CellFace cellFace, int neighborX, int neighborY, int neighborZ)
		{
			Point3 point = CellFace.FaceToPoint3(cellFace.Face);
			int x = cellFace.X - point.X;
			int y = cellFace.Y - point.Y;
			int z = cellFace.Z - point.Z;
			if (base.SubsystemElectricity.SubsystemTerrain.Terrain.IsCellValid(x, y, z))
			{
				int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
				if ((!block.IsCollidable || block.IsFaceTransparent(base.SubsystemElectricity.SubsystemTerrain, cellFace.Face, cellValue)) && (cellFace.Face != 4 || !(block is FenceBlock)))
				{
					base.SubsystemElectricity.SubsystemTerrain.DestroyCell(0, cellFace.X, cellFace.Y, cellFace.Z, 0, false, false);
				}
			}
		}
	}
}
