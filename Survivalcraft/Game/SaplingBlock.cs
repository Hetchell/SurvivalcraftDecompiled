using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x020000E9 RID: 233
	public class SaplingBlock : CrossBlock
	{
		// Token: 0x06000463 RID: 1123 RVA: 0x00017814 File Offset: 0x00015A14
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			switch (Terrain.ExtractData(value))
			{
			case 0:
				return "橡树树苗";
			case 1:
				return "白桦树苗";
			case 2:
				return "云杉树苗";
			case 3:
				return "高云杉树苗";
			case 4:
				return "合金欢树幼苗";
			default:
				return "树苗";
			}
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x00017868 File Offset: 0x00015A68
		public override int GetFaceTextureSlot(int face, int value)
		{
			switch (Terrain.ExtractData(value))
			{
			case 0:
				return 56;
			case 1:
				return 72;
			case 2:
				return 73;
			case 3:
				return 73;
			case 4:
				return 72;
			default:
				return 56;
			}
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x000178A9 File Offset: 0x00015AA9
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(119, 0, 0);
			yield return Terrain.MakeBlockValue(119, 0, 1);
			yield return Terrain.MakeBlockValue(119, 0, 2);
			yield return Terrain.MakeBlockValue(119, 0, 3);
			yield return Terrain.MakeBlockValue(119, 0, 4);
			yield break;
		}

		// Token: 0x040001F8 RID: 504
		public const int Index = 119;
	}
}
