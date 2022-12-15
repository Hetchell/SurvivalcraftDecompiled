using System;
using Engine;

namespace Game
{
	// Token: 0x020002EF RID: 751
	public class SevenSegmentDisplayElectricElement : MountedElectricElement
	{
		// Token: 0x06001566 RID: 5478 RVA: 0x000A2F74 File Offset: 0x000A1174
		public SevenSegmentDisplayElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemGlow = subsystemElectricity.Project.FindSubsystem<SubsystemGlow>(true);
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x000A3118 File Offset: 0x000A1318
		public override void OnAdded()
		{
			CellFace cellFace = base.CellFaces[0];
			int data = Terrain.ExtractData(base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z));
			int mountingFace = SevenSegmentDisplayBlock.GetMountingFace(data);
			this.m_color = LedBlock.LedColors[SevenSegmentDisplayBlock.GetColor(data)];
			for (int i = 0; i < 7; i++)
			{
				Vector3 v = new Vector3((float)cellFace.X + 0.5f, (float)cellFace.Y + 0.5f, (float)cellFace.Z + 0.5f);
				Vector3 vector = CellFace.FaceToVector3(mountingFace);
				Vector3 vector2 = (mountingFace < 4) ? Vector3.UnitY : Vector3.UnitX;
				Vector3 v2 = Vector3.Cross(vector, vector2);
				this.m_glowPoints[i] = this.m_subsystemGlow.AddGlowPoint();
				this.m_glowPoints[i].Position = v - 0.4375f * CellFace.FaceToVector3(mountingFace) + this.m_centers[i].X * 0.0625f * v2 + this.m_centers[i].Y * 0.0625f * vector2;
				this.m_glowPoints[i].Forward = vector;
				this.m_glowPoints[i].Right = v2 * this.m_sizes[i].X * 0.0625f;
				this.m_glowPoints[i].Up = vector2 * this.m_sizes[i].Y * 0.0625f;
				this.m_glowPoints[i].Color = Color.Transparent;
				this.m_glowPoints[i].Size = 1.35f;
				this.m_glowPoints[i].FarSize = 1.35f;
				this.m_glowPoints[i].FarDistance = 1f;
				this.m_glowPoints[i].Type = ((this.m_sizes[i].X > this.m_sizes[i].Y) ? GlowPointType.HorizontalRectangle : GlowPointType.VerticalRectangle);
			}
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x000A3360 File Offset: 0x000A1560
		public override void OnRemoved()
		{
			for (int i = 0; i < 7; i++)
			{
				this.m_subsystemGlow.RemoveGlowPoint(this.m_glowPoints[i]);
			}
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x000A338C File Offset: 0x000A158C
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
				for (int i = 0; i < 7; i++)
				{
					if ((this.m_patterns[num] & 1 << i) != 0)
					{
						this.m_glowPoints[i].Color = this.m_color;
					}
					else
					{
						this.m_glowPoints[i].Color = Color.Transparent;
					}
				}
			}
			return false;
		}

		// Token: 0x04000F2A RID: 3882
		public SubsystemGlow m_subsystemGlow;

		// Token: 0x04000F2B RID: 3883
		public float m_voltage = float.PositiveInfinity;

		// Token: 0x04000F2C RID: 3884
		public GlowPoint[] m_glowPoints = new GlowPoint[7];

		// Token: 0x04000F2D RID: 3885
		public Color m_color;

		// Token: 0x04000F2E RID: 3886
		public Vector2[] m_centers = new Vector2[]
		{
			new Vector2(0f, 6f),
			new Vector2(-4f, 3f),
			new Vector2(-4f, -3f),
			new Vector2(0f, -6f),
			new Vector2(4f, -3f),
			new Vector2(4f, 3f),
			new Vector2(0f, 0f)
		};

		// Token: 0x04000F2F RID: 3887
		public Vector2[] m_sizes = new Vector2[]
		{
			new Vector2(3.2f, 1f),
			new Vector2(1f, 2.3f),
			new Vector2(1f, 2.3f),
			new Vector2(3.2f, 1f),
			new Vector2(1f, 2.3f),
			new Vector2(1f, 2.3f),
			new Vector2(3.2f, 1f)
		};

		// Token: 0x04000F30 RID: 3888
		public int[] m_patterns = new int[]
		{
			63,
			6,
			91,
			79,
			102,
			109,
			125,
			7,
			127,
			111,
			119,
			124,
			57,
			94,
			121,
			113
		};
	}
}
