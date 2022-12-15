using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001A2 RID: 418
	public class SubsystemRotBlockBehavior : SubsystemPollableBlockBehavior
	{
		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060009F4 RID: 2548 RVA: 0x00048623 File Offset: 0x00046823
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

        // Token: 0x060009F5 RID: 2549 RVA: 0x0004862C File Offset: 0x0004682C
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemItemsScanner = base.Project.FindSubsystem<SubsystemItemsScanner>(true);
			this.m_lastRotTime = valuesDictionary.GetValue<double>("LastRotTime");
			this.m_rotStep = valuesDictionary.GetValue<int>("RotStep");
			this.m_subsystemItemsScanner.ItemsScanned += this.ItemsScanned;
			this.m_isRotEnabled = (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative && this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Adventure);
		}

        // Token: 0x060009F6 RID: 2550 RVA: 0x000486CE File Offset: 0x000468CE
        public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			valuesDictionary.SetValue<double>("LastRotTime", this.m_lastRotTime);
			valuesDictionary.SetValue<int>("RotStep", this.m_rotStep);
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x000486FC File Offset: 0x000468FC
		public override void OnPoll(int value, int x, int y, int z, int pollPass)
		{
			if (this.m_isRotEnabled)
			{
				int num = Terrain.ExtractContents(value);
				Block block = BlocksManager.Blocks[num];
				int rotPeriod = block.GetRotPeriod(value);
				if (rotPeriod > 0 && pollPass % rotPeriod == 0)
				{
					int num2 = block.GetDamage(value) + 1;
					value = ((num2 > 1) ? block.GetDamageDestructionValue(value) : block.SetDamage(value, num2));
					base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
				}
			}
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x00048764 File Offset: 0x00046964
		public void ItemsScanned(ReadOnlyList<ScannedItemData> items)
		{
			int num = (int)((this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_lastRotTime) / 60.0);
			if (num > 0)
			{
				if (this.m_isRotEnabled)
				{
					foreach (ScannedItemData scannedItemData in items)
					{
						int num2 = Terrain.ExtractContents(scannedItemData.Value);
						Block block = BlocksManager.Blocks[num2];
						int rotPeriod = block.GetRotPeriod(scannedItemData.Value);
						if (rotPeriod > 0)
						{
							int num3 = block.GetDamage(scannedItemData.Value);
							int num4 = 0;
							while (num4 < num && num3 <= 1)
							{
								if ((num4 + this.m_rotStep) % rotPeriod == 0)
								{
									num3++;
								}
								num4++;
							}
							if (num3 <= 1)
							{
								this.m_subsystemItemsScanner.TryModifyItem(scannedItemData, block.SetDamage(scannedItemData.Value, num3));
							}
							else
							{
								this.m_subsystemItemsScanner.TryModifyItem(scannedItemData, block.GetDamageDestructionValue(scannedItemData.Value));
							}
						}
					}
				}
				this.m_rotStep += num;
				this.m_lastRotTime += (double)((float)num * 60f);
			}
		}

		// Token: 0x04000550 RID: 1360
		public const int MaxRot = 1;

		// Token: 0x04000551 RID: 1361
		public SubsystemItemsScanner m_subsystemItemsScanner;

		// Token: 0x04000552 RID: 1362
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000553 RID: 1363
		public double m_lastRotTime;

		// Token: 0x04000554 RID: 1364
		public int m_rotStep;

		// Token: 0x04000555 RID: 1365
		public const float m_rotPeriod = 60f;

		// Token: 0x04000556 RID: 1366
		public bool m_isRotEnabled;
	}
}
