using System;
using Engine;

namespace Game
{
	// Token: 0x0200021A RID: 538
	public class AStar<T>
	{
		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06001085 RID: 4229 RVA: 0x0007E0BC File Offset: 0x0007C2BC
		// (set) Token: 0x06001086 RID: 4230 RVA: 0x0007E0C4 File Offset: 0x0007C2C4
		public float PathCost { get; set; }

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x06001087 RID: 4231 RVA: 0x0007E0CD File Offset: 0x0007C2CD
		// (set) Token: 0x06001088 RID: 4232 RVA: 0x0007E0D5 File Offset: 0x0007C2D5
		public DynamicArray<T> Path { get; set; }

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06001089 RID: 4233 RVA: 0x0007E0DE File Offset: 0x0007C2DE
		// (set) Token: 0x0600108A RID: 4234 RVA: 0x0007E0E6 File Offset: 0x0007C2E6
		public IAStarWorld<T> World { get; set; }

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x0600108B RID: 4235 RVA: 0x0007E0EF File Offset: 0x0007C2EF
		// (set) Token: 0x0600108C RID: 4236 RVA: 0x0007E0F7 File Offset: 0x0007C2F7
		public IAStarStorage<T> OpenStorage { get; set; }

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x0600108D RID: 4237 RVA: 0x0007E100 File Offset: 0x0007C300
		// (set) Token: 0x0600108E RID: 4238 RVA: 0x0007E108 File Offset: 0x0007C308
		public IAStarStorage<T> ClosedStorage { get; set; }

		// Token: 0x0600108F RID: 4239 RVA: 0x0007E114 File Offset: 0x0007C314
		public void BuildPathFromEndNode(AStar<T>.Node startNode, AStar<T>.Node endNode)
		{
			this.PathCost = endNode.G;
			this.Path.Clear();
			for (AStar<T>.Node node = endNode; node != startNode; node = (AStar<T>.Node)this.ClosedStorage.Get(node.PreviousPosition))
			{
				this.Path.Add(node.Position);
			}
		}

		// Token: 0x06001090 RID: 4240 RVA: 0x0007E168 File Offset: 0x0007C368
		public void FindPath(T start, T end, float minHeuristic, int maxPositionsToCheck)
		{
			if (this.Path == null)
			{
				throw new InvalidOperationException("Path not specified.");
			}
			if (this.World == null)
			{
				throw new InvalidOperationException("AStar World not specified.");
			}
			if (this.OpenStorage == null)
			{
				throw new InvalidOperationException("AStar OpenStorage not specified.");
			}
			if (this.OpenStorage == null)
			{
				throw new InvalidOperationException("AStar ClosedStorage not specified.");
			}
			this.m_nodesCacheIndex = 0;
			this.m_openHeap.Clear();
			this.OpenStorage.Clear();
			this.ClosedStorage.Clear();
			AStar<T>.Node node = this.NewNode(start, default(T), 0f, 0f);
			this.OpenStorage.Set(start, node);
			this.HeapEnqueue(node);
			AStar<T>.Node node2 = null;
			int num = 0;
			AStar<T>.Node node3;
			for (;;)
			{
				node3 = ((this.m_openHeap.Count > 0) ? this.HeapDequeue() : null);
				if (node3 == null || num >= maxPositionsToCheck)
				{
					break;
				}
				if (this.World.IsGoal(node3.Position))
				{
					goto IL_264;
				}
				this.ClosedStorage.Set(node3.Position, node3);
				this.OpenStorage.Set(node3.Position, null);
				num++;
				this.m_neighbors.Clear();
				this.World.Neighbors(node3.Position, this.m_neighbors);
				for (int i = 0; i < this.m_neighbors.Count; i++)
				{
					T t = this.m_neighbors.Array[i];
					if (this.ClosedStorage.Get(t) == null)
					{
						float num2 = this.World.Cost(node3.Position, t);
						if (num2 != float.PositiveInfinity)
						{
							float num3 = node3.G + num2;
							float num4 = this.World.Heuristic(t, end);
							if (node3 != node && (node2 == null || num4 < node2.H))
							{
								node2 = node3;
							}
							AStar<T>.Node node4 = (AStar<T>.Node)this.OpenStorage.Get(t);
							if (node4 != null)
							{
								if (num3 < node4.G)
								{
									node4.G = num3;
									node4.F = num3 + node4.H;
									node4.PreviousPosition = node3.Position;
									this.HeapUpdate(node4);
								}
							}
							else
							{
								node4 = this.NewNode(t, node3.Position, num3, num4);
								this.OpenStorage.Set(t, node4);
								this.HeapEnqueue(node4);
							}
						}
					}
				}
			}
			if (node2 != null)
			{
				this.BuildPathFromEndNode(node, node2);
				return;
			}
			this.Path.Clear();
			this.PathCost = 0f;
			return;
			IL_264:
			this.BuildPathFromEndNode(node, node3);
		}

		// Token: 0x06001091 RID: 4241 RVA: 0x0007E3E1 File Offset: 0x0007C5E1
		public void HeapEnqueue(AStar<T>.Node node)
		{
			this.m_openHeap.Add(node);
			this.HeapifyFromPosToStart(this.m_openHeap.Count - 1);
		}

		// Token: 0x06001092 RID: 4242 RVA: 0x0007E404 File Offset: 0x0007C604
		public AStar<T>.Node HeapDequeue()
		{
			AStar<T>.Node result = this.m_openHeap.Array[0];
			if (this.m_openHeap.Count <= 1)
			{
				this.m_openHeap.Clear();
				return result;
			}
			this.m_openHeap.Array[0] = this.m_openHeap.Array[this.m_openHeap.Count - 1];
			DynamicArray<AStar<T>.Node> openHeap = this.m_openHeap;
			int count = openHeap.Count - 1;
			openHeap.Count = count;
			this.HeapifyFromPosToEnd(0);
			return result;
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x0007E480 File Offset: 0x0007C680
		public void HeapUpdate(AStar<T>.Node node)
		{
			int pos = -1;
			for (int i = 0; i < this.m_openHeap.Count; i++)
			{
				if (this.m_openHeap.Array[i] == node)
				{
					pos = i;
					break;
				}
			}
			this.HeapifyFromPosToStart(pos);
		}

		// Token: 0x06001094 RID: 4244 RVA: 0x0007E4C0 File Offset: 0x0007C6C0
		public void HeapifyFromPosToEnd(int pos)
		{
			for (;;)
			{
				int num = pos;
				int num2 = 2 * pos + 1;
				int num3 = 2 * pos + 2;
				if (num2 < this.m_openHeap.Count && this.m_openHeap.Array[num2].F < this.m_openHeap.Array[num].F)
				{
					num = num2;
				}
				if (num3 < this.m_openHeap.Count && this.m_openHeap.Array[num3].F < this.m_openHeap.Array[num].F)
				{
					num = num3;
				}
				if (num == pos)
				{
					break;
				}
				AStar<T>.Node node = this.m_openHeap.Array[num];
				this.m_openHeap.Array[num] = this.m_openHeap.Array[pos];
				this.m_openHeap.Array[pos] = node;
				pos = num;
			}
		}

		// Token: 0x06001095 RID: 4245 RVA: 0x0007E58C File Offset: 0x0007C78C
		public void HeapifyFromPosToStart(int pos)
		{
			int num;
			for (int i = pos; i > 0; i = num)
			{
				num = (i - 1) / 2;
				AStar<T>.Node node = this.m_openHeap.Array[num];
				AStar<T>.Node node2 = this.m_openHeap.Array[i];
				if (node.F <= node2.F)
				{
					break;
				}
				this.m_openHeap.Array[num] = node2;
				this.m_openHeap.Array[i] = node;
			}
		}

		// Token: 0x06001096 RID: 4246 RVA: 0x0007E5F0 File Offset: 0x0007C7F0
		public AStar<T>.Node NewNode(T position, T previousPosition, float g, float h)
		{
			while (this.m_nodesCacheIndex >= this.m_nodesCache.Count)
			{
				this.m_nodesCache.Add(new AStar<T>.Node());
			}
			AStar<T>.Node[] array = this.m_nodesCache.Array;
			int nodesCacheIndex = this.m_nodesCacheIndex;
			this.m_nodesCacheIndex = nodesCacheIndex + 1;
			Node obj = array[nodesCacheIndex];
			obj.Position = position;
			obj.PreviousPosition = previousPosition;
			obj.F = g + h;
			obj.G = g;
			obj.H = h;
			return obj;
		}

		// Token: 0x04000ACE RID: 2766
		public int m_nodesCacheIndex;

		// Token: 0x04000ACF RID: 2767
		public DynamicArray<AStar<T>.Node> m_nodesCache = new DynamicArray<AStar<T>.Node>();

		// Token: 0x04000AD0 RID: 2768
		public DynamicArray<AStar<T>.Node> m_openHeap = new DynamicArray<AStar<T>.Node>();

		// Token: 0x04000AD1 RID: 2769
		public DynamicArray<T> m_neighbors = new DynamicArray<T>();

		// Token: 0x0200046D RID: 1133
		public class Node
		{
			// Token: 0x0400167B RID: 5755
			public T Position;

			// Token: 0x0400167C RID: 5756
			public T PreviousPosition;

			// Token: 0x0400167D RID: 5757
			public float F;

			// Token: 0x0400167E RID: 5758
			public float G;

			// Token: 0x0400167F RID: 5759
			public float H;
		}
	}
}
