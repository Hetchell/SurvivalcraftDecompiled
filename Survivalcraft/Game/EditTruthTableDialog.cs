using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000260 RID: 608
	public class EditTruthTableDialog : Dialog
	{
		// Token: 0x06001235 RID: 4661 RVA: 0x0008D5A8 File Offset: 0x0008B7A8
		public EditTruthTableDialog(TruthTableData truthTableData, Action<bool> handler)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditTruthTableDialog");
			base.LoadContents(this, node);
			this.m_linearPanel = this.Children.Find<Widget>("EditTruthTableDialog.LinearPanel", true);
			this.m_gridPanel = this.Children.Find<Widget>("EditTruthTableDialog.GridPanel", true);
			this.m_okButton = this.Children.Find<ButtonWidget>("EditTruthTableDialog.OK", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("EditTruthTableDialog.Cancel", true);
			this.m_switchViewButton = this.Children.Find<ButtonWidget>("EditTruthTableDialog.SwitchViewButton", true);
			this.m_linearTextBox = this.Children.Find<TextBoxWidget>("EditTruthTableDialog.LinearText", true);
			for (int i = 0; i < 16; i++)
			{
				this.m_lineCheckboxes[i] = this.Children.Find<CheckboxWidget>("EditTruthTableDialog.Line" + i.ToString(), true);
			}
			this.m_handler = handler;
			this.m_truthTableData = truthTableData;
			this.m_tmpTruthTableData = (TruthTableData)this.m_truthTableData.Copy();
			this.m_linearPanel.IsVisible = false;
			this.m_linearTextBox.TextChanged += delegate(TextBoxWidget p)
			{
				if (!this.m_ignoreTextChanges)
				{
					this.m_tmpTruthTableData = new TruthTableData();
					this.m_tmpTruthTableData.LoadBinaryString(this.m_linearTextBox.Text);
				}
			};
		}

		// Token: 0x06001236 RID: 4662 RVA: 0x0008D6E0 File Offset: 0x0008B8E0
		public override void Update()
		{
			this.m_ignoreTextChanges = true;
			try
			{
				this.m_linearTextBox.Text = this.m_tmpTruthTableData.SaveBinaryString();
			}
			finally
			{
				this.m_ignoreTextChanges = false;
			}
			for (int i = 0; i < 16; i++)
			{
				if (this.m_lineCheckboxes[i].IsClicked)
				{
					this.m_tmpTruthTableData.Data[i] = (byte)((this.m_tmpTruthTableData.Data[i] == 0) ? 15 : 0);
				}
				this.m_lineCheckboxes[i].IsChecked = (this.m_tmpTruthTableData.Data[i] > 0);
			}
			if (this.m_linearPanel.IsVisible)
			{
				this.m_switchViewButton.Text = "Table";
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
				this.m_truthTableData.Data = this.m_tmpTruthTableData.Data;
				this.Dismiss(true);
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss(false);
			}
		}

		// Token: 0x06001237 RID: 4663 RVA: 0x0008D844 File Offset: 0x0008BA44
		public void Dismiss(bool result)
		{
			DialogsManager.HideDialog(this);
			if (this.m_handler != null)
			{
				this.m_handler(result);
			}
		}

		// Token: 0x04000C70 RID: 3184
		public Action<bool> m_handler;

		// Token: 0x04000C71 RID: 3185
		public Widget m_linearPanel;

		// Token: 0x04000C72 RID: 3186
		public Widget m_gridPanel;

		// Token: 0x04000C73 RID: 3187
		public ButtonWidget m_okButton;

		// Token: 0x04000C74 RID: 3188
		public ButtonWidget m_cancelButton;

		// Token: 0x04000C75 RID: 3189
		public ButtonWidget m_switchViewButton;

		// Token: 0x04000C76 RID: 3190
		public CheckboxWidget[] m_lineCheckboxes = new CheckboxWidget[16];

		// Token: 0x04000C77 RID: 3191
		public TextBoxWidget m_linearTextBox;

		// Token: 0x04000C78 RID: 3192
		public TruthTableData m_truthTableData;

		// Token: 0x04000C79 RID: 3193
		public TruthTableData m_tmpTruthTableData;

		// Token: 0x04000C7A RID: 3194
		public bool m_ignoreTextChanges;
	}
}
