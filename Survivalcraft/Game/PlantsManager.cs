using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace Game
{
	// Token: 0x020002CB RID: 715
	public static class PlantsManager
	{
		// Token: 0x06001426 RID: 5158 RVA: 0x0009C084 File Offset: 0x0009A284
		static PlantsManager()
		{
			Game.Random random = new Game.Random(33);
			PlantsManager.m_treeBrushesByType[0] = new List<TerrainBrush>();
			for (int i = 0; i < 16; i++)
			{
				int[] array = new int[]
				{
					5,
					6,
					7,
					7,
					8,
					8,
					9,
					9,
					9,
					10,
					10,
					11,
					12,
					13,
					14,
					16
				};
				int height4 = array[i];
				int branchesCount = (int)MathUtils.Lerp(10f, 20f, (float)i / 16f);
				TerrainBrush item = PlantsManager.CreateTreeBrush(random, PlantsManager.GetTreeTrunkValue(TreeType.Oak), PlantsManager.GetTreeLeavesValue(TreeType.Oak), height4, branchesCount, delegate(int y)
				{
					float num = 0.4f;
					if ((float)y < 0.2f * (float)height4)
					{
						num = 0f;
					}
					else if ((float)y >= 0.2f * (float)height4 && y <= height4)
					{
						num *= 1.5f;
					}
					return num;
				}, delegate(int y)
				{
					if ((float)y < (float)height4 * 0.3f || (float)y > (float)height4 * 0.9f)
					{
						return 0f;
					}
					float num = ((float)y < (float)height4 * 0.7f) ? (0.5f * (float)height4) : (0.35f * (float)height4);
					return random.Float(0.33f, 1f) * num;
				});
				PlantsManager.m_treeBrushesByType[0].Add(item);
			}
			PlantsManager.m_treeBrushesByType[1] = new List<TerrainBrush>();
			for (int j = 0; j < 16; j++)
			{
				int[] array2 = new int[]
				{
					4,
					5,
					6,
					6,
					7,
					7,
					7,
					8,
					8,
					8,
					9,
					9,
					9,
					10,
					10,
					11
				};
				int height3 = array2[j];
				int branchesCount2 = (int)MathUtils.Lerp(0f, 20f, (float)j / 16f);
				TerrainBrush item2 = PlantsManager.CreateTreeBrush(random, PlantsManager.GetTreeTrunkValue(TreeType.Birch), PlantsManager.GetTreeLeavesValue(TreeType.Birch), height3, branchesCount2, delegate(int y)
				{
					float num = 0.66f;
					if (y < height3 / 2 - 1)
					{
						num = 0f;
					}
					else if (y > height3 / 2 && y <= height3)
					{
						num *= 1.5f;
					}
					return num;
				}, delegate(int y)
				{
					if ((float)y >= (float)height3 * 0.35f && (float)y <= (float)height3 * 0.75f)
					{
						return random.Float(0f, 0.33f * (float)height3);
					}
					return 0f;
				});
				PlantsManager.m_treeBrushesByType[1].Add(item2);
			}
			PlantsManager.m_treeBrushesByType[2] = new List<TerrainBrush>();
			for (int k = 0; k < 16; k++)
			{
				int[] array3 = new int[]
				{
					7,
					8,
					9,
					10,
					10,
					11,
					11,
					12,
					12,
					13,
					13,
					14,
					14,
					15,
					16,
					17
				};
				int height2 = array3[k];
				int branchesCount3 = height2 * 3;
				TerrainBrush item3 = PlantsManager.CreateTreeBrush(random, PlantsManager.GetTreeTrunkValue(TreeType.Spruce), PlantsManager.GetTreeLeavesValue(TreeType.Spruce), height2, branchesCount3, delegate(int y)
				{
					float num = MathUtils.Lerp(1.4f, 0.3f, (float)y / (float)height2);
					if (y < 3)
					{
						num = 0f;
					}
					if (y % 2 == 0)
					{
						num *= 0.3f;
					}
					return num;
				}, delegate(int y)
				{
					if (y < 3 || (float)y > (float)height2 * 0.8f)
					{
						return 0f;
					}
					if (y % 2 != 0)
					{
						return MathUtils.Lerp(0.3f * (float)height2, 0f, MathUtils.Saturate((float)y / (float)height2));
					}
					return 0f;
				});
				PlantsManager.m_treeBrushesByType[2].Add(item3);
			}
			PlantsManager.m_treeBrushesByType[3] = new List<TerrainBrush>();
			for (int l = 0; l < 16; l++)
			{
				int[] array4 = new int[]
				{
					20,
					21,
					22,
					23,
					24,
					24,
					25,
					25,
					26,
					26,
					27,
					27,
					28,
					28,
					29,
					29,
					30,
					30
				};
				int height = array4[l];
				int branchesCount4 = height * 3;
				float startHeight = (0.3f + (float)(l % 4) * 0.05f) * (float)height;
				TerrainBrush item4 = PlantsManager.CreateTreeBrush(random, PlantsManager.GetTreeTrunkValue(TreeType.TallSpruce), PlantsManager.GetTreeLeavesValue(TreeType.TallSpruce), height, branchesCount4, delegate(int y)
				{
					float num = MathUtils.Saturate((float)y / (float)height);
					float num2 = MathUtils.Lerp(1.5f, 0f, MathUtils.Saturate((num - 0.6f) / 0.4f));
					if ((float)y < startHeight)
					{
						num2 = 0f;
					}
					if (y % 3 != 0 && y < height - 4)
					{
						num2 *= 0.2f;
					}
					return num2;
				}, delegate(int y)
				{
					float num = MathUtils.Saturate((float)y / (float)height);
					if (y % 3 != 0)
					{
						return 0f;
					}
					if ((float)y >= startHeight)
					{
						return MathUtils.Lerp(0.18f * (float)height, 0f, MathUtils.Saturate((num - 0.6f) / 0.4f));
					}
					if ((float)y < startHeight - 4f)
					{
						return 0f;
					}
					return 0.1f * (float)height;
				});
				PlantsManager.m_treeBrushesByType[3].Add(item4);
			}
			PlantsManager.m_treeBrushesByType[4] = new List<TerrainBrush>();
			for (int m = 0; m < 16; m++)
			{
				PlantsManager.m_treeBrushesByType[4].Add(PlantsManager.CreateMimosaBrush(random, MathUtils.Lerp(6f, 9f, (float)m / 15f)));
			}
		}

		// Token: 0x06001427 RID: 5159 RVA: 0x0009C406 File Offset: 0x0009A606
		public static int GetTreeTrunkValue(TreeType treeType)
		{
			return PlantsManager.m_treeTrunksByType[(int)treeType];
		}

		// Token: 0x06001428 RID: 5160 RVA: 0x0009C40F File Offset: 0x0009A60F
		public static int GetTreeLeavesValue(TreeType treeType)
		{
			return PlantsManager.m_treeLeavesByType[(int)treeType];
		}

		// Token: 0x06001429 RID: 5161 RVA: 0x0009C418 File Offset: 0x0009A618
		public static ReadOnlyList<TerrainBrush> GetTreeBrushes(TreeType treeType)
		{
			return new ReadOnlyList<TerrainBrush>(PlantsManager.m_treeBrushesByType[(int)treeType]);
		}

		// Token: 0x0600142A RID: 5162 RVA: 0x0009C428 File Offset: 0x0009A628
		public static int GenerateRandomPlantValue(Game.Random random, int groundValue, int temperature, int humidity, int y)
		{
			int num = Terrain.ExtractContents(groundValue);
			if (num != 2)
			{
				if (num != 7)
				{
					if (num != 8)
					{
						return 0;
					}
				}
				else
				{
					if (humidity >= 8 || random.Float(0f, 1f) >= 0.01f)
					{
						return 0;
					}
					if (random.Float(0f, 1f) < 0.05f)
					{
						return Terrain.MakeBlockValue(99, 0, 0);
					}
					return Terrain.MakeBlockValue(28, 0, 0);
				}
			}
			if (humidity >= 6)
			{
				if (random.Float(0f, 1f) < (float)humidity / 60f)
				{
					int result = Terrain.MakeBlockValue(19, 0, TallGrassBlock.SetIsSmall(0, false));
					if (!SubsystemWeather.IsPlaceFrozen(temperature, y))
					{
						float num2 = random.Float(0f, 1f);
						if (num2 < 0.04f)
						{
							result = Terrain.MakeBlockValue(20);
						}
						else if (num2 < 0.07f)
						{
							result = Terrain.MakeBlockValue(24);
						}
						else if (num2 < 0.09f)
						{
							result = Terrain.MakeBlockValue(25);
						}
						else if (num2 < 0.17f)
						{
							result = Terrain.MakeBlockValue(174, 0, RyeBlock.SetIsWild(RyeBlock.SetSize(0, 7), true));
						}
						else if (num2 < 0.19f)
						{
							result = Terrain.MakeBlockValue(204, 0, CottonBlock.SetIsWild(CottonBlock.SetSize(0, 2), true));
						}
					}
					return result;
				}
			}
			else if (random.Float(0f, 1f) < 0.025f)
			{
				if (random.Float(0f, 1f) < 0.2f)
				{
					return Terrain.MakeBlockValue(99, 0, 0);
				}
				return Terrain.MakeBlockValue(28, 0, 0);
			}
			return 0;
		}

		// Token: 0x0600142B RID: 5163 RVA: 0x0009C5A8 File Offset: 0x0009A7A8
		public static TreeType? GenerateRandomTreeType(Game.Random random, int temperature, int humidity, int y, float densityMultiplier = 1f)
		{
			TreeType? result = null;
			float num = random.Float() * PlantsManager.CalculateTreeProbability(TreeType.Oak, temperature, humidity, y);
			float num2 = random.Float() * PlantsManager.CalculateTreeProbability(TreeType.Birch, temperature, humidity, y);
			float num3 = random.Float() * PlantsManager.CalculateTreeProbability(TreeType.Spruce, temperature, humidity, y);
			float num4 = random.Float() * PlantsManager.CalculateTreeProbability(TreeType.TallSpruce, temperature, humidity, y);
			float num5 = random.Float() * PlantsManager.CalculateTreeProbability(TreeType.Mimosa, temperature, humidity, y);
			float num6 = MathUtils.Max(MathUtils.Max(num, num2, num3, num4), num5);
			if (num6 > 0f)
			{
				if (num6 == num)
				{
					result = new TreeType?(TreeType.Oak);
				}
				if (num6 == num2)
				{
					result = new TreeType?(TreeType.Birch);
				}
				if (num6 == num3)
				{
					result = new TreeType?(TreeType.Spruce);
				}
				if (num6 == num4)
				{
					result = new TreeType?(TreeType.TallSpruce);
				}
				if (num6 == num5)
				{
					result = new TreeType?(TreeType.Mimosa);
				}
			}
			if (result != null && random.Bool(densityMultiplier * PlantsManager.CalculateTreeDensity(result.Value, temperature, humidity, y)))
			{
				return result;
			}
			return null;
		}

		// Token: 0x0600142C RID: 5164 RVA: 0x0009C6A4 File Offset: 0x0009A8A4
		public static float CalculateTreeDensity(TreeType treeType, int temperature, int humidity, int y)
		{
			switch (treeType)
			{
			case TreeType.Oak:
				return PlantsManager.RangeProbability((float)humidity, 4f, 15f, 15f, 15f);
			case TreeType.Birch:
				return PlantsManager.RangeProbability((float)humidity, 4f, 15f, 15f, 15f);
			case TreeType.Spruce:
				return PlantsManager.RangeProbability((float)humidity, 4f, 15f, 15f, 15f);
			case TreeType.TallSpruce:
				return PlantsManager.RangeProbability((float)humidity, 4f, 15f, 15f, 15f);
			case TreeType.Mimosa:
				return 0.03f;
			default:
				return 0f;
			}
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x0009C748 File Offset: 0x0009A948
		public static float CalculateTreeProbability(TreeType treeType, int temperature, int humidity, int y)
		{
			switch (treeType)
			{
			case TreeType.Oak:
				return PlantsManager.RangeProbability((float)temperature, 4f, 10f, 15f, 15f) * PlantsManager.RangeProbability((float)humidity, 6f, 8f, 15f, 15f) * PlantsManager.RangeProbability((float)y, 0f, 0f, 82f, 87f);
			case TreeType.Birch:
				return PlantsManager.RangeProbability((float)temperature, 5f, 9f, 9f, 14f) * PlantsManager.RangeProbability((float)humidity, 3f, 15f, 15f, 15f) * PlantsManager.RangeProbability((float)y, 0f, 0f, 82f, 87f);
			case TreeType.Spruce:
				return PlantsManager.RangeProbability((float)temperature, 0f, 0f, 6f, 10f) * PlantsManager.RangeProbability((float)humidity, 3f, 10f, 11f, 12f);
			case TreeType.TallSpruce:
				return 0.25f * PlantsManager.RangeProbability((float)temperature, 0f, 0f, 6f, 10f) * PlantsManager.RangeProbability((float)humidity, 9f, 11f, 15f, 15f) * PlantsManager.RangeProbability((float)y, 0f, 0f, 95f, 100f);
			case TreeType.Mimosa:
				return PlantsManager.RangeProbability((float)temperature, 2f, 4f, 12f, 14f) * PlantsManager.RangeProbability((float)humidity, 0f, 0f, 4f, 6f);
			default:
				return 0f;
			}
		}

		// Token: 0x0600142E RID: 5166 RVA: 0x0009C8EB File Offset: 0x0009AAEB
		public static float RangeProbability(float v, float a, float b, float c, float d)
		{
			if (v < a)
			{
				return 0f;
			}
			if (v < b)
			{
				return (v - a) / (b - a);
			}
			if (v <= c)
			{
				return 1f;
			}
			if (v <= d)
			{
				return 1f - (v - c) / (d - c);
			}
			return 0f;
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x0009C928 File Offset: 0x0009AB28
		public static TerrainBrush CreateTreeBrush(Game.Random random, int woodIndex, int leavesIndex, int height, int branchesCount, Func<int, float> leavesProbabilityByHeight, Func<int, float> branchesLengthByHeight)
		{
			TerrainBrush terrainBrush = new TerrainBrush();
			terrainBrush.AddRay(0, -1, 0, 0, height, 0, 1, 1, 1, woodIndex);
			for (int i = 0; i < branchesCount; i++)
			{
				int x = 0;
				int num = random.Int(0, height);
				int z = 0;
				float s = branchesLengthByHeight(num);
				Vector3 vector = Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(0f, 0.33f), random.Float(-1f, 1f))) * s;
				int x2 = (int)MathUtils.Round(vector.X);
				int y = num + (int)MathUtils.Round(vector.Y);
				int z2 = (int)MathUtils.Round(vector.Z);
				int cutFace = 0;
				if (MathUtils.Abs(vector.X) == MathUtils.Max(MathUtils.Abs(vector.X), MathUtils.Abs(vector.Y), MathUtils.Abs(vector.Z)))
				{
					cutFace = 1;
				}
				else if (MathUtils.Abs(vector.Y) == MathUtils.Max(MathUtils.Abs(vector.X), MathUtils.Abs(vector.Y), MathUtils.Abs(vector.Z)))
				{
					cutFace = 4;
				}
				terrainBrush.AddRay(x, num, z, x2, y, z2, 1, 1, 1, (TerrainBrush.Brush)delegate(int? v)
				{
					if (v == null)
					{
						return new int?(Terrain.MakeBlockValue(woodIndex, 0, WoodBlock.SetCutFace(0, cutFace)));
					}
					return new int?();
				});
			}
			Func<int?, int> func0 = null;
			for (int j = 0; j < 3; j++)
			{
				Point3 point;
				Point3 point2;
				terrainBrush.CalculateBounds(out point, out point2);
				for (int k = point.X - 1; k <= point2.X + 1; k++)
				{
					for (int l = point.Z - 1; l <= point2.Z + 1; l++)
					{
						for (int m = 1; m <= point2.Y + 1; m++)
						{
							float num2 = leavesProbabilityByHeight(m);
							if (random.Float(0f, 1f) < num2 && terrainBrush.GetValue(k, m, l) == null)
							{
								if (terrainBrush.CountNonDiagonalNeighbors(k, m, l, leavesIndex) == 0)
								{
									TerrainBrush terrainBrush2 = terrainBrush;
									int x3 = k;
									int y2 = m;
									int z3 = l;
									Func<int?, int> handler;
									if ((handler = func0) == null)
									{
										handler = (func0 = delegate(int? v)
										{
											if (v == null || Terrain.ExtractContents(v.Value) != woodIndex)
											{
												return 0;
											}
											return 1;
										});
									}
									if (terrainBrush2.CountNonDiagonalNeighbors(x3, y2, z3, handler) == 0)
									{
										goto IL_250;
									}
								}
								terrainBrush.AddCell(k, m, l, 0);
							}
							IL_250:;
						}
					}
				}
				terrainBrush.Replace(0, leavesIndex);
			}
			terrainBrush.AddCell(0, height, 0, leavesIndex);
			terrainBrush.Compile();
			return terrainBrush;
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x0009CBF4 File Offset: 0x0009ADF4
		public static TerrainBrush CreateMimosaBrush(Game.Random random, float size)
		{
			TerrainBrush terrainBrush = new TerrainBrush();
			int value = PlantsManager.m_treeTrunksByType[4];
			int value2 = PlantsManager.m_treeLeavesByType[4];
			terrainBrush.AddRay(0, -1, 0, 0, 0, 0, 1, 1, 1, value);
			List<Point3> list = new List<Point3>();
			float num = random.Float(0f, 6.2831855f);
			for (int i = 0; i < 3; i++)
			{
				float radians = num + (float)i * MathUtils.DegToRad(120f);
				Vector3 v = Vector3.Transform(Vector3.Normalize(new Vector3(1f, random.Float(1f, 1.5f), 0f)), Matrix.CreateRotationY(radians));
				int num2 = random.Int((int)(0.7f * size), (int)size);
				Point3 point = new Point3(0, 0, 0);
				Point3 point2 = new Point3(Vector3.Round(new Vector3(point) + v * (float)num2));
				terrainBrush.AddRay(point.X, point.Y, point.Z, point2.X, point2.Y, point2.Z, 1, 1, 1, value);
				list.Add(point2);
			}
			foreach (Point3 point3 in list)
			{
				float num3 = random.Float(0.3f * size, 0.45f * size);
				int num4 = (int)MathUtils.Ceiling(num3);
				for (int j = point3.X - num4; j <= point3.X + num4; j++)
				{
					for (int k = point3.Y - num4; k <= point3.Y + num4; k++)
					{
						for (int l = point3.Z - num4; l <= point3.Z + num4; l++)
						{
							int num5 = MathUtils.Abs(j - point3.X) + MathUtils.Abs(k - point3.Y) + MathUtils.Abs(l - point3.Z);
							float num6 = ((new Vector3((float)j, (float)k, (float)l) - new Vector3(point3)) * new Vector3(1f, 1.7f, 1f)).Length();
							if (num6 <= num3 && (num3 - num6 > 1f || num5 <= 2 || random.Bool(0.7f)) && terrainBrush.GetValue(j, k, l) == null)
							{
								terrainBrush.AddCell(j, k, l, value2);
							}
						}
					}
				}
			}
			terrainBrush.Compile();
			return terrainBrush;
		}

		// Token: 0x04000E07 RID: 3591
		public static List<TerrainBrush>[] m_treeBrushesByType = new List<TerrainBrush>[EnumUtils.GetEnumValues(typeof(TreeType)).Max() + 1];

		// Token: 0x04000E08 RID: 3592
		public static int[] m_treeTrunksByType = new int[]
		{
			9,
			10,
			11,
			11,
			255
		};

		// Token: 0x04000E09 RID: 3593
		public static int[] m_treeLeavesByType = new int[]
		{
			12,
			13,
			14,
			225,
			256
		};
	}
}
