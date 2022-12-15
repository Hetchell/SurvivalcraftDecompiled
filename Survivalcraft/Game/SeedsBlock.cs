using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000EC RID: 236
	public class SeedsBlock : FlatBlock
	{
		// Token: 0x06000473 RID: 1139 RVA: 0x00017E04 File Offset: 0x00016004
		public override IEnumerable<int> GetCreativeValues()
		{
			List<int> list = new List<int>();
			foreach (int data in EnumUtils.GetEnumValues(typeof(SeedsBlock.SeedType)))
			{
				list.Add(Terrain.MakeBlockValue(173, 0, data));
			}
			return list;
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x00017E6C File Offset: 0x0001606C
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			switch (Terrain.ExtractData(value))
			{
			case 0:
				return "高草种子";
			case 1:
				return "红花种子";
			case 2:
				return "紫花";
			case 3:
				return "白花种子";
			case 4:
				return "野生小麦种子";
			case 5:
				return "小麦种子";
			case 6:
				return "棉花种子";
			case 7:
				return "南瓜种子";
			default:
				return string.Empty;
			}
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x00017EE0 File Offset: 0x000160E0
		public override int GetFaceTextureSlot(int face, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num == 5 || num == 4)
			{
				return 74;
			}
			return 75;
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x00017F04 File Offset: 0x00016104
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			BlockPlacementData result = default(BlockPlacementData);
			result.CellFace = raycastResult.CellFace;
			if (raycastResult.CellFace.Face == 4)
			{
				switch (Terrain.ExtractData(value))
				{
				case 0:
					result.Value = Terrain.MakeBlockValue(19, 0, TallGrassBlock.SetIsSmall(0, true));
					break;
				case 1:
					result.Value = Terrain.MakeBlockValue(20, 0, FlowerBlock.SetIsSmall(0, true));
					break;
				case 2:
					result.Value = Terrain.MakeBlockValue(24, 0, FlowerBlock.SetIsSmall(0, true));
					break;
				case 3:
					result.Value = Terrain.MakeBlockValue(25, 0, FlowerBlock.SetIsSmall(0, true));
					break;
				case 4:
					result.Value = Terrain.MakeBlockValue(174, 0, RyeBlock.SetSize(RyeBlock.SetIsWild(0, false), 0));
					break;
				case 5:
					result.Value = Terrain.MakeBlockValue(174, 0, RyeBlock.SetSize(RyeBlock.SetIsWild(0, false), 0));
					break;
				case 6:
					result.Value = Terrain.MakeBlockValue(204, 0, CottonBlock.SetSize(CottonBlock.SetIsWild(0, false), 0));
					break;
				case 7:
					result.Value = Terrain.MakeBlockValue(131, 0, BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, false), 0));
					break;
				}
			}
			return result;
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x0001805C File Offset: 0x0001625C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			switch (Terrain.ExtractData(value))
			{
			case 0:
				color *= new Color(160, 150, 125);
				break;
			case 1:
				color *= new Color(192, 160, 160);
				break;
			case 2:
				color *= new Color(192, 160, 192);
				break;
			case 3:
				color *= new Color(192, 192, 192);
				break;
			case 4:
				color *= new Color(60, 138, 76);
				break;
			case 6:
				color *= new Color(255, 255, 255);
				break;
			case 7:
				color *= new Color(240, 225, 190);
				break;
			}
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}

		// Token: 0x04000200 RID: 512
		public const int Index = 173;

		// Token: 0x020003E3 RID: 995
		public enum SeedType
		{
			// Token: 0x0400148E RID: 5262
			TallGrass,
			// Token: 0x0400148F RID: 5263
			RedFlower,
			// Token: 0x04001490 RID: 5264
			PurpleFlower,
			// Token: 0x04001491 RID: 5265
			WhiteFlower,
			// Token: 0x04001492 RID: 5266
			WildRye,
			// Token: 0x04001493 RID: 5267
			Rye,
			// Token: 0x04001494 RID: 5268
			Cotton,
			// Token: 0x04001495 RID: 5269
			Pumpkin
		}
	}
}
