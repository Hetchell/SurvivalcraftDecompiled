using System;
using System.Linq;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001B4 RID: 436
	public class SubsystemTreasureGeneratorBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000AD5 RID: 2773 RVA: 0x0005026B File Offset: 0x0004E46B
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					190
				};
			}
		}

		// Token: 0x06000AD6 RID: 2774 RVA: 0x0005027C File Offset: 0x0004E47C
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(neighborX, neighborY, neighborZ);
			if (cellContents != 0 && cellContents != 18)
			{
				return;
			}
			base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(0), true);
			if (!this.m_random.Bool(0.25f))
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			float max = SubsystemTreasureGeneratorBlockBehavior.m_treasureData.Sum((SubsystemTreasureGeneratorBlockBehavior.TreasureData t) => t.Probability);
			float num3 = this.m_random.Float(0f, max);
			foreach (SubsystemTreasureGeneratorBlockBehavior.TreasureData treasureData2 in SubsystemTreasureGeneratorBlockBehavior.m_treasureData)
			{
				num3 -= treasureData2.Probability;
				if (num3 <= 0f)
				{
					num = treasureData2.Value;
					num2 = this.m_random.Int(1, treasureData2.MaxCount);
					break;
				}
			}
			if (num != 0 && num2 > 0)
			{
				for (int j = 0; j < num2; j++)
				{
					this.m_subsystemPickables.AddPickable(num, 1, new Vector3((float)x, (float)y, (float)z) + this.m_random.Vector3(0.1f, 0.4f) + new Vector3(0.5f), new Vector3?(Vector3.Zero), null);
				}
				int num4 = this.m_random.Int(3, 6);
				for (int k = 0; k < num4; k++)
				{
					this.m_subsystemPickables.AddPickable(248, 1, new Vector3((float)x, (float)y, (float)z) + this.m_random.Vector3(0.1f, 0.4f) + new Vector3(0.5f), new Vector3?(Vector3.Zero), null);
				}
			}
		}

        // Token: 0x06000AD7 RID: 2775 RVA: 0x00050457 File Offset: 0x0004E657
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x00050474 File Offset: 0x0004E674
		static SubsystemTreasureGeneratorBlockBehavior()
		{
			SubsystemTreasureGeneratorBlockBehavior.TreasureData[] array = new SubsystemTreasureGeneratorBlockBehavior.TreasureData[61];
			SubsystemTreasureGeneratorBlockBehavior.TreasureData treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 79,
				Probability = 4f,
				MaxCount = 4
			};
			array[0] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 111,
				Probability = 1f,
				MaxCount = 1
			};
			array[1] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 43,
				Probability = 4f,
				MaxCount = 4
			};
			array[2] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 40,
				Probability = 2f,
				MaxCount = 3
			};
			array[3] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 42,
				Probability = 4f,
				MaxCount = 3
			};
			array[4] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 22,
				Probability = 4f,
				MaxCount = 4
			};
			array[5] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 103,
				Probability = 2f,
				MaxCount = 4
			};
			array[6] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 150,
				Probability = 1f,
				MaxCount = 1
			};
			array[7] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 21,
				Probability = 2f,
				MaxCount = 16
			};
			array[8] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 159,
				Probability = 2f,
				MaxCount = 4
			};
			array[9] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 207,
				Probability = 2f,
				MaxCount = 4
			};
			array[10] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 17,
				Probability = 2f,
				MaxCount = 2
			};
			array[11] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 31,
				Probability = 4f,
				MaxCount = 4
			};
			array[12] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 108,
				Probability = 4f,
				MaxCount = 8
			};
			array[13] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 109,
				Probability = 2f,
				MaxCount = 4
			};
			array[14] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 105,
				Probability = 1f,
				MaxCount = 4
			};
			array[15] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 106,
				Probability = 1f,
				MaxCount = 2
			};
			array[16] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 107,
				Probability = 1f,
				MaxCount = 1
			};
			array[17] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 234,
				Probability = 1f,
				MaxCount = 4
			};
			array[18] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 235,
				Probability = 1f,
				MaxCount = 2
			};
			array[19] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 236,
				Probability = 1f,
				MaxCount = 1
			};
			array[20] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 132,
				Probability = 2f,
				MaxCount = 2
			};
			array[21] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(173, 0, 6),
				Probability = 2f,
				MaxCount = 8
			};
			array[22] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(173, 0, 7),
				Probability = 8f,
				MaxCount = 8
			};
			array[23] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(173, 0, 5),
				Probability = 8f,
				MaxCount = 8
			};
			array[24] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(173, 0, 6),
				Probability = 2f,
				MaxCount = 8
			};
			array[25] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(119, 0, 0),
				Probability = 2f,
				MaxCount = 8
			};
			array[26] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(119, 0, 1),
				Probability = 2f,
				MaxCount = 8
			};
			array[27] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(119, 0, 2),
				Probability = 2f,
				MaxCount = 8
			};
			array[28] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(119, 0, 3),
				Probability = 2f,
				MaxCount = 8
			};
			array[29] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(119, 0, 4),
				Probability = 2f,
				MaxCount = 8
			};
			array[30] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 191,
				Probability = 4f,
				MaxCount = 1
			};
			array[31] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, ArrowBlock.ArrowType.CopperArrow)),
				Probability = 2f,
				MaxCount = 2
			};
			array[32] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, ArrowBlock.ArrowType.IronArrow)),
				Probability = 2f,
				MaxCount = 2
			};
			array[33] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, ArrowBlock.ArrowType.DiamondArrow)),
				Probability = 1f,
				MaxCount = 2
			};
			array[34] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, ArrowBlock.ArrowType.FireArrow)),
				Probability = 2f,
				MaxCount = 2
			};
			array[35] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 200,
				Probability = 1f,
				MaxCount = 1
			};
			array[36] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, ArrowBlock.ArrowType.IronBolt)),
				Probability = 2f,
				MaxCount = 2
			};
			array[37] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, ArrowBlock.ArrowType.DiamondBolt)),
				Probability = 1f,
				MaxCount = 2
			};
			array[38] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, ArrowBlock.ArrowType.ExplosiveBolt)),
				Probability = 1f,
				MaxCount = 2
			};
			array[39] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 212,
				Probability = 1f,
				MaxCount = 1
			};
			array[40] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 124,
				Probability = 1f,
				MaxCount = 1
			};
			array[41] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 125,
				Probability = 1f,
				MaxCount = 1
			};
			array[42] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 82,
				Probability = 1f,
				MaxCount = 1
			};
			array[43] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 116,
				Probability = 1f,
				MaxCount = 1
			};
			array[44] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 36,
				Probability = 1f,
				MaxCount = 1
			};
			array[45] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 113,
				Probability = 1f,
				MaxCount = 1
			};
			array[46] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 38,
				Probability = 1f,
				MaxCount = 1
			};
			array[47] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 115,
				Probability = 1f,
				MaxCount = 1
			};
			array[48] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 37,
				Probability = 1f,
				MaxCount = 1
			};
			array[49] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 114,
				Probability = 1f,
				MaxCount = 1
			};
			array[50] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 171,
				Probability = 1f,
				MaxCount = 1
			};
			array[51] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 172,
				Probability = 1f,
				MaxCount = 1
			};
			array[52] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 90,
				Probability = 1f,
				MaxCount = 1
			};
			array[53] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 160,
				Probability = 1f,
				MaxCount = 1
			};
			array[54] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 158,
				Probability = 2f,
				MaxCount = 1
			};
			array[55] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 133,
				Probability = 1f,
				MaxCount = 10
			};
			array[56] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 179,
				Probability = 1f,
				MaxCount = 2
			};
			array[57] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 142,
				Probability = 1f,
				MaxCount = 2
			};
			array[58] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 141,
				Probability = 1f,
				MaxCount = 2
			};
			array[59] = treasureData;
			treasureData = new SubsystemTreasureGeneratorBlockBehavior.TreasureData
			{
				Value = 237,
				Probability = 1f,
				MaxCount = 2
			};
			array[60] = treasureData;
			SubsystemTreasureGeneratorBlockBehavior.m_treasureData = array;
		}

		// Token: 0x040005ED RID: 1517
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x040005EE RID: 1518
		public Game.Random m_random = new Game.Random();

		// Token: 0x040005EF RID: 1519
		public static SubsystemTreasureGeneratorBlockBehavior.TreasureData[] m_treasureData;

		// Token: 0x02000442 RID: 1090
		public struct TreasureData
		{
			// Token: 0x04001611 RID: 5649
			public int Value;

			// Token: 0x04001612 RID: 5650
			public float Probability;

			// Token: 0x04001613 RID: 5651
			public int MaxCount;
		}
	}
}
