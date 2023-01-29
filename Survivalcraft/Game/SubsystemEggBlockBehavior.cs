using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000174 RID: 372
	public class SubsystemEggBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000806 RID: 2054 RVA: 0x000347E0 File Offset: 0x000329E0
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x000347E8 File Offset: 0x000329E8
		public override bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
		{
			int data = Terrain.ExtractData(worldItem.Value);
			bool isCooked = EggBlock.GetIsCooked(data);
			bool isLaid = EggBlock.GetIsLaid(data);
			if (!isCooked && (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative || this.m_random.Float(0f, 1f) <= (isLaid ? 0.15f : 1f)))
			{
				if (this.m_subsystemCreatureSpawn.Creatures.Count < 100)
				{
					EggBlock.EggType eggType = this.m_eggBlock.GetEggType(data);
					Entity entity = DatabaseManager.CreateEntity(base.Project, eggType.TemplateName, true);
					entity.FindComponent<ComponentBody>(true).Position = worldItem.Position;
					entity.FindComponent<ComponentBody>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, this.m_random.Float(0f, 6.2831855f));
					entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0.25f;
					base.Project.AddEntity(entity);
				}
				else
				{
					Projectile projectile = worldItem as Projectile;
					ComponentPlayer componentPlayer = ((projectile != null) ? projectile.Owner : null) as ComponentPlayer;
					if (componentPlayer != null)
					{
						componentPlayer.ComponentGui.DisplaySmallMessage("Too many creatures", Color.White, true, false);
					}
				}
			}
			return true;
		}

        // Token: 0x06000808 RID: 2056 RVA: 0x00034912 File Offset: 0x00032B12
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemCreatureSpawn = base.Project.FindSubsystem<SubsystemCreatureSpawn>(true);
		}

		// Token: 0x04000431 RID: 1073
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000432 RID: 1074
		public SubsystemCreatureSpawn m_subsystemCreatureSpawn;

		// Token: 0x04000433 RID: 1075
		public EggBlock m_eggBlock = (EggBlock)BlocksManager.Blocks[118];

		// Token: 0x04000434 RID: 1076
		public Game.Random m_random = new Game.Random();
	}
}
