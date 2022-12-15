using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001A0 RID: 416
	public class SubsystemProjectiles : Subsystem, IUpdateable, IDrawable
	{
		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060009DB RID: 2523 RVA: 0x00046AD7 File Offset: 0x00044CD7
		public ReadOnlyList<Projectile> Projectiles
		{
			get
			{
				return new ReadOnlyList<Projectile>(this.m_projectiles);
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060009DC RID: 2524 RVA: 0x00046AE4 File Offset: 0x00044CE4
		public int[] DrawOrders
		{
			get
			{
				return SubsystemProjectiles.m_drawOrders;
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060009DD RID: 2525 RVA: 0x00046AEB File Offset: 0x00044CEB
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x060009DE RID: 2526 RVA: 0x00046AF0 File Offset: 0x00044CF0
		// (remove) Token: 0x060009DF RID: 2527 RVA: 0x00046B28 File Offset: 0x00044D28
		public event Action<Projectile> ProjectileAdded;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x060009E0 RID: 2528 RVA: 0x00046B60 File Offset: 0x00044D60
		// (remove) Token: 0x060009E1 RID: 2529 RVA: 0x00046B98 File Offset: 0x00044D98
		public event Action<Projectile> ProjectileRemoved;

		// Token: 0x060009E2 RID: 2530 RVA: 0x00046BD0 File Offset: 0x00044DD0
		public Projectile AddProjectile(int value, Vector3 position, Vector3 velocity, Vector3 angularVelocity, ComponentCreature owner)
		{
			Projectile projectile = new Projectile();
			projectile.Value = value;
			projectile.Position = position;
			projectile.Velocity = velocity;
			projectile.Rotation = Vector3.Zero;
			projectile.AngularVelocity = angularVelocity;
			projectile.CreationTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
			projectile.IsInWater = this.IsWater(position);
			projectile.Owner = owner;
			projectile.ProjectileStoppedAction = ProjectileStoppedAction.TurnIntoPickable;
			this.m_projectiles.Add(projectile);
			if (this.ProjectileAdded != null)
			{
				this.ProjectileAdded(projectile);
			}
			if (owner != null && owner.PlayerStats != null)
			{
				owner.PlayerStats.RangedAttacks += 1L;
			}
			return projectile;
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x00046C7C File Offset: 0x00044E7C
		public Projectile FireProjectile(int value, Vector3 position, Vector3 velocity, Vector3 angularVelocity, ComponentCreature owner)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			Vector3 v = Vector3.Normalize(velocity);
			Vector3 vector = position;
			if (owner != null)
			{
				Ray3 ray = new Ray3(position + v * 5f, -v);
				BoundingBox boundingBox = owner.ComponentBody.BoundingBox;
				boundingBox.Min -= new Vector3(0.4f);
				boundingBox.Max += new Vector3(0.4f);
				float? num2 = ray.Intersection(boundingBox);
				if (num2 != null)
				{
					if (num2.Value == 0f)
					{
						return null;
					}
					vector = position + v * (5f - num2.Value + 0.1f);
				}
			}
			Vector3 end = vector + v * block.ProjectileTipOffset;
			if (this.m_subsystemTerrain.Raycast(position, end, false, true, (int testValue, float distance) => BlocksManager.Blocks[Terrain.ExtractContents(testValue)].IsCollidable) == null)
			{
				Projectile projectile = this.AddProjectile(value, vector, velocity, angularVelocity, owner);
				SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(num);
				for (int i = 0; i < blockBehaviors.Length; i++)
				{
					blockBehaviors[i].OnFiredAsProjectile(projectile);
				}
				return projectile;
			}
			return null;
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x00046DE8 File Offset: 0x00044FE8
		public void AddTrail(Projectile projectile, Vector3 offset, ITrailParticleSystem particleSystem)
		{
			this.RemoveTrail(projectile);
			projectile.TrailParticleSystem = particleSystem;
			projectile.TrailOffset = offset;
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x00046DFF File Offset: 0x00044FFF
		public void RemoveTrail(Projectile projectile)
		{
			if (projectile.TrailParticleSystem != null)
			{
				if (this.m_subsystemParticles.ContainsParticleSystem((ParticleSystemBase)projectile.TrailParticleSystem))
				{
					this.m_subsystemParticles.RemoveParticleSystem((ParticleSystemBase)projectile.TrailParticleSystem);
				}
				projectile.TrailParticleSystem = null;
			}
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x00046E40 File Offset: 0x00045040
		public void Draw(Camera camera, int drawOrder)
		{
			this.m_drawBlockEnvironmentData.SubsystemTerrain = this.m_subsystemTerrain;
			this.m_drawBlockEnvironmentData.InWorldMatrix = Matrix.Identity;
			float num = MathUtils.Sqr(this.m_subsystemSky.VisibilityRange);
			foreach (Projectile projectile in this.m_projectiles)
			{
				Vector3 position = projectile.Position;
				if (!projectile.NoChunk && Vector3.DistanceSquared(camera.ViewPosition, position) < num && camera.ViewFrustum.Intersection(position))
				{
					int x = Terrain.ToCell(position.X);
					int num2 = Terrain.ToCell(position.Y);
					int z = Terrain.ToCell(position.Z);
					int num3 = Terrain.ExtractContents(projectile.Value);
					Block block = BlocksManager.Blocks[num3];
					TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(x, z);
					if (chunkAtCell != null && chunkAtCell.State >= TerrainChunkState.InvalidVertices1 && num2 >= 0 && num2 < 255)
					{
						this.m_drawBlockEnvironmentData.Humidity = this.m_subsystemTerrain.Terrain.GetSeasonalHumidity(x, z);
						this.m_drawBlockEnvironmentData.Temperature = this.m_subsystemTerrain.Terrain.GetSeasonalTemperature(x, z) + SubsystemWeather.GetTemperatureAdjustmentAtHeight(num2);
						projectile.Light = this.m_subsystemTerrain.Terrain.GetCellLightFast(x, num2, z);
					}
					this.m_drawBlockEnvironmentData.Light = projectile.Light;
					this.m_drawBlockEnvironmentData.BillboardDirection = (block.AlignToVelocity ? null : new Vector3?(camera.ViewDirection));
					this.m_drawBlockEnvironmentData.InWorldMatrix.Translation = position;
					Matrix matrix;
					if (block.AlignToVelocity)
					{
						SubsystemProjectiles.CalculateVelocityAlignMatrix(block, position, projectile.Velocity, out matrix);
					}
					else if (projectile.Rotation != Vector3.Zero)
					{
						matrix = Matrix.CreateFromAxisAngle(Vector3.Normalize(projectile.Rotation), projectile.Rotation.Length());
						matrix.Translation = projectile.Position;
					}
					else
					{
						matrix = Matrix.CreateTranslation(projectile.Position);
					}
					block.DrawBlock(this.m_primitivesRenderer, projectile.Value, Color.White, 0.3f, ref matrix, this.m_drawBlockEnvironmentData);
				}
			}
			this.m_primitivesRenderer.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x000470C8 File Offset: 0x000452C8
		public void Update(float dt)
		{
			double totalElapsedGameTime = this.m_subsystemGameInfo.TotalElapsedGameTime;
			foreach (Projectile projectile in this.m_projectiles)
			{
				if (projectile.ToRemove)
				{
					this.m_projectilesToRemove.Add(projectile);
				}
				else
				{
					Block block = BlocksManager.Blocks[Terrain.ExtractContents(projectile.Value)];
					if (totalElapsedGameTime - projectile.CreationTime > 40.0)
					{
						projectile.ToRemove = true;
					}
					TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(Terrain.ToCell(projectile.Position.X), Terrain.ToCell(projectile.Position.Z));
					if (chunkAtCell == null || chunkAtCell.State <= TerrainChunkState.InvalidContents4)
					{
						projectile.NoChunk = true;
						if (projectile.TrailParticleSystem != null)
						{
							projectile.TrailParticleSystem.IsStopped = true;
						}
					}
					else
					{
						projectile.NoChunk = false;
						Vector3 position = projectile.Position;
						Vector3 vector = position + projectile.Velocity * dt;
						Vector3 v = block.ProjectileTipOffset * Vector3.Normalize(projectile.Velocity);
						BodyRaycastResult? bodyRaycastResult = this.m_subsystemBodies.Raycast(position + v, vector + v, 0.2f, (ComponentBody body, float distance) => true);
						TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(position + v, vector + v, false, true, (int value, float distance) => BlocksManager.Blocks[Terrain.ExtractContents(value)].IsCollidable);
						bool flag = block.DisintegratesOnHit;
						if (terrainRaycastResult != null || bodyRaycastResult != null)
						{
							CellFace? cellFace = (terrainRaycastResult != null) ? new CellFace?(terrainRaycastResult.Value.CellFace) : null;
							ComponentBody componentBody = (bodyRaycastResult != null) ? bodyRaycastResult.Value.ComponentBody : null;
							SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(projectile.Value));
							for (int i = 0; i < blockBehaviors.Length; i++)
							{
								try
								{
                                    flag |= blockBehaviors[i].OnHitAsProjectile(cellFace, componentBody, projectile);
                                } catch (Exception e)
								{
									flag = true;
									break;
								}

                            }
							projectile.ToRemove = flag;
						}
						Vector3? vector2 = null;
						if (bodyRaycastResult != null && (terrainRaycastResult == null || bodyRaycastResult.Value.Distance < terrainRaycastResult.Value.Distance))
						{
							if (projectile.Velocity.Length() > 10f)
							{
								ComponentMiner.AttackBody(bodyRaycastResult.Value.ComponentBody, projectile.Owner, bodyRaycastResult.Value.HitPoint(), Vector3.Normalize(projectile.Velocity), block.GetProjectilePower(projectile.Value), false);
								if (projectile.Owner != null && projectile.Owner.PlayerStats != null)
								{
									projectile.Owner.PlayerStats.RangedHits += 1L;
								}
							}
							if (projectile.IsIncendiary)
							{
								ComponentOnFire componentOnFire = bodyRaycastResult.Value.ComponentBody.Entity.FindComponent<ComponentOnFire>();
								if (componentOnFire != null)
								{
									componentOnFire.SetOnFire((projectile != null) ? projectile.Owner : null, this.m_random.Float(6f, 8f));
								}
							}
							vector = position;
							projectile.Velocity *= -0.05f;
							projectile.Velocity += this.m_random.Vector3(0.33f * projectile.Velocity.Length());
							projectile.AngularVelocity *= -0.05f;
						}
						else if (terrainRaycastResult != null)
						{
							CellFace cellFace2 = terrainRaycastResult.Value.CellFace;
							int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(cellFace2.X, cellFace2.Y, cellFace2.Z);
							int num = Terrain.ExtractContents(cellValue);
							Block block2 = BlocksManager.Blocks[num];
							float num2 = projectile.Velocity.Length();
							SubsystemBlockBehavior[] blockBehaviors2 = this.m_subsystemBlockBehaviors.GetBlockBehaviors(num);
							for (int j = 0; j < blockBehaviors2.Length; j++)
							{
								blockBehaviors2[j].OnHitByProjectile(cellFace2, projectile);
							}
							if (num2 > 10f && this.m_random.Float(0f, 1f) > block2.ProjectileResilience)
							{
								this.m_subsystemTerrain.DestroyCell(0, cellFace2.X, cellFace2.Y, cellFace2.Z, 0, true, false);
								this.m_subsystemSoundMaterials.PlayImpactSound(cellValue, position, 1f);
							}
							if (projectile.IsIncendiary)
							{
								this.m_subsystemFireBlockBehavior.SetCellOnFire(terrainRaycastResult.Value.CellFace.X, terrainRaycastResult.Value.CellFace.Y, terrainRaycastResult.Value.CellFace.Z, 1f);
								Vector3 vector3 = projectile.Position - 0.75f * Vector3.Normalize(projectile.Velocity);
								for (int k = 0; k < 8; k++)
								{
									Vector3 v2 = (k == 0) ? Vector3.Normalize(projectile.Velocity) : this.m_random.Vector3(1.5f);
									TerrainRaycastResult? terrainRaycastResult2 = this.m_subsystemTerrain.Raycast(vector3, vector3 + v2, false, true, (int value, float distance) => true);
									if (terrainRaycastResult2 != null)
									{
										this.m_subsystemFireBlockBehavior.SetCellOnFire(terrainRaycastResult2.Value.CellFace.X, terrainRaycastResult2.Value.CellFace.Y, terrainRaycastResult2.Value.CellFace.Z, 1f);
									}
								}
							}
							if (num2 > 5f)
							{
								this.m_subsystemSoundMaterials.PlayImpactSound(cellValue, position, 1f);
							}
							if (block.IsStickable && num2 > 10f && this.m_random.Bool(block2.ProjectileStickProbability))
							{
								Vector3 v3 = Vector3.Normalize(projectile.Velocity);
								float s = MathUtils.Lerp(0.1f, 0.2f, MathUtils.Saturate((num2 - 15f) / 20f));
								vector2 = new Vector3?(position + terrainRaycastResult.Value.Distance * Vector3.Normalize(projectile.Velocity) + v3 * s);
							}
							else
							{
								Plane plane = cellFace2.CalculatePlane();
								vector = position;
								if (plane.Normal.X != 0f)
								{
									projectile.Velocity *= new Vector3(-0.3f, 0.3f, 0.3f);
								}
								if (plane.Normal.Y != 0f)
								{
									projectile.Velocity *= new Vector3(0.3f, -0.3f, 0.3f);
								}
								if (plane.Normal.Z != 0f)
								{
									projectile.Velocity *= new Vector3(0.3f, 0.3f, -0.3f);
								}
								float num3 = projectile.Velocity.Length();
								projectile.Velocity = num3 * Vector3.Normalize(projectile.Velocity + this.m_random.Vector3(num3 / 6f, num3 / 3f));
								projectile.AngularVelocity *= -0.3f;
							}
							this.MakeProjectileNoise(projectile);
						}
						if (terrainRaycastResult != null || bodyRaycastResult != null)
						{
							if (flag)
							{
								this.m_subsystemParticles.AddParticleSystem(block.CreateDebrisParticleSystem(this.m_subsystemTerrain, projectile.Position, projectile.Value, 1f));
							}
							else if (!projectile.ToRemove && (vector2 != null || projectile.Velocity.Length() < 1f))
							{
								if (projectile.ProjectileStoppedAction == ProjectileStoppedAction.TurnIntoPickable)
								{
									int num4 = BlocksManager.DamageItem(projectile.Value, 1);
									if (num4 != 0)
									{
										if (vector2 != null)
										{
											Matrix value2;
											SubsystemProjectiles.CalculateVelocityAlignMatrix(block, vector2.Value, projectile.Velocity, out value2);
											this.m_subsystemPickables.AddPickable(num4, 1, projectile.Position, new Vector3?(Vector3.Zero), new Matrix?(value2));
										}
										else
										{
											this.m_subsystemPickables.AddPickable(num4, 1, position, new Vector3?(Vector3.Zero), null);
										}
									}
									projectile.ToRemove = true;
								}
								else if (projectile.ProjectileStoppedAction == ProjectileStoppedAction.Disappear)
								{
									projectile.ToRemove = true;
								}
							}
						}
						float s2 = projectile.IsInWater ? MathUtils.Pow(0.001f, dt) : MathUtils.Pow(block.ProjectileDamping, dt);
						Projectile projectile2 = projectile;
						projectile2.Velocity.Y = projectile2.Velocity.Y + -10f * dt;
						projectile.Velocity *= s2;
						projectile.AngularVelocity *= s2;
						projectile.Position = vector;
						projectile.Rotation += projectile.AngularVelocity * dt;
						if (projectile.TrailParticleSystem != null)
						{
							if (!this.m_subsystemParticles.ContainsParticleSystem((ParticleSystemBase)projectile.TrailParticleSystem))
							{
								this.m_subsystemParticles.AddParticleSystem((ParticleSystemBase)projectile.TrailParticleSystem);
							}
							Vector3 v4 = (projectile.TrailOffset != Vector3.Zero) ? Vector3.TransformNormal(projectile.TrailOffset, Matrix.CreateFromAxisAngle(Vector3.Normalize(projectile.Rotation), projectile.Rotation.Length())) : Vector3.Zero;
							projectile.TrailParticleSystem.Position = projectile.Position + v4;
							if (projectile.IsInWater)
							{
								projectile.TrailParticleSystem.IsStopped = true;
							}
						}
						bool flag2 = this.IsWater(projectile.Position);
						if (projectile.IsInWater != flag2)
						{
							if (flag2)
							{
								float num5 = new Vector2(projectile.Velocity.X + projectile.Velocity.Z).Length();
								if (num5 > 6f && num5 > 4f * MathUtils.Abs(projectile.Velocity.Y))
								{
									projectile.Velocity *= 0.5f;
									Projectile projectile3 = projectile;
									projectile3.Velocity.Y = projectile3.Velocity.Y * -1f;
									flag2 = false;
								}
								else
								{
									projectile.Velocity *= 0.2f;
								}
								float? surfaceHeight = this.m_subsystemFluidBlockBehavior.GetSurfaceHeight(Terrain.ToCell(projectile.Position.X), Terrain.ToCell(projectile.Position.Y), Terrain.ToCell(projectile.Position.Z));
								if (surfaceHeight != null)
								{
									this.m_subsystemParticles.AddParticleSystem(new WaterSplashParticleSystem(this.m_subsystemTerrain, new Vector3(projectile.Position.X, surfaceHeight.Value, projectile.Position.Z), false));
									this.m_subsystemAudio.PlayRandomSound("Audio/Splashes", 1f, this.m_random.Float(-0.2f, 0.2f), projectile.Position, 6f, true);
									this.MakeProjectileNoise(projectile);
								}
							}
							projectile.IsInWater = flag2;
						}
						if (this.IsMagma(projectile.Position))
						{
							this.m_subsystemParticles.AddParticleSystem(new MagmaSplashParticleSystem(this.m_subsystemTerrain, projectile.Position, false));
							this.m_subsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, this.m_random.Float(-0.2f, 0.2f), projectile.Position, 3f, true);
							projectile.ToRemove = true;
							this.m_subsystemExplosions.TryExplodeBlock(Terrain.ToCell(projectile.Position.X), Terrain.ToCell(projectile.Position.Y), Terrain.ToCell(projectile.Position.Z), projectile.Value);
						}
						if (this.m_subsystemTime.PeriodicGameTimeEvent(1.0, (double)(projectile.GetHashCode() % 100) / 100.0) && (this.m_subsystemFireBlockBehavior.IsCellOnFire(Terrain.ToCell(projectile.Position.X), Terrain.ToCell(projectile.Position.Y + 0.1f), Terrain.ToCell(projectile.Position.Z)) || this.m_subsystemFireBlockBehavior.IsCellOnFire(Terrain.ToCell(projectile.Position.X), Terrain.ToCell(projectile.Position.Y + 0.1f) - 1, Terrain.ToCell(projectile.Position.Z))))
						{
							this.m_subsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, this.m_random.Float(-0.2f, 0.2f), projectile.Position, 3f, true);
							projectile.ToRemove = true;
							this.m_subsystemExplosions.TryExplodeBlock(Terrain.ToCell(projectile.Position.X), Terrain.ToCell(projectile.Position.Y), Terrain.ToCell(projectile.Position.Z), projectile.Value);
						}
					}
				}
			}
			foreach (Projectile projectile4 in this.m_projectilesToRemove)
			{
				if (projectile4.TrailParticleSystem != null)
				{
					projectile4.TrailParticleSystem.IsStopped = true;
				}
				this.m_projectiles.Remove(projectile4);
				if (this.ProjectileRemoved != null)
				{
					this.ProjectileRemoved(projectile4);
				}
			}
			this.m_projectilesToRemove.Clear();
		}

        // Token: 0x060009E8 RID: 2536 RVA: 0x00047EB0 File Offset: 0x000460B0
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemSoundMaterials = base.Project.FindSubsystem<SubsystemSoundMaterials>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemNoise = base.Project.FindSubsystem<SubsystemNoise>(true);
			this.m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.m_subsystemFluidBlockBehavior = base.Project.FindSubsystem<SubsystemFluidBlockBehavior>(true);
			this.m_subsystemFireBlockBehavior = base.Project.FindSubsystem<SubsystemFireBlockBehavior>(true);
			foreach (object obj in from v in valuesDictionary.GetValue<ValuesDictionary>("Projectiles").Values
			where v is ValuesDictionary
			select v)
			{
				ValuesDictionary valuesDictionary2 = (ValuesDictionary)obj;
				Projectile projectile = new Projectile();
				projectile.Value = valuesDictionary2.GetValue<int>("Value");
				projectile.Position = valuesDictionary2.GetValue<Vector3>("Position");
				projectile.Velocity = valuesDictionary2.GetValue<Vector3>("Velocity");
				projectile.CreationTime = valuesDictionary2.GetValue<double>("CreationTime");
				this.m_projectiles.Add(projectile);
			}
		}

        // Token: 0x060009E9 RID: 2537 RVA: 0x0004807C File Offset: 0x0004627C
        public override void Save(ValuesDictionary valuesDictionary)
		{
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Projectiles", valuesDictionary2);
			int num = 0;
			foreach (Projectile projectile in this.m_projectiles)
			{
				ValuesDictionary valuesDictionary3 = new ValuesDictionary();
				valuesDictionary2.SetValue<ValuesDictionary>(num.ToString(CultureInfo.InvariantCulture), valuesDictionary3);
				valuesDictionary3.SetValue<int>("Value", projectile.Value);
				valuesDictionary3.SetValue<Vector3>("Position", projectile.Position);
				valuesDictionary3.SetValue<Vector3>("Velocity", projectile.Velocity);
				valuesDictionary3.SetValue<double>("CreationTime", projectile.CreationTime);
				num++;
			}
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x00048144 File Offset: 0x00046344
		public bool IsWater(Vector3 position)
		{
			int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
			return BlocksManager.Blocks[cellContents] is WaterBlock;
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x00048194 File Offset: 0x00046394
		public bool IsMagma(Vector3 position)
		{
			int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
			return BlocksManager.Blocks[cellContents] is MagmaBlock;
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x000481E4 File Offset: 0x000463E4
		public void MakeProjectileNoise(Projectile projectile)
		{
			if (this.m_subsystemTime.GameTime - projectile.LastNoiseTime > 0.5)
			{
				this.m_subsystemNoise.MakeNoise(projectile.Position, 0.25f, 6f);
				projectile.LastNoiseTime = this.m_subsystemTime.GameTime;
			}
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x0004823C File Offset: 0x0004643C
		public static void CalculateVelocityAlignMatrix(Block projectileBlock, Vector3 position, Vector3 velocity, out Matrix matrix)
		{
			matrix = Matrix.Identity;
			matrix.Up = Vector3.Normalize(velocity);
			matrix.Right = Vector3.Normalize(Vector3.Cross(matrix.Up, Vector3.UnitY));
			matrix.Forward = Vector3.Normalize(Vector3.Cross(matrix.Up, matrix.Right));
			matrix.Translation = position;
		}

		// Token: 0x04000536 RID: 1334
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000537 RID: 1335
		public SubsystemSoundMaterials m_subsystemSoundMaterials;

		// Token: 0x04000538 RID: 1336
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000539 RID: 1337
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x0400053A RID: 1338
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x0400053B RID: 1339
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400053C RID: 1340
		public SubsystemSky m_subsystemSky;

		// Token: 0x0400053D RID: 1341
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400053E RID: 1342
		public SubsystemNoise m_subsystemNoise;

		// Token: 0x0400053F RID: 1343
		public SubsystemExplosions m_subsystemExplosions;

		// Token: 0x04000540 RID: 1344
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000541 RID: 1345
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x04000542 RID: 1346
		public SubsystemFluidBlockBehavior m_subsystemFluidBlockBehavior;

		// Token: 0x04000543 RID: 1347
		public SubsystemFireBlockBehavior m_subsystemFireBlockBehavior;

		// Token: 0x04000544 RID: 1348
		public List<Projectile> m_projectiles = new List<Projectile>();

		// Token: 0x04000545 RID: 1349
		public List<Projectile> m_projectilesToRemove = new List<Projectile>();

		// Token: 0x04000546 RID: 1350
		public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

		// Token: 0x04000547 RID: 1351
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000548 RID: 1352
		public DrawBlockEnvironmentData m_drawBlockEnvironmentData = new DrawBlockEnvironmentData();

		// Token: 0x04000549 RID: 1353
		public const float BodyInflateAmount = 0.2f;

		// Token: 0x0400054A RID: 1354
		public static int[] m_drawOrders = new int[]
		{
			10
		};
	}
}
