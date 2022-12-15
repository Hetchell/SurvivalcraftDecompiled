using System;
using Engine;
using Engine.Audio;
using Engine.Media;

namespace Game
{
	// Token: 0x020002B8 RID: 696
	public static class MusicManager
	{
		// Token: 0x170002EF RID: 751
		// (get) Token: 0x060013E0 RID: 5088 RVA: 0x0009A078 File Offset: 0x00098278
		// (set) Token: 0x060013E1 RID: 5089 RVA: 0x0009A07F File Offset: 0x0009827F
		public static MusicManager.Mix CurrentMix
		{
			get
			{
				return MusicManager.m_currentMix;
			}
			set
			{
				if (value != MusicManager.m_currentMix)
				{
					MusicManager.m_currentMix = value;
					MusicManager.m_nextSongTime = 0.0;
				}
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x060013E2 RID: 5090 RVA: 0x0009A09D File Offset: 0x0009829D
		public static bool IsPlaying
		{
			get
			{
				return MusicManager.m_sound != null && MusicManager.m_sound.State > SoundState.Stopped;
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x060013E3 RID: 5091 RVA: 0x0009A0B5 File Offset: 0x000982B5
		public static float Volume
		{
			get
			{
				return SettingsManager.MusicVolume * 0.6f;
			}
		}

		// Token: 0x060013E4 RID: 5092 RVA: 0x0009A0C4 File Offset: 0x000982C4
		public static void Update()
		{
			if (MusicManager.m_fadeSound != null)
			{
				MusicManager.m_fadeSound.Volume = MathUtils.Min(MusicManager.m_fadeSound.Volume - 0.33f * MusicManager.Volume * Time.FrameDuration, MusicManager.Volume);
				if (MusicManager.m_fadeSound.Volume <= 0f)
				{
					MusicManager.m_fadeSound.Dispose();
					MusicManager.m_fadeSound = null;
				}
			}
			if (MusicManager.m_sound != null && Time.FrameStartTime >= MusicManager.m_fadeStartTime)
			{
				MusicManager.m_sound.Volume = MathUtils.Min(MusicManager.m_sound.Volume + 0.33f * MusicManager.Volume * Time.FrameDuration, MusicManager.Volume);
			}
			if (MusicManager.m_currentMix == MusicManager.Mix.None || MusicManager.Volume == 0f)
			{
				MusicManager.StopMusic();
				return;
			}
			if (MusicManager.m_currentMix == MusicManager.Mix.Menu && (Time.FrameStartTime >= MusicManager.m_nextSongTime || !MusicManager.IsPlaying))
			{
				float startPercentage = MusicManager.IsPlaying ? MusicManager.m_random.Float(0f, 0.75f) : 0f;
				switch (MusicManager.m_random.Int(0, 5))
				{
				case 0:
					MusicManager.PlayMusic("Music/NativeAmericanFluteSpirit", startPercentage);
					break;
				case 1:
					MusicManager.PlayMusic("Music/AloneForever", startPercentage);
					break;
				case 2:
					MusicManager.PlayMusic("Music/NativeAmerican", startPercentage);
					break;
				case 3:
					MusicManager.PlayMusic("Music/NativeAmericanHeart", startPercentage);
					break;
				case 4:
					MusicManager.PlayMusic("Music/NativeAmericanPeaceFlute", startPercentage);
					break;
				case 5:
					MusicManager.PlayMusic("Music/NativeIndianChant", startPercentage);
					break;
				}
				MusicManager.m_nextSongTime = Time.FrameStartTime + (double)MusicManager.m_random.Float(40f, 60f);
			}
		}

		// Token: 0x060013E5 RID: 5093 RVA: 0x0009A264 File Offset: 0x00098464
		public static void PlayMusic(string name, float startPercentage)
		{
			if (string.IsNullOrEmpty(name))
			{
				MusicManager.StopMusic();
				return;
			}
			try
			{
				MusicManager.StopMusic();
				MusicManager.m_fadeStartTime = Time.FrameStartTime + 2.0;
				float volume = (MusicManager.m_fadeSound != null) ? 0f : MusicManager.Volume;
				StreamingSource streamingSource = ContentManager.Get<StreamingSource>(name).Duplicate();
				streamingSource.Position = (long)(MathUtils.Saturate(startPercentage) * (float)(streamingSource.BytesCount / (long)streamingSource.ChannelsCount / 2L)) / 16L * 16L;
				MusicManager.m_sound = new StreamingSound(streamingSource, volume, 1f, 0f, false, false, 1f);
				MusicManager.m_sound.Play();
			}
			catch
			{
				Log.Warning("Error playing music \"{0}\".", new object[]
				{
					name
				});
			}
		}

		// Token: 0x060013E6 RID: 5094 RVA: 0x0009A334 File Offset: 0x00098534
		public static void StopMusic()
		{
			if (MusicManager.m_sound != null)
			{
				if (MusicManager.m_fadeSound != null)
				{
					MusicManager.m_fadeSound.Dispose();
				}
				MusicManager.m_fadeSound = MusicManager.m_sound;
				MusicManager.m_sound = null;
			}
		}

		// Token: 0x04000DB7 RID: 3511
		public const float m_fadeSpeed = 0.33f;

		// Token: 0x04000DB8 RID: 3512
		public const float m_fadeWait = 2f;

		// Token: 0x04000DB9 RID: 3513
		public static StreamingSound m_fadeSound;

		// Token: 0x04000DBA RID: 3514
		public static StreamingSound m_sound;

		// Token: 0x04000DBB RID: 3515
		public static double m_fadeStartTime;

		// Token: 0x04000DBC RID: 3516
		public static MusicManager.Mix m_currentMix;

		// Token: 0x04000DBD RID: 3517
		public static double m_nextSongTime;

		// Token: 0x04000DBE RID: 3518
		public static Game.Random m_random = new Game.Random();

		// Token: 0x020004BA RID: 1210
		public enum Mix
		{
			// Token: 0x0400177C RID: 6012
			None,
			// Token: 0x0400177D RID: 6013
			Menu
		}
	}
}
