using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000199 RID: 409
	public class SubsystemPickables : Subsystem, IDrawable, IUpdateable
	{
		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000992 RID: 2450 RVA: 0x00042E5C File Offset: 0x0004105C
		public ReadOnlyList<Pickable> Pickables
		{
			get
			{
				return new ReadOnlyList<Pickable>(this.m_pickables);
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000993 RID: 2451 RVA: 0x00042E69 File Offset: 0x00041069
		public int[] DrawOrders
		{
			get
			{
				return SubsystemPickables.m_drawOrders;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000994 RID: 2452 RVA: 0x00042E70 File Offset: 0x00041070
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000995 RID: 2453 RVA: 0x00042E74 File Offset: 0x00041074
		// (remove) Token: 0x06000996 RID: 2454 RVA: 0x00042EAC File Offset: 0x000410AC
		public event Action<Pickable> PickableAdded;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000997 RID: 2455 RVA: 0x00042EE4 File Offset: 0x000410E4
		// (remove) Token: 0x06000998 RID: 2456 RVA: 0x00042F1C File Offset: 0x0004111C
		public event Action<Pickable> PickableRemoved;

		// Token: 0x06000999 RID: 2457 RVA: 0x00042F54 File Offset: 0x00041154
		public Pickable AddPickable(int value, int count, Vector3 position, Vector3? velocity, Matrix? stuckMatrix)
		{
			Pickable pickable = new Pickable();
			pickable.Value = value;
			pickable.Count = count;
			//pickable.Count = 0;
			pickable.Position = position;
			pickable.StuckMatrix = stuckMatrix;
			pickable.CreationTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
			if (velocity != null)
			{
				pickable.Velocity = velocity.Value;
			}
			else if (Terrain.ExtractContents(value) == 248)
			{
				Vector2 vector = this.m_random.Vector2(1.5f, 2f);
				pickable.Velocity = new Vector3(vector.X, 3f, vector.Y);
			}
			else
			{
				pickable.Velocity = new Vector3(this.m_random.Float(-0.5f, 0.5f), this.m_random.Float(1f, 1.2f), this.m_random.Float(-0.5f, 0.5f));
			}
			this.m_pickables.Add(pickable);
			if (this.PickableAdded != null)
			{
				this.PickableAdded(pickable);
			}
			return pickable;
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x0004305C File Offset: 0x0004125C
		public void Draw(Camera camera, int drawOrder)
		{
			double totalElapsedGameTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
			this.m_drawBlockEnvironmentData.SubsystemTerrain = this.m_subsystemTerrain;
			Matrix matrix = Matrix.CreateRotationY((float)MathUtils.Remainder(totalElapsedGameTime, 6.2831854820251465));
			float num = MathUtils.Min(this.m_subsystemSky.VisibilityRange, 30f);
			foreach (Pickable pickable in this.m_pickables)
			{
				Vector3 position = pickable.Position;
				float num2 = Vector3.Dot(camera.ViewDirection, position - camera.ViewPosition);
				if (num2 > -0.5f && num2 < num)
				{
					int num3 = Terrain.ExtractContents(pickable.Value);
					Block block = BlocksManager.Blocks[num3];
					float num4 = (float)(totalElapsedGameTime - pickable.CreationTime);
					if (pickable.StuckMatrix == null)
					{
						position.Y += 0.25f * MathUtils.Saturate(3f * num4);
					}
					int x = Terrain.ToCell(position.X);
					int num5 = Terrain.ToCell(position.Y);
					int z = Terrain.ToCell(position.Z);
					TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(x, z);
					if (chunkAtCell != null && chunkAtCell.State >= TerrainChunkState.InvalidVertices1 && num5 >= 0 && num5 < 255)
					{
						this.m_drawBlockEnvironmentData.Humidity = this.m_subsystemTerrain.Terrain.GetSeasonalHumidity(x, z);
						this.m_drawBlockEnvironmentData.Temperature = this.m_subsystemTerrain.Terrain.GetSeasonalTemperature(x, z) + SubsystemWeather.GetTemperatureAdjustmentAtHeight(num5);
						float f = MathUtils.Max(position.Y - (float)num5 - 0.75f, 0f) / 0.25f;
						pickable.Light = (int)MathUtils.Lerp((float)this.m_subsystemTerrain.Terrain.GetCellLightFast(x, num5, z), (float)this.m_subsystemTerrain.Terrain.GetCellLightFast(x, num5 + 1, z), f);
					}
					this.m_drawBlockEnvironmentData.Light = pickable.Light;
					this.m_drawBlockEnvironmentData.BillboardDirection = new Vector3?(pickable.Position - camera.ViewPosition);
					this.m_drawBlockEnvironmentData.InWorldMatrix.Translation = position;
					if (pickable.StuckMatrix != null)
					{
						Matrix value = pickable.StuckMatrix.Value;
						block.DrawBlock(this.m_primitivesRenderer, pickable.Value, Color.White, 0.3f, ref value, this.m_drawBlockEnvironmentData);
					}
					else
					{
						matrix.Translation = position + new Vector3(0f, 0.04f * MathUtils.Sin(3f * num4), 0f);
						block.DrawBlock(this.m_primitivesRenderer, pickable.Value, Color.White, 0.3f, ref matrix, this.m_drawBlockEnvironmentData);
					}
				}
			}
			this.m_primitivesRenderer.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x00043394 File Offset: 0x00041594
		public void Update(float dt)
		{
			double totalElapsedGameTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
			float s = MathUtils.Pow(0.5f, dt);
			float num = MathUtils.Pow(0.001f, dt);
			this.m_tmpPlayers.Clear();
			foreach (ComponentPlayer componentPlayer in this.m_subsystemPlayers.ComponentPlayers)
			{
				if (componentPlayer.ComponentHealth.Health > 0f)
				{
					this.m_tmpPlayers.Add(componentPlayer);
				}
			}
			foreach (Pickable pickable in this.m_pickables)
			{
				if (pickable.ToRemove)
				{
					this.m_pickablesToRemove.Add(pickable);
				}
				else
				{
					Block block = BlocksManager.Blocks[Terrain.ExtractContents(pickable.Value)];
					int num2 = this.m_pickables.Count - this.m_pickablesToRemove.Count;
					float num3 = MathUtils.Lerp(300f, 90f, MathUtils.Saturate((float)num2 / 60f));
					double num4 = totalElapsedGameTime - pickable.CreationTime;
					num3 = 4.5f;
					if (num4 > (double)num3)
					{
						pickable.ToRemove = true;
					}
					else
					{
						TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(Terrain.ToCell(pickable.Position.X), Terrain.ToCell(pickable.Position.Z));
						if (chunkAtCell != null && chunkAtCell.State > TerrainChunkState.InvalidContents4)
						{
							Vector3 position = pickable.Position;
							Vector3 vector = position + pickable.Velocity * dt;
							if (pickable.FlyToPosition == null && num4 > 0.5)
							{
								foreach (ComponentPlayer componentPlayer2 in this.m_tmpPlayers)
								{
									ComponentBody componentBody = componentPlayer2.ComponentBody;
									Vector3 v = componentBody.Position + new Vector3(0f, 0.75f, 0f);
									float num5 = (v - pickable.Position).LengthSquared();
									if (num5 < 3.0625f)
									{
										bool flag = Terrain.ExtractContents(pickable.Value) == 248;
										IInventory inventory = componentPlayer2.ComponentMiner.Inventory;
										if (flag || ComponentInventoryBase.FindAcquireSlotForItem(inventory, pickable.Value) >= 0)
										{
											if (num5 < 1f)
											{
												if (flag)
												{
													componentPlayer2.ComponentLevel.AddExperience(pickable.Count, true);
													pickable.ToRemove = true;
												}
												else
												{
													pickable.Count = ComponentInventoryBase.AcquireItems(inventory, pickable.Value, pickable.Count);
													if (pickable.Count == 0)
													{
														pickable.ToRemove = true;
														this.m_subsystemAudio.PlaySound("Audio/PickableCollected", 0.7f, -0.4f, pickable.Position, 2f, false);
													}
												}
											}
											else if (pickable.StuckMatrix == null)
											{
												pickable.FlyToPosition = new Vector3?(v + 0.1f * MathUtils.Sqrt(num5) * componentBody.getVectorSpeed());
											}
										}
									}
								}
							}
							if (pickable.FlyToPosition != null)
							{
								Vector3 v2 = pickable.FlyToPosition.Value - pickable.Position;
								float num6 = v2.LengthSquared();
								if (num6 >= 0.25f)
								{
									pickable.Velocity = 6f * v2 / MathUtils.Sqrt(num6);
								}
								else
								{
									pickable.FlyToPosition = null;
								}
							}
							else
							{
								FluidBlock fluidBlock;
								float? num7;
								Vector2? vector2 = this.m_subsystemFluidBlockBehavior.CalculateFlowSpeed(Terrain.ToCell(pickable.Position.X), Terrain.ToCell(pickable.Position.Y + 0.1f), Terrain.ToCell(pickable.Position.Z), out fluidBlock, out num7);
								if (pickable.StuckMatrix == null)
								{
									TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(position, vector, false, true, (int value, float distance) => BlocksManager.Blocks[Terrain.ExtractContents(value)].IsCollidable);
									if (terrainRaycastResult != null)
									{
										int contents = Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValue(terrainRaycastResult.Value.CellFace.X, terrainRaycastResult.Value.CellFace.Y, terrainRaycastResult.Value.CellFace.Z));
										SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(contents);
										for (int i = 0; i < blockBehaviors.Length; i++)
										{
											blockBehaviors[i].OnHitByProjectile(terrainRaycastResult.Value.CellFace, pickable);
										}
										if (this.m_subsystemTerrain.Raycast(position, position, false, true, (int value2, float distance) => BlocksManager.Blocks[Terrain.ExtractContents(value2)].IsCollidable) != null)
										{
											int num8 = Terrain.ToCell(position.X);
											int num9 = Terrain.ToCell(position.Y);
											int num10 = Terrain.ToCell(position.Z);
											int num11 = 0;
											int num12 = 0;
											int num13 = 0;
											int? num14 = null;
											for (int j = -3; j <= 3; j++)
											{
												for (int k = -3; k <= 3; k++)
												{
													for (int l = -3; l <= 3; l++)
													{
														if (!BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(j + num8, k + num9, l + num10)].IsCollidable)
														{
															int num15 = j * j + k * k + l * l;
															if (num14 == null || num15 < num14.Value)
															{
																num11 = j + num8;
																num12 = k + num9;
																num13 = l + num10;
																num14 = new int?(num15);
															}
														}
													}
												}
											}
											if (num14 != null)
											{
												pickable.FlyToPosition = new Vector3?(new Vector3((float)num11, (float)num12, (float)num13) + new Vector3(0.5f));
											}
											else
											{
												pickable.ToRemove = true;
											}
										}
										else
										{
											Plane plane = terrainRaycastResult.Value.CellFace.CalculatePlane();
											bool flag2 = vector2 != null && vector2.Value != Vector2.Zero;
											if (plane.Normal.X != 0f)
											{
												float num16 = (flag2 || MathUtils.Sqrt(MathUtils.Sqr(pickable.Velocity.Y) + MathUtils.Sqr(pickable.Velocity.Z)) > 10f) ? 0.95f : 0.25f;
												pickable.Velocity *= new Vector3(0f - num16, num16, num16);
											}
											if (plane.Normal.Y != 0f)
											{
												float num17 = (flag2 || MathUtils.Sqrt(MathUtils.Sqr(pickable.Velocity.X) + MathUtils.Sqr(pickable.Velocity.Z)) > 10f) ? 0.95f : 0.25f;
												pickable.Velocity *= new Vector3(num17, 0f - num17, num17);
												if (flag2)
												{
													Pickable pickable2 = pickable;
													pickable2.Velocity.Y = pickable2.Velocity.Y + 0.1f * plane.Normal.Y;
												}
											}
											if (plane.Normal.Z != 0f)
											{
												float num18 = (flag2 || MathUtils.Sqrt(MathUtils.Sqr(pickable.Velocity.X) + MathUtils.Sqr(pickable.Velocity.Y)) > 10f) ? 0.95f : 0.25f;
												pickable.Velocity *= new Vector3(num18, num18, 0f - num18);
											}
											vector = position;
										}
									}
								}
								else
								{
									Vector3 vector3 = pickable.StuckMatrix.Value.Translation + pickable.StuckMatrix.Value.Up * block.ProjectileTipOffset;
									if (this.m_subsystemTerrain.Raycast(vector3, vector3, false, true, (int value, float distance) => BlocksManager.Blocks[Terrain.ExtractContents(value)].IsCollidable) == null)
									{
										pickable.Position = pickable.StuckMatrix.Value.Translation;
										pickable.Velocity = Vector3.Zero;
										pickable.StuckMatrix = null;
									}
								}
								if (fluidBlock is WaterBlock && !pickable.SplashGenerated)
								{
									this.m_subsystemParticles.AddParticleSystem(new WaterSplashParticleSystem(this.m_subsystemTerrain, pickable.Position, false));
									this.m_subsystemAudio.PlayRandomSound("Audio/Splashes", 1f, this.m_random.Float(-0.2f, 0.2f), pickable.Position, 6f, true);
									pickable.SplashGenerated = true;
								}
								else if (fluidBlock is MagmaBlock && !pickable.SplashGenerated)
								{
									this.m_subsystemParticles.AddParticleSystem(new MagmaSplashParticleSystem(this.m_subsystemTerrain, pickable.Position, false));
									this.m_subsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, this.m_random.Float(-0.2f, 0.2f), pickable.Position, 3f, true);
									pickable.ToRemove = true;
									pickable.SplashGenerated = true;
									this.m_subsystemExplosions.TryExplodeBlock(Terrain.ToCell(pickable.Position.X), Terrain.ToCell(pickable.Position.Y), Terrain.ToCell(pickable.Position.Z), pickable.Value);
								}
								else if (fluidBlock == null)
								{
									pickable.SplashGenerated = false;
								}
								if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, (double)(pickable.GetHashCode() % 100) / 100.0) && (this.m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(pickable.Position.X), Terrain.ToCell(pickable.Position.Y + 0.1f), Terrain.ToCell(pickable.Position.Z)) == 104 || this.m_subsystemFireBlockBehavior.IsCellOnFire(Terrain.ToCell(pickable.Position.X), Terrain.ToCell(pickable.Position.Y + 0.1f), Terrain.ToCell(pickable.Position.Z))))
								{
									this.m_subsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, this.m_random.Float(-0.2f, 0.2f), pickable.Position, 3f, true);
									pickable.ToRemove = true;
									this.m_subsystemExplosions.TryExplodeBlock(Terrain.ToCell(pickable.Position.X), Terrain.ToCell(pickable.Position.Y), Terrain.ToCell(pickable.Position.Z), pickable.Value);
								}
								if (pickable.StuckMatrix == null)
								{
									if (vector2 != null && num7 != null)
									{
										float num19 = num7.Value - pickable.Position.Y;
										float num20 = MathUtils.Saturate(3f * num19);
										Pickable pickable3 = pickable;
										pickable3.Velocity.X = pickable3.Velocity.X + 4f * dt * (vector2.Value.X - pickable.Velocity.X);
										Pickable pickable4 = pickable;
										pickable4.Velocity.Y = pickable4.Velocity.Y - 10f * dt;
										Pickable pickable5 = pickable;
										pickable5.Velocity.Y = pickable5.Velocity.Y + 10f * (1f / block.Density * num20) * dt;
										Pickable pickable6 = pickable;
										pickable6.Velocity.Z = pickable6.Velocity.Z + 4f * dt * (vector2.Value.Y - pickable.Velocity.Z);
										Pickable pickable7 = pickable;
										pickable7.Velocity.Y = pickable7.Velocity.Y * num;
									}
									else
									{
										Pickable pickable8 = pickable;
										pickable8.Velocity.Y = pickable8.Velocity.Y - 10f * dt;
										pickable.Velocity *= s;
									}
								}
							}
							pickable.Position = vector;
						}
					}
				}
			}
			foreach (Pickable pickable9 in this.m_pickablesToRemove)
			{
				this.m_pickables.Remove(pickable9);
				if (this.PickableRemoved != null)
				{
					this.PickableRemoved(pickable9);
				}
			}
			this.m_pickablesToRemove.Clear();
		}

        // Token: 0x0600099C RID: 2460 RVA: 0x000440E8 File Offset: 0x000422E8
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.m_subsystemFireBlockBehavior = base.Project.FindSubsystem<SubsystemFireBlockBehavior>(true);
			this.m_subsystemFluidBlockBehavior = base.Project.FindSubsystem<SubsystemFluidBlockBehavior>(true);
			foreach (object obj in from v in valuesDictionary.GetValue<ValuesDictionary>("Pickables").Values
			where v is ValuesDictionary
			select v)
			{
				ValuesDictionary valuesDictionary2 = (ValuesDictionary)obj;
				Pickable pickable = new Pickable();
				pickable.Value = valuesDictionary2.GetValue<int>("Value");
				pickable.Count = valuesDictionary2.GetValue<int>("Count");
				pickable.Position = valuesDictionary2.GetValue<Vector3>("Position");
				pickable.Velocity = valuesDictionary2.GetValue<Vector3>("Velocity");
				pickable.CreationTime = valuesDictionary2.GetValue<double>("CreationTime", 0.0);
				if (valuesDictionary2.ContainsKey("StuckMatrix"))
				{
					pickable.StuckMatrix = new Matrix?(valuesDictionary2.GetValue<Matrix>("StuckMatrix"));
				}
				this.m_pickables.Add(pickable);
			}
		}

        // Token: 0x0600099D RID: 2461 RVA: 0x000442C0 File Offset: 0x000424C0
        public override void Save(ValuesDictionary valuesDictionary)
		{
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Pickables", valuesDictionary2);
			int num = 0;
			foreach (Pickable pickable in this.m_pickables)
			{
				ValuesDictionary valuesDictionary3 = new ValuesDictionary();
				valuesDictionary2.SetValue<ValuesDictionary>(num.ToString(), valuesDictionary3);
				valuesDictionary3.SetValue<int>("Value", pickable.Value);
				valuesDictionary3.SetValue<int>("Count", pickable.Count);
				valuesDictionary3.SetValue<Vector3>("Position", pickable.Position);
				valuesDictionary3.SetValue<Vector3>("Velocity", pickable.Velocity);
				valuesDictionary3.SetValue<double>("CreationTime", pickable.CreationTime);
				if (pickable.StuckMatrix != null)
				{
					valuesDictionary3.SetValue<Matrix>("StuckMatrix", pickable.StuckMatrix.Value);
				}
				num++;
			}
		}

		// Token: 0x04000509 RID: 1289
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400050A RID: 1290
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x0400050B RID: 1291
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400050C RID: 1292
		public SubsystemSky m_subsystemSky;

		// Token: 0x0400050D RID: 1293
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400050E RID: 1294
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x0400050F RID: 1295
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000510 RID: 1296
		public SubsystemExplosions m_subsystemExplosions;

		// Token: 0x04000511 RID: 1297
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x04000512 RID: 1298
		public SubsystemFireBlockBehavior m_subsystemFireBlockBehavior;

		// Token: 0x04000513 RID: 1299
		public SubsystemFluidBlockBehavior m_subsystemFluidBlockBehavior;

		// Token: 0x04000514 RID: 1300
		public List<ComponentPlayer> m_tmpPlayers = new List<ComponentPlayer>();

		// Token: 0x04000515 RID: 1301
		public List<Pickable> m_pickables = new List<Pickable>();

		// Token: 0x04000516 RID: 1302
		public List<Pickable> m_pickablesToRemove = new List<Pickable>();

		// Token: 0x04000517 RID: 1303
		public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

		// Token: 0x04000518 RID: 1304
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000519 RID: 1305
		public DrawBlockEnvironmentData m_drawBlockEnvironmentData = new DrawBlockEnvironmentData();

		// Token: 0x0400051A RID: 1306
		public static int[] m_drawOrders = new int[]
		{
			10
		};
	}
}
