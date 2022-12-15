using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Engine;
using Engine.Graphics;
using Engine.Media;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001A6 RID: 422
	public class SubsystemSignBlockBehavior : SubsystemBlockBehavior, IDrawable, IUpdateable
	{
		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000A15 RID: 2581 RVA: 0x000495C2 File Offset: 0x000477C2
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					97,
					98,
					210,
					211
				};
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000A16 RID: 2582 RVA: 0x000495D5 File Offset: 0x000477D5
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000A17 RID: 2583 RVA: 0x000495D8 File Offset: 0x000477D8
		public int[] DrawOrders
		{
			get
			{
				return SubsystemSignBlockBehavior.m_drawOrders;
			}
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x000495E0 File Offset: 0x000477E0
		public SignData GetSignData(Point3 point)
		{
			SubsystemSignBlockBehavior.TextData textData;
			if (this.m_textsByPoint.TryGetValue(point, out textData))
			{
				return new SignData
				{
					Lines = textData.Lines.ToArray<string>(),
					Colors = textData.Colors.ToArray<Color>(),
					Url = textData.Url
				};
			}
			return null;
		}

		// Token: 0x06000A19 RID: 2585 RVA: 0x00049634 File Offset: 0x00047834
		public void SetSignData(Point3 point, string[] lines, Color[] colors, string url)
		{
			SubsystemSignBlockBehavior.TextData textData = new SubsystemSignBlockBehavior.TextData();
			textData.Point = point;
			for (int i = 0; i < 4; i++)
			{
				textData.Lines[i] = lines[i];
				textData.Colors[i] = colors[i];
			}
			textData.Url = url;
			this.m_textsByPoint[point] = textData;
			this.m_lastUpdatePositions.Clear();
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x00049698 File Offset: 0x00047898
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellValueFast = base.SubsystemTerrain.Terrain.GetCellValueFast(x, y, z);
			int num = Terrain.ExtractContents(cellValueFast);
			int data = Terrain.ExtractData(cellValueFast);
			Block block = BlocksManager.Blocks[num];
			if (block is AttachedSignBlock)
			{
				Point3 point = CellFace.FaceToPoint3(AttachedSignBlock.GetFace(data));
				int x2 = x - point.X;
				int y2 = y - point.Y;
				int z2 = z - point.Z;
				int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x2, y2, z2);
				if (!BlocksManager.Blocks[cellContents].IsCollidable)
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
					return;
				}
			}
			else if (block is PostedSignBlock)
			{
				int num2 = PostedSignBlock.GetHanging(data) ? base.SubsystemTerrain.Terrain.GetCellContents(x, y + 1, z) : base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
				if (!BlocksManager.Blocks[num2].IsCollidable)
				{
					base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
				}
			}
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x00049798 File Offset: 0x00047998
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			Point3 point = new Point3(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Adventure)
			{
				SignData signData = this.GetSignData(point);
				if (signData != null && !string.IsNullOrEmpty(signData.Url))
				{
					WebBrowserManager.LaunchBrowser(signData.Url);
				}
			}
			else if (componentMiner.ComponentPlayer != null)
			{
				DialogsManager.ShowDialog(componentMiner.ComponentPlayer.GuiWidget, new EditSignDialog(this, point));
			}
			return true;
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x00049840 File Offset: 0x00047A40
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			Point3 key = new Point3(x, y, z);
			this.m_textsByPoint.Remove(key);
			this.m_lastUpdatePositions.Clear();
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x00049871 File Offset: 0x00047A71
		public void Update(float dt)
		{
			this.UpdateRenderTarget();
		}

		// Token: 0x06000A1E RID: 2590 RVA: 0x00049879 File Offset: 0x00047A79
		public void Draw(Camera camera, int drawOrder)
		{
			this.DrawSigns(camera);
		}

        // Token: 0x06000A1F RID: 2591 RVA: 0x00049884 File Offset: 0x00047A84
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemViews = base.Project.FindSubsystem<SubsystemGameWidgets>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			foreach (object obj in valuesDictionary.GetValue<ValuesDictionary>("Texts").Values)
			{
				ValuesDictionary valuesDictionary2 = (ValuesDictionary)obj;
				Point3 value = valuesDictionary2.GetValue<Point3>("Point");
				string value2 = valuesDictionary2.GetValue<string>("Line1", string.Empty);
				string value3 = valuesDictionary2.GetValue<string>("Line2", string.Empty);
				string value4 = valuesDictionary2.GetValue<string>("Line3", string.Empty);
				string value5 = valuesDictionary2.GetValue<string>("Line4", string.Empty);
				Color value6 = valuesDictionary2.GetValue<Color>("Color1", Color.Black);
				Color value7 = valuesDictionary2.GetValue<Color>("Color2", Color.Black);
				Color value8 = valuesDictionary2.GetValue<Color>("Color3", Color.Black);
				Color value9 = valuesDictionary2.GetValue<Color>("Color4", Color.Black);
				string value10 = valuesDictionary2.GetValue<string>("Url", string.Empty);
				this.SetSignData(value, new string[]
				{
					value2,
					value3,
					value4,
					value5
				}, new Color[]
				{
					value6,
					value7,
					value8,
					value9
				}, value10);
			}
			this.CreateRenderTarget();
			Display.DeviceReset += this.Display_DeviceReset;
		}

        // Token: 0x06000A20 RID: 2592 RVA: 0x00049A34 File Offset: 0x00047C34
        public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			int num = 0;
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Texts", valuesDictionary2);
			foreach (SubsystemSignBlockBehavior.TextData textData in this.m_textsByPoint.Values)
			{
				ValuesDictionary valuesDictionary3 = new ValuesDictionary();
				valuesDictionary3.SetValue<Point3>("Point", textData.Point);
				if (!string.IsNullOrEmpty(textData.Lines[0]))
				{
					valuesDictionary3.SetValue<string>("Line1", textData.Lines[0]);
				}
				if (!string.IsNullOrEmpty(textData.Lines[1]))
				{
					valuesDictionary3.SetValue<string>("Line2", textData.Lines[1]);
				}
				if (!string.IsNullOrEmpty(textData.Lines[2]))
				{
					valuesDictionary3.SetValue<string>("Line3", textData.Lines[2]);
				}
				if (!string.IsNullOrEmpty(textData.Lines[3]))
				{
					valuesDictionary3.SetValue<string>("Line4", textData.Lines[3]);
				}
				if (textData.Colors[0] != Color.Black)
				{
					valuesDictionary3.SetValue<Color>("Color1", textData.Colors[0]);
				}
				if (textData.Colors[1] != Color.Black)
				{
					valuesDictionary3.SetValue<Color>("Color2", textData.Colors[1]);
				}
				if (textData.Colors[2] != Color.Black)
				{
					valuesDictionary3.SetValue<Color>("Color3", textData.Colors[2]);
				}
				if (textData.Colors[3] != Color.Black)
				{
					valuesDictionary3.SetValue<Color>("Color4", textData.Colors[3]);
				}
				if (!string.IsNullOrEmpty(textData.Url))
				{
					valuesDictionary3.SetValue<string>("Url", textData.Url);
				}
				valuesDictionary2.SetValue<ValuesDictionary>(num++.ToString(CultureInfo.InvariantCulture), valuesDictionary3);
			}
		}

		// Token: 0x06000A21 RID: 2593 RVA: 0x00049C54 File Offset: 0x00047E54
		public override void Dispose()
		{
			base.Dispose();
			Utilities.Dispose<RenderTarget2D>(ref this.m_renderTarget);
			Display.DeviceReset -= this.Display_DeviceReset;
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x00049C78 File Offset: 0x00047E78
		public void Display_DeviceReset()
		{
			this.InvalidateRenderTarget();
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x00049C80 File Offset: 0x00047E80
		public void CreateRenderTarget()
		{
			this.m_renderTarget = new RenderTarget2D(128, 1024, 1, ColorFormat.Rgba8888, DepthFormat.None);
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x00049C9C File Offset: 0x00047E9C
		public void InvalidateRenderTarget()
		{
			this.m_lastUpdatePositions.Clear();
			for (int i = 0; i < this.m_textureLocations.Length; i++)
			{
				this.m_textureLocations[i] = null;
			}
			foreach (SubsystemSignBlockBehavior.TextData textData in this.m_textsByPoint.Values)
			{
				textData.TextureLocation = null;
			}
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x00049D20 File Offset: 0x00047F20
		public void RenderText(FontBatch2D fontBatch, FlatBatch2D flatBatch, SubsystemSignBlockBehavior.TextData textData)
		{
			if (textData.TextureLocation == null)
			{
				return;
			}
			List<string> list = new List<string>();
			List<Color> list2 = new List<Color>();
			for (int i = 0; i < textData.Lines.Length; i++)
			{
				if (!string.IsNullOrEmpty(textData.Lines[i]))
				{
					list.Add(textData.Lines[i].Replace("\\", "").ToUpper());
					list2.Add(textData.Colors[i]);
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			float usedTextureWidth = (float)(list.Max((string l) => l.Length) * 32);
			float usedTextureHeight = (float)(list.Count * 32);
			int num = 0;
			foreach (string text in list)
			{
				fontBatch.QueueText(list[num], new Vector2(0f, (float)(num * 32 + textData.TextureLocation.Value * 32)), 0f, list2[num], TextAnchor.Default);
				num++;
			}
			textData.UsedTextureHeight = usedTextureHeight;
			textData.UsedTextureWidth = usedTextureWidth;
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x00049E70 File Offset: 0x00048070
		public void UpdateRenderTarget()
		{
			bool flag = false;
			foreach (GameWidget gameWidget in this.m_subsystemViews.GameWidgets)
			{
				bool flag2 = false;
				foreach (Vector3 v2 in this.m_lastUpdatePositions)
				{
					if (Vector3.DistanceSquared(gameWidget.ActiveCamera.ViewPosition, v2) < 4f)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			this.m_lastUpdatePositions.Clear();
			this.m_lastUpdatePositions.AddRange(from v in this.m_subsystemViews.GameWidgets
			select v.ActiveCamera.ViewPosition);
			this.m_nearTexts.Clear();
			foreach (SubsystemSignBlockBehavior.TextData textData in this.m_textsByPoint.Values)
			{
				Point3 point = textData.Point;
				float num = this.m_subsystemViews.CalculateSquaredDistanceFromNearestView(new Vector3(point));
				if (num <= 400f)
				{
					textData.Distance = num;
					this.m_nearTexts.Add(textData);
				}
			}
			this.m_nearTexts.Sort((SubsystemSignBlockBehavior.TextData d1, SubsystemSignBlockBehavior.TextData d2) => Comparer<float>.Default.Compare(d1.Distance, d2.Distance));
			if (this.m_nearTexts.Count > 32)
			{
				this.m_nearTexts.RemoveRange(32, this.m_nearTexts.Count - 32);
			}
			foreach (SubsystemSignBlockBehavior.TextData textData2 in this.m_nearTexts)
			{
				textData2.ToBeRenderedFrame = Time.FrameIndex;
			}
			bool flag3 = false;
			for (int i = 0; i < MathUtils.Min(this.m_nearTexts.Count, 32); i++)
			{
				SubsystemSignBlockBehavior.TextData textData3 = this.m_nearTexts[i];
				if (textData3.TextureLocation == null)
				{
					int num2 = this.m_textureLocations.FirstIndex((SubsystemSignBlockBehavior.TextData d) => d == null);
					if (num2 < 0)
					{
						num2 = this.m_textureLocations.FirstIndex((SubsystemSignBlockBehavior.TextData d) => d.ToBeRenderedFrame != Time.FrameIndex);
					}
					if (num2 >= 0)
					{
						SubsystemSignBlockBehavior.TextData textData4 = this.m_textureLocations[num2];
						if (textData4 != null)
						{
							textData4.TextureLocation = null;
							this.m_textureLocations[num2] = null;
						}
						this.m_textureLocations[num2] = textData3;
						textData3.TextureLocation = new int?(num2);
						flag3 = true;
					}
				}
			}
			if (flag3)
			{
				RenderTarget2D renderTarget = Display.RenderTarget;
				Display.RenderTarget = this.m_renderTarget;
				try
				{
					Display.Clear(new Vector4?(new Vector4(Color.Transparent)), null, null);
					FlatBatch2D flatBatch = this.m_primitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, BlendState.Opaque);
					FontBatch2D fontBatch = this.m_primitivesRenderer2D.FontBatch(this.m_font, 1, DepthStencilState.None, null, BlendState.Opaque, SamplerState.PointClamp);
					for (int j = 0; j < this.m_textureLocations.Length; j++)
					{
						SubsystemSignBlockBehavior.TextData textData5 = this.m_textureLocations[j];
						if (textData5 != null)
						{
							this.RenderText(fontBatch, flatBatch, textData5);
						}
					}
					this.m_primitivesRenderer2D.Flush(true, int.MaxValue);
				}
				finally
				{
					Display.RenderTarget = renderTarget;
				}
			}
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x0004A260 File Offset: 0x00048460
		public void DrawSigns(Camera camera)
		{
			if (this.m_nearTexts.Count > 0)
			{
				TexturedBatch3D texturedBatch3D = this.m_primitivesRenderer3D.TexturedBatch(this.m_renderTarget, false, 0, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwiseScissor, null, SamplerState.PointClamp);
				foreach (SubsystemSignBlockBehavior.TextData textData in this.m_nearTexts)
				{
					if (textData.TextureLocation != null)
					{
						int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(textData.Point.X, textData.Point.Y, textData.Point.Z);
						int num = Terrain.ExtractContents(cellValue);
						SignBlock signBlock = BlocksManager.Blocks[num] as SignBlock;
						if (signBlock != null)
						{
							int data = Terrain.ExtractData(cellValue);
							BlockMesh signSurfaceBlockMesh = signBlock.GetSignSurfaceBlockMesh(data);
							if (signSurfaceBlockMesh != null)
							{
								TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(textData.Point.X, textData.Point.Z);
								if (chunkAtCell != null && chunkAtCell.State >= TerrainChunkState.InvalidVertices1)
								{
									textData.Light = Terrain.ExtractLight(cellValue);
								}
								float num2 = LightingManager.LightIntensityByLightValue[textData.Light];
								Color color = new Color(num2, num2, num2);
								float x = 0f;
								float x2 = textData.UsedTextureWidth / 128f;
								float x3 = (float)textData.TextureLocation.Value / 32f;
								float x4 = ((float)textData.TextureLocation.Value + textData.UsedTextureHeight / 32f) / 32f;
								Vector3 signSurfaceNormal = signBlock.GetSignSurfaceNormal(data);
								Vector3 vector = new Vector3((float)textData.Point.X, (float)textData.Point.Y, (float)textData.Point.Z);
								float num3 = Vector3.Dot(camera.ViewPosition - (vector + new Vector3(0.5f)), signSurfaceNormal);
								Vector3 v = MathUtils.Max(0.01f * num3, 0.005f) * signSurfaceNormal;
								for (int i = 0; i < signSurfaceBlockMesh.Indices.Count / 3; i++)
								{
									BlockMeshVertex blockMeshVertex = signSurfaceBlockMesh.Vertices.Array[(int)signSurfaceBlockMesh.Indices.Array[i * 3]];
									BlockMeshVertex blockMeshVertex2 = signSurfaceBlockMesh.Vertices.Array[(int)signSurfaceBlockMesh.Indices.Array[i * 3 + 1]];
									BlockMeshVertex blockMeshVertex3 = signSurfaceBlockMesh.Vertices.Array[(int)signSurfaceBlockMesh.Indices.Array[i * 3 + 2]];
									Vector3 p = blockMeshVertex.Position + vector + v;
									Vector3 p2 = blockMeshVertex2.Position + vector + v;
									Vector3 p3 = blockMeshVertex3.Position + vector + v;
									Vector2 textureCoordinates = blockMeshVertex.TextureCoordinates;
									Vector2 textureCoordinates2 = blockMeshVertex2.TextureCoordinates;
									Vector2 textureCoordinates3 = blockMeshVertex3.TextureCoordinates;
									textureCoordinates.X = MathUtils.Lerp(x, x2, textureCoordinates.X);
									textureCoordinates2.X = MathUtils.Lerp(x, x2, textureCoordinates2.X);
									textureCoordinates3.X = MathUtils.Lerp(x, x2, textureCoordinates3.X);
									textureCoordinates.Y = MathUtils.Lerp(x3, x4, textureCoordinates.Y);
									textureCoordinates2.Y = MathUtils.Lerp(x3, x4, textureCoordinates2.Y);
									textureCoordinates3.Y = MathUtils.Lerp(x3, x4, textureCoordinates3.Y);
									texturedBatch3D.QueueTriangle(p, p2, p3, textureCoordinates, textureCoordinates2, textureCoordinates3, color);
								}
							}
						}
					}
				}
				this.m_primitivesRenderer3D.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
			}
		}

		// Token: 0x04000562 RID: 1378
		public const float m_maxVisibilityDistanceSqr = 400f;

		// Token: 0x04000563 RID: 1379
		public const float m_minUpdateDistance = 2f;

		// Token: 0x04000564 RID: 1380
		public const int m_textWidth = 128;

		// Token: 0x04000565 RID: 1381
		public const int m_textHeight = 32;

		// Token: 0x04000566 RID: 1382
		public const int m_maxTexts = 32;

		// Token: 0x04000567 RID: 1383
		public SubsystemGameWidgets m_subsystemViews;

		// Token: 0x04000568 RID: 1384
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000569 RID: 1385
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x0400056A RID: 1386
		public Dictionary<Point3, SubsystemSignBlockBehavior.TextData> m_textsByPoint = new Dictionary<Point3, SubsystemSignBlockBehavior.TextData>();

		// Token: 0x0400056B RID: 1387
		public SubsystemSignBlockBehavior.TextData[] m_textureLocations = new SubsystemSignBlockBehavior.TextData[32];

		// Token: 0x0400056C RID: 1388
		public List<SubsystemSignBlockBehavior.TextData> m_nearTexts = new List<SubsystemSignBlockBehavior.TextData>();

		// Token: 0x0400056D RID: 1389
		public BitmapFont m_font = ContentManager.Get<BitmapFont>("Fonts/Pericles");

		// Token: 0x0400056E RID: 1390
		public RenderTarget2D m_renderTarget;

		// Token: 0x0400056F RID: 1391
		public List<Vector3> m_lastUpdatePositions = new List<Vector3>();

		// Token: 0x04000570 RID: 1392
		public PrimitivesRenderer2D m_primitivesRenderer2D = new PrimitivesRenderer2D();

		// Token: 0x04000571 RID: 1393
		public PrimitivesRenderer3D m_primitivesRenderer3D = new PrimitivesRenderer3D();

		// Token: 0x04000572 RID: 1394
		public bool ShowSignsTexture;

		// Token: 0x04000573 RID: 1395
		public bool CopySignsText;

		// Token: 0x04000574 RID: 1396
		public static int[] m_drawOrders = new int[]
		{
			50
		};

		// Token: 0x0200043B RID: 1083
		public class TextData
		{
			// Token: 0x040015ED RID: 5613
			public Point3 Point;

			// Token: 0x040015EE RID: 5614
			public string[] Lines = new string[]
			{
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty
			};

			// Token: 0x040015EF RID: 5615
			public Color[] Colors = new Color[]
			{
				Color.Black,
				Color.Black,
				Color.Black,
				Color.Black
			};

			// Token: 0x040015F0 RID: 5616
			public string Url = string.Empty;

			// Token: 0x040015F1 RID: 5617
			public int? TextureLocation;

			// Token: 0x040015F2 RID: 5618
			public float UsedTextureWidth;

			// Token: 0x040015F3 RID: 5619
			public float UsedTextureHeight;

			// Token: 0x040015F4 RID: 5620
			public float Distance;

			// Token: 0x040015F5 RID: 5621
			public int ToBeRenderedFrame;

			// Token: 0x040015F6 RID: 5622
			public int Light;
		}
	}
}
