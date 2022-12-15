using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000145 RID: 325
	public class RecipaediaScreen : Screen
	{
		// Token: 0x0600061D RID: 1565 RVA: 0x00024774 File Offset: 0x00022974
		public RecipaediaScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/RecipaediaScreen");
			base.LoadContents(this, node);
			this.m_blocksList = this.Children.Find<ListPanelWidget>("BlocksList", true);
			this.m_categoryLabel = this.Children.Find<LabelWidget>("Category", true);
			this.m_prevCategoryButton = this.Children.Find<ButtonWidget>("PreviousCategory", true);
			this.m_nextCategoryButton = this.Children.Find<ButtonWidget>("NextCategory", true);
			this.m_detailsButton = this.Children.Find<ButtonWidget>("DetailsButton", true);
			this.m_recipesButton = this.Children.Find<ButtonWidget>("RecipesButton", true);
			this.m_categories.Add(null);
			this.m_categories.AddRange(BlocksManager.Categories);
			this.m_blocksList.ItemWidgetFactory = delegate(object item)
			{
				int value = (int)item;
				int num = Terrain.ExtractContents(value);
				Block block = BlocksManager.Blocks[num];
				XElement node2 = ContentManager.Get<XElement>("Widgets/RecipaediaItem");
				ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(this, node2, null);
				containerWidget.Children.Find<BlockIconWidget>("RecipaediaItem.Icon", true).Value = value;
				containerWidget.Children.Find<LabelWidget>("RecipaediaItem.Text", true).Text = block.GetDisplayName(null, value);
				containerWidget.Children.Find<LabelWidget>("RecipaediaItem.Details", true).Text = block.GetDescription(value);
				return containerWidget;
			};
			this.m_blocksList.ItemClicked += delegate(object item)
			{
				if (this.m_blocksList.SelectedItem == item && item is int)
				{
					ScreensManager.SwitchScreen("RecipaediaDescription", new object[]
					{
						item,
						this.m_blocksList.Items.Cast<int>().ToList<int>()
					});
				}
			};
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x00024885 File Offset: 0x00022A85
		public override void Enter(object[] parameters)
		{
			if (ScreensManager.PreviousScreen != ScreensManager.FindScreen<Screen>("RecipaediaRecipes") && ScreensManager.PreviousScreen != ScreensManager.FindScreen<Screen>("RecipaediaDescription"))
			{
				this.m_previousScreen = ScreensManager.PreviousScreen;
			}
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x000248B4 File Offset: 0x00022AB4
		public override void Update()
		{
			if (this.m_listCategoryIndex != this.m_categoryIndex)
			{
				this.PopulateBlocksList();
			}
			string arg = this.m_categories[this.m_categoryIndex] ?? LanguageControl.Get("BlocksManager", "All Blocks");
			this.m_categoryLabel.Text = string.Format("{0} ({1})", arg, this.m_blocksList.Items.Count);
			this.m_prevCategoryButton.IsEnabled = (this.m_categoryIndex > 0);
			this.m_nextCategoryButton.IsEnabled = (this.m_categoryIndex < this.m_categories.Count - 1);
			int? value = null;
			int num = 0;
			if (this.m_blocksList.SelectedItem is int)
			{
				value = new int?((int)this.m_blocksList.SelectedItem);
				num = CraftingRecipesManager.Recipes.Count(delegate(CraftingRecipe r)
				{
					int resultValue = r.ResultValue;
					
					return resultValue == value.GetValueOrDefault() & value != null;
				});
			}
			if (num > 0)
			{
				this.m_recipesButton.Text = string.Format("{0} {1}", num, (num == 1) ? LanguageControl.Get(RecipaediaScreen.fName, 1) : LanguageControl.Get(RecipaediaScreen.fName, 2));
				this.m_recipesButton.IsEnabled = true;
			}
			else
			{
				this.m_recipesButton.Text = LanguageControl.Get(RecipaediaScreen.fName, 3);
				this.m_recipesButton.IsEnabled = false;
			}
			this.m_detailsButton.IsEnabled = (value != null);
			if (this.m_prevCategoryButton.IsClicked || base.Input.Left)
			{
				this.m_categoryIndex = MathUtils.Max(this.m_categoryIndex - 1, 0);
			}
			if (this.m_nextCategoryButton.IsClicked || base.Input.Right)
			{
				this.m_categoryIndex = MathUtils.Min(this.m_categoryIndex + 1, this.m_categories.Count - 1);
			}
			if (value != null && this.m_detailsButton.IsClicked)
			{
				ScreensManager.SwitchScreen("RecipaediaDescription", new object[]
				{
					value.Value,
					this.m_blocksList.Items.Cast<int>().ToList<int>()
				});
			}
			if (value != null && this.m_recipesButton.IsClicked)
			{
				ScreensManager.SwitchScreen("RecipaediaRecipes", new object[]
				{
					value.Value
				});
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(this.m_previousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x00024B7C File Offset: 0x00022D7C
		public void PopulateBlocksList()
		{
			this.m_listCategoryIndex = this.m_categoryIndex;
			string text = this.m_categories[this.m_categoryIndex];
			this.m_blocksList.ScrollPosition = 0f;
			this.m_blocksList.ClearItems();
			foreach (Block block in from b in BlocksManager.Blocks
			orderby b.DisplayOrder
			select b)
			{
				foreach (int num in block.GetCreativeValues())
				{
					if (text == null || block.GetCategory(num) == text)
					{
						this.m_blocksList.AddItem(num);
					}
				}
			}
		}

		// Token: 0x04000307 RID: 775
		public ListPanelWidget m_blocksList;

		// Token: 0x04000308 RID: 776
		public LabelWidget m_categoryLabel;

		// Token: 0x04000309 RID: 777
		public ButtonWidget m_prevCategoryButton;

		// Token: 0x0400030A RID: 778
		public ButtonWidget m_nextCategoryButton;

		// Token: 0x0400030B RID: 779
		public ButtonWidget m_detailsButton;

		// Token: 0x0400030C RID: 780
		public ButtonWidget m_recipesButton;

		// Token: 0x0400030D RID: 781
		public Screen m_previousScreen;

		// Token: 0x0400030E RID: 782
		public List<string> m_categories = new List<string>();

		// Token: 0x0400030F RID: 783
		public int m_categoryIndex;

		// Token: 0x04000310 RID: 784
		public static string fName = "RecipaediaScreen";

		// Token: 0x04000311 RID: 785
		public int m_listCategoryIndex = -1;
	}
}
