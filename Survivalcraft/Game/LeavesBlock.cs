using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200009F RID: 159
	public abstract class LeavesBlock : AlphaTestCubeBlock
	{
		// Token: 0x06000306 RID: 774 RVA: 0x000118D0 File Offset: 0x0000FAD0
		public LeavesBlock(BlockColorsMap blockColorsMap)
		{
			this.m_blockColorsMap = blockColorsMap;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x000118EC File Offset: 0x0000FAEC
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			Color color = this.m_blockColorsMap.Lookup(generator.Terrain, x, y, z);
			generator.GenerateCubeVertices(this, value, x, y, z, color, geometry.AlphaTestSubsetsByFace);
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00011926 File Offset: 0x0000FB26
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			color *= this.m_blockColorsMap.Lookup(environmentData.Temperature, environmentData.Humidity);
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}

		// Token: 0x06000309 RID: 777 RVA: 0x00011960 File Offset: 0x0000FB60
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			Color color = this.m_blockColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, color, this.GetFaceTextureSlot(4, value));
		}

		// Token: 0x0600030A RID: 778 RVA: 0x000119B8 File Offset: 0x0000FBB8
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			if (this.m_random.Bool(0.15f))
			{
				dropValues.Add(new BlockDropValue
				{
					Value = 23,
					Count = 1
				});
				showDebris = true;
				return;
			}
			base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
		}

		// Token: 0x04000165 RID: 357
		public BlockColorsMap m_blockColorsMap;

		// Token: 0x04000166 RID: 358
		public Game.Random m_random = new Game.Random();
	}
}
