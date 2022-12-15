using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000230 RID: 560
	public class BuildFurnitureDialog : Dialog
	{
		// Token: 0x0600112B RID: 4395 RVA: 0x0008603C File Offset: 0x0008423C
		public BuildFurnitureDialog(FurnitureDesign design, FurnitureDesign sourceDesign, Action<bool> handler)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/BuildFurnitureDialog");
			base.LoadContents(this, node);
			this.m_nameLabel = this.Children.Find<LabelWidget>("BuildFurnitureDialog.Name", true);
			this.m_statusLabel = this.Children.Find<LabelWidget>("BuildFurnitureDialog.Status", true);
			this.m_designWidget2d = this.Children.Find<FurnitureDesignWidget>("BuildFurnitureDialog.Design2d", true);
			this.m_designWidget3d = this.Children.Find<FurnitureDesignWidget>("BuildFurnitureDialog.Design3d", true);
			this.m_nameButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.NameButton", true);
			this.m_axisButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.AxisButton", true);
			this.m_leftButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.LeftButton", true);
			this.m_rightButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.RightButton", true);
			this.m_upButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.UpButton", true);
			this.m_downButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.DownButton", true);
			this.m_mirrorButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.MirrorButton", true);
			this.m_turnRightButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.TurnRightButton", true);
			this.m_increaseResolutionButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.IncreaseResolutionButton", true);
			this.m_decreaseResolutionButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.DecreaseResolutionButton", true);
			this.m_resolutionLabel = this.Children.Find<LabelWidget>("BuildFurnitureDialog.ResolutionLabel", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.CancelButton", true);
			this.m_buildButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.BuildButton", true);
			this.m_handler = handler;
			this.m_design = design;
			this.m_sourceDesign = sourceDesign;
			this.m_axis = 1;
			int num = 0;
			num += this.m_design.Geometry.SubsetOpaqueByFace.Sum(delegate(BlockMesh b)
			{
				if (b == null)
				{
					return 0;
				}
				return b.Indices.Count / 3;
			});
			num += this.m_design.Geometry.SubsetAlphaTestByFace.Sum(delegate(BlockMesh b)
			{
				if (b == null)
				{
					return 0;
				}
				return b.Indices.Count / 3;
			});
			this.m_isValid = (num <= 65535);
			this.m_statusLabel.Text = string.Format(LanguageControl.Get(BuildFurnitureDialog.fName, 1), num, 65535, this.m_isValid ? LanguageControl.Get(BuildFurnitureDialog.fName, 2) : LanguageControl.Get(BuildFurnitureDialog.fName, 3));
			this.m_designWidget2d.Design = this.m_design;
			this.m_designWidget3d.Design = this.m_design;
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x000862F4 File Offset: 0x000844F4
		public override void Update()
		{
			this.m_nameLabel.Text = (string.IsNullOrEmpty(this.m_design.Name) ? this.m_design.GetDefaultName() : this.m_design.Name);
			this.m_designWidget2d.Mode = (FurnitureDesignWidget.ViewMode)this.m_axis;
			this.m_designWidget3d.Mode = FurnitureDesignWidget.ViewMode.Perspective;
			if (this.m_designWidget2d.Mode == FurnitureDesignWidget.ViewMode.Side)
			{
				this.m_axisButton.Text = LanguageControl.Get(BuildFurnitureDialog.fName, 4);
			}
			if (this.m_designWidget2d.Mode == FurnitureDesignWidget.ViewMode.Top)
			{
				this.m_axisButton.Text = LanguageControl.Get(BuildFurnitureDialog.fName, 5);
			}
			if (this.m_designWidget2d.Mode == FurnitureDesignWidget.ViewMode.Front)
			{
				this.m_axisButton.Text = LanguageControl.Get(BuildFurnitureDialog.fName, 6);
			}
			this.m_leftButton.IsEnabled = this.IsShiftPossible(BuildFurnitureDialog.DirectionAxisToDelta(0, this.m_axis));
			this.m_rightButton.IsEnabled = this.IsShiftPossible(BuildFurnitureDialog.DirectionAxisToDelta(1, this.m_axis));
			this.m_upButton.IsEnabled = this.IsShiftPossible(BuildFurnitureDialog.DirectionAxisToDelta(2, this.m_axis));
			this.m_downButton.IsEnabled = this.IsShiftPossible(BuildFurnitureDialog.DirectionAxisToDelta(3, this.m_axis));
			this.m_decreaseResolutionButton.IsEnabled = this.IsDecreaseResolutionPossible();
			this.m_increaseResolutionButton.IsEnabled = this.IsIncreaseResolutionPossible();
			this.m_resolutionLabel.Text = string.Format("{0}", this.m_design.Resolution);
			this.m_buildButton.IsEnabled = this.m_isValid;
			if (this.m_nameButton.IsClicked)
			{
				List<Tuple<string, Action>> list = new List<Tuple<string, Action>>();
				if (this.m_sourceDesign != null)
				{
					list.Add(new Tuple<string, Action>(LanguageControl.Get(BuildFurnitureDialog.fName, 7), delegate()
					{
						this.Dismiss(false);
						DialogsManager.ShowDialog(base.ParentWidget, new TextBoxDialog(LanguageControl.Get(BuildFurnitureDialog.fName, 10), this.m_sourceDesign.Name, 20, delegate(string s)
						{
							try
							{
								if (s != null)
								{
									this.m_sourceDesign.Name = s;
								}
							}
							catch (Exception ex)
							{
								DialogsManager.ShowDialog(base.ParentWidget, new MessageDialog(LanguageControl.Get("Usual", "error"), ex.Message, LanguageControl.Get("Usual", "ok"), null, null));
							}
						}));
					}));
					list.Add(new Tuple<string, Action>(LanguageControl.Get(BuildFurnitureDialog.fName, 8), delegate()
					{
						DialogsManager.ShowDialog(base.ParentWidget, new TextBoxDialog(LanguageControl.Get(BuildFurnitureDialog.fName, 11), this.m_design.Name, 20, delegate(string s)
						{
							try
							{
								if (s != null)
								{
									this.m_design.Name = s;
								}
							}
							catch (Exception ex)
							{
								DialogsManager.ShowDialog(base.ParentWidget, new MessageDialog(LanguageControl.Get("Usual", "error"), ex.Message, LanguageControl.Get("Usual", "ok"), null, null));
							}
						}));
					}));
				}
				else
				{
					list.Add(new Tuple<string, Action>(LanguageControl.Get(BuildFurnitureDialog.fName, 9), delegate()
					{
						DialogsManager.ShowDialog(base.ParentWidget, new TextBoxDialog(LanguageControl.Get(BuildFurnitureDialog.fName, 11), this.m_design.Name, 20, delegate(string s)
						{
							try
							{
								if (s != null)
								{
									this.m_design.Name = s;
								}
							}
							catch (Exception ex)
							{
								DialogsManager.ShowDialog(base.ParentWidget, new MessageDialog(LanguageControl.Get("Usual", "error"), ex.Message, LanguageControl.Get("Usual", "ok"), null, null));
							}
						}));
					}));
				}
				if (list.Count == 1)
				{
					list[0].Item2();
				}
				else
				{
					DialogsManager.ShowDialog(base.ParentWidget, new ListSelectionDialog(LanguageControl.Get(BuildFurnitureDialog.fName, 11), list, 64f, (object t) => ((Tuple<string, Action>)t).Item1, delegate(object t)
					{
						((Tuple<string, Action>)t).Item2();
					}));
				}
			}
			if (this.m_axisButton.IsClicked)
			{
				this.m_axis = (this.m_axis + 1) % 3;
			}
			if (this.m_leftButton.IsClicked)
			{
				this.Shift(BuildFurnitureDialog.DirectionAxisToDelta(0, this.m_axis));
			}
			if (this.m_rightButton.IsClicked)
			{
				this.Shift(BuildFurnitureDialog.DirectionAxisToDelta(1, this.m_axis));
			}
			if (this.m_upButton.IsClicked)
			{
				this.Shift(BuildFurnitureDialog.DirectionAxisToDelta(2, this.m_axis));
			}
			if (this.m_downButton.IsClicked)
			{
				this.Shift(BuildFurnitureDialog.DirectionAxisToDelta(3, this.m_axis));
			}
			if (this.m_mirrorButton.IsClicked)
			{
				this.m_design.Mirror(this.m_axis);
			}
			if (this.m_turnRightButton.IsClicked)
			{
				this.m_design.Rotate(this.m_axis, 1);
			}
			if (this.m_decreaseResolutionButton.IsClicked)
			{
				this.DecreaseResolution();
			}
			if (this.m_increaseResolutionButton.IsClicked)
			{
				this.IncreaseResolution();
			}
			if (this.m_buildButton.IsClicked && this.m_isValid)
			{
				this.Dismiss(true);
			}
			if (base.Input.Back || this.m_cancelButton.IsClicked)
			{
				this.Dismiss(false);
			}
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x000866C8 File Offset: 0x000848C8
		public bool IsShiftPossible(Point3 delta)
		{
			int resolution = this.m_design.Resolution;
			Box box = this.m_design.Box;
			box.Location += delta;
			return box.Left >= 0 && box.Top >= 0 && box.Near >= 0 && box.Right <= resolution && box.Bottom <= resolution && box.Far <= resolution;
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x0008673E File Offset: 0x0008493E
		public void Shift(Point3 delta)
		{
			if (this.IsShiftPossible(delta))
			{
				this.m_design.Shift(delta);
			}
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x00086758 File Offset: 0x00084958
		public bool IsDecreaseResolutionPossible()
		{
			int resolution = this.m_design.Resolution;
			if (resolution > 2)
			{
				int num = MathUtils.Max(this.m_design.Box.Width, this.m_design.Box.Height, this.m_design.Box.Depth);
				return resolution > num;
			}
			return false;
		}

		// Token: 0x06001130 RID: 4400 RVA: 0x000867B4 File Offset: 0x000849B4
		public void DecreaseResolution()
		{
			if (this.IsDecreaseResolutionPossible())
			{
				int resolution = this.m_design.Resolution;
				Point3 zero = Point3.Zero;
				if (this.m_design.Box.Right >= resolution)
				{
					zero.X = -1;
				}
				if (this.m_design.Box.Bottom >= resolution)
				{
					zero.Y = -1;
				}
				if (this.m_design.Box.Far >= resolution)
				{
					zero.Z = -1;
				}
				this.m_design.Shift(zero);
				this.m_design.Resize(resolution - 1);
			}
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x00086852 File Offset: 0x00084A52
		public bool IsIncreaseResolutionPossible()
		{
			return this.m_design.Resolution < 16;
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x00086863 File Offset: 0x00084A63
		public void IncreaseResolution()
		{
			if (this.IsIncreaseResolutionPossible())
			{
				this.m_design.Resize(this.m_design.Resolution + 1);
			}
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x00086888 File Offset: 0x00084A88
		public static Point3 DirectionAxisToDelta(int direction, int axis)
		{
			if (direction == 0)
			{
				switch (axis)
				{
				case 0:
					return new Point3(0, 0, 1);
				case 1:
					return new Point3(1, 0, 0);
				case 2:
					return new Point3(1, 0, 0);
				}
			}
			if (direction == 1)
			{
				switch (axis)
				{
				case 0:
					return new Point3(0, 0, -1);
				case 1:
					return new Point3(-1, 0, 0);
				case 2:
					return new Point3(-1, 0, 0);
				}
			}
			if (direction == 2)
			{
				switch (axis)
				{
				case 0:
					return new Point3(0, 1, 0);
				case 1:
					return new Point3(0, 0, 1);
				case 2:
					return new Point3(0, 1, 0);
				}
			}
			if (direction == 3)
			{
				switch (axis)
				{
				case 0:
					return new Point3(0, -1, 0);
				case 1:
					return new Point3(0, 0, -1);
				case 2:
					return new Point3(0, -1, 0);
				}
			}
			return Point3.Zero;
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x00086965 File Offset: 0x00084B65
		public void Dismiss(bool result)
		{
			DialogsManager.HideDialog(this);
			this.m_handler(result);
		}

		// Token: 0x04000B7D RID: 2941
		public FurnitureDesign m_design;

		// Token: 0x04000B7E RID: 2942
		public FurnitureDesign m_sourceDesign;

		// Token: 0x04000B7F RID: 2943
		public int m_axis;

		// Token: 0x04000B80 RID: 2944
		public static string fName = "BuildFurnitureDialog";

		// Token: 0x04000B81 RID: 2945
		public Action<bool> m_handler;

		// Token: 0x04000B82 RID: 2946
		public bool m_isValid;

		// Token: 0x04000B83 RID: 2947
		public LabelWidget m_nameLabel;

		// Token: 0x04000B84 RID: 2948
		public LabelWidget m_statusLabel;

		// Token: 0x04000B85 RID: 2949
		public FurnitureDesignWidget m_designWidget2d;

		// Token: 0x04000B86 RID: 2950
		public FurnitureDesignWidget m_designWidget3d;

		// Token: 0x04000B87 RID: 2951
		public ButtonWidget m_axisButton;

		// Token: 0x04000B88 RID: 2952
		public ButtonWidget m_leftButton;

		// Token: 0x04000B89 RID: 2953
		public ButtonWidget m_rightButton;

		// Token: 0x04000B8A RID: 2954
		public ButtonWidget m_upButton;

		// Token: 0x04000B8B RID: 2955
		public ButtonWidget m_downButton;

		// Token: 0x04000B8C RID: 2956
		public ButtonWidget m_mirrorButton;

		// Token: 0x04000B8D RID: 2957
		public ButtonWidget m_turnRightButton;

		// Token: 0x04000B8E RID: 2958
		public ButtonWidget m_increaseResolutionButton;

		// Token: 0x04000B8F RID: 2959
		public ButtonWidget m_decreaseResolutionButton;

		// Token: 0x04000B90 RID: 2960
		public LabelWidget m_resolutionLabel;

		// Token: 0x04000B91 RID: 2961
		public ButtonWidget m_nameButton;

		// Token: 0x04000B92 RID: 2962
		public ButtonWidget m_buildButton;

		// Token: 0x04000B93 RID: 2963
		public ButtonWidget m_cancelButton;
	}
}
