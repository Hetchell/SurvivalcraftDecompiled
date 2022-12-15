using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x020002C9 RID: 713
	public class PistonElectricElement : ElectricElement
	{
		// Token: 0x06001424 RID: 5156 RVA: 0x0009BEE0 File Offset: 0x0009A0E0
		public PistonElectricElement(SubsystemElectricity subsystemElectricity, Point3 point) : base(subsystemElectricity, new List<CellFace>
		{
			new CellFace(point.X, point.Y, point.Z, 0),
			new CellFace(point.X, point.Y, point.Z, 1),
			new CellFace(point.X, point.Y, point.Z, 2),
			new CellFace(point.X, point.Y, point.Z, 3),
			new CellFace(point.X, point.Y, point.Z, 4),
			new CellFace(point.X, point.Y, point.Z, 5)
		})
		{
		}

		// Token: 0x06001425 RID: 5157 RVA: 0x0009BFB4 File Offset: 0x0009A1B4
		public override bool Simulate()
		{
			float num = 0f;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					num = MathUtils.Max(num, electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
				}
			}
			int num2 = MathUtils.Max((int)(num * 15.999f) - 7, 0);
			if (num2 != this.m_lastLength)
			{
				this.m_lastLength = num2;
				base.SubsystemElectricity.Project.FindSubsystem<SubsystemPistonBlockBehavior>(true).AdjustPiston(base.CellFaces[0].Point, num2);
			}
			return false;
		}

		// Token: 0x04000E02 RID: 3586
		public int m_lastLength = -1;
	}
}
