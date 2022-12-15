using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x0200000D RID: 13
	public class AdjustableDelayGateBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x06000085 RID: 133 RVA: 0x00005589 File Offset: 0x00003789
		public AdjustableDelayGateBlock() : base("Models/Gates", "AdjustableDelayGate", 0.375f)
		{
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000055A0 File Offset: 0x000037A0
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			if (toolLevel >= this.RequiredToolLevel)
			{
				int delay = AdjustableDelayGateBlock.GetDelay(Terrain.ExtractData(oldValue));
				int data = AdjustableDelayGateBlock.SetDelay(0, delay);
				dropValues.Add(new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(224, 0, data),
					Count = 1
				});
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000055FA File Offset: 0x000037FA
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new AdjustableDelayGateElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00005614 File Offset: 0x00003814
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			if (this.GetFace(value) == face)
			{
				ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(this.GetFace(value), RotateableMountedElectricElementBlock.GetRotation(data), connectorFace);
				ElectricConnectorDirection? electricConnectorDirection = connectorDirection;
				ElectricConnectorDirection electricConnectorDirection2 = ElectricConnectorDirection.Bottom;
				if (electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null)
				{
					return new ElectricConnectorType?(ElectricConnectorType.Input);
				}
				electricConnectorDirection = connectorDirection;
				electricConnectorDirection2 = ElectricConnectorDirection.Top;
				if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
				{
					electricConnectorDirection = connectorDirection;
					electricConnectorDirection2 = ElectricConnectorDirection.In;
					if (!(electricConnectorDirection.GetValueOrDefault() == electricConnectorDirection2 & electricConnectorDirection != null))
					{
						goto IL_7C;
					}
				}
				return new ElectricConnectorType?(ElectricConnectorType.Output);
			}
			IL_7C:
			return null;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000056A7 File Offset: 0x000038A7
		public static int GetDelay(int data)
		{
			return data >> 5 & 255;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000056B2 File Offset: 0x000038B2
		public static int SetDelay(int data, int delay)
		{
			return (data & -8161) | (delay & 255) << 5;
		}

		// Token: 0x0400004B RID: 75
		public const int Index = 224;
	}
}
