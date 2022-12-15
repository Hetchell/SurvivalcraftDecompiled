using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000250 RID: 592
	public static class DialogsManager
	{
		// Token: 0x17000291 RID: 657
		// (get) Token: 0x060011DD RID: 4573 RVA: 0x0008A137 File Offset: 0x00088337
		public static ReadOnlyList<Dialog> Dialogs
		{
			get
			{
				return new ReadOnlyList<Dialog>(DialogsManager.m_dialogs);
			}
		}

		// Token: 0x060011DE RID: 4574 RVA: 0x0008A144 File Offset: 0x00088344
		public static bool HasDialogs(Widget parentWidget)
		{
			if (parentWidget == null)
			{
				parentWidget = (ScreensManager.CurrentScreen ?? ScreensManager.RootWidget);
			}
			using (List<Dialog>.Enumerator enumerator = DialogsManager.m_dialogs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ParentWidget == parentWidget)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060011DF RID: 4575 RVA: 0x0008A1B0 File Offset: 0x000883B0
		public static void ShowDialog(ContainerWidget parentWidget, Dialog dialog)
		{
			Dispatcher.Dispatch(delegate
			{
				if (!DialogsManager.m_dialogs.Contains(dialog))
				{
					if (parentWidget == null)
					{
						parentWidget = (ScreensManager.CurrentScreen ?? ScreensManager.RootWidget);
					}
					dialog.WidgetsHierarchyInput = null;
					DialogsManager.m_dialogs.Add(dialog);
					DialogsManager.AnimationData animationData = new DialogsManager.AnimationData
					{
						Direction = 1
					};
					DialogsManager.m_animationData[dialog] = animationData;
					parentWidget.Children.Add(animationData.CoverRectangle);
					if (dialog.ParentWidget != null)
					{
						dialog.ParentWidget.Children.Remove(dialog);
					}
					parentWidget.Children.Add(dialog);
					DialogsManager.UpdateDialog(dialog, animationData);
					dialog.Input.Clear();
				}
			}, false);
		}

		// Token: 0x060011E0 RID: 4576 RVA: 0x0008A1D6 File Offset: 0x000883D6
		public static void HideDialog(Dialog dialog)
		{
			Dispatcher.Dispatch(delegate
			{
				if (DialogsManager.m_dialogs.Contains(dialog))
				{
					dialog.ParentWidget.Input.Clear();
					dialog.WidgetsHierarchyInput = new WidgetInput(WidgetInputDevice.None);
					DialogsManager.m_dialogs.Remove(dialog);
					DialogsManager.m_animationData[dialog].Direction = -1;
				}
			}, false);
		}

		// Token: 0x060011E1 RID: 4577 RVA: 0x0008A1F8 File Offset: 0x000883F8
		public static void HideAllDialogs()
		{
			Dialog[] array = DialogsManager.m_dialogs.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				DialogsManager.HideDialog(array[i]);
			}
		}

		// Token: 0x060011E2 RID: 4578 RVA: 0x0008A228 File Offset: 0x00088428
		public static void Update()
		{
			foreach (KeyValuePair<Dialog, DialogsManager.AnimationData> keyValuePair in DialogsManager.m_animationData)
			{
				Dialog key = keyValuePair.Key;
				DialogsManager.AnimationData value = keyValuePair.Value;
				if (value.Direction > 0)
				{
					value.Factor = MathUtils.Min(value.Factor + 6f * Time.FrameDuration, 1f);
				}
				else if (value.Direction < 0)
				{
					value.Factor = MathUtils.Max(value.Factor - 6f * Time.FrameDuration, 0f);
					if (value.Factor <= 0f)
					{
						DialogsManager.m_toRemove.Add(key);
					}
				}
				DialogsManager.UpdateDialog(key, value);
			}
			foreach (Dialog dialog in DialogsManager.m_toRemove)
			{
				DialogsManager.AnimationData animationData = DialogsManager.m_animationData[dialog];
				DialogsManager.m_animationData.Remove(dialog);
				dialog.ParentWidget.Children.Remove(dialog);
				animationData.CoverRectangle.ParentWidget.Children.Remove(animationData.CoverRectangle);
			}
			DialogsManager.m_toRemove.Clear();
		}

		// Token: 0x060011E3 RID: 4579 RVA: 0x0008A394 File Offset: 0x00088594
		public static void UpdateDialog(Dialog dialog, DialogsManager.AnimationData animationData)
		{
			if (animationData.Factor < 1f)
			{
				float factor = animationData.Factor;
				float num = 0.75f + 0.25f * MathUtils.Pow(animationData.Factor, 0.25f);
				dialog.RenderTransform = Matrix.CreateTranslation((0f - dialog.ActualSize.X) / 2f, (0f - dialog.ActualSize.Y) / 2f, 0f) * Matrix.CreateScale(num, num, 1f) * Matrix.CreateTranslation(dialog.ActualSize.X / 2f, dialog.ActualSize.Y / 2f, 0f);
				dialog.ColorTransform = Color.White * factor;
				animationData.CoverRectangle.ColorTransform = Color.White * factor;
				return;
			}
			dialog.RenderTransform = Matrix.Identity;
			dialog.ColorTransform = Color.White;
			animationData.CoverRectangle.ColorTransform = Color.White;
		}

		// Token: 0x04000C04 RID: 3076
		public static Dictionary<Dialog, DialogsManager.AnimationData> m_animationData = new Dictionary<Dialog, DialogsManager.AnimationData>();

		// Token: 0x04000C05 RID: 3077
		public static List<Dialog> m_dialogs = new List<Dialog>();

		// Token: 0x04000C06 RID: 3078
		public static List<Dialog> m_toRemove = new List<Dialog>();

		// Token: 0x02000484 RID: 1156
		public class AnimationData
		{
			// Token: 0x040016D3 RID: 5843
			public float Factor;

			// Token: 0x040016D4 RID: 5844
			public int Direction;

			// Token: 0x040016D5 RID: 5845
			public RectangleWidget CoverRectangle = new RectangleWidget
			{
				OutlineColor = Color.Transparent,
				FillColor = new Color(0, 0, 0, 192),
				IsHitTestVisible = true
			};
		}
	}
}
