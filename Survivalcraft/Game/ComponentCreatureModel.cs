using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001CF RID: 463
	public class ComponentCreatureModel : ComponentModel, IUpdateable
	{
		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000C4A RID: 3146 RVA: 0x0005CC07 File Offset: 0x0005AE07
		// (set) Token: 0x06000C4B RID: 3147 RVA: 0x0005CC0F File Offset: 0x0005AE0F
		public float Bob { get; set; }

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000C4C RID: 3148 RVA: 0x0005CC18 File Offset: 0x0005AE18
		// (set) Token: 0x06000C4D RID: 3149 RVA: 0x0005CC20 File Offset: 0x0005AE20
		public float MovementAnimationPhase { get; set; }

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06000C4E RID: 3150 RVA: 0x0005CC29 File Offset: 0x0005AE29
		// (set) Token: 0x06000C4F RID: 3151 RVA: 0x0005CC31 File Offset: 0x0005AE31
		public float DeathPhase { get; set; }

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06000C50 RID: 3152 RVA: 0x0005CC3A File Offset: 0x0005AE3A
		// (set) Token: 0x06000C51 RID: 3153 RVA: 0x0005CC42 File Offset: 0x0005AE42
		public Vector3 DeathCauseOffset { get; set; }

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000C52 RID: 3154 RVA: 0x0005CC4B File Offset: 0x0005AE4B
		// (set) Token: 0x06000C53 RID: 3155 RVA: 0x0005CC53 File Offset: 0x0005AE53
		public Vector3? LookAtOrder { get; set; }

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x06000C54 RID: 3156 RVA: 0x0005CC5C File Offset: 0x0005AE5C
		// (set) Token: 0x06000C55 RID: 3157 RVA: 0x0005CC64 File Offset: 0x0005AE64
		public bool LookRandomOrder { get; set; }

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06000C56 RID: 3158 RVA: 0x0005CC6D File Offset: 0x0005AE6D
		// (set) Token: 0x06000C57 RID: 3159 RVA: 0x0005CC75 File Offset: 0x0005AE75
		public float HeadShakeOrder { get; set; }

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000C58 RID: 3160 RVA: 0x0005CC7E File Offset: 0x0005AE7E
		// (set) Token: 0x06000C59 RID: 3161 RVA: 0x0005CC86 File Offset: 0x0005AE86
		public bool AttackOrder { get; set; }

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000C5A RID: 3162 RVA: 0x0005CC8F File Offset: 0x0005AE8F
		// (set) Token: 0x06000C5B RID: 3163 RVA: 0x0005CC97 File Offset: 0x0005AE97
		public bool FeedOrder { get; set; }

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000C5C RID: 3164 RVA: 0x0005CCA0 File Offset: 0x0005AEA0
		// (set) Token: 0x06000C5D RID: 3165 RVA: 0x0005CCA8 File Offset: 0x0005AEA8
		public bool RowLeftOrder { get; set; }

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000C5E RID: 3166 RVA: 0x0005CCB1 File Offset: 0x0005AEB1
		// (set) Token: 0x06000C5F RID: 3167 RVA: 0x0005CCB9 File Offset: 0x0005AEB9
		public bool RowRightOrder { get; set; }

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000C60 RID: 3168 RVA: 0x0005CCC2 File Offset: 0x0005AEC2
		// (set) Token: 0x06000C61 RID: 3169 RVA: 0x0005CCCA File Offset: 0x0005AECA
		public float AimHandAngleOrder { get; set; }

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000C62 RID: 3170 RVA: 0x0005CCD3 File Offset: 0x0005AED3
		// (set) Token: 0x06000C63 RID: 3171 RVA: 0x0005CCDB File Offset: 0x0005AEDB
		public Vector3 InHandItemOffsetOrder { get; set; }

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000C64 RID: 3172 RVA: 0x0005CCE4 File Offset: 0x0005AEE4
		// (set) Token: 0x06000C65 RID: 3173 RVA: 0x0005CCEC File Offset: 0x0005AEEC
		public Vector3 InHandItemRotationOrder { get; set; }

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000C66 RID: 3174 RVA: 0x0005CCF5 File Offset: 0x0005AEF5
		// (set) Token: 0x06000C67 RID: 3175 RVA: 0x0005CCFD File Offset: 0x0005AEFD
		public bool IsAttackHitMoment { get; set; }

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000C68 RID: 3176 RVA: 0x0005CD06 File Offset: 0x0005AF06
		public Vector3 EyePosition
		{
			get
			{
				if (this.m_eyePosition == null)
				{
					this.m_eyePosition = new Vector3?(this.CalculateEyePosition());
				}
				return this.m_eyePosition.Value;
			}
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000C69 RID: 3177 RVA: 0x0005CD31 File Offset: 0x0005AF31
		public Quaternion EyeRotation
		{
			get
			{
				if (this.m_eyeRotation == null)
				{
					this.m_eyeRotation = new Quaternion?(this.CalculateEyeRotation());
				}
				return this.m_eyeRotation.Value;
			}
		}

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000C6A RID: 3178 RVA: 0x0005CD5C File Offset: 0x0005AF5C
		public UpdateOrder UpdateOrder
		{
			get
			{
				ComponentBody parentBody = this.m_componentCreature.ComponentBody.ParentBody;
				if (parentBody != null)
				{
					ComponentCreatureModel componentCreatureModel = parentBody.Entity.FindComponent<ComponentCreatureModel>();
					if (componentCreatureModel != null)
					{
						return componentCreatureModel.UpdateOrder + 1;
					}
				}
				return UpdateOrder.CreatureModels;
			}
		}

		// Token: 0x06000C6B RID: 3179 RVA: 0x0005CD98 File Offset: 0x0005AF98
		public override void Animate()
		{
			base.Opacity = new float?((this.m_componentCreature.ComponentSpawn.SpawnDuration > 0f) ? ((float)MathUtils.Saturate((this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_componentCreature.ComponentSpawn.SpawnTime) / (double)this.m_componentCreature.ComponentSpawn.SpawnDuration)) : 1f);
			if (this.m_componentCreature.ComponentSpawn.DespawnTime != null)
			{
				base.Opacity = new float?(MathUtils.Min(base.Opacity.Value, (float)MathUtils.Saturate(1.0 - (this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_componentCreature.ComponentSpawn.DespawnTime.Value) / (double)this.m_componentCreature.ComponentSpawn.DespawnDuration)));
			}
			base.DiffuseColor = new Vector3?(Vector3.Lerp(Vector3.One, new Vector3(1f, 0f, 0f), this.m_injuryColorFactor));
			if (base.Opacity == null || base.Opacity.Value >= 1f)
			{
				this.RenderingMode = ModelRenderingMode.Solid;
				return;
			}
			bool flag = this.m_componentCreature.ComponentBody.ImmersionFactor >= 1f;
			bool flag2 = this.m_subsystemSky.ViewUnderWaterDepth > 0f;
			if (flag == flag2)
			{
				this.RenderingMode = ModelRenderingMode.TransparentAfterWater;
				return;
			}
			this.RenderingMode = ModelRenderingMode.TransparentBeforeWater;
		}

        // Token: 0x06000C6C RID: 3180 RVA: 0x0005CF1C File Offset: 0x0005B11C
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentCreature.ComponentHealth.Attacked += delegate(ComponentCreature attacker)
			{
				if (this.DeathPhase == 0f && this.m_componentCreature.ComponentHealth.Health == 0f)
				{
					this.DeathCauseOffset = attacker.ComponentBody.BoundingBox.Center() - this.m_componentCreature.ComponentBody.BoundingBox.Center();
				}
			};
		}

        // Token: 0x06000C6D RID: 3181 RVA: 0x0005CF95 File Offset: 0x0005B195
        public override void OnEntityAdded()
		{
			this.m_componentCreature.ComponentBody.PositionChanged += delegate(ComponentFrame p)
			{
				this.m_eyePosition = null;
			};
			this.m_componentCreature.ComponentBody.RotationChanged += delegate(ComponentFrame p)
			{
				this.m_eyeRotation = null;
			};
		}

		// Token: 0x06000C6E RID: 3182 RVA: 0x0005CFD0 File Offset: 0x0005B1D0
		public virtual void Update(float dt)
		{
			if (this.LookRandomOrder)
			{
				Matrix matrix = this.m_componentCreature.ComponentBody.Matrix;
				Vector3 v = Vector3.Normalize(this.m_randomLookPoint - this.m_componentCreature.ComponentCreatureModel.EyePosition);
				if (this.m_random.Float(0f, 1f) < 0.25f * dt || Vector3.Dot(matrix.Forward, v) < 0.2f)
				{
					float s = this.m_random.Float(-5f, 5f);
					float s2 = this.m_random.Float(-1f, 1f);
					float s3 = this.m_random.Float(3f, 8f);
					this.m_randomLookPoint = this.m_componentCreature.ComponentCreatureModel.EyePosition + s3 * matrix.Forward + s2 * matrix.Up + s * matrix.Right;
				}
				this.LookAtOrder = new Vector3?(this.m_randomLookPoint);
			}
			if (this.LookAtOrder != null)
			{
				Vector3 forward = this.m_componentCreature.ComponentBody.Matrix.Forward;
				Vector3 vector = this.LookAtOrder.Value - this.m_componentCreature.ComponentCreatureModel.EyePosition;
				float x = Vector2.Angle(new Vector2(forward.X, forward.Z), new Vector2(vector.X, vector.Z));
				float y = MathUtils.Asin(0.99f * Vector3.Normalize(vector).Y);
				this.m_componentCreature.ComponentLocomotion.LookOrder = new Vector2(x, y) - this.m_componentCreature.ComponentLocomotion.LookAngles;
			}
			if (this.HeadShakeOrder > 0f)
			{
				this.HeadShakeOrder = MathUtils.Max(this.HeadShakeOrder - dt, 0f);
				float num = 1f * MathUtils.Saturate(4f * this.HeadShakeOrder);
				this.m_componentCreature.ComponentLocomotion.LookOrder = new Vector2(num * (float)MathUtils.Sin(16.0 * this.m_subsystemTime.GameTime + (double)(0.01f * (float)this.GetHashCode())), 0f) - this.m_componentCreature.ComponentLocomotion.LookAngles;
			}
			if (this.m_componentCreature.ComponentHealth.Health == 0f)
			{
				this.DeathPhase = MathUtils.Min(this.DeathPhase + 3f * dt, 1f);
			}
			if (this.m_componentCreature.ComponentHealth.HealthChange < 0f)
			{
				this.m_injuryColorFactor = 1f;
			}
			this.m_injuryColorFactor = MathUtils.Saturate(this.m_injuryColorFactor - 3f * dt);
			this.m_eyePosition = null;
			this.m_eyeRotation = null;
			this.LookRandomOrder = false;
			this.LookAtOrder = null;
		}

		// Token: 0x06000C6F RID: 3183 RVA: 0x0005D2F8 File Offset: 0x0005B4F8
		public virtual Vector3 CalculateEyePosition()
		{
			Matrix matrix = this.m_componentCreature.ComponentBody.Matrix;
			return this.m_componentCreature.ComponentBody.Position + matrix.Up * 0.95f * this.m_componentCreature.ComponentBody.BoxSize.Y + matrix.Forward * 0.45f * this.m_componentCreature.ComponentBody.BoxSize.Z;
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x0005D388 File Offset: 0x0005B588
		public virtual Quaternion CalculateEyeRotation()
		{
			return this.m_componentCreature.ComponentBody.Rotation * Quaternion.CreateFromYawPitchRoll(0f - this.m_componentCreature.ComponentLocomotion.LookAngles.X, this.m_componentCreature.ComponentLocomotion.LookAngles.Y, 0f);
		}

		// Token: 0x04000701 RID: 1793
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000702 RID: 1794
		public new SubsystemSky m_subsystemSky;

		// Token: 0x04000703 RID: 1795
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000704 RID: 1796
		public ComponentCreature m_componentCreature;

		// Token: 0x04000705 RID: 1797
		public Vector3? m_eyePosition;

		// Token: 0x04000706 RID: 1798
		public Quaternion? m_eyeRotation;

		// Token: 0x04000707 RID: 1799
		public float m_injuryColorFactor;

		// Token: 0x04000708 RID: 1800
		public Vector3 m_randomLookPoint;

		// Token: 0x04000709 RID: 1801
		public Game.Random m_random = new Game.Random();
	}
}
