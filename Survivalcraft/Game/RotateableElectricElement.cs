using System;
using Engine;

namespace Game
{
	// Token: 0x020002E7 RID: 743
	public abstract class RotateableElectricElement : MountedElectricElement
	{
		// Token: 0x1700031E RID: 798
		// (get) Token: 0x060014DF RID: 5343 RVA: 0x000A1DC8 File Offset: 0x0009FFC8
		// (set) Token: 0x060014E0 RID: 5344 RVA: 0x000A1E18 File Offset: 0x000A0018
		public int Rotation
		{
			get
			{
				CellFace cellFace = base.CellFaces[0];
				return RotateableMountedElectricElementBlock.GetRotation(Terrain.ExtractData(base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z)));
			}
			set
			{
				CellFace cellFace = base.CellFaces[0];
				int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
				int value2 = Terrain.ReplaceData(cellValue, RotateableMountedElectricElementBlock.SetRotation(Terrain.ExtractData(cellValue), value % 4));
				base.SubsystemElectricity.SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y, cellFace.Z, value2, true);
				base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Click", 1f, 0f, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 2f, true);
			}
		}

		// Token: 0x060014E1 RID: 5345 RVA: 0x000A1ED3 File Offset: 0x000A00D3
		public RotateableElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
		}

		// Token: 0x060014E2 RID: 5346 RVA: 0x000A1EE0 File Offset: 0x000A00E0
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			int rotation = this.Rotation + 1;
			this.Rotation = rotation;
			return true;
		}
	}
}
