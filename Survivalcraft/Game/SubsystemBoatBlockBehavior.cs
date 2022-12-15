using System;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000161 RID: 353
	public class SubsystemBoatBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060006EC RID: 1772 RVA: 0x0002BBFD File Offset: 0x00029DFD
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					178
				};
			}
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x0002BC10 File Offset: 0x00029E10
		public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			IInventory inventory = componentMiner.Inventory;
			if (Terrain.ExtractContents(componentMiner.ActiveBlockValue) == 178)
			{
				TerrainRaycastResult? terrainRaycastResult = componentMiner.Raycast<TerrainRaycastResult>(ray, RaycastMode.Digging, true, true, true);
				if (terrainRaycastResult != null)
				{
					Vector3 vector = terrainRaycastResult.Value.HitPoint(0f);
					DynamicArray<ComponentBody> dynamicArray = new DynamicArray<ComponentBody>();
					this.m_subsystemBodies.FindBodiesInArea(new Vector2(vector.X, vector.Z) - new Vector2(8f), new Vector2(vector.X, vector.Z) + new Vector2(8f), dynamicArray);
					if (dynamicArray.Count((ComponentBody b) => b.Entity.ValuesDictionary.DatabaseObject.Name == "Boat") < 6)
					{
						Entity entity = DatabaseManager.CreateEntity(base.Project, "Boat", true);
						entity.FindComponent<ComponentFrame>(true).Position = vector;
						entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, this.m_random.Float(0f, 6.2831855f));
						entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
						base.Project.AddEntity(entity);
						componentMiner.RemoveActiveTool(1);
						this.m_subsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, vector, 3f, true);
					}
					else
					{
						ComponentPlayer componentPlayer = componentMiner.ComponentPlayer;
						if (componentPlayer != null)
						{
							componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(SubsystemBoatBlockBehavior.fName, 1), Color.White, true, false);
						}
					}
					return true;
				}
			}
			return false;
		}

        // Token: 0x060006EE RID: 1774 RVA: 0x0002BDA4 File Offset: 0x00029FA4
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
		}

		// Token: 0x040003D8 RID: 984
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040003D9 RID: 985
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x040003DA RID: 986
		public Game.Random m_random = new Game.Random();

		// Token: 0x040003DB RID: 987
		public static string fName = "SubsystemBoatBlockBehavior";
	}
}
