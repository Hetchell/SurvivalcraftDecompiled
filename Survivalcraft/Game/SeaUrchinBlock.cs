using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000EB RID: 235
	public class SeaUrchinBlock : BottomSuckerBlock
	{
		// Token: 0x0600046C RID: 1132 RVA: 0x000179B8 File Offset: 0x00015BB8
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/SeaUrchin");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Urchin", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bottom", true).ParentBone);
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					Vector2 zero = Vector2.Zero;
					if (i < 4)
					{
						zero.Y = (float)i * 3.1415927f / 2f;
					}
					else if (i == 4)
					{
						zero.X = -1.5707964f;
					}
					else
					{
						zero.X = 1.5707964f;
					}
					Matrix m = Matrix.CreateRotationX(1.5707964f) * Matrix.CreateRotationZ(0.3f + 2f * (float)j) * Matrix.CreateTranslation(SeaUrchinBlock.m_offsets[j].X, SeaUrchinBlock.m_offsets[j].Y, -0.49f) * Matrix.CreateRotationX(zero.X) * Matrix.CreateRotationY(zero.Y) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f);
					int num = 4 * i + j;
					this.m_blockMeshes[num] = new BlockMesh();
					this.m_blockMeshes[num].AppendModelMeshPart(model.FindMesh("Urchin", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
					this.m_collisionBoxes[num] = new BoundingBox[]
					{
						this.m_blockMeshes[num].CalculateBoundingBox()
					};
				}
			}
			this.m_standaloneBlockMesh = new BlockMesh();
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Urchin", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Bottom", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x00017C10 File Offset: 0x00015E10
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int data = Terrain.ExtractData(value);
			int face = BottomSuckerBlock.GetFace(data);
			int subvariant = BottomSuckerBlock.GetSubvariant(data);
			return this.m_collisionBoxes[4 * face + subvariant];
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x00017C3C File Offset: 0x00015E3C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int face = BottomSuckerBlock.GetFace(data);
			int subvariant = BottomSuckerBlock.GetSubvariant(data);
			Color color = SeaUrchinBlock.m_colors[subvariant];
			generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshes[4 * face + subvariant], color, null, geometry.SubsetOpaque);
			base.GenerateTerrainVertices(generator, geometry, value, x, y, z);
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x00017CA0 File Offset: 0x00015EA0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color * new Color(40, 40, 40), 3f * size, ref matrix, environmentData);
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x00017CCB File Offset: 0x00015ECB
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return new BlockDebrisParticleSystem(subsystemTerrain, position, 0.75f * strength, this.DestructionDebrisScale, new Color(64, 64, 64), this.DefaultTextureSlot);
		}

		// Token: 0x040001FA RID: 506
		public new const int Index = 226;

		// Token: 0x040001FB RID: 507
		public BlockMesh[] m_blockMeshes = new BlockMesh[24];

		// Token: 0x040001FC RID: 508
		public BlockMesh m_standaloneBlockMesh;

		// Token: 0x040001FD RID: 509
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[24][];

		// Token: 0x040001FE RID: 510
		public static Color[] m_colors = new Color[]
		{
			new Color(20, 20, 20),
			new Color(50, 20, 20),
			new Color(80, 30, 30),
			new Color(20, 20, 40)
		};

		// Token: 0x040001FF RID: 511
		public static Vector2[] m_offsets = new Vector2[]
		{
			0.15f * new Vector2(-0.8f, -1f),
			0.15f * new Vector2(1f, -0.75f),
			0.15f * new Vector2(-0.65f, 1f),
			0.15f * new Vector2(0.9f, 0.7f)
		};
	}
}
