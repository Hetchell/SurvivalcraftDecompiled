using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000029 RID: 41
	public class BulletBlock : FlatBlock
	{
		// Token: 0x06000104 RID: 260 RVA: 0x00007785 File Offset: 0x00005985
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00007788 File Offset: 0x00005988
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int bulletType = (int)BulletBlock.GetBulletType(Terrain.ExtractData(value));
			float size2 = (bulletType >= 0 && bulletType < BulletBlock.m_sizes.Length) ? (size * BulletBlock.m_sizes[bulletType]) : size;
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size2, ref matrix, null, color, false, environmentData);
		}

		// Token: 0x06000106 RID: 262 RVA: 0x000077D0 File Offset: 0x000059D0
		public override float GetProjectilePower(int value)
		{
			int bulletType = (int)BulletBlock.GetBulletType(Terrain.ExtractData(value));
			if (bulletType < 0 || bulletType >= BulletBlock.m_weaponPowers.Length)
			{
				return 0f;
			}
			return BulletBlock.m_weaponPowers[bulletType];
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00007804 File Offset: 0x00005A04
		public override float GetExplosionPressure(int value)
		{
			int bulletType = (int)BulletBlock.GetBulletType(Terrain.ExtractData(value));
			if (bulletType < 0 || bulletType >= BulletBlock.m_explosionPressures.Length)
			{
				return 0f;
			}
			return BulletBlock.m_explosionPressures[bulletType];
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00007838 File Offset: 0x00005A38
		public override IEnumerable<int> GetCreativeValues()
		{
			foreach (int bulletType in EnumUtils.GetEnumValues(typeof(BulletBlock.BulletType)))
			{
				yield return Terrain.MakeBlockValue(214, 0, BulletBlock.SetBulletType(0, (BulletBlock.BulletType)bulletType));
			}
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00007844 File Offset: 0x00005A44
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int bulletType = (int)BulletBlock.GetBulletType(Terrain.ExtractData(value));
			if (bulletType < 0 || bulletType >= BulletBlock.m_displayNames.Length)
			{
				return string.Empty;
			}
			return BulletBlock.m_displayNames[bulletType];
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00007878 File Offset: 0x00005A78
		public override int GetFaceTextureSlot(int face, int value)
		{
			int bulletType = (int)BulletBlock.GetBulletType(Terrain.ExtractData(value));
			if (bulletType < 0 || bulletType >= BulletBlock.m_textureSlots.Length)
			{
				return 229;
			}
			return BulletBlock.m_textureSlots[bulletType];
		}

		// Token: 0x0600010B RID: 267 RVA: 0x000078AC File Offset: 0x00005AAC
		public static BulletBlock.BulletType GetBulletType(int data)
		{
			return (BulletBlock.BulletType)(data & 15);
		}

		// Token: 0x0600010C RID: 268 RVA: 0x000078B2 File Offset: 0x00005AB2
		public static int SetBulletType(int data, BulletBlock.BulletType bulletType)
		{
			return (data & -16) | (int)(bulletType & (BulletBlock.BulletType)15);
		}

		// Token: 0x04000086 RID: 134
		public const int Index = 214;

		// Token: 0x04000087 RID: 135
		public static string[] m_displayNames = new string[]
		{
			"枪弹",
			"铅弹",
			"铅弹球"
		};

		// Token: 0x04000088 RID: 136
		public static float[] m_sizes = new float[]
		{
			1f,
			1f,
			0.33f
		};

		// Token: 0x04000089 RID: 137
		public static int[] m_textureSlots = new int[]
		{
			229,
			231,
			229
		};

		// Token: 0x0400008A RID: 138
		public static float[] m_weaponPowers = new float[]
		{
			80f,
			0f,
			3.6f
		};

		// Token: 0x0400008B RID: 139
		public static float[] m_explosionPressures = new float[3];

		// Token: 0x020003BF RID: 959
		public enum BulletType
		{
			// Token: 0x040013EC RID: 5100
			MusketBall,
			// Token: 0x040013ED RID: 5101
			Buckshot,
			// Token: 0x040013EE RID: 5102
			BuckshotBall
		}
	}
}
