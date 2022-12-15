using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Engine;
using GameEntitySystem;

namespace Game
{
	// Token: 0x0200037B RID: 891
	public class CreativeInventoryWidget : CanvasWidget
	{
		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06001977 RID: 6519 RVA: 0x000C7719 File Offset: 0x000C5919
		public Entity Entity
		{
			get
			{
				return this.m_componentCreativeInventory.Entity;
			}
		}

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06001978 RID: 6520 RVA: 0x000C7726 File Offset: 0x000C5926
		public ButtonWidget PageDownButton
		{
			get
			{
				return this.m_pageDownButton;
			}
		}

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06001979 RID: 6521 RVA: 0x000C772E File Offset: 0x000C592E
		public ButtonWidget PageUpButton
		{
			get
			{
				return this.m_pageUpButton;
			}
		}

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x0600197A RID: 6522 RVA: 0x000C7736 File Offset: 0x000C5936
		public LabelWidget PageLabel
		{
			get
			{
				return this.m_pageLabel;
			}
		}

		// Token: 0x0600197B RID: 6523 RVA: 0x000C7740 File Offset: 0x000C5940
		public CreativeInventoryWidget(Entity entity)
		{
			this.m_componentCreativeInventory = entity.FindComponent<ComponentCreativeInventory>(true);
			XElement node = ContentManager.Get<XElement>("Widgets/CreativeInventoryWidget");
			base.LoadContents(this, node);
			this.m_categoryLeftButton = this.Children.Find<ButtonWidget>("CategoryLeftButton", true);
			this.m_categoryRightButton = this.Children.Find<ButtonWidget>("CategoryRightButton", true);
			this.m_categoryButton = this.Children.Find<ButtonWidget>("CategoryButton", true);
			this.m_pageUpButton = this.Children.Find<ButtonWidget>("PageUpButton", true);
			this.m_pageDownButton = this.Children.Find<ButtonWidget>("PageDownButton", true);
			this.m_pageLabel = this.Children.Find<LabelWidget>("PageLabel", true);
			this.m_panelContainer = this.Children.Find<ContainerWidget>("PanelContainer", true);
			CreativeInventoryPanel creativeInventoryPanel = new CreativeInventoryPanel(this)
			{
				IsVisible = false
			};
			this.m_panelContainer.Children.Add(creativeInventoryPanel);
			FurnitureInventoryPanel furnitureInventoryPanel = new FurnitureInventoryPanel(this)
			{
				IsVisible = false
			};
			this.m_panelContainer.Children.Add(furnitureInventoryPanel);
			foreach (string name in BlocksManager.Categories)
			{
				this.m_categories.Add(new CreativeInventoryWidget.Category
				{
					Name = name,
					Panel = creativeInventoryPanel
				});
			}
			this.m_categories.Add(new CreativeInventoryWidget.Category
			{
				Name = LanguageControl.Get(CreativeInventoryWidget.fName, 1),
				Panel = furnitureInventoryPanel
			});
			this.m_categories.Add(new CreativeInventoryWidget.Category
			{
				Name = LanguageControl.Get(CreativeInventoryWidget.fName, 2),
				Panel = creativeInventoryPanel
			});
			for (int i = 0; i < this.m_categories.Count; i++)
			{
				if (this.m_categories[i].Name == LanguageControl.Get("BlocksManager", "Electrics"))
				{
					this.m_categories[i].Color = new Color(128, 140, 255);
				}
				if (this.m_categories[i].Name == LanguageControl.Get("BlocksManager", "Plants"))
				{
					this.m_categories[i].Color = new Color(64, 160, 64);
				}
				if (this.m_categories[i].Name == LanguageControl.Get("BlocksManager", "Weapons"))
				{
					this.m_categories[i].Color = new Color(255, 128, 112);
				}
			}
		}

		// Token: 0x0600197C RID: 6524 RVA: 0x000C7A18 File Offset: 0x000C5C18
		public string GetCategoryName(int index)
		{
			return this.m_categories[index].Name;
		}

		// Token: 0x0600197D RID: 6525 RVA: 0x000C7A2C File Offset: 0x000C5C2C
		public override void Update()
		{
			if (this.m_categoryLeftButton.IsClicked || base.Input.Left)
			{
				ComponentCreativeInventory componentCreativeInventory = this.m_componentCreativeInventory;
				int categoryIndex = componentCreativeInventory.CategoryIndex - 1;
				componentCreativeInventory.CategoryIndex = categoryIndex;
			}
			if (this.m_categoryRightButton.IsClicked || base.Input.Right)
			{
				ComponentCreativeInventory componentCreativeInventory2 = this.m_componentCreativeInventory;
				int categoryIndex = componentCreativeInventory2.CategoryIndex + 1;
				componentCreativeInventory2.CategoryIndex = categoryIndex;
			}
			if (this.m_categoryButton.IsClicked)
			{
				ComponentPlayer componentPlayer = this.Entity.FindComponent<ComponentPlayer>();
				if (componentPlayer != null)
				{
					DialogsManager.ShowDialog(componentPlayer.GuiWidget, new ListSelectionDialog(string.Empty, this.m_categories, 56f, (object c) => new LabelWidget
					{
						Text = ((CreativeInventoryWidget.Category)c).Name,
						Color = ((CreativeInventoryWidget.Category)c).Color,
						HorizontalAlignment = WidgetAlignment.Center,
						VerticalAlignment = WidgetAlignment.Center
					}, delegate(object c)
					{
						if (c != null)
						{
							this.m_componentCreativeInventory.CategoryIndex = this.m_categories.IndexOf((CreativeInventoryWidget.Category)c);
						}
					}));
				}
			}
			this.m_componentCreativeInventory.CategoryIndex = MathUtils.Clamp(this.m_componentCreativeInventory.CategoryIndex, 0, this.m_categories.Count - 1);
			this.m_categoryButton.Text = this.m_categories[this.m_componentCreativeInventory.CategoryIndex].Name;
			this.m_categoryLeftButton.IsEnabled = (this.m_componentCreativeInventory.CategoryIndex > 0);
			this.m_categoryRightButton.IsEnabled = (this.m_componentCreativeInventory.CategoryIndex < this.m_categories.Count - 1);
			if (this.m_componentCreativeInventory.CategoryIndex != this.m_activeCategoryIndex)
			{
				foreach (CreativeInventoryWidget.Category category in this.m_categories)
				{
					category.Panel.IsVisible = false;
				}
				this.m_categories[this.m_componentCreativeInventory.CategoryIndex].Panel.IsVisible = true;
				this.m_activeCategoryIndex = this.m_componentCreativeInventory.CategoryIndex;
			}
		}

		// Token: 0x040011D3 RID: 4563
		public List<CreativeInventoryWidget.Category> m_categories = new List<CreativeInventoryWidget.Category>();

		// Token: 0x040011D4 RID: 4564
		public int m_activeCategoryIndex = -1;

		// Token: 0x040011D5 RID: 4565
		public ComponentCreativeInventory m_componentCreativeInventory;

		// Token: 0x040011D6 RID: 4566
		public ButtonWidget m_pageUpButton;

		// Token: 0x040011D7 RID: 4567
		public ButtonWidget m_pageDownButton;

		// Token: 0x040011D8 RID: 4568
		public LabelWidget m_pageLabel;

		// Token: 0x040011D9 RID: 4569
		public ButtonWidget m_categoryLeftButton;

		// Token: 0x040011DA RID: 4570
		public ButtonWidget m_categoryRightButton;

		// Token: 0x040011DB RID: 4571
		public static string fName = "CreativeInventoryWidget";

		// Token: 0x040011DC RID: 4572
		public ButtonWidget m_categoryButton;

		// Token: 0x040011DD RID: 4573
		public ContainerWidget m_panelContainer;

		// Token: 0x02000518 RID: 1304
		public class Category
		{
			// Token: 0x040018DA RID: 6362
			public string Name;

			// Token: 0x040018DB RID: 6363
			public Color Color = Color.White;

			// Token: 0x040018DC RID: 6364
			public ContainerWidget Panel;
		}
	}
}
