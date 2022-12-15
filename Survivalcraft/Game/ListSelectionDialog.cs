using System;
using System.Collections;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x020002A5 RID: 677
	public class ListSelectionDialog : Dialog
	{
		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x0600138F RID: 5007 RVA: 0x00097E25 File Offset: 0x00096025
		// (set) Token: 0x06001390 RID: 5008 RVA: 0x00097E32 File Offset: 0x00096032
		public Vector2 ContentSize
		{
			get
			{
				return this.m_contentWidget.Size;
			}
			set
			{
				this.m_contentWidget.Size = value;
			}
		}

		// Token: 0x06001391 RID: 5009 RVA: 0x00097E40 File Offset: 0x00096040
		public ListSelectionDialog(string title, IEnumerable items, float itemSize, Func<object, Widget> itemWidgetFactory, Action<object> selectionHandler)
		{
			this.m_selectionHandler = selectionHandler;
			XElement node = ContentManager.Get<XElement>("Dialogs/ListSelectionDialog");
			base.LoadContents(this, node);
			this.m_titleLabelWidget = this.Children.Find<LabelWidget>("ListSelectionDialog.Title", true);
			this.m_listWidget = this.Children.Find<ListPanelWidget>("ListSelectionDialog.List", true);
			this.m_contentWidget = this.Children.Find<CanvasWidget>("ListSelectionDialog.Content", true);
			this.m_titleLabelWidget.Text = title;
			this.m_titleLabelWidget.IsVisible = !string.IsNullOrEmpty(title);
			this.m_listWidget.ItemSize = itemSize;
			if (itemWidgetFactory != null)
			{
				this.m_listWidget.ItemWidgetFactory = itemWidgetFactory;
			}
			foreach (object item in items)
			{
				this.m_listWidget.AddItem(item);
			}
			for (int i = this.m_listWidget.Items.Count; i >= 0; i--)
			{
				float num = MathUtils.Min((float)i + 0.5f, (float)this.m_listWidget.Items.Count);
				if (num * itemSize <= this.m_contentWidget.Size.Y)
				{
					this.m_contentWidget.Size = new Vector2(this.m_contentWidget.Size.X, num * itemSize);
					return;
				}
			}
		}

		// Token: 0x06001392 RID: 5010 RVA: 0x00097FB8 File Offset: 0x000961B8
		public ListSelectionDialog(string title, IEnumerable items, float itemSize, Func<object, string> itemToStringConverter, Action<object> selectionHandler) : this(title, items, itemSize, (object item) => new LabelWidget
		{
			Text = itemToStringConverter(item),
			HorizontalAlignment = WidgetAlignment.Center,
			VerticalAlignment = WidgetAlignment.Center
		}, selectionHandler)
		{
		}

		// Token: 0x06001393 RID: 5011 RVA: 0x00097FEC File Offset: 0x000961EC
		public override void Update()
		{
			if (base.Input.Back || base.Input.Cancel)
			{
				this.m_dismissTime = new double?(0.0);
			}
			else if (base.Input.Tap != null && !this.m_listWidget.HitTest(base.Input.Tap.Value))
			{
				this.m_dismissTime = new double?(0.0);
			}
			else if (this.m_dismissTime == null && this.m_listWidget.SelectedItem != null)
			{
				this.m_dismissTime = new double?(Time.FrameStartTime + 0.05000000074505806);
			}
			if (this.m_dismissTime != null && Time.FrameStartTime >= this.m_dismissTime.Value)
			{
				this.Dismiss(this.m_listWidget.SelectedItem);
			}
		}

		// Token: 0x06001394 RID: 5012 RVA: 0x000980D8 File Offset: 0x000962D8
		public void Dismiss(object result)
		{
			if (!this.m_isDismissed)
			{
				this.m_isDismissed = true;
				DialogsManager.HideDialog(this);
				if (this.m_selectionHandler != null && result != null)
				{
					this.m_selectionHandler(result);
				}
			}
		}

		// Token: 0x04000D66 RID: 3430
		public Action<object> m_selectionHandler;

		// Token: 0x04000D67 RID: 3431
		public LabelWidget m_titleLabelWidget;

		// Token: 0x04000D68 RID: 3432
		public ListPanelWidget m_listWidget;

		// Token: 0x04000D69 RID: 3433
		public CanvasWidget m_contentWidget;

		// Token: 0x04000D6A RID: 3434
		public double? m_dismissTime;

		// Token: 0x04000D6B RID: 3435
		public bool m_isDismissed;
	}
}
