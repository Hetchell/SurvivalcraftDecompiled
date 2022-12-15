using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Engine;
using Engine.Serialization;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001AB RID: 427
	public class SubsystemSpawn : Subsystem, IUpdateable
	{
		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000A66 RID: 2662 RVA: 0x0004D232 File Offset: 0x0004B432
		public Dictionary<ComponentSpawn, bool>.KeyCollection Spawns
		{
			get
			{
				return this.m_spawns.Keys;
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000A67 RID: 2663 RVA: 0x0004D23F File Offset: 0x0004B43F
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000A68 RID: 2664 RVA: 0x0004D244 File Offset: 0x0004B444
		// (remove) Token: 0x06000A69 RID: 2665 RVA: 0x0004D27C File Offset: 0x0004B47C
		public event Action<SpawnChunk> SpawningChunk;

		// Token: 0x06000A6A RID: 2666 RVA: 0x0004D2B4 File Offset: 0x0004B4B4
		public SpawnChunk GetSpawnChunk(Point2 point)
		{
			SpawnChunk result;
			this.m_chunks.TryGetValue(point, out result);
			return result;
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x0004D2D4 File Offset: 0x0004B4D4
		public void Update(float dt)
		{
			if (this.m_subsystemTime.GameTime >= this.m_nextDiscardOldChunksTime)
			{
				this.m_nextDiscardOldChunksTime = this.m_subsystemTime.GameTime + 60.0;
				this.DiscardOldChunks();
			}
			if (this.m_subsystemTime.GameTime >= this.m_nextVisitedTime)
			{
				this.m_nextVisitedTime = this.m_subsystemTime.GameTime + 5.0;
				this.UpdateLastVisitedTime();
			}
			if (this.m_subsystemTime.GameTime >= this.m_nextChunkSpawnTime)
			{
				this.m_nextChunkSpawnTime = this.m_subsystemTime.GameTime + 4.0;
				this.SpawnChunks();
			}
			if (this.m_subsystemTime.GameTime >= this.m_nextDespawnTime)
			{
				this.m_nextDespawnTime = this.m_subsystemTime.GameTime + 2.0;
				this.DespawnChunks();
			}
		}

        // Token: 0x06000A6C RID: 2668 RVA: 0x0004D3B4 File Offset: 0x0004B5B4
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			this.m_subsystemViews = base.Project.FindSubsystem<SubsystemGameWidgets>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			foreach (KeyValuePair<string, object> keyValuePair in valuesDictionary.GetValue<ValuesDictionary>("Chunks"))
			{
				ValuesDictionary valuesDictionary2 = (ValuesDictionary)keyValuePair.Value;
				SpawnChunk spawnChunk = new SpawnChunk();
				spawnChunk.Point = HumanReadableConverter.ConvertFromString<Point2>(keyValuePair.Key);
				spawnChunk.IsSpawned = valuesDictionary2.GetValue<bool>("IsSpawned");
				spawnChunk.LastVisitedTime = new double?(valuesDictionary2.GetValue<double>("LastVisitedTime"));
				string value = valuesDictionary2.GetValue<string>("SpawnsData", string.Empty);
				if (!string.IsNullOrEmpty(value))
				{
					this.LoadSpawnsData(value, spawnChunk.SpawnsData);
				}
				this.m_chunks[spawnChunk.Point] = spawnChunk;
			}
		}

        // Token: 0x06000A6D RID: 2669 RVA: 0x0004D4E8 File Offset: 0x0004B6E8
        public override void Save(ValuesDictionary valuesDictionary)
		{
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Chunks", valuesDictionary2);
			foreach (SpawnChunk spawnChunk in this.m_chunks.Values)
			{
				if (spawnChunk.LastVisitedTime != null)
				{
					ValuesDictionary valuesDictionary3 = new ValuesDictionary();
					valuesDictionary2.SetValue<ValuesDictionary>(HumanReadableConverter.ConvertToString(spawnChunk.Point), valuesDictionary3);
					valuesDictionary3.SetValue<bool>("IsSpawned", spawnChunk.IsSpawned);
					valuesDictionary3.SetValue<double>("LastVisitedTime", spawnChunk.LastVisitedTime.Value);
					string value = this.SaveSpawnsData(spawnChunk.SpawnsData);
					if (!string.IsNullOrEmpty(value))
					{
						valuesDictionary3.SetValue<string>("SpawnsData", value);
					}
				}
			}
		}

        // Token: 0x06000A6E RID: 2670 RVA: 0x0004D5C4 File Offset: 0x0004B7C4
        public override void OnEntityAdded(Entity entity)
		{
			foreach (ComponentSpawn key in entity.FindComponents<ComponentSpawn>())
			{
				this.m_spawns.Add(key, true);
			}
		}

        // Token: 0x06000A6F RID: 2671 RVA: 0x0004D620 File Offset: 0x0004B820
        public override void OnEntityRemoved(Entity entity)
		{
			foreach (ComponentSpawn key in entity.FindComponents<ComponentSpawn>())
			{
				this.m_spawns.Remove(key);
			}
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x0004D67C File Offset: 0x0004B87C
		public SpawnChunk GetOrCreateSpawnChunk(Point2 point)
		{
			SpawnChunk spawnChunk = this.GetSpawnChunk(point);
			if (spawnChunk == null)
			{
				spawnChunk = new SpawnChunk
				{
					Point = point
				};
				this.m_chunks.Add(point, spawnChunk);
			}
			return spawnChunk;
		}

		// Token: 0x06000A71 RID: 2673 RVA: 0x0004D6B0 File Offset: 0x0004B8B0
		public void DiscardOldChunks()
		{
			List<Point2> list = new List<Point2>();
			foreach (SpawnChunk spawnChunk in this.m_chunks.Values)
			{
				if (spawnChunk.LastVisitedTime == null || this.m_subsystemGameInfo.TotalElapsedGameTime - spawnChunk.LastVisitedTime.Value > 76800.0)
				{
					list.Add(spawnChunk.Point);
				}
			}
			foreach (Point2 key in list)
			{
				this.m_chunks.Remove(key);
			}
		}

		// Token: 0x06000A72 RID: 2674 RVA: 0x0004D788 File Offset: 0x0004B988
		public void UpdateLastVisitedTime()
		{
			foreach (ComponentPlayer componentPlayer in this.m_subsystemPlayers.ComponentPlayers)
			{
				Vector2 v = new Vector2(componentPlayer.ComponentBody.Position.X, componentPlayer.ComponentBody.Position.Z);
				Vector2 p = v - new Vector2(8f);
				Vector2 p2 = v + new Vector2(8f);
				Point2 point = Terrain.ToChunk(p);
				Point2 point2 = Terrain.ToChunk(p2);
				for (int i = point.X; i <= point2.X; i++)
				{
					for (int j = point.Y; j <= point2.Y; j++)
					{
						SpawnChunk spawnChunk = this.GetSpawnChunk(new Point2(i, j));
						if (spawnChunk != null)
						{
							spawnChunk.LastVisitedTime = new double?(this.m_subsystemGameInfo.TotalElapsedGameTime);
						}
					}
				}
			}
		}

		// Token: 0x06000A73 RID: 2675 RVA: 0x0004D8A0 File Offset: 0x0004BAA0
		public void SpawnChunks()
		{
			List<SpawnChunk> list = new List<SpawnChunk>();
			foreach (GameWidget gameWidget in this.m_subsystemViews.GameWidgets)
			{
				Vector2 v = new Vector2(gameWidget.ActiveCamera.ViewPosition.X, gameWidget.ActiveCamera.ViewPosition.Z);
				Vector2 p = v - new Vector2(40f);
				Vector2 p2 = v + new Vector2(40f);
				Point2 point = Terrain.ToChunk(p);
				Point2 point2 = Terrain.ToChunk(p2);
				for (int i = point.X; i <= point2.X; i++)
				{
					for (int j = point.Y; j <= point2.Y; j++)
					{
						Vector2 vector = new Vector2(((float)i + 0.5f) * 16f, ((float)j + 0.5f) * 16f);
						if (Vector2.DistanceSquared(v, vector) < 1600f)
						{
							TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(Terrain.ToCell(vector.X), Terrain.ToCell(vector.Y));
							if (chunkAtCell != null && chunkAtCell.State > TerrainChunkState.InvalidPropagatedLight)
							{
								Point2 point3 = new Point2(i, j);
								SpawnChunk orCreateSpawnChunk = this.GetOrCreateSpawnChunk(point3);
								foreach (SpawnEntityData data in orCreateSpawnChunk.SpawnsData)
								{
									this.SpawnEntity(data);
								}
								orCreateSpawnChunk.SpawnsData.Clear();
								Action<SpawnChunk> spawningChunk = this.SpawningChunk;
								if (spawningChunk != null)
								{
									spawningChunk(orCreateSpawnChunk);
								}
								orCreateSpawnChunk.IsSpawned = true;
							}
						}
					}
				}
			}
			foreach (SpawnChunk spawnChunk in list)
			{
				foreach (SpawnEntityData data2 in spawnChunk.SpawnsData)
				{
					this.SpawnEntity(data2);
				}
				spawnChunk.SpawnsData.Clear();
			}
		}

		// Token: 0x06000A74 RID: 2676 RVA: 0x0004DB54 File Offset: 0x0004BD54
		public void DespawnChunks()
		{
			List<ComponentSpawn> list = new List<ComponentSpawn>(0);
			foreach (ComponentSpawn componentSpawn in this.m_spawns.Keys)
			{
				if (componentSpawn.AutoDespawn && !componentSpawn.IsDespawning)
				{
					bool flag = true;
					Vector3 position = componentSpawn.ComponentFrame.Position;
					Vector2 v = new Vector2(position.X, position.Z);
					foreach (GameWidget gameWidget in this.m_subsystemViews.GameWidgets)
					{
						Vector3 viewPosition = gameWidget.ActiveCamera.ViewPosition;
						Vector2 v2 = new Vector2(viewPosition.X, viewPosition.Z);
						if (Vector2.DistanceSquared(v, v2) <= 2704f)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						list.Add(componentSpawn);
					}
				}
			}
			foreach (ComponentSpawn componentSpawn2 in list)
			{
				Point2 point = Terrain.ToChunk(componentSpawn2.ComponentFrame.Position.XZ);
				List<SpawnEntityData> spawnsData = this.GetOrCreateSpawnChunk(point).SpawnsData;
				SpawnEntityData spawnEntityData = new SpawnEntityData();
				spawnEntityData.TemplateName = componentSpawn2.Entity.ValuesDictionary.DatabaseObject.Name;
				spawnEntityData.Position = componentSpawn2.ComponentFrame.Position;
				ComponentCreature componentCreature = componentSpawn2.ComponentCreature;
				spawnEntityData.ConstantSpawn = (componentCreature != null && componentCreature.ConstantSpawn);
				spawnsData.Add(spawnEntityData);
				componentSpawn2.Despawn();
			}
		}

		// Token: 0x06000A75 RID: 2677 RVA: 0x0004DD30 File Offset: 0x0004BF30
		public Entity SpawnEntity(SpawnEntityData data)
		{
			Entity result;
			try
			{
				Entity entity = DatabaseManager.CreateEntity(base.Project, data.TemplateName, true);
				entity.FindComponent<ComponentBody>(true).Position = data.Position;
				entity.FindComponent<ComponentBody>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, this.m_random.Float(0f, 6.2831855f));
				ComponentCreature componentCreature = entity.FindComponent<ComponentCreature>();
				if (componentCreature != null)
				{
					componentCreature.ConstantSpawn = data.ConstantSpawn;
				}
				base.Project.AddEntity(entity);
				result = entity;
			}
			catch (Exception ex)
			{
				Log.Error("Unable to spawn entity with template \"" + data.TemplateName + "\". Reason: " + ex.Message);
				result = null;
			}
			return result;
		}

		// Token: 0x06000A76 RID: 2678 RVA: 0x0004DDEC File Offset: 0x0004BFEC
		public void LoadSpawnsData(string data, List<SpawnEntityData> creaturesData)
		{
			string[] array = data.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array2.Length < 4)
				{
					throw new InvalidOperationException("Invalid spawn data string.");
				}
				SpawnEntityData spawnEntityData = new SpawnEntityData
				{
					TemplateName = array2[0],
					Position = new Vector3
					{
						X = float.Parse(array2[1], CultureInfo.InvariantCulture),
						Y = float.Parse(array2[2], CultureInfo.InvariantCulture),
						Z = float.Parse(array2[3], CultureInfo.InvariantCulture)
					}
				};
				if (array2.Length >= 5)
				{
					spawnEntityData.ConstantSpawn = bool.Parse(array2[4]);
				}
				creaturesData.Add(spawnEntityData);
			}
		}

		// Token: 0x06000A77 RID: 2679 RVA: 0x0004DEC0 File Offset: 0x0004C0C0
		public string SaveSpawnsData(List<SpawnEntityData> spawnsData)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SpawnEntityData spawnEntityData in spawnsData)
			{
				stringBuilder.Append(spawnEntityData.TemplateName);
				stringBuilder.Append(',');
				stringBuilder.Append((MathUtils.Round(spawnEntityData.Position.X * 10f) / 10f).ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				stringBuilder.Append((MathUtils.Round(spawnEntityData.Position.Y * 10f) / 10f).ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				stringBuilder.Append((MathUtils.Round(spawnEntityData.Position.Z * 10f) / 10f).ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				stringBuilder.Append(spawnEntityData.ConstantSpawn.ToString());
				stringBuilder.Append(';');
			}
			return stringBuilder.ToString();
		}

		// Token: 0x040005AE RID: 1454
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040005AF RID: 1455
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x040005B0 RID: 1456
		public SubsystemGameWidgets m_subsystemViews;

		// Token: 0x040005B1 RID: 1457
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040005B2 RID: 1458
		public SubsystemTime m_subsystemTime;

		// Token: 0x040005B3 RID: 1459
		public Game.Random m_random = new Game.Random();

		// Token: 0x040005B4 RID: 1460
		public double m_nextDiscardOldChunksTime = 1.0;

		// Token: 0x040005B5 RID: 1461
		public double m_nextVisitedTime = 1.0;

		// Token: 0x040005B6 RID: 1462
		public double m_nextChunkSpawnTime = 1.0;

		// Token: 0x040005B7 RID: 1463
		public double m_nextDespawnTime = 1.0;

		// Token: 0x040005B8 RID: 1464
		public Dictionary<Point2, SpawnChunk> m_chunks = new Dictionary<Point2, SpawnChunk>();

		// Token: 0x040005B9 RID: 1465
		public Dictionary<ComponentSpawn, bool> m_spawns = new Dictionary<ComponentSpawn, bool>();

		// Token: 0x040005BA RID: 1466
		public const float MaxChunkAge = 76800f;

		// Token: 0x040005BB RID: 1467
		public const float VisitedRadius = 8f;

		// Token: 0x040005BC RID: 1468
		public const float SpawnRadius = 40f;

		// Token: 0x040005BD RID: 1469
		public const float DespawnRadius = 52f;
	}
}
