using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001EB RID: 491
	public class ComponentIntroShip : Component, IUpdateable
	{
		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000DE0 RID: 3552 RVA: 0x0006BD80 File Offset: 0x00069F80
		// (set) Token: 0x06000DE1 RID: 3553 RVA: 0x0006BD88 File Offset: 0x00069F88
		public float Heading { get; set; }

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06000DE2 RID: 3554 RVA: 0x0006BD91 File Offset: 0x00069F91
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000DE3 RID: 3555 RVA: 0x0006BD94 File Offset: 0x00069F94
		public void Update(float dt)
		{
			float s = 3.5f * MathUtils.Saturate(0.07f * ((float)this.m_subsystemGameInfo.TotalElapsedGameTime - 6f));
			Matrix matrix = this.m_componentFrame.Matrix;
			Vector3 vector = Quaternion.CreateFromRotationMatrix(matrix).ToYawPitchRoll();
			vector.X = this.Heading;
			vector.Y = 0.05f * MathUtils.Sin((float)MathUtils.NormalizeAngle(0.77 * this.m_subsystemTime.GameTime + 1.0));
			vector.Z = 0.12f * MathUtils.Sin((float)MathUtils.NormalizeAngle(1.12 * this.m_subsystemTime.GameTime + 2.0));
			matrix = Matrix.CreateFromYawPitchRoll(vector.X, vector.Y, vector.Z) * Matrix.CreateTranslation(matrix.Translation);
			matrix.Translation += s * matrix.Forward * new Vector3(1f, 0f, 1f) * dt;
			this.m_componentFrame.Position = matrix.Translation;
			this.m_componentFrame.Rotation = Quaternion.CreateFromRotationMatrix(matrix);
			this.m_componentModel.SetBoneTransform(this.m_componentModel.Model.RootBone.Index, new Matrix?(matrix));
			if (this.m_subsystemTime.GameTime - this.m_creationTime > 10.0 && this.m_subsystemViews.CalculateDistanceFromNearestView(matrix.Translation) > this.m_subsystemSky.VisibilityRange + 30f)
			{
				base.Project.RemoveEntity(base.Entity, true);
			}
		}

        // Token: 0x06000DE4 RID: 3556 RVA: 0x0006BF60 File Offset: 0x0006A160
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemViews = base.Project.FindSubsystem<SubsystemGameWidgets>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_componentFrame = base.Entity.FindComponent<ComponentFrame>(true);
			this.m_componentModel = base.Entity.FindComponent<ComponentModel>(true);
			this.m_creationTime = this.m_subsystemTime.GameTime;
			this.Heading = valuesDictionary.GetValue<float>("Heading");
		}

        // Token: 0x06000DE5 RID: 3557 RVA: 0x0006BFFB File Offset: 0x0006A1FB
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("Heading", this.Heading);
		}

		// Token: 0x040008C3 RID: 2243
		public SubsystemTime m_subsystemTime;

		// Token: 0x040008C4 RID: 2244
		public SubsystemGameWidgets m_subsystemViews;

		// Token: 0x040008C5 RID: 2245
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040008C6 RID: 2246
		public SubsystemSky m_subsystemSky;

		// Token: 0x040008C7 RID: 2247
		public ComponentFrame m_componentFrame;

		// Token: 0x040008C8 RID: 2248
		public ComponentModel m_componentModel;

		// Token: 0x040008C9 RID: 2249
		public double m_creationTime;
	}
}
