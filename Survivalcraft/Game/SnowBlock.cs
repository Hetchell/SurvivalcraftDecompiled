using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x020000F5 RID: 245
	public class SnowBlock : CubeBlock
	{
		// Token: 0x060004AB RID: 1195 RVA: 0x00018F3D File Offset: 0x0001713D
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return face != 5;
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x00018F48 File Offset: 0x00017148
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(this, value, x, y, z, 0.125f, 0.125f, 0.125f, 0.125f, Color.White, Color.White, Color.White, Color.White, Color.White, -1, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x00018F98 File Offset: 0x00017198
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			if (toolLevel >= this.RequiredToolLevel)
			{
				int num = this.Random.Int(1, 3);
				for (int i = 0; i < num; i++)
				{
					dropValues.Add(new BlockDropValue
					{
						Value = Terrain.MakeBlockValue(85),
						Count = 1
					});
				}
			}
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x00018FF3 File Offset: 0x000171F3
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.m_collisionBoxes;
		}

		// Token: 0x04000214 RID: 532
		public const int Index = 61;

		// Token: 0x04000215 RID: 533
		public BoundingBox[] m_collisionBoxes = new BoundingBox[]
		{
			new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.125f, 1f))
		};
	}
}
