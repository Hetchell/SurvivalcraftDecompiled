using System;
using System.Collections.Generic;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000172 RID: 370
	public class SubsystemDrawing : Subsystem
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060007F2 RID: 2034 RVA: 0x000340C6 File Offset: 0x000322C6
		public int DrawablesCount
		{
			get
			{
				return this.m_drawables.Count;
			}
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x000340D3 File Offset: 0x000322D3
		public void AddDrawable(IDrawable drawable)
		{
			this.m_drawables.Add(drawable, true);
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x000340E2 File Offset: 0x000322E2
		public void RemoveDrawable(IDrawable drawable)
		{
			this.m_drawables.Remove(drawable);
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x000340F4 File Offset: 0x000322F4
		public void Draw(Camera camera)
		{
			this.m_sortedDrawables.Clear();
			foreach (IDrawable drawable in this.m_drawables.Keys)
			{
				foreach (int key in drawable.DrawOrders)
				{
					this.m_sortedDrawables.Add(key, drawable);
				}
			}
			for (int j = 0; j < this.m_sortedDrawables.Count; j++)
			{
				try
				{
					KeyValuePair<int, IDrawable> keyValuePair = this.m_sortedDrawables[j];
					keyValuePair.Value.Draw(camera, keyValuePair.Key);
				}
				catch (Exception)
				{
				}
			}
		}

        // Token: 0x060007F6 RID: 2038 RVA: 0x000341C8 File Offset: 0x000323C8
        public override void Load(ValuesDictionary valuesDictionary)
		{
			foreach (IDrawable drawable in base.Project.FindSubsystems<IDrawable>())
			{
				this.AddDrawable(drawable);
			}
		}

        // Token: 0x060007F7 RID: 2039 RVA: 0x0003421C File Offset: 0x0003241C
        public override void OnEntityAdded(Entity entity)
		{
			foreach (IDrawable drawable in entity.FindComponents<IDrawable>())
			{
				this.AddDrawable(drawable);
			}
		}

        // Token: 0x060007F8 RID: 2040 RVA: 0x00034274 File Offset: 0x00032474
        public override void OnEntityRemoved(Entity entity)
		{
			foreach (IDrawable drawable in entity.FindComponents<IDrawable>())
			{
				this.RemoveDrawable(drawable);
			}
		}

		// Token: 0x0400042B RID: 1067
		public Dictionary<IDrawable, bool> m_drawables = new Dictionary<IDrawable, bool>();

		// Token: 0x0400042C RID: 1068
		public SortedMultiCollection<int, IDrawable> m_sortedDrawables = new SortedMultiCollection<int, IDrawable>();
	}
}
