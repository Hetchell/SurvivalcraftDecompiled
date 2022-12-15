using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x0200039B RID: 923
	public class SmeltingRecipeWidget : CanvasWidget
	{
		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x06001AFE RID: 6910 RVA: 0x000D39A8 File Offset: 0x000D1BA8
		// (set) Token: 0x06001AFF RID: 6911 RVA: 0x000D39B0 File Offset: 0x000D1BB0
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

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x06001B00 RID: 6912 RVA: 0x000D39CE File Offset: 0x000D1BCE
		// (set) Token: 0x06001B01 RID: 6913 RVA: 0x000D39D6 File Offset: 0x000D1BD6
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

		// Token: 0x06001B02 RID: 6914 RVA: 0x000D39F0 File Offset: 0x000D1BF0
		public SmeltingRecipeWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/SmeltingRecipe");
			base.LoadContents(this, node);
			this.m_nameWidget = this.Children.Find<LabelWidget>("SmeltingRecipeWidget.Name", true);
			this.m_descriptionWidget = this.Children.Find<LabelWidget>("SmeltingRecipeWidget.Description", true);
			this.m_gridWidget = this.Children.Find<GridPanelWidget>("SmeltingRecipeWidget.Ingredients", true);
			this.m_fireWidget = this.Children.Find<FireWidget>("SmeltingRecipeWidget.Fire", true);
			this.m_resultWidget = this.Children.Find<CraftingRecipeSlotWidget>("SmeltingRecipeWidget.Result", true);
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

		// Token: 0x06001B03 RID: 6915 RVA: 0x000D3AE6 File Offset: 0x000D1CE6
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			if (this.m_dirty)
			{
				this.UpdateWidgets();
			}
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x06001B04 RID: 6916 RVA: 0x000D3B00 File Offset: 0x000D1D00
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
				this.m_fireWidget.ParticlesPerSecond = 40f;
				return;
			}
			this.m_nameWidget.IsVisible = false;
			this.m_descriptionWidget.IsVisible = false;
			foreach (Widget widget2 in this.m_gridWidget.Children)
			{
				((CraftingRecipeSlotWidget)widget2).SetIngredient(null);
			}
			this.m_resultWidget.SetResult(0, 0);
			this.m_fireWidget.ParticlesPerSecond = 0f;
		}

		// Token: 0x040012C4 RID: 4804
		public LabelWidget m_nameWidget;

		// Token: 0x040012C5 RID: 4805
		public LabelWidget m_descriptionWidget;

		// Token: 0x040012C6 RID: 4806
		public GridPanelWidget m_gridWidget;

		// Token: 0x040012C7 RID: 4807
		public FireWidget m_fireWidget;

		// Token: 0x040012C8 RID: 4808
		public CraftingRecipeSlotWidget m_resultWidget;

		// Token: 0x040012C9 RID: 4809
		public CraftingRecipe m_recipe;

		// Token: 0x040012CA RID: 4810
		public string m_nameSuffix;

		// Token: 0x040012CB RID: 4811
		public bool m_dirty = true;
	}
}
