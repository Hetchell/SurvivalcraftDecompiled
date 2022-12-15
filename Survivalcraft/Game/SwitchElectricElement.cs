using System;
using Engine;

namespace Game
{
	// Token: 0x02000307 RID: 775
	public class SwitchElectricElement : MountedElectricElement
	{
		// Token: 0x060015D2 RID: 5586 RVA: 0x000A631E File Offset: 0x000A451E
		public SwitchElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace, int value) : base(subsystemElectricity, cellFace)
		{
			this.m_voltage = (float)(SwitchBlock.GetLeverState(value) ? 1 : 0);
		}

		// Token: 0x060015D3 RID: 5587 RVA: 0x000A633B File Offset: 0x000A453B
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x000A6344 File Offset: 0x000A4544
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			CellFace cellFace = base.CellFaces[0];
			int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			int value = SwitchBlock.SetLeverState(cellValue, !SwitchBlock.GetLeverState(cellValue));
			base.SubsystemElectricity.SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y, cellFace.Z, value, true);
			base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Click", 1f, 0f, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 2f, true);
			return true;
		}

		// Token: 0x04000F82 RID: 3970
		public float m_voltage;
	}
}
