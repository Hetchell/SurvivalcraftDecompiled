using System;
using Engine;

namespace Game
{
	// Token: 0x02000233 RID: 563
	public class ButtonElectricElement : MountedElectricElement
	{
		// Token: 0x06001145 RID: 4421 RVA: 0x000870C0 File Offset: 0x000852C0
		public ButtonElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x06001146 RID: 4422 RVA: 0x000870CC File Offset: 0x000852CC
		public void Press()
		{
			if (!this.m_wasPressed && !ElectricElement.IsSignalHigh(this.m_voltage))
			{
				this.m_wasPressed = true;
				CellFace cellFace = base.CellFaces[0];
				base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Click", 1f, 0f, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 2f, true);
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 1);
			}
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x0008715D File Offset: 0x0008535D
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x00087168 File Offset: 0x00085368
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			if (this.m_wasPressed)
			{
				this.m_wasPressed = false;
				this.m_voltage = 1f;
				base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10);
			}
			else
			{
				this.m_voltage = 0f;
			}
			return this.m_voltage != voltage;
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x000871C9 File Offset: 0x000853C9
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			this.Press();
			return true;
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x000871D2 File Offset: 0x000853D2
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			this.Press();
		}

		// Token: 0x04000B98 RID: 2968
		public float m_voltage;

		// Token: 0x04000B99 RID: 2969
		public bool m_wasPressed;
	}
}
