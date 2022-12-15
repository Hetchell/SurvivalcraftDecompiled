using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200025C RID: 604
	public class EditMemoryBankDialog : Dialog
	{
		// Token: 0x06001225 RID: 4645 RVA: 0x0008C300 File Offset: 0x0008A500
		public EditMemoryBankDialog(MemoryBankData memoryBankData, Action handler)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditMemoryBankDialog");
			base.LoadContents(this, node);
			this.m_linearPanel = this.Children.Find<Widget>("EditMemoryBankDialog.LinearPanel", true);
			this.m_gridPanel = this.Children.Find<Widget>("EditMemoryBankDialog.GridPanel", true);
			this.m_okButton = this.Children.Find<ButtonWidget>("EditMemoryBankDialog.OK", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("EditMemoryBankDialog.Cancel", true);
			this.m_switchViewButton = this.Children.Find<ButtonWidget>("EditMemoryBankDialog.SwitchViewButton", true);
			this.m_linearTextBox = this.Children.Find<TextBoxWidget>("EditMemoryBankDialog.LinearText", true);
			for (int i = 0; i < 16; i++)
			{
				this.m_lineTextBoxes[i] = this.Children.Find<TextBoxWidget>("EditMemoryBankDialog.Line" + i.ToString(), true);
			}
			this.m_handler = handler;
			this.m_memoryBankData = memoryBankData;
			this.m_tmpMemoryBankData = (MemoryBankData)this.m_memoryBankData.Copy();
			this.m_linearPanel.IsVisible = false;
			for (int j = 0; j < 16; j++)
			{
				this.m_lineTextBoxes[j].TextChanged += this.TextBox_TextChanged;
			}
			this.m_linearTextBox.TextChanged += this.TextBox_TextChanged;
		}

		// Token: 0x06001226 RID: 4646 RVA: 0x0008C45C File Offset: 0x0008A65C
		public void TextBox_TextChanged(TextBoxWidget textBox)
		{
			if (this.m_ignoreTextChanges)
			{
				return;
			}
			if (textBox == this.m_linearTextBox)
			{
				this.m_tmpMemoryBankData = new MemoryBankData();
				this.m_tmpMemoryBankData.LoadString(this.m_linearTextBox.Text);
				return;
			}
			string text = string.Empty;
			for (int i = 0; i < 16; i++)
			{
				text += this.m_lineTextBoxes[i].Text;
			}
			this.m_tmpMemoryBankData = new MemoryBankData();
			this.m_tmpMemoryBankData.LoadString(text);
		}

		// Token: 0x06001227 RID: 4647 RVA: 0x0008C4DC File Offset: 0x0008A6DC
		public override void Update()
		{
			this.m_ignoreTextChanges = true;
			try
			{
				string text = this.m_tmpMemoryBankData.SaveString(false);
				if (text.Length < 256)
				{
					text += new string('0', 256 - text.Length);
				}
				for (int i = 0; i < 16; i++)
				{
					this.m_lineTextBoxes[i].Text = text.Substring(i * 16, 16);
				}
				this.m_linearTextBox.Text = this.m_tmpMemoryBankData.SaveString(false);
			}
			finally
			{
				this.m_ignoreTextChanges = false;
			}
			if (this.m_linearPanel.IsVisible)
			{
				this.m_switchViewButton.Text = "Grid";
				if (this.m_switchViewButton.IsClicked)
				{
					this.m_linearPanel.IsVisible = false;
					this.m_gridPanel.IsVisible = true;
				}
			}
			else
			{
				this.m_switchViewButton.Text = "Linear";
				if (this.m_switchViewButton.IsClicked)
				{
					this.m_linearPanel.IsVisible = true;
					this.m_gridPanel.IsVisible = false;
				}
			}
			if (this.m_okButton.IsClicked)
			{
				this.m_memoryBankData.Data = this.m_tmpMemoryBankData.Data;
				this.Dismiss(true);
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss(false);
			}
		}

		// Token: 0x06001228 RID: 4648 RVA: 0x0008C640 File Offset: 0x0008A840
		public void Dismiss(bool result)
		{
			DialogsManager.HideDialog(this);
			if (this.m_handler != null && result)
			{
				this.m_handler();
			}
		}

		// Token: 0x04000C3C RID: 3132
		public Action m_handler;

		// Token: 0x04000C3D RID: 3133
		public Widget m_linearPanel;

		// Token: 0x04000C3E RID: 3134
		public Widget m_gridPanel;

		// Token: 0x04000C3F RID: 3135
		public ButtonWidget m_okButton;

		// Token: 0x04000C40 RID: 3136
		public ButtonWidget m_cancelButton;

		// Token: 0x04000C41 RID: 3137
		public ButtonWidget m_switchViewButton;

		// Token: 0x04000C42 RID: 3138
		public TextBoxWidget[] m_lineTextBoxes = new TextBoxWidget[16];

		// Token: 0x04000C43 RID: 3139
		public TextBoxWidget m_linearTextBox;

		// Token: 0x04000C44 RID: 3140
		public MemoryBankData m_memoryBankData;

		// Token: 0x04000C45 RID: 3141
		public MemoryBankData m_tmpMemoryBankData;

		// Token: 0x04000C46 RID: 3142
		public bool m_ignoreTextChanges;
	}
}
