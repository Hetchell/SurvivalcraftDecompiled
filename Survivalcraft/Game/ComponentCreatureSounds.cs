using System;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D0 RID: 464
	public class ComponentCreatureSounds : Component
	{
		// Token: 0x06000C75 RID: 3189 RVA: 0x0005D47C File Offset: 0x0005B67C
		public void PlayIdleSound(bool skipIfRecentlyPlayed)
		{
			if (!string.IsNullOrEmpty(this.m_idleSound) && this.m_subsystemTime.GameTime > this.m_lastSoundTime + (double)(skipIfRecentlyPlayed ? 12f : 1f))
			{
				this.m_lastSoundTime = this.m_subsystemTime.GameTime;
				this.m_subsystemAudio.PlayRandomSound(this.m_idleSound, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, this.m_idleSoundMinDistance, false);
			}
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x0005D510 File Offset: 0x0005B710
		public void PlayPainSound()
		{
			if (!string.IsNullOrEmpty(this.m_painSound) && this.m_subsystemTime.GameTime > this.m_lastSoundTime + 1.0)
			{
				this.m_lastSoundTime = this.m_subsystemTime.GameTime;
				this.m_subsystemAudio.PlayRandomSound(this.m_painSound, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, this.m_painSoundMinDistance, false);
			}
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x0005D59C File Offset: 0x0005B79C
		public void PlayMoanSound()
		{
			if (!string.IsNullOrEmpty(this.m_moanSound) && this.m_subsystemTime.GameTime > this.m_lastSoundTime + 1.0)
			{
				this.m_lastSoundTime = this.m_subsystemTime.GameTime;
				this.m_subsystemAudio.PlayRandomSound(this.m_moanSound, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, this.m_moanSoundMinDistance, false);
			}
		}

		// Token: 0x06000C78 RID: 3192 RVA: 0x0005D628 File Offset: 0x0005B828
		public void PlaySneezeSound()
		{
			if (!string.IsNullOrEmpty(this.m_sneezeSound) && this.m_subsystemTime.GameTime > this.m_lastSoundTime + 1.0)
			{
				this.m_lastSoundTime = this.m_subsystemTime.GameTime;
				this.m_subsystemAudio.PlayRandomSound(this.m_sneezeSound, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, this.m_sneezeSoundMinDistance, false);
			}
		}

		// Token: 0x06000C79 RID: 3193 RVA: 0x0005D6B4 File Offset: 0x0005B8B4
		public void PlayCoughSound()
		{
			if (!string.IsNullOrEmpty(this.m_coughSound) && this.m_subsystemTime.GameTime > this.m_lastCoughingSoundTime + 1.0)
			{
				this.m_lastCoughingSoundTime = this.m_subsystemTime.GameTime;
				this.m_subsystemAudio.PlayRandomSound(this.m_coughSound, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, this.m_coughSoundMinDistance, false);
			}
		}

		// Token: 0x06000C7A RID: 3194 RVA: 0x0005D740 File Offset: 0x0005B940
		public void PlayPukeSound()
		{
			if (!string.IsNullOrEmpty(this.m_pukeSound) && this.m_subsystemTime.GameTime > this.m_lastPukeSoundTime + 1.0)
			{
				this.m_lastPukeSoundTime = this.m_subsystemTime.GameTime;
				this.m_subsystemAudio.PlayRandomSound(this.m_pukeSound, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, this.m_pukeSoundMinDistance, false);
			}
		}

		// Token: 0x06000C7B RID: 3195 RVA: 0x0005D7CC File Offset: 0x0005B9CC
		public void PlayAttackSound()
		{
			if (!string.IsNullOrEmpty(this.m_attackSound) && this.m_subsystemTime.GameTime > this.m_lastSoundTime + 1.0)
			{
				this.m_lastSoundTime = this.m_subsystemTime.GameTime;
				this.m_subsystemAudio.PlayRandomSound(this.m_attackSound, 1f, this.m_random.Float(-0.1f, 0.1f), this.m_componentCreature.ComponentBody.Position, this.m_attackSoundMinDistance, false);
			}
		}

		// Token: 0x06000C7C RID: 3196 RVA: 0x0005D856 File Offset: 0x0005BA56
		public bool PlayFootstepSound(float loudnessMultiplier)
		{
			return this.m_subsystemSoundMaterials.PlayFootstepSound(this.m_componentCreature, loudnessMultiplier);
		}

        // Token: 0x06000C7D RID: 3197 RVA: 0x0005D86C File Offset: 0x0005BA6C
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemSoundMaterials = base.Project.FindSubsystem<SubsystemSoundMaterials>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_idleSound = valuesDictionary.GetValue<string>("IdleSound");
			this.m_painSound = valuesDictionary.GetValue<string>("PainSound");
			this.m_moanSound = valuesDictionary.GetValue<string>("MoanSound");
			this.m_sneezeSound = valuesDictionary.GetValue<string>("SneezeSound");
			this.m_coughSound = valuesDictionary.GetValue<string>("CoughSound");
			this.m_pukeSound = valuesDictionary.GetValue<string>("PukeSound");
			this.m_attackSound = valuesDictionary.GetValue<string>("AttackSound");
			this.m_idleSoundMinDistance = valuesDictionary.GetValue<float>("IdleSoundMinDistance");
			this.m_painSoundMinDistance = valuesDictionary.GetValue<float>("PainSoundMinDistance");
			this.m_moanSoundMinDistance = valuesDictionary.GetValue<float>("MoanSoundMinDistance");
			this.m_sneezeSoundMinDistance = valuesDictionary.GetValue<float>("SneezeSoundMinDistance");
			this.m_coughSoundMinDistance = valuesDictionary.GetValue<float>("CoughSoundMinDistance");
			this.m_pukeSoundMinDistance = valuesDictionary.GetValue<float>("PukeSoundMinDistance");
			this.m_attackSoundMinDistance = valuesDictionary.GetValue<float>("AttackSoundMinDistance");
		}

		// Token: 0x04000719 RID: 1817
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400071A RID: 1818
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400071B RID: 1819
		public SubsystemSoundMaterials m_subsystemSoundMaterials;

		// Token: 0x0400071C RID: 1820
		public ComponentCreature m_componentCreature;

		// Token: 0x0400071D RID: 1821
		public Random m_random = new Random();

		// Token: 0x0400071E RID: 1822
		public string m_idleSound;

		// Token: 0x0400071F RID: 1823
		public string m_painSound;

		// Token: 0x04000720 RID: 1824
		public string m_moanSound;

		// Token: 0x04000721 RID: 1825
		public string m_sneezeSound;

		// Token: 0x04000722 RID: 1826
		public string m_coughSound;

		// Token: 0x04000723 RID: 1827
		public string m_pukeSound;

		// Token: 0x04000724 RID: 1828
		public string m_attackSound;

		// Token: 0x04000725 RID: 1829
		public float m_idleSoundMinDistance;

		// Token: 0x04000726 RID: 1830
		public float m_painSoundMinDistance;

		// Token: 0x04000727 RID: 1831
		public float m_moanSoundMinDistance;

		// Token: 0x04000728 RID: 1832
		public float m_sneezeSoundMinDistance;

		// Token: 0x04000729 RID: 1833
		public float m_coughSoundMinDistance;

		// Token: 0x0400072A RID: 1834
		public float m_pukeSoundMinDistance;

		// Token: 0x0400072B RID: 1835
		public float m_attackSoundMinDistance;

		// Token: 0x0400072C RID: 1836
		public double m_lastSoundTime = -1000.0;

		// Token: 0x0400072D RID: 1837
		public double m_lastCoughingSoundTime = -1000.0;

		// Token: 0x0400072E RID: 1838
		public double m_lastPukeSoundTime = -1000.0;
	}
}
