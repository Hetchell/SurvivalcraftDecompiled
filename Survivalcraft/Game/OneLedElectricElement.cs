using System;
using Engine;

namespace Game
{
	// Token: 0x020002BC RID: 700
	public class OneLedElectricElement : MountedElectricElement
	{
		// Token: 0x060013F1 RID: 5105 RVA: 0x0009A5A8 File Offset: 0x000987A8
		public OneLedElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemGlow = subsystemElectricity.Project.FindSubsystem<SubsystemGlow>(true);
		}

		// Token: 0x060013F2 RID: 5106 RVA: 0x0009A5C4 File Offset: 0x000987C4
		public override void OnAdded()
		{
			CellFace cellFace = base.CellFaces[0];
			int data = Terrain.ExtractData(base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z));
			int mountingFace = FourLedBlock.GetMountingFace(data);
			this.m_color = LedBlock.LedColors[FourLedBlock.GetColor(data)];
			Vector3 v = new Vector3((float)cellFace.X + 0.5f, (float)cellFace.Y + 0.5f, (float)cellFace.Z + 0.5f);
			Vector3 vector = CellFace.FaceToVector3(mountingFace);
			Vector3 vector2 = (mountingFace < 4) ? Vector3.UnitY : Vector3.UnitX;
			Vector3 right = Vector3.Cross(vector, vector2);
			this.m_glowPoint = this.m_subsystemGlow.AddGlowPoint();
			this.m_glowPoint.Position = v - 0.4375f * CellFace.FaceToVector3(mountingFace);
			this.m_glowPoint.Forward = vector;
			this.m_glowPoint.Up = vector2;
			this.m_glowPoint.Right = right;
			this.m_glowPoint.Color = Color.Transparent;
			this.m_glowPoint.Size = 0.52f;
			this.m_glowPoint.FarSize = 0.52f;
			this.m_glowPoint.FarDistance = 1f;
			this.m_glowPoint.Type = GlowPointType.Square;
		}

		// Token: 0x060013F3 RID: 5107 RVA: 0x0009A724 File Offset: 0x00098924
		public override void OnRemoved()
		{
			this.m_subsystemGlow.RemoveGlowPoint(this.m_glowPoint);
		}

		// Token: 0x060013F4 RID: 5108 RVA: 0x0009A738 File Offset: 0x00098938
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

		// Token: 0x04000DC2 RID: 3522
		public SubsystemGlow m_subsystemGlow;

		// Token: 0x04000DC3 RID: 3523
		public float m_voltage;

		// Token: 0x04000DC4 RID: 3524
		public Color m_color;

		// Token: 0x04000DC5 RID: 3525
		public GlowPoint m_glowPoint;
	}
}
