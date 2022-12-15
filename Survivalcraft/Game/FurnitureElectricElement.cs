using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x0200027C RID: 636
	public abstract class FurnitureElectricElement : ElectricElement
	{
		// Token: 0x060012D7 RID: 4823 RVA: 0x000930BD File Offset: 0x000912BD
		public FurnitureElectricElement(SubsystemElectricity subsystemElectricity, Point3 point) : base(subsystemElectricity, FurnitureElectricElement.GetMountingCellFaces(subsystemElectricity, point))
		{
		}

		// Token: 0x060012D8 RID: 4824 RVA: 0x000930CD File Offset: 0x000912CD
		public static IEnumerable<CellFace> GetMountingCellFaces(SubsystemElectricity subsystemElectricity, Point3 point)
		{
			int data = Terrain.ExtractData(subsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z));
			int rotation = FurnitureBlock.GetRotation(data);
			int designIndex = FurnitureBlock.GetDesignIndex(data);
			FurnitureDesign design = subsystemElectricity.SubsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
			if (design == null)
			{
				yield break;
			}
			int num2;
			for (int face = 0; face < 6; face = num2)
			{
				int num = (face < 4) ? ((face - rotation + 4) % 4) : face;
				if ((design.MountingFacesMask & 1 << num) != 0)
				{
					yield return new CellFace(point.X, point.Y, point.Z, CellFace.OppositeFace(face));
				}
				num2 = face + 1;
			}
			yield break;
		}
	}
}
