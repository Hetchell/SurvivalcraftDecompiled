using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000124 RID: 292
	public abstract class WoodBlock : CubeBlock
	{
		// Token: 0x06000587 RID: 1415 RVA: 0x0001E387 File Offset: 0x0001C587
		public WoodBlock(int cutTextureSlot, int sideTextureSlot)
		{
			this.m_cutTextureSlot = cutTextureSlot;
			this.m_sideTextureSlot = sideTextureSlot;
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x0001E3A0 File Offset: 0x0001C5A0
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int cutFace = WoodBlock.GetCutFace(Terrain.ExtractData(value));
			if (cutFace == 0)
			{
				generator.GenerateCubeVertices(this, value, x, y, z, 1, 0, 0, Color.White, geometry.OpaqueSubsetsByFace);
				return;
			}
			if (cutFace == 4)
			{
				generator.GenerateCubeVertices(this, value, x, y, z, Color.White, geometry.OpaqueSubsetsByFace);
				return;
			}
			generator.GenerateCubeVertices(this, value, x, y, z, 0, 1, 1, Color.White, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x0001E414 File Offset: 0x0001C614
		public override int GetFaceTextureSlot(int face, int value)
		{
			int cutFace = WoodBlock.GetCutFace(Terrain.ExtractData(value));
			if (cutFace == face || CellFace.OppositeFace(cutFace) == face)
			{
				return this.m_cutTextureSlot;
			}
			return this.m_sideTextureSlot;
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x0001E448 File Offset: 0x0001C648
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = float.NegativeInfinity;
			int cutFace = 0;
			for (int i = 0; i < 6; i++)
			{
				float num2 = Vector3.Dot(CellFace.FaceToVector3(i), forward);
				if (num2 > num)
				{
					num = num2;
					cutFace = i;
				}
			}
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, WoodBlock.SetCutFace(0, cutFace)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x0001E4D8 File Offset: 0x0001C6D8
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			data = WoodBlock.SetCutFace(data, 4);
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, data),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x0001E524 File Offset: 0x0001C724
		public static int GetCutFace(int data)
		{
			data &= 3;
			if (data == 0)
			{
				return 4;
			}
			if (data != 1)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x0001E539 File Offset: 0x0001C739
		public static int SetCutFace(int data, int cutFace)
		{
			data &= -4;
			switch (cutFace)
			{
			case 0:
			case 2:
				return data | 1;
			case 1:
			case 3:
				return data | 2;
			default:
				return data;
			}
		}

		// Token: 0x0400026D RID: 621
		public int m_cutTextureSlot;

		// Token: 0x0400026E RID: 622
		public int m_sideTextureSlot;
	}
}
