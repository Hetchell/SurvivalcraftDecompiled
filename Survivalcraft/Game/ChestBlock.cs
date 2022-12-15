using System;
using Engine;

namespace Game
{
	// Token: 0x02000031 RID: 49
	public class ChestBlock : CubeBlock
	{
		// Token: 0x06000135 RID: 309 RVA: 0x0000867C File Offset: 0x0000687C
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4)
			{
				return 42;
			}
			if (face == 5)
			{
				return 42;
			}
			switch (Terrain.ExtractData(value))
			{
			case 0:
				if (face == 0)
				{
					return 27;
				}
				if (face != 2)
				{
					return 25;
				}
				return 26;
			case 1:
				if (face == 1)
				{
					return 27;
				}
				if (face != 3)
				{
					return 25;
				}
				return 26;
			case 2:
				if (face == 0)
				{
					return 26;
				}
				if (face == 2)
				{
					return 27;
				}
				return 25;
			default:
				if (face == 1)
				{
					return 26;
				}
				if (face == 3)
				{
					return 27;
				}
				return 25;
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x000086FC File Offset: 0x000068FC
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int data = 0;
			if (num == MathUtils.Max(num, num2, num3, num4))
			{
				data = 2;
			}
			else if (num2 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 3;
			}
			else if (num3 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 0;
			}
			else if (num4 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 1;
			}
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, 45), data),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0400009C RID: 156
		public const int Index = 45;
	}
}
