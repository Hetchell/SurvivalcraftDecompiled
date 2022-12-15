using System;
using Engine;

namespace Game
{
	// Token: 0x020002D5 RID: 725
	public class PressurePlateElectricElement : MountedElectricElement
	{
		// Token: 0x0600147C RID: 5244 RVA: 0x0009F770 File Offset: 0x0009D970
		public PressurePlateElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x0009F77C File Offset: 0x0009D97C
		public void Press(float pressure)
		{
			this.m_lastPressFrameIndex = Time.FrameIndex;
			if (pressure > this.m_pressure)
			{
				this.m_pressure = pressure;
				CellFace cellFace = base.CellFaces[0];
				base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0.3f, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 2.5f, true);
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 1);
			}
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x0009F80C File Offset: 0x0009DA0C
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x0009F814 File Offset: 0x0009DA14
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			if (this.m_pressure > 0f && Time.FrameIndex - this.m_lastPressFrameIndex < 2)
			{
				this.m_voltage = PressurePlateElectricElement.PressureToVoltage(this.m_pressure);
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10);
			}
			else
			{
				if (ElectricElement.IsSignalHigh(this.m_voltage))
				{
					CellFace cellFace = base.CellFaces[0];
					base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/BlockPlaced", 0.6f, -0.1f, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 2.5f, true);
				}
				this.m_voltage = 0f;
				this.m_pressure = 0f;
			}
			return this.m_voltage != voltage;
		}

		// Token: 0x06001480 RID: 5248 RVA: 0x0009F8EF File Offset: 0x0009DAEF
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			this.Press(componentBody.Mass);
			componentBody.ApplyImpulse(new Vector3(0f, -2E-05f, 0f));
		}

		// Token: 0x06001481 RID: 5249 RVA: 0x0009F918 File Offset: 0x0009DB18
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			int num = Terrain.ExtractContents(worldItem.Value);
			Block block = BlocksManager.Blocks[num];
			this.Press(1f * block.Density);
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x0009F94C File Offset: 0x0009DB4C
		public static float PressureToVoltage(float pressure)
		{
			if (pressure <= 0f)
			{
				return 0f;
			}
			if (pressure < 1f)
			{
				return 0.53333336f;
			}
			if (pressure < 2f)
			{
				return 0.6f;
			}
			if (pressure < 5f)
			{
				return 0.6666667f;
			}
			if (pressure < 25f)
			{
				return 0.73333335f;
			}
			if (pressure < 100f)
			{
				return 0.8f;
			}
			if (pressure < 250f)
			{
				return 0.8666667f;
			}
			if (pressure < 500f)
			{
				return 0.93333334f;
			}
			return 1f;
		}

		// Token: 0x04000E8D RID: 3725
		public float m_voltage;

		// Token: 0x04000E8E RID: 3726
		public int m_lastPressFrameIndex;

		// Token: 0x04000E8F RID: 3727
		public float m_pressure;
	}
}
