using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001DB RID: 475
	public class ComponentFlightlessBirdModel : ComponentCreatureModel
	{
		// Token: 0x06000CE6 RID: 3302 RVA: 0x0006165C File Offset: 0x0005F85C
		public override void Update(float dt)
		{
			float footstepsPhase = this.m_footstepsPhase;
			float num = this.m_componentCreature.ComponentLocomotion.SlipSpeed ?? Vector3.Dot(this.m_componentCreature.ComponentBody.Velocity, this.m_componentCreature.ComponentBody.Matrix.Forward);
			if (MathUtils.Abs(num) > 0.2f)
			{
				base.MovementAnimationPhase += num * dt * this.m_walkAnimationSpeed;
				this.m_footstepsPhase += 1.25f * this.m_walkAnimationSpeed * num * dt;
			}
			else
			{
				base.MovementAnimationPhase = 0f;
				this.m_footstepsPhase = 0f;
			}
			float num2 = (0f - this.m_walkBobHeight) * MathUtils.Sqr(MathUtils.Sin(6.2831855f * base.MovementAnimationPhase));
			float num3 = MathUtils.Min(12f * this.m_subsystemTime.GameTimeDelta, 1f);
			base.Bob += num3 * (num2 - base.Bob);
			float num4 = MathUtils.Floor(this.m_footstepsPhase);
			if (this.m_footstepsPhase > num4 && footstepsPhase <= num4)
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(1f);
			}
			if (base.FeedOrder)
			{
				this.m_feedFactor = MathUtils.Min(this.m_feedFactor + 2f * dt, 1f);
			}
			else
			{
				this.m_feedFactor = MathUtils.Max(this.m_feedFactor - 2f * dt, 0f);
			}
			base.IsAttackHitMoment = false;
			if (base.AttackOrder)
			{
				this.m_kickFactor = MathUtils.Min(this.m_kickFactor + 6f * dt, 1f);
				float kickPhase = this.m_kickPhase;
				this.m_kickPhase = MathUtils.Remainder(this.m_kickPhase + dt * 2f, 1f);
				if (kickPhase < 0.5f && this.m_kickPhase >= 0.5f)
				{
					base.IsAttackHitMoment = true;
				}
			}
			else
			{
				this.m_kickFactor = MathUtils.Max(this.m_kickFactor - 6f * dt, 0f);
				if (this.m_kickPhase != 0f)
				{
					if (this.m_kickPhase > 0.5f)
					{
						this.m_kickPhase = MathUtils.Remainder(MathUtils.Min(this.m_kickPhase + dt * 2f, 1f), 1f);
					}
					else if (this.m_kickPhase > 0f)
					{
						this.m_kickPhase = MathUtils.Max(this.m_kickPhase - dt * 2f, 0f);
					}
				}
			}
			base.FeedOrder = false;
			base.AttackOrder = false;
			base.Update(dt);
		}

		// Token: 0x06000CE7 RID: 3303 RVA: 0x0006190C File Offset: 0x0005FB0C
		public override void Animate()
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			Vector3 vector = this.m_componentCreature.ComponentBody.Rotation.ToYawPitchRoll();
			if (this.m_componentCreature.ComponentHealth.Health > 0f)
			{
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				if (base.MovementAnimationPhase != 0f && (this.m_componentCreature.ComponentBody.StandingOnValue != null || this.m_componentCreature.ComponentBody.ImmersionFactor > 0f))
				{
					float f0 = (Vector3.Dot(this.m_componentCreature.ComponentBody.Velocity, this.m_componentCreature.ComponentBody.Matrix.Forward) > 0.75f * this.m_componentCreature.ComponentLocomotion.WalkSpeed) ? (1.5f * this.m_walkLegsAngle) : this.m_walkLegsAngle;
					float num4 = MathUtils.Sin(6.2831855f * (base.MovementAnimationPhase + 0f));
					float num5 = MathUtils.Sin(6.2831855f * (base.MovementAnimationPhase + 0.5f));
					num = f0 * num4 + this.m_kickPhase;
					num2 = f0 * num5;
					num3 = MathUtils.DegToRad(5f) * MathUtils.Sin(12.566371f * base.MovementAnimationPhase);
				}
				if (this.m_kickFactor != 0f)
				{
					float x = MathUtils.DegToRad(60f) * MathUtils.Sin(3.1415927f * MathUtils.Sigmoid(this.m_kickPhase, 5f));
					num = MathUtils.Lerp(num, x, this.m_kickFactor);
				}
				float num6 = MathUtils.Min(12f * this.m_subsystemTime.GameTimeDelta, 1f);
				this.m_legAngle1 += num6 * (num - this.m_legAngle1);
				this.m_legAngle2 += num6 * (num2 - this.m_legAngle2);
				this.m_headAngleY += num6 * (num3 - this.m_headAngleY);
				Vector2 vector2 = this.m_componentCreature.ComponentLocomotion.LookAngles;
				vector2.Y += this.m_headAngleY;
				if (this.m_feedFactor > 0f)
				{
					float y = 0f - MathUtils.DegToRad(35f + 55f * SimplexNoise.OctavedNoise((float)this.m_subsystemTime.GameTime, 3f, 2, 2f, 0.75f, false));
					Vector2 v = new Vector2(0f, y);
					vector2 = Vector2.Lerp(vector2, v, this.m_feedFactor);
				}
				vector2.X = MathUtils.Clamp(vector2.X, 0f - MathUtils.DegToRad(90f), MathUtils.DegToRad(90f));
				vector2.Y = MathUtils.Clamp(vector2.Y, 0f - MathUtils.DegToRad(90f), MathUtils.DegToRad(50f));
				Vector2 vector3 = Vector2.Zero;
				if (this.m_neckBone != null)
				{
					vector3 = 0.4f * vector2;
					vector2 = 0.6f * vector2;
				}
				base.SetBoneTransform(this.m_bodyBone.Index, new Matrix?(Matrix.CreateRotationY(vector.X) * Matrix.CreateTranslation(position.X, position.Y + base.Bob, position.Z)));
				base.SetBoneTransform(this.m_headBone.Index, new Matrix?(Matrix.CreateRotationX(vector2.Y) * Matrix.CreateRotationZ(0f - vector2.X)));
				if (this.m_neckBone != null)
				{
					base.SetBoneTransform(this.m_neckBone.Index, new Matrix?(Matrix.CreateRotationX(vector3.Y) * Matrix.CreateRotationZ(0f - vector3.X)));
				}
				base.SetBoneTransform(this.m_leg1Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle1)));
				base.SetBoneTransform(this.m_leg2Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle2)));
			}
			else
			{
				float num7 = 1f - base.DeathPhase;
				float num8 = (float)((Vector3.Dot(this.m_componentFrame.Matrix.Right, base.DeathCauseOffset) > 0f) ? 1 : -1);
				float num9 = this.m_componentCreature.ComponentBody.BoundingBox.Max.Y - this.m_componentCreature.ComponentBody.BoundingBox.Min.Y;
				base.SetBoneTransform(this.m_bodyBone.Index, new Matrix?(Matrix.CreateTranslation(-0.5f * num9 * base.DeathPhase * Vector3.UnitY) * Matrix.CreateFromYawPitchRoll(vector.X, 0f, 1.5707964f * base.DeathPhase * num8) * Matrix.CreateTranslation(0.2f * num9 * base.DeathPhase * Vector3.UnitY) * Matrix.CreateTranslation(position)));
				base.SetBoneTransform(this.m_headBone.Index, new Matrix?(Matrix.Identity));
				if (this.m_neckBone != null)
				{
					base.SetBoneTransform(this.m_neckBone.Index, new Matrix?(Matrix.Identity));
				}
				base.SetBoneTransform(this.m_leg1Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle1 * num7)));
				base.SetBoneTransform(this.m_leg2Bone.Index, new Matrix?(Matrix.CreateRotationX(this.m_legAngle2 * num7)));
			}
			base.Animate();
		}

        // Token: 0x06000CE8 RID: 3304 RVA: 0x00061EB4 File Offset: 0x000600B4
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_walkAnimationSpeed = valuesDictionary.GetValue<float>("WalkAnimationSpeed");
			this.m_walkLegsAngle = valuesDictionary.GetValue<float>("WalkLegsAngle");
			this.m_walkBobHeight = valuesDictionary.GetValue<float>("WalkBobHeight");
		}

		// Token: 0x06000CE9 RID: 3305 RVA: 0x00061EF4 File Offset: 0x000600F4
		public override void SetModel(Model model)
		{
			base.SetModel(model);
			if (base.Model != null)
			{
				this.m_bodyBone = base.Model.FindBone("Body", true);
				this.m_neckBone = base.Model.FindBone("Neck", false);
				this.m_headBone = base.Model.FindBone("Head", true);
				this.m_leg1Bone = base.Model.FindBone("Leg1", true);
				this.m_leg2Bone = base.Model.FindBone("Leg2", true);
				return;
			}
			this.m_bodyBone = null;
			this.m_neckBone = null;
			this.m_headBone = null;
			this.m_leg1Bone = null;
			this.m_leg2Bone = null;
		}

		// Token: 0x040007B7 RID: 1975
		public ModelBone m_bodyBone;

		// Token: 0x040007B8 RID: 1976
		public ModelBone m_neckBone;

		// Token: 0x040007B9 RID: 1977
		public ModelBone m_headBone;

		// Token: 0x040007BA RID: 1978
		public ModelBone m_leg1Bone;

		// Token: 0x040007BB RID: 1979
		public ModelBone m_leg2Bone;

		// Token: 0x040007BC RID: 1980
		public float m_walkAnimationSpeed;

		// Token: 0x040007BD RID: 1981
		public float m_walkLegsAngle;

		// Token: 0x040007BE RID: 1982
		public float m_walkBobHeight;

		// Token: 0x040007BF RID: 1983
		public float m_feedFactor;

		// Token: 0x040007C0 RID: 1984
		public float m_footstepsPhase;

		// Token: 0x040007C1 RID: 1985
		public float m_kickFactor;

		// Token: 0x040007C2 RID: 1986
		public float m_kickPhase;

		// Token: 0x040007C3 RID: 1987
		public float m_legAngle1;

		// Token: 0x040007C4 RID: 1988
		public float m_legAngle2;

		// Token: 0x040007C5 RID: 1989
		public float m_headAngleY;
	}
}
