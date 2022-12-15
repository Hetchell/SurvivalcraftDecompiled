using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000378 RID: 888
	public class CraftingRecipeSlotWidget : CanvasWidget
	{
		// Token: 0x0600196A RID: 6506 RVA: 0x000C70E0 File Offset: 0x000C52E0
		public CraftingRecipeSlotWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/CraftingRecipeSlot");
			base.LoadContents(this, node);
			this.m_blockIconWidget = this.Children.Find<BlockIconWidget>("CraftingRecipeSlotWidget.Icon", true);
			this.m_labelWidget = this.Children.Find<LabelWidget>("CraftingRecipeSlotWidget.Count", true);
		}

		// Token: 0x0600196B RID: 6507 RVA: 0x000C7134 File Offset: 0x000C5334
		public void SetIngredient(string ingredient)
		{
			this.m_ingredient = ingredient;
			this.m_resultValue = 0;
			this.m_resultCount = 0;
		}

		// Token: 0x0600196C RID: 6508 RVA: 0x000C714B File Offset: 0x000C534B
		public void SetResult(int value, int count)
		{
			this.m_resultValue = value;
			this.m_resultCount = count;
			this.m_ingredient = null;
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x000C7164 File Offset: 0x000C5364
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			this.m_blockIconWidget.IsVisible = false;
			this.m_labelWidget.IsVisible = false;
			if (!string.IsNullOrEmpty(this.m_ingredient))
			{
				string craftingId;
				int? num;
				CraftingRecipesManager.DecodeIngredient(this.m_ingredient, out craftingId, out num);
				Block[] array = BlocksManager.FindBlocksByCraftingId(craftingId);
				if (array.Length != 0)
				{
					Block block = array[(int)(1.0 * Time.RealTime) % array.Length];
					if (block != null)
					{
						this.m_blockIconWidget.Value = Terrain.MakeBlockValue(block.BlockIndex, 0, (num != null) ? num.Value : 4);
						this.m_blockIconWidget.Light = 15;
						this.m_blockIconWidget.IsVisible = true;
					}
				}
			}
			else if (this.m_resultValue != 0)
			{
				this.m_blockIconWidget.Value = this.m_resultValue;
				this.m_blockIconWidget.Light = 15;
				this.m_labelWidget.Text = this.m_resultCount.ToString();
				this.m_blockIconWidget.IsVisible = true;
				this.m_labelWidget.IsVisible = true;
			}
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x040011C2 RID: 4546
		public BlockIconWidget m_blockIconWidget;

		// Token: 0x040011C3 RID: 4547
		public LabelWidget m_labelWidget;

		// Token: 0x040011C4 RID: 4548
		public string m_ingredient;

		// Token: 0x040011C5 RID: 4549
		public int m_resultValue;

		// Token: 0x040011C6 RID: 4550
		public int m_resultCount;
	}
}
