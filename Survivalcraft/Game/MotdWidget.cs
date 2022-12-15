using System;
using System.Collections.Generic;
using Engine;
using Engine.Input;
using Engine.Media;

namespace Game
{
	// Token: 0x02000391 RID: 913
	public class MotdWidget : CanvasWidget
	{
		// Token: 0x06001A87 RID: 6791 RVA: 0x000D0660 File Offset: 0x000CE860
		public MotdWidget()
		{
			this.m_containerWidget = new CanvasWidget();
			this.Children.Add(this.m_containerWidget);
			MotdManager.MessageOfTheDayUpdated += this.MotdManager_MessageOfTheDayUpdated;
			this.MotdManager_MessageOfTheDayUpdated();
		}

		// Token: 0x06001A88 RID: 6792 RVA: 0x000D06B4 File Offset: 0x000CE8B4
		public override void Update()
		{
			Vector2? tap = base.Input.Tap;
			if (tap != null)
			{
				tap = base.Input.Tap;
				Widget widget = base.HitTestGlobal(tap.Value, null);
				if (widget != null && (widget == this || widget.IsChildWidgetOf(this)))
				{
					this.m_tapsCount++;
				}
			}
			if (this.m_tapsCount >= 5)
			{
				this.m_tapsCount = 0;
				MotdManager.ForceRedownload();
				AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			}
			if (base.Input.IsKeyDownOnce(Key.Delete))
			{
				this.GotoLine(this.m_currentLineIndex - 1);
			}
			if (base.Input.IsKeyDownOnce(Key.PageUp))
			{
				this.GotoLine(this.m_currentLineIndex + 1);
			}
			if (this.m_lines.Count > 0)
			{
				this.m_currentLineIndex %= this.m_lines.Count;
				double realTime = Time.RealTime;
				if (this.m_lastLineChangeTime == 0.0 || realTime - this.m_lastLineChangeTime >= (double)this.m_lines[this.m_currentLineIndex].Time)
				{
					this.GotoLine((this.m_lastLineChangeTime != 0.0) ? (this.m_currentLineIndex + 1) : 0);
				}
				float num = (float)(realTime - this.m_lastLineChangeTime);
				float num2 = (float)(this.m_lastLineChangeTime + (double)this.m_lines[this.m_currentLineIndex].Time - 0.33000001311302185 - realTime);
				tap = new Vector2?(new Vector2((num >= num2) ? (base.ActualSize.X * (1f - MathUtils.PowSign(MathUtils.Sin(MathUtils.Saturate(1.5f * num2) * 3.1415927f / 2f), 0.33f))) : (base.ActualSize.X * (MathUtils.PowSign(MathUtils.Sin(MathUtils.Saturate(1.5f * num) * 3.1415927f / 2f), 0.33f) - 1f)), 0f));
				base.SetWidgetPosition(this.m_containerWidget, tap);
				this.m_containerWidget.Size = base.ActualSize;
				return;
			}
			this.m_containerWidget.Children.Clear();
		}

		// Token: 0x06001A89 RID: 6793 RVA: 0x000D08EC File Offset: 0x000CEAEC
		public void GotoLine(int index)
		{
			if (this.m_lines.Count > 0)
			{
				this.m_currentLineIndex = MathUtils.Max(index, 0) % this.m_lines.Count;
				this.m_containerWidget.Children.Clear();
				this.m_containerWidget.Children.Add(this.m_lines[this.m_currentLineIndex].Widget);
				this.m_lastLineChangeTime = Time.RealTime;
				this.m_tapsCount = 0;
			}
		}

		// Token: 0x06001A8A RID: 6794 RVA: 0x000D0968 File Offset: 0x000CEB68
		public void Restart()
		{
			this.m_currentLineIndex = 0;
			this.m_lastLineChangeTime = 0.0;
		}

		// Token: 0x06001A8B RID: 6795 RVA: 0x000D0980 File Offset: 0x000CEB80
		public void MotdManager_MessageOfTheDayUpdated()
		{
			this.m_lines.Clear();
			if (MotdManager.MessageOfTheDay != null)
			{
				foreach (MotdManager.Line line in MotdManager.MessageOfTheDay.Lines)
				{
					try
					{
						MotdWidget.LineData item = this.ParseLine(line);
						this.m_lines.Add(item);
					}
					catch (Exception ex)
					{
						Log.Warning(string.Format("Error loading MOTD line {0}. Reason: {1}", MotdManager.MessageOfTheDay.Lines.IndexOf(line) + 1, ex.Message));
					}
				}
			}
			this.Restart();
		}

		// Token: 0x06001A8C RID: 6796 RVA: 0x000D0A3C File Offset: 0x000CEC3C
		public MotdWidget.LineData ParseLine(MotdManager.Line line)
		{
			MotdWidget.LineData lineData = new MotdWidget.LineData();
			lineData.Time = line.Time;
			if (line.Node != null)
			{
				lineData.Widget = Widget.LoadWidget(null, line.Node, null);
			}
			else
			{
				if (string.IsNullOrEmpty(line.Text))
				{
					throw new InvalidOperationException("Invalid MOTD line.");
				}
				StackPanelWidget stackPanelWidget = new StackPanelWidget
				{
					Direction = LayoutDirection.Vertical,
					HorizontalAlignment = WidgetAlignment.Center,
					VerticalAlignment = WidgetAlignment.Center
				};
				string[] array = line.Text.Replace("\r", "").Split(new string[]
				{
					"\n"
				}, StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i].Trim();
					if (!string.IsNullOrEmpty(text))
					{
						LabelWidget widget = new LabelWidget
						{
							Text = text,
							Font = ContentManager.Get<BitmapFont>("Fonts/Pericles"),
							HorizontalAlignment = WidgetAlignment.Center,
							VerticalAlignment = WidgetAlignment.Center,
							DropShadow = true
						};
						stackPanelWidget.Children.Add(widget);
					}
				}
				lineData.Widget = stackPanelWidget;
			}
			return lineData;
		}

		// Token: 0x0400127F RID: 4735
		public CanvasWidget m_containerWidget;

		// Token: 0x04001280 RID: 4736
		public List<MotdWidget.LineData> m_lines = new List<MotdWidget.LineData>();

		// Token: 0x04001281 RID: 4737
		public int m_currentLineIndex;

		// Token: 0x04001282 RID: 4738
		public double m_lastLineChangeTime;

		// Token: 0x04001283 RID: 4739
		public int m_tapsCount;

		// Token: 0x02000524 RID: 1316
		public class LineData
		{
			// Token: 0x040018F6 RID: 6390
			public float Time;

			// Token: 0x040018F7 RID: 6391
			public Widget Widget;
		}
	}
}
