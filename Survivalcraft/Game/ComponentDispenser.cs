using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D4 RID: 468
	public class ComponentDispenser : ComponentInventoryBase
	{
		// Token: 0x06000C98 RID: 3224 RVA: 0x0005E498 File Offset: 0x0005C698
		public void Dispense()
		{
			Point3 coordinates = this.m_componentBlockEntity.Coordinates;
			int data = Terrain.ExtractData(this.m_subsystemTerrain.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z));
			int direction = DispenserBlock.GetDirection(data);
			DispenserBlock.Mode mode = DispenserBlock.GetMode(data);
			for (int i = 0; i < this.SlotsCount; i++)
			{
				int slotValue = this.GetSlotValue(i);
				int slotCount = this.GetSlotCount(i);
				if (slotValue != 0 && slotCount > 0)
				{
					int num = this.RemoveSlotItems(i, 1);
					for (int j = 0; j < num; j++)
					{
						this.DispenseItem(coordinates, direction, slotValue, mode);
					}
					return;
				}
			}
		}

        // Token: 0x06000C99 RID: 3225 RVA: 0x0005E538 File Offset: 0x0005C738
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_subsystemProjectiles = base.Project.FindSubsystem<SubsystemProjectiles>(true);
			this.m_componentBlockEntity = base.Entity.FindComponent<ComponentBlockEntity>(true);
		}

		// Token: 0x06000C9A RID: 3226 RVA: 0x0005E5A8 File Offset: 0x0005C7A8
		public void DispenseItem(Point3 point, int face, int value, DispenserBlock.Mode mode)
		{
			Vector3 vector = CellFace.FaceToVector3(face);
			Vector3 vector2 = new Vector3((float)point.X + 0.5f, (float)point.Y + 0.5f, (float)point.Z + 0.5f) + 0.6f * vector;
			if (mode == DispenserBlock.Mode.Dispense)
			{
				float s = 1.8f;
				this.m_subsystemPickables.AddPickable(value, 1, vector2, new Vector3?(s * (vector + this.m_random.Vector3(0.2f))), null);
				this.m_subsystemAudio.PlaySound("Audio/DispenserDispense", 1f, 0f, new Vector3(vector2.X, vector2.Y, vector2.Z), 3f, true);
				return;
			}
			float s2 = this.m_random.Float(39f, 41f);
			if (this.m_subsystemProjectiles.FireProjectile(value, vector2, s2 * (vector + this.m_random.Vector3(0.025f) + new Vector3(0f, 0.05f, 0f)), Vector3.Zero, null) != null)
			{
				this.m_subsystemAudio.PlaySound("Audio/DispenserShoot", 1f, 0f, new Vector3(vector2.X, vector2.Y, vector2.Z), 4f, true);
				return;
			}
			this.DispenseItem(point, face, value, DispenserBlock.Mode.Dispense);
		}

		// Token: 0x04000758 RID: 1880
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000759 RID: 1881
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400075A RID: 1882
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x0400075B RID: 1883
		public SubsystemProjectiles m_subsystemProjectiles;

		// Token: 0x0400075C RID: 1884
		public ComponentBlockEntity m_componentBlockEntity;

		// Token: 0x0400075D RID: 1885
		public new Game.Random m_random = new Game.Random();
	}
}
