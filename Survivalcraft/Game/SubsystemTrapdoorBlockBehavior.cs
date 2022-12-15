using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001B3 RID: 435
	public class SubsystemTrapdoorBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000ACD RID: 2765 RVA: 0x0004FEF4 File Offset: 0x0004E0F4
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					83,
					84
				};
			}
		}

		// Token: 0x06000ACE RID: 2766 RVA: 0x0004FF08 File Offset: 0x0004E108
		public bool IsTrapdoorElectricallyConnected(int x, int y, int z)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			if (BlocksManager.Blocks[num] is TrapdoorBlock)
			{
				ElectricElement electricElement = this.m_subsystemElectricity.GetElectricElement(x, y, z, TrapdoorBlock.GetMountingFace(data));
				if (electricElement != null && electricElement.Connections.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x0004FF6C File Offset: 0x0004E16C
		public bool OpenCloseTrapdoor(int x, int y, int z, bool open)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			if (BlocksManager.Blocks[num] is TrapdoorBlock)
			{
				int data = TrapdoorBlock.SetOpen(Terrain.ExtractData(cellValue), open);
				int value = Terrain.ReplaceData(cellValue, data);
				base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
				string name = open ? "Audio/Doors/DoorOpen" : "Audio/Doors/DoorClose";
				base.SubsystemTerrain.Project.FindSubsystem<SubsystemAudio>(true).PlaySound(name, 0.7f, SubsystemTrapdoorBlockBehavior.m_random.Float(-0.1f, 0.1f), new Vector3((float)x, (float)y, (float)z), 4f, true);
				return true;
			}
			return false;
		}

		// Token: 0x06000AD0 RID: 2768 RVA: 0x00050020 File Offset: 0x0004E220
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			CellFace cellFace = raycastResult.CellFace;
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			if (num == 83 || !this.IsTrapdoorElectricallyConnected(cellFace.X, cellFace.Y, cellFace.Z))
			{
				bool open = TrapdoorBlock.GetOpen(data);
				return this.OpenCloseTrapdoor(cellFace.X, cellFace.Y, cellFace.Z, !open);
			}
			return true;
		}

		// Token: 0x06000AD1 RID: 2769 RVA: 0x000500A8 File Offset: 0x0004E2A8
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			Block block = BlocksManager.Blocks[num];
			int data = Terrain.ExtractData(cellValue);
			if (block is TrapdoorBlock)
			{
				int rotation = TrapdoorBlock.GetRotation(data);
				bool upsideDown = TrapdoorBlock.GetUpsideDown(data);
				bool flag = false;
				Point3 point = CellFace.FaceToPoint3(rotation);
				int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x - point.X, y - point.Y, z - point.Z);
				flag |= !BlocksManager.Blocks[cellContents].IsTransparent;
				if (upsideDown)
				{
					int cellContents2 = base.SubsystemTerrain.Terrain.GetCellContents(x, y + 1, z);
					flag |= !BlocksManager.Blocks[cellContents2].IsTransparent;
					int cellContents3 = base.SubsystemTerrain.Terrain.GetCellContents(x - point.X, y - point.Y + 1, z - point.Z);
					flag |= !BlocksManager.Blocks[cellContents3].IsTransparent;
				}
				else
				{
					int cellContents4 = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
					flag |= !BlocksManager.Blocks[cellContents4].IsTransparent;
					int cellContents5 = base.SubsystemTerrain.Terrain.GetCellContents(x - point.X, y - point.Y - 1, z - point.Z);
					flag |= !BlocksManager.Blocks[cellContents5].IsTransparent;
				}
				if (!flag)
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
				}
			}
		}

        // Token: 0x06000AD2 RID: 2770 RVA: 0x0005023C File Offset: 0x0004E43C
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemElectricity = base.Project.FindSubsystem<SubsystemElectricity>(true);
		}

		// Token: 0x040005EB RID: 1515
		public SubsystemElectricity m_subsystemElectricity;

		// Token: 0x040005EC RID: 1516
		public static Game.Random m_random = new Game.Random();
	}
}
