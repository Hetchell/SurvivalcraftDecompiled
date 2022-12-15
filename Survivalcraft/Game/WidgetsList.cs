using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Game
{
	// Token: 0x020003A9 RID: 937
	public class WidgetsList : IEnumerable<Widget>, IEnumerable
	{
		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x06001C2F RID: 7215 RVA: 0x000D99D3 File Offset: 0x000D7BD3
		public int Count
		{
			get
			{
				return this.m_widgets.Count;
			}
		}

		// Token: 0x170004FF RID: 1279
		public Widget this[int index]
		{
			get
			{
				return this.m_widgets[index];
			}
		}

		// Token: 0x06001C31 RID: 7217 RVA: 0x000D99EE File Offset: 0x000D7BEE
		public WidgetsList(ContainerWidget containerWidget)
		{
			this.m_containerWidget = containerWidget;
		}

		// Token: 0x06001C32 RID: 7218 RVA: 0x000D9A08 File Offset: 0x000D7C08
		public void Add(Widget widget)
		{
			this.Insert(this.Count, widget);
		}

		// Token: 0x06001C33 RID: 7219 RVA: 0x000D9A17 File Offset: 0x000D7C17
		public void Add(params Widget[] widgets)
		{
			this.AddRange(widgets);
		}

		// Token: 0x06001C34 RID: 7220 RVA: 0x000D9A20 File Offset: 0x000D7C20
		public void AddRange(IEnumerable<Widget> widgets)
		{
			foreach (Widget widget in widgets)
			{
				this.Add(widget);
			}
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x000D9A68 File Offset: 0x000D7C68
		public void Insert(int index, Widget widget)
		{
			if (this.m_widgets.Contains(widget))
			{
				throw new InvalidOperationException("Child widget already present in container.");
			}
			if (index < 0 || index > this.m_widgets.Count)
			{
				throw new InvalidOperationException("Widget index out of range.");
			}
			widget.ChangeParent(this.m_containerWidget);
			this.m_widgets.Insert(index, widget);
			this.m_containerWidget.WidgetAdded(widget);
			this.m_version++;
		}

		// Token: 0x06001C36 RID: 7222 RVA: 0x000D9AE0 File Offset: 0x000D7CE0
		public void InsertBefore(Widget beforeWidget, Widget widget)
		{
			int num = this.m_widgets.IndexOf(beforeWidget);
			if (num < 0)
			{
				throw new InvalidOperationException("Widget not present in container.");
			}
			this.Insert(num, widget);
		}

		// Token: 0x06001C37 RID: 7223 RVA: 0x000D9B14 File Offset: 0x000D7D14
		public void InsertAfter(Widget afterWidget, Widget widget)
		{
			int num = this.m_widgets.IndexOf(afterWidget);
			if (num < 0)
			{
				throw new InvalidOperationException("Widget not present in container.");
			}
			this.Insert(num + 1, widget);
		}

		// Token: 0x06001C38 RID: 7224 RVA: 0x000D9B48 File Offset: 0x000D7D48
		public void Remove(Widget widget)
		{
			int num = this.IndexOf(widget);
			if (num >= 0)
			{
				this.RemoveAt(num);
				return;
			}
			throw new InvalidOperationException("Child widget not present in container.");
		}

		// Token: 0x06001C39 RID: 7225 RVA: 0x000D9B74 File Offset: 0x000D7D74
		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.m_widgets.Count)
			{
				throw new InvalidOperationException("Widget index out of range.");
			}
			Widget widget = this.m_widgets[index];
			widget.ChangeParent(null);
			this.m_widgets.RemoveAt(index);
			this.m_containerWidget.WidgetRemoved(widget);
			this.m_version--;
		}

		// Token: 0x06001C3A RID: 7226 RVA: 0x000D9BD8 File Offset: 0x000D7DD8
		public void Clear()
		{
			while (this.Count > 0)
			{
				this.RemoveAt(this.Count - 1);
			}
		}

		// Token: 0x06001C3B RID: 7227 RVA: 0x000D9BF3 File Offset: 0x000D7DF3
		public int IndexOf(Widget widget)
		{
			return this.m_widgets.IndexOf(widget);
		}

		// Token: 0x06001C3C RID: 7228 RVA: 0x000D9C01 File Offset: 0x000D7E01
		public bool Contains(Widget widget)
		{
			return this.m_widgets.Contains(widget);
		}

		// Token: 0x06001C3D RID: 7229 RVA: 0x000D9C10 File Offset: 0x000D7E10
		public Widget Find(string name, Type type, bool throwIfNotFound = true)
		{
			foreach (Widget widget in this.m_widgets)
			{
				if ((name == null || (widget.Name != null && widget.Name == name)) && (type == null || type == widget.GetType() || widget.GetType().GetTypeInfo().IsSubclassOf(type)))
				{
					return widget;
				}
				ContainerWidget containerWidget = widget as ContainerWidget;
				if (containerWidget != null)
				{
					Widget widget2 = containerWidget.Children.Find(name, type, false);
					if (widget2 != null)
					{
						return widget2;
					}
				}
			}
			if (throwIfNotFound)
			{
				throw new Exception(string.Format("Required widget \"{0}\" of type \"{1}\" not found.", name, type));
			}
			return null;
		}

		// Token: 0x06001C3E RID: 7230 RVA: 0x000D9CE0 File Offset: 0x000D7EE0
		public Widget Find(string name, bool throwIfNotFound = true)
		{
			return this.Find(name, null, throwIfNotFound);
		}

		// Token: 0x06001C3F RID: 7231 RVA: 0x000D9CEB File Offset: 0x000D7EEB
		public T Find<T>(string name, bool throwIfNotFound = true) where T : class
		{
			return this.Find(name, typeof(T), throwIfNotFound) as T;
		}

		// Token: 0x06001C40 RID: 7232 RVA: 0x000D9D09 File Offset: 0x000D7F09
		public T Find<T>(bool throwIfNotFound = true) where T : class
		{
			return this.Find(null, typeof(T), throwIfNotFound) as T;
		}

		// Token: 0x06001C41 RID: 7233 RVA: 0x000D9D27 File Offset: 0x000D7F27
		public WidgetsList.Enumerator GetEnumerator()
		{
			return new WidgetsList.Enumerator(this);
		}

		// Token: 0x06001C42 RID: 7234 RVA: 0x000D9D2F File Offset: 0x000D7F2F
		IEnumerator<Widget> IEnumerable<Widget>.GetEnumerator()
		{
			return new WidgetsList.Enumerator(this);
		}

		// Token: 0x06001C43 RID: 7235 RVA: 0x000D9D3C File Offset: 0x000D7F3C
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new WidgetsList.Enumerator(this);
		}

		// Token: 0x04001386 RID: 4998
		public ContainerWidget m_containerWidget;

		// Token: 0x04001387 RID: 4999
		public List<Widget> m_widgets = new List<Widget>();

		// Token: 0x04001388 RID: 5000
		public int m_version;

		// Token: 0x0200052D RID: 1325
		public struct Enumerator : IEnumerator<Widget>, IDisposable, IEnumerator
		{
			// Token: 0x1700056E RID: 1390
			// (get) Token: 0x0600216A RID: 8554 RVA: 0x000E71D6 File Offset: 0x000E53D6
			public Widget Current
			{
				get
				{
					return this.m_current;
				}
			}

			// Token: 0x1700056F RID: 1391
			// (get) Token: 0x0600216B RID: 8555 RVA: 0x000E71DE File Offset: 0x000E53DE
			object IEnumerator.Current
			{
				get
				{
					return this.m_current;
				}
			}

			// Token: 0x0600216C RID: 8556 RVA: 0x000E71E6 File Offset: 0x000E53E6
			public Enumerator(WidgetsList collection)
			{
				this.m_collection = collection;
				this.m_current = null;
				this.m_index = 0;
				this.m_version = collection.m_version;
			}

			// Token: 0x0600216D RID: 8557 RVA: 0x000E7209 File Offset: 0x000E5409
			public void Dispose()
			{
			}

			// Token: 0x0600216E RID: 8558 RVA: 0x000E720C File Offset: 0x000E540C
			public bool MoveNext()
			{
				if (this.m_collection.m_version != this.m_version)
				{
					throw new InvalidOperationException("WidgetsList was modified, enumeration cannot continue.");
				}
				if (this.m_index < this.m_collection.m_widgets.Count)
				{
					this.m_current = this.m_collection.m_widgets[this.m_index];
					this.m_index++;
					return true;
				}
				this.m_current = null;
				return false;
			}

			// Token: 0x0600216F RID: 8559 RVA: 0x000E7283 File Offset: 0x000E5483
			public void Reset()
			{
				if (this.m_collection.m_version != this.m_version)
				{
					throw new InvalidOperationException("SortedMultiCollection was modified, enumeration cannot continue.");
				}
				this.m_index = 0;
				this.m_current = null;
			}

			// Token: 0x0400190B RID: 6411
			public WidgetsList m_collection;

			// Token: 0x0400190C RID: 6412
			public Widget m_current;

			// Token: 0x0400190D RID: 6413
			public int m_index;

			// Token: 0x0400190E RID: 6414
			public int m_version;
		}
	}
}
