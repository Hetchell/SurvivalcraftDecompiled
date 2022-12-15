using System;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000206 RID: 518
	public class ComponentSpawn : Component, IUpdateable
	{
		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06000FC3 RID: 4035 RVA: 0x000789FB File Offset: 0x00076BFB
		// (set) Token: 0x06000FC4 RID: 4036 RVA: 0x00078A03 File Offset: 0x00076C03
		public ComponentFrame ComponentFrame { get; set; }

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06000FC5 RID: 4037 RVA: 0x00078A0C File Offset: 0x00076C0C
		// (set) Token: 0x06000FC6 RID: 4038 RVA: 0x00078A14 File Offset: 0x00076C14
		public ComponentCreature ComponentCreature { get; set; }

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06000FC7 RID: 4039 RVA: 0x00078A1D File Offset: 0x00076C1D
		// (set) Token: 0x06000FC8 RID: 4040 RVA: 0x00078A25 File Offset: 0x00076C25
		public bool AutoDespawn { get; set; }

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06000FC9 RID: 4041 RVA: 0x00078A30 File Offset: 0x00076C30
		public bool IsDespawning
		{
			get
			{
				return this.DespawnTime != null;
			}
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06000FCA RID: 4042 RVA: 0x00078A4B File Offset: 0x00076C4B
		// (set) Token: 0x06000FCB RID: 4043 RVA: 0x00078A53 File Offset: 0x00076C53
		public double SpawnTime { get; set; }

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06000FCC RID: 4044 RVA: 0x00078A5C File Offset: 0x00076C5C
		// (set) Token: 0x06000FCD RID: 4045 RVA: 0x00078A64 File Offset: 0x00076C64
		public double? DespawnTime { get; set; }

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06000FCE RID: 4046 RVA: 0x00078A6D File Offset: 0x00076C6D
		// (set) Token: 0x06000FCF RID: 4047 RVA: 0x00078A75 File Offset: 0x00076C75
		public float SpawnDuration { get; set; }

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06000FD0 RID: 4048 RVA: 0x00078A7E File Offset: 0x00076C7E
		// (set) Token: 0x06000FD1 RID: 4049 RVA: 0x00078A86 File Offset: 0x00076C86
		public float DespawnDuration { get; set; }

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06000FD2 RID: 4050 RVA: 0x00078A8F File Offset: 0x00076C8F
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x06000FD3 RID: 4051 RVA: 0x00078A94 File Offset: 0x00076C94
		// (remove) Token: 0x06000FD4 RID: 4052 RVA: 0x00078ACC File Offset: 0x00076CCC
		public event Action<ComponentSpawn> Despawned;

		// Token: 0x06000FD5 RID: 4053 RVA: 0x00078B04 File Offset: 0x00076D04
		public void Despawn()
		{
			if (this.DespawnTime == null)
			{
				this.DespawnTime = new double?(this.m_subsystemGameInfo.TotalElapsedGameTime);
			}
		}

        // Token: 0x06000FD6 RID: 4054 RVA: 0x00078B38 File Offset: 0x00076D38
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.ComponentFrame = base.Entity.FindComponent<ComponentFrame>(true);
			this.ComponentCreature = base.Entity.FindComponent<ComponentCreature>();
			this.AutoDespawn = valuesDictionary.GetValue<bool>("AutoDespawn");
			double value = valuesDictionary.GetValue<double>("SpawnTime");
			double value2 = valuesDictionary.GetValue<double>("DespawnTime");
			this.SpawnDuration = 2f;
			this.DespawnDuration = 2f;
			this.SpawnTime = ((value < 0.0) ? this.m_subsystemGameInfo.TotalElapsedGameTime : value);
			this.DespawnTime = ((value2 >= 0.0) ? new double?(value2) : null);
		}

        // Token: 0x06000FD7 RID: 4055 RVA: 0x00078BFC File Offset: 0x00076DFC
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<double>("SpawnTime", this.SpawnTime);
			if (this.DespawnTime != null)
			{
				valuesDictionary.SetValue<double>("DespawnTime", this.DespawnTime.Value);
			}
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x00078C44 File Offset: 0x00076E44
		public void Update(float dt)
		{
			if (this.DespawnTime != null && this.m_subsystemGameInfo.TotalElapsedGameTime >= this.DespawnTime.Value + (double)this.DespawnDuration)
			{
				base.Project.RemoveEntity(base.Entity, true);
				if (this.Despawned != null)
				{
					this.Despawned(this);
				}
			}
		}

		// Token: 0x04000A34 RID: 2612
		public SubsystemGameInfo m_subsystemGameInfo;
	}
}
