using System;

namespace Game
{
	// Token: 0x020000D0 RID: 208
	public class RandomGeneratorBlock : RotateableMountedElectricElementBlock
	{
		// Token: 0x06000429 RID: 1065 RVA: 0x00016923 File Offset: 0x00014B23
		public RandomGeneratorBlock() : base("Models/Gates", "RandomGenerator", 0.375f)
		{
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x0001693A File Offset: 0x00014B3A
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new RandomGeneratorElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)));
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x00016954 File Offset: 0x00014B54
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

		// Token: 0x040001D6 RID: 470
		public const int Index = 157;
	}
}
