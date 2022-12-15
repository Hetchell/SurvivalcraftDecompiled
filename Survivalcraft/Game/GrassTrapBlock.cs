using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000078 RID: 120
	public class GrassTrapBlock : Block
	{
		// Token: 0x0600028E RID: 654 RVA: 0x0000F444 File Offset: 0x0000D644
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/GrassTrap");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("GrassTrap", true).ParentBone);
			Color color = BlockColorsMap.GrassColorsMap.Lookup(8, 15);
			this.m_blockMesh.AppendModelMeshPart(model.FindMesh("GrassTrap", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0.75f, 0.5f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("GrassTrap", true).MeshParts[0], boneAbsoluteTransform, false, false, false, false, color);
			this.m_collisionBoxes[0] = new BoundingBox(new Vector3(0f, 0.75f, 0f), new Vector3(1f, 0.95f, 1f));
			base.Initialize();
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0000F534 File Offset: 0x0000D734
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateShadedMeshVertices(this, x, y, z, this.m_blockMesh, BlockColorsMap.GrassColorsMap.Lookup(generator.Terrain, x, y, z), null, null, geometry.SubsetAlphaTest);
		}

		// Token: 0x06000290 RID: 656 RVA: 0x0000F57A File Offset: 0x0000D77A
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000F590 File Offset: 0x0000D790
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			Color color = BlockColorsMap.GrassColorsMap.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, color, this.GetFaceTextureSlot(4, value));
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000F5E7 File Offset: 0x0000D7E7
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.m_collisionBoxes;
		}

		// Token: 0x04000124 RID: 292
		public const int Index = 87;

		// Token: 0x04000125 RID: 293
		public BlockMesh m_blockMesh = new BlockMesh();

		// Token: 0x04000126 RID: 294
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000127 RID: 295
		public BoundingBox[] m_collisionBoxes = new BoundingBox[1];
	}
}
