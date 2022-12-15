using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000070 RID: 112
	public class FurnaceBlock : Block
	{
		// Token: 0x06000260 RID: 608 RVA: 0x0000E0B8 File Offset: 0x0000C2B8
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Furnace");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Furnace", true).ParentBone);
			for (int i = 0; i < 4; i++)
			{
				this.m_blockMeshesByData[i] = new BlockMesh();
				Matrix matrix = Matrix.Identity;
				matrix *= Matrix.CreateRotationY((float)i * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				this.m_blockMeshesByData[i].AppendModelMeshPart(model.FindMesh("Furnace", true).MeshParts[0], boneAbsoluteTransform * matrix, false, false, false, false, Color.White);
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Furnace", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000E1C6 File Offset: 0x0000C3C6
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return false;
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000E1CC File Offset: 0x0000C3CC
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length)
			{
				generator.GenerateShadedMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, null, geometry.SubsetAlphaTest);
			}
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000E215 File Offset: 0x0000C415
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000E22C File Offset: 0x0000C42C
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int data = 0;
			if (num == MathUtils.Max(num, num2, num3, num4))
			{
				data = 2;
			}
			else if (num2 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 3;
			}
			else if (num3 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 0;
			}
			else if (num4 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 1;
			}
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, 64), data),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x04000117 RID: 279
		public const int Index = 64;

		// Token: 0x04000118 RID: 280
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000119 RID: 281
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[4];
	}
}
