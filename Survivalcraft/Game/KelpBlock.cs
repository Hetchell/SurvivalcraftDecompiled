using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000099 RID: 153
	public class KelpBlock : WaterPlantBlock
	{
		// Token: 0x060002F1 RID: 753 RVA: 0x000113EA File Offset: 0x0000F5EA
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face < 0)
			{
				return 104;
			}
			return base.GetFaceTextureSlot(face, value);
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x000113FC File Offset: 0x0000F5FC
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color * BlockColorsMap.KelpColorsMap.Lookup(environmentData.Temperature, environmentData.Humidity), false, environmentData);
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00011438 File Offset: 0x0000F638
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			Color color = BlockColorsMap.KelpColorsMap.Lookup(generator.Terrain, x, y, z);
			generator.GenerateCrossfaceVertices(this, value, x, y, z, color, this.GetFaceTextureSlot(-1, value), geometry.SubsetAlphaTest);
			base.GenerateTerrainVertices(generator, geometry, value, x, y, z);
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00011488 File Offset: 0x0000F688
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return new BlockDebrisParticleSystem(subsystemTerrain, position, 0.75f * strength, this.DestructionDebrisScale, BlockColorsMap.KelpColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z)), 104);
		}

		// Token: 0x0400015A RID: 346
		public new const int Index = 232;
	}
}
