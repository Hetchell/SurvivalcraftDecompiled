using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001B2 RID: 434
	public class SubsystemTorchBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000AC2 RID: 2754 RVA: 0x0004FB19 File Offset: 0x0004DD19
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					31,
					17,
					132
				};
			}
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x0004FB2C File Offset: 0x0004DD2C
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValueFast = base.SubsystemTerrain.Terrain.GetCellValueFast(x, y, z);
			int num = Terrain.ExtractContents(cellValueFast);
			if (num != 31)
			{
				if (num != 132)
				{
					return;
				}
				int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
				if (!BlocksManager.Blocks[cellContents].IsCollidable)
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
				}
			}
			else
			{
				Point3 point = CellFace.FaceToPoint3(Terrain.ExtractData(cellValueFast));
				int x2 = x - point.X;
				int y2 = y - point.Y;
				int z2 = z - point.Z;
				int cellContents2 = base.SubsystemTerrain.Terrain.GetCellContents(x2, y2, z2);
				if (!BlocksManager.Blocks[cellContents2].IsCollidable)
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
					return;
				}
			}
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x0004FBFC File Offset: 0x0004DDFC
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.AddTorch(value, x, y, z);
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x0004FC0A File Offset: 0x0004DE0A
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			this.RemoveTorch(x, y, z);
		}

		// Token: 0x06000AC6 RID: 2758 RVA: 0x0004FC17 File Offset: 0x0004DE17
		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			this.RemoveTorch(x, y, z);
			this.AddTorch(value, x, y, z);
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x0004FC30 File Offset: 0x0004DE30
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			this.AddTorch(value, x, y, z);
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x0004FC40 File Offset: 0x0004DE40
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			List<Point3> list = new List<Point3>();
			foreach (Point3 point in this.m_particleSystemsByCell.Keys)
			{
				if (point.X >= chunk.Origin.X && point.X < chunk.Origin.X + 16 && point.Z >= chunk.Origin.Y && point.Z < chunk.Origin.Y + 16)
				{
					list.Add(point);
				}
			}
			foreach (Point3 point2 in list)
			{
				this.RemoveTorch(point2.X, point2.Y, point2.Z);
			}
		}

        // Token: 0x06000AC9 RID: 2761 RVA: 0x0004FD44 File Offset: 0x0004DF44
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x0004FD60 File Offset: 0x0004DF60
		public void AddTorch(int value, int x, int y, int z)
		{
			int num = Terrain.ExtractContents(value);
			Vector3 v;
			float size;
			if (num != 31)
			{
				if (num != 132)
				{
					v = new Vector3(0.5f, 0.2f, 0.5f);
					size = 0.2f;
				}
				else
				{
					v = new Vector3(0.5f, 0.1f, 0.5f);
					size = 0.1f;
				}
			}
			else
			{
				switch (Terrain.ExtractData(value))
				{
				case 0:
					v = new Vector3(0.5f, 0.58f, 0.27f);
					break;
				case 1:
					v = new Vector3(0.27f, 0.58f, 0.5f);
					break;
				case 2:
					v = new Vector3(0.5f, 0.58f, 0.73f);
					break;
				case 3:
					v = new Vector3(0.73f, 0.58f, 0.5f);
					break;
				default:
					v = new Vector3(0.5f, 0.53f, 0.5f);
					break;
				}
				size = 0.15f;
			}
			FireParticleSystem fireParticleSystem = new FireParticleSystem(new Vector3((float)x, (float)y, (float)z) + v, size, 24f);
			this.m_subsystemParticles.AddParticleSystem(fireParticleSystem);
			this.m_particleSystemsByCell[new Point3(x, y, z)] = fireParticleSystem;
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x0004FEA4 File Offset: 0x0004E0A4
		public void RemoveTorch(int x, int y, int z)
		{
			Point3 key = new Point3(x, y, z);
			FireParticleSystem particleSystem = this.m_particleSystemsByCell[key];
			this.m_subsystemParticles.RemoveParticleSystem(particleSystem);
			this.m_particleSystemsByCell.Remove(key);
		}

		// Token: 0x040005E9 RID: 1513
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x040005EA RID: 1514
		public Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();
	}
}
