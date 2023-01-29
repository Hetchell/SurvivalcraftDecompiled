using System;
using Engine;

namespace Game
{
	// Token: 0x0200030C RID: 780
	public class TerrainChunk : IDisposable
	{
		// Token: 0x0600162C RID: 5676 RVA: 0x000A7698 File Offset: 0x000A5898
		public TerrainChunk(Terrain terrain, int x, int z)
		{
			this.Terrain = terrain;
			this.Coords = new Point2(x, z);
			this.Origin = new Point2(x * 16, z * 16);
			this.BoundingBox = new BoundingBox(new Vector3((float)this.Origin.X, 0f, (float)this.Origin.Y), new Vector3((float)(this.Origin.X + 16), 256f, (float)(this.Origin.Y + 16)));
			this.Center = new Vector2((float)this.Origin.X + 8f, (float)this.Origin.Y + 8f);
		}

		// Token: 0x0600162D RID: 5677 RVA: 0x000A779A File Offset: 0x000A599A
		public void Dispose()
		{
			this.Geometry.Dispose();
		}

		// Token: 0x0600162E RID: 5678 RVA: 0x000A77A7 File Offset: 0x000A59A7
		public static bool IsCellValid(int x, int y, int z)
		{
			return x >= 0 && x < 16 && y >= 0 && y < 256 && z >= 0 && z < 16;
		}

		// Token: 0x0600162F RID: 5679 RVA: 0x000A77C9 File Offset: 0x000A59C9
		public static bool IsShaftValid(int x, int z)
		{
			return x >= 0 && x < 16 && z >= 0 && z < 16;
		}

		// Token: 0x06001630 RID: 5680 RVA: 0x000A77DF File Offset: 0x000A59DF
		public static int CalculateCellIndex(int x, int y, int z)
		{
			return y + x * 256 + z * 256 * 16;
		}

		// Token: 0x06001631 RID: 5681 RVA: 0x000A77F8 File Offset: 0x000A59F8
		public int CalculateTopmostCellHeight(int x, int z)
		{
			int num = TerrainChunk.CalculateCellIndex(x, 255, z);
			int i = 255;
			while (i >= 0)
			{
				if (Terrain.ExtractContents(this.GetCellValueFast(num)) != 0)
				{
					return i;
				}
				i--;
				num--;
			}
			return 0;
		}

		// Token: 0x06001632 RID: 5682 RVA: 0x000A7837 File Offset: 0x000A5A37
		public int GetCellValueFast(int index)
		{
			return this.Cells[index];
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x000A7841 File Offset: 0x000A5A41
		public int GetCellValueFast(int x, int y, int z)
		{
			return this.Cells[y + x * 256 + z * 256 * 16];
		}

		// Token: 0x06001634 RID: 5684 RVA: 0x000A785E File Offset: 0x000A5A5E
		public void SetCellValueFast(int x, int y, int z, int value)
		{
			//this.Cells[getIndex(x, y, z)] = value;
			this.Cells[y + x * 256 + z * 256 * 16] = value;
		}

		private static int getIndex(int x, int y, int z)
		{
			return x << 12 | z << 8 | y;
		}

		// Token: 0x06001635 RID: 5685 RVA: 0x000A787D File Offset: 0x000A5A7D
		public void SetCellValueFast(int index, int value)
		{
			this.Cells[index] = value;
		}

		// Token: 0x06001636 RID: 5686 RVA: 0x000A7888 File Offset: 0x000A5A88
		public int GetCellContentsFast(int x, int y, int z)
		{
			return Terrain.ExtractContents(this.GetCellValueFast(x, y, z));
		}

		// Token: 0x06001637 RID: 5687 RVA: 0x000A7898 File Offset: 0x000A5A98
		public int GetCellLightFast(int x, int y, int z)
		{
			return Terrain.ExtractLight(this.GetCellValueFast(x, y, z));
		}

		// Token: 0x06001638 RID: 5688 RVA: 0x000A78A8 File Offset: 0x000A5AA8
		public int GetShaftValueFast(int x, int z)
		{
			return this.Shafts[x + z * 16];
		}

		// Token: 0x06001639 RID: 5689 RVA: 0x000A78B7 File Offset: 0x000A5AB7
		public void SetShaftValueFast(int x, int z, int value)
		{
			this.Shafts[x + z * 16] = value;
		}

		// Token: 0x0600163A RID: 5690 RVA: 0x000A78C7 File Offset: 0x000A5AC7
		public int GetTemperatureFast(int x, int z)
		{
			return Terrain.ExtractTemperature(this.GetShaftValueFast(x, z));
		}

		// Token: 0x0600163B RID: 5691 RVA: 0x000A78D6 File Offset: 0x000A5AD6
		public void SetTemperatureFast(int x, int z, int temperature)
		{
			this.SetShaftValueFast(x, z, Terrain.ReplaceTemperature(this.GetShaftValueFast(x, z), temperature));
		}

		// Token: 0x0600163C RID: 5692 RVA: 0x000A78EE File Offset: 0x000A5AEE
		public int GetHumidityFast(int x, int z)
		{
			return Terrain.ExtractHumidity(this.GetShaftValueFast(x, z));
		}

		// Token: 0x0600163D RID: 5693 RVA: 0x000A78FD File Offset: 0x000A5AFD
		public void SetHumidityFast(int x, int z, int humidity)
		{
			this.SetShaftValueFast(x, z, Terrain.ReplaceHumidity(this.GetShaftValueFast(x, z), humidity));
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x000A7915 File Offset: 0x000A5B15
		public int GetTopHeightFast(int x, int z)
		{
			return Terrain.ExtractTopHeight(this.GetShaftValueFast(x, z));
		}

		// Token: 0x0600163F RID: 5695 RVA: 0x000A7924 File Offset: 0x000A5B24
		public void SetTopHeightFast(int x, int z, int topHeight)
		{
			this.SetShaftValueFast(x, z, Terrain.ReplaceTopHeight(this.GetShaftValueFast(x, z), topHeight));
		}

		// Token: 0x06001640 RID: 5696 RVA: 0x000A793C File Offset: 0x000A5B3C
		public int GetBottomHeightFast(int x, int z)
		{
			return Terrain.ExtractBottomHeight(this.GetShaftValueFast(x, z));
		}

		// Token: 0x06001641 RID: 5697 RVA: 0x000A794B File Offset: 0x000A5B4B
		public void SetBottomHeightFast(int x, int z, int bottomHeight)
		{
			this.SetShaftValueFast(x, z, Terrain.ReplaceBottomHeight(this.GetShaftValueFast(x, z), bottomHeight));
		}

		// Token: 0x06001642 RID: 5698 RVA: 0x000A7963 File Offset: 0x000A5B63
		public int GetSunlightHeightFast(int x, int z)
		{
			return Terrain.ExtractSunlightHeight(this.GetShaftValueFast(x, z));
		}

		// Token: 0x06001643 RID: 5699 RVA: 0x000A7972 File Offset: 0x000A5B72
		public void SetSunlightHeightFast(int x, int z, int sunlightHeight)
		{
			this.SetShaftValueFast(x, z, Terrain.ReplaceSunlightHeight(this.GetShaftValueFast(x, z), sunlightHeight));
		}

		// Token: 0x04000F9C RID: 3996
		public const int SizeBits = 4;

		// Token: 0x04000F9D RID: 3997
		public const int Size = 16;

		// Token: 0x04000F9E RID: 3998
		public const int HeightBits = 8;

		// Token: 0x04000F9F RID: 3999
		public const int Height = 256;

		// Token: 0x04000FA0 RID: 4000
		public const int SizeMinusOne = 15;

		// Token: 0x04000FA1 RID: 4001
		public const int HeightMinusOne = 255;

		// Token: 0x04000FA2 RID: 4002
		public const int SliceHeight = 16;

		// Token: 0x04000FA3 RID: 4003
		public const int SlicesCount = 16;

		// Token: 0x04000FA4 RID: 4004
		public Terrain Terrain;

		// Token: 0x04000FA5 RID: 4005
		public Point2 Coords;

		// Token: 0x04000FA6 RID: 4006
		public Point2 Origin;

		// Token: 0x04000FA7 RID: 4007
		public BoundingBox BoundingBox;

		// Token: 0x04000FA8 RID: 4008
		public Vector2 Center;

		// Token: 0x04000FA9 RID: 4009
		public TerrainChunkState State;

		// Token: 0x04000FAA RID: 4010
		public TerrainChunkState ThreadState;

		// Token: 0x04000FAB RID: 4011
		public bool WasDowngraded;

		// Token: 0x04000FAC RID: 4012
		public TerrainChunkState? DowngradedState;

		// Token: 0x04000FAD RID: 4013
		public bool WasUpgraded;

		// Token: 0x04000FAE RID: 4014
		public TerrainChunkState? UpgradedState;

		// Token: 0x04000FAF RID: 4015
		public float DrawDistanceSquared;

		// Token: 0x04000FB0 RID: 4016
		public int LightPropagationMask;

		// Token: 0x04000FB1 RID: 4017
		public int ModificationCounter;

		// Token: 0x04000FB2 RID: 4018
		public float[] FogEnds = new float[4];

		// Token: 0x04000FB3 RID: 4019
		public int[] SliceContentsHashes = new int[16];

		// Token: 0x04000FB4 RID: 4020
		public bool AreBehaviorsNotified;

		// Token: 0x04000FB5 RID: 4021
		public bool IsLoaded;

		// Token: 0x04000FB6 RID: 4022
		public volatile bool NewGeometryData;

		// Token: 0x04000FB7 RID: 4023
		public TerrainChunkGeometry Geometry = new TerrainChunkGeometry();

		// Token: 0x04000FB8 RID: 4024
		public int[] Cells = new int[65536];

		// Token: 0x04000FB9 RID: 4025
		public int[] Shafts = new int[256];
	}
}
