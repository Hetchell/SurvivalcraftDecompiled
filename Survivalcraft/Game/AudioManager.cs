using System;
using Engine;
using Engine.Audio;

namespace Game
{
	// Token: 0x0200021B RID: 539
	public static class AudioManager
	{
		// Token: 0x1700025F RID: 607
		// (get) Token: 0x06001098 RID: 4248 RVA: 0x0007E68F File Offset: 0x0007C88F
		public static float MinAudibleVolume
		{
			get
			{
				return 0.05f * SettingsManager.SoundsVolume;
			}
		}

		// Token: 0x06001099 RID: 4249 RVA: 0x0007E69C File Offset: 0x0007C89C
		public static void PlaySound(string name, float volume, float pitch, float pan)
		{
			if (SettingsManager.SoundsVolume > 0f)
			{
				float num = volume * SettingsManager.SoundsVolume;
				if (num > AudioManager.MinAudibleVolume)
				{
					try
					{
						new Sound(ContentManager.Get<SoundBuffer>(name), num, AudioManager.ToEnginePitch(pitch), pan, false, true).Play();
					}
					catch (Exception)
					{
					}
				}
			}
		}

		// Token: 0x0600109A RID: 4250 RVA: 0x0007E6F4 File Offset: 0x0007C8F4
		public static float ToEnginePitch(float pitch)
		{
			return MathUtils.Pow(2f, pitch);
		}
	}
}
