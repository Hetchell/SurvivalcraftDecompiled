using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000F9 RID: 249
	public class SpikedPlankBlock : MountedElectricElementBlock
	{
		// Token: 0x060004C4 RID: 1220 RVA: 0x000194E0 File Offset: 0x000176E0
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/SpikedPlanks");
			string[] array = new string[]
			{
				"SpikedPlankRetracted",
				"SpikedPlank"
			};
			for (int i = 0; i < 2; i++)
			{
				string name = array[i];
				Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(name, true).ParentBone);
				for (int j = 0; j < 6; j++)
				{
					int num = SpikedPlankBlock.SetMountingFace(SpikedPlankBlock.SetSpikesState(0, i != 0), j);
					Matrix m = (j >= 4) ? ((j != 4) ? (Matrix.CreateRotationX(3.1415927f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.5707964f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)j * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
					this.m_blockMeshesByData[num] = new BlockMesh();
					this.m_blockMeshesByData[num].AppendModelMeshPart(model.FindMesh(name, true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
					this.m_collisionBoxesByData[num] = new BoundingBox[]
					{
						this.m_blockMeshesByData[num].CalculateBoundingBox()
					};
				}
				Matrix identity = Matrix.Identity;
				this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh(name, true).MeshParts[0], boneAbsoluteTransform * identity, false, false, false, false, Color.White);
			}
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x000196A4 File Offset: 0x000178A4
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			int mountingFace = SpikedPlankBlock.GetMountingFace(Terrain.ExtractData(value));
			return face != CellFace.OppositeFace(mountingFace);
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x000196C9 File Offset: 0x000178C9
		public override bool ShouldAvoid(int value)
		{
			return SpikedPlankBlock.GetSpikesState(Terrain.ExtractData(value));
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x000196D6 File Offset: 0x000178D6
		public static bool GetSpikesState(int data)
		{
			return (data & 1) == 0;
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x000196DE File Offset: 0x000178DE
		public static int SetSpikesState(int data, bool spikesState)
		{
			if (spikesState)
			{
				return data & -2;
			}
			return data | 1;
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x000196EB File Offset: 0x000178EB
		public static int GetMountingFace(int data)
		{
			return ((data >> 1) + 4) % 6;
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x000196F4 File Offset: 0x000178F4
		public static int SetMountingFace(int data, int face)
		{
			data &= -15;
			data |= ((face + 2) % 6 & 7) << 1;
			return data;
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x0001970A File Offset: 0x0001790A
		public override int GetFace(int value)
		{
			return SpikedPlankBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x00019718 File Offset: 0x00017918
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = SpikedPlankBlock.SetMountingFace(SpikedPlankBlock.SetSpikesState(Terrain.ExtractData(value), true), raycastResult.CellFace.Face);
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(value, data),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x00019768 File Offset: 0x00017968
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= this.m_collisionBoxesByData.Length)
			{
				return base.GetCustomCollisionBoxes(terrain, value);
			}
			return this.m_collisionBoxesByData[num];
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x00019798 File Offset: 0x00017998
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshesByData.Length && this.m_blockMeshesByData[num] != null)
			{
				generator.GenerateShadedMeshVertices(this, x, y, z, this.m_blockMeshesByData[num], Color.White, null, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, this.GetFace(value), 1f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x0001980F File Offset: 0x00017A0F
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 1f * size, ref matrix, environmentData);
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0001982A File Offset: 0x00017A2A
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new SpikedPlankElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x00019844 File Offset: 0x00017A44
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x0400021D RID: 541
		public const int Index = 86;

		// Token: 0x0400021E RID: 542
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x0400021F RID: 543
		public BlockMesh[] m_blockMeshesByData = new BlockMesh[12];

		// Token: 0x04000220 RID: 544
		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[12][];
	}
}
