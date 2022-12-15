using System;
using System.Collections.Generic;
using Engine;
using Engine.Audio;
using Engine.Content;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200015A RID: 346
	public class SubsystemAudio : Subsystem, IUpdateable
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x0002AD74 File Offset: 0x00028F74
		public ReadOnlyList<Vector3> ListenerPositions
		{
			get
			{
				return new ReadOnlyList<Vector3>(this.m_listenerPositions);
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060006A3 RID: 1699 RVA: 0x0002AD81 File Offset: 0x00028F81
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x0002AD84 File Offset: 0x00028F84
		public float CalculateListenerDistanceSquared(Vector3 p)
		{
			float num = float.MaxValue;
			for (int i = 0; i < this.m_listenerPositions.Count; i++)
			{
				float num2 = Vector3.DistanceSquared(this.m_listenerPositions[i], p);
				if (num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x0002ADC7 File Offset: 0x00028FC7
		public float CalculateListenerDistance(Vector3 p)
		{
			return MathUtils.Sqrt(this.CalculateListenerDistanceSquared(p));
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x0002ADD8 File Offset: 0x00028FD8
		public void Mute()
		{
			foreach (Sound sound in this.m_sounds)
			{
				if (sound.State == SoundState.Playing)
				{
					this.m_mutedSounds[sound] = true;
					sound.Pause();
				}
			}
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x0002AE40 File Offset: 0x00029040
		public void Unmute()
		{
			foreach (Sound sound in this.m_mutedSounds.Keys)
			{
				sound.Play();
			}
			this.m_mutedSounds.Clear();
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x0002AEA0 File Offset: 0x000290A0
		public void PlaySound(string name, float volume, float pitch, float pan, float delay)
		{
			double num = this.m_subsystemTime.GameTime + (double)delay;
			this.m_nextSoundTime = MathUtils.Min(this.m_nextSoundTime, num);
			this.m_queuedSounds.Add(new SubsystemAudio.SoundInfo
			{
				Time = num,
				Name = name,
				Volume = volume,
				Pitch = pitch,
				Pan = pan
			});
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x0002AF0C File Offset: 0x0002910C
		public void PlaySound(string name, float volume, float pitch, Vector3 position, float minDistance, float delay)
		{
			float num = this.CalculateVolume(this.CalculateListenerDistance(position), minDistance, 2f);
			this.PlaySound(name, volume * num, pitch, 0f, delay);
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x0002AF44 File Offset: 0x00029144
		public void PlaySound(string name, float volume, float pitch, Vector3 position, float minDistance, bool autoDelay)
		{
			float num = this.CalculateVolume(this.CalculateListenerDistance(position), minDistance, 2f);
			this.PlaySound(name, volume * num, pitch, 0f, autoDelay ? this.CalculateDelay(position) : 0f);
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x0002AF8C File Offset: 0x0002918C
		public void PlayRandomSound(string directory, float volume, float pitch, float pan, float delay)
		{
			ReadOnlyList<ContentInfo> readOnlyList = ContentManager.List(directory);
			if (readOnlyList.Count > 0)
			{
				int index = this.m_random.Int(0, readOnlyList.Count - 1);
				this.PlaySound(readOnlyList[index].Name, volume, pitch, pan, delay);
				return;
			}
			Log.Warning("Sounds directory \"{0}\" not found or empty.", new object[]
			{
				directory
			});
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x0002AFF0 File Offset: 0x000291F0
		public void PlayRandomSound(string directory, float volume, float pitch, Vector3 position, float minDistance, float delay)
		{
			float num = this.CalculateVolume(this.CalculateListenerDistance(position), minDistance, 2f);
			this.PlayRandomSound(directory, volume * num, pitch, 0f, delay);
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x0002B028 File Offset: 0x00029228
		public void PlayRandomSound(string directory, float volume, float pitch, Vector3 position, float minDistance, bool autoDelay)
		{
			float num = this.CalculateVolume(this.CalculateListenerDistance(position), minDistance, 2f);
			this.PlayRandomSound(directory, volume * num, pitch, 0f, autoDelay ? this.CalculateDelay(position) : 0f);
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x0002B070 File Offset: 0x00029270
		public Sound CreateSound(string name)
		{
			Sound sound = new Sound(ContentManager.Get<SoundBuffer>(name), 1f, 1f, 0f, false, false);
			this.m_sounds.Add(sound);
			return sound;
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x0002B0A7 File Offset: 0x000292A7
		public float CalculateVolume(float distance, float minDistance, float rolloffFactor = 2f)
		{
			if (distance > minDistance)
			{
				return minDistance / (minDistance + MathUtils.Max(rolloffFactor * (distance - minDistance), 0f));
			}
			return 1f;
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x0002B0C6 File Offset: 0x000292C6
		public float CalculateDelay(Vector3 position)
		{
			return this.CalculateDelay(this.CalculateListenerDistance(position));
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x0002B0D5 File Offset: 0x000292D5
		public float CalculateDelay(float distance)
		{
			return MathUtils.Min(distance / 100f, 5f);
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x0002B0E8 File Offset: 0x000292E8
		public void Update(float dt)
		{
			this.m_listenerPositions.Clear();
			foreach (GameWidget gameWidget in this.m_subsystemViews.GameWidgets)
			{
				this.m_listenerPositions.Add(gameWidget.ActiveCamera.ViewPosition);
			}
			if (this.m_subsystemTime.GameTime < this.m_nextSoundTime)
			{
				return;
			}
			this.m_nextSoundTime = double.MaxValue;
			int i = 0;
			while (i < this.m_queuedSounds.Count)
			{
				SubsystemAudio.SoundInfo soundInfo = this.m_queuedSounds[i];
				if (this.m_subsystemTime.GameTime >= soundInfo.Time)
				{
					if (this.m_subsystemTime.GameTimeFactor == 1f && this.m_subsystemTime.FixedTimeStep == null && soundInfo.Volume * SettingsManager.SoundsVolume > AudioManager.MinAudibleVolume && this.UpdateCongestion(soundInfo.Name, soundInfo.Volume))
					{
						AudioManager.PlaySound(soundInfo.Name, soundInfo.Volume, soundInfo.Pitch, soundInfo.Pan);
					}
					this.m_queuedSounds.RemoveAt(i);
				}
				else
				{
					this.m_nextSoundTime = MathUtils.Min(this.m_nextSoundTime, soundInfo.Time);
					i++;
				}
			}
		}

        // Token: 0x060006B3 RID: 1715 RVA: 0x0002B25C File Offset: 0x0002945C
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemViews = base.Project.FindSubsystem<SubsystemGameWidgets>(true);
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x0002B284 File Offset: 0x00029484
		public override void Dispose()
		{
			foreach (Sound sound in this.m_sounds)
			{
				sound.Dispose();
			}
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x0002B2D4 File Offset: 0x000294D4
		public bool UpdateCongestion(string name, float volume)
		{
			SubsystemAudio.Congestion congestion;
			if (!this.m_congestions.TryGetValue(name, out congestion))
			{
				congestion = new SubsystemAudio.Congestion();
				this.m_congestions.Add(name, congestion);
			}
			double realTime = Time.RealTime;
			double lastUpdateTime = congestion.LastUpdateTime;
			double lastPlayedTime = congestion.LastPlayedTime;
			float num = (lastUpdateTime > 0.0) ? ((float)(realTime - lastUpdateTime)) : 0f;
			congestion.Value = MathUtils.Max(congestion.Value - 10f * num, 0f);
			congestion.LastUpdateTime = realTime;
			if (congestion.Value <= 6f && (lastPlayedTime == 0.0 || volume > congestion.LastPlayedVolume || realTime - lastPlayedTime >= 0.0))
			{
				congestion.LastPlayedTime = realTime;
				congestion.LastPlayedVolume = volume;
				congestion.Value += 1f;
				return true;
			}
			return false;
		}

		// Token: 0x040003C0 RID: 960
		public SubsystemTime m_subsystemTime;

		// Token: 0x040003C1 RID: 961
		public SubsystemGameWidgets m_subsystemViews;

		// Token: 0x040003C2 RID: 962
		public Game.Random m_random = new Game.Random();

		// Token: 0x040003C3 RID: 963
		public List<Vector3> m_listenerPositions = new List<Vector3>();

		// Token: 0x040003C4 RID: 964
		public Dictionary<string, SubsystemAudio.Congestion> m_congestions = new Dictionary<string, SubsystemAudio.Congestion>();

		// Token: 0x040003C5 RID: 965
		public double m_nextSoundTime;

		// Token: 0x040003C6 RID: 966
		public List<SubsystemAudio.SoundInfo> m_queuedSounds = new List<SubsystemAudio.SoundInfo>();

		// Token: 0x040003C7 RID: 967
		public List<Sound> m_sounds = new List<Sound>();

		// Token: 0x040003C8 RID: 968
		public Dictionary<Sound, bool> m_mutedSounds = new Dictionary<Sound, bool>();

		// Token: 0x02000412 RID: 1042
		public class Congestion
		{
			// Token: 0x0400153C RID: 5436
			public double LastUpdateTime;

			// Token: 0x0400153D RID: 5437
			public double LastPlayedTime;

			// Token: 0x0400153E RID: 5438
			public float LastPlayedVolume;

			// Token: 0x0400153F RID: 5439
			public float Value;
		}

		// Token: 0x02000413 RID: 1043
		public struct SoundInfo
		{
			// Token: 0x04001540 RID: 5440
			public double Time;

			// Token: 0x04001541 RID: 5441
			public string Name;

			// Token: 0x04001542 RID: 5442
			public float Volume;

			// Token: 0x04001543 RID: 5443
			public float Pitch;

			// Token: 0x04001544 RID: 5444
			public float Pan;
		}
	}
}
