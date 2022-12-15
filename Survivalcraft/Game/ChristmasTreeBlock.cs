using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000032 RID: 50
	public class ChristmasTreeBlock : Block, IElectricElementBlock
	{
		// Token: 0x06000138 RID: 312 RVA: 0x000087DC File Offset: 0x000069DC
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/ChristmasTree");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("StandTrunk", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Leaves", true).ParentBone);
			Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Decorations", true).ParentBone);
			Color color = BlockColorsMap.SpruceLeavesColorsMap.Lookup(4, 15);
			this.m_leavesBlockMesh.AppendModelMeshPart(model.FindMesh("Leaves", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, true, false, Color.White);
			this.m_standTrunkBlockMesh.AppendModelMeshPart(model.FindMesh("StandTrunk", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
			this.m_decorationsBlockMesh.AppendModelMeshPart(model.FindMesh("Decorations", true).MeshParts[0], boneAbsoluteTransform3 * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
			this.m_litDecorationsBlockMesh.AppendModelMeshPart(model.FindMesh("Decorations", true).MeshParts[0], boneAbsoluteTransform3 * Matrix.CreateTranslation(0.5f, 0f, 0.5f), true, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("StandTrunk", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -1f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Leaves", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -1f, 0f), false, false, true, false, color);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Decorations", true).MeshParts[0], boneAbsoluteTransform3 * Matrix.CreateTranslation(0f, -1f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00008A4C File Offset: 0x00006C4C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			Color color = BlockColorsMap.SpruceLeavesColorsMap.Lookup(generator.Terrain, x, y, z);
			if (ChristmasTreeBlock.GetLightState(Terrain.ExtractData(value)))
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_standTrunkBlockMesh, Color.White, null, geometry.SubsetOpaque);
				generator.GenerateMeshVertices(this, x, y, z, this.m_litDecorationsBlockMesh, Color.White, null, geometry.SubsetOpaque);
				generator.GenerateMeshVertices(this, x, y, z, this.m_leavesBlockMesh, color, null, geometry.SubsetAlphaTest);
			}
			else
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_standTrunkBlockMesh, Color.White, null, geometry.SubsetOpaque);
				generator.GenerateMeshVertices(this, x, y, z, this.m_decorationsBlockMesh, Color.White, null, geometry.SubsetOpaque);
				generator.GenerateMeshVertices(this, x, y, z, this.m_leavesBlockMesh, color, null, geometry.SubsetAlphaTest);
			}
			generator.GenerateWireVertices(value, x, y, z, 4, 0.01f, Vector2.Zero, geometry.SubsetOpaque);
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00008B7F File Offset: 0x00006D7F
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00008B94 File Offset: 0x00006D94
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			Color color = BlockColorsMap.SpruceLeavesColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, color, this.DefaultTextureSlot);
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00008BE9 File Offset: 0x00006DE9
		public override int GetEmittedLightAmount(int value)
		{
			if (!ChristmasTreeBlock.GetLightState(Terrain.ExtractData(value)))
			{
				return 0;
			}
			return this.DefaultEmittedLightAmount;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00008C00 File Offset: 0x00006E00
		public override int GetShadowStrength(int value)
		{
			if (!ChristmasTreeBlock.GetLightState(Terrain.ExtractData(value)))
			{
				return this.DefaultShadowStrength;
			}
			return -99;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00008C18 File Offset: 0x00006E18
		public static bool GetLightState(int data)
		{
			return (data & 1) != 0;
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00008C20 File Offset: 0x00006E20
		public static int SetLightState(int data, bool state)
		{
			if (!state)
			{
				return data & -2;
			}
			return data | 1;
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00008C2D File Offset: 0x00006E2D
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new ChristmasTreeElectricElement(subsystemElectricity, new CellFace(x, y, z, 4), value);
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00008C44 File Offset: 0x00006E44
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if (face == 4 && SubsystemElectricity.GetConnectorDirection(4, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00008C78 File Offset: 0x00006E78
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x0400009D RID: 157
		public const int Index = 63;

		// Token: 0x0400009E RID: 158
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x0400009F RID: 159
		public BlockMesh m_leavesBlockMesh = new BlockMesh();

		// Token: 0x040000A0 RID: 160
		public BlockMesh m_standTrunkBlockMesh = new BlockMesh();

		// Token: 0x040000A1 RID: 161
		public BlockMesh m_decorationsBlockMesh = new BlockMesh();

		// Token: 0x040000A2 RID: 162
		public BlockMesh m_litDecorationsBlockMesh = new BlockMesh();
	}
}
