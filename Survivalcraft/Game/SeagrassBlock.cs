using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000EA RID: 234
	public class SeagrassBlock : WaterPlantBlock
	{
		// Token: 0x06000467 RID: 1127 RVA: 0x000178BA File Offset: 0x00015ABA
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face < 0)
			{
				return 105;
			}
			return base.GetFaceTextureSlot(face, value);
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x000178CC File Offset: 0x00015ACC
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color * BlockColorsMap.SeagrassColorsMap.Lookup(environmentData.Temperature, environmentData.Humidity), false, environmentData);
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x00017908 File Offset: 0x00015B08
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			Color color = BlockColorsMap.SeagrassColorsMap.Lookup(generator.Terrain, x, y, z);
			generator.GenerateCrossfaceVertices(this, value, x, y, z, color, this.GetFaceTextureSlot(-1, value), geometry.SubsetAlphaTest);
			base.GenerateTerrainVertices(generator, geometry, value, x, y, z);
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x00017958 File Offset: 0x00015B58
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return new BlockDebrisParticleSystem(subsystemTerrain, position, 0.75f * strength, this.DestructionDebrisScale, BlockColorsMap.SeagrassColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z)), 104);
		}

		// Token: 0x040001F9 RID: 505
		public new const int Index = 233;
	}
}
