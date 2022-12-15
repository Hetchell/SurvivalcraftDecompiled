using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace Game
{
	// Token: 0x0200032A RID: 810
	public class TruthTableData : IEditableItemData
	{
		// Token: 0x06001721 RID: 5921 RVA: 0x000B8A84 File Offset: 0x000B6C84
		public IEditableItemData Copy()
		{
			return new TruthTableData
			{
				Data = (byte[])this.Data.Clone()
			};
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x000B8AA4 File Offset: 0x000B6CA4
		public void LoadString(string data)
		{
			for (int i = 0; i < 16; i++)
			{
				int num = (i < data.Length) ? TruthTableData.m_hexChars.IndexOf(char.ToUpperInvariant(data[i])) : 0;
				if (num < 0)
				{
					num = 0;
				}
				this.Data[i] = (byte)num;
			}
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x000B8AF4 File Offset: 0x000B6CF4
		public void LoadBinaryString(string data)
		{
			for (int i = 0; i < 16; i++)
			{
				this.Data[i] = (byte)((i < data.Length && data[i] != '0') ? 15 : 0);
			}
		}

		// Token: 0x06001724 RID: 5924 RVA: 0x000B8B30 File Offset: 0x000B6D30
		public string SaveString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.Data.Length; i++)
			{
				int index = MathUtils.Clamp((int)this.Data[i], 0, 15);
				stringBuilder.Append(TruthTableData.m_hexChars[index]);
			}
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				'0'
			});
		}

		// Token: 0x06001725 RID: 5925 RVA: 0x000B8B90 File Offset: 0x000B6D90
		public string SaveBinaryString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.Data.Length; i++)
			{
				stringBuilder.Append((this.Data[i] != 0) ? '1' : '0');
			}
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				'0'
			});
		}

		// Token: 0x040010D0 RID: 4304
		public static List<char> m_hexChars = new List<char>
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F'
		};

		// Token: 0x040010D1 RID: 4305
		public byte[] Data = new byte[16];
	}
}
