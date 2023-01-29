using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C5 RID: 453
	public class ComponentBoat : Component, IUpdateable
	{
		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000B6A RID: 2922 RVA: 0x00056033 File Offset: 0x00054233
		// (set) Token: 0x06000B6B RID: 2923 RVA: 0x0005603B File Offset: 0x0005423B
		public float MoveOrder { get; set; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000B6C RID: 2924 RVA: 0x00056044 File Offset: 0x00054244
		// (set) Token: 0x06000B6D RID: 2925 RVA: 0x0005604C File Offset: 0x0005424C
		public float TurnOrder { get; set; }

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000B6E RID: 2926 RVA: 0x00056055 File Offset: 0x00054255
		// (set) Token: 0x06000B6F RID: 2927 RVA: 0x0005605D File Offset: 0x0005425D
		public float Health { get; set; }

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000B70 RID: 2928 RVA: 0x00056066 File Offset: 0x00054266
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x00056069 File Offset: 0x00054269
		public void Injure(float amount, ComponentCreature attacker, bool ignoreInvulnerability)
		{
			if (amount > 0f)
			{
				this.Health = MathUtils.Max(this.Health - amount, 0f);
			}
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x0005608C File Offset: 0x0005428C
		public void Update(float dt)
		{
			this.m_componentDamage.Hitpoints = 18494.5f;
			if (this.m_componentDamage.Hitpoints < 0.33f)
			{
				this.m_componentBody.Density = 1.15f;
				if (this.m_componentDamage.Hitpoints - this.m_componentDamage.HitpointsChange >= 0.33f && this.m_componentBody.ImmersionFactor > 0f)
				{
					this.m_subsystemAudio.PlaySound("Audio/Sinking", 1f, 0f, this.m_componentBody.Position, 4f, true);
				}
			}
			else if (this.m_componentDamage.Hitpoints < 0.66f)
			{
				this.m_componentBody.Density = 0.7f;
				if (this.m_componentDamage.Hitpoints - this.m_componentDamage.HitpointsChange >= 0.66f && this.m_componentBody.ImmersionFactor > 0f)
				{
					this.m_subsystemAudio.PlaySound("Audio/Sinking", 1f, 0f, this.m_componentBody.Position, 4f, true);
				}
			}
			bool flag = this.m_componentBody.ImmersionFactor > 0.95f;
			object obj = !flag && this.m_componentBody.ImmersionFactor > 0.01f && this.m_componentBody.StandingOnValue == null && this.m_componentBody.StandingOnBody == null;
			this.m_turnSpeed += 2.5f * this.m_subsystemTime.GameTimeDelta * (1f * this.TurnOrder - this.m_turnSpeed);
			Quaternion rotation = this.m_componentBody.Rotation;
			float num = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
			object obj2 = obj;
			if (obj2 != null)
			{
				num -= this.m_turnSpeed * dt;
			}
			this.m_componentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num);
			if (obj2 != null && this.MoveOrder != 0f)
			{
				this.m_componentBody.Velocity += dt * 3f * this.MoveOrder * this.m_componentBody.Matrix.Forward;
			}
			//if (flag)
			//{
			//	this.m_componentDamage.Damage(0.005f * dt);
			//	if (this.m_componentMount.Rider != null)
			//	{
			//		this.m_componentMount.Rider.StartDismounting();
			//	}
			//}
			this.MoveOrder = 0f;
			this.TurnOrder = 0f;
		}

        // Token: 0x06000B73 RID: 2931 RVA: 0x00056338 File Offset: 0x00054538
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_componentMount = base.Entity.FindComponent<ComponentMount>(true);
			this.m_componentBody = base.Entity.FindComponent<ComponentBody>(true);
			this.m_componentDamage = base.Entity.FindComponent<ComponentDamage>(true);
		}

		// Token: 0x04000661 RID: 1633
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000662 RID: 1634
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000663 RID: 1635
		public ComponentMount m_componentMount;

		// Token: 0x04000664 RID: 1636
		public ComponentBody m_componentBody;

		// Token: 0x04000665 RID: 1637
		public ComponentDamage m_componentDamage;

		// Token: 0x04000666 RID: 1638
		public float m_turnSpeed;
	}
}
