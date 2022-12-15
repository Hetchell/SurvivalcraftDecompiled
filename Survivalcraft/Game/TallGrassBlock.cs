using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200010E RID: 270
	public class TallGrassBlock : CrossBlock
	{
		// Token: 0x0600051A RID: 1306 RVA: 0x0001BDE8 File Offset: 0x00019FE8
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			if (!TallGrassBlock.GetIsSmall(data))
			{
				dropValues.Add(new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(19, 0, data),
					Count = 1
				});
			}
			showDebris = true;
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x0001BE30 File Offset: 0x0001A030
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (!TallGrassBlock.GetIsSmall(Terrain.ExtractData(value)))
			{
				return 85;
			}
			return 84;
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x0001BE44 File Offset: 0x0001A044
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color * BlockColorsMap.GrassColorsMap.Lookup(environmentData.Temperature, environmentData.Humidity), false, environmentData);
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x0001BE80 File Offset: 0x0001A080
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateCrossfaceVertices(this, value, x, y, z, BlockColorsMap.GrassColorsMap.Lookup(generator.Terrain, x, y, z), this.GetFaceTextureSlot(0, value), geometry.SubsetAlphaTest);
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x0001BEBF File Offset: 0x0001A0BF
		public override int GetShadowStrength(int value)
		{
			if (!TallGrassBlock.GetIsSmall(Terrain.ExtractData(value)))
			{
				return this.DefaultShadowStrength;
			}
			return this.DefaultShadowStrength / 2;
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x0001BEE0 File Offset: 0x0001A0E0
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			Color color = BlockColorsMap.GrassColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, color, this.GetFaceTextureSlot(4, value));
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0001BF37 File Offset: 0x0001A137
		public static bool GetIsSmall(int data)
		{
			return (data & 8) != 0;
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x0001BF3F File Offset: 0x0001A13F
		public static int SetIsSmall(int data, bool isSmall)
		{
			if (!isSmall)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x04000245 RID: 581
		public const int Index = 19;
	}
}
