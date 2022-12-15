using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000CB RID: 203
	public class PressurePlateBlock : MountedElectricElementBlock
	{
		// Token: 0x0600040B RID: 1035 RVA: 0x00015EF4 File Offset: 0x000140F4
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/PressurePlate");
			for (int i = 0; i < 2; i++)
			{
				Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("PressurePlate", true).ParentBone);
				int num = this.m_textureSlotsByMaterial[i];
				for (int j = 0; j < 6; j++)
				{
					int num2 = PressurePlateBlock.SetMountingFace(PressurePlateBlock.SetMaterial(0, i), j);
					Matrix matrix = (j >= 4) ? ((j != 4) ? (Matrix.CreateRotationX(3.1415927f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.5707964f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)j * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
					this.m_blockMeshesByData[num2] = new BlockMesh();
					this.m_blockMeshesByData[num2].AppendModelMeshPart(model.FindMesh("PressurePlate", true).MeshParts[0], boneAbsoluteTransform * matrix, false, false, false, false, Color.White);
					this.m_blockMeshesByData[num2].TransformTextureCoordinates(Matrix.CreateTranslation((float)(num % 16) / 16f, (float)(num / 16) / 16f, 0f), -1);
					this.m_blockMeshesByData[num2].GenerateSidesData();
					Vector3 vector = Vector3.Transform(new Vector3(-0.5f, 0f, -0.5f), matrix);
					Vector3 vector2 = Vector3.Transform(new Vector3(0.5f, 0.0625f, 0.5f), matrix);
					vector.X = MathUtils.Round(vector.X * 100f) / 100f;
					vector.Y = MathUtils.Round(vector.Y * 100f) / 100f;
					vector.Z = MathUtils.Round(vector.Z * 100f) / 100f;
					vector2.X = MathUtils.Round(vector2.X * 100f) / 100f;
					vector2.Y = MathUtils.Round(vector2.Y * 100f) / 100f;
					vector2.Z = MathUtils.Round(vector2.Z * 100f) / 100f;
					this.m_collisionBoxesByData[num2] = new BoundingBox[]
					{
						new BoundingBox(new Vector3(MathUtils.Min(vector.X, vector2.X), MathUtils.Min(vector.Y, vector2.Y), MathUtils.Min(vector.Z, vector2.Z)), new Vector3(MathUtils.Max(vector.X, vector2.X), MathUtils.Max(vector.Y, vector2.Y), MathUtils.Max(vector.Z, vector2.Z)))
					};
				}
				Matrix identity = Matrix.Identity;
				this.m_standaloneBlockMeshesByMaterial[i] = new BlockMesh();
				this.m_standaloneBlockMeshesByMaterial[i].AppendModelMeshPart(model.FindMesh("PressurePlate", true).MeshParts[0], boneAbsoluteTransform * identity, false, false, false, false, Color.White);
				this.m_standaloneBlockMeshesByMaterial[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(num % 16) / 16f, (float)(num / 16) / 16f, 0f), -1);
			}
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x0001628C File Offset: 0x0001448C
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int material = PressurePlateBlock.GetMaterial(Terrain.ExtractData(value));
			return this.m_displayNamesByMaterial[material];
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x000162AD File Offset: 0x000144AD
		public override IEnumerable<int> GetCreativeValues()
		{
			return this.m_creativeValuesByMaterial;
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x000162B8 File Offset: 0x000144B8
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int material = PressurePlateBlock.GetMaterial(Terrain.ExtractData(value));
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.m_textureSlotsByMaterial[material]);
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x000162F0 File Offset: 0x000144F0
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = PressurePlateBlock.SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face);
			int value2 = Terrain.ReplaceData(value, data);
			return new BlockPlacementData
			{
				Value = value2,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x0001633C File Offset: 0x0001453C
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int material = PressurePlateBlock.GetMaterial(Terrain.ExtractData(oldValue));
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(144, 0, PressurePlateBlock.SetMaterial(0, material)),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x0001638C File Offset: 0x0001458C
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return null;
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x000163B5 File Offset: 0x000145B5
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return face != CellFace.OppositeFace(this.GetFace(value));
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x000163CC File Offset: 0x000145CC
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length && this.m_blockMeshesByData[num] != null)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 0.8125f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x00016444 File Offset: 0x00014644
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int material = PressurePlateBlock.GetMaterial(Terrain.ExtractData(value));
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshesByMaterial[material], color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x00016478 File Offset: 0x00014678
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new PressurePlateElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x00016494 File Offset: 0x00014694
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x000164D0 File Offset: 0x000146D0
		public static int GetMaterial(int data)
		{
			return data & 1;
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x000164D5 File Offset: 0x000146D5
		public static int SetMaterial(int data, int material)
		{
			return (data & -2) | (material & 1);
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x000164DF File Offset: 0x000146DF
		public static int GetMountingFace(int data)
		{
			return data >> 1 & 7;
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x000164E6 File Offset: 0x000146E6
		public static int SetMountingFace(int data, int face)
		{
			return (data & -15) | (face & 7) << 1;
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x000164F2 File Offset: 0x000146F2
		public override int GetFace(int value)
		{
			return PressurePlateBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x040001C8 RID: 456
		public const int Index = 144;

		// Token: 0x040001C9 RID: 457
		public BlockMesh[] m_standaloneBlockMeshesByMaterial = new BlockMesh[2];

		// Token: 0x040001CA RID: 458
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[16];

		// Token: 0x040001CB RID: 459
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[16][];

		// Token: 0x040001CC RID: 460
		public string[] m_displayNamesByMaterial = new string[]
		{
			"木质压力板",
			"石质压力板"
		};

		// Token: 0x040001CD RID: 461
		public int[] m_creativeValuesByMaterial = new int[]
		{
			Terrain.MakeBlockValue(144, 0, 0),
			Terrain.MakeBlockValue(144, 0, 1)
		};

		// Token: 0x040001CE RID: 462
		public int[] m_textureSlotsByMaterial = new int[]
		{
			4,
			1
		};
	}
}
