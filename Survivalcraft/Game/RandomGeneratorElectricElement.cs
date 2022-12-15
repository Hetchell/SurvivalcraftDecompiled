using System;
using Engine;

namespace Game
{
	// Token: 0x020002E0 RID: 736
	public class RandomGeneratorElectricElement : RotateableElectricElement
	{
		// Token: 0x060014CC RID: 5324 RVA: 0x000A12A0 File Offset: 0x0009F4A0
		public RandomGeneratorElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			float? num = base.SubsystemElectricity.ReadPersistentVoltage(base.CellFaces[0].Point);
			if (num != null)
			{
				this.m_voltage = num.Value;
				return;
			}
			this.m_voltage = RandomGeneratorElectricElement.GetRandomVoltage();
		}

		// Token: 0x060014CD RID: 5325 RVA: 0x000A1301 File Offset: 0x0009F501
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060014CE RID: 5326 RVA: 0x000A130C File Offset: 0x0009F50C
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			bool flag = false;
			bool flag2 = false;
			int rotation = base.Rotation;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					if (ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace)))
					{
						if (this.m_clockAllowed)
						{
							flag = true;
							this.m_clockAllowed = false;
						}
					}
					else
					{
						this.m_clockAllowed = true;
					}
					flag2 = true;
				}
			}
			if (flag2)
			{
				if (flag)
				{
					this.m_voltage = RandomGeneratorElectricElement.GetRandomVoltage();
				}
			}
			else
			{
				this.m_voltage = RandomGeneratorElectricElement.GetRandomVoltage();
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + MathUtils.Max((int)(RandomGeneratorElectricElement.s_random.Float(0.25f, 0.75f) / 0.01f), 1));
			}
			if (this.m_voltage != voltage)
			{
				base.SubsystemElectricity.WritePersistentVoltage(base.CellFaces[0].Point, this.m_voltage);
				return true;
			}
			return false;
		}

		// Token: 0x060014CF RID: 5327 RVA: 0x000A1444 File Offset: 0x0009F644
		public static float GetRandomVoltage()
		{
			return (float)RandomGeneratorElectricElement.s_random.Int(0, 15) / 15f;
		}

		// Token: 0x04000EC4 RID: 3780
		public bool m_clockAllowed = true;

		// Token: 0x04000EC5 RID: 3781
		public float m_voltage;

		// Token: 0x04000EC6 RID: 3782
		public static Game.Random s_random = new Game.Random();
	}
}
