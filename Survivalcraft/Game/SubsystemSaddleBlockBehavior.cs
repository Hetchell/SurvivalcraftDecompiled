using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001A3 RID: 419
	public class SubsystemSaddleBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060009FA RID: 2554 RVA: 0x000488AC File Offset: 0x00046AAC
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					158
				};
			}
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x000488BC File Offset: 0x00046ABC
		public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			BodyRaycastResult? bodyRaycastResult = componentMiner.Raycast<BodyRaycastResult>(ray, RaycastMode.Interaction, true, true, true);
			if (bodyRaycastResult != null)
			{
				ComponentHealth componentHealth = bodyRaycastResult.Value.ComponentBody.Entity.FindComponent<ComponentHealth>();
				if (componentHealth == null || componentHealth.Health > 0f)
				{
					string entityTemplateName = bodyRaycastResult.Value.ComponentBody.Entity.ValuesDictionary.DatabaseObject.Name + "_Saddled";
					Entity entity = DatabaseManager.CreateEntity(base.Project, entityTemplateName, false);
					if (entity != null)
					{
						ComponentBody componentBody = entity.FindComponent<ComponentBody>(true);
						componentBody.Position = bodyRaycastResult.Value.ComponentBody.Position;
						componentBody.Rotation = bodyRaycastResult.Value.ComponentBody.Rotation;
						componentBody.setVectorSpeed(bodyRaycastResult.Value.ComponentBody.getVectorSpeed());
						entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
						base.Project.RemoveEntity(bodyRaycastResult.Value.ComponentBody.Entity, true);
						base.Project.AddEntity(entity);
						this.m_subsystemAudio.PlaySound("Audio/BlockPlaced", 1f, this.m_random.Float(-0.1f, 0.1f), ray.Position, 1f, true);
						componentMiner.RemoveActiveTool(1);
					}
				}
				return true;
			}
			return false;
		}

        // Token: 0x060009FC RID: 2556 RVA: 0x00048A0F File Offset: 0x00046C0F
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
		}

		// Token: 0x04000557 RID: 1367
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000558 RID: 1368
		public Game.Random m_random = new Game.Random();
	}
}
