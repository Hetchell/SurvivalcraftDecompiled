using System;
using Engine;

namespace Game
{
	// Token: 0x020002A1 RID: 673
	public class LedElectricElement : MountedElectricElement
	{
		// Token: 0x06001381 RID: 4993 RVA: 0x000974B3 File Offset: 0x000956B3
		public LedElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemGlow = subsystemElectricity.Project.FindSubsystem<SubsystemGlow>(true);
		}

		// Token: 0x06001382 RID: 4994 RVA: 0x000974D0 File Offset: 0x000956D0
		public override void OnAdded()
		{
			this.m_glowPoint = this.m_subsystemGlow.AddGlowPoint();
			CellFace cellFace = base.CellFaces[0];
			int data = Terrain.ExtractData(base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z));
			int mountingFace = LedBlock.GetMountingFace(data);
			this.m_color = LedBlock.LedColors[LedBlock.GetColor(data)];
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

		// Token: 0x06001383 RID: 4995 RVA: 0x00097636 File Offset: 0x00095836
		public override void OnRemoved()
		{
			this.m_subsystemGlow.RemoveGlowPoint(this.m_glowPoint);
		}

		// Token: 0x06001384 RID: 4996 RVA: 0x0009764C File Offset: 0x0009584C
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			this.m_voltage = this.CalculateVoltage();
			if (ElectricElement.IsSignalHigh(this.m_voltage) != ElectricElement.IsSignalHigh(voltage))
			{
				if (ElectricElement.IsSignalHigh(this.m_voltage))
				{
					this.m_glowPoint.Color = this.m_color;
				}
				else
				{
					this.m_glowPoint.Color = Color.Transparent;
				}
			}
			return false;
		}

		// Token: 0x06001385 RID: 4997 RVA: 0x000976B0 File Offset: 0x000958B0
		public float CalculateVoltage()
		{
			return (float)((base.CalculateHighInputsCount() > 0) ? 1 : 0);
		}

		// Token: 0x04000D54 RID: 3412
		public SubsystemGlow m_subsystemGlow;

		// Token: 0x04000D55 RID: 3413
		public float m_voltage;

		// Token: 0x04000D56 RID: 3414
		public GlowPoint m_glowPoint;

		// Token: 0x04000D57 RID: 3415
		public Color m_color;
	}
}
