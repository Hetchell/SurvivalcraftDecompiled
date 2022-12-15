using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D1 RID: 465
	public class ComponentDamage : Component, IUpdateable
	{
		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000C7F RID: 3199 RVA: 0x0005D9EF File Offset: 0x0005BBEF
		// (set) Token: 0x06000C80 RID: 3200 RVA: 0x0005D9F7 File Offset: 0x0005BBF7
		public float Hitpoints { get; set; }

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000C81 RID: 3201 RVA: 0x0005DA00 File Offset: 0x0005BC00
		// (set) Token: 0x06000C82 RID: 3202 RVA: 0x0005DA08 File Offset: 0x0005BC08
		public float HitpointsChange { get; set; }

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000C83 RID: 3203 RVA: 0x0005DA11 File Offset: 0x0005BC11
		// (set) Token: 0x06000C84 RID: 3204 RVA: 0x0005DA19 File Offset: 0x0005BC19
		public float AttackResilience { get; set; }

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000C85 RID: 3205 RVA: 0x0005DA22 File Offset: 0x0005BC22
		// (set) Token: 0x06000C86 RID: 3206 RVA: 0x0005DA2A File Offset: 0x0005BC2A
		public string DamageSoundName { get; set; }

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06000C87 RID: 3207 RVA: 0x0005DA33 File Offset: 0x0005BC33
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000C88 RID: 3208 RVA: 0x0005DA36 File Offset: 0x0005BC36
		public void Damage(float amount)
		{
			if (amount > 0f && this.Hitpoints > 0f)
			{
				this.Hitpoints = MathUtils.Max(this.Hitpoints - amount, 0f);
			}
		}

		// Token: 0x06000C89 RID: 3209 RVA: 0x0005DA68 File Offset: 0x0005BC68
		public void Update(float dt)
		{
			Vector3 position = this.m_componentBody.Position;
			if (this.Hitpoints <= 0f)
			{
				this.m_subsystemParticles.AddParticleSystem(new BlockDebrisParticleSystem(this.m_subsystemTerrain, position + this.m_componentBody.BoxSize.Y / 2f * Vector3.UnitY, this.m_debrisStrength, this.m_debrisScale, Color.White, this.m_debrisTextureSlot));
				this.m_subsystemAudio.PlayRandomSound(this.DamageSoundName, 1f, 0f, this.m_componentBody.Position, 4f, true);
				base.Project.RemoveEntity(base.Entity, true);
			}
			float num = MathUtils.Abs(this.m_componentBody.CollisionVelocityChange.Y);
			if (num > this.m_fallResilience)
			{
				float amount = MathUtils.Sqr(MathUtils.Max(num - this.m_fallResilience, 0f)) / 15f;
				this.Damage(amount);
			}
			if (position.Y < -10f || position.Y > 276f)
			{
				this.Damage(this.Hitpoints);
			}
			if (this.m_componentOnFire != null && (this.m_componentOnFire.IsOnFire || this.m_componentOnFire.TouchesFire))
			{
				this.Damage(dt / this.m_fireResilience);
			}
			this.HitpointsChange = this.Hitpoints - this.m_lastHitpoints;
			this.m_lastHitpoints = this.Hitpoints;
		}

        // Token: 0x06000C8A RID: 3210 RVA: 0x0005DBDC File Offset: 0x0005BDDC
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_componentBody = base.Entity.FindComponent<ComponentBody>(true);
			this.m_componentOnFire = base.Entity.FindComponent<ComponentOnFire>();
			this.Hitpoints = valuesDictionary.GetValue<float>("Hitpoints");
			this.AttackResilience = valuesDictionary.GetValue<float>("AttackResilience");
			this.m_fallResilience = valuesDictionary.GetValue<float>("FallResilience");
			this.m_fireResilience = valuesDictionary.GetValue<float>("FireResilience");
			this.m_debrisTextureSlot = valuesDictionary.GetValue<int>("DebrisTextureSlot");
			this.m_debrisStrength = valuesDictionary.GetValue<float>("DebrisStrength");
			this.m_debrisScale = valuesDictionary.GetValue<float>("DebrisScale");
			this.DamageSoundName = valuesDictionary.GetValue<string>("DestructionSoundName");
		}

        // Token: 0x06000C8B RID: 3211 RVA: 0x0005DCCA File Offset: 0x0005BECA
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("Hitpoints", this.Hitpoints);
		}

		// Token: 0x0400072F RID: 1839
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000730 RID: 1840
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000731 RID: 1841
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000732 RID: 1842
		public ComponentBody m_componentBody;

		// Token: 0x04000733 RID: 1843
		public ComponentOnFire m_componentOnFire;

		// Token: 0x04000734 RID: 1844
		public float m_lastHitpoints;

		// Token: 0x04000735 RID: 1845
		public float m_fallResilience;

		// Token: 0x04000736 RID: 1846
		public float m_fireResilience;

		// Token: 0x04000737 RID: 1847
		public int m_debrisTextureSlot;

		// Token: 0x04000738 RID: 1848
		public float m_debrisStrength;

		// Token: 0x04000739 RID: 1849
		public float m_debrisScale;
	}
}
