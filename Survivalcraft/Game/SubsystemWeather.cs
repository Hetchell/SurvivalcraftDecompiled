using System;
using System.Collections.Generic;
using Engine;
using Engine.Audio;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001B9 RID: 441
	public class SubsystemWeather : Subsystem, IDrawable, IUpdateable
	{
		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000AF1 RID: 2801 RVA: 0x0005187F File Offset: 0x0004FA7F
		// (set) Token: 0x06000AF2 RID: 2802 RVA: 0x00051887 File Offset: 0x0004FA87
		public SubsystemTerrain SubsystemTerrain { get; set; }

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000AF3 RID: 2803 RVA: 0x00051890 File Offset: 0x0004FA90
		// (set) Token: 0x06000AF4 RID: 2804 RVA: 0x00051898 File Offset: 0x0004FA98
		public SubsystemSky SubsystemSky { get; set; }

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000AF5 RID: 2805 RVA: 0x000518A1 File Offset: 0x0004FAA1
		// (set) Token: 0x06000AF6 RID: 2806 RVA: 0x000518A9 File Offset: 0x0004FAA9
		public SubsystemTime SubsystemTime { get; set; }

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000AF7 RID: 2807 RVA: 0x000518B2 File Offset: 0x0004FAB2
		// (set) Token: 0x06000AF8 RID: 2808 RVA: 0x000518BA File Offset: 0x0004FABA
		public RainSplashParticleSystem RainSplashParticleSystem { get; set; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000AF9 RID: 2809 RVA: 0x000518C3 File Offset: 0x0004FAC3
		// (set) Token: 0x06000AFA RID: 2810 RVA: 0x000518CB File Offset: 0x0004FACB
		public SnowSplashParticleSystem SnowSplashParticleSystem { get; set; }

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000AFB RID: 2811 RVA: 0x000518D4 File Offset: 0x0004FAD4
		// (set) Token: 0x06000AFC RID: 2812 RVA: 0x000518DC File Offset: 0x0004FADC
		public Color RainColor { get; set; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000AFD RID: 2813 RVA: 0x000518E5 File Offset: 0x0004FAE5
		// (set) Token: 0x06000AFE RID: 2814 RVA: 0x000518ED File Offset: 0x0004FAED
		public Color SnowColor { get; set; }

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000AFF RID: 2815 RVA: 0x000518F6 File Offset: 0x0004FAF6
		// (set) Token: 0x06000B00 RID: 2816 RVA: 0x000518FE File Offset: 0x0004FAFE
		public float GlobalPrecipitationIntensity { get; set; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000B01 RID: 2817 RVA: 0x00051907 File Offset: 0x0004FB07
		public int[] DrawOrders
		{
			get
			{
				return SubsystemWeather.m_drawOrders;
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000B02 RID: 2818 RVA: 0x0005190E File Offset: 0x0004FB0E
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x00051914 File Offset: 0x0004FB14
		public PrecipitationShaftInfo GetPrecipitationShaftInfo(int x, int z)
		{
			int shaftValue = this.SubsystemTerrain.Terrain.GetShaftValue(x, z);
			int seasonalTemperature = this.SubsystemTerrain.Terrain.GetSeasonalTemperature(shaftValue);
			int num = Terrain.ExtractTopHeight(shaftValue);
			if (SubsystemWeather.IsPlaceFrozen(seasonalTemperature, num))
			{
				return new PrecipitationShaftInfo
				{
					Intensity = this.GlobalPrecipitationIntensity,
					Type = PrecipitationType.Snow,
					YLimit = num + 1
				};
			}
			int seasonalHumidity = this.SubsystemTerrain.Terrain.GetSeasonalHumidity(shaftValue);
			if (seasonalTemperature <= 8 || seasonalHumidity >= 8)
			{
				return new PrecipitationShaftInfo
				{
					Intensity = this.GlobalPrecipitationIntensity,
					Type = PrecipitationType.Rain,
					YLimit = num + 1
				};
			}
			return new PrecipitationShaftInfo
			{
				Intensity = 0f,
				Type = PrecipitationType.Rain,
				YLimit = num + 1
			};
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x000519EC File Offset: 0x0004FBEC
		public void ManualLightingStrike(Vector3 position, Vector3 direction)
		{
			int num = Terrain.ToCell(position.X + direction.X * 32f);
			int num2 = Terrain.ToCell(position.Z + direction.Z * 32f);
			Vector3? vector = null;
			for (int i = 0; i < 300; i++)
			{
				int num3 = this.m_random.Int(-8, 8);
				int num4 = this.m_random.Int(-8, 8);
				int num5 = num + num3;
				int num6 = num2 + num4;
				int num7 = this.SubsystemTerrain.Terrain.CalculateTopmostCellHeight(num5, num6);
				if (vector == null || (float)num7 > vector.Value.Y)
				{
					vector = new Vector3?(new Vector3((float)num5, (float)num7, (float)num6));
				}
			}
			if (vector != null)
			{
				this.SubsystemSky.MakeLightningStrike(vector.Value);
			}
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x00051ACF File Offset: 0x0004FCCF
		public static int GetTemperatureAdjustmentAtHeight(int y)
		{
			return (int)MathUtils.Round((y > 64) ? (-0.0008f * (float)MathUtils.Sqr(y - 64)) : (0.1f * (float)(64 - y)));
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x00051AF9 File Offset: 0x0004FCF9
		public static bool IsPlaceFrozen(int temperature, int y)
		{
			return temperature + SubsystemWeather.GetTemperatureAdjustmentAtHeight(y) <= 0;
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x00051B09 File Offset: 0x0004FD09
		public static bool ShaftHasSnowOnIce(int x, int z)
		{
			return MathUtils.Hash((uint)((x & 65535) | z << 16)) > 429496729U;
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x00051B24 File Offset: 0x0004FD24
		public void Draw(Camera camera, int drawOrder)
		{
			int num = (SettingsManager.VisibilityRange > 128) ? 9 : ((SettingsManager.VisibilityRange <= 64) ? 7 : 8);
			int num2 = num * num;
			Dictionary<Point2, PrecipitationShaftParticleSystem> activeShafts = this.GetActiveShafts(camera.GameWidget);
			byte b = (byte)(255f * MathUtils.Lerp(0.15f, 1f, this.SubsystemSky.SkyLightIntensity));
			this.RainColor = new Color(b, b, b);
			byte b2 = (byte)(255f * MathUtils.Lerp(0.15f, 1f, this.SubsystemSky.SkyLightIntensity));
			this.SnowColor = new Color(b2, b2, b2);
			Vector2 vector = new Vector2(camera.ViewPosition.X, camera.ViewPosition.Z);
			Point2 point = Terrain.ToCell(vector);
			Vector2? vector2 = null;
			this.m_lastShaftsUpdatePositions.TryGetValue(camera.GameWidget, out vector2);
			if (vector2 != null && Vector2.DistanceSquared(vector2.Value, vector) <= 1f)
			{
				return;
			}
			this.m_lastShaftsUpdatePositions[camera.GameWidget] = new Vector2?(vector);
			this.m_toRemove.Clear();
			foreach (PrecipitationShaftParticleSystem precipitationShaftParticleSystem in activeShafts.Values)
			{
				if (MathUtils.Sqr((float)precipitationShaftParticleSystem.Point.X + 0.5f - vector.X) + MathUtils.Sqr((float)precipitationShaftParticleSystem.Point.Y + 0.5f - vector.Y) > (float)num2 + 1f)
				{
					this.m_toRemove.Add(precipitationShaftParticleSystem);
				}
			}
			foreach (PrecipitationShaftParticleSystem precipitationShaftParticleSystem2 in this.m_toRemove)
			{
				if (this.m_subsystemParticles.ContainsParticleSystem(precipitationShaftParticleSystem2))
				{
					this.m_subsystemParticles.RemoveParticleSystem(precipitationShaftParticleSystem2);
				}
				activeShafts.Remove(precipitationShaftParticleSystem2.Point);
			}
			for (int i = point.X - num; i <= point.X + num; i++)
			{
				for (int j = point.Y - num; j <= point.Y + num; j++)
				{
					if (MathUtils.Sqr((float)i + 0.5f - vector.X) + MathUtils.Sqr((float)j + 0.5f - vector.Y) <= (float)num2)
					{
						Point2 point2 = new Point2(i, j);
						if (!activeShafts.ContainsKey(point2))
						{
							PrecipitationShaftParticleSystem precipitationShaftParticleSystem3 = new PrecipitationShaftParticleSystem(camera.GameWidget, this, this.m_random, point2, this.GetPrecipitationShaftInfo(point2.X, point2.Y).Type);
							this.m_subsystemParticles.AddParticleSystem(precipitationShaftParticleSystem3);
							activeShafts.Add(point2, precipitationShaftParticleSystem3);
						}
					}
				}
			}
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x00051E20 File Offset: 0x00050020
		public void Update(float dt)
		{
			if (this.m_subsystemGameInfo.TotalElapsedGameTime > this.m_precipitationEndTime)
			{
				if (this.m_precipitationEndTime == 0.0)
				{
					if (this.m_subsystemGameInfo.WorldSettings.StartingPositionMode == StartingPositionMode.Hard)
					{
						this.m_precipitationStartTime = this.m_subsystemGameInfo.TotalElapsedGameTime + (double)(60f * this.m_random.Float(2f, 3f));
						this.m_lightningIntensity = this.m_random.Float(0.5f, 1f);
					}
					else
					{
						this.m_precipitationStartTime = this.m_subsystemGameInfo.TotalElapsedGameTime + (double)(60f * this.m_random.Float(3f, 6f));
						this.m_lightningIntensity = this.m_random.Float(0.33f, 0.66f);
					}
				}
				else
				{
					this.m_precipitationStartTime = this.m_subsystemGameInfo.TotalElapsedGameTime + (double)(60f * this.m_random.Float(5f, 45f));
					this.m_lightningIntensity = ((this.m_random.Float(0f, 1f) < 0.5f) ? this.m_random.Float(0.33f, 1f) : 0f);
				}
				this.m_precipitationEndTime = this.m_precipitationStartTime + (double)(60f * this.m_random.Float(3f, 6f));
			}
			float num = (float)MathUtils.Max(0.0, MathUtils.Min(this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_precipitationStartTime, this.m_precipitationEndTime - this.m_subsystemGameInfo.TotalElapsedGameTime));
			this.GlobalPrecipitationIntensity = (this.m_subsystemGameInfo.WorldSettings.AreWeatherEffectsEnabled ? MathUtils.Saturate(num * 0.04f) : 0f);
			if (this.GlobalPrecipitationIntensity == 1f && this.SubsystemTime.PeriodicGameTimeEvent(1.0, 0.0))
			{
				TerrainChunk[] allocatedChunks = this.SubsystemTerrain.Terrain.AllocatedChunks;
				for (int i = 0; i < allocatedChunks.Length; i++)
				{
					TerrainChunk terrainChunk = allocatedChunks[this.m_random.Int(0, allocatedChunks.Length - 1)];
					if (terrainChunk.State >= TerrainChunkState.InvalidVertices1 && this.m_random.Bool(this.m_lightningIntensity * 0.0002f))
					{
						int num2 = terrainChunk.Origin.X + this.m_random.Int(0, 15);
						int num3 = terrainChunk.Origin.Y + this.m_random.Int(0, 15);
						Vector3? vector = null;
						for (int j = num2 - 8; j < num2 + 8; j++)
						{
							for (int k = num3 - 8; k < num3 + 8; k++)
							{
								int topHeight = this.SubsystemTerrain.Terrain.GetTopHeight(j, k);
								if (vector == null || (float)topHeight > vector.Value.Y)
								{
									vector = new Vector3?(new Vector3((float)j, (float)topHeight, (float)k));
								}
							}
						}
						if (vector != null)
						{
							this.SubsystemSky.MakeLightningStrike(vector.Value);
							return;
						}
					}
				}
			}
			if (Time.PeriodicEvent(0.5, 0.0))
			{
				float num4 = 0f;
				if (this.GlobalPrecipitationIntensity > 0f)
				{
					float num5 = 0f;
					foreach (Vector3 vector2 in this.m_subsystemAudio.ListenerPositions)
					{
						int num6 = Terrain.ToCell(vector2.X) - 5;
						int num7 = Terrain.ToCell(vector2.Z) - 5;
						int num8 = Terrain.ToCell(vector2.X) + 5;
						int num9 = Terrain.ToCell(vector2.Z) + 5;
						Vector3 vector3 = default(Vector3);
						for (int l = num6; l <= num8; l++)
						{
							for (int m = num7; m <= num9; m++)
							{
								PrecipitationShaftInfo precipitationShaftInfo = this.GetPrecipitationShaftInfo(l, m);
								if (precipitationShaftInfo.Type == PrecipitationType.Rain && precipitationShaftInfo.Intensity > 0f)
								{
									vector3.X = (float)l + 0.5f;
									vector3.Y = MathUtils.Max((float)precipitationShaftInfo.YLimit, vector2.Y);
									vector3.Z = (float)m + 0.5f;
									float num10 = vector3.X - vector2.X;
									float num11 = 8f * (vector3.Y - vector2.Y);
									float num12 = vector3.Z - vector2.Z;
									float distance = MathUtils.Sqrt(num10 * num10 + num11 * num11 + num12 * num12);
									num5 += this.m_subsystemAudio.CalculateVolume(distance, 1.5f, 2f) * precipitationShaftInfo.Intensity;
								}
							}
						}
					}
					num4 = MathUtils.Max(num4, num5);
				}
				this.m_targetRainSoundVolume = MathUtils.Saturate(1.5f * num4 / this.m_rainVolumeFactor);
			}
			this.m_rainSound.Volume = MathUtils.Saturate(MathUtils.Lerp(this.m_rainSound.Volume, SettingsManager.SoundsVolume * this.m_targetRainSoundVolume, 5f * dt));
			if (this.m_rainSound.Volume > AudioManager.MinAudibleVolume)
			{
				this.m_rainSound.Play();
				return;
			}
			this.m_rainSound.Pause();
		}

        // Token: 0x06000B0A RID: 2826 RVA: 0x000523BC File Offset: 0x000505BC
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.SubsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemBlocksScanner = base.Project.FindSubsystem<SubsystemBlocksScanner>(true);
			this.SubsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.SubsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_precipitationStartTime = valuesDictionary.GetValue<double>("WeatherStartTime");
			this.m_precipitationEndTime = valuesDictionary.GetValue<double>("WeatherEndTime");
			this.m_lightningIntensity = valuesDictionary.GetValue<float>("LightningIntensity");
			this.m_rainSound = this.m_subsystemAudio.CreateSound("Audio/Rain");
			this.m_rainSound.IsLooped = true;
			this.m_rainSound.Volume = 0f;
			this.RainSplashParticleSystem = new RainSplashParticleSystem();
			this.m_subsystemParticles.AddParticleSystem(this.RainSplashParticleSystem);
			this.SnowSplashParticleSystem = new SnowSplashParticleSystem();
			this.m_subsystemParticles.AddParticleSystem(this.SnowSplashParticleSystem);
			this.m_rainVolumeFactor = 0f;
			for (int i = -5; i <= 5; i++)
			{
				for (int j = -5; j <= 5; j++)
				{
					float distance = MathUtils.Sqrt((float)(i * i + j * j));
					this.m_rainVolumeFactor += this.m_subsystemAudio.CalculateVolume(distance, 1.5f, 2f);
				}
			}
			this.m_subsystemBlocksScanner.ScanningChunkCompleted += delegate(TerrainChunk chunk)
			{
				if (this.m_subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode == EnvironmentBehaviorMode.Living)
				{
					this.FreezeThawAndDepositSnow(chunk);
				}
			};
		}

        // Token: 0x06000B0B RID: 2827 RVA: 0x00052551 File Offset: 0x00050751
        public override void Save(ValuesDictionary valuesDictionary)
		{
			valuesDictionary.SetValue<double>("WeatherStartTime", this.m_precipitationStartTime);
			valuesDictionary.SetValue<double>("WeatherEndTime", this.m_precipitationEndTime);
			valuesDictionary.SetValue<float>("LightningIntensity", this.m_lightningIntensity);
		}

		// Token: 0x06000B0C RID: 2828 RVA: 0x00052588 File Offset: 0x00050788
		public Dictionary<Point2, PrecipitationShaftParticleSystem> GetActiveShafts(GameWidget gameWidget)
		{
			Dictionary<Point2, PrecipitationShaftParticleSystem> dictionary;
			if (!this.m_activeShafts.TryGetValue(gameWidget, out dictionary))
			{
				dictionary = new Dictionary<Point2, PrecipitationShaftParticleSystem>();
				this.m_activeShafts.Add(gameWidget, dictionary);
			}
			return dictionary;
		}

		// Token: 0x06000B0D RID: 2829 RVA: 0x000525BC File Offset: 0x000507BC
		public void FreezeThawAndDepositSnow(TerrainChunk chunk)
		{
			Terrain terrain = this.SubsystemTerrain.Terrain;
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					if (this.m_random.Int() % 2 != 0)
					{
						int topHeightFast = chunk.GetTopHeightFast(i, j);
						int cellValueFast = chunk.GetCellValueFast(i, topHeightFast, j);
						int num = Terrain.ExtractContents(cellValueFast);
						int num2 = chunk.Origin.X + i;
						int num3 = topHeightFast;
						int num4 = chunk.Origin.Y + j;
						PrecipitationShaftInfo precipitationShaftInfo = this.GetPrecipitationShaftInfo(num2, num4);
						if (precipitationShaftInfo.Type == PrecipitationType.Snow)
						{
							if (num == 18)
							{
								int cellContents = terrain.GetCellContents(num2 + 1, num3, num4);
								int cellContents2 = terrain.GetCellContents(num2 - 1, num3, num4);
								int cellContents3 = terrain.GetCellContents(num2, num3, num4 - 1);
								int cellContents4 = terrain.GetCellContents(num2, num3, num4 + 1);
								bool flag = cellContents != 18 && cellContents != 0;
								bool flag2 = cellContents2 != 18 && cellContents2 != 0;
								bool flag3 = cellContents3 != 18 && cellContents3 != 0;
								bool flag4 = cellContents4 != 18 && cellContents4 != 0;
								if (flag || flag2 || flag3 || flag4)
								{
									this.SubsystemTerrain.ChangeCell(num2, num3, num4, Terrain.MakeBlockValue(62), true);
								}
							}
							else if (precipitationShaftInfo.Intensity > 0.5f && SubsystemSnowBlockBehavior.CanSupportSnow(cellValueFast) && (num != 62 || SubsystemWeather.ShaftHasSnowOnIce(num2, num4)) && num3 + 1 < 255)
							{
								this.SubsystemTerrain.ChangeCell(num2, num3 + 1, num4, Terrain.MakeBlockValue(61), true);
							}
						}
						else if (num != 61)
						{
							if (num == 62)
							{
								this.SubsystemTerrain.DestroyCell(0, num2, num3, num4, 0, false, true);
							}
						}
						else
						{
							this.SubsystemTerrain.DestroyCell(0, num2, num3, num4, 0, true, true);
						}
					}
				}
			}
		}

		// Token: 0x040005F7 RID: 1527
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040005F8 RID: 1528
		public SubsystemBlocksScanner m_subsystemBlocksScanner;

		// Token: 0x040005F9 RID: 1529
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x040005FA RID: 1530
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040005FB RID: 1531
		public Game.Random m_random = new Game.Random();

		// Token: 0x040005FC RID: 1532
		public Dictionary<GameWidget, Dictionary<Point2, PrecipitationShaftParticleSystem>> m_activeShafts = new Dictionary<GameWidget, Dictionary<Point2, PrecipitationShaftParticleSystem>>();

		// Token: 0x040005FD RID: 1533
		public List<PrecipitationShaftParticleSystem> m_toRemove = new List<PrecipitationShaftParticleSystem>();

		// Token: 0x040005FE RID: 1534
		public Dictionary<GameWidget, Vector2?> m_lastShaftsUpdatePositions = new Dictionary<GameWidget, Vector2?>();

		// Token: 0x040005FF RID: 1535
		public float m_targetRainSoundVolume;

		// Token: 0x04000600 RID: 1536
		public double m_precipitationStartTime;

		// Token: 0x04000601 RID: 1537
		public double m_precipitationEndTime;

		// Token: 0x04000602 RID: 1538
		public float m_lightningIntensity;

		// Token: 0x04000603 RID: 1539
		public const int m_rainSoundRadius = 5;

		// Token: 0x04000604 RID: 1540
		public float m_rainVolumeFactor;

		// Token: 0x04000605 RID: 1541
		public Sound m_rainSound;

		// Token: 0x04000606 RID: 1542
		public static int[] m_drawOrders = new int[]
		{
			50
		};
	}
}
