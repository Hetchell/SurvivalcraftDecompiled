using System;
using Engine;

namespace Game
{
	// Token: 0x020002B3 RID: 691
	public class MotionDetectorElectricElement : MountedElectricElement
	{
		// Token: 0x060013D4 RID: 5076 RVA: 0x00099894 File Offset: 0x00097A94
		public MotionDetectorElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemBodies = subsystemElectricity.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_center = new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z) + new Vector3(0.5f) - 0.25f * this.m_direction;
			this.m_direction = CellFace.FaceToVector3(cellFace.Face);
			Vector3 vector = Vector3.One - new Vector3(MathUtils.Abs(this.m_direction.X), MathUtils.Abs(this.m_direction.Y), MathUtils.Abs(this.m_direction.Z));
			Vector3 vector2 = this.m_center - 8f * vector;
			Vector3 vector3 = this.m_center + 8f * (vector + this.m_direction);
			this.m_corner1 = new Vector2(vector2.X, vector2.Z);
			this.m_corner2 = new Vector2(vector3.X, vector3.Z);
		}

		// Token: 0x060013D5 RID: 5077 RVA: 0x000999C2 File Offset: 0x00097BC2
		public override float GetOutputVoltage(int face)
		{
			return this.m_voltage;
		}

		// Token: 0x060013D6 RID: 5078 RVA: 0x000999CC File Offset: 0x00097BCC
		public override bool Simulate()
		{
			float voltage = this.m_voltage;
			this.m_voltage = this.CalculateMotionVoltage();
			if (this.m_voltage > 0f && voltage == 0f)
			{
				base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/MotionDetectorClick", 1f, 0f, this.m_center, 1f, true);
			}
			float num = 0.5f * (0.9f + 0.00020000001f * (float)(this.GetHashCode() % 1000));
			base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + MathUtils.Max((int)(num / 0.01f), 1));
			return this.m_voltage != voltage;
		}

		// Token: 0x060013D7 RID: 5079 RVA: 0x00099A80 File Offset: 0x00097C80
		public float CalculateMotionVoltage()
		{
			float num = 0f;
			this.m_bodies.Clear();
			this.m_subsystemBodies.FindBodiesInArea(this.m_corner1, this.m_corner2, this.m_bodies);
			for (int i = 0; i < this.m_bodies.Count; i++)
			{
				ComponentBody componentBody = this.m_bodies.Array[i];
				if (componentBody.Velocity.LengthSquared() > 0.0625f)
				{
					Vector3 vector = componentBody.Position + new Vector3(0f, 0.5f * componentBody.BoxSize.Y, 0f);
					float num2 = Vector3.DistanceSquared(vector, this.m_center);
					if (num2 < 64f && Vector3.Dot(Vector3.Normalize(vector - (this.m_center - 0.75f * this.m_direction)), this.m_direction) > 0.5f)
					{
						if (base.SubsystemElectricity.SubsystemTerrain.Raycast(this.m_center, vector, false, true, delegate(int value, float d)
						{
							Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
							return block.IsCollidable && block.BlockIndex != 15 && block.BlockIndex != 60 && block.BlockIndex != 44 && block.BlockIndex != 18;
						}) == null)
						{
							num = MathUtils.Max(num, MathUtils.Saturate(1f - MathUtils.Sqrt(num2) / 8f));
						}
					}
				}
			}
			if (num <= 0f)
			{
				return 0f;
			}
			return MathUtils.Lerp(0.51f, 1f, MathUtils.Saturate(num * 1.1f));
		}

		// Token: 0x04000DA5 RID: 3493
		public const float m_range = 8f;

		// Token: 0x04000DA6 RID: 3494
		public const float m_speedThreshold = 0.25f;

		// Token: 0x04000DA7 RID: 3495
		public const float m_pollingPeriod = 0.5f;

		// Token: 0x04000DA8 RID: 3496
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000DA9 RID: 3497
		public float m_voltage;

		// Token: 0x04000DAA RID: 3498
		public Vector3 m_center;

		// Token: 0x04000DAB RID: 3499
		public Vector3 m_direction;

		// Token: 0x04000DAC RID: 3500
		public Vector2 m_corner1;

		// Token: 0x04000DAD RID: 3501
		public Vector2 m_corner2;

		// Token: 0x04000DAE RID: 3502
		public DynamicArray<ComponentBody> m_bodies = new DynamicArray<ComponentBody>();
	}
}
