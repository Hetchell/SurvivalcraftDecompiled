using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000253 RID: 595
	public class DispenserElectricElement : ElectricElement
	{
		// Token: 0x060011F6 RID: 4598 RVA: 0x0008A6FC File Offset: 0x000888FC
		public DispenserElectricElement(SubsystemElectricity subsystemElectricity, Point3 point) : base(subsystemElectricity, new List<CellFace>
		{
			new CellFace(point.X, point.Y, point.Z, 0),
			new CellFace(point.X, point.Y, point.Z, 1),
			new CellFace(point.X, point.Y, point.Z, 2),
			new CellFace(point.X, point.Y, point.Z, 3),
			new CellFace(point.X, point.Y, point.Z, 4),
			new CellFace(point.X, point.Y, point.Z, 5)
		})
		{
			this.m_subsystemBlockEntities = base.SubsystemElectricity.Project.FindSubsystem<SubsystemBlockEntities>(true);
		}

		// Token: 0x060011F7 RID: 4599 RVA: 0x0008A7E8 File Offset: 0x000889E8
		public override bool Simulate()
		{
			if (base.CalculateHighInputsCount() > 0)
			{
				if (this.m_isDispenseAllowed)
				{
					if (this.m_lastDispenseTime != null)
					{
						double? num = base.SubsystemElectricity.SubsystemTime.GameTime - this.m_lastDispenseTime;
						double num2 = 0.1;
						if (!(num.GetValueOrDefault() > num2 & num != null))
						{
							return false;
						}
					}
					this.m_isDispenseAllowed = false;
					this.m_lastDispenseTime = new double?(base.SubsystemElectricity.SubsystemTime.GameTime);
					ComponentBlockEntity blockEntity = this.m_subsystemBlockEntities.GetBlockEntity(base.CellFaces[0].Point.X, base.CellFaces[0].Point.Y, base.CellFaces[0].Point.Z);
					if (blockEntity != null)
					{
						ComponentDispenser componentDispenser = blockEntity.Entity.FindComponent<ComponentDispenser>();
						if (componentDispenser != null)
						{
							componentDispenser.Dispense();
						}
					}
				}
			}
			else
			{
				this.m_isDispenseAllowed = true;
			}
			return false;
		}

		// Token: 0x04000C08 RID: 3080
		public bool m_isDispenseAllowed = true;

		// Token: 0x04000C09 RID: 3081
		public double? m_lastDispenseTime;

		// Token: 0x04000C0A RID: 3082
		public SubsystemBlockEntities m_subsystemBlockEntities;
	}
}
