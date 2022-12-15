using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000190 RID: 400
	public class SubsystemMetersBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000926 RID: 2342 RVA: 0x0003EC1F File Offset: 0x0003CE1F
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					120,
					121
				};
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000927 RID: 2343 RVA: 0x0003EC31 File Offset: 0x0003CE31
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000928 RID: 2344 RVA: 0x0003EC34 File Offset: 0x0003CE34
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			Point3 point = CellFace.FaceToPoint3(Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z)));
			int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x - point.X, y - point.Y, z - point.Z);
			if (BlocksManager.Blocks[cellContents].IsTransparent)
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x0003ECA8 File Offset: 0x0003CEA8
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.AddMeter(value, x, y, z);
		}

		// Token: 0x0600092A RID: 2346 RVA: 0x0003ECB6 File Offset: 0x0003CEB6
		public override void OnBlockRemoved(int value, int oldValue, int x, int y, int z)
		{
			this.RemoveMeter(oldValue, x, y, z);
		}

		// Token: 0x0600092B RID: 2347 RVA: 0x0003ECC4 File Offset: 0x0003CEC4
		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			this.RemoveMeter(oldValue, x, y, z);
			this.AddMeter(value, x, y, z);
		}

		// Token: 0x0600092C RID: 2348 RVA: 0x0003ECDE File Offset: 0x0003CEDE
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			this.AddMeter(value, x, y, z);
		}

		// Token: 0x0600092D RID: 2349 RVA: 0x0003ECEC File Offset: 0x0003CEEC
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			List<Point3> list = new List<Point3>();
			foreach (Point3 point in this.m_thermometersByPoint.Keys)
			{
				if (point.X >= chunk.Origin.X && point.X < chunk.Origin.X + 16 && point.Z >= chunk.Origin.Y && point.Z < chunk.Origin.Y + 16)
				{
					list.Add(point);
				}
			}
			foreach (Point3 key in list)
			{
				this.m_thermometersByPoint.Remove(key);
			}
		}

        // Token: 0x0600092E RID: 2350 RVA: 0x0003EDE4 File Offset: 0x0003CFE4
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemWeather = base.Project.FindSubsystem<SubsystemWeather>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x0003EE24 File Offset: 0x0003D024
		public void Update(float dt)
		{
			if (this.m_thermometersToSimulateIndex < this.m_thermometersToSimulate.Count)
			{
				double period = MathUtils.Max(5.0 / (double)this.m_thermometersToSimulate.Count, 1.0);
				if (this.m_subsystemTime.PeriodicGameTimeEvent(period, 0.0))
				{
					Point3 point = this.m_thermometersToSimulate.Array[this.m_thermometersToSimulateIndex];
					this.SimulateThermometer(point.X, point.Y, point.Z, true);
					this.m_thermometersToSimulateIndex++;
					return;
				}
			}
			else if (this.m_thermometersByPoint.Count > 0)
			{
				this.m_thermometersToSimulateIndex = 0;
				this.m_thermometersToSimulate.Clear();
				this.m_thermometersToSimulate.AddRange(this.m_thermometersByPoint.Keys);
			}
		}

		// Token: 0x06000930 RID: 2352 RVA: 0x0003EEF8 File Offset: 0x0003D0F8
		public int GetThermometerReading(int x, int y, int z)
		{
			int result = 0;
			this.m_thermometersByPoint.TryGetValue(new Point3(x, y, z), out result);
			return result;
		}

		// Token: 0x06000931 RID: 2353 RVA: 0x0003EF20 File Offset: 0x0003D120
		public void CalculateTemperature(int x, int y, int z, float meterTemperature, float meterInsulation, out float temperature, out float temperatureFlux)
		{
			this.m_toVisit.Clear();
			for (int i = 0; i < this.m_visited.Length; i++)
			{
				this.m_visited[i] = 0;
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			this.m_toVisit.Add(133152);
			for (int j = 0; j < this.m_toVisit.Count; j++)
			{
				int num7 = this.m_toVisit.Array[j];
				if ((this.m_visited[num7 / 32] & 1 << num7) == 0)
				{
					this.m_visited[num7 / 32] |= 1 << num7;
					int num8 = (num7 & 63) - 32;
					int num9 = (num7 >> 6 & 63) - 32;
					int num10 = (num7 >> 12 & 63) - 32;
					int num11 = num8 + x;
					int num12 = num9 + y;
					int num13 = num10 + z;
					Terrain terrain = base.SubsystemTerrain.Terrain;
					TerrainChunk chunkAtCell = terrain.GetChunkAtCell(num11, num13);
					if (chunkAtCell != null && num12 >= 0 && num12 < 256)
					{
						int x2 = num11 & 15;
						int y2 = num12;
						int z2 = num13 & 15;
						int cellValueFast = chunkAtCell.GetCellValueFast(x2, y2, z2);
						int num14 = Terrain.ExtractContents(cellValueFast);
						Block block = BlocksManager.Blocks[num14];
						float heat = SubsystemMetersBlockBehavior.GetHeat(cellValueFast);
						if (heat > 0f)
						{
							int num15 = MathUtils.Abs(num8) + MathUtils.Abs(num9) + MathUtils.Abs(num10);
							int num16 = (num15 <= 0) ? 1 : (4 * num15 * num15 + 2);
							float num17 = 1f / (float)num16;
							num5 += num17 * 36f * heat;
							num6 += num17;
						}
						else if (block.IsHeatBlocker(cellValueFast))
						{
							int num18 = MathUtils.Abs(num8) + MathUtils.Abs(num9) + MathUtils.Abs(num10);
							int num19 = (num18 <= 0) ? 1 : (4 * num18 * num18 + 2);
							float num20 = 1f / (float)num19;
							float num21 = (float)terrain.SeasonTemperature;
							float num22 = (float)SubsystemWeather.GetTemperatureAdjustmentAtHeight(y2);
							float num23 = (block is WaterBlock) ? (MathUtils.Max((float)chunkAtCell.GetTemperatureFast(x2, z2) + num21 - 6f, 0f) + num22) : ((!(block is IceBlock)) ? ((float)chunkAtCell.GetTemperatureFast(x2, z2) + num21 + num22) : (0f + num21 + num22));
							num += num20 * num23;
							num2 += num20;
						}
						else if (y >= chunkAtCell.GetTopHeightFast(x2, z2))
						{
							int num24 = MathUtils.Abs(num8) + MathUtils.Abs(num9) + MathUtils.Abs(num10);
							int num25 = (num24 <= 0) ? 1 : (4 * num24 * num24 + 2);
							float num26 = 1f / (float)num25;
							PrecipitationShaftInfo precipitationShaftInfo = this.m_subsystemWeather.GetPrecipitationShaftInfo(x, z);
							float num27 = (float)terrain.SeasonTemperature;
							float num28 = (y >= precipitationShaftInfo.YLimit) ? MathUtils.Lerp(0f, -2f, precipitationShaftInfo.Intensity) : 0f;
							float num29 = MathUtils.Lerp(-6f, 0f, this.m_subsystemSky.SkyLightIntensity);
							float num30 = (float)SubsystemWeather.GetTemperatureAdjustmentAtHeight(y2);
							num3 += num26 * ((float)chunkAtCell.GetTemperatureFast(x2, z2) + num27 + num28 + num29 + num30);
							num4 += num26;
						}
						else if (this.m_toVisit.Count < 4090)
						{
							if (num8 > -30)
							{
								this.m_toVisit.Add(num7 - 1);
							}
							if (num8 < 30)
							{
								this.m_toVisit.Add(num7 + 1);
							}
							if (num9 > -30)
							{
								this.m_toVisit.Add(num7 - 64);
							}
							if (num9 < 30)
							{
								this.m_toVisit.Add(num7 + 64);
							}
							if (num10 > -30)
							{
								this.m_toVisit.Add(num7 - 4096);
							}
							if (num10 < 30)
							{
								this.m_toVisit.Add(num7 + 4096);
							}
						}
					}
				}
			}
			float num31 = 0f;
			for (int k = -7; k <= 7; k++)
			{
				for (int l = -7; l <= 7; l++)
				{
					TerrainChunk chunkAtCell2 = base.SubsystemTerrain.Terrain.GetChunkAtCell(x + k, z + l);
					if (chunkAtCell2 != null && chunkAtCell2.State >= TerrainChunkState.InvalidVertices1)
					{
						for (int m = -7; m <= 7; m++)
						{
							int num32 = k * k + m * m + l * l;
							if (num32 <= 49 && num32 > 0)
							{
								int x3 = x + k & 15;
								int num33 = y + m;
								int z3 = z + l & 15;
								if (num33 >= 0 && num33 < 256)
								{
									float heat2 = SubsystemMetersBlockBehavior.GetHeat(chunkAtCell2.GetCellValueFast(x3, num33, z3));
									if (heat2 > 0f)
									{
										if (base.SubsystemTerrain.Raycast(new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f, 0.75f, 0.5f), new Vector3((float)(x + k), (float)(y + m), (float)(z + l)) + new Vector3(0.5f, 0.75f, 0.5f), false, true, delegate(int raycastValue, float d)
										{
											Block block2 = BlocksManager.Blocks[Terrain.ExtractContents(raycastValue)];
											return block2.IsCollidable && !block2.IsTransparent;
										}) == null)
										{
											num31 += heat2 * 3f / (float)(num32 + 2);
										}
									}
								}
							}
						}
					}
				}
			}
			float num34 = 0f;
			float num35 = 0f;
			if (num31 > 0f)
			{
				float num36 = 3f * num31;
				num34 += 35f * num36;
				num35 += num36;
			}
			if (num2 > 0f)
			{
				float num37 = 1f;
				num34 += num / num2 * num37;
				num35 += num37;
			}
			if (num4 > 0f)
			{
				float num38 = 4f * MathUtils.Pow(num4, 0.25f);
				num34 += num3 / num4 * num38;
				num35 += num38;
			}
			if (num6 > 0f)
			{
				float num39 = 1.5f * MathUtils.Pow(num6, 0.25f);
				num34 += num5 / num6 * num39;
				num35 += num39;
			}
			if (meterInsulation > 0f)
			{
				num34 += meterTemperature * meterInsulation;
				num35 += meterInsulation;
			}
			temperature = ((num35 > 0f) ? (num34 / num35) : meterTemperature);
			temperatureFlux = num35 - meterInsulation;
		}

		// Token: 0x06000932 RID: 2354 RVA: 0x0003F5A0 File Offset: 0x0003D7A0
		public static float GetHeat(int value)
		{
			int num = Terrain.ExtractContents(value);
			return BlocksManager.Blocks[num].GetHeat(value);
		}

		// Token: 0x06000933 RID: 2355 RVA: 0x0003F5C4 File Offset: 0x0003D7C4
		public void SimulateThermometer(int x, int y, int z, bool invalidateTerrainOnChange)
		{
			Point3 key = new Point3(x, y, z);
			if (!this.m_thermometersByPoint.ContainsKey(key))
			{
				return;
			}
			int num = this.m_thermometersByPoint[key];
			float x2;
			float num2;
			this.CalculateTemperature(x, y, z, 0f, 0f, out x2, out num2);
			int num3 = MathUtils.Clamp((int)MathUtils.Round(x2), 0, 15);
			if (num3 == num)
			{
				return;
			}
			this.m_thermometersByPoint[new Point3(x, y, z)] = num3;
			if (invalidateTerrainOnChange)
			{
				TerrainChunk chunkAtCell = base.SubsystemTerrain.Terrain.GetChunkAtCell(x, z);
				if (chunkAtCell != null)
				{
					base.SubsystemTerrain.TerrainUpdater.DowngradeChunkNeighborhoodState(chunkAtCell.Coords, 0, TerrainChunkState.InvalidVertices1, true);
				}
			}
		}

		// Token: 0x06000934 RID: 2356 RVA: 0x0003F66D File Offset: 0x0003D86D
		public void AddMeter(int value, int x, int y, int z)
		{
			if (Terrain.ExtractContents(value) == 120)
			{
				this.m_thermometersByPoint.Add(new Point3(x, y, z), 0);
				this.SimulateThermometer(x, y, z, false);
			}
		}

		// Token: 0x06000935 RID: 2357 RVA: 0x0003F699 File Offset: 0x0003D899
		public void RemoveMeter(int value, int x, int y, int z)
		{
			this.m_thermometersByPoint.Remove(new Point3(x, y, z));
		}

		// Token: 0x040004C6 RID: 1222
		public SubsystemTime m_subsystemTime;

		// Token: 0x040004C7 RID: 1223
		public SubsystemWeather m_subsystemWeather;

		// Token: 0x040004C8 RID: 1224
		public SubsystemSky m_subsystemSky;

		// Token: 0x040004C9 RID: 1225
		public Dictionary<Point3, int> m_thermometersByPoint = new Dictionary<Point3, int>();

		// Token: 0x040004CA RID: 1226
		public DynamicArray<Point3> m_thermometersToSimulate = new DynamicArray<Point3>();

		// Token: 0x040004CB RID: 1227
		public int m_thermometersToSimulateIndex;

		// Token: 0x040004CC RID: 1228
		public const int m_diameterBits = 6;

		// Token: 0x040004CD RID: 1229
		public const int m_diameter = 64;

		// Token: 0x040004CE RID: 1230
		public const int m_diameterMask = 63;

		// Token: 0x040004CF RID: 1231
		public const int m_radius = 32;

		// Token: 0x040004D0 RID: 1232
		public DynamicArray<int> m_toVisit = new DynamicArray<int>();

		// Token: 0x040004D1 RID: 1233
		public int[] m_visited = new int[8192];
	}
}
