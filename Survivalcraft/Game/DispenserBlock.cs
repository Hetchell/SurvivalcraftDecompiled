using System;
using Engine;

namespace Game
{
	// Token: 0x0200005C RID: 92
	public class DispenserBlock : CubeBlock, IElectricElementBlock
	{
		// Token: 0x060001A3 RID: 419 RVA: 0x0000A100 File Offset: 0x00008300
		public override int GetFaceTextureSlot(int face, int value)
		{
			int direction = DispenserBlock.GetDirection(Terrain.ExtractData(value));
			if (face != direction)
			{
				return 42;
			}
			return 59;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000A124 File Offset: 0x00008324
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			float num5 = Vector3.Dot(forward, Vector3.UnitY);
			float num6 = Vector3.Dot(forward, -Vector3.UnitY);
			float num7 = MathUtils.Min(MathUtils.Min(num, num2, num3), MathUtils.Min(num4, num5, num6));
			int direction = 0;
			if (num == num7)
			{
				direction = 0;
			}
			else if (num2 == num7)
			{
				direction = 1;
			}
			else if (num3 == num7)
			{
				direction = 2;
			}
			else if (num4 == num7)
			{
				direction = 3;
			}
			else if (num5 == num7)
			{
				direction = 4;
			}
			else if (num6 == num7)
			{
				direction = 5;
			}
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(216, 0, DispenserBlock.SetDirection(0, direction)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000A231 File Offset: 0x00008431
		public static int GetDirection(int data)
		{
			return data & 7;
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000A236 File Offset: 0x00008436
		public static int SetDirection(int data, int direction)
		{
			return (data & -8) | (direction & 7);
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000A240 File Offset: 0x00008440
		public static DispenserBlock.Mode GetMode(int data)
		{
			if ((data & 8) != 0)
			{
				return DispenserBlock.Mode.Shoot;
			}
			return DispenserBlock.Mode.Dispense;
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000A24A File Offset: 0x0000844A
		public static int SetMode(int data, DispenserBlock.Mode mode)
		{
			return (data & -9) | ((mode != DispenserBlock.Mode.Dispense) ? 8 : 0);
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000A258 File Offset: 0x00008458
		public static bool GetAcceptsDrops(int data)
		{
			return (data & 16) != 0;
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000A261 File Offset: 0x00008461
		public static int SetAcceptsDrops(int data, bool acceptsDrops)
		{
			return (data & -17) | (acceptsDrops ? 16 : 0);
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000A270 File Offset: 0x00008470
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new DispenserElectricElement(subsystemElectricity, new Point3(x, y, z));
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000A282 File Offset: 0x00008482
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return new ElectricConnectorType?(ElectricConnectorType.Input);
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000A28A File Offset: 0x0000848A
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x040000D7 RID: 215
		public const int Index = 216;

		// Token: 0x020003C4 RID: 964
		public enum Mode
		{
			// Token: 0x040013FD RID: 5117
			Dispense,
			// Token: 0x040013FE RID: 5118
			Shoot
		}
	}
}
