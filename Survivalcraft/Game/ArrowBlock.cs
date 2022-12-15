using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000012 RID: 18
	public class ArrowBlock : Block
	{
		// Token: 0x06000096 RID: 150 RVA: 0x000058CC File Offset: 0x00003ACC
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Arrows");
			foreach (int num in EnumUtils.GetEnumValues(typeof(ArrowBlock.ArrowType)))
			{
				if (num > 15)
				{
					throw new InvalidOperationException("Too many arrow types.");
				}
				Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(ArrowBlock.m_shaftNames[num], true).ParentBone);
				Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(ArrowBlock.m_stabilizerNames[num], true).ParentBone);
				Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(ArrowBlock.m_tipNames[num], true).ParentBone);
				BlockMesh blockMesh = new BlockMesh();
				blockMesh.AppendModelMeshPart(model.FindMesh(ArrowBlock.m_tipNames[num], true).MeshParts[0], boneAbsoluteTransform3 * Matrix.CreateTranslation(0f, ArrowBlock.m_offsets[num], 0f), false, false, false, false, Color.White);
				blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(ArrowBlock.m_tipTextureSlots[num] % 16) / 16f, (float)(ArrowBlock.m_tipTextureSlots[num] / 16) / 16f, 0f), -1);
				BlockMesh blockMesh2 = new BlockMesh();
				blockMesh2.AppendModelMeshPart(model.FindMesh(ArrowBlock.m_shaftNames[num], true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, ArrowBlock.m_offsets[num], 0f), false, false, false, false, Color.White);
				blockMesh2.TransformTextureCoordinates(Matrix.CreateTranslation((float)(ArrowBlock.m_shaftTextureSlots[num] % 16) / 16f, (float)(ArrowBlock.m_shaftTextureSlots[num] / 16) / 16f, 0f), -1);
				BlockMesh blockMesh3 = new BlockMesh();
				blockMesh3.AppendModelMeshPart(model.FindMesh(ArrowBlock.m_stabilizerNames[num], true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, ArrowBlock.m_offsets[num], 0f), false, false, true, false, Color.White);
				blockMesh3.TransformTextureCoordinates(Matrix.CreateTranslation((float)(ArrowBlock.m_stabilizerTextureSlots[num] % 16) / 16f, (float)(ArrowBlock.m_stabilizerTextureSlots[num] / 16) / 16f, 0f), -1);
				BlockMesh blockMesh4 = new BlockMesh();
				blockMesh4.AppendBlockMesh(blockMesh);
				blockMesh4.AppendBlockMesh(blockMesh2);
				blockMesh4.AppendBlockMesh(blockMesh3);
				this.m_standaloneBlockMeshes.Add(blockMesh4);
			}
			base.Initialize();
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00005B58 File Offset: 0x00003D58
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00005B5C File Offset: 0x00003D5C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int arrowType = (int)ArrowBlock.GetArrowType(Terrain.ExtractData(value));
			if (arrowType >= 0 && arrowType < this.m_standaloneBlockMeshes.Count)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshes[arrowType], color, 2f * size, ref matrix, environmentData);
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00005BA8 File Offset: 0x00003DA8
		public override float GetProjectilePower(int value)
		{
			int arrowType = (int)ArrowBlock.GetArrowType(Terrain.ExtractData(value));
			if (arrowType < 0 || arrowType >= ArrowBlock.m_weaponPowers.Length)
			{
				return 0f;
			}
			return ArrowBlock.m_weaponPowers[arrowType];
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00005BDC File Offset: 0x00003DDC
		public override float GetExplosionPressure(int value)
		{
			int arrowType = (int)ArrowBlock.GetArrowType(Terrain.ExtractData(value));
			if (arrowType < 0 || arrowType >= ArrowBlock.m_explosionPressures.Length)
			{
				return 0f;
			}
			return ArrowBlock.m_explosionPressures[arrowType];
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00005C10 File Offset: 0x00003E10
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			int arrowType = (int)ArrowBlock.GetArrowType(Terrain.ExtractData(value));
			if (arrowType < 0 || arrowType >= ArrowBlock.m_iconViewScales.Length)
			{
				return 1f;
			}
			return ArrowBlock.m_iconViewScales[arrowType];
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00005C44 File Offset: 0x00003E44
		public override IEnumerable<int> GetCreativeValues()
		{
			int num;
			for (int i = 0; i < ArrowBlock.m_order.Length; i = num)
			{
				yield return Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, (ArrowBlock.ArrowType)ArrowBlock.m_order[i]));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005C50 File Offset: 0x00003E50
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int arrowType = (int)ArrowBlock.GetArrowType(Terrain.ExtractData(value));
			if (arrowType < 0 || arrowType >= ArrowBlock.m_displayNames.Length)
			{
				return string.Empty;
			}
			return ArrowBlock.m_displayNames[arrowType];
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005C84 File Offset: 0x00003E84
		public static ArrowBlock.ArrowType GetArrowType(int data)
		{
			return (ArrowBlock.ArrowType)(data & 15);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00005C8A File Offset: 0x00003E8A
		public static int SetArrowType(int data, ArrowBlock.ArrowType arrowType)
		{
			return (data & -16) | (int)(arrowType & (ArrowBlock.ArrowType)15);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00005CA8 File Offset: 0x00003EA8
		// Note: this type is marked as 'beforefieldinit'.
		static ArrowBlock()
		{
			float[] array = new float[9];
			array[7] = 40f;
			ArrowBlock.m_explosionPressures = array;
		}

		// Token: 0x0400004F RID: 79
		public const int Index = 192;

		// Token: 0x04000050 RID: 80
		public List<BlockMesh> m_standaloneBlockMeshes = new List<BlockMesh>();

		// Token: 0x04000051 RID: 81
		public static int[] m_order = new int[]
		{
			0,
			1,
			8,
			2,
			3,
			4,
			5,
			6,
			7
		};

		// Token: 0x04000052 RID: 82
		public static string[] m_tipNames = new string[]
		{
			"ArrowTip",
			"ArrowTip",
			"ArrowTip",
			"ArrowTip",
			"ArrowFireTip",
			"BoltTip",
			"BoltTip",
			"BoltExplosiveTip",
			"ArrowTip"
		};

		// Token: 0x04000053 RID: 83
		public static int[] m_tipTextureSlots = new int[]
		{
			47,
			1,
			63,
			182,
			62,
			63,
			182,
			183,
			79
		};

		// Token: 0x04000054 RID: 84
		public static string[] m_shaftNames = new string[]
		{
			"ArrowShaft",
			"ArrowShaft",
			"ArrowShaft",
			"ArrowShaft",
			"ArrowShaft",
			"BoltShaft",
			"BoltShaft",
			"BoltShaft",
			"ArrowShaft"
		};

		// Token: 0x04000055 RID: 85
		public static int[] m_shaftTextureSlots = new int[]
		{
			4,
			4,
			4,
			4,
			4,
			63,
			63,
			63,
			4
		};

		// Token: 0x04000056 RID: 86
		public static string[] m_stabilizerNames = new string[]
		{
			"ArrowStabilizer",
			"ArrowStabilizer",
			"ArrowStabilizer",
			"ArrowStabilizer",
			"ArrowStabilizer",
			"BoltStabilizer",
			"BoltStabilizer",
			"BoltStabilizer",
			"ArrowStabilizer"
		};

		// Token: 0x04000057 RID: 87
		public static int[] m_stabilizerTextureSlots = new int[]
		{
			15,
			15,
			15,
			15,
			15,
			63,
			63,
			63,
			15
		};

		// Token: 0x04000058 RID: 88
		public static string[] m_displayNames = new string[]
		{
			"木尖箭头",
			"石尖箭头",
			"铁尖箭头",
			"钻石尖箭头",
			"火尖箭头",
			"铁螺栓",
			"钻石尖螺栓",
			"爆炸螺栓",
			"铜尖箭头"
		};

		// Token: 0x04000059 RID: 89
		public static float[] m_offsets = new float[]
		{
			-0.5f,
			-0.5f,
			-0.5f,
			-0.5f,
			-0.5f,
			-0.3f,
			-0.3f,
			-0.3f,
			-0.5f
		};

		// Token: 0x0400005A RID: 90
		public static float[] m_weaponPowers = new float[]
		{
			5f,
			7f,
			14f,
			18f,
			4f,
			28f,
			36f,
			8f,
			10f
		};

		// Token: 0x0400005B RID: 91
		public static float[] m_iconViewScales = new float[]
		{
			0.8f,
			0.8f,
			0.8f,
			0.8f,
			0.8f,
			1.1f,
			1.1f,
			1.1f,
			0.8f
		};

		// Token: 0x0400005C RID: 92
		public static float[] m_explosionPressures;

		// Token: 0x020003BC RID: 956
		public enum ArrowType
		{
			// Token: 0x040013DA RID: 5082
			WoodenArrow,
			// Token: 0x040013DB RID: 5083
			StoneArrow,
			// Token: 0x040013DC RID: 5084
			IronArrow,
			// Token: 0x040013DD RID: 5085
			DiamondArrow,
			// Token: 0x040013DE RID: 5086
			FireArrow,
			// Token: 0x040013DF RID: 5087
			IronBolt,
			// Token: 0x040013E0 RID: 5088
			DiamondBolt,
			// Token: 0x040013E1 RID: 5089
			ExplosiveBolt,
			// Token: 0x040013E2 RID: 5090
			CopperArrow
		}
	}
}
