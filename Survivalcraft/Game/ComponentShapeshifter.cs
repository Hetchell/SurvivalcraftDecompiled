using System;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000202 RID: 514
	public class ComponentShapeshifter : Component, IUpdateable
	{
		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06000F9F RID: 3999 RVA: 0x00077612 File Offset: 0x00075812
		// (set) Token: 0x06000FA0 RID: 4000 RVA: 0x0007761A File Offset: 0x0007581A
		public bool IsEnabled { get; set; }

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06000FA1 RID: 4001 RVA: 0x00077623 File Offset: 0x00075823
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000FA2 RID: 4002 RVA: 0x00077628 File Offset: 0x00075828
		public void Update(float dt)
		{
			bool areSupernaturalCreaturesEnabled = this.m_subsystemGameInfo.WorldSettings.AreSupernaturalCreaturesEnabled;
			if (this.IsEnabled && !this.m_componentSpawn.IsDespawning && this.m_componentHealth.Health > 0f)
			{
				if (!areSupernaturalCreaturesEnabled && !string.IsNullOrEmpty(this.m_dayEntityTemplateName))
				{
					this.ShapeshiftTo(this.m_dayEntityTemplateName);
				}
				else if (this.m_subsystemSky.SkyLightIntensity > 0.25f && !string.IsNullOrEmpty(this.m_dayEntityTemplateName))
				{
					this.m_timeToSwitch -= 2f * dt;
					if (this.m_timeToSwitch <= 0f)
					{
						this.ShapeshiftTo(this.m_dayEntityTemplateName);
					}
				}
				else if (areSupernaturalCreaturesEnabled && this.m_subsystemSky.SkyLightIntensity < 0.1f && (this.m_subsystemSky.MoonPhase == 0 || this.m_subsystemSky.MoonPhase == 4) && !string.IsNullOrEmpty(this.m_nightEntityTemplateName))
				{
					this.m_timeToSwitch -= dt;
					if (this.m_timeToSwitch <= 0f)
					{
						this.ShapeshiftTo(this.m_nightEntityTemplateName);
					}
				}
			}
			if (!string.IsNullOrEmpty(this.m_spawnEntityTemplateName))
			{
				if (this.m_particleSystem == null)
				{
					this.m_particleSystem = new ShapeshiftParticleSystem();
					this.m_subsystemParticles.AddParticleSystem(this.m_particleSystem);
				}
				this.m_particleSystem.BoundingBox = this.m_componentBody.BoundingBox;
			}
		}

        // Token: 0x06000FA3 RID: 4003 RVA: 0x00077790 File Offset: 0x00075990
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_componentSpawn = base.Entity.FindComponent<ComponentSpawn>(true);
			this.m_componentBody = base.Entity.FindComponent<ComponentBody>(true);
			this.m_componentHealth = base.Entity.FindComponent<ComponentHealth>(true);
			this.m_dayEntityTemplateName = valuesDictionary.GetValue<string>("DayEntityTemplateName");
			this.m_nightEntityTemplateName = valuesDictionary.GetValue<string>("NightEntityTemplateName");
			float value = valuesDictionary.GetValue<float>("Probability");
			if (!string.IsNullOrEmpty(this.m_dayEntityTemplateName))
			{
				DatabaseManager.FindEntityValuesDictionary(this.m_dayEntityTemplateName, true);
			}
			if (!string.IsNullOrEmpty(this.m_nightEntityTemplateName))
			{
				DatabaseManager.FindEntityValuesDictionary(this.m_nightEntityTemplateName, true);
			}
			this.m_timeToSwitch = ComponentShapeshifter.s_random.Float(3f, 15f);
			this.IsEnabled = (ComponentShapeshifter.s_random.Float(0f, 1f) < value);
			this.m_componentSpawn.Despawned += this.ComponentSpawn_Despawned;
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x000778CC File Offset: 0x00075ACC
		public void ShapeshiftTo(string entityTemplateName)
		{
			if (string.IsNullOrEmpty(this.m_spawnEntityTemplateName))
			{
				this.m_spawnEntityTemplateName = entityTemplateName;
				this.m_componentSpawn.DespawnDuration = 3f;
				this.m_componentSpawn.Despawn();
				this.m_subsystemAudio.PlaySound("Audio/Shapeshift", 1f, 0f, this.m_componentBody.Position, 3f, true);
			}
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x00077934 File Offset: 0x00075B34
		public void ComponentSpawn_Despawned(ComponentSpawn componentSpawn)
		{
			if (this.m_componentHealth.Health > 0f && !string.IsNullOrEmpty(this.m_spawnEntityTemplateName))
			{
				Entity entity = DatabaseManager.CreateEntity(base.Project, this.m_spawnEntityTemplateName, true);
				ComponentBody componentBody = entity.FindComponent<ComponentBody>(true);
				componentBody.Position = this.m_componentBody.Position;
				componentBody.Rotation = this.m_componentBody.Rotation;
				componentBody.setVectorSpeed(this.m_componentBody.getVectorSpeed());
				entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0.5f;
				base.Project.AddEntity(entity);
			}
			if (this.m_particleSystem != null)
			{
				this.m_particleSystem.Stopped = true;
			}
		}

		// Token: 0x04000A0A RID: 2570
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000A0B RID: 2571
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000A0C RID: 2572
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000A0D RID: 2573
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000A0E RID: 2574
		public ComponentSpawn m_componentSpawn;

		// Token: 0x04000A0F RID: 2575
		public ComponentBody m_componentBody;

		// Token: 0x04000A10 RID: 2576
		public ComponentHealth m_componentHealth;

		// Token: 0x04000A11 RID: 2577
		public ShapeshiftParticleSystem m_particleSystem;

		// Token: 0x04000A12 RID: 2578
		public string m_nightEntityTemplateName;

		// Token: 0x04000A13 RID: 2579
		public string m_dayEntityTemplateName;

		// Token: 0x04000A14 RID: 2580
		public float m_timeToSwitch;

		// Token: 0x04000A15 RID: 2581
		public string m_spawnEntityTemplateName;

		// Token: 0x04000A16 RID: 2582
		public static Random s_random = new Random();
	}
}
