using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000021 RID: 33
	public class BowBlock : Block
	{
		// Token: 0x060000ED RID: 237 RVA: 0x00007258 File Offset: 0x00005458
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Bows");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("BowRelaxed", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("StringRelaxed", true).ParentBone);
			Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("BowTensed", true).ParentBone);
			Matrix boneAbsoluteTransform4 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("StringTensed", true).ParentBone);
			BlockMesh blockMesh = new BlockMesh();
			blockMesh.AppendModelMeshPart(model.FindMesh("BowRelaxed", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			blockMesh.AppendModelMeshPart(model.FindMesh("StringRelaxed", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			BlockMesh blockMesh2 = new BlockMesh();
			blockMesh2.AppendModelMeshPart(model.FindMesh("BowTensed", true).MeshParts[0], boneAbsoluteTransform3 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			blockMesh2.AppendModelMeshPart(model.FindMesh("StringTensed", true).MeshParts[0], boneAbsoluteTransform4 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			for (int i = 0; i < 16; i++)
			{
				float factor = (float)i / 15f;
				this.m_standaloneBlockMeshes[i] = new BlockMesh();
				this.m_standaloneBlockMeshes[i].AppendBlockMesh(blockMesh);
				this.m_standaloneBlockMeshes[i].BlendBlockMesh(blockMesh2, factor);
			}
			base.Initialize();
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00007442 File Offset: 0x00005642
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00007444 File Offset: 0x00005644
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int data = Terrain.ExtractData(value);
			int draw = BowBlock.GetDraw(data);
			ArrowBlock.ArrowType? arrowType = BowBlock.GetArrowType(data);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshes[draw], color, 2f * size, ref matrix, environmentData);
			if (arrowType != null)
			{
				float num = MathUtils.Lerp(0.14f, 0.68f, (float)draw / 15f);
				Matrix matrix2 = Matrix.CreateRotationX(-1.5707964f) * Matrix.CreateTranslation(0f, 0.4f * size, (-1f + 2f * num) * size) * matrix;
				int value2 = Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, arrowType.Value));
				BlocksManager.Blocks[192].DrawBlock(primitivesRenderer, value2, color, size, ref matrix2, environmentData);
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00007515 File Offset: 0x00005715
		public override int GetDamage(int value)
		{
			return Terrain.ExtractData(value) >> 8 & 255;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00007528 File Offset: 0x00005728
		public override int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			num &= -65281;
			num |= MathUtils.Clamp(damage, 0, 255) << 8;
			return Terrain.ReplaceData(value, num);
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x0000755C File Offset: 0x0000575C
		public override bool IsSwapAnimationNeeded(int oldValue, int newValue)
		{
			int num = Terrain.ExtractContents(oldValue);
			int data = Terrain.ExtractData(oldValue);
			int data2 = Terrain.ExtractData(newValue);
			if (num == 191)
			{
				ArrowBlock.ArrowType? arrowType = BowBlock.GetArrowType(data);
				ArrowBlock.ArrowType? arrowType2 = BowBlock.GetArrowType(data2);
				if (arrowType.GetValueOrDefault() == arrowType2.GetValueOrDefault() & arrowType != null == (arrowType2 != null))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000075B8 File Offset: 0x000057B8
		public static ArrowBlock.ArrowType? GetArrowType(int data)
		{
			int num = data >> 4 & 15;
			if (num != 0)
			{
				return new ArrowBlock.ArrowType?((ArrowBlock.ArrowType)(num - 1));
			}
			return null;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x000075E4 File Offset: 0x000057E4
		public static int SetArrowType(int data, ArrowBlock.ArrowType? arrowType)
		{
			int num = (int)((arrowType != null) ? (arrowType.Value + 1) : ArrowBlock.ArrowType.WoodenArrow);
			return (data & -241) | (num & 15) << 4;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00007615 File Offset: 0x00005815
		public static int GetDraw(int data)
		{
			return data & 15;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000761B File Offset: 0x0000581B
		public static int SetDraw(int data, int draw)
		{
			return (data & -16) | (draw & 15);
		}

		// Token: 0x0400007D RID: 125
		public const int Index = 191;

		// Token: 0x0400007E RID: 126
		public BlockMesh[] m_standaloneBlockMeshes = new BlockMesh[16];
	}
}
