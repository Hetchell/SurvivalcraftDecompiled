using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Engine;
using Engine.Media;

namespace Game
{
	// Token: 0x0200034F RID: 847
	public class ViewGameLogDialog : Dialog
	{
		// Token: 0x060017E7 RID: 6119 RVA: 0x000BD7CC File Offset: 0x000BB9CC
		public ViewGameLogDialog()
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/ViewGameLogDialog");
			base.LoadContents(this, node);
			this.m_listPanel = this.Children.Find<ListPanelWidget>("ViewGameLogDialog.ListPanel", true);
			this.m_copyButton = this.Children.Find<ButtonWidget>("ViewGameLogDialog.CopyButton", true);
			this.m_filterButton = this.Children.Find<ButtonWidget>("ViewGameLogDialog.FilterButton", true);
			this.m_closeButton = this.Children.Find<ButtonWidget>("ViewGameLogDialog.CloseButton", true);
			this.m_listPanel.ItemClicked += delegate(object item)
			{
				if (this.m_listPanel.SelectedItem == item)
				{
					DialogsManager.ShowDialog(base.ParentWidget, new MessageDialog("Log Item", item.ToString(), "OK", null, null));
				}
			};
			this.PopulateList();
		}

		// Token: 0x060017E8 RID: 6120 RVA: 0x000BD86C File Offset: 0x000BBA6C
		public override void Update()
		{
			if (this.m_copyButton.IsClicked)
			{
				ClipboardManager.ClipboardString = GameLogSink.GetRecentLog(131072);
			}
			if (this.m_filterButton.IsClicked)
			{
				if (this.m_filter < LogType.Warning)
				{
					this.m_filter = LogType.Warning;
				}
				else if (this.m_filter < LogType.Error)
				{
					this.m_filter = LogType.Error;
				}
				else
				{
					this.m_filter = LogType.Debug;
				}
				this.PopulateList();
			}
			if (base.Input.Cancel || this.m_closeButton.IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
			if (this.m_filter == LogType.Debug)
			{
				this.m_filterButton.Text = "All";
				return;
			}
			if (this.m_filter == LogType.Warning)
			{
				this.m_filterButton.Text = "Warnings";
				return;
			}
			if (this.m_filter == LogType.Error)
			{
				this.m_filterButton.Text = "Errors";
			}
		}

		// Token: 0x060017E9 RID: 6121 RVA: 0x000BD940 File Offset: 0x000BBB40
		public void PopulateList()
		{
			this.m_listPanel.ItemWidgetFactory = delegate(object item)
			{
				string text2 = (item != null) ? item.ToString() : string.Empty;
				Color color = Color.Gray;
				if (text2.Contains("ERROR:"))
				{
					color = Color.Red;
				}
				else if (text2.Contains("WARNING:"))
				{
					color = Color.DarkYellow;
				}
				else if (text2.Contains("INFO:"))
				{
					color = Color.LightGray;
				}
				return new LabelWidget
				{
					Text = text2,
					Font = BitmapFont.DebugFont,
					HorizontalAlignment = WidgetAlignment.Near,
					VerticalAlignment = WidgetAlignment.Center,
					Color = color
				};
			};
			List<string> recentLogLines = GameLogSink.GetRecentLogLines(131072);
			this.m_listPanel.ClearItems();
			if (recentLogLines.Count > 1000)
			{
				recentLogLines.RemoveRange(0, recentLogLines.Count - 1000);
			}
			foreach (string text in recentLogLines)
			{
				if (this.m_filter == LogType.Warning)
				{
					if (!text.Contains("WARNING:") && !text.Contains("ERROR:"))
					{
						continue;
					}
				}
				else if (this.m_filter == LogType.Error && !text.Contains("ERROR:"))
				{
					continue;
				}
				this.m_listPanel.AddItem(text);
			}
			this.m_listPanel.ScrollPosition = (float)this.m_listPanel.Items.Count * this.m_listPanel.ItemSize;
		}

		// Token: 0x040010EA RID: 4330
		public ListPanelWidget m_listPanel;

		// Token: 0x040010EB RID: 4331
		public ButtonWidget m_copyButton;

		// Token: 0x040010EC RID: 4332
		public ButtonWidget m_filterButton;

		// Token: 0x040010ED RID: 4333
		public ButtonWidget m_closeButton;

		// Token: 0x040010EE RID: 4334
		public LogType m_filter;
	}
}
