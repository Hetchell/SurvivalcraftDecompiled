using Engine;
using GameEntitySystem;
using System;
using System.Collections.Generic;
using System.Globalization;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F2 RID: 498
	public class ComponentLoot : Component, IUpdateable
	{
		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000E71 RID: 3697 RVA: 0x0006F883 File Offset: 0x0006DA83
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000E72 RID: 3698 RVA: 0x0006F888 File Offset: 0x0006DA88
		public static List<ComponentLoot.Loot> ParseLootList(ValuesDictionary lootVd)
		{
			List<ComponentLoot.Loot> list = new List<ComponentLoot.Loot>();
			foreach (object obj in lootVd.Values)
			{
				string lootString = (string)obj;
				list.Add(ComponentLoot.ParseLoot(lootString));
			}
			list.Sort((ComponentLoot.Loot l1, ComponentLoot.Loot l2) => l1.Value - l2.Value);
			return list;
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x0006F90C File Offset: 0x0006DB0C
		public void Update(float dt)
		{
			if (!this.m_lootDropped && this.m_componentCreature.ComponentHealth.DeathTime != null && this.m_subsystemGameInfo.TotalElapsedGameTime >= this.m_componentCreature.ComponentHealth.DeathTime.Value + (double)this.m_componentCreature.ComponentHealth.CorpseDuration)
			{
				ComponentOnFire componentOnFire = this.m_componentCreature.Entity.FindComponent<ComponentOnFire>();
				bool flag = componentOnFire != null && componentOnFire.IsOnFire;
				this.m_lootDropped = true;
				foreach (ComponentLoot.Loot loot in (flag ? this.m_lootOnFireList : this.m_lootList))
				{
					if (this.m_random.Float(0f, 1f) < loot.Probability)
					{
						int num = this.m_random.Int(loot.MinCount, loot.MaxCount);
						for (int i = 0; i < num; i++)
						{
							Vector3 position = (this.m_componentCreature.ComponentBody.BoundingBox.Min + this.m_componentCreature.ComponentBody.BoundingBox.Max) / 2f;
							this.m_subsystemPickables.AddPickable(loot.Value, 1, position, null, null);
						}
					}
				}
			}
		}

        // Token: 0x06000E74 RID: 3700 RVA: 0x0006FA9C File Offset: 0x0006DC9C
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_lootDropped = valuesDictionary.GetValue<bool>("LootDropped");
			this.m_lootList = ComponentLoot.ParseLootList(valuesDictionary.GetValue<ValuesDictionary>("Loot"));
			this.m_lootOnFireList = ComponentLoot.ParseLootList(valuesDictionary.GetValue<ValuesDictionary>("LootOnFire"));
		}

        // Token: 0x06000E75 RID: 3701 RVA: 0x0006FB1C File Offset: 0x0006DD1C
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<bool>("LootDropped", this.m_lootDropped);
		}

		// Token: 0x06000E76 RID: 3702 RVA: 0x0006FB30 File Offset: 0x0006DD30
		public static ComponentLoot.Loot ParseLoot(string lootString)
		{
			string[] array = lootString.Split(new string[]
			{
				";"
			}, StringSplitOptions.None);
			if (array.Length >= 3)
			{
				int value = CraftingRecipesManager.DecodeResult(array[0]);
				return new ComponentLoot.Loot
				{
					Value = value,
					MinCount = int.Parse(array[1], CultureInfo.InvariantCulture),
					MaxCount = int.Parse(array[2], CultureInfo.InvariantCulture),
					Probability = ((array.Length >= 4) ? float.Parse(array[3], CultureInfo.InvariantCulture) : 1f)
				};
			}
			throw new InvalidOperationException("Invalid loot string.");
		}

		// Token: 0x04000928 RID: 2344
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000929 RID: 2345
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x0400092A RID: 2346
		public ComponentCreature m_componentCreature;

		// Token: 0x0400092B RID: 2347
		public List<ComponentLoot.Loot> m_lootList;

		// Token: 0x0400092C RID: 2348
		public List<ComponentLoot.Loot> m_lootOnFireList;

		// Token: 0x0400092D RID: 2349
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400092E RID: 2350
		public bool m_lootDropped;

		// Token: 0x0200045C RID: 1116
		public struct Loot
		{
			// Token: 0x04001652 RID: 5714
			public int Value;

			// Token: 0x04001653 RID: 5715
			public int MinCount;

			// Token: 0x04001654 RID: 5716
			public int MaxCount;

			// Token: 0x04001655 RID: 5717
			public float Probability;
		}
	}
}
