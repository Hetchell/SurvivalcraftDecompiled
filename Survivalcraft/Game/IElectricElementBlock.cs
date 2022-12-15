using System;

namespace Game
{
	// Token: 0x02000080 RID: 128
	public interface IElectricElementBlock
	{
		// Token: 0x060002BB RID: 699
		ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z);

		// Token: 0x060002BC RID: 700
		ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z);

		// Token: 0x060002BD RID: 701
		int GetConnectionMask(int value);
	}
}
