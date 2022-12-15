using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000077 RID: 119
	public class GrassBlock : CubeBlock
	{
		// Token: 0x06000289 RID: 649 RVA: 0x0000F323 File Offset: 0x0000D523
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4)
			{
				return 0;
			}
			if (face == 5)
			{
				return 2;
			}
			if (Terrain.ExtractData(value) == 0)
			{
				return 3;
			}
			return 68;
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0000F340 File Offset: 0x0000D540
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			Color topColor = color * BlockColorsMap.GrassColorsMap.Lookup(environmentData.Temperature, environmentData.Humidity);
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, topColor, environmentData);
		}

		// Token: 0x0600028B RID: 651 RVA: 0x0000F380 File Offset: 0x0000D580
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			Color topColor = BlockColorsMap.GrassColorsMap.Lookup(generator.Terrain, x, y, z);
			Color topColor2 = BlockColorsMap.GrassColorsMap.Lookup(generator.Terrain, x + 1, y, z);
			Color topColor3 = BlockColorsMap.GrassColorsMap.Lookup(generator.Terrain, x + 1, y, z + 1);
			Color topColor4 = BlockColorsMap.GrassColorsMap.Lookup(generator.Terrain, x, y, z + 1);
			generator.GenerateCubeVertices(this, value, x, y, z, 1f, 1f, 1f, 1f, Color.White, topColor, topColor2, topColor3, topColor4, -1, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0000F423 File Offset: 0x0000D623
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, 2);
		}

		// Token: 0x04000123 RID: 291
		public const int Index = 8;
	}
}
