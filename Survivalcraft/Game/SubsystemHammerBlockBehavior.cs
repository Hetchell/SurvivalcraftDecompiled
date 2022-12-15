using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000186 RID: 390
	public class SubsystemHammerBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060008EE RID: 2286 RVA: 0x0003DA27 File Offset: 0x0003BC27
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x060008EF RID: 2287 RVA: 0x0003DA30 File Offset: 0x0003BC30
		public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			TerrainRaycastResult? terrainRaycastResult = componentMiner.Raycast<TerrainRaycastResult>(ray, RaycastMode.Digging, true, true, true);
			if (terrainRaycastResult != null)
			{
				this.m_subsystemFurnitureBlockBehavior.ScanDesign(terrainRaycastResult.Value.CellFace, ray.Direction, componentMiner);
				return true;
			}
			return false;
		}

        // Token: 0x060008F0 RID: 2288 RVA: 0x0003DA73 File Offset: 0x0003BC73
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemFurnitureBlockBehavior = base.Project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true);
		}

		// Token: 0x040004B1 RID: 1201
		public SubsystemFurnitureBlockBehavior m_subsystemFurnitureBlockBehavior;
	}
}
