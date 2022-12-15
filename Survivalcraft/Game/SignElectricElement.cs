using System;
using Engine;

namespace Game
{
	// Token: 0x020002F2 RID: 754
	public class SignElectricElement : ElectricElement
	{
		// Token: 0x06001573 RID: 5491 RVA: 0x000A37DB File Offset: 0x000A19DB
		public SignElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x06001574 RID: 5492 RVA: 0x000A37EC File Offset: 0x000A19EC
		public override bool Simulate()
		{
			bool flag = base.CalculateHighInputsCount() > 0;
			if (flag && this.m_isMessageAllowed && (this.m_lastMessageTime == null || base.SubsystemElectricity.SubsystemTime.GameTime - this.m_lastMessageTime.Value > 0.5))
			{
				this.m_isMessageAllowed = false;
				this.m_lastMessageTime = new double?(base.SubsystemElectricity.SubsystemTime.GameTime);
				SignData signData = base.SubsystemElectricity.Project.FindSubsystem<SubsystemSignBlockBehavior>(true).GetSignData(new Point3(base.CellFaces[0].X, base.CellFaces[0].Y, base.CellFaces[0].Z));
				if (signData != null)
				{
					string text = string.Join("\n", signData.Lines);
					text = text.Trim(new char[]
					{
						'\n'
					});
					text = text.Replace("\\\n", "");
					Color color = (signData.Colors[0] == Color.Black) ? Color.White : signData.Colors[0];
					color *= 255f / (float)MathUtils.Max((int)color.R, (int)color.G, (int)color.B);
					foreach (ComponentPlayer componentPlayer in base.SubsystemElectricity.Project.FindSubsystem<SubsystemPlayers>(true).ComponentPlayers)
					{
						componentPlayer.ComponentGui.DisplaySmallMessage(text, color, true, true);
					}
				}
			}
			if (!flag)
			{
				this.m_isMessageAllowed = true;
			}
			return false;
		}

		// Token: 0x04000F3A RID: 3898
		public bool m_isMessageAllowed = true;

		// Token: 0x04000F3B RID: 3899
		public double? m_lastMessageTime;
	}
}
