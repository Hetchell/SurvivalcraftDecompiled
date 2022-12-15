using System;

namespace Game
{
	// Token: 0x020000B5 RID: 181
	public abstract class MountedElectricElementBlock : Block, IElectricElementBlock
	{
		// Token: 0x06000363 RID: 867
		public abstract int GetFace(int value);

		// Token: 0x06000364 RID: 868
		public abstract ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z);

		// Token: 0x06000365 RID: 869
		public abstract ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z);

		// Token: 0x06000366 RID: 870 RVA: 0x00013377 File Offset: 0x00011577
		public virtual int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}
	}
}
