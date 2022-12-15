using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D9 RID: 473
	public class ComponentFishModel : ComponentCreatureModel
	{
		// Token: 0x17000155 RID: 341
		// (get) Token: 0x06000CD3 RID: 3283 RVA: 0x0006088B File Offset: 0x0005EA8B
		// (set) Token: 0x06000CD4 RID: 3284 RVA: 0x00060893 File Offset: 0x0005EA93
		public float? BendOrder { get; set; }

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x06000CD5 RID: 3285 RVA: 0x0006089C File Offset: 0x0005EA9C
		// (set) Token: 0x06000CD6 RID: 3286 RVA: 0x000608A4 File Offset: 0x0005EAA4
		public float DigInOrder { get; set; }

		// Token: 0x06000CD7 RID: 3287 RVA: 0x000608B0 File Offset: 0x0005EAB0
		public override void Update(float dt)
		{
			if (this.m_componentCreature.ComponentLocomotion.LastSwimOrder != null && this.m_componentCreature.ComponentLocomotion.LastSwimOrder.Value != Vector3.Zero)
			{
				float num = (this.m_componentCreature.ComponentLocomotion.LastSwimOrder.Value.LengthSquared() > 0.99f) ? 1.75f : 1f;
				base.MovementAnimationPhase = MathUtils.Remainder(base.MovementAnimationPhase + this.m_swimAnimationSpeed * num * dt, 1000f);
			}
			else
			{
				base.MovementAnimationPhase = MathUtils.Remainder(base.MovementAnimationPhase + 0.15f * this.m_swimAnimationSpeed * dt, 1000f);
			}
			if (this.BendOrder != null)
			{
				if (this.m_hasVerticalTail)
				{
					this.m_tailTurn.X = 0f;
					this.m_tailTurn.Y = this.BendOrder.Value;
				}
				else
				{
					this.m_tailTurn.X = this.BendOrder.Value;
					this.m_tailTurn.Y = 0f;
				}
			}
			else
			{
				this.m_tailTurn.X = this.m_tailTurn.X + MathUtils.Saturate(2f * this.m_componentCreature.ComponentLocomotion.TurnSpeed * dt) * (0f - this.m_componentCreature.ComponentLocomotion.LastTurnOrder.X - this.m_tailTurn.X);
			}
			if (this.DigInOrder > this.m_digInDepth)
			{
				float num2 = (this.DigInOrder - this.m_digInDepth) * MathUtils.Min(1.5f * dt, 1f);
				this.m_digInDepth += num2;
				this.m_digInTailPhase += 20f * num2;
			}
			else if (this.DigInOrder < this.m_digInDepth)
			{
				this.m_digInDepth += (this.DigInOrder - this.m_digInDepth) * MathUtils.Min(5f * dt, 1f);
			}
			float num3 = 0.33f * this.m_componentCreature.ComponentLocomotion.TurnSpeed;
			float num4 = 1f * this.m_componentCreature.ComponentLocomotion.TurnSpeed;
			base.IsAttackHitMoment = false;
			if (base.AttackOrder || base.FeedOrder)
			{
				if (base.AttackOrder)
				{
					this.m_tailWagPhase = MathUtils.Remainder(this.m_tailWagPhase + num3 * dt, 1f);
				}
				float bitingPhase = this.m_bitingPhase;
				this.m_bitingPhase = MathUtils.Remainder(this.m_bitingPhase + num4 * dt, 1f);
				if (base.AttackOrder && bitingPhase < 0.5f && this.m_bitingPhase >= 0.5f)
				{
					base.IsAttackHitMoment = true;
				}
			}
			else
			{
				if (this.m_tailWagPhase != 0f)
				{
					this.m_tailWagPhase = MathUtils.Remainder(MathUtils.Min(this.m_tailWagPhase + num3 * dt, 1f), 1f);
				}
				if (this.m_bitingPhase != 0f)
				{
					this.m_bitingPhase = MathUtils.Remainder(MathUtils.Min(this.m_bitingPhase + num4 * dt, 1f), 1f);
				}
			}
			base.AttackOrder = false;
			base.FeedOrder = false;
			this.BendOrder = null;
			this.DigInOrder = 0f;
			base.Update(dt);
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x00060C14 File Offset: 0x0005EE14
		public override void Animate()
		{
			Vector3 vector = this.m_componentCreature.ComponentBody.Rotation.ToYawPitchRoll();
			if (this.m_componentCreature.ComponentHealth.Health > 0f)
			{
				float num = this.m_digInTailPhase + this.m_tailWagPhase;
				float num2;
				float num3;
				float num4;
				float num5;
				if (this.m_hasVerticalTail)
				{
					num2 = MathUtils.DegToRad(25f) * MathUtils.Clamp(0.5f * MathUtils.Sin(6.2831855f * num) - this.m_tailTurn.X, -1f, 1f);
					num3 = MathUtils.DegToRad(30f) * MathUtils.Clamp(0.5f * MathUtils.Sin(2f * (3.1415927f * MathUtils.Max(num - 0.25f, 0f))) - this.m_tailTurn.X, -1f, 1f);
					num4 = MathUtils.DegToRad(25f) * MathUtils.Clamp(0.5f * MathUtils.Sin(6.2831855f * base.MovementAnimationPhase) - this.m_tailTurn.Y, -1f, 1f);
					num5 = MathUtils.DegToRad(30f) * MathUtils.Clamp(0.5f * MathUtils.Sin(6.2831855f * MathUtils.Max(base.MovementAnimationPhase - 0.25f, 0f)) - this.m_tailTurn.Y, -1f, 1f);
				}
				else
				{
					num2 = MathUtils.DegToRad(25f) * MathUtils.Clamp(0.5f * MathUtils.Sin(6.2831855f * (base.MovementAnimationPhase + num)) - this.m_tailTurn.X, -1f, 1f);
					num3 = MathUtils.DegToRad(30f) * MathUtils.Clamp(0.5f * MathUtils.Sin(2f * (3.1415927f * MathUtils.Max(base.MovementAnimationPhase + num - 0.25f, 0f))) - this.m_tailTurn.X, -1f, 1f);
					num4 = MathUtils.DegToRad(25f) * MathUtils.Clamp(0f - this.m_tailTurn.Y, -1f, 1f);
					num5 = MathUtils.DegToRad(30f) * MathUtils.Clamp(0f - this.m_tailTurn.Y, -1f, 1f);
				}
				float radians = 0f;
				if (this.m_bitingPhase > 0f)
				{
					radians = (0f - MathUtils.DegToRad(30f)) * MathUtils.Sin(3.1415927f * this.m_bitingPhase);
				}
				Matrix value = Matrix.CreateFromYawPitchRoll(vector.X, 0f, 0f) * Matrix.CreateTranslation(this.m_componentCreature.ComponentBody.Position + new Vector3(0f, 0f - this.m_digInDepth, 0f));
				base.SetBoneTransform(this.m_bodyBone.Index, new Matrix?(value));
				Matrix matrix = Matrix.Identity;
				if (num2 != 0f)
				{
					matrix *= Matrix.CreateRotationZ(num2);
				}
				if (num4 != 0f)
				{
					matrix *= Matrix.CreateRotationX(num4);
				}
				Matrix matrix2 = Matrix.Identity;
				if (num3 != 0f)
				{
					matrix2 *= Matrix.CreateRotationZ(num3);
				}
				if (num5 != 0f)
				{
					matrix2 *= Matrix.CreateRotationX(num5);
				}
				base.SetBoneTransform(this.m_tail1Bone.Index, new Matrix?(matrix));
				base.SetBoneTransform(this.m_tail2Bone.Index, new Matrix?(matrix2));
				if (this.m_jawBone != null)
				{
					base.SetBoneTransform(this.m_jawBone.Index, new Matrix?(Matrix.CreateRotationX(radians)));
				}
			}
			else
			{
				float num6 = this.m_componentCreature.ComponentBody.BoundingBox.Max.Y - this.m_componentCreature.ComponentBody.BoundingBox.Min.Y;
				Vector3 position = this.m_componentCreature.ComponentBody.Position + 1f * num6 * base.DeathPhase * Vector3.UnitY;
				base.SetBoneTransform(this.m_bodyBone.Index, new Matrix?(Matrix.CreateFromYawPitchRoll(vector.X, 0f, 3.1415927f * base.DeathPhase) * Matrix.CreateTranslation(position)));
				base.SetBoneTransform(this.m_tail1Bone.Index, new Matrix?(Matrix.Identity));
				base.SetBoneTransform(this.m_tail2Bone.Index, new Matrix?(Matrix.Identity));
				if (this.m_jawBone != null)
				{
					base.SetBoneTransform(this.m_jawBone.Index, new Matrix?(Matrix.Identity));
				}
			}
			base.Animate();
		}

        // Token: 0x06000CD9 RID: 3289 RVA: 0x000610EC File Offset: 0x0005F2EC
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_hasVerticalTail = valuesDictionary.GetValue<bool>("HasVerticalTail");
			this.m_swimAnimationSpeed = valuesDictionary.GetValue<float>("SwimAnimationSpeed");
		}

		// Token: 0x06000CDA RID: 3290 RVA: 0x00061118 File Offset: 0x0005F318
		public override void SetModel(Model model)
		{
			base.SetModel(model);
			if (base.Model != null)
			{
				this.m_bodyBone = base.Model.FindBone("Body", true);
				this.m_tail1Bone = base.Model.FindBone("Tail1", true);
				this.m_tail2Bone = base.Model.FindBone("Tail2", true);
				this.m_jawBone = base.Model.FindBone("Jaw", false);
				return;
			}
			this.m_bodyBone = null;
			this.m_tail1Bone = null;
			this.m_tail2Bone = null;
			this.m_jawBone = null;
		}

		// Token: 0x06000CDB RID: 3291 RVA: 0x000611B0 File Offset: 0x0005F3B0
		public override Vector3 CalculateEyePosition()
		{
			Matrix matrix = this.m_componentCreature.ComponentBody.Matrix;
			return this.m_componentCreature.ComponentBody.Position + matrix.Up * 1f * this.m_componentCreature.ComponentBody.BoxSize.Y + matrix.Forward * 0.45f * this.m_componentCreature.ComponentBody.BoxSize.Z;
		}

		// Token: 0x040007A1 RID: 1953
		public ModelBone m_bodyBone;

		// Token: 0x040007A2 RID: 1954
		public ModelBone m_tail1Bone;

		// Token: 0x040007A3 RID: 1955
		public ModelBone m_tail2Bone;

		// Token: 0x040007A4 RID: 1956
		public ModelBone m_jawBone;

		// Token: 0x040007A5 RID: 1957
		public float m_swimAnimationSpeed;

		// Token: 0x040007A6 RID: 1958
		public bool m_hasVerticalTail;

		// Token: 0x040007A7 RID: 1959
		public float m_bitingPhase;

		// Token: 0x040007A8 RID: 1960
		public float m_tailWagPhase;

		// Token: 0x040007A9 RID: 1961
		public Vector2 m_tailTurn;

		// Token: 0x040007AA RID: 1962
		public float m_digInDepth;

		// Token: 0x040007AB RID: 1963
		public float m_digInTailPhase;
	}
}
