using System;
using Engine;

namespace Game
{
	// Token: 0x02000234 RID: 564
	public class ButtonFurnitureElectricElement : FurnitureElectricElement
	{
		// Token: 0x0600114B RID: 4427 RVA: 0x000871DA File Offset: 0x000853DA
		public ButtonFurnitureElectricElement(SubsystemElectricity subsystemElectricity, Point3 point) : base(subsystemElectricity, point)
		{
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x000871E4 File Offset: 0x000853E4
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

		// Token: 0x0600114D RID: 4429 RVA: 0x00087275 File Offset: 0x00085475
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x0600114E RID: 4430 RVA: 0x00087280 File Offset: 0x00085480
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

		// Token: 0x0600114F RID: 4431 RVA: 0x000872E1 File Offset: 0x000854E1
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			this.Press();
			return true;
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x000872EA File Offset: 0x000854EA
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			this.Press();
		}

		// Token: 0x04000B9A RID: 2970
		public float m_voltage;

		// Token: 0x04000B9B RID: 2971
		public bool m_wasPressed;
	}
}
