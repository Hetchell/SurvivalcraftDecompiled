using System;
using System.Collections.Generic;
using System.Diagnostics;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001B6 RID: 438
	public class SubsystemUpdate : Subsystem
	{
		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000ADE RID: 2782 RVA: 0x00051263 File Offset: 0x0004F463
		public int UpdateablesCount
		{
			get
			{
				return this.m_updateables.Count;
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000ADF RID: 2783 RVA: 0x00051270 File Offset: 0x0004F470
		// (set) Token: 0x06000AE0 RID: 2784 RVA: 0x00051278 File Offset: 0x0004F478
		public int UpdatesPerFrame { get; set; }

		// Token: 0x06000AE1 RID: 2785 RVA: 0x00051284 File Offset: 0x0004F484
		public void Update()
		{
			for (int i = 0; i < this.UpdatesPerFrame; i++)
			{
				this.m_subsystemTime.NextFrame();
				bool flag = false;
				foreach (KeyValuePair<IUpdateable, bool> keyValuePair in this.m_toAddOrRemove)
				{
					if (keyValuePair.Value)
					{
						this.m_updateables.Add(keyValuePair.Key, new SubsystemUpdate.UpdateableInfo
						{
							UpdateOrder = keyValuePair.Key.UpdateOrder
						});
						flag = true;
					}
					else
					{
						this.m_updateables.Remove(keyValuePair.Key);
						flag = true;
					}
				}
				this.m_toAddOrRemove.Clear();
				foreach (KeyValuePair<IUpdateable, SubsystemUpdate.UpdateableInfo> keyValuePair2 in this.m_updateables)
				{
					UpdateOrder updateOrder = keyValuePair2.Key.UpdateOrder;
					if (updateOrder != keyValuePair2.Value.UpdateOrder)
					{
						flag = true;
						keyValuePair2.Value.UpdateOrder = updateOrder;
					}
				}
				if (flag)
				{
					this.m_sortedUpdateables.Clear();
					foreach (IUpdateable item in this.m_updateables.Keys)
					{
						this.m_sortedUpdateables.Add(item);
					}
					this.m_sortedUpdateables.Sort(SubsystemUpdate.Comparer.Instance);
				}
				float dt = MathUtils.Clamp(this.m_subsystemTime.GameTimeDelta, 0f, 0.1f);
				int u = 0;
				foreach (IUpdateable updateable in this.m_sortedUpdateables)
				{
					try
					{
						u++;
						//Debug.WriteLine("Element " + u + " slot: " + updateable.ToString());
						updateable.Update(dt);
					}
					catch (Exception)
					{
					}
				}
			}
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x00051494 File Offset: 0x0004F694
		public void AddUpdateable(IUpdateable updateable)
		{
			this.m_toAddOrRemove[updateable] = true;
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x000514A3 File Offset: 0x0004F6A3
		public void RemoveUpdateable(IUpdateable updateable)
		{
			this.m_toAddOrRemove[updateable] = false;
		}

        // Token: 0x06000AE4 RID: 2788 RVA: 0x000514B4 File Offset: 0x0004F6B4
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			foreach (IUpdateable updateable in base.Project.FindSubsystems<IUpdateable>())
			{
				this.AddUpdateable(updateable);
			}
			this.UpdatesPerFrame = 1;
		}

        // Token: 0x06000AE5 RID: 2789 RVA: 0x00051520 File Offset: 0x0004F720
        public override void OnEntityAdded(Entity entity)
		{
			foreach (IUpdateable updateable in entity.FindComponents<IUpdateable>())
			{
				this.AddUpdateable(updateable);
			}
		}

        // Token: 0x06000AE6 RID: 2790 RVA: 0x00051578 File Offset: 0x0004F778
        public override void OnEntityRemoved(Entity entity)
		{
			foreach (IUpdateable updateable in entity.FindComponents<IUpdateable>())
			{
				this.RemoveUpdateable(updateable);
			}
		}

		// Token: 0x040005F0 RID: 1520
		public SubsystemTime m_subsystemTime;

		// Token: 0x040005F1 RID: 1521
		public Dictionary<IUpdateable, SubsystemUpdate.UpdateableInfo> m_updateables = new Dictionary<IUpdateable, SubsystemUpdate.UpdateableInfo>();

		// Token: 0x040005F2 RID: 1522
		public Dictionary<IUpdateable, bool> m_toAddOrRemove = new Dictionary<IUpdateable, bool>();

		// Token: 0x040005F3 RID: 1523
		public List<IUpdateable> m_sortedUpdateables = new List<IUpdateable>();

		// Token: 0x02000446 RID: 1094
		public class UpdateableInfo
		{
			// Token: 0x04001622 RID: 5666
			public UpdateOrder UpdateOrder;
		}

		// Token: 0x02000447 RID: 1095
		public class Comparer : IComparer<IUpdateable>
		{
			// Token: 0x06001EBB RID: 7867 RVA: 0x000DFF7C File Offset: 0x000DE17C
			public int Compare(IUpdateable u1, IUpdateable u2)
			{
				int num = u1.UpdateOrder - u2.UpdateOrder;
				if (num != 0)
				{
					return num;
				}
				return u1.GetHashCode() - u2.GetHashCode();
			}

			// Token: 0x04001623 RID: 5667
			public static SubsystemUpdate.Comparer Instance = new SubsystemUpdate.Comparer();
		}
	}
}
