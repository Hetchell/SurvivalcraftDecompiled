using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000E1 RID: 225
	public class RyeBlock : CrossBlock
	{
		// Token: 0x0600044D RID: 1101 RVA: 0x0001739E File Offset: 0x0001559E
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(174, 0, RyeBlock.SetIsWild(RyeBlock.SetSize(0, 7), true));
			yield return Terrain.MakeBlockValue(174, 0, RyeBlock.SetIsWild(RyeBlock.SetSize(0, 1), false));
			yield return Terrain.MakeBlockValue(174, 0, RyeBlock.SetIsWild(RyeBlock.SetSize(0, 3), false));
			yield return Terrain.MakeBlockValue(174, 0, RyeBlock.SetIsWild(RyeBlock.SetSize(0, 5), false));
			yield return Terrain.MakeBlockValue(174, 0, RyeBlock.SetIsWild(RyeBlock.SetSize(0, 7), false));
			yield break;
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x000173A8 File Offset: 0x000155A8
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			if (RyeBlock.GetIsWild(Terrain.ExtractData(value)))
			{
				color *= BlockColorsMap.GrassColorsMap.Lookup(environmentData.Temperature, environmentData.Humidity);
				BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
				return;
			}
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x00017404 File Offset: 0x00015604
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			if (RyeBlock.GetIsWild(Terrain.ExtractData(value)))
			{
				Color color = BlockColorsMap.GrassColorsMap.Lookup(generator.Terrain, x, y, z);
				generator.GenerateCrossfaceVertices(this, value, x, y, z, color, this.GetFaceTextureSlot(0, value), geometry.SubsetAlphaTest);
				return;
			}
			generator.GenerateCrossfaceVertices(this, value, x, y, z, Color.White, this.GetFaceTextureSlot(0, value), geometry.SubsetAlphaTest);
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x00017474 File Offset: 0x00015674
		public override int GetShadowStrength(int value)
		{
			return RyeBlock.GetSize(Terrain.ExtractData(value)) + 1;
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x00017484 File Offset: 0x00015684
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			if (RyeBlock.GetIsWild(Terrain.ExtractData(value)))
			{
				Color color = BlockColorsMap.GrassColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
				return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, color, this.GetFaceTextureSlot(4, value));
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.GetFaceTextureSlot(4, value));
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x00017508 File Offset: 0x00015708
		public override int GetFaceTextureSlot(int face, int value)
		{
			int data = Terrain.ExtractData(value);
			int size = RyeBlock.GetSize(data);
			if (!RyeBlock.GetIsWild(data))
			{
				return 88 + size;
			}
			if (size > 2)
			{
				return 87;
			}
			return 86;
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x00017538 File Offset: 0x00015738
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int num = 0;
			int data = 0;
			int data2 = Terrain.ExtractData(oldValue);
			int size = RyeBlock.GetSize(data2);
			bool isWild = RyeBlock.GetIsWild(data2);
			if (isWild)
			{
				num = ((size > 2 && this.Random.Float(0f, 1f) < 0.33f) ? 1 : 0);
				data = 4;
			}
			else
			{
				switch (size)
				{
				case 5:
					num = 1;
					data = 4;
					break;
				case 6:
					num = this.Random.Int(1, 2);
					data = 4;
					break;
				case 7:
					num = this.Random.Int(1, 3);
					data = 5;
					break;
				}
			}
			showDebris = true;
			for (int i = 0; i < num; i++)
			{
				BlockDropValue item = new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(173, 0, data),
					Count = 1
				};
				dropValues.Add(item);
			}
			if (size == 7 && !isWild && this.Random.Bool(0.5f))
			{
				BlockDropValue item = new BlockDropValue
				{
					Value = 248,
					Count = 1
				};
				dropValues.Add(item);
			}
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x0001764C File Offset: 0x0001584C
		public static int GetSize(int data)
		{
			return data & 7;
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x00017651 File Offset: 0x00015851
		public static int SetSize(int data, int size)
		{
			size = MathUtils.Clamp(size, 0, 7);
			return (data & -8) | (size & 7);
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x00017665 File Offset: 0x00015865
		public static bool GetIsWild(int data)
		{
			return (data & 8) != 0;
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x0001766D File Offset: 0x0001586D
		public static int SetIsWild(int data, bool isWild)
		{
			if (!isWild)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x040001EF RID: 495
		public const int Index = 174;
	}
}
