using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200017A RID: 378
	public class SubsystemFenceGateBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600085C RID: 2140 RVA: 0x000385F9 File Offset: 0x000367F9
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x0600085D RID: 2141 RVA: 0x00038604 File Offset: 0x00036804
		public bool OpenCloseGate(int x, int y, int z, bool open)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			if (BlocksManager.Blocks[num] is FenceGateBlock)
			{
				int data = FenceGateBlock.SetOpen(Terrain.ExtractData(cellValue), open);
				int value = Terrain.ReplaceData(cellValue, data);
				base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
				string name = open ? "Audio/Doors/DoorOpen" : "Audio/Doors/DoorClose";
				base.SubsystemTerrain.Project.FindSubsystem<SubsystemAudio>(true).PlaySound(name, 0.7f, SubsystemFenceGateBlockBehavior.m_random.Float(-0.1f, 0.1f), new Vector3((float)x, (float)y, (float)z), 4f, true);
				return true;
			}
			return false;
		}

		// Token: 0x0600085E RID: 2142 RVA: 0x000386B8 File Offset: 0x000368B8
		public bool IsGateElectricallyConnected(int x, int y, int z)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			if (BlocksManager.Blocks[num] is FenceGateBlock)
			{
				ElectricElement electricElement = this.m_subsystemElectricity.GetElectricElement(x, y, z, FenceGateBlock.GetHingeFace(data));
				if (electricElement != null && electricElement.Connections.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x0003871C File Offset: 0x0003691C
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			CellFace cellFace = raycastResult.CellFace;
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			int num = Terrain.ExtractContents(cellValue);
			int data = Terrain.ExtractData(cellValue);
			if (num == 166 || !this.IsGateElectricallyConnected(cellFace.X, cellFace.Y, cellFace.Z))
			{
				bool open = FenceGateBlock.GetOpen(data);
				return this.OpenCloseGate(cellFace.X, cellFace.Y, cellFace.Z, !open);
			}
			return true;
		}

        // Token: 0x06000860 RID: 2144 RVA: 0x000387A6 File Offset: 0x000369A6
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemElectricity = base.Project.FindSubsystem<SubsystemElectricity>(true);
		}

		// Token: 0x04000468 RID: 1128
		public SubsystemElectricity m_subsystemElectricity;

		// Token: 0x04000469 RID: 1129
		public static Game.Random m_random = new Game.Random();
	}
}
