using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C8 RID: 200
	public class PistonHeadBlock : Block
	{
		// Token: 0x060003E4 RID: 996 RVA: 0x00015214 File Offset: 0x00013414
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Pistons");
			for (int i = 0; i < 2; i++)
			{
				string name = (i == 0) ? "PistonHead" : "PistonShaft";
				Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(name, true).ParentBone);
				for (PistonMode pistonMode = PistonMode.Pushing; pistonMode <= PistonMode.StrictPulling; pistonMode++)
				{
					for (int j = 0; j < 6; j++)
					{
						int num = PistonHeadBlock.SetFace(PistonHeadBlock.SetMode(PistonHeadBlock.SetIsShaft(0, i != 0), pistonMode), j);
						Matrix m = (j < 4) ? (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationY((float)j * 3.1415927f / 2f + 3.1415927f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)) : ((j != 4) ? (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationX(-1.5707964f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)) : (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationX(1.5707964f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)));
						this.m_blockMeshesByData[num] = new BlockMesh();
						this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh(name, true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
						if (pistonMode != PistonMode.Pulling)
						{
							if (pistonMode == PistonMode.StrictPulling)
							{
								this.m_blockMeshesByData[num].TransformTextureCoordinates(Matrix.CreateTranslation(0f, 0.125f, 0f), 1 << j);
							}
						}
						else
						{
							this.m_blockMeshesByData[num].TransformTextureCoordinates(Matrix.CreateTranslation(0f, 0.0625f, 0f), 1 << j);
						}
					}
				}
			}
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x00015428 File Offset: 0x00013628
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			int data = Terrain.ExtractData(value);
			return face != PistonHeadBlock.GetFace(data);
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x00015448 File Offset: 0x00013648
		public override int GetShadowStrength(int value)
		{
			if (!PistonHeadBlock.GetIsShaft(Terrain.ExtractData(value)))
			{
				return base.GetShadowStrength(value);
			}
			return 0;
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x00015460 File Offset: 0x00013660
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length && this.m_blockMeshesByData[num] != null)
			{
				generator.GenerateShadedMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, null, geometry.SubsetOpaque);
			}
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x000154B3 File Offset: 0x000136B3
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x000154B5 File Offset: 0x000136B5
		public static PistonMode GetMode(int data)
		{
			return (PistonMode)(data & 3);
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x000154BA File Offset: 0x000136BA
		public static int SetMode(int data, PistonMode mode)
		{
			return (data & -4) | (int)(mode & (PistonMode)3);
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x000154C4 File Offset: 0x000136C4
		public static bool GetIsShaft(int data)
		{
			return (data & 4) != 0;
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x000154CC File Offset: 0x000136CC
		public static int SetIsShaft(int data, bool isShaft)
		{
			return (data & -5) | (isShaft ? 4 : 0);
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x000154DA File Offset: 0x000136DA
		public static int GetFace(int data)
		{
			return data >> 3 & 7;
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x000154E1 File Offset: 0x000136E1
		public static int SetFace(int data, int face)
		{
			return (data & -57) | (face & 7) << 3;
		}

		// Token: 0x040001BA RID: 442
		public const int Index = 238;

		// Token: 0x040001BB RID: 443
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[48];
	}
}
