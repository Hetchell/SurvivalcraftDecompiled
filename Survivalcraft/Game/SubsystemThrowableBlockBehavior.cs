using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001AF RID: 431
	public class SubsystemThrowableBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000AA9 RID: 2729 RVA: 0x0004F393 File Offset: 0x0004D593
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000AAA RID: 2730 RVA: 0x0004F39C File Offset: 0x0004D59C
		public override bool OnAim(Ray3 aim, ComponentMiner componentMiner, AimState state)
		{
			if (state != AimState.InProgress)
			{
				if (state == AimState.Completed)
				{
					Vector3 vector = componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition + componentMiner.ComponentCreature.ComponentBody.Matrix.Right * 0.4f;
					Vector3 v = Vector3.Normalize(vector + aim.Direction * 10f - vector);
					if (componentMiner.Inventory != null)
					{
						int activeSlotIndex = componentMiner.Inventory.ActiveSlotIndex;
						int slotValue = componentMiner.Inventory.GetSlotValue(activeSlotIndex);
						int slotCount = componentMiner.Inventory.GetSlotCount(activeSlotIndex);
						int num = Terrain.ExtractContents(slotValue);
						Block block = BlocksManager.Blocks[num];
						if (slotCount > 0)
						{
							float num2 = block.ProjectileSpeed;
							if (componentMiner.ComponentPlayer != null)
							{
								num2 *= 0.5f * (componentMiner.ComponentPlayer.ComponentLevel.StrengthFactor - 1f) + 1f;
							}
							if (this.m_subsystemProjectiles.FireProjectile(slotValue, vector, v * num2, this.m_random.Vector3(5f, 10f), componentMiner.ComponentCreature) != null)
							{
								componentMiner.Inventory.RemoveSlotItems(activeSlotIndex, 1);
								this.m_subsystemAudio.PlaySound("Audio/Throw", this.m_random.Float(0.2f, 0.3f), this.m_random.Float(-0.2f, 0.2f), aim.Position, 2f, true);
								componentMiner.Poke(false);
							}
						}
					}
				}
			}
			else
			{
				componentMiner.ComponentCreature.ComponentCreatureModel.AimHandAngleOrder = 3.2f;
				Block block2 = BlocksManager.Blocks[Terrain.ExtractContents(componentMiner.ActiveBlockValue)];
				ComponentFirstPersonModel componentFirstPersonModel = componentMiner.Entity.FindComponent<ComponentFirstPersonModel>();
				if (componentFirstPersonModel != null)
				{
					ComponentPlayer componentPlayer = componentMiner.ComponentPlayer;
					if (componentPlayer != null)
					{
						componentPlayer.ComponentAimingSights.ShowAimingSights(aim.Position, aim.Direction);
					}
					componentFirstPersonModel.ItemOffsetOrder = new Vector3(0f, 0.35f, 0.17f);
					if (block2 is SpearBlock)
					{
						componentFirstPersonModel.ItemRotationOrder = new Vector3(-1.5f, 0f, 0f);
					}
				}
				if (block2 is SpearBlock)
				{
					componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemOffsetOrder = new Vector3(0f, -0.25f, 0f);
					componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemRotationOrder = new Vector3(3.14159f, 0f, 0f);
				}
			}
			return false;
		}

        // Token: 0x06000AAB RID: 2731 RVA: 0x0004F614 File Offset: 0x0004D814
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemProjectiles = base.Project.FindSubsystem<SubsystemProjectiles>(true);
			base.Load(valuesDictionary);
		}

		// Token: 0x040005D9 RID: 1497
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040005DA RID: 1498
		public SubsystemProjectiles m_subsystemProjectiles;

		// Token: 0x040005DB RID: 1499
		public Game.Random m_random = new Game.Random();
	}
}
