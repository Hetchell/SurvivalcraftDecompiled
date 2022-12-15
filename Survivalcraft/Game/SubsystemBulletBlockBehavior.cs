using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000167 RID: 359
	public class SubsystemBulletBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000717 RID: 1815 RVA: 0x0002D47B File Offset: 0x0002B67B
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x0002D484 File Offset: 0x0002B684
		public override bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem)
		{
			BulletBlock.BulletType bulletType = BulletBlock.GetBulletType(Terrain.ExtractData(worldItem.Value));
			bool result = true;
			if (cellFace != null)
			{
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(cellFace.Value.X, cellFace.Value.Y, cellFace.Value.Z);
				int num = Terrain.ExtractContents(cellValue);
				Block block = BlocksManager.Blocks[num];
				if (worldItem.Velocity.Length() > 30f)
				{
					this.m_subsystemExplosions.TryExplodeBlock(cellFace.Value.X, cellFace.Value.Y, cellFace.Value.Z, cellValue);
				}
				if (block.Density >= 1.5f && worldItem.Velocity.Length() > 30f)
				{
					float num2 = 1f;
					float minDistance = 8f;
					if (bulletType == BulletBlock.BulletType.BuckshotBall)
					{
						num2 = 0.25f;
						minDistance = 4f;
					}
					if (this.m_random.Float(0f, 1f) < num2)
					{
						this.m_subsystemAudio.PlayRandomSound("Audio/Ricochets", 1f, this.m_random.Float(-0.2f, 0.2f), new Vector3((float)cellFace.Value.X, (float)cellFace.Value.Y, (float)cellFace.Value.Z), minDistance, true);
						result = false;
					}
				}
			}
			return result;
		}

        // Token: 0x06000719 RID: 1817 RVA: 0x0002D5EF File Offset: 0x0002B7EF
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
		}

		// Token: 0x040003F0 RID: 1008
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040003F1 RID: 1009
		public SubsystemExplosions m_subsystemExplosions;

		// Token: 0x040003F2 RID: 1010
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040003F3 RID: 1011
		public Game.Random m_random = new Game.Random();
	}
}
