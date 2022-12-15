using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200001A RID: 26
	public class BatteryBlock : Block, IElectricElementBlock
	{
		// Token: 0x060000D1 RID: 209 RVA: 0x00006D4C File Offset: 0x00004F4C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Battery");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Battery", true).ParentBone);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Battery", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			this.m_blockMesh.AppendModelMeshPart(model.FindMesh("Battery", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
			this.m_collisionBoxes[0] = this.m_blockMesh.CalculateBoundingBox();
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00006E24 File Offset: 0x00005024
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			if (toolLevel >= this.RequiredToolLevel)
			{
				int data = Terrain.ExtractData(oldValue);
				dropValues.Add(new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(138, 0, data),
					Count = 1
				});
			}
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00006E71 File Offset: 0x00005071
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.m_collisionBoxes;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00006E7C File Offset: 0x0000507C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(this, x, y, z, this.m_blockMesh, Color.White, null, geometry.SubsetOpaque);
			generator.GenerateWireVertices(value, x, y, z, 4, 0.72f, Vector2.Zero, geometry.SubsetOpaque);
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00006ECE File Offset: 0x000050CE
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 1f * size, ref matrix, environmentData);
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00006EE9 File Offset: 0x000050E9
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new BatteryElectricElement(subsystemElectricity, new CellFace(x, y, z, 4));
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00006EFC File Offset: 0x000050FC
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if (face == 4 && SubsystemElectricity.GetConnectorDirection(4, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			return null;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00006F30 File Offset: 0x00005130
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00006F37 File Offset: 0x00005137
		public static int GetVoltageLevel(int data)
		{
			return 15 - (data & 15);
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00006F40 File Offset: 0x00005140
		public static int SetVoltageLevel(int data, int voltageLevel)
		{
			return (data & -16) | 15 - (voltageLevel & 15);
		}

		// Token: 0x04000072 RID: 114
		public const int Index = 138;

		// Token: 0x04000073 RID: 115
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x04000074 RID: 116
		public BlockMesh m_blockMesh = new BlockMesh();

		// Token: 0x04000075 RID: 117
		public BoundingBox[] m_collisionBoxes = new BoundingBox[1];
	}
}
