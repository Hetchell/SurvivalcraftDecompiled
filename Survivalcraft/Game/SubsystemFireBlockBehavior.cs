using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200017C RID: 380
	public class SubsystemFireBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000867 RID: 2151 RVA: 0x00038A22 File Offset: 0x00036C22
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000868 RID: 2152 RVA: 0x00038A25 File Offset: 0x00036C25
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					104
				};
			}
		}

		// Token: 0x06000869 RID: 2153 RVA: 0x00038A34 File Offset: 0x00036C34
		public bool IsCellOnFire(int x, int y, int z)
		{
			for (int i = 0; i < 4; i++)
			{
				Point3 point = CellFace.FaceToPoint3(i);
				int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x + point.X, y + point.Y, z + point.Z);
				if (Terrain.ExtractContents(cellValue) == 104)
				{
					bool flag = Terrain.ExtractData(cellValue) != 0;
					int num = CellFace.OppositeFace(i);
					if (((flag ? 1 : 0) & 1 << num) != 0)
					{
						return true;
					}
				}
			}
			int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z);
			return Terrain.ExtractContents(cellValue2) == 104 && Terrain.ExtractData(cellValue2) == 0;
		}

		// Token: 0x0600086A RID: 2154 RVA: 0x00038ACC File Offset: 0x00036CCC
		public bool SetCellOnFire(int x, int y, int z, float fireExpandability)
		{
			int value = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(value);
			Block blockTarget = BlocksManager.Blocks[num];
            if (blockTarget.FireDuration == 0f)
			{
				return false;
			}
			bool result = false;
			for (int i = 0; i < 5; i++)
			{
				Point3 point = CellFace.FaceToPoint3(i);
				int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x + point.X, y + point.Y, z + point.Z);
				int num2 = Terrain.ExtractContents(cellValue);
				if (num2 == 0 || num2 == 104 || num2 == 61)
				{
					int num3 = (num2 == 104) ? Terrain.ExtractData(cellValue) : 0;
					int num4 = CellFace.OppositeFace(i);
					num3 |= (1 << num4 & 15);
					value = Terrain.ReplaceData(Terrain.ReplaceContents(0, 104), num3);
					this.AddFire(x + point.X, y + point.Y, z + point.Z, fireExpandability);
					base.SubsystemTerrain.ChangeCell(x + point.X, y + point.Y, z + point.Z, value, true);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x00038BF8 File Offset: 0x00036DF8
		public void Update(float dt)
		{
			if (this.m_firePointsCopy.Count == 0)
			{
				this.m_firePointsCopy.Count += this.m_fireData.Count;
				this.m_fireData.Keys.CopyTo(this.m_firePointsCopy.Array, 0);
				this.m_copyIndex = 0;
				this.m_lastScanDuration = (float)(this.m_subsystemTime.GameTime - this.m_lastScanTime);
				this.m_lastScanTime = this.m_subsystemTime.GameTime;
				if (this.m_firePointsCopy.Count == 0)
				{
					this.m_fireSoundVolume = 0f;
				}
			}
			if (this.m_firePointsCopy.Count > 0)
			{
				float num = MathUtils.Min(1f * dt * (float)this.m_firePointsCopy.Count + this.m_remainderToScan, 50f);
				int num2 = (int)num;
				this.m_remainderToScan = num - (float)num2;
				int num3 = MathUtils.Min(this.m_copyIndex + num2, this.m_firePointsCopy.Count);
				while (this.m_copyIndex < num3)
				{
					SubsystemFireBlockBehavior.FireData fireData;
					if (this.m_fireData.TryGetValue(this.m_firePointsCopy.Array[this.m_copyIndex], out fireData))
					{
						int x = fireData.Point.X;
						int y = fireData.Point.Y;
						int z = fireData.Point.Z;
						int num4 = Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z));
						this.m_fireSoundIntensity += 1f / (this.m_subsystemAudio.CalculateListenerDistanceSquared(new Vector3((float)x, (float)y, (float)z)) + 0.01f);
						if ((num4 & 1) != 0)
						{
							fireData.Time0 -= this.m_lastScanDuration;
							if (fireData.Time0 <= 0f)
							{
								this.QueueBurnAway(x, y, z + 1, fireData.FireExpandability * 0.85f);
							}
							foreach (KeyValuePair<Point3, float> keyValuePair in this.m_expansionProbabilities)
							{
								if (this.m_random.Float(0f, 1f) < keyValuePair.Value * this.m_lastScanDuration * fireData.FireExpandability)
								{
									this.m_toExpand[new Point3(x + keyValuePair.Key.X, y + keyValuePair.Key.Y, z + 1 + keyValuePair.Key.Z)] = fireData.FireExpandability * 0.85f;
								}
							}
						}
						if ((num4 & 2) != 0)
						{
							fireData.Time1 -= this.m_lastScanDuration;
							if (fireData.Time1 <= 0f)
							{
								this.QueueBurnAway(x + 1, y, z, fireData.FireExpandability * 0.85f);
							}
							foreach (KeyValuePair<Point3, float> keyValuePair2 in this.m_expansionProbabilities)
							{
								if (this.m_random.Float(0f, 1f) < keyValuePair2.Value * this.m_lastScanDuration * fireData.FireExpandability)
								{
									this.m_toExpand[new Point3(x + 1 + keyValuePair2.Key.X, y + keyValuePair2.Key.Y, z + keyValuePair2.Key.Z)] = fireData.FireExpandability * 0.85f;
								}
							}
						}
						if ((num4 & 4) != 0)
						{
							fireData.Time2 -= this.m_lastScanDuration;
							if (fireData.Time2 <= 0f)
							{
								this.QueueBurnAway(x, y, z - 1, fireData.FireExpandability * 0.85f);
							}
							foreach (KeyValuePair<Point3, float> keyValuePair3 in this.m_expansionProbabilities)
							{
								if (this.m_random.Float(0f, 1f) < keyValuePair3.Value * this.m_lastScanDuration * fireData.FireExpandability)
								{
									this.m_toExpand[new Point3(x + keyValuePair3.Key.X, y + keyValuePair3.Key.Y, z - 1 + keyValuePair3.Key.Z)] = fireData.FireExpandability * 0.85f;
								}
							}
						}
						if ((num4 & 8) != 0)
						{
							fireData.Time3 -= this.m_lastScanDuration;
							if (fireData.Time3 <= 0f)
							{
								this.QueueBurnAway(x - 1, y, z, fireData.FireExpandability * 0.85f);
							}
							foreach (KeyValuePair<Point3, float> keyValuePair4 in this.m_expansionProbabilities)
							{
								if (this.m_random.Float(0f, 1f) < keyValuePair4.Value * this.m_lastScanDuration * fireData.FireExpandability)
								{
									this.m_toExpand[new Point3(x - 1 + keyValuePair4.Key.X, y + keyValuePair4.Key.Y, z + keyValuePair4.Key.Z)] = fireData.FireExpandability * 0.85f;
								}
							}
						}
						if (num4 == 0)
						{
							fireData.Time5 -= this.m_lastScanDuration;
							if (fireData.Time5 <= 0f)
							{
								this.QueueBurnAway(x, y - 1, z, fireData.FireExpandability * 0.85f);
							}
						}
					}
					this.m_copyIndex++;
				}
				if (this.m_copyIndex >= this.m_firePointsCopy.Count)
				{
					this.m_fireSoundVolume = 0.75f * this.m_fireSoundIntensity;
					this.m_firePointsCopy.Clear();
					this.m_fireSoundIntensity = 0f;
				}
			}
			if (this.m_subsystemTime.PeriodicGameTimeEvent(5.0, 0.0))
			{
				int num5 = 0;
				int num6 = 0;
				foreach (KeyValuePair<Point3, float> keyValuePair5 in this.m_toBurnAway)
				{
					Point3 key = keyValuePair5.Key;
					float value = keyValuePair5.Value;
					base.SubsystemTerrain.ChangeCell(key.X, key.Y, key.Z, Terrain.ReplaceContents(0, 0), true);
					if (value > 0.25f)
					{
						for (int i = 0; i < 5; i++)
						{
							Point3 point = CellFace.FaceToPoint3(i);
							this.SetCellOnFire(key.X + point.X, key.Y + point.Y, key.Z + point.Z, value);
						}
					}
					float num7 = this.m_subsystemViews.CalculateDistanceFromNearestView(new Vector3(key));
					if (num5 < 15 && num7 < 24f)
					{
						this.m_subsystemParticles.AddParticleSystem(new BurntDebrisParticleSystem(base.SubsystemTerrain, key.X, key.Y, key.Z));
						num5++;
					}
					if (num6 < 4 && num7 < 16f)
					{
						this.m_subsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, this.m_random.Float(-0.25f, 0.25f), new Vector3((float)key.X, (float)key.Y, (float)key.Z), 3f, true);
						num6++;
					}
				}
				foreach (KeyValuePair<Point3, float> keyValuePair6 in this.m_toExpand)
				{
					this.SetCellOnFire(keyValuePair6.Key.X, keyValuePair6.Key.Y, keyValuePair6.Key.Z, keyValuePair6.Value);
				}
				this.m_toBurnAway.Clear();
				this.m_toExpand.Clear();
			}
			this.m_subsystemAmbientSounds.FireSoundVolume = MathUtils.Max(this.m_subsystemAmbientSounds.FireSoundVolume, this.m_fireSoundVolume);
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x000394C4 File Offset: 0x000376C4
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int num = Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z));
			if ((num & 1) != 0 && BlocksManager.Blocks[base.SubsystemTerrain.Terrain.GetCellContents(x, y, z + 1)].FireDuration == 0f)
			{
				num &= -2;
			}
			if ((num & 2) != 0 && BlocksManager.Blocks[base.SubsystemTerrain.Terrain.GetCellContents(x + 1, y, z)].FireDuration == 0f)
			{
				num &= -3;
			}
			if ((num & 4) != 0 && BlocksManager.Blocks[base.SubsystemTerrain.Terrain.GetCellContents(x, y, z - 1)].FireDuration == 0f)
			{
				num &= -5;
			}
			if ((num & 8) != 0 && BlocksManager.Blocks[base.SubsystemTerrain.Terrain.GetCellContents(x - 1, y, z)].FireDuration == 0f)
			{
				num &= -9;
			}
			SubsystemFireBlockBehavior.FireData fireData;
			if (this.m_fireData.TryGetValue(new Point3(x, y, z), out fireData))
			{
				if ((num & 1) != 0 && neighborX == x && neighborY == y && neighborZ == z + 1)
				{
					this.InitializeFireDataTime(fireData, 0);
				}
				if ((num & 2) != 0 && neighborX == x + 1 && neighborY == y && neighborZ == z)
				{
					this.InitializeFireDataTime(fireData, 1);
				}
				if ((num & 4) != 0 && neighborX == x && neighborY == y && neighborZ == z - 1)
				{
					this.InitializeFireDataTime(fireData, 2);
				}
				if ((num & 8) != 0 && neighborX == x - 1 && neighborY == y && neighborZ == z)
				{
					this.InitializeFireDataTime(fireData, 3);
				}
				if (num == 0 && neighborX == x && neighborY == y - 1 && neighborZ == z)
				{
					this.InitializeFireDataTime(fireData, 5);
				}
			}
			int contents = 104;
			if (num == 0 && BlocksManager.Blocks[base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z)].FireDuration == 0f)
			{
				contents = 0;
			}
			int value = Terrain.ReplaceData(Terrain.ReplaceContents(0, contents), num);
			base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x000396A9 File Offset: 0x000378A9
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.AddFire(x, y, z, 1f);
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x000396BB File Offset: 0x000378BB
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			this.RemoveFire(x, y, z);
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x000396C8 File Offset: 0x000378C8
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			this.AddFire(x, y, z, 1f);
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x000396DC File Offset: 0x000378DC
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			List<Point3> list = new List<Point3>();
			foreach (Point3 point in this.m_fireData.Keys)
			{
				if (point.X >= chunk.Origin.X && point.X < chunk.Origin.X + 16 && point.Z >= chunk.Origin.Y && point.Z < chunk.Origin.Y + 16)
				{
					list.Add(point);
				}
			}
			foreach (Point3 point2 in list)
			{
				this.RemoveFire(point2.X, point2.Y, point2.Z);
			}
		}

        // Token: 0x06000871 RID: 2161 RVA: 0x000397E0 File Offset: 0x000379E0
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemViews = base.Project.FindSubsystem<SubsystemGameWidgets>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemAmbientSounds = base.Project.FindSubsystem<SubsystemAmbientSounds>(true);
			for (int i = -2; i <= 2; i++)
			{
				for (int j = -1; j <= 2; j++)
				{
					for (int k = -2; k <= 2; k++)
					{
						if (i != 0 || j != 0 || k != 0)
						{
							float num = (j < 0) ? 1.5f : 2.5f;
							if (MathUtils.Sqrt((float)(i * i + j * j + k * k)) <= num)
							{
								float num2 = MathUtils.Sqrt((float)(i * i + k * k));
								float num3 = (j > 0) ? (0.5f * (float)j) : ((float)(-(float)j));
								this.m_expansionProbabilities[new Point3(i, j, k)] = 0.02f / (num2 + num3);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000872 RID: 2162 RVA: 0x000398EC File Offset: 0x00037AEC
		public void AddFire(int x, int y, int z, float expandability)
		{
			Point3 point = new Point3(x, y, z);
			if (!this.m_fireData.ContainsKey(point))
			{
				SubsystemFireBlockBehavior.FireData fireData = new SubsystemFireBlockBehavior.FireData();
				fireData.Point = point;
				fireData.FireExpandability = expandability;
				this.InitializeFireDataTimes(fireData);
				this.m_fireData[point] = fireData;
			}
		}

		// Token: 0x06000873 RID: 2163 RVA: 0x0003993C File Offset: 0x00037B3C
		public void RemoveFire(int x, int y, int z)
		{
			Point3 key = new Point3(x, y, z);
			this.m_fireData.Remove(key);
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x00039960 File Offset: 0x00037B60
		public void InitializeFireDataTimes(SubsystemFireBlockBehavior.FireData fireData)
		{
			this.InitializeFireDataTime(fireData, 0);
			this.InitializeFireDataTime(fireData, 1);
			this.InitializeFireDataTime(fireData, 2);
			this.InitializeFireDataTime(fireData, 3);
			this.InitializeFireDataTime(fireData, 5);
		}

		// Token: 0x06000875 RID: 2165 RVA: 0x0003998C File Offset: 0x00037B8C
		public void InitializeFireDataTime(SubsystemFireBlockBehavior.FireData fireData, int face)
		{
			Point3 point = CellFace.FaceToPoint3(face);
			int x = fireData.Point.X + point.X;
			int y = fireData.Point.Y + point.Y;
			int z = fireData.Point.Z + point.Z;
			int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y, z);
			Block block = BlocksManager.Blocks[cellContents];
			switch (face)
			{
			case 0:
				fireData.Time0 = block.FireDuration * this.m_random.Float(0.75f, 1.25f);
				return;
			case 1:
				fireData.Time1 = block.FireDuration * this.m_random.Float(0.75f, 1.25f);
				return;
			case 2:
				fireData.Time2 = block.FireDuration * this.m_random.Float(0.75f, 1.25f);
				return;
			case 3:
				fireData.Time3 = block.FireDuration * this.m_random.Float(0.75f, 1.25f);
				return;
			case 4:
				break;
			case 5:
				fireData.Time5 = block.FireDuration * this.m_random.Float(0.75f, 1.25f);
				break;
			default:
				return;
			}
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x00039ACC File Offset: 0x00037CCC
		public void QueueBurnAway(int x, int y, int z, float expandability)
		{
			Point3 key = new Point3(x, y, z);
			if (!this.m_toBurnAway.ContainsKey(key))
			{
				this.m_toBurnAway.Add(key, expandability);
			}
		}

		// Token: 0x0400046D RID: 1133
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400046E RID: 1134
		public SubsystemGameWidgets m_subsystemViews;

		// Token: 0x0400046F RID: 1135
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000470 RID: 1136
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000471 RID: 1137
		public SubsystemAmbientSounds m_subsystemAmbientSounds;

		// Token: 0x04000472 RID: 1138
		public Dictionary<Point3, float> m_expansionProbabilities = new Dictionary<Point3, float>();

		// Token: 0x04000473 RID: 1139
		public Dictionary<Point3, SubsystemFireBlockBehavior.FireData> m_fireData = new Dictionary<Point3, SubsystemFireBlockBehavior.FireData>();

		// Token: 0x04000474 RID: 1140
		public DynamicArray<Point3> m_firePointsCopy = new DynamicArray<Point3>();

		// Token: 0x04000475 RID: 1141
		public Dictionary<Point3, float> m_toBurnAway = new Dictionary<Point3, float>();

		// Token: 0x04000476 RID: 1142
		public Dictionary<Point3, float> m_toExpand = new Dictionary<Point3, float>();

		// Token: 0x04000477 RID: 1143
		public int m_copyIndex;

		// Token: 0x04000478 RID: 1144
		public float m_remainderToScan;

		// Token: 0x04000479 RID: 1145
		public double m_lastScanTime;

		// Token: 0x0400047A RID: 1146
		public float m_lastScanDuration;

		// Token: 0x0400047B RID: 1147
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400047C RID: 1148
		public float m_fireSoundVolume;

		// Token: 0x0400047D RID: 1149
		public float m_fireSoundIntensity;

		// Token: 0x02000422 RID: 1058
		public class FireData
		{
			// Token: 0x0400157C RID: 5500
			public Point3 Point;

			// Token: 0x0400157D RID: 5501
			public float Time0;

			// Token: 0x0400157E RID: 5502
			public float Time1;

			// Token: 0x0400157F RID: 5503
			public float Time2;

			// Token: 0x04001580 RID: 5504
			public float Time3;

			// Token: 0x04001581 RID: 5505
			public float Time5;

			// Token: 0x04001582 RID: 5506
			public float FireExpandability;
		}
	}
}
