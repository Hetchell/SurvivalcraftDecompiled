using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200016C RID: 364
	public class SubsystemCollapsingBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000741 RID: 1857 RVA: 0x0002E36C File Offset: 0x0002C56C
		public override int[] HandledBlocks
		{
			get
			{
				return SubsystemCollapsingBlockBehavior.m_handledBlocks;
			}
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x0002E373 File Offset: 0x0002C573
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			if (this.m_subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode == EnvironmentBehaviorMode.Living)
			{
				this.TryCollapseColumn(new Point3(x, y, z));
			}
		}

        // Token: 0x06000743 RID: 1859 RVA: 0x0002E398 File Offset: 0x0002C598
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemSoundMaterials = base.Project.FindSubsystem<SubsystemSoundMaterials>(true);
			this.m_subsystemMovingBlocks = base.Project.FindSubsystem<SubsystemMovingBlocks>(true);
			this.m_subsystemMovingBlocks.Stopped += this.MovingBlocksStopped;
			this.m_subsystemMovingBlocks.CollidedWithTerrain += this.MovingBlocksCollidedWithTerrain;
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x0002E410 File Offset: 0x0002C610
		public void MovingBlocksCollidedWithTerrain(IMovingBlockSet movingBlockSet, Point3 p)
		{
			if (movingBlockSet.Id == "CollapsingBlock")
			{
				int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(p.X, p.Y, p.Z);
				if (this.IsCollapseSupportBlock(cellValue))
				{
					movingBlockSet.Stop();
					return;
				}
				if (SubsystemCollapsingBlockBehavior.IsCollapseDestructibleBlock(cellValue))
				{
					base.SubsystemTerrain.DestroyCell(0, p.X, p.Y, p.Z, 0, false, false);
				}
			}
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x0002E48C File Offset: 0x0002C68C
		public void MovingBlocksStopped(IMovingBlockSet movingBlockSet)
		{
			if (movingBlockSet.Id == "CollapsingBlock")
			{
				Point3 p = Terrain.ToCell(MathUtils.Round(movingBlockSet.Position.X), MathUtils.Round(movingBlockSet.Position.Y), MathUtils.Round(movingBlockSet.Position.Z));
				foreach (MovingBlock movingBlock in movingBlockSet.Blocks)
				{
					Point3 point = p + movingBlock.Offset;
					base.SubsystemTerrain.DestroyCell(0, point.X, point.Y, point.Z, movingBlock.Value, false, false);
				}
				this.m_subsystemMovingBlocks.RemoveMovingBlockSet(movingBlockSet);
				if (movingBlockSet.Blocks.Count > 0)
				{
					this.m_subsystemSoundMaterials.PlayImpactSound(movingBlockSet.Blocks[0].Value, movingBlockSet.Position, 1f);
				}
			}
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x0002E5A4 File Offset: 0x0002C7A4
		public void TryCollapseColumn(Point3 p)
		{
			if (p.Y <= 0)
			{
				return;
			}
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(p.X, p.Y - 1, p.Z);
			if (this.IsCollapseSupportBlock(cellValue))
			{
				return;
			}
			List<MovingBlock> list = new List<MovingBlock>();
			for (int i = p.Y; i < 256; i++)
			{
				int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(p.X, i, p.Z);
				if (!SubsystemCollapsingBlockBehavior.IsCollapsibleBlock(cellValue2))
				{
					break;
				}
				list.Add(new MovingBlock
				{
					Value = cellValue2,
					Offset = new Point3(0, i - p.Y, 0)
				});
			}
			if (list.Count != 0 && this.m_subsystemMovingBlocks.AddMovingBlockSet(new Vector3(p), new Vector3((float)p.X, (float)(-(float)list.Count - 1), (float)p.Z), 0f, 10f, 0.7f, new Vector2(0f), list, "CollapsingBlock", null, true) != null)
			{
				foreach (MovingBlock movingBlock in list)
				{
					Point3 point = p + movingBlock.Offset;
					base.SubsystemTerrain.ChangeCell(point.X, point.Y, point.Z, 0, true);
				}
			}
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x0002E724 File Offset: 0x0002C924
		public static bool IsCollapsibleBlock(int value)
		{
			return SubsystemCollapsingBlockBehavior.m_handledBlocks.Contains(Terrain.ExtractContents(value));
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x0002E738 File Offset: 0x0002C938
		public bool IsCollapseSupportBlock(int value)
		{
			int num = Terrain.ExtractContents(value);
			if (num == 0)
			{
				return false;
			}
			int data = Terrain.ExtractData(value);
			Block block = BlocksManager.Blocks[num];
			if (block is TrapdoorBlock)
			{
				return TrapdoorBlock.GetUpsideDown(data) && !TrapdoorBlock.GetOpen(data);
			}
			return block.BlockIndex == 238 || !block.IsFaceTransparent(base.SubsystemTerrain, 4, value) || block is SoilBlock;
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x0002E7A8 File Offset: 0x0002C9A8
		public static bool IsCollapseDestructibleBlock(int value)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			if (block is TrapdoorBlock)
			{
				int data = Terrain.ExtractData(value);
				if (TrapdoorBlock.GetUpsideDown(data) && TrapdoorBlock.GetOpen(data))
				{
					return false;
				}
			}
			else if (block is FluidBlock)
			{
				return false;
			}
			return true;
		}

		// Token: 0x04000405 RID: 1029
		public const string IdString = "CollapsingBlock";

		// Token: 0x04000406 RID: 1030
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000407 RID: 1031
		public SubsystemSoundMaterials m_subsystemSoundMaterials;

		// Token: 0x04000408 RID: 1032
		public SubsystemMovingBlocks m_subsystemMovingBlocks;

		// Token: 0x04000409 RID: 1033
		public static int[] m_handledBlocks = new int[]
		{
			7,
			6
		};
	}
}
