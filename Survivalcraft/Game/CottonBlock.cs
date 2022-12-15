using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000048 RID: 72
	public class CottonBlock : CrossBlock
	{
		// Token: 0x0600015F RID: 351 RVA: 0x000090E6 File Offset: 0x000072E6
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(204, 0, CottonBlock.SetIsWild(CottonBlock.SetSize(0, 2), true));
			yield return Terrain.MakeBlockValue(204, 0, CottonBlock.SetIsWild(CottonBlock.SetSize(0, 1), false));
			yield return Terrain.MakeBlockValue(204, 0, CottonBlock.SetIsWild(CottonBlock.SetSize(0, 2), false));
			yield break;
		}

		// Token: 0x06000160 RID: 352 RVA: 0x000090EF File Offset: 0x000072EF
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			if (!CottonBlock.GetIsWild(Terrain.ExtractData(value)))
			{
				return "棉花";
			}
			return "野生棉花";
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000910C File Offset: 0x0000730C
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			if (CottonBlock.GetSize(data) == 2)
			{
				BlockDropValue item = new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(173, 0, 6),
					Count = this.Random.Int(1, 2)
				};
				dropValues.Add(item);
				if (!CottonBlock.GetIsWild(data))
				{
					int num = this.Random.Int(1, 2);
					for (int i = 0; i < num; i++)
					{
						item = new BlockDropValue
						{
							Value = Terrain.MakeBlockValue(205, 0, 0),
							Count = 1
						};
						dropValues.Add(item);
					}
					if (this.Random.Bool(0.5f))
					{
						item = new BlockDropValue
						{
							Value = Terrain.MakeBlockValue(248),
							Count = 1
						};
						dropValues.Add(item);
					}
				}
			}
			showDebris = true;
		}

		// Token: 0x06000162 RID: 354 RVA: 0x000091FC File Offset: 0x000073FC
		public override int GetFaceTextureSlot(int face, int value)
		{
			int size = CottonBlock.GetSize(Terrain.ExtractData(value));
			if (size == 0)
			{
				return 11;
			}
			if (size != 1)
			{
				return 30;
			}
			return 29;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00009228 File Offset: 0x00007428
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			if (CottonBlock.GetIsWild(Terrain.ExtractData(value)))
			{
				color *= BlockColorsMap.GrassColorsMap.Lookup(environmentData.Temperature, environmentData.Humidity);
				BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
				return;
			}
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00009284 File Offset: 0x00007484
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			if (CottonBlock.GetIsWild(Terrain.ExtractData(value)))
			{
				Color color = BlockColorsMap.GrassColorsMap.Lookup(generator.Terrain, x, y, z);
				generator.GenerateCrossfaceVertices(this, value, x, y, z, color, this.GetFaceTextureSlot(0, value), geometry.SubsetAlphaTest);
				return;
			}
			generator.GenerateCrossfaceVertices(this, value, x, y, z, Color.White, this.GetFaceTextureSlot(0, value), geometry.SubsetAlphaTest);
		}

		// Token: 0x06000165 RID: 357 RVA: 0x000092F4 File Offset: 0x000074F4
		public override int GetShadowStrength(int value)
		{
			int size = CottonBlock.GetSize(Terrain.ExtractData(value));
			return 2 + size * 2;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00009314 File Offset: 0x00007514
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			if (CottonBlock.GetIsWild(Terrain.ExtractData(value)))
			{
				Color color = BlockColorsMap.GrassColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
				return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, color, this.GetFaceTextureSlot(4, value));
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.GetFaceTextureSlot(4, value));
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00009395 File Offset: 0x00007595
		public static int GetSize(int data)
		{
			return data & 3;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000939A File Offset: 0x0000759A
		public static int SetSize(int data, int size)
		{
			size = MathUtils.Clamp(size, 0, 2);
			return (data & -4) | (size & 3);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x000093AE File Offset: 0x000075AE
		public static bool GetIsWild(int data)
		{
			return (data & 8) != 0;
		}

		// Token: 0x0600016A RID: 362 RVA: 0x000093B6 File Offset: 0x000075B6
		public static int SetIsWild(int data, bool isWild)
		{
			if (!isWild)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x040000BE RID: 190
		public const int Index = 204;
	}
}
