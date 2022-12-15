using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000F6 RID: 246
	public class SoilBlock : CubeBlock
	{
		// Token: 0x060004B0 RID: 1200 RVA: 0x00019050 File Offset: 0x00017250
		public override int GetFaceTextureSlot(int face, int value)
		{
			int nitrogen = SoilBlock.GetNitrogen(Terrain.ExtractData(value));
			if (face != 4)
			{
				return 2;
			}
			if (nitrogen <= 0)
			{
				return 37;
			}
			return 53;
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x00019078 File Offset: 0x00017278
		public static bool GetHydration(int data)
		{
			return (data & 1) != 0;
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x00019080 File Offset: 0x00017280
		public static int GetNitrogen(int data)
		{
			return data >> 1 & 3;
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x00019087 File Offset: 0x00017287
		public static int SetHydration(int data, bool hydration)
		{
			if (!hydration)
			{
				return data & -2;
			}
			return data | 1;
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x00019094 File Offset: 0x00017294
		public static int SetNitrogen(int data, int nitrogen)
		{
			nitrogen = MathUtils.Clamp(nitrogen, 0, 3);
			return (data & -7) | (nitrogen & 3) << 1;
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x000190AA File Offset: 0x000172AA
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, SoilBlock.SetHydration(0, false));
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, SoilBlock.SetHydration(0, true));
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, SoilBlock.SetHydration(SoilBlock.SetNitrogen(0, 3), true));
			yield break;
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x000190BC File Offset: 0x000172BC
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			int nitrogen = SoilBlock.GetNitrogen(data);
			bool hydration = SoilBlock.GetHydration(data);
			if (nitrogen > 0 && hydration)
			{
				LanguageControl.Get(SoilBlock.fName, 2);
				return LanguageControl.Get(SoilBlock.fName, 1);
			}
			if (nitrogen > 0)
			{
				LanguageControl.Get(SoilBlock.fName, 2);
				return LanguageControl.Get(SoilBlock.fName, 2);
			}
			if (hydration)
			{
				return LanguageControl.Get(SoilBlock.fName, 3);
			}
			return LanguageControl.Get(SoilBlock.fName, 4);
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x00019134 File Offset: 0x00017334
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			Color color = SoilBlock.GetHydration(Terrain.ExtractData(value)) ? new Color(180, 170, 150) : Color.White;
			generator.GenerateCubeVertices(this, value, x, y, z, 0.9375f, 0.9375f, 0.9375f, 0.9375f, color, color, color, color, color, -1, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x00019198 File Offset: 0x00017398
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			Color c = SoilBlock.GetHydration(Terrain.ExtractData(value)) ? new Color(180, 170, 150) : Color.White;
			base.DrawBlock(primitivesRenderer, value, color * c, size, ref matrix, environmentData);
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x000191E3 File Offset: 0x000173E3
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return SoilBlock.m_collisionBoxes;
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x000191EA File Offset: 0x000173EA
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return face != 5;
		}

		// Token: 0x04000216 RID: 534
		public const int Index = 168;

		// Token: 0x04000217 RID: 535
		public new static string fName = "SoilBlock";

		// Token: 0x04000218 RID: 536
		public static BoundingBox[] m_collisionBoxes = new BoundingBox[]
		{
			new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.9375f, 1f))
		};
	}
}
