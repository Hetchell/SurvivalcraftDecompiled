using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x0200006A RID: 106
	public abstract class FlowerBlock : CrossBlock
	{
		// Token: 0x0600022C RID: 556 RVA: 0x0000D56A File Offset: 0x0000B76A
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (!FlowerBlock.GetIsSmall(Terrain.ExtractData(value)))
			{
				return base.GetFaceTextureSlot(face, value);
			}
			return 11;
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000D584 File Offset: 0x0000B784
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			if (!FlowerBlock.GetIsSmall(data))
			{
				dropValues.Add(new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(Terrain.ExtractContents(oldValue), 0, data),
					Count = 1
				});
			}
			showDebris = true;
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000D5D0 File Offset: 0x0000B7D0
		public override int GetShadowStrength(int value)
		{
			if (!FlowerBlock.GetIsSmall(Terrain.ExtractData(value)))
			{
				return this.DefaultShadowStrength;
			}
			return this.DefaultShadowStrength / 2;
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0000D5EE File Offset: 0x0000B7EE
		public static bool GetIsSmall(int data)
		{
			return (data & 1) != 0;
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000D5F6 File Offset: 0x0000B7F6
		public static int SetIsSmall(int data, bool isSmall)
		{
			if (!isSmall)
			{
				return data & -2;
			}
			return data | 1;
		}
	}
}
