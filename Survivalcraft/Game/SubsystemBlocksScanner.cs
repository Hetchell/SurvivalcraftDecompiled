using System;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200015F RID: 351
	public class SubsystemBlocksScanner : Subsystem, IUpdateable
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060006DD RID: 1757 RVA: 0x0002B780 File Offset: 0x00029980
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.BlocksScanner;
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060006DE RID: 1758 RVA: 0x0002B784 File Offset: 0x00029984
		// (remove) Token: 0x060006DF RID: 1759 RVA: 0x0002B7BC File Offset: 0x000299BC
		public event Action<TerrainChunk> ScanningChunkCompleted;

		// Token: 0x060006E0 RID: 1760 RVA: 0x0002B7F4 File Offset: 0x000299F4
		public void Update(float dt)
		{
			Terrain terrain = this.m_subsystemTerrain.Terrain;
			this.m_pollCount += (float)(terrain.AllocatedChunks.Length * 16 * 16) * dt / 60f;
			this.m_pollCount = MathUtils.Clamp(this.m_pollCount, 0f, 200f);
			TerrainChunk nextChunk = terrain.GetNextChunk(this.m_pollChunkCoordinates.X, this.m_pollChunkCoordinates.Y);
			if (nextChunk == null)
			{
				return;
			}
			while (this.m_pollCount >= 1f)
			{
				if (nextChunk.State <= TerrainChunkState.InvalidContents4)
				{
					this.m_pollCount -= 65536f;
				}
				else
				{
					while (this.m_pollX < 16)
					{
						while (this.m_pollZ < 16)
						{
							if (this.m_pollCount < 1f)
							{
								return;
							}
							this.m_pollCount -= 1f;
							int topHeightFast = nextChunk.GetTopHeightFast(this.m_pollX, this.m_pollZ);
							int num = TerrainChunk.CalculateCellIndex(this.m_pollX, 0, this.m_pollZ);
							int i = 0;
							while (i <= topHeightFast)
							{
								int cellValueFast = nextChunk.GetCellValueFast(num);
								int num2 = Terrain.ExtractContents(cellValueFast);
								if (num2 != 0)
								{
									SubsystemPollableBlockBehavior[] array = this.m_pollableBehaviorsByContents[num2];
									for (int j = 0; j < array.Length; j++)
									{
										array[j].OnPoll(cellValueFast, nextChunk.Origin.X + this.m_pollX, i, nextChunk.Origin.Y + this.m_pollZ, this.m_pollPass);
									}
								}
								i++;
								num++;
							}
							this.m_pollZ++;
						}
						this.m_pollZ = 0;
						this.m_pollX++;
					}
					this.m_pollX = 0;
				}
				Action<TerrainChunk> scanningChunkCompleted = this.ScanningChunkCompleted;
				if (scanningChunkCompleted != null)
				{
					scanningChunkCompleted(nextChunk);
				}
				nextChunk = terrain.GetNextChunk(nextChunk.Coords.X + 1, nextChunk.Coords.Y);
				if (nextChunk == null)
				{
					break;
				}
				if (Terrain.ComparePoints(nextChunk.Coords, this.m_pollChunkCoordinates) < 0)
				{
					this.m_pollPass++;
				}
				this.m_pollChunkCoordinates = nextChunk.Coords;
			}
		}

        // Token: 0x060006E1 RID: 1761 RVA: 0x0002BA0C File Offset: 0x00029C0C
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.m_pollChunkCoordinates = valuesDictionary.GetValue<Point2>("PollChunkCoordinates");
			Point2 value = valuesDictionary.GetValue<Point2>("PollPoint");
			this.m_pollX = value.X;
			this.m_pollZ = value.Y;
			this.m_pollPass = valuesDictionary.GetValue<int>("PollPass");
			this.m_pollableBehaviorsByContents = new SubsystemPollableBlockBehavior[BlocksManager.Blocks.Length][];
			for (int i = 0; i < this.m_pollableBehaviorsByContents.Length; i++)
			{
				this.m_pollableBehaviorsByContents[i] = (from s in this.m_subsystemBlockBehaviors.GetBlockBehaviors(i)
				where s is SubsystemPollableBlockBehavior
				select (SubsystemPollableBlockBehavior)s).ToArray<SubsystemPollableBlockBehavior>();
			}
		}

        // Token: 0x060006E2 RID: 1762 RVA: 0x0002BB09 File Offset: 0x00029D09
        public override void Save(ValuesDictionary valuesDictionary)
		{
			valuesDictionary.SetValue<Point2>("PollChunkCoordinates", this.m_pollChunkCoordinates);
			valuesDictionary.SetValue<Point2>("PollPoint", new Point2(this.m_pollX, this.m_pollZ));
			valuesDictionary.SetValue<int>("PollPass", this.m_pollPass);
		}

		// Token: 0x040003CD RID: 973
		public const float ScanPeriod = 60f;

		// Token: 0x040003CE RID: 974
		public SubsystemPollableBlockBehavior[][] m_pollableBehaviorsByContents;

		// Token: 0x040003CF RID: 975
		public Point2 m_pollChunkCoordinates;

		// Token: 0x040003D0 RID: 976
		public int m_pollX;

		// Token: 0x040003D1 RID: 977
		public int m_pollZ;

		// Token: 0x040003D2 RID: 978
		public int m_pollPass;

		// Token: 0x040003D3 RID: 979
		public float m_pollCount;

		// Token: 0x040003D4 RID: 980
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040003D5 RID: 981
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;
	}
}
