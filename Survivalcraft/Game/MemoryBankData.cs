using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace Game
{
	// Token: 0x020002AB RID: 683
	public class MemoryBankData : IEditableItemData
	{
		// Token: 0x170002DD RID: 733
		// (get) Token: 0x060013A2 RID: 5026 RVA: 0x000986F2 File Offset: 0x000968F2
		// (set) Token: 0x060013A3 RID: 5027 RVA: 0x000986FA File Offset: 0x000968FA
		public byte LastOutput { get; set; }

		// Token: 0x060013A4 RID: 5028 RVA: 0x00098703 File Offset: 0x00096903
		public byte Read(int address)
		{
			if (address >= 0 && address < this.Data.Count)
			{
				return this.Data.Array[address];
			}
			return 0;
		}

		// Token: 0x060013A5 RID: 5029 RVA: 0x00098728 File Offset: 0x00096928
		public void Write(int address, byte data)
		{
			if (address >= 0 && address < this.Data.Count)
			{
				this.Data.Array[address] = data;
				return;
			}
			if (address >= 0 && address < 256 && data != 0)
			{
				this.Data.Count = MathUtils.Max(this.Data.Count, address + 1);
				this.Data.Array[address] = data;
			}
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x00098791 File Offset: 0x00096991
		public IEditableItemData Copy()
		{
			return new MemoryBankData
			{
				Data = new DynamicArray<byte>(this.Data),
				LastOutput = this.LastOutput
			};
		}

		// Token: 0x060013A7 RID: 5031 RVA: 0x000987B8 File Offset: 0x000969B8
		public void LoadString(string data)
		{
			string[] array = data.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length >= 1)
			{
				string text = array[0];
				text = text.TrimEnd(new char[]
				{
					'0'
				});
				this.Data.Clear();
				for (int i = 0; i < MathUtils.Min(text.Length, 256); i++)
				{
					int num = MemoryBankData.m_hexChars.IndexOf(char.ToUpperInvariant(text[i]));
					if (num < 0)
					{
						num = 0;
					}
					this.Data.Add((byte)num);
				}
			}
			if (array.Length >= 2)
			{
				string text2 = array[1];
				int num2 = MemoryBankData.m_hexChars.IndexOf(char.ToUpperInvariant(text2[0]));
				if (num2 < 0)
				{
					num2 = 0;
				}
				this.LastOutput = (byte)num2;
			}
		}

		// Token: 0x060013A8 RID: 5032 RVA: 0x00098879 File Offset: 0x00096A79
		public string SaveString()
		{
			return this.SaveString(true);
		}

		// Token: 0x060013A9 RID: 5033 RVA: 0x00098884 File Offset: 0x00096A84
		public string SaveString(bool saveLastOutput)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			for (int i = 0; i < this.Data.Count; i++)
			{
				if (this.Data.Array[i] != 0)
				{
					num = i + 1;
				}
			}
			for (int j = 0; j < num; j++)
			{
				int index = MathUtils.Clamp((int)this.Data.Array[j], 0, 15);
				stringBuilder.Append(MemoryBankData.m_hexChars[index]);
			}
			if (saveLastOutput)
			{
				stringBuilder.Append(';');
				stringBuilder.Append(MemoryBankData.m_hexChars[MathUtils.Clamp((int)this.LastOutput, 0, 15)]);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04000D76 RID: 3446
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

		// Token: 0x04000D77 RID: 3447
		public DynamicArray<byte> Data = new DynamicArray<byte>();
	}
}
