using System;
using Engine;

namespace Game
{
	// Token: 0x020002B7 RID: 695
	public class MultistateFurnitureElectricElement : FurnitureElectricElement
	{
		// Token: 0x060013DE RID: 5086 RVA: 0x00099F5C File Offset: 0x0009815C
		public MultistateFurnitureElectricElement(SubsystemElectricity subsystemElectricity, Point3 point) : base(subsystemElectricity, point)
		{
		}

		// Token: 0x060013DF RID: 5087 RVA: 0x00099F68 File Offset: 0x00098168
		public override bool Simulate()
		{
			if (base.CalculateHighInputsCount() > 0)
			{
				if (this.m_isActionAllowed)
				{
					if (this.m_lastActionTime != null)
					{
						double? num = base.SubsystemElectricity.SubsystemTime.GameTime - this.m_lastActionTime;
						double num2 = 0.1;
						if (!(num.GetValueOrDefault() > num2 & num != null))
						{
							return false;
						}
					}
					this.m_isActionAllowed = false;
					this.m_lastActionTime = new double?(base.SubsystemElectricity.SubsystemTime.GameTime);
					base.SubsystemElectricity.Project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true).SwitchToNextState(base.CellFaces[0].X, base.CellFaces[0].Y, base.CellFaces[0].Z, false);
				}
			}
			else
			{
				this.m_isActionAllowed = true;
			}
			return false;
		}

		// Token: 0x04000DB5 RID: 3509
		public bool m_isActionAllowed;

		// Token: 0x04000DB6 RID: 3510
		public double? m_lastActionTime;
	}
}
