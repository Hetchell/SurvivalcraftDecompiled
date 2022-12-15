using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000193 RID: 403
	public class SubsystemMusketBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600095D RID: 2397 RVA: 0x00041779 File Offset: 0x0003F979
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x00041781 File Offset: 0x0003F981
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			componentPlayer.ComponentGui.ModalPanelWidget = ((componentPlayer.ComponentGui.ModalPanelWidget == null) ? new MusketWidget(inventory, slotIndex) : null);
			return true;
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x000417A8 File Offset: 0x0003F9A8
		public override bool OnAim(Ray3 aim, ComponentMiner componentMiner, AimState state)
		{
			IInventory inventory = componentMiner.Inventory;
			if (inventory != null)
			{
				int activeSlotIndex = inventory.ActiveSlotIndex;
				if (activeSlotIndex >= 0)
				{
					int slotValue = inventory.GetSlotValue(activeSlotIndex);
					int slotCount = inventory.GetSlotCount(activeSlotIndex);
					int num = Terrain.ExtractContents(slotValue);
					int data = Terrain.ExtractData(slotValue);
					int num2 = slotValue;
					int num3 = 0;
					if (num == 212 && slotCount > 0)
					{
						double gameTime;
						if (!this.m_aimStartTimes.TryGetValue(componentMiner, out gameTime))
						{
							gameTime = this.m_subsystemTime.GameTime;
							this.m_aimStartTimes[componentMiner] = gameTime;
						}
						float num4 = (float)(this.m_subsystemTime.GameTime - gameTime);
						float num5 = (float)MathUtils.Remainder(this.m_subsystemTime.GameTime, 1000.0);
						Vector3 v = ((componentMiner.ComponentCreature.ComponentBody.IsSneaking ? 0.01f : 0.03f) + 0.2f * MathUtils.Saturate((num4 - 2.5f) / 6f)) * new Vector3
						{
							X = SimplexNoise.OctavedNoise(num5, 2f, 3, 2f, 0.5f, false),
							Y = SimplexNoise.OctavedNoise(num5 + 100f, 2f, 3, 2f, 0.5f, false),
							Z = SimplexNoise.OctavedNoise(num5 + 200f, 2f, 3, 2f, 0.5f, false)
						};
						aim.Direction = Vector3.Normalize(aim.Direction + v);
						switch (state)
						{
						case AimState.InProgress:
						{
							if (num4 >= 10f)
							{
								componentMiner.ComponentCreature.ComponentCreatureSounds.PlayMoanSound();
								return true;
							}
							if (num4 > 0.5f && !MusketBlock.GetHammerState(Terrain.ExtractData(num2)))
							{
								num2 = Terrain.MakeBlockValue(num, 0, MusketBlock.SetHammerState(Terrain.ExtractData(num2), true));
								this.m_subsystemAudio.PlaySound("Audio/HammerCock", 1f, this.m_random.Float(-0.1f, 0.1f), 0f, 0f);
							}
							ComponentFirstPersonModel componentFirstPersonModel = componentMiner.Entity.FindComponent<ComponentFirstPersonModel>();
							if (componentFirstPersonModel != null)
							{
								ComponentPlayer componentPlayer = componentMiner.ComponentPlayer;
								if (componentPlayer != null)
								{
									componentPlayer.ComponentAimingSights.ShowAimingSights(aim.Position, aim.Direction);
								}
								componentFirstPersonModel.ItemOffsetOrder = new Vector3(-0.21f, 0.15f, 0.08f);
								componentFirstPersonModel.ItemRotationOrder = new Vector3(-0.7f, 0f, 0f);
							}
							componentMiner.ComponentCreature.ComponentCreatureModel.AimHandAngleOrder = 1.4f;
							componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemOffsetOrder = new Vector3(-0.08f, -0.08f, 0.07f);
							componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemRotationOrder = new Vector3(-1.7f, 0f, 0f);
							break;
						}
						case AimState.Cancelled:
							if (MusketBlock.GetHammerState(Terrain.ExtractData(num2)))
							{
								num2 = Terrain.MakeBlockValue(num, 0, MusketBlock.SetHammerState(Terrain.ExtractData(num2), false));
								this.m_subsystemAudio.PlaySound("Audio/HammerUncock", 1f, this.m_random.Float(-0.1f, 0.1f), 0f, 0f);
							}
							this.m_aimStartTimes.Remove(componentMiner);
							break;
						case AimState.Completed:
						{
							bool flag = false;
							int value = 0;
							int num6 = 0;
							float s = 0f;
							Vector3 zero = Vector3.Zero;
							MusketBlock.LoadState loadState = MusketBlock.GetLoadState(data);
							BulletBlock.BulletType? bulletType = MusketBlock.GetBulletType(data);
							if (MusketBlock.GetHammerState(Terrain.ExtractData(num2)))
							{
								switch (loadState)
								{
								case MusketBlock.LoadState.Empty:
								{
									ComponentPlayer componentPlayer2 = componentMiner.ComponentPlayer;
									if (componentPlayer2 != null)
									{
										componentPlayer2.ComponentGui.DisplaySmallMessage(LanguageControl.Get(SubsystemMusketBlockBehavior.fName, 0), Color.White, true, false);
									}
									break;
								}
								case MusketBlock.LoadState.Gunpowder:
								case MusketBlock.LoadState.Wad:
								{
									flag = true;
									ComponentPlayer componentPlayer3 = componentMiner.ComponentPlayer;
									if (componentPlayer3 != null)
									{
										componentPlayer3.ComponentGui.DisplaySmallMessage(LanguageControl.Get(SubsystemMusketBlockBehavior.fName, 1), Color.White, true, false);
									}
									break;
								}
								case MusketBlock.LoadState.Loaded:
								{
									flag = true;
									BulletBlock.BulletType? bulletType2 = bulletType;
									BulletBlock.BulletType bulletType3 = BulletBlock.BulletType.Buckshot;
									if (bulletType2.GetValueOrDefault() == bulletType3 & bulletType2 != null)
									{
										value = Terrain.MakeBlockValue(214, 0, BulletBlock.SetBulletType(0, BulletBlock.BulletType.BuckshotBall));
										num6 = 8;
										zero = new Vector3(0.04f, 0.04f, 0.25f);
										s = 80f;
									}
									else
									{
										bulletType2 = bulletType;
										bulletType3 = BulletBlock.BulletType.BuckshotBall;
										if (bulletType2.GetValueOrDefault() == bulletType3 & bulletType2 != null)
										{
											value = Terrain.MakeBlockValue(214, 0, BulletBlock.SetBulletType(0, BulletBlock.BulletType.BuckshotBall));
											num6 = 1;
											zero = new Vector3(0.06f, 0.06f, 0f);
											s = 60f;
										}
										else if (bulletType != null)
										{
											value = Terrain.MakeBlockValue(214, 0, BulletBlock.SetBulletType(0, bulletType.Value));
											num6 = 1;
											s = 120f;
										}
									}
									break;
								}
								}
							}
							if (flag)
							{
								if (componentMiner.ComponentCreature.ComponentBody.ImmersionFactor > 0.4f)
								{
									this.m_subsystemAudio.PlaySound("Audio/MusketMisfire", 1f, this.m_random.Float(-0.1f, 0.1f), componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition, 3f, true);
								}
								else
								{
									Vector3 vector = componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition + componentMiner.ComponentCreature.ComponentBody.Matrix.Right * 0.3f - componentMiner.ComponentCreature.ComponentBody.Matrix.Up * 0.2f;
									Vector3 vector2 = Vector3.Normalize(vector + aim.Direction * 10f - vector);
									Vector3 vector3 = Vector3.Normalize(Vector3.Cross(vector2, Vector3.UnitY));
									Vector3 v2 = Vector3.Normalize(Vector3.Cross(vector2, vector3));
									for (int i = 0; i < num6; i++)
									{
										Vector3 v3 = this.m_random.Float(0f - zero.X, zero.X) * vector3 + this.m_random.Float(0f - zero.Y, zero.Y) * v2 + this.m_random.Float(0f - zero.Z, zero.Z) * vector2;
										Projectile projectile = this.m_subsystemProjectiles.FireProjectile(value, vector, s * (vector2 + v3), Vector3.Zero, componentMiner.ComponentCreature);
										if (projectile != null)
										{
											projectile.ProjectileStoppedAction = ProjectileStoppedAction.Disappear;
										}
									}
									this.m_subsystemAudio.PlaySound("Audio/MusketFire", 1f, this.m_random.Float(-0.1f, 0.1f), componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition, 10f, true);
									this.m_subsystemParticles.AddParticleSystem(new GunSmokeParticleSystem(this.m_subsystemTerrain, vector + 0.3f * vector2, vector2));
									this.m_subsystemNoise.MakeNoise(vector, 1f, 40f);
									componentMiner.ComponentCreature.ComponentBody.ApplyImpulse(-4f * vector2);
								}
								num2 = Terrain.MakeBlockValue(Terrain.ExtractContents(num2), 0, MusketBlock.SetLoadState(Terrain.ExtractData(num2), MusketBlock.LoadState.Empty));
								num3 = 1;
							}
							if (MusketBlock.GetHammerState(Terrain.ExtractData(num2)))
							{
								num2 = Terrain.MakeBlockValue(Terrain.ExtractContents(num2), 0, MusketBlock.SetHammerState(Terrain.ExtractData(num2), false));
								this.m_subsystemAudio.PlaySound("Audio/HammerRelease", 1f, this.m_random.Float(-0.1f, 0.1f), 0f, 0f);
							}
							this.m_aimStartTimes.Remove(componentMiner);
							break;
						}
						}
					}
					if (num2 != slotValue)
					{
						inventory.RemoveSlotItems(activeSlotIndex, 1);
						inventory.AddSlotItems(activeSlotIndex, num2, 1);
					}
					if (num3 > 0)
					{
						componentMiner.DamageActiveTool(num3);
					}
				}
			}
			return false;
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x00041FAC File Offset: 0x000401AC
		public override int GetProcessInventoryItemCapacity(IInventory inventory, int slotIndex, int value)
		{
			int num = Terrain.ExtractContents(value);
			MusketBlock.LoadState loadState = MusketBlock.GetLoadState(Terrain.ExtractData(inventory.GetSlotValue(slotIndex)));
			if (loadState == MusketBlock.LoadState.Empty && num == 109)
			{
				return 1;
			}
			if (loadState == MusketBlock.LoadState.Gunpowder && num == 205)
			{
				return 1;
			}
			if (loadState == MusketBlock.LoadState.Wad && num == 214)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x00041FFC File Offset: 0x000401FC
		public override void ProcessInventoryItem(IInventory inventory, int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			processedValue = value;
			processedCount = count;
			if (processCount == 1)
			{
				int data = Terrain.ExtractData(inventory.GetSlotValue(slotIndex));
				MusketBlock.LoadState loadState = MusketBlock.GetLoadState(data);
				BulletBlock.BulletType? bulletType = MusketBlock.GetBulletType(data);
				switch (loadState)
				{
				case MusketBlock.LoadState.Empty:
					loadState = MusketBlock.LoadState.Gunpowder;
					bulletType = null;
					break;
				case MusketBlock.LoadState.Gunpowder:
					loadState = MusketBlock.LoadState.Wad;
					bulletType = null;
					break;
				case MusketBlock.LoadState.Wad:
				{
					loadState = MusketBlock.LoadState.Loaded;
					int data2 = Terrain.ExtractData(value);
					bulletType = new BulletBlock.BulletType?(BulletBlock.GetBulletType(data2));
					break;
				}
				}
				processedValue = 0;
				processedCount = 0;
				inventory.RemoveSlotItems(slotIndex, 1);
				inventory.AddSlotItems(slotIndex, Terrain.MakeBlockValue(212, 0, MusketBlock.SetBulletType(MusketBlock.SetLoadState(data, loadState), bulletType)), 1);
			}
		}

        // Token: 0x06000962 RID: 2402 RVA: 0x000420A8 File Offset: 0x000402A8
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemProjectiles = base.Project.FindSubsystem<SubsystemProjectiles>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemNoise = base.Project.FindSubsystem<SubsystemNoise>(true);
			base.Load(valuesDictionary);
		}

		// Token: 0x040004EF RID: 1263
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040004F0 RID: 1264
		public SubsystemTime m_subsystemTime;

		// Token: 0x040004F1 RID: 1265
		public SubsystemProjectiles m_subsystemProjectiles;

		// Token: 0x040004F2 RID: 1266
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x040004F3 RID: 1267
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040004F4 RID: 1268
		public SubsystemNoise m_subsystemNoise;

		// Token: 0x040004F5 RID: 1269
		public static string fName = "SubsystemMusketBlockBehavior";

		// Token: 0x040004F6 RID: 1270
		public Game.Random m_random = new Game.Random();

		// Token: 0x040004F7 RID: 1271
		public Dictionary<ComponentMiner, double> m_aimStartTimes = new Dictionary<ComponentMiner, double>();
	}
}
