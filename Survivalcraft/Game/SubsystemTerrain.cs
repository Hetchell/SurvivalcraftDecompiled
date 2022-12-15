using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001AE RID: 430
	public class SubsystemTerrain : Subsystem, IDrawable, IUpdateable
	{
		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000A88 RID: 2696 RVA: 0x0004E698 File Offset: 0x0004C898
		// (set) Token: 0x06000A89 RID: 2697 RVA: 0x0004E6A0 File Offset: 0x0004C8A0
		public SubsystemGameInfo SubsystemGameInfo { get; set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000A8A RID: 2698 RVA: 0x0004E6A9 File Offset: 0x0004C8A9
		// (set) Token: 0x06000A8B RID: 2699 RVA: 0x0004E6B1 File Offset: 0x0004C8B1
		public SubsystemAnimatedTextures SubsystemAnimatedTextures { get; set; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000A8C RID: 2700 RVA: 0x0004E6BA File Offset: 0x0004C8BA
		// (set) Token: 0x06000A8D RID: 2701 RVA: 0x0004E6C2 File Offset: 0x0004C8C2
		public SubsystemFurnitureBlockBehavior SubsystemFurnitureBlockBehavior { get; set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000A8E RID: 2702 RVA: 0x0004E6CB File Offset: 0x0004C8CB
		// (set) Token: 0x06000A8F RID: 2703 RVA: 0x0004E6D3 File Offset: 0x0004C8D3
		public SubsystemPalette SubsystemPalette { get; set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000A90 RID: 2704 RVA: 0x0004E6DC File Offset: 0x0004C8DC
		// (set) Token: 0x06000A91 RID: 2705 RVA: 0x0004E6E4 File Offset: 0x0004C8E4
		public Terrain Terrain { get; set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000A92 RID: 2706 RVA: 0x0004E6ED File Offset: 0x0004C8ED
		// (set) Token: 0x06000A93 RID: 2707 RVA: 0x0004E6F5 File Offset: 0x0004C8F5
		public TerrainUpdater TerrainUpdater { get; set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000A94 RID: 2708 RVA: 0x0004E6FE File Offset: 0x0004C8FE
		// (set) Token: 0x06000A95 RID: 2709 RVA: 0x0004E706 File Offset: 0x0004C906
		public TerrainRenderer TerrainRenderer { get; set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000A96 RID: 2710 RVA: 0x0004E70F File Offset: 0x0004C90F
		// (set) Token: 0x06000A97 RID: 2711 RVA: 0x0004E717 File Offset: 0x0004C917
		public TerrainSerializer22 TerrainSerializer { get; set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000A98 RID: 2712 RVA: 0x0004E720 File Offset: 0x0004C920
		// (set) Token: 0x06000A99 RID: 2713 RVA: 0x0004E728 File Offset: 0x0004C928
		public ITerrainContentsGenerator TerrainContentsGenerator { get; set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000A9A RID: 2714 RVA: 0x0004E731 File Offset: 0x0004C931
		// (set) Token: 0x06000A9B RID: 2715 RVA: 0x0004E739 File Offset: 0x0004C939
		public BlockGeometryGenerator BlockGeometryGenerator { get; set; }

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000A9C RID: 2716 RVA: 0x0004E742 File Offset: 0x0004C942
		public int[] DrawOrders
		{
			get
			{
				return SubsystemTerrain.m_drawOrders;
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000A9D RID: 2717 RVA: 0x0004E749 File Offset: 0x0004C949
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Terrain;
			}
		}

		// Token: 0x06000A9E RID: 2718 RVA: 0x0004E750 File Offset: 0x0004C950
		public void ProcessModifiedCells()
		{
			this.m_modifiedList.Clear();
			foreach (Point3 item in this.m_modifiedCells.Keys)
			{
				this.m_modifiedList.Add(item);
			}
			this.m_modifiedCells.Clear();
			for (int i = 0; i < this.m_modifiedList.Count; i++)
			{
				Point3 point = this.m_modifiedList.Array[i];
				for (int j = 0; j < SubsystemTerrain.m_neighborOffsets.Length; j++)
				{
					Point3 point2 = SubsystemTerrain.m_neighborOffsets[j];
					int cellContents = this.Terrain.GetCellContents(point.X + point2.X, point.Y + point2.Y, point.Z + point2.Z);
					SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(cellContents);
					for (int k = 0; k < blockBehaviors.Length; k++)
					{
						blockBehaviors[k].OnNeighborBlockChanged(point.X + point2.X, point.Y + point2.Y, point.Z + point2.Z, point.X, point.Y, point.Z);
					}
				}
			}
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x0004E8BC File Offset: 0x0004CABC
		public TerrainRaycastResult? Raycast(Vector3 start, Vector3 end, bool useInteractionBoxes, bool skipAirBlocks, Func<int, float, bool> action)
		{
			float num = Vector3.Distance(start, end);
			if (num > 1000f)
			{
				Log.Warning("Terrain raycast too long, trimming.");
				end = start + 1000f * Vector3.Normalize(end - start);
			}
			Ray3 ray = new Ray3(start, Vector3.Normalize(end - start));
			float x = start.X;
			float y = start.Y;
			float z = start.Z;
			float x2 = end.X;
			float y2 = end.Y;
			float z2 = end.Z;
			int num2 = Terrain.ToCell(x);
			int num3 = Terrain.ToCell(y);
			int num4 = Terrain.ToCell(z);
			int num5 = Terrain.ToCell(x2);
			int num6 = Terrain.ToCell(y2);
			int num7 = Terrain.ToCell(z2);
			int num8 = (x < x2) ? 1 : ((x > x2) ? -1 : 0);
			int num9 = (y < y2) ? 1 : ((y > y2) ? -1 : 0);
			int num10 = (z < z2) ? 1 : ((z > z2) ? -1 : 0);
			float num11 = MathUtils.Floor(x);
			float num12 = num11 + 1f;
			float num13 = ((x > x2) ? (x - num11) : (num12 - x)) / Math.Abs(x2 - x);
			float num14 = MathUtils.Floor(y);
			float num15 = num14 + 1f;
			float num16 = ((y > y2) ? (y - num14) : (num15 - y)) / Math.Abs(y2 - y);
			float num17 = MathUtils.Floor(z);
			float num18 = num17 + 1f;
			float num19 = ((z > z2) ? (z - num17) : (num18 - z)) / Math.Abs(z2 - z);
			float num20 = 1f / Math.Abs(x2 - x);
			float num21 = 1f / Math.Abs(y2 - y);
			float num22 = 1f / Math.Abs(z2 - z);
			BoundingBox boundingBox;
			int collisionBoxIndex;
			float? num23;
			int cellValue;
			for (;;)
			{
				boundingBox = default(BoundingBox);
				collisionBoxIndex = 0;
				num23 = null;
				cellValue = this.Terrain.GetCellValue(num2, num3, num4);
				int num24 = Terrain.ExtractContents(cellValue);
				if (num24 != 0 || !skipAirBlocks)
				{
					Ray3 ray2 = new Ray3(ray.Position - new Vector3((float)num2, (float)num3, (float)num4), ray.Direction);
					int num26;
					BoundingBox boundingBox2;
					float? num25 = BlocksManager.Blocks[num24].Raycast(ray2, this, cellValue, useInteractionBoxes, out num26, out boundingBox2);
					if (num25 != null && (num23 == null || num25.Value < num23.Value))
					{
						num23 = num25;
						collisionBoxIndex = num26;
						boundingBox = boundingBox2;
					}
				}
				if (num23 != null && num23.Value <= num && (action == null || action(cellValue, num23.Value)))
				{
					break;
				}
				if (num13 <= num16 && num13 <= num19)
				{
					if (num2 == num5)
					{
						goto IL_46D;
					}
					num13 += num20;
					num2 += num8;
				}
				else if (num16 <= num13 && num16 <= num19)
				{
					if (num3 == num6)
					{
						goto IL_46D;
					}
					num16 += num21;
					num3 += num9;
				}
				else
				{
					if (num4 == num7)
					{
						goto IL_46D;
					}
					num19 += num22;
					num4 += num10;
				}
			}
			int face = 0;
			Vector3 vector = start - new Vector3((float)num2, (float)num3, (float)num4) + num23.Value * ray.Direction;
			float num27 = float.MaxValue;
			float num28 = MathUtils.Abs(vector.X - boundingBox.Min.X);
			if (num28 < num27)
			{
				num27 = num28;
				face = 3;
			}
			num28 = MathUtils.Abs(vector.X - boundingBox.Max.X);
			if (num28 < num27)
			{
				num27 = num28;
				face = 1;
			}
			num28 = MathUtils.Abs(vector.Y - boundingBox.Min.Y);
			if (num28 < num27)
			{
				num27 = num28;
				face = 5;
			}
			num28 = MathUtils.Abs(vector.Y - boundingBox.Max.Y);
			if (num28 < num27)
			{
				num27 = num28;
				face = 4;
			}
			num28 = MathUtils.Abs(vector.Z - boundingBox.Min.Z);
			if (num28 < num27)
			{
				num27 = num28;
				face = 2;
			}
			num28 = MathUtils.Abs(vector.Z - boundingBox.Max.Z);
			if (num28 < num27)
			{
				face = 0;
			}
			return new TerrainRaycastResult?(new TerrainRaycastResult
			{
				Ray = ray,
				Value = cellValue,
				CellFace = new CellFace
				{
					X = num2,
					Y = num3,
					Z = num4,
					Face = face
				},
				CollisionBoxIndex = collisionBoxIndex,
				Distance = num23.Value
			});
			IL_46D:
			return null;
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x0004ED40 File Offset: 0x0004CF40
		public void ChangeCell(int x, int y, int z, int value, bool updateModificationCounter = true)
		{
			if (!this.Terrain.IsCellValid(x, y, z))
			{
				return;
			}
			int num = this.Terrain.GetCellValueFast(x, y, z);
			value = Terrain.ReplaceLight(value, 0);
			num = Terrain.ReplaceLight(num, 0);
			if (value == num)
			{
				return;
			}
			this.Terrain.SetCellValueFast(x, y, z, value);
			TerrainChunk chunkAtCell = this.Terrain.GetChunkAtCell(x, z);
			if (chunkAtCell != null)
			{
				if (updateModificationCounter)
				{
					chunkAtCell.ModificationCounter++;
				}
				this.TerrainUpdater.DowngradeChunkNeighborhoodState(chunkAtCell.Coords, 1, TerrainChunkState.InvalidLight, false);
			}
			this.m_modifiedCells[new Point3(x, y, z)] = true;
			int num2 = Terrain.ExtractContents(num);
			int num3 = Terrain.ExtractContents(value);
			if (num3 != num2)
			{
				SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(num2);
				for (int i = 0; i < blockBehaviors.Length; i++)
				{
					blockBehaviors[i].OnBlockRemoved(num, value, x, y, z);
				}
				SubsystemBlockBehavior[] blockBehaviors2 = this.m_subsystemBlockBehaviors.GetBlockBehaviors(num3);
				for (int j = 0; j < blockBehaviors2.Length; j++)
				{
					blockBehaviors2[j].OnBlockAdded(value, num, x, y, z);
				}
				return;
			}
			SubsystemBlockBehavior[] blockBehaviors3 = this.m_subsystemBlockBehaviors.GetBlockBehaviors(num3);
			for (int k = 0; k < blockBehaviors3.Length; k++)
			{
				blockBehaviors3[k].OnBlockModified(value, num, x, y, z);
			}
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x0004EE88 File Offset: 0x0004D088
		public void DestroyCell(int toolLevel, int x, int y, int z, int newValue, bool noDrop, bool noParticleSystem)
		{
			int cellValue = this.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			Block block = BlocksManager.Blocks[num];
			if (num != 0)
			{
				bool flag = true;
				if (!noDrop)
				{
					this.m_dropValues.Clear();
					block.GetDropValues(this, cellValue, newValue, toolLevel, this.m_dropValues, out flag);
					for (int i = 0; i < this.m_dropValues.Count; i++)
					{
						BlockDropValue blockDropValue = this.m_dropValues[i];
						if (blockDropValue.Count > 0)
						{
							SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(blockDropValue.Value));
							for (int j = 0; j < blockBehaviors.Length; j++)
							{
								blockBehaviors[j].OnItemHarvested(x, y, z, cellValue, ref blockDropValue, ref newValue);
							}
							if (blockDropValue.Count > 0 && Terrain.ExtractContents(blockDropValue.Value) != 0)
							{
								Vector3 position = new Vector3((float)x, (float)y, (float)z) + new Vector3(0.5f);
								this.m_subsystemPickables.AddPickable(blockDropValue.Value, blockDropValue.Count, position, null, null);
							}
						}
					}
				}
				if (flag && !noParticleSystem && this.m_subsystemViews.CalculateDistanceFromNearestView(new Vector3((float)x, (float)y, (float)z)) < 16f)
				{
					this.m_subsystemParticles.AddParticleSystem(block.CreateDebrisParticleSystem(this, new Vector3((float)x + 0.5f, (float)y + 0.5f, (float)z + 0.5f), cellValue, 1f));
				}
			}
			this.ChangeCell(x, y, z, newValue, true);
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x0004F02C File Offset: 0x0004D22C
		public void Draw(Camera camera, int drawOrder)
		{
			if (SubsystemTerrain.TerrainRenderingEnabled)
			{
				if (drawOrder == SubsystemTerrain.m_drawOrders[0])
				{
					this.TerrainUpdater.PrepareForDrawing(camera);
					this.TerrainRenderer.PrepareForDrawing(camera);
					this.TerrainRenderer.DrawOpaque(camera);
					this.TerrainRenderer.DrawAlphaTested(camera);
					return;
				}
				if (drawOrder == SubsystemTerrain.m_drawOrders[1])
				{
					this.TerrainRenderer.DrawTransparent(camera);
				}
			}
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x0004F091 File Offset: 0x0004D291
		public void Update(float dt)
		{
			this.TerrainUpdater.Update();
			this.ProcessModifiedCells();
		}

        // Token: 0x06000AA4 RID: 2724 RVA: 0x0004F0A4 File Offset: 0x0004D2A4
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemViews = base.Project.FindSubsystem<SubsystemGameWidgets>(true);
			this.SubsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.SubsystemAnimatedTextures = base.Project.FindSubsystem<SubsystemAnimatedTextures>(true);
			this.SubsystemFurnitureBlockBehavior = base.Project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true);
			this.SubsystemPalette = base.Project.FindSubsystem<SubsystemPalette>(true);
			this.Terrain = new Terrain();
			this.TerrainRenderer = new TerrainRenderer(this);
			this.TerrainUpdater = new TerrainUpdater(this);
			this.TerrainSerializer = new TerrainSerializer22(this.Terrain, this.SubsystemGameInfo.DirectoryName);
			this.BlockGeometryGenerator = new BlockGeometryGenerator(this.Terrain, this, base.Project.FindSubsystem<SubsystemElectricity>(true), this.SubsystemFurnitureBlockBehavior, base.Project.FindSubsystem<SubsystemMetersBlockBehavior>(true), this.SubsystemPalette);
			if (string.CompareOrdinal(this.SubsystemGameInfo.WorldSettings.OriginalSerializationVersion, "2.1") <= 0)
			{
				TerrainGenerationMode terrainGenerationMode = this.SubsystemGameInfo.WorldSettings.TerrainGenerationMode;
				if (terrainGenerationMode == TerrainGenerationMode.FlatContinent || terrainGenerationMode == TerrainGenerationMode.FlatIsland)
				{
					this.TerrainContentsGenerator = new TerrainContentsGeneratorFlat(this);
					return;
				}
				this.TerrainContentsGenerator = new TerrainContentGeneratorVersion2_1_inactive(this);
				return;
			}
			else
			{
				TerrainGenerationMode terrainGenerationMode2 = this.SubsystemGameInfo.WorldSettings.TerrainGenerationMode;
				if (terrainGenerationMode2 == TerrainGenerationMode.FlatContinent || terrainGenerationMode2 == TerrainGenerationMode.FlatIsland)
				{
					this.TerrainContentsGenerator = new TerrainContentsGeneratorFlat(this);
					return;
				}
				this.TerrainContentsGenerator = new TerrainChunkGeneratorProviderActive(this);
				return;
			}
		}

        // Token: 0x06000AA5 RID: 2725 RVA: 0x0004F238 File Offset: 0x0004D438
        public override void Save(ValuesDictionary valuesDictionary)
		{
			//return;
			this.TerrainUpdater.UpdateEvent.WaitOne();
			//Console.Beep();
			try
			{
				foreach (TerrainChunk chunk in this.Terrain.AllocatedChunks)
				{
					this.TerrainSerializer.SaveChunk(chunk);
				}
			}
			finally
			{
				this.TerrainUpdater.UpdateEvent.Set();
			}
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x0004F2A4 File Offset: 0x0004D4A4
		public override void Dispose()
		{
			this.TerrainRenderer.Dispose();
			this.TerrainUpdater.Dispose();
			this.TerrainSerializer.Dispose();
			this.Terrain.Dispose();
		}

		// Token: 0x040005C5 RID: 1477
		public static bool TerrainRenderingEnabled = true;

		// Token: 0x040005C6 RID: 1478
		public Dictionary<Point3, bool> m_modifiedCells = new Dictionary<Point3, bool>();

		// Token: 0x040005C7 RID: 1479
		public DynamicArray<Point3> m_modifiedList = new DynamicArray<Point3>();

		// Token: 0x040005C8 RID: 1480
		public static Point3[] m_neighborOffsets = new Point3[]
		{
			new Point3(0, 0, 0),
			new Point3(-1, 0, 0),
			new Point3(1, 0, 0),
			new Point3(0, -1, 0),
			new Point3(0, 1, 0),
			new Point3(0, 0, -1),
			new Point3(0, 0, 1)
		};

		// Token: 0x040005C9 RID: 1481
		public SubsystemGameWidgets m_subsystemViews;

		// Token: 0x040005CA RID: 1482
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x040005CB RID: 1483
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x040005CC RID: 1484
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x040005CD RID: 1485
		public List<BlockDropValue> m_dropValues = new List<BlockDropValue>();

		// Token: 0x040005CE RID: 1486
		public static int[] m_drawOrders = new int[]
		{
			0,
			100
		};
	}
}
