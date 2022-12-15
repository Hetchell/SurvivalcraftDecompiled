using System;
using Engine;
using Engine.Audio;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000157 RID: 343
	public class SubsystemAmbientSounds : Subsystem, IUpdateable
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000686 RID: 1670 RVA: 0x000299D7 File Offset: 0x00027BD7
		// (set) Token: 0x06000687 RID: 1671 RVA: 0x000299DF File Offset: 0x00027BDF
		public SubsystemAudio SubsystemAudio { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000688 RID: 1672 RVA: 0x000299E8 File Offset: 0x00027BE8
		// (set) Token: 0x06000689 RID: 1673 RVA: 0x000299F0 File Offset: 0x00027BF0
		public float FireSoundVolume { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600068A RID: 1674 RVA: 0x000299F9 File Offset: 0x00027BF9
		// (set) Token: 0x0600068B RID: 1675 RVA: 0x00029A01 File Offset: 0x00027C01
		public float WaterSoundVolume { get; set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600068C RID: 1676 RVA: 0x00029A0A File Offset: 0x00027C0A
		// (set) Token: 0x0600068D RID: 1677 RVA: 0x00029A12 File Offset: 0x00027C12
		public float MagmaSoundVolume { get; set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600068E RID: 1678 RVA: 0x00029A1B File Offset: 0x00027C1B
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x00029A20 File Offset: 0x00027C20
		public void Update(float dt)
		{
			this.m_fireSound.Volume = MathUtils.Lerp(this.m_fireSound.Volume, SettingsManager.SoundsVolume * this.FireSoundVolume, MathUtils.Saturate(3f * Time.FrameDuration));
			if (this.m_fireSound.Volume > 0.5f * AudioManager.MinAudibleVolume)
			{
				this.m_fireSound.Play();
			}
			else
			{
				this.m_fireSound.Pause();
			}
			this.m_waterSound.Volume = MathUtils.Lerp(this.m_waterSound.Volume, SettingsManager.SoundsVolume * this.WaterSoundVolume, MathUtils.Saturate(3f * Time.FrameDuration));
			if (this.m_waterSound.Volume > 0.5f * AudioManager.MinAudibleVolume)
			{
				this.m_waterSound.Play();
			}
			else
			{
				this.m_waterSound.Pause();
			}
			this.m_magmaSound.Volume = MathUtils.Lerp(this.m_magmaSound.Volume, SettingsManager.SoundsVolume * this.MagmaSoundVolume, MathUtils.Saturate(3f * Time.FrameDuration));
			if (this.m_magmaSound.Volume > 0.5f * AudioManager.MinAudibleVolume)
			{
				this.m_magmaSound.Play();
			}
			else
			{
				this.m_magmaSound.Pause();
			}
			if (this.m_magmaSound.State == SoundState.Playing && this.m_random.Bool(0.2f * dt))
			{
				this.SubsystemAudio.PlayRandomSound("Audio/Sizzles", this.m_magmaSound.Volume, this.m_random.Float(-0.2f, 0.2f), 0f, 0f);
			}
			this.FireSoundVolume = 0f;
			this.WaterSoundVolume = 0f;
			this.MagmaSoundVolume = 0f;
		}

        // Token: 0x06000690 RID: 1680 RVA: 0x00029BE0 File Offset: 0x00027DE0
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.SubsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_fireSound = this.SubsystemAudio.CreateSound("Audio/Fire");
			this.m_fireSound.IsLooped = true;
			this.m_fireSound.Volume = 0f;
			this.m_waterSound = this.SubsystemAudio.CreateSound("Audio/Water");
			this.m_waterSound.IsLooped = true;
			this.m_waterSound.Volume = 0f;
			this.m_magmaSound = this.SubsystemAudio.CreateSound("Audio/Magma");
			this.m_magmaSound.IsLooped = true;
			this.m_magmaSound.Volume = 0f;
		}

		// Token: 0x040003A7 RID: 935
		public Sound m_fireSound;

		// Token: 0x040003A8 RID: 936
		public Sound m_waterSound;

		// Token: 0x040003A9 RID: 937
		public Sound m_magmaSound;

		// Token: 0x040003AA RID: 938
		public Game.Random m_random = new Game.Random();
	}
}
