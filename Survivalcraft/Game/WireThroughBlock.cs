using System;
using Engine;

namespace Game
{
	// Token: 0x0200011E RID: 286
	public abstract class WireThroughBlock : CubeBlock, IElectricWireElementBlock, IElectricElementBlock
	{
		// Token: 0x06000579 RID: 1401 RVA: 0x0001E1A3 File Offset: 0x0001C3A3
		public WireThroughBlock(int wiredTextureSlot, int unwiredTextureSlot)
		{
			this.m_wiredTextureSlot = wiredTextureSlot;
			this.m_unwiredTextureSlot = unwiredTextureSlot;
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x0001E1B9 File Offset: 0x0001C3B9
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return null;
		}

		// Token: 0x0600057B RID: 1403 RVA: 0x0001E1BC File Offset: 0x0001C3BC
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int wiredFace = WireThroughBlock.GetWiredFace(Terrain.ExtractData(value));
			if ((face == wiredFace || face == CellFace.OppositeFace(wiredFace)) && connectorFace == CellFace.OppositeFace(face))
			{
				return new ElectricConnectorType?(ElectricConnectorType.InputOutput);
			}
			return null;
		}

		// Token: 0x0600057C RID: 1404 RVA: 0x0001E1FC File Offset: 0x0001C3FC
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x0001E204 File Offset: 0x0001C404
		public int GetConnectedWireFacesMask(int value, int face)
		{
			int wiredFace = WireThroughBlock.GetWiredFace(Terrain.ExtractData(value));
			if (wiredFace == face || CellFace.OppositeFace(wiredFace) == face)
			{
				return 1 << wiredFace | 1 << CellFace.OppositeFace(wiredFace);
			}
			return 0;
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x0001E240 File Offset: 0x0001C440
		public override int GetFaceTextureSlot(int face, int value)
		{
			int wiredFace = WireThroughBlock.GetWiredFace(Terrain.ExtractData(value));
			if (wiredFace == face || CellFace.OppositeFace(wiredFace) == face)
			{
				return this.m_wiredTextureSlot;
			}
			return this.m_unwiredTextureSlot;
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x0001E274 File Offset: 0x0001C474
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = float.NegativeInfinity;
			int wiredFace = 0;
			for (int i = 0; i < 6; i++)
			{
				float num2 = Vector3.Dot(CellFace.FaceToVector3(i), forward);
				if (num2 > num)
				{
					num = num2;
					wiredFace = i;
				}
			}
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, WireThroughBlock.SetWiredFace(0, wiredFace)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000580 RID: 1408 RVA: 0x0001E303 File Offset: 0x0001C503
		public static int GetWiredFace(int data)
		{
			if ((data & 3) == 0)
			{
				return 0;
			}
			if ((data & 3) == 1)
			{
				return 1;
			}
			return 4;
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x0001E315 File Offset: 0x0001C515
		public static int SetWiredFace(int data, int wiredFace)
		{
			data &= -4;
			switch (wiredFace)
			{
			case 0:
			case 2:
				return data;
			case 1:
			case 3:
				return data | 1;
			default:
				return data | 2;
			}
		}

		// Token: 0x04000266 RID: 614
		public int m_wiredTextureSlot;

		// Token: 0x04000267 RID: 615
		public int m_unwiredTextureSlot;
	}
}
