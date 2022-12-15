using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000013 RID: 19
	public abstract class AttachedSignBlock : SignBlock, IElectricElementBlock, IPaintableBlock
	{
		// Token: 0x060000A2 RID: 162 RVA: 0x00005EBC File Offset: 0x000040BC
		public AttachedSignBlock(string modelName, int coloredTextureSlot, int postedSignBlockIndex)
		{
			this.m_modelName = modelName;
			this.m_coloredTextureSlot = coloredTextureSlot;
			this.m_postedSignBlockIndex = postedSignBlockIndex;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00005F38 File Offset: 0x00004138
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Sign", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Surface", true).ParentBone);
			for (int i = 0; i < 4; i++)
			{
				float radians = 1.5707964f * (float)i;
				Matrix m = Matrix.CreateTranslation(0f, 0f, -0.46875f) * Matrix.CreateRotationY(radians) * Matrix.CreateTranslation(0.5f, -0.3125f, 0.5f);
				BlockMesh blockMesh = new BlockMesh();
				blockMesh.AppendModelMeshPart(model.FindMesh("Sign", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_blockMeshes[i] = new BlockMesh();
				this.m_blockMeshes[i].AppendBlockMesh(blockMesh);
				this.m_coloredBlockMeshes[i] = new BlockMesh();
				this.m_coloredBlockMeshes[i].AppendBlockMesh(this.m_blockMeshes[i]);
				this.m_blockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
				this.m_coloredBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
				this.m_collisionBoxes[i] = new BoundingBox[1];
				this.m_collisionBoxes[i][0] = blockMesh.CalculateBoundingBox();
				this.m_surfaceMeshes[i] = new BlockMesh();
				this.m_surfaceMeshes[i].AppendModelMeshPart(model.FindMesh("Surface", true).MeshParts[0], boneAbsoluteTransform2 * m, false, false, false, false, Color.White);
				this.m_surfaceNormals[i] = -m.Forward;
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Sign", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.6f, 0f), false, false, false, false, Color.White);
			this.m_standaloneColoredBlockMesh.AppendBlockMesh(this.m_standaloneBlockMesh);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
			this.m_standaloneColoredBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
			base.Initialize();
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x0000620C File Offset: 0x0000440C
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			int? color = AttachedSignBlock.GetColor(Terrain.ExtractData(oldValue));
			int data = PostedSignBlock.SetColor(0, color);
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(this.m_postedSignBlockIndex, 0, data),
				Count = 1
			});
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00006260 File Offset: 0x00004460
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int? color = AttachedSignBlock.GetColor(Terrain.ExtractData(value));
			if (color != null)
			{
				return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, SubsystemPalette.GetColor(subsystemTerrain, color), this.m_coloredTextureSlot);
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.DefaultTextureSlot);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000062BC File Offset: 0x000044BC
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int face = AttachedSignBlock.GetFace(data);
			int? color = AttachedSignBlock.GetColor(data);
			if (color != null)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_coloredBlockMeshes[face], SubsystemPalette.GetColor(generator, color), null, geometry.SubsetOpaque);
			}
			else
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshes[face], Color.White, null, geometry.SubsetOpaque);
			}
			generator.GenerateWireVertices(value, x, y, z, AttachedSignBlock.GetFace(data), 0.375f, Vector2.Zero, geometry.SubsetOpaque);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00006360 File Offset: 0x00004560
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? color2 = AttachedSignBlock.GetColor(Terrain.ExtractData(value));
			if (color2 != null)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneColoredBlockMesh, color * SubsystemPalette.GetColor(environmentData, color2), 1.25f * size, ref matrix, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 1.25f * size, ref matrix, environmentData);
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x000063C2 File Offset: 0x000045C2
		public int? GetPaintColor(int value)
		{
			return AttachedSignBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000063D0 File Offset: 0x000045D0
		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, AttachedSignBlock.SetColor(data, color));
		}

		// Token: 0x060000AA RID: 170 RVA: 0x000063F4 File Offset: 0x000045F4
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int face = AttachedSignBlock.GetFace(Terrain.ExtractData(value));
			return this.m_collisionBoxes[face];
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00006418 File Offset: 0x00004618
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return default(BlockPlacementData);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x0000642E File Offset: 0x0000462E
		public override BlockMesh GetSignSurfaceBlockMesh(int data)
		{
			return this.m_surfaceMeshes[AttachedSignBlock.GetFace(data)];
		}

		// Token: 0x060000AD RID: 173 RVA: 0x0000643D File Offset: 0x0000463D
		public override Vector3 GetSignSurfaceNormal(int data)
		{
			return this.m_surfaceNormals[AttachedSignBlock.GetFace(data)];
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00006450 File Offset: 0x00004650
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			return new SignElectricElement(subsystemElectricity, new CellFace(x, y, z, AttachedSignBlock.GetFace(data)));
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0000647C File Offset: 0x0000467C
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			if (face != AttachedSignBlock.GetFace(data) || SubsystemElectricity.GetConnectorDirection(face, 0, connectorFace) == null)
			{
				return null;
			}
			return new ElectricConnectorType?(ElectricConnectorType.Input);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000064BC File Offset: 0x000046BC
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000064C3 File Offset: 0x000046C3
		public static int GetFace(int data)
		{
			return data & 3;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000064C8 File Offset: 0x000046C8
		public static int SetFace(int data, int face)
		{
			return (data & -4) | (face & 3);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000064D4 File Offset: 0x000046D4
		public static int? GetColor(int data)
		{
			if ((data & 4) != 0)
			{
				return new int?(data >> 3 & 15);
			}
			return null;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x000064FB File Offset: 0x000046FB
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -125) | 4 | (color.Value & 15) << 3;
			}
			return data & -125;
		}

		// Token: 0x0400005D RID: 93
		public string m_modelName;

		// Token: 0x0400005E RID: 94
		public int m_coloredTextureSlot;

		// Token: 0x0400005F RID: 95
		public int m_postedSignBlockIndex;

		// Token: 0x04000060 RID: 96
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000061 RID: 97
		public BlockMesh m_standaloneColoredBlockMesh = new BlockMesh();

		// Token: 0x04000062 RID: 98
		public BlockMesh[] m_blockMeshes = new BlockMesh[4];

		// Token: 0x04000063 RID: 99
		public BlockMesh[] m_coloredBlockMeshes = new BlockMesh[4];

		// Token: 0x04000064 RID: 100
		public BlockMesh[] m_surfaceMeshes = new BlockMesh[4];

		// Token: 0x04000065 RID: 101
		public Vector3[] m_surfaceNormals = new Vector3[4];

		// Token: 0x04000066 RID: 102
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[4][];
	}
}
