using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200017B RID: 379
	public class SubsystemFertilizerBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000863 RID: 2147 RVA: 0x000387D5 File Offset: 0x000369D5
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					102
				};
			}
		}

		// Token: 0x06000864 RID: 2148 RVA: 0x000387E4 File Offset: 0x000369E4
		public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			TerrainRaycastResult? terrainRaycastResult = componentMiner.Raycast<TerrainRaycastResult>(ray, RaycastMode.Interaction, true, true, true);
			if (terrainRaycastResult != null && terrainRaycastResult.Value.CellFace.Face == 4)
			{
				int y = terrainRaycastResult.Value.CellFace.Y;
				for (int i = terrainRaycastResult.Value.CellFace.X - 1; i <= terrainRaycastResult.Value.CellFace.X + 1; i++)
				{
					for (int j = terrainRaycastResult.Value.CellFace.Z - 1; j <= terrainRaycastResult.Value.CellFace.Z + 1; j++)
					{
						int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(i, y, j);
						if (Terrain.ExtractContents(cellValue) == 168)
						{
							int data = SoilBlock.SetNitrogen(Terrain.ExtractData(cellValue), 3);
							int value = Terrain.ReplaceData(cellValue, data);
							this.m_subsystemTerrain.ChangeCell(i, y, j, value, true);
						}
					}
				}
				this.m_subsystemAudio.PlayRandomSound("Audio/Impacts/Dirt", 0.5f, 0f, new Vector3((float)terrainRaycastResult.Value.CellFace.X, (float)terrainRaycastResult.Value.CellFace.Y, (float)terrainRaycastResult.Value.CellFace.Z), 3f, true);
				Vector3 position = new Vector3((float)terrainRaycastResult.Value.CellFace.X + 0.5f, (float)terrainRaycastResult.Value.CellFace.Y + 1.5f, (float)terrainRaycastResult.Value.CellFace.Z + 0.5f);
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(componentMiner.ActiveBlockValue)];
				this.m_subsystemParticles.AddParticleSystem(block.CreateDebrisParticleSystem(this.m_subsystemTerrain, position, componentMiner.ActiveBlockValue, 1.25f));
				componentMiner.RemoveActiveTool(1);
				return true;
			}
			return false;
		}

        // Token: 0x06000865 RID: 2149 RVA: 0x000389DB File Offset: 0x00036BDB
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
		}

		// Token: 0x0400046A RID: 1130
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400046B RID: 1131
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x0400046C RID: 1132
		public SubsystemAudio m_subsystemAudio;
	}
}
