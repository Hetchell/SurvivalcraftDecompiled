using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000198 RID: 408
	public class SubsystemPathfinding : Subsystem
	{
		// Token: 0x06000988 RID: 2440 RVA: 0x0004285C File Offset: 0x00040A5C
		public void QueuePathSearch(Vector3 start, Vector3 end, float minDistance, Vector3 boxSize, int maxPositionsToCheck, PathfindingResult result)
		{
			Queue<SubsystemPathfinding.Request> requests = this.m_requests;
			lock (requests)
			{
				if (this.m_requests.Count < 10)
				{
					result.IsCompleted = false;
					result.IsInProgress = true;
					this.m_requests.Enqueue(new SubsystemPathfinding.Request
					{
						Start = start,
						End = end,
						MinDistance = minDistance,
						BoxSize = boxSize,
						MaxPositionsToCheck = maxPositionsToCheck,
						PathfindingResult = result
					});
					Monitor.Pulse(this.m_requests);
				}
				else
				{
					result.IsCompleted = true;
					result.IsInProgress = false;
					result.Path.Clear();
					result.PathCost = 0f;
					result.PositionsChecked = 0;
				}
			}
		}

        // Token: 0x06000989 RID: 2441 RVA: 0x00042934 File Offset: 0x00040B34
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			SubsystemPathfinding.World world = new SubsystemPathfinding.World();
			world.SubsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_astar.OpenStorage = new SubsystemPathfinding.Storage();
			this.m_astar.ClosedStorage = new SubsystemPathfinding.Storage();
			this.m_astar.World = world;
			Task.Run(new Action(this.ThreadFunction));
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x000429AC File Offset: 0x00040BAC
		public override void Dispose()
		{
			Queue<SubsystemPathfinding.Request> requests = this.m_requests;
			lock (requests)
			{
				this.m_requests.Clear();
				this.m_requests.Enqueue(null);
				Monitor.Pulse(this.m_requests);
			}
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x00042A08 File Offset: 0x00040C08
		public void ThreadFunction()
		{
			for (;;)
			{
				Queue<SubsystemPathfinding.Request> requests = this.m_requests;
				SubsystemPathfinding.Request request;
				lock (requests)
				{
					while (this.m_requests.Count == 0)
					{
						Monitor.Wait(this.m_requests);
					}
					request = this.m_requests.Dequeue();
				}
				if (request == null)
				{
					break;
				}
				this.ProcessRequest(request);
				Task.Delay(250).Wait();
			}
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x00042A84 File Offset: 0x00040C84
		public void ProcessRequest(SubsystemPathfinding.Request request)
		{
			((SubsystemPathfinding.World)this.m_astar.World).Request = request;
			this.m_astar.Path = request.PathfindingResult.Path;
			double realTime = Time.RealTime;
			this.m_astar.FindPath(request.Start, request.End, request.MinDistance, request.MaxPositionsToCheck);
			double realTime2 = Time.RealTime;
			this.SmoothPath(this.m_astar.Path, request.BoxSize);
			double realTime3 = Time.RealTime;
			request.PathfindingResult.PathCost = this.m_astar.PathCost;
			request.PathfindingResult.PositionsChecked = ((SubsystemPathfinding.Storage)this.m_astar.ClosedStorage).Dictionary.Count;
			request.PathfindingResult.IsInProgress = false;
			request.PathfindingResult.IsCompleted = true;
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x00042B60 File Offset: 0x00040D60
		public void SmoothPath(DynamicArray<Vector3> path, Vector3 boxSize)
		{
			for (int i = path.Count - 2; i > 0; i--)
			{
				if (this.IsPassable(path.Array[i + 1], path.Array[i - 1], boxSize))
				{
					path.RemoveAt(i);
				}
			}
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x00042BAC File Offset: 0x00040DAC
		public bool IsPassable(Vector3 p1, Vector3 p2, Vector3 boxSize)
		{
			Vector3 vector = new Vector3(p1.X, p1.Y + 0.5f, p1.Z);
			Vector3 vector2 = new Vector3(p2.X, p2.Y + 0.5f, p2.Z);
			Vector3 v = (0.5f * boxSize.X + 0.1f) * Vector3.Normalize(Vector3.Cross(Vector3.UnitY, vector2 - vector));
			return this.m_subsystemTerrain.Raycast(vector, vector2, false, true, new Func<int, float, bool>(SubsystemPathfinding.SmoothingRaycastFunction_Obstacle)) == null && this.m_subsystemTerrain.Raycast(vector - v, vector2 - v, false, true, new Func<int, float, bool>(SubsystemPathfinding.SmoothingRaycastFunction_Obstacle)) == null && this.m_subsystemTerrain.Raycast(vector + v, vector2 + v, false, true, new Func<int, float, bool>(SubsystemPathfinding.SmoothingRaycastFunction_Obstacle)) == null && this.m_subsystemTerrain.Raycast(vector + new Vector3(0f, -1f, 0f), vector2 + new Vector3(0f, -1f, 0f), false, false, new Func<int, float, bool>(SubsystemPathfinding.SmoothingRaycastFunction_Support)) == null && this.m_subsystemTerrain.Raycast(vector + new Vector3(0f, -1f, 0f) - v, vector2 + new Vector3(0f, -1f, 0f) - v, false, false, new Func<int, float, bool>(SubsystemPathfinding.SmoothingRaycastFunction_Support)) == null && this.m_subsystemTerrain.Raycast(vector + new Vector3(0f, -1f, 0f) + v, vector2 + new Vector3(0f, -1f, 0f) + v, false, false, new Func<int, float, bool>(SubsystemPathfinding.SmoothingRaycastFunction_Support)) == null;
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x00042DD8 File Offset: 0x00040FD8
		public static bool SmoothingRaycastFunction_Obstacle(int value, float distance)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			return block.ShouldAvoid(value) || block.IsCollidable;
		}

		// Token: 0x06000990 RID: 2448 RVA: 0x00042E0C File Offset: 0x0004100C
		public static bool SmoothingRaycastFunction_Support(int value, float distance)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			return block.ShouldAvoid(value) || !block.IsCollidable;
		}

		// Token: 0x04000506 RID: 1286
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000507 RID: 1287
		public Queue<SubsystemPathfinding.Request> m_requests = new Queue<SubsystemPathfinding.Request>();

		// Token: 0x04000508 RID: 1288
		public AStar<Vector3> m_astar = new AStar<Vector3>();

		// Token: 0x02000431 RID: 1073
		public class Request
		{
			// Token: 0x040015C6 RID: 5574
			public Vector3 Start;

			// Token: 0x040015C7 RID: 5575
			public Vector3 End;

			// Token: 0x040015C8 RID: 5576
			public float MinDistance;

			// Token: 0x040015C9 RID: 5577
			public Vector3 BoxSize;

			// Token: 0x040015CA RID: 5578
			public int MaxPositionsToCheck;

			// Token: 0x040015CB RID: 5579
			public PathfindingResult PathfindingResult;
		}

		// Token: 0x02000432 RID: 1074
		public class Storage : IAStarStorage<Vector3>
		{
			// Token: 0x06001E86 RID: 7814 RVA: 0x000DF555 File Offset: 0x000DD755
			public void Clear()
			{
				this.Dictionary.Clear();
			}

			// Token: 0x06001E87 RID: 7815 RVA: 0x000DF564 File Offset: 0x000DD764
			public object Get(Vector3 p)
			{
				object result;
				this.Dictionary.TryGetValue(p, out result);
				return result;
			}

			// Token: 0x06001E88 RID: 7816 RVA: 0x000DF581 File Offset: 0x000DD781
			public void Set(Vector3 p, object data)
			{
				this.Dictionary[p] = data;
			}

			// Token: 0x040015CC RID: 5580
			public Dictionary<Vector3, object> Dictionary = new Dictionary<Vector3, object>();
		}

		// Token: 0x02000433 RID: 1075
		public class World : IAStarWorld<Vector3>
		{
			// Token: 0x06001E8A RID: 7818 RVA: 0x000DF5A3 File Offset: 0x000DD7A3
			public float Cost(Vector3 p1, Vector3 p2)
			{
				return 0.999f - 0.1f * Vector3.Dot(Vector3.Normalize(p2 - p1), Vector3.Normalize(this.Request.End - p1));
			}

			// Token: 0x06001E8B RID: 7819 RVA: 0x000DF5D8 File Offset: 0x000DD7D8
			public void Neighbors(Vector3 p, DynamicArray<Vector3> neighbors)
			{
				neighbors.Count = 0;
				this.AddNeighbor(neighbors, p, 1, 0);
				this.AddNeighbor(neighbors, p, -1, 0);
				this.AddNeighbor(neighbors, p, 0, -1);
				this.AddNeighbor(neighbors, p, 0, 1);
				this.AddNeighbor(neighbors, p, -1, -1);
				this.AddNeighbor(neighbors, p, 1, -1);
				this.AddNeighbor(neighbors, p, 1, 1);
				this.AddNeighbor(neighbors, p, -1, 1);
			}

			// Token: 0x06001E8C RID: 7820 RVA: 0x000DF63C File Offset: 0x000DD83C
			public float Heuristic(Vector3 p1, Vector3 p2)
			{
				float num = MathUtils.Abs(p1.X - p2.X);
				float num2 = MathUtils.Abs(p1.Z - p2.Z);
				if (num > num2)
				{
					return 1.41f * num2 + 1f * (num - num2);
				}
				return 1.41f * num + 1f * (num2 - num);
			}

			// Token: 0x06001E8D RID: 7821 RVA: 0x000DF696 File Offset: 0x000DD896
			public bool IsGoal(Vector3 p)
			{
				return Vector3.DistanceSquared(p, this.Request.End) <= this.Request.MinDistance * this.Request.MinDistance;
			}

			// Token: 0x06001E8E RID: 7822 RVA: 0x000DF6C8 File Offset: 0x000DD8C8
			public void AddNeighbor(DynamicArray<Vector3> neighbors, Vector3 p, int dx, int dz)
			{
				float y = p.Y;
				float num = p.Y;
				int num2 = Terrain.ToCell(p.X) + dx;
				int num3 = Terrain.ToCell(p.Y);
				int num4 = Terrain.ToCell(p.Z) + dz;
				int cellValue = this.SubsystemTerrain.Terrain.GetCellValue(num2, num3, num4);
				int num5 = Terrain.ExtractContents(cellValue);
				Block block = BlocksManager.Blocks[num5];
				if (block.ShouldAvoid(cellValue))
				{
					return;
				}
				if (block.IsCollidable)
				{
					float blockWalkingHeight = this.GetBlockWalkingHeight(block, cellValue);
					if (blockWalkingHeight > 0.5f && (block.NoAutoJump || block.NoSmoothRise))
					{
						return;
					}
					y = (float)num3 + blockWalkingHeight;
					num = (float)num3 + blockWalkingHeight;
				}
				else
				{
					bool flag = false;
					for (int i = -1; i >= -4; i--)
					{
						int cellValue2 = this.SubsystemTerrain.Terrain.GetCellValue(num2, num3 + i, num4);
						int num6 = Terrain.ExtractContents(cellValue2);
						Block block2 = BlocksManager.Blocks[num6];
						if (block2.ShouldAvoid(cellValue2))
						{
							return;
						}
						if (block2.IsCollidable)
						{
							y = (float)(num3 + i + 1);
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return;
					}
				}
				int num7 = (dx == 0 || dz == 0) ? 2 : 3;
				Vector3 vector = new Vector3(p.X, num + 0.01f, p.Z);
				Vector3 v = new Vector3((float)num2 + 0.5f, num + 0.01f, (float)num4 + 0.5f);
				Vector3 v2 = 1f / (float)num7 * (v - vector);
				for (int j = 1; j <= num7; j++)
				{
					Vector3 v3 = vector + (float)j * v2;
					BoundingBox box = new BoundingBox(v3 - new Vector3(this.Request.BoxSize.X / 2f + 0.01f, 0f, this.Request.BoxSize.Z / 2f + 0.01f), v3 + new Vector3(this.Request.BoxSize.X / 2f - 0.01f, this.Request.BoxSize.Y, this.Request.BoxSize.Z / 2f - 0.01f));
					if (this.IsBlocked(box))
					{
						return;
					}
				}
				neighbors.Add(new Vector3((float)num2 + 0.5f, y, (float)num4 + 0.5f));
			}

			// Token: 0x06001E8F RID: 7823 RVA: 0x000DF940 File Offset: 0x000DDB40
			public float GetBlockWalkingHeight(Block block, int value)
			{
				if (block is DoorBlock || block is FenceGateBlock)
				{
					return 0f;
				}
				float num = 0f;
				foreach (BoundingBox boundingBox in block.GetCustomCollisionBoxes(this.SubsystemTerrain, value))
				{
					num = MathUtils.Max(num, boundingBox.Max.Y);
				}
				return num;
			}

			// Token: 0x06001E90 RID: 7824 RVA: 0x000DF9A0 File Offset: 0x000DDBA0
			public bool IsBlocked(BoundingBox box)
			{
				int num = Terrain.ToCell(box.Min.X);
				int num2 = MathUtils.Max(Terrain.ToCell(box.Min.Y), 0);
				int num3 = Terrain.ToCell(box.Min.Z);
				int num4 = Terrain.ToCell(box.Max.X);
				int num5 = MathUtils.Min(Terrain.ToCell(box.Max.Y), 255);
				int num6 = Terrain.ToCell(box.Max.Z);
				for (int i = num; i <= num4; i++)
				{
					for (int j = num3; j <= num6; j++)
					{
						TerrainChunk chunkAtCell = this.SubsystemTerrain.Terrain.GetChunkAtCell(i, j);
						if (chunkAtCell != null)
						{
							int num7 = TerrainChunk.CalculateCellIndex(i & 15, num2, j & 15);
							int k = num2;
							while (k <= num5)
							{
								int cellValueFast = chunkAtCell.GetCellValueFast(num7);
								int num8 = Terrain.ExtractContents(cellValueFast);
								if (num8 != 0)
								{
									Block block = BlocksManager.Blocks[num8];
									if (block.ShouldAvoid(cellValueFast))
									{
										return true;
									}
									if (block.IsCollidable)
									{
										Vector3 v = new Vector3((float)i, (float)k, (float)j);
										foreach (BoundingBox boundingBox in block.GetCustomCollisionBoxes(this.SubsystemTerrain, cellValueFast))
										{
											if (box.Intersection(new BoundingBox(v + boundingBox.Min, v + boundingBox.Max)))
											{
												return true;
											}
										}
									}
								}
								k++;
								num7++;
							}
						}
					}
				}
				return false;
			}

			// Token: 0x040015CD RID: 5581
			public SubsystemTerrain SubsystemTerrain;

			// Token: 0x040015CE RID: 5582
			public SubsystemPathfinding.Request Request;
		}
	}
}
