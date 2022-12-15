using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000171 RID: 369
	public class SubsystemDoorBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060007E8 RID: 2024 RVA: 0x00033C85 File Offset: 0x00031E85
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					56,
					57,
					58
				};
			}
		}

		// Token: 0x060007E9 RID: 2025 RVA: 0x00033C98 File Offset: 0x00031E98
		public bool OpenCloseDoor(int x, int y, int z, bool open)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			if (BlocksManager.Blocks[num] is DoorBlock)
			{
				int data = DoorBlock.SetOpen(Terrain.ExtractData(cellValue), open);
				int value = Terrain.ReplaceData(cellValue, data);
				base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
				string name = open ? "Audio/Doors/DoorOpen" : "Audio/Doors/DoorClose";
				base.SubsystemTerrain.Project.FindSubsystem<SubsystemAudio>(true).PlaySound(name, 0.7f, SubsystemDoorBlockBehavior.m_random.Float(-0.1f, 0.1f), new Vector3((float)x, (float)y, (float)z), 4f, true);
				return true;
			}
			return false;
		}

		// Token: 0x060007EA RID: 2026 RVA: 0x00033D4C File Offset: 0x00031F4C
		public bool IsDoorElectricallyConnected(int x, int y, int z)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			if (BlocksManager.Blocks[num] is DoorBlock)
			{
				int num2 = DoorBlock.IsBottomPart(base.SubsystemTerrain.Terrain, x, y, z) ? y : (y - 1);
				for (int i = num2; i <= num2 + 1; i++)
				{
					ElectricElement electricElement = this.m_subsystemElectricity.GetElectricElement(x, i, z, DoorBlock.GetHingeFace(data));
					if (electricElement != null && electricElement.Connections.Count > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x00033DE0 File Offset: 0x00031FE0
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			CellFace cellFace = raycastResult.CellFace;
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			if (num == 56 || !this.IsDoorElectricallyConnected(cellFace.X, cellFace.Y, cellFace.Z))
			{
				bool open = DoorBlock.GetOpen(data);
				return this.OpenCloseDoor(cellFace.X, cellFace.Y, cellFace.Z, !open);
			}
			return true;
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x00033E68 File Offset: 0x00032068
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
			int cellContents2 = base.SubsystemTerrain.Terrain.GetCellContents(x, y + 1, z);
			if (!BlocksManager.Blocks[cellContents].IsTransparent && cellContents2 == 0)
			{
				base.SubsystemTerrain.ChangeCell(x, y + 1, z, value, true);
			}
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x00033ECC File Offset: 0x000320CC
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			if (DoorBlock.IsTopPart(base.SubsystemTerrain.Terrain, x, y, z))
			{
				base.SubsystemTerrain.ChangeCell(x, y - 1, z, 0, true);
			}
			if (DoorBlock.IsBottomPart(base.SubsystemTerrain.Terrain, x, y, z))
			{
				base.SubsystemTerrain.ChangeCell(x, y + 1, z, 0, true);
			}
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x00033F30 File Offset: 0x00032130
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			Block block = BlocksManager.Blocks[num];
			int data = Terrain.ExtractData(cellValue);
			if (!(block is DoorBlock))
			{
				return;
			}
			if (neighborX == x && neighborY == y && neighborZ == z)
			{
				if (DoorBlock.IsBottomPart(base.SubsystemTerrain.Terrain, x, y, z))
				{
					int value = Terrain.ReplaceData(base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z), data);
					base.SubsystemTerrain.ChangeCell(x, y + 1, z, value, true);
				}
				if (DoorBlock.IsTopPart(base.SubsystemTerrain.Terrain, x, y, z))
				{
					int value2 = Terrain.ReplaceData(base.SubsystemTerrain.Terrain.GetCellValue(x, y - 1, z), data);
					base.SubsystemTerrain.ChangeCell(x, y - 1, z, value2, true);
				}
			}
			if (DoorBlock.IsBottomPart(base.SubsystemTerrain.Terrain, x, y, z))
			{
				int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
				if (BlocksManager.Blocks[cellContents].IsTransparent)
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
				}
			}
			if (!DoorBlock.IsBottomPart(base.SubsystemTerrain.Terrain, x, y, z) && !DoorBlock.IsTopPart(base.SubsystemTerrain.Terrain, x, y, z))
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

        // Token: 0x060007EF RID: 2031 RVA: 0x00034097 File Offset: 0x00032297
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemElectricity = base.Project.FindSubsystem<SubsystemElectricity>(true);
		}

		// Token: 0x04000429 RID: 1065
		public SubsystemElectricity m_subsystemElectricity;

		// Token: 0x0400042A RID: 1066
		public static Game.Random m_random = new Game.Random();
	}
}
