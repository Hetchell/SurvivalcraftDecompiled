using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000065 RID: 101
	public abstract class FenceGateBlock : Block, IElectricElementBlock, IPaintableBlock
	{
		// Token: 0x060001F7 RID: 503 RVA: 0x0000BBD0 File Offset: 0x00009DD0
		public FenceGateBlock(string modelName, float pivotDistance, bool doubleSided, bool useAlphaTest, int coloredTextureSlot, Color postColor, Color unpaintedColor)
		{
			this.m_modelName = modelName;
			this.m_pivotDistance = pivotDistance;
			this.m_doubleSided = doubleSided;
			this.m_useAlphaTest = useAlphaTest;
			this.m_coloredTextureSlot = coloredTextureSlot;
			this.m_postColor = postColor;
			this.m_unpaintedColor = unpaintedColor;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000BC58 File Offset: 0x00009E58
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Post", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Planks", true).ParentBone);
			for (int i = 0; i < 16; i++)
			{
				int rotation = FenceGateBlock.GetRotation(i);
				bool open = FenceGateBlock.GetOpen(i);
				bool rightHanded = FenceGateBlock.GetRightHanded(i);
				float num = (float)((!rightHanded) ? 1 : -1);
				Matrix matrix = Matrix.Identity;
				matrix *= Matrix.CreateScale(0f - num, 1f, 1f);
				matrix *= Matrix.CreateTranslation((0.5f - this.m_pivotDistance) * num, 0f, 0f) * Matrix.CreateRotationY(open ? (num * 3.1415927f / 2f) : 0f) * Matrix.CreateTranslation((0f - (0.5f - this.m_pivotDistance)) * num, 0f, 0f);
				matrix *= Matrix.CreateTranslation(0f, 0f, 0f) * Matrix.CreateRotationY((float)rotation * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				this.m_blockMeshes[i] = new BlockMesh();
				this.m_blockMeshes[i].AppendModelMeshPart(model.FindMesh("Post", true).MeshParts[0], boneAbsoluteTransform * matrix, false, !rightHanded, false, false, this.m_postColor);
				this.m_blockMeshes[i].AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * matrix, false, !rightHanded, false, false, Color.White);
				if (this.m_doubleSided)
				{
					this.m_blockMeshes[i].AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * matrix, false, rightHanded, false, true, Color.White);
				}
				this.m_coloredBlockMeshes[i] = new BlockMesh();
				this.m_coloredBlockMeshes[i].AppendBlockMesh(this.m_blockMeshes[i]);
				this.m_blockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
				this.m_coloredBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
				BoundingBox boundingBox = this.m_blockMeshes[i].CalculateBoundingBox();
				boundingBox.Min.X = MathUtils.Saturate(boundingBox.Min.X);
				boundingBox.Min.Y = MathUtils.Saturate(boundingBox.Min.Y);
				boundingBox.Min.Z = MathUtils.Saturate(boundingBox.Min.Z);
				boundingBox.Max.X = MathUtils.Saturate(boundingBox.Max.X);
				boundingBox.Max.Y = MathUtils.Saturate(boundingBox.Max.Y);
				boundingBox.Max.Z = MathUtils.Saturate(boundingBox.Max.Z);
				this.m_collisionBoxes[i] = new BoundingBox[]
				{
					boundingBox
				};
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Post", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, this.m_postColor);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			if (this.m_doubleSided)
			{
				this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Planks", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, true, false, true, Color.White);
			}
			this.m_standaloneColoredBlockMesh.AppendBlockMesh(this.m_standaloneBlockMesh);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
			this.m_standaloneColoredBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
			base.Initialize();
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000C158 File Offset: 0x0000A358
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int? color = FenceGateBlock.GetColor(Terrain.ExtractData(value));
			return SubsystemPalette.GetName(subsystemTerrain, color, base.GetDisplayName(subsystemTerrain, value));
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000C180 File Offset: 0x0000A380
		public override string GetCategory(int value)
		{
			if (FenceGateBlock.GetColor(Terrain.ExtractData(value)) == null)
			{
				return base.GetCategory(value);
			}
			return LanguageControl.Get("BlocksManager", "Painted");
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000C1B9 File Offset: 0x0000A3B9
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, FenceGateBlock.SetColor(0, null));
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(this.BlockIndex, 0, FenceGateBlock.SetColor(0, new int?(i)));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000C1CC File Offset: 0x0000A3CC
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			int data = FenceGateBlock.SetVariant(Terrain.ExtractData(oldValue), 0);
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, data),
				Count = 1
			});
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000C218 File Offset: 0x0000A418
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int num5 = 0;
			if (num == MathUtils.Max(num, num2, num3, num4))
			{
				num5 = 2;
			}
			else if (num2 == MathUtils.Max(num, num2, num3, num4))
			{
				num5 = 3;
			}
			else if (num3 == MathUtils.Max(num, num2, num3, num4))
			{
				num5 = 0;
			}
			else if (num4 == MathUtils.Max(num, num2, num3, num4))
			{
				num5 = 1;
			}
			Point3 point = CellFace.FaceToPoint3(raycastResult.CellFace.Face);
			int num6 = raycastResult.CellFace.X + point.X;
			int y = raycastResult.CellFace.Y + point.Y;
			int num7 = raycastResult.CellFace.Z + point.Z;
			int num8 = 0;
			int num9 = 0;
			switch (num5)
			{
			case 0:
				num8 = -1;
				break;
			case 1:
				num9 = 1;
				break;
			case 2:
				num8 = 1;
				break;
			default:
				num9 = -1;
				break;
			}
			int cellValue = subsystemTerrain.Terrain.GetCellValue(num6 + num8, y, num7 + num9);
			int cellValue2 = subsystemTerrain.Terrain.GetCellValue(num6 - num8, y, num7 - num9);
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
			Block block2 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)];
			int data = Terrain.ExtractData(cellValue);
			int data2 = Terrain.ExtractData(cellValue2);
			bool rightHanded = (block is FenceGateBlock && FenceGateBlock.GetRotation(data) == num5) || ((!(block2 is FenceGateBlock) || FenceGateBlock.GetRotation(data2) != num5) && !block.IsCollidable);
			int data3 = FenceGateBlock.SetRightHanded(FenceGateBlock.SetOpen(FenceGateBlock.SetRotation(Terrain.ExtractData(value), num5), false), rightHanded);
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, this.BlockIndex), data3),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000C430 File Offset: 0x0000A630
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int? color = FenceGateBlock.GetColor(Terrain.ExtractData(value));
			if (color != null)
			{
				return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, SubsystemPalette.GetColor(subsystemTerrain, color), this.m_coloredTextureSlot);
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.DefaultTextureSlot);
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000C48C File Offset: 0x0000A68C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int variant = FenceGateBlock.GetVariant(data);
			int? color = FenceGateBlock.GetColor(data);
			if (color != null)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_coloredBlockMeshes[variant], SubsystemPalette.GetColor(generator, color), null, this.m_useAlphaTest ? geometry.SubsetAlphaTest : geometry.SubsetOpaque);
			}
			else
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshes[variant], this.m_unpaintedColor, null, this.m_useAlphaTest ? geometry.SubsetAlphaTest : geometry.SubsetOpaque);
			}
			generator.GenerateWireVertices(value, x, y, z, FenceGateBlock.GetHingeFace(data), this.m_pivotDistance * 2f, Vector2.Zero, geometry.SubsetOpaque);
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000C558 File Offset: 0x0000A758
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? color2 = FenceGateBlock.GetColor(Terrain.ExtractData(value));
			if (color2 != null)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneColoredBlockMesh, color * SubsystemPalette.GetColor(environmentData, color2), size, ref matrix, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color * this.m_unpaintedColor, size, ref matrix, environmentData);
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000C5B9 File Offset: 0x0000A7B9
		public int? GetPaintColor(int value)
		{
			return FenceGateBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000C5C8 File Offset: 0x0000A7C8
		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, FenceGateBlock.SetColor(data, color));
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000C5EC File Offset: 0x0000A7EC
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int variant = FenceGateBlock.GetVariant(Terrain.ExtractData(value));
			return this.m_collisionBoxes[variant];
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000C610 File Offset: 0x0000A810
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			return new FenceGateElectricElement(subsystemElectricity, new CellFace(x, y, z, FenceGateBlock.GetHingeFace(data)));
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000C63C File Offset: 0x0000A83C
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int hingeFace = FenceGateBlock.GetHingeFace(Terrain.ExtractData(value));
			if (face == hingeFace)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000C669 File Offset: 0x0000A869
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000C670 File Offset: 0x0000A870
		public static int GetRotation(int data)
		{
			return data & 3;
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000C675 File Offset: 0x0000A875
		public static bool GetOpen(int data)
		{
			return (data & 4) != 0;
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000C67D File Offset: 0x0000A87D
		public static bool GetRightHanded(int data)
		{
			return (data & 8) == 0;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000C685 File Offset: 0x0000A885
		public static int SetRotation(int data, int rotation)
		{
			return (data & -4) | (rotation & 3);
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000C68F File Offset: 0x0000A88F
		public static int SetOpen(int data, bool open)
		{
			if (!open)
			{
				return data & -5;
			}
			return data | 4;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000C69C File Offset: 0x0000A89C
		public static int SetRightHanded(int data, bool rightHanded)
		{
			if (rightHanded)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000C6AC File Offset: 0x0000A8AC
		public static int GetHingeFace(int data)
		{
			int rotation = FenceGateBlock.GetRotation(data);
			int num = (rotation - 1 < 0) ? 3 : (rotation - 1);
			if (!FenceGateBlock.GetRightHanded(data))
			{
				num = CellFace.OppositeFace(num);
			}
			return num;
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000C6DD File Offset: 0x0000A8DD
		public static int GetVariant(int data)
		{
			return data & 15;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000C6E3 File Offset: 0x0000A8E3
		public static int SetVariant(int data, int variant)
		{
			return (data & -16) | (variant & 15);
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000C6F0 File Offset: 0x0000A8F0
		public static int? GetColor(int data)
		{
			if ((data & 16) != 0)
			{
				return new int?(data >> 5 & 15);
			}
			return null;
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000C718 File Offset: 0x0000A918
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -497) | 16 | (color.Value & 15) << 5;
			}
			return data & -497;
		}

		// Token: 0x040000F2 RID: 242
		public float m_pivotDistance;

		// Token: 0x040000F3 RID: 243
		public string m_modelName;

		// Token: 0x040000F4 RID: 244
		public bool m_doubleSided;

		// Token: 0x040000F5 RID: 245
		public bool m_useAlphaTest;

		// Token: 0x040000F6 RID: 246
		public int m_coloredTextureSlot;

		// Token: 0x040000F7 RID: 247
		public Color m_postColor;

		// Token: 0x040000F8 RID: 248
		public Color m_unpaintedColor;

		// Token: 0x040000F9 RID: 249
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x040000FA RID: 250
		public BlockMesh m_standaloneColoredBlockMesh = new BlockMesh();

		// Token: 0x040000FB RID: 251
		public BlockMesh[] m_blockMeshes = new BlockMesh[16];

		// Token: 0x040000FC RID: 252
		public BlockMesh[] m_coloredBlockMeshes = new BlockMesh[16];

		// Token: 0x040000FD RID: 253
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[16][];
	}
}
