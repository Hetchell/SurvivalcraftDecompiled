using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000379 RID: 889
	public class CraftingRecipeWidget : CanvasWidget
	{
		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x0600196E RID: 6510 RVA: 0x000C7270 File Offset: 0x000C5470
		// (set) Token: 0x0600196F RID: 6511 RVA: 0x000C7278 File Offset: 0x000C5478
		public string NameSuffix
		{
			get
			{
				return this.m_nameSuffix;
			}
			set
			{
				if (value != this.m_nameSuffix)
				{
					this.m_nameSuffix = value;
					this.m_dirty = true;
				}
			}
		}

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06001970 RID: 6512 RVA: 0x000C7296 File Offset: 0x000C5496
		// (set) Token: 0x06001971 RID: 6513 RVA: 0x000C729E File Offset: 0x000C549E
		public CraftingRecipe Recipe
		{
			get
			{
				return this.m_recipe;
			}
			set
			{
				if (value != this.m_recipe)
				{
					this.m_recipe = value;
					this.m_dirty = true;
				}
			}
		}

		// Token: 0x06001972 RID: 6514 RVA: 0x000C72B8 File Offset: 0x000C54B8
		public CraftingRecipeWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/CraftingRecipe");
			base.LoadContents(this, node);
			this.m_nameWidget = this.Children.Find<LabelWidget>("CraftingRecipeWidget.Name", true);
			this.m_descriptionWidget = this.Children.Find<LabelWidget>("CraftingRecipeWidget.Description", true);
			this.m_gridWidget = this.Children.Find<GridPanelWidget>("CraftingRecipeWidget.Ingredients", true);
			this.m_resultWidget = this.Children.Find<CraftingRecipeSlotWidget>("CraftingRecipeWidget.Result", true);
			for (int i = 0; i < this.m_gridWidget.RowsCount; i++)
			{
				for (int j = 0; j < this.m_gridWidget.ColumnsCount; j++)
				{
					CraftingRecipeSlotWidget widget = new CraftingRecipeSlotWidget();
					this.m_gridWidget.Children.Add(widget);
					this.m_gridWidget.SetWidgetCell(widget, new Point2(j, i));
				}
			}
		}

		// Token: 0x06001973 RID: 6515 RVA: 0x000C7397 File Offset: 0x000C5597
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			if (this.m_dirty)
			{
				this.UpdateWidgets();
			}
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x06001974 RID: 6516 RVA: 0x000C73B0 File Offset: 0x000C55B0
		public void UpdateWidgets()
		{
			this.m_dirty = false;
			if (this.m_recipe != null)
			{
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(this.m_recipe.ResultValue)];
				this.m_nameWidget.Text = block.GetDisplayName(null, this.m_recipe.ResultValue) + ((!string.IsNullOrEmpty(this.NameSuffix)) ? this.NameSuffix : string.Empty);
				this.m_descriptionWidget.Text = this.m_recipe.Description;
				this.m_nameWidget.IsVisible = true;
				this.m_descriptionWidget.IsVisible = true;
				foreach (Widget widget in this.m_gridWidget.Children)
				{
					CraftingRecipeSlotWidget craftingRecipeSlotWidget = (CraftingRecipeSlotWidget)widget;
					Point2 widgetCell = this.m_gridWidget.GetWidgetCell(craftingRecipeSlotWidget);
					craftingRecipeSlotWidget.SetIngredient(this.m_recipe.Ingredients[widgetCell.X + widgetCell.Y * 3]);
				}
				this.m_resultWidget.SetResult(this.m_recipe.ResultValue, this.m_recipe.ResultCount);
				return;
			}
			this.m_nameWidget.IsVisible = false;
			this.m_descriptionWidget.IsVisible = false;
			foreach (Widget widget2 in this.m_gridWidget.Children)
			{
				((CraftingRecipeSlotWidget)widget2).SetIngredient(null);
			}
			this.m_resultWidget.SetResult(0, 0);
		}

		// Token: 0x040011C7 RID: 4551
		public LabelWidget m_nameWidget;

		// Token: 0x040011C8 RID: 4552
		public LabelWidget m_descriptionWidget;

		// Token: 0x040011C9 RID: 4553
		public GridPanelWidget m_gridWidget;

		// Token: 0x040011CA RID: 4554
		public CraftingRecipeSlotWidget m_resultWidget;

		// Token: 0x040011CB RID: 4555
		public CraftingRecipe m_recipe;

		// Token: 0x040011CC RID: 4556
		public string m_nameSuffix;

		// Token: 0x040011CD RID: 4557
		public bool m_dirty = true;
	}
}
