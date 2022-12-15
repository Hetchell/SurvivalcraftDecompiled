using System;
using Engine;

namespace Game
{
	// Token: 0x020002C7 RID: 711
	public class PhotodiodeElectricElement : MountedElectricElement
	{
		// Token: 0x0600141F RID: 5151 RVA: 0x0009BDC9 File Offset: 0x00099FC9
		public PhotodiodeElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_voltage = this.CalculateVoltage();
		}

		// Token: 0x06001420 RID: 5152 RVA: 0x0009BDDF File Offset: 0x00099FDF
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x06001421 RID: 5153 RVA: 0x0009BDE8 File Offset: 0x00099FE8
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			this.m_voltage = this.CalculateVoltage();
			base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + MathUtils.Max(50, 1));
			return this.m_voltage != voltage;
		}

		// Token: 0x06001422 RID: 5154 RVA: 0x0009BE34 File Offset: 0x0009A034
		public float CalculateVoltage()
		{
			CellFace cellFace = base.CellFaces[0];
			Point3 point = CellFace.FaceToPoint3(cellFace.Face);
			int cellLight = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellLight(cellFace.X, cellFace.Y, cellFace.Z);
			int cellLight2 = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellLight(cellFace.X + point.X, cellFace.Y + point.Y, cellFace.Z + point.Z);
			return (float)MathUtils.Max(cellLight, cellLight2) / 15f;
		}

		// Token: 0x04000DFD RID: 3581
		public float m_voltage;
	}
}
