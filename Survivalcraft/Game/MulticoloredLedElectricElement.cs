using System;
using Engine;

namespace Game
{
	// Token: 0x020002B6 RID: 694
	public class MulticoloredLedElectricElement : MountedElectricElement
	{
		// Token: 0x060013DA RID: 5082 RVA: 0x00099CFA File Offset: 0x00097EFA
		public MulticoloredLedElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemGlow = subsystemElectricity.Project.FindSubsystem<SubsystemGlow>(true);
		}

		// Token: 0x060013DB RID: 5083 RVA: 0x00099D18 File Offset: 0x00097F18
		public override void OnAdded()
		{
			this.m_glowPoint = this.m_subsystemGlow.AddGlowPoint();
			CellFace cellFace = base.CellFaces[0];
			int mountingFace = MulticoloredLedBlock.GetMountingFace(Terrain.ExtractData(base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z)));
			Vector3 v = new Vector3((float)cellFace.X + 0.5f, (float)cellFace.Y + 0.5f, (float)cellFace.Z + 0.5f);
			this.m_glowPoint.Position = v - 0.4375f * CellFace.FaceToVector3(mountingFace);
			this.m_glowPoint.Forward = CellFace.FaceToVector3(mountingFace);
			this.m_glowPoint.Up = ((mountingFace < 4) ? Vector3.UnitY : Vector3.UnitX);
			this.m_glowPoint.Right = Vector3.Cross(this.m_glowPoint.Forward, this.m_glowPoint.Up);
			this.m_glowPoint.Color = Color.Transparent;
			this.m_glowPoint.Size = 0.0324f;
			this.m_glowPoint.FarSize = 0.0324f;
			this.m_glowPoint.FarDistance = 0f;
			this.m_glowPoint.Type = GlowPointType.Square;
		}

		// Token: 0x060013DC RID: 5084 RVA: 0x00099E65 File Offset: 0x00098065
		public override void OnRemoved()
		{
			this.m_subsystemGlow.RemoveGlowPoint(this.m_glowPoint);
		}

		// Token: 0x060013DD RID: 5085 RVA: 0x00099E78 File Offset: 0x00098078
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			this.m_voltage = 0f;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					this.m_voltage = MathUtils.Max(this.m_voltage, electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
				}
			}
			if (this.m_voltage != voltage)
			{
				int num = (int)MathUtils.Round(this.m_voltage * 15f);
				if (num >= 8)
				{
					this.m_glowPoint.Color = LedBlock.LedColors[MathUtils.Clamp(num - 8, 0, 7)];
				}
				else
				{
					this.m_glowPoint.Color = Color.Transparent;
				}
			}
			return false;
		}

		// Token: 0x04000DB2 RID: 3506
		public SubsystemGlow m_subsystemGlow;

		// Token: 0x04000DB3 RID: 3507
		public float m_voltage;

		// Token: 0x04000DB4 RID: 3508
		public GlowPoint m_glowPoint;
	}
}
