using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200007C RID: 124
	public abstract class GunpowderKegBlock : Block, IElectricElementBlock
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060002A3 RID: 675 RVA: 0x0000FCEB File Offset: 0x0000DEEB
		// (set) Token: 0x060002A4 RID: 676 RVA: 0x0000FCF3 File Offset: 0x0000DEF3
		public Vector3 FuseOffset { get; set; }

		// Token: 0x060002A5 RID: 677 RVA: 0x0000FCFC File Offset: 0x0000DEFC
		public GunpowderKegBlock(string modelName, bool isIncendiary)
		{
			this.m_modelName = modelName;
			this.m_isIncendiary = isIncendiary;
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0000FD28 File Offset: 0x0000DF28
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Keg", true).ParentBone);
			this.FuseOffset = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Fuse", true).ParentBone).Translation + new Vector3(0.5f, 0f, 0.5f);
			BlockMesh blockMesh = new BlockMesh();
			blockMesh.AppendModelMeshPart(model.FindMesh("Keg", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
			this.m_blockMesh.AppendBlockMesh(blockMesh);
			if (this.m_isIncendiary)
			{
				this.m_blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(-0.25f, 0f, 0f), -1);
			}
			this.m_collisionBoxes = new BoundingBox[]
			{
				blockMesh.CalculateBoundingBox()
			};
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Keg", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			if (this.m_isIncendiary)
			{
				this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(-0.25f, 0f, 0f), -1);
			}
			base.Initialize();
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x0000FEA4 File Offset: 0x0000E0A4
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(this, x, y, z, this.m_blockMesh, Color.White, null, geometry.SubsetOpaque);
			generator.GenerateWireVertices(value, x, y, z, 4, 0.25f, Vector2.Zero, geometry.SubsetOpaque);
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0000FEF6 File Offset: 0x0000E0F6
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x0000FF0B File Offset: 0x0000E10B
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.m_collisionBoxes;
		}

		// Token: 0x060002AA RID: 682 RVA: 0x0000FF13 File Offset: 0x0000E113
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new GunpowderKegElectricElement(subsystemElectricity, new CellFace(x, y, z, 4));
		}

		// Token: 0x060002AB RID: 683 RVA: 0x0000FF28 File Offset: 0x0000E128
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if (face == 4)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x060002AC RID: 684 RVA: 0x0000FF49 File Offset: 0x0000E149
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x0400012E RID: 302
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x0400012F RID: 303
		public BlockMesh m_blockMesh = new BlockMesh();

		// Token: 0x04000130 RID: 304
		public BoundingBox[] m_collisionBoxes;

		// Token: 0x04000131 RID: 305
		public string m_modelName;

		// Token: 0x04000132 RID: 306
		public bool m_isIncendiary;
	}
}
