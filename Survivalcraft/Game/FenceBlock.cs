using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000064 RID: 100
	public abstract class FenceBlock : Block, IPaintableBlock
	{
		// Token: 0x060001E6 RID: 486 RVA: 0x0000B09C File Offset: 0x0000929C
		public FenceBlock(string modelName, bool doubleSidedPlanks, bool useAlphaTest, int coloredTextureSlot, Color postColor, Color unpaintedColor)
		{
			this.m_modelName = modelName;
			this.m_doubleSidedPlanks = doubleSidedPlanks;
			this.m_useAlphaTest = useAlphaTest;
			this.m_coloredTextureSlot = coloredTextureSlot;
			this.m_postColor = postColor;
			this.m_unpaintedColor = unpaintedColor;
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000B11C File Offset: 0x0000931C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Post", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Planks", true).ParentBone);
			for (int i = 0; i < 16; i++)
			{
				bool flag = (i & 1) != 0;
				bool flag2 = (i & 2) != 0;
				bool flag3 = (i & 4) != 0;
				bool flag4 = (i & 8) != 0;
				List<BoundingBox> list = new List<BoundingBox>();
				Matrix m = Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				BlockMesh blockMesh = new BlockMesh();
				blockMesh.AppendModelMeshPart(model.FindMesh("Post", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				BoundingBox item = blockMesh.CalculateBoundingBox();
				item.Min.X = item.Min.X - 0.1f;
				item.Min.Z = item.Min.Z - 0.1f;
				item.Max.X = item.Max.X + 0.1f;
				item.Max.Z = item.Max.Z + 0.1f;
				list.Add(item);
				BlockMesh blockMesh2 = new BlockMesh();
				if (flag)
				{
					BlockMesh blockMesh3 = new BlockMesh();
					Matrix m2 = Matrix.CreateRotationY(0f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
					blockMesh3.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * m2, false, false, false, false, Color.White);
					if (this.m_doubleSidedPlanks)
					{
						blockMesh3.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * m2, false, true, false, true, Color.White);
					}
					blockMesh2.AppendBlockMesh(blockMesh3);
					BoundingBox item2 = blockMesh3.CalculateBoundingBox();
					list.Add(item2);
				}
				if (flag2)
				{
					BlockMesh blockMesh4 = new BlockMesh();
					Matrix m3 = Matrix.CreateRotationY(3.1415927f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
					blockMesh4.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * m3, false, false, false, false, Color.White);
					if (this.m_doubleSidedPlanks)
					{
						blockMesh4.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * m3, false, true, false, true, Color.White);
					}
					blockMesh2.AppendBlockMesh(blockMesh4);
					BoundingBox item3 = blockMesh4.CalculateBoundingBox();
					list.Add(item3);
				}
				if (flag3)
				{
					BlockMesh blockMesh5 = new BlockMesh();
					Matrix m4 = Matrix.CreateRotationY(4.712389f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
					blockMesh5.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * m4, false, false, false, false, Color.White);
					if (this.m_doubleSidedPlanks)
					{
						blockMesh5.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * m4, false, true, false, true, Color.White);
					}
					blockMesh2.AppendBlockMesh(blockMesh5);
					BoundingBox item4 = blockMesh5.CalculateBoundingBox();
					list.Add(item4);
				}
				if (flag4)
				{
					BlockMesh blockMesh6 = new BlockMesh();
					Matrix m5 = Matrix.CreateRotationY(1.5707964f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
					blockMesh6.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * m5, false, false, false, false, Color.White);
					if (this.m_doubleSidedPlanks)
					{
						blockMesh6.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * m5, false, true, false, true, Color.White);
					}
					blockMesh2.AppendBlockMesh(blockMesh6);
					BoundingBox item5 = blockMesh6.CalculateBoundingBox();
					list.Add(item5);
				}
				blockMesh.ModulateColor(this.m_postColor, -1);
				this.m_blockMeshes[i] = new BlockMesh();
				this.m_blockMeshes[i].AppendBlockMesh(blockMesh);
				this.m_blockMeshes[i].AppendBlockMesh(blockMesh2);
				this.m_blockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
				this.m_blockMeshes[i].GenerateSidesData();
				this.m_coloredBlockMeshes[i] = new BlockMesh();
				this.m_coloredBlockMeshes[i].AppendBlockMesh(blockMesh);
				this.m_coloredBlockMeshes[i].AppendBlockMesh(blockMesh2);
				this.m_coloredBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
				this.m_coloredBlockMeshes[i].GenerateSidesData();
				this.m_collisionBoxes[i] = list.ToArray();
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Post", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(-0.5f, -0.5f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Post", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, -0.5f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateRotationY(0f) * Matrix.CreateTranslation(-0.5f, -0.5f, 0f), false, false, false, false, Color.White);
			if (this.m_doubleSidedPlanks)
			{
				this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateRotationY(0f) * Matrix.CreateTranslation(-0.5f, -0.5f, 0f), false, true, false, true, Color.White);
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateRotationY(3.1415927f) * Matrix.CreateTranslation(0.5f, -0.5f, 0f), false, false, false, false, Color.White);
			if (this.m_doubleSidedPlanks)
			{
				this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateRotationY(3.1415927f) * Matrix.CreateTranslation(0.5f, -0.5f, 0f), false, true, false, true, Color.White);
			}
			this.m_standaloneColoredBlockMesh.AppendBlockMesh(this.m_standaloneBlockMesh);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
			this.m_standaloneColoredBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
			base.Initialize();
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000B8C8 File Offset: 0x00009AC8
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int? color = FenceBlock.GetColor(Terrain.ExtractData(value));
			return SubsystemPalette.GetName(subsystemTerrain, color, base.GetDisplayName(subsystemTerrain, value));
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000B8F0 File Offset: 0x00009AF0
		public override string GetCategory(int value)
		{
			if (FenceBlock.GetColor(Terrain.ExtractData(value)) == null)
			{
				return base.GetCategory(value);
			}
			return LanguageControl.Get("BlocksManager", "Painted");
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000B929 File Offset: 0x00009B29
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, FenceBlock.SetColor(0, null));
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(this.BlockIndex, 0, FenceBlock.SetColor(0, new int?(i)));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000B93C File Offset: 0x00009B3C
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			int data = FenceBlock.SetVariant(Terrain.ExtractData(oldValue), 0);
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, data),
				Count = 1
			});
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000B988 File Offset: 0x00009B88
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int? color = FenceBlock.GetColor(Terrain.ExtractData(value));
			if (color != null)
			{
				return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, SubsystemPalette.GetColor(subsystemTerrain, color), this.m_coloredTextureSlot);
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.DefaultTextureSlot);
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000B9E4 File Offset: 0x00009BE4
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int variant = FenceBlock.GetVariant(data);
			int? color = FenceBlock.GetColor(data);
			if (color != null)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_coloredBlockMeshes[variant], SubsystemPalette.GetColor(generator, color), null, this.m_useAlphaTest ? geometry.SubsetAlphaTest : geometry.SubsetOpaque);
				return;
			}
			generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshes[variant], this.m_unpaintedColor, null, this.m_useAlphaTest ? geometry.SubsetAlphaTest : geometry.SubsetOpaque);
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000BA84 File Offset: 0x00009C84
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? color2 = FenceBlock.GetColor(Terrain.ExtractData(value));
			if (color2 != null)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneColoredBlockMesh, color * SubsystemPalette.GetColor(environmentData, color2), size, ref matrix, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color * this.m_unpaintedColor, size, ref matrix, environmentData);
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000BAE5 File Offset: 0x00009CE5
		public int? GetPaintColor(int value)
		{
			return FenceBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000BAF4 File Offset: 0x00009CF4
		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, FenceBlock.SetColor(data, color));
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000BB18 File Offset: 0x00009D18
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int variant = FenceBlock.GetVariant(Terrain.ExtractData(value));
			return this.m_collisionBoxes[variant];
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000BB3C File Offset: 0x00009D3C
		public virtual bool ShouldConnectTo(int value)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			return block is FenceBlock || block is FenceGateBlock;
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000BB6B File Offset: 0x00009D6B
		public static int GetVariant(int data)
		{
			return data & 15;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000BB71 File Offset: 0x00009D71
		public static int SetVariant(int data, int variant)
		{
			return (data & -16) | (variant & 15);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000BB7C File Offset: 0x00009D7C
		public static int? GetColor(int data)
		{
			if ((data & 16) != 0)
			{
				return new int?(data >> 5 & 15);
			}
			return null;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000BBA4 File Offset: 0x00009DA4
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -497) | 16 | (color.Value & 15) << 5;
			}
			return data & -497;
		}

		// Token: 0x040000E7 RID: 231
		public string m_modelName;

		// Token: 0x040000E8 RID: 232
		public bool m_doubleSidedPlanks;

		// Token: 0x040000E9 RID: 233
		public bool m_useAlphaTest;

		// Token: 0x040000EA RID: 234
		public int m_coloredTextureSlot;

		// Token: 0x040000EB RID: 235
		public Color m_postColor;

		// Token: 0x040000EC RID: 236
		public Color m_unpaintedColor;

		// Token: 0x040000ED RID: 237
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x040000EE RID: 238
		public BlockMesh m_standaloneColoredBlockMesh = new BlockMesh();

		// Token: 0x040000EF RID: 239
		public BlockMesh[] m_blockMeshes = new BlockMesh[16];

		// Token: 0x040000F0 RID: 240
		public BlockMesh[] m_coloredBlockMeshes = new BlockMesh[16];

		// Token: 0x040000F1 RID: 241
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[16][];
	}
}
