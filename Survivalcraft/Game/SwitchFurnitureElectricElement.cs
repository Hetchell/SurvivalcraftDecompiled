using System;
using Engine;

namespace Game
{
	// Token: 0x02000308 RID: 776
	public class SwitchFurnitureElectricElement : FurnitureElectricElement
	{
		// Token: 0x060015D5 RID: 5589 RVA: 0x000A63FC File Offset: 0x000A45FC
		public SwitchFurnitureElectricElement(SubsystemElectricity subsystemElectricity, Point3 point, int value) : base(subsystemElectricity, point)
		{
			FurnitureDesign design = FurnitureBlock.GetDesign(subsystemElectricity.SubsystemTerrain.SubsystemFurnitureBlockBehavior, value);
			if (design != null && design.LinkedDesign != null)
			{
				this.m_voltage = (float)((design.Index >= design.LinkedDesign.Index) ? 1 : 0);
			}
		}

		// Token: 0x060015D6 RID: 5590 RVA: 0x000A644C File Offset: 0x000A464C
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060015D7 RID: 5591 RVA: 0x000A6454 File Offset: 0x000A4654
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			CellFace cellFace = base.CellFaces[0];
			base.SubsystemElectricity.SubsystemTerrain.SubsystemFurnitureBlockBehavior.SwitchToNextState(cellFace.X, cellFace.Y, cellFace.Z, false);
			base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Click", 1f, 0f, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 2f, true);
			return true;
		}

		// Token: 0x04000F83 RID: 3971
		public float m_voltage;
	}
}
