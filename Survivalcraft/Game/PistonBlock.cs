using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C7 RID: 199
	public class PistonBlock : Block, IElectricElementBlock
	{
		// Token: 0x060003CC RID: 972 RVA: 0x00014C7C File Offset: 0x00012E7C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Pistons");
			for (PistonMode pistonMode = PistonMode.Pushing; pistonMode <= PistonMode.StrictPulling; pistonMode++)
			{
				for (int i = 0; i < 2; i++)
				{
					string name = (i == 0) ? "PistonRetracted" : "PistonExtended";
					Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(name, true).ParentBone);
					for (int j = 0; j < 6; j++)
					{
						int num = PistonBlock.SetFace(PistonBlock.SetIsExtended(PistonBlock.SetMode(0, pistonMode), i != 0), j);
						Matrix m = (j < 4) ? (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationY((float)j * 3.1415927f / 2f + 3.1415927f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)) : ((j != 4) ? (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationX(-1.5707964f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)) : (Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateRotationX(1.5707964f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f)));
						this.m_blockMeshesByData[num] = new BlockMesh();
						this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh(name, true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
						if (i == 0)
						{
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
				Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("PistonRetracted", true).ParentBone);
				this.m_standaloneBlockMeshes[(int)pistonMode] = new BlockMesh();
				this.m_standaloneBlockMeshes[(int)pistonMode].AppendModelMeshPart(model.FindMesh("PistonRetracted", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
				if (pistonMode != PistonMode.Pulling)
				{
					if (pistonMode == PistonMode.StrictPulling)
					{
						this.m_standaloneBlockMeshes[(int)pistonMode].TransformTextureCoordinates(Matrix.CreateTranslation(0f, 0.125f, 0f), 4);
					}
				}
				else
				{
					this.m_standaloneBlockMeshes[(int)pistonMode].TransformTextureCoordinates(Matrix.CreateTranslation(0f, 0.0625f, 0f), 4);
				}
			}
		}

		// Token: 0x060003CD RID: 973 RVA: 0x00014F50 File Offset: 0x00013150
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			int data = Terrain.ExtractData(value);
			int face2 = PistonBlock.GetFace(data);
			return PistonBlock.GetIsExtended(data) && face != face2 && face != CellFace.OppositeFace(face2);
		}

		// Token: 0x060003CE RID: 974 RVA: 0x00014F88 File Offset: 0x00013188
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value) & 63;
			if (num < this.m_blockMeshesByData.Length && this.m_blockMeshesByData[num] != null)
			{
				generator.GenerateShadedMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, null, geometry.SubsetOpaque);
			}
		}

		// Token: 0x060003CF RID: 975 RVA: 0x00014FE0 File Offset: 0x000131E0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int mode = (int)PistonBlock.GetMode(Terrain.ExtractData(value));
			if (mode < this.m_standaloneBlockMeshes.Length && this.m_standaloneBlockMeshes[mode] != null)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshes[mode], color, 1f * size, ref matrix, environmentData);
			}
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x00015029 File Offset: 0x00013229
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new PistonElectricElement(subsystemElectricity, new Point3(x, y, z));
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x0001503B File Offset: 0x0001323B
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return new ElectricConnectorType?(ElectricConnectorType.Input);
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x00015043 File Offset: 0x00013243
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x0001504A File Offset: 0x0001324A
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(237, 0, PistonBlock.SetMode(PistonBlock.SetMaxExtension(0, 7), PistonMode.Pushing));
			yield return Terrain.MakeBlockValue(237, 0, PistonBlock.SetMode(PistonBlock.SetMaxExtension(0, 7), PistonMode.Pulling));
			yield return Terrain.MakeBlockValue(237, 0, PistonBlock.SetMode(PistonBlock.SetMaxExtension(0, 7), PistonMode.StrictPulling));
			yield break;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x00015054 File Offset: 0x00013254
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			PistonMode mode = PistonBlock.GetMode(Terrain.ExtractData(value));
			if (mode == PistonMode.Pulling)
			{
				return "粘性活塞";
			}
			if (mode != PistonMode.StrictPulling)
			{
				return "活塞";
			}
			return "严格粘性活塞";
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00015088 File Offset: 0x00013288
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = float.PositiveInfinity;
			int face = 0;
			for (int i = 0; i < 6; i++)
			{
				float num2 = Vector3.Dot(CellFace.FaceToVector3(i), forward);
				if (num2 < num)
				{
					num = num2;
					face = i;
				}
			}
			int data = Terrain.ExtractData(value);
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, PistonBlock.SetFace(data, face)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x00015120 File Offset: 0x00013320
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(237, 0, PistonBlock.SetFace(PistonBlock.SetIsExtended(data, false), 0)),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x0001516F File Offset: 0x0001336F
		public static bool GetIsExtended(int data)
		{
			return (data & 1) != 0;
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x00015177 File Offset: 0x00013377
		public static int SetIsExtended(int data, bool isExtended)
		{
			return (data & -2) | (isExtended ? 1 : 0);
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x00015185 File Offset: 0x00013385
		public static PistonMode GetMode(int data)
		{
			return (PistonMode)(data >> 1 & 3);
		}

		// Token: 0x060003DA RID: 986 RVA: 0x0001518C File Offset: 0x0001338C
		public static int SetMode(int data, PistonMode mode)
		{
			return (data & -7) | (int)((int)(mode & (PistonMode)3) << 1);
		}

		// Token: 0x060003DB RID: 987 RVA: 0x00015198 File Offset: 0x00013398
		public static int GetFace(int data)
		{
			return data >> 3 & 7;
		}

		// Token: 0x060003DC RID: 988 RVA: 0x0001519F File Offset: 0x0001339F
		public static int SetFace(int data, int face)
		{
			return (data & -57) | (face & 7) << 3;
		}

		// Token: 0x060003DD RID: 989 RVA: 0x000151AB File Offset: 0x000133AB
		public static int GetMaxExtension(int data)
		{
			return data >> 6 & 7;
		}

		// Token: 0x060003DE RID: 990 RVA: 0x000151B2 File Offset: 0x000133B2
		public static int SetMaxExtension(int data, int maxExtension)
		{
			return (data & -449) | (maxExtension & 7) << 6;
		}

		// Token: 0x060003DF RID: 991 RVA: 0x000151C1 File Offset: 0x000133C1
		public static int GetPullCount(int data)
		{
			return data >> 9 & 7;
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x000151C9 File Offset: 0x000133C9
		public static int SetPullCount(int data, int pullCount)
		{
			return (data & -3585) | (pullCount & 7) << 9;
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x000151D9 File Offset: 0x000133D9
		public static int GetSpeed(int data)
		{
			return data >> 12 & 7;
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x000151E1 File Offset: 0x000133E1
		public static int SetSpeed(int data, int speed)
		{
			return (data & -12289) | (speed & 3) << 12;
		}

		// Token: 0x040001B7 RID: 439
		public const int Index = 237;

		// Token: 0x040001B8 RID: 440
		public BlockMesh[] m_standaloneBlockMeshes = new BlockMesh[4];

		// Token: 0x040001B9 RID: 441
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[48];
	}
}
