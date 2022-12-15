using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000166 RID: 358
	public class SubsystemBucketBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000713 RID: 1811 RVA: 0x0002CF6D File Offset: 0x0002B16D
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					90,
					91,
					93,
					110,
					245,
					251,
					252,
					129,
					128
				};
			}
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x0002CF84 File Offset: 0x0002B184
		public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			IInventory inventory = componentMiner.Inventory;
			int activeBlockValue = componentMiner.ActiveBlockValue;
			int num = Terrain.ExtractContents(activeBlockValue);
			if (num == 90)
			{
				object obj = componentMiner.Raycast(ray, RaycastMode.Gathering, true, true, true);
				if (obj is TerrainRaycastResult)
				{
					CellFace cellFace = ((TerrainRaycastResult)obj).CellFace;
					int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
					int num2 = Terrain.ExtractContents(cellValue);
					int data = Terrain.ExtractData(cellValue);
					Block block = BlocksManager.Blocks[num2];
					if (block is WaterBlock && FluidBlock.GetLevel(data) == 0)
					{
						int value = Terrain.ReplaceContents(activeBlockValue, 91);
						inventory.RemoveSlotItems(inventory.ActiveSlotIndex, inventory.GetSlotCount(inventory.ActiveSlotIndex));
						if (inventory.GetSlotCount(inventory.ActiveSlotIndex) == 0)
						{
							inventory.AddSlotItems(inventory.ActiveSlotIndex, value, 1);
						}
						base.SubsystemTerrain.DestroyCell(0, cellFace.X, cellFace.Y, cellFace.Z, 0, false, false);
						return true;
					}
					if (block is MagmaBlock && FluidBlock.GetLevel(data) == 0)
					{
						int value2 = Terrain.ReplaceContents(activeBlockValue, 93);
						inventory.RemoveSlotItems(inventory.ActiveSlotIndex, inventory.GetSlotCount(inventory.ActiveSlotIndex));
						if (inventory.GetSlotCount(inventory.ActiveSlotIndex) == 0)
						{
							inventory.AddSlotItems(inventory.ActiveSlotIndex, value2, 1);
						}
						base.SubsystemTerrain.DestroyCell(0, cellFace.X, cellFace.Y, cellFace.Z, 0, false, false);
						return true;
					}
				}
				else if (obj is BodyRaycastResult)
				{
					ComponentUdder componentUdder = ((BodyRaycastResult)obj).ComponentBody.Entity.FindComponent<ComponentUdder>();
					if (componentUdder != null && componentUdder.Milk(componentMiner))
					{
						int value3 = Terrain.ReplaceContents(activeBlockValue, 110);
						inventory.RemoveSlotItems(inventory.ActiveSlotIndex, inventory.GetSlotCount(inventory.ActiveSlotIndex));
						if (inventory.GetSlotCount(inventory.ActiveSlotIndex) == 0)
						{
							inventory.AddSlotItems(inventory.ActiveSlotIndex, value3, 1);
						}
						this.m_subsystemAudio.PlaySound("Audio/Milked", 1f, 0f, ray.Position, 2f, true);
					}
					return true;
				}
			}
			if (num == 91)
			{
				TerrainRaycastResult? terrainRaycastResult = componentMiner.Raycast<TerrainRaycastResult>(ray, RaycastMode.Interaction, true, true, true);
				if (terrainRaycastResult != null && componentMiner.Place(terrainRaycastResult.Value, Terrain.MakeBlockValue(18)))
				{
					inventory.RemoveSlotItems(inventory.ActiveSlotIndex, 1);
					if (inventory.GetSlotCount(inventory.ActiveSlotIndex) == 0)
					{
						int value4 = Terrain.ReplaceContents(activeBlockValue, 90);
						inventory.AddSlotItems(inventory.ActiveSlotIndex, value4, 1);
					}
					return true;
				}
			}
			if (num == 93)
			{
				TerrainRaycastResult? terrainRaycastResult2 = componentMiner.Raycast<TerrainRaycastResult>(ray, RaycastMode.Interaction, true, true, true);
				if (terrainRaycastResult2 != null)
				{
					if (componentMiner.Place(terrainRaycastResult2.Value, Terrain.MakeBlockValue(92)))
					{
						inventory.RemoveSlotItems(inventory.ActiveSlotIndex, 1);
						if (inventory.GetSlotCount(inventory.ActiveSlotIndex) == 0)
						{
							int value5 = Terrain.ReplaceContents(activeBlockValue, 90);
							inventory.AddSlotItems(inventory.ActiveSlotIndex, value5, 1);
						}
					}
					return true;
				}
			}
			if (num <= 129)
			{
				if (num != 110)
				{
					if (num - 128 > 1)
					{
						return false;
					}
					TerrainRaycastResult? terrainRaycastResult3 = componentMiner.Raycast<TerrainRaycastResult>(ray, RaycastMode.Digging, true, true, true);
					if (terrainRaycastResult3 != null)
					{
						CellFace cellFace2 = terrainRaycastResult3.Value.CellFace;
						int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(cellFace2.X, cellFace2.Y, cellFace2.Z);
						int num3 = Terrain.ExtractContents(cellValue2);
						Block block2 = BlocksManager.Blocks[num3];
						if (block2 is IPaintableBlock)
						{
							Vector3 normal = CellFace.FaceToVector3(terrainRaycastResult3.Value.CellFace.Face);
							Vector3 position = terrainRaycastResult3.Value.HitPoint(0f);
							int? num4 = (num == 128) ? null : new int?(PaintBucketBlock.GetColor(Terrain.ExtractData(activeBlockValue)));
							Color color = (num4 != null) ? SubsystemPalette.GetColor(base.SubsystemTerrain, num4) : new Color(128, 128, 128, 128);
							int value6 = ((IPaintableBlock)block2).Paint(base.SubsystemTerrain, cellValue2, num4);
							base.SubsystemTerrain.ChangeCell(cellFace2.X, cellFace2.Y, cellFace2.Z, value6, true);
							componentMiner.DamageActiveTool(1);
							this.m_subsystemAudio.PlayRandomSound("Audio/Paint", 0.4f, this.m_random.Float(-0.1f, 0.1f), componentMiner.ComponentCreature.ComponentBody.Position, 2f, true);
							this.m_subsystemParticles.AddParticleSystem(new PaintParticleSystem(base.SubsystemTerrain, position, normal, color));
						}
						return true;
					}
					return false;
				}
			}
			else if (num != 245)
			{
				if (num - 251 > 1)
				{
					return false;
				}
				return true;
			}
			return true;
		}

        // Token: 0x06000715 RID: 1813 RVA: 0x0002D43B File Offset: 0x0002B63B
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
		}

		// Token: 0x040003ED RID: 1005
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040003EE RID: 1006
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x040003EF RID: 1007
		public Game.Random m_random = new Game.Random();
	}
}
