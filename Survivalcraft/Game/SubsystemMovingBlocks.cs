using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Engine;
using Engine.Graphics;
using Engine.Serialization;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000192 RID: 402
	public class SubsystemMovingBlocks : Subsystem, IUpdateable, IDrawable
	{
		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000944 RID: 2372 RVA: 0x0004001F File Offset: 0x0003E21F
		public IReadOnlyList<IMovingBlockSet> MovingBlockSets
		{
			get
			{
				return this.m_movingBlockSets;
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000945 RID: 2373 RVA: 0x00040027 File Offset: 0x0003E227
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000946 RID: 2374 RVA: 0x0004002A File Offset: 0x0003E22A
		public int[] DrawOrders
		{
			get
			{
				return SubsystemMovingBlocks.m_drawOrders;
			}
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000947 RID: 2375 RVA: 0x00040034 File Offset: 0x0003E234
		// (remove) Token: 0x06000948 RID: 2376 RVA: 0x0004006C File Offset: 0x0003E26C
		public event Action<IMovingBlockSet, Point3> CollidedWithTerrain;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000949 RID: 2377 RVA: 0x000400A4 File Offset: 0x0003E2A4
		// (remove) Token: 0x0600094A RID: 2378 RVA: 0x000400DC File Offset: 0x0003E2DC
		public event Action<IMovingBlockSet> Stopped;

		// Token: 0x0600094B RID: 2379 RVA: 0x00040114 File Offset: 0x0003E314
		public IMovingBlockSet AddMovingBlockSet(Vector3 position, Vector3 targetPosition, float speed, float acceleration, float drag, Vector2 smoothness, IEnumerable<MovingBlock> blocks, string id, object tag, bool testCollision)
		{
			SubsystemMovingBlocks.MovingBlockSet movingBlockSet = new SubsystemMovingBlocks.MovingBlockSet
			{
				Position = position,
				StartPosition = position,
				TargetPosition = targetPosition,
				Speed = speed,
				Acceleration = acceleration,
				Drag = drag,
				Smoothness = smoothness,
				Id = id,
				Tag = tag,
				Blocks = blocks.ToList<MovingBlock>()
			};
			movingBlockSet.UpdateBox();
			if (testCollision)
			{
				this.MovingBlocksCollision(movingBlockSet);
				if (movingBlockSet.Stop)
				{
					return null;
				}
			}
			if (this.m_canGenerateGeometry)
			{
				this.GenerateGeometry(movingBlockSet);
			}
			this.m_movingBlockSets.Add(movingBlockSet);
			return movingBlockSet;
		}

		// Token: 0x0600094C RID: 2380 RVA: 0x000401B0 File Offset: 0x0003E3B0
		public void RemoveMovingBlockSet(IMovingBlockSet movingBlockSet)
		{
			SubsystemMovingBlocks.MovingBlockSet movingBlockSet2 = (SubsystemMovingBlocks.MovingBlockSet)movingBlockSet;
			if (this.m_movingBlockSets.Remove(movingBlockSet2))
			{
				this.m_removing.Add(movingBlockSet2);
				movingBlockSet2.RemainCounter = 4;
			}
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x000401E8 File Offset: 0x0003E3E8
		public void FindMovingBlocks(BoundingBox boundingBox, bool extendToFillCells, DynamicArray<IMovingBlockSet> result)
		{
			foreach (SubsystemMovingBlocks.MovingBlockSet movingBlockSet in this.m_movingBlockSets)
			{
				if (SubsystemMovingBlocks.ExclusiveBoxIntersection(boundingBox, movingBlockSet.BoundingBox(extendToFillCells)))
				{
					result.Add(movingBlockSet);
				}
			}
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x0004024C File Offset: 0x0003E44C
		public IMovingBlockSet FindMovingBlocks(string id, object tag)
		{
			foreach (SubsystemMovingBlocks.MovingBlockSet movingBlockSet in this.m_movingBlockSets)
			{
				if (movingBlockSet.Id == id && object.Equals(movingBlockSet.Tag, tag))
				{
					return movingBlockSet;
				}
			}
			return null;
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x000402BC File Offset: 0x0003E4BC
		public MovingBlocksRaycastResult? Raycast(Vector3 start, Vector3 end, bool extendToFillCells)
		{
			Ray3 ray = new Ray3(start, Vector3.Normalize(end - start));
			BoundingBox boundingBox = new BoundingBox(Vector3.Min(start, end), Vector3.Max(start, end));
			this.m_result.Clear();
			this.FindMovingBlocks(boundingBox, extendToFillCells, this.m_result);
			float num = float.MaxValue;
			SubsystemMovingBlocks.MovingBlockSet movingBlockSet = null;
			foreach (IMovingBlockSet movingBlockSet2 in this.m_result)
			{
				SubsystemMovingBlocks.MovingBlockSet movingBlockSet3 = (SubsystemMovingBlocks.MovingBlockSet)movingBlockSet2;
				BoundingBox box = movingBlockSet3.BoundingBox(extendToFillCells);
				float? num2 = ray.Intersection(box);
				if (num2 != null && num2.Value < num)
				{
					num = num2.Value;
					movingBlockSet = movingBlockSet3;
				}
			}
			if (movingBlockSet != null)
			{
				return new MovingBlocksRaycastResult?(new MovingBlocksRaycastResult
				{
					Ray = ray,
					Distance = num,
					MovingBlockSet = movingBlockSet
				});
			}
			return null;
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x000403C4 File Offset: 0x0003E5C4
		public void Update(float dt)
		{
			this.m_canGenerateGeometry = true;
			foreach (SubsystemMovingBlocks.MovingBlockSet movingBlockSet in this.m_movingBlockSets)
			{
				TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(Terrain.ToCell(movingBlockSet.Position.X), Terrain.ToCell(movingBlockSet.Position.Z));
				if (chunkAtCell != null && chunkAtCell.State > TerrainChunkState.InvalidContents4)
				{
					movingBlockSet.Speed += movingBlockSet.Acceleration * this.m_subsystemTime.GameTimeDelta;
					if (movingBlockSet.Drag != 0f)
					{
						movingBlockSet.Speed *= MathUtils.Pow(1f - movingBlockSet.Drag, this.m_subsystemTime.GameTimeDelta);
					}
					float x = Vector3.Distance(movingBlockSet.StartPosition, movingBlockSet.Position);
					float num = Vector3.Distance(movingBlockSet.TargetPosition, movingBlockSet.Position);
					float num2 = (movingBlockSet.Smoothness.X > 0f) ? MathUtils.Saturate((MathUtils.Sqrt(x) + 0.05f) / movingBlockSet.Smoothness.X) : 1f;
					float num3 = (movingBlockSet.Smoothness.Y > 0f) ? MathUtils.Saturate((num + 0.05f) / movingBlockSet.Smoothness.Y) : 1f;
					float num4 = num2 * num3;
					bool flag = false;
					Vector3 vector = (num > 0f) ? ((movingBlockSet.TargetPosition - movingBlockSet.Position) / num) : Vector3.Zero;
					float x2 = (this.m_subsystemTime.GameTimeDelta > 0f) ? (0.95f / this.m_subsystemTime.GameTimeDelta) : 0f;
					float num5 = MathUtils.Min(movingBlockSet.Speed * num4, x2);
					if (num5 * this.m_subsystemTime.GameTimeDelta >= num)
					{
						movingBlockSet.Position = movingBlockSet.TargetPosition;
						movingBlockSet.CurrentVelocity = Vector3.Zero;
						flag = true;
					}
					else
					{
						movingBlockSet.CurrentVelocity = num5 / num * (movingBlockSet.TargetPosition - movingBlockSet.Position);
						movingBlockSet.Position += movingBlockSet.CurrentVelocity * this.m_subsystemTime.GameTimeDelta;
					}
					movingBlockSet.Stop = false;
					this.MovingBlocksCollision(movingBlockSet);
					this.TerrainCollision(movingBlockSet);
					if (movingBlockSet.Stop)
					{
						if (vector.X < 0f)
						{
							movingBlockSet.Position.X = MathUtils.Ceiling(movingBlockSet.Position.X);
						}
						else if (vector.X > 0f)
						{
							movingBlockSet.Position.X = MathUtils.Floor(movingBlockSet.Position.X);
						}
						if (vector.Y < 0f)
						{
							movingBlockSet.Position.Y = MathUtils.Ceiling(movingBlockSet.Position.Y);
						}
						else if (vector.Y > 0f)
						{
							movingBlockSet.Position.Y = MathUtils.Floor(movingBlockSet.Position.Y);
						}
						if (vector.Z < 0f)
						{
							movingBlockSet.Position.Z = MathUtils.Ceiling(movingBlockSet.Position.Z);
						}
						else if (vector.Z > 0f)
						{
							movingBlockSet.Position.Z = MathUtils.Floor(movingBlockSet.Position.Z);
						}
					}
					if (movingBlockSet.Stop || flag)
					{
						this.m_stopped.Add(movingBlockSet);
					}
				}
			}
			foreach (SubsystemMovingBlocks.MovingBlockSet obj in this.m_stopped)
			{
				Action<IMovingBlockSet> stopped = this.Stopped;
				if (stopped != null)
				{
					stopped(obj);
				}
			}
			this.m_stopped.Clear();
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x000407D0 File Offset: 0x0003E9D0
		public void Draw(Camera camera, int drawOrder)
		{
			this.m_vertices.Clear();
			this.m_indices.Clear();
			foreach (SubsystemMovingBlocks.MovingBlockSet movingBlockSet in this.m_movingBlockSets)
			{
				this.DrawMovingBlockSet(camera, movingBlockSet);
			}
			int i = 0;
			while (i < this.m_removing.Count)
			{
				SubsystemMovingBlocks.MovingBlockSet movingBlockSet2 = this.m_removing[i];
				SubsystemMovingBlocks.MovingBlockSet movingBlockSet3 = movingBlockSet2;
				int remainCounter = movingBlockSet3.RemainCounter;
				movingBlockSet3.RemainCounter = remainCounter - 1;
				if (remainCounter > 0)
				{
					this.DrawMovingBlockSet(camera, movingBlockSet2);
					i++;
				}
				else
				{
					this.m_removing.RemoveAt(i);
				}
			}
			if (this.m_vertices.Count > 0)
			{
				Vector3 viewPosition = camera.ViewPosition;
				Vector3 v = new Vector3(MathUtils.Floor(viewPosition.X), 0f, MathUtils.Floor(viewPosition.Z));
				Matrix value = Matrix.CreateTranslation(v - viewPosition) * camera.ViewMatrix.OrientationMatrix * camera.ProjectionMatrix;
				Display.BlendState = BlendState.Opaque;
				Display.DepthStencilState = DepthStencilState.Default;
				Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
				this.m_shader.GetParameter("u_origin", false).SetValue(v.XZ);
				this.m_shader.GetParameter("u_viewProjectionMatrix", false).SetValue(value);
				this.m_shader.GetParameter("u_viewPosition", false).SetValue(camera.ViewPosition);
				this.m_shader.GetParameter("u_texture", false).SetValue(this.m_subsystemAnimatedTextures.AnimatedBlocksTexture);
				this.m_shader.GetParameter("u_samplerState", false).SetValue(SamplerState.PointClamp);
				this.m_shader.GetParameter("u_fogColor", false).SetValue(new Vector3(this.m_subsystemSky.ViewFogColor));
				this.m_shader.GetParameter("u_fogStartInvLength", false).SetValue(new Vector2(this.m_subsystemSky.ViewFogRange.X, 1f / (this.m_subsystemSky.ViewFogRange.Y - this.m_subsystemSky.ViewFogRange.X)));
				Display.DrawUserIndexed<TerrainVertex>(PrimitiveType.TriangleList, this.m_shader, TerrainVertex.VertexDeclaration, this.m_vertices.Array, 0, this.m_vertices.Count, this.m_indices.Array, 0, this.m_indices.Count);
			}
			if (SubsystemMovingBlocks.DebugDrawMovingBlocks)
			{
				this.DebugDraw();
			}
		}

        // Token: 0x06000952 RID: 2386 RVA: 0x00040A68 File Offset: 0x0003EC68
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemAnimatedTextures = base.Project.FindSubsystem<SubsystemAnimatedTextures>(true);
			this.m_shader = ContentManager.Get<Shader>("Shaders/AlphaTested");
			foreach (Object obj in valuesDictionary.GetValue<ValuesDictionary>("MovingBlockSets").Values)
			{
				ValuesDictionary valuesDictionary2 = (ValuesDictionary)obj;
				Vector3 value = valuesDictionary2.GetValue<Vector3>("Position");
				Vector3 value2 = valuesDictionary2.GetValue<Vector3>("TargetPosition");
				float value3 = valuesDictionary2.GetValue<float>("Speed");
				float value4 = valuesDictionary2.GetValue<float>("Acceleration");
				float value5 = valuesDictionary2.GetValue<float>("Drag");
				Vector2 value6 = valuesDictionary2.GetValue<Vector2>("Smoothness", Vector2.Zero);
				string value7 = valuesDictionary2.GetValue<string>("Id", null);
				object value8 = valuesDictionary2.GetValue<object>("Tag", null);
				List<MovingBlock> list = new List<MovingBlock>();
				foreach (string text in valuesDictionary2.GetValue<string>("Blocks").Split(new char[]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries))
				{
					MovingBlock item = default(MovingBlock);
					string[] array2 = text.Split(new char[]
					{
						','
					}, StringSplitOptions.RemoveEmptyEntries);
					item.Value = HumanReadableConverter.ConvertFromString<int>(array2[0]);
					item.Offset.X = HumanReadableConverter.ConvertFromString<int>(array2[1]);
					item.Offset.Y = HumanReadableConverter.ConvertFromString<int>(array2[2]);
					item.Offset.Z = HumanReadableConverter.ConvertFromString<int>(array2[3]);
					list.Add(item);
				}
				this.AddMovingBlockSet(value, value2, value3, value4, value5, value6, list, value7, value8, false);
			}
		}

        // Token: 0x06000953 RID: 2387 RVA: 0x00040C5C File Offset: 0x0003EE5C
        public override void Save(ValuesDictionary valuesDictionary)
		{
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("MovingBlockSets", valuesDictionary2);
			int num = 0;
			foreach (SubsystemMovingBlocks.MovingBlockSet movingBlockSet in this.m_movingBlockSets)
			{
				ValuesDictionary valuesDictionary3 = new ValuesDictionary();
				valuesDictionary2.SetValue<ValuesDictionary>(num.ToString(CultureInfo.InvariantCulture), valuesDictionary3);
				valuesDictionary3.SetValue<Vector3>("Position", movingBlockSet.Position);
				valuesDictionary3.SetValue<Vector3>("TargetPosition", movingBlockSet.TargetPosition);
				valuesDictionary3.SetValue<float>("Speed", movingBlockSet.Speed);
				valuesDictionary3.SetValue<float>("Acceleration", movingBlockSet.Acceleration);
				valuesDictionary3.SetValue<float>("Drag", movingBlockSet.Drag);
				if (movingBlockSet.Smoothness != Vector2.Zero)
				{
					valuesDictionary3.SetValue<Vector2>("Smoothness", movingBlockSet.Smoothness);
				}
				if (movingBlockSet.Id != null)
				{
					valuesDictionary3.SetValue<string>("Id", movingBlockSet.Id);
				}
				if (movingBlockSet.Tag != null)
				{
					valuesDictionary3.SetValue<object>("Tag", movingBlockSet.Tag);
				}
				StringBuilder stringBuilder = new StringBuilder();
				foreach (MovingBlock movingBlock in movingBlockSet.Blocks)
				{
					stringBuilder.Append(HumanReadableConverter.ConvertToString(movingBlock.Value));
					stringBuilder.Append(',');
					stringBuilder.Append(HumanReadableConverter.ConvertToString(movingBlock.Offset.X));
					stringBuilder.Append(',');
					stringBuilder.Append(HumanReadableConverter.ConvertToString(movingBlock.Offset.Y));
					stringBuilder.Append(',');
					stringBuilder.Append(HumanReadableConverter.ConvertToString(movingBlock.Offset.Z));
					stringBuilder.Append(';');
				}
				valuesDictionary3.SetValue<string>("Blocks", stringBuilder.ToString());
				num++;
			}
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x00040EA4 File Offset: 0x0003F0A4
		public override void Dispose()
		{
			if (this.m_blockGeometryGenerator != null && this.m_blockGeometryGenerator.Terrain != null)
			{
				this.m_blockGeometryGenerator.Terrain.Dispose();
			}
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x00040ECB File Offset: 0x0003F0CB
		public void DebugDraw()
		{
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x00040ED0 File Offset: 0x0003F0D0
		public void MovingBlocksCollision(SubsystemMovingBlocks.MovingBlockSet movingBlockSet)
		{
			BoundingBox boundingBox = movingBlockSet.BoundingBox(true);
			this.m_result.Clear();
			this.FindMovingBlocks(boundingBox, true, this.m_result);
			for (int i = 0; i < this.m_result.Count; i++)
			{
				if (this.m_result.Array[i] != movingBlockSet)
				{
					movingBlockSet.Stop = true;
					return;
				}
			}
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x00040F2C File Offset: 0x0003F12C
		public void TerrainCollision(SubsystemMovingBlocks.MovingBlockSet movingBlockSet)
		{
			Point3 point = default(Point3);
			point.X = (int)MathUtils.Floor((float)movingBlockSet.Box.Left + movingBlockSet.Position.X);
			point.Y = (int)MathUtils.Floor((float)movingBlockSet.Box.Top + movingBlockSet.Position.Y);
			point.Z = (int)MathUtils.Floor((float)movingBlockSet.Box.Near + movingBlockSet.Position.Z);
			Point3 point2 = default(Point3);
			point2.X = (int)MathUtils.Ceiling((float)movingBlockSet.Box.Right + movingBlockSet.Position.X);
			point2.Y = (int)MathUtils.Ceiling((float)movingBlockSet.Box.Bottom + movingBlockSet.Position.Y);
			point2.Z = (int)MathUtils.Ceiling((float)movingBlockSet.Box.Far + movingBlockSet.Position.Z);
			for (int i = point.X; i < point2.X; i++)
			{
				for (int j = point.Z; j < point2.Z; j++)
				{
					for (int k = point.Y; k < point2.Y; k++)
					{
						if (Terrain.ExtractContents(this.m_subsystemTerrain.Terrain.GetCellValue(i, k, j)) != 0)
						{
							Action<IMovingBlockSet, Point3> collidedWithTerrain = this.CollidedWithTerrain;
							if (collidedWithTerrain != null)
							{
								collidedWithTerrain(movingBlockSet, new Point3(i, k, j));
							}
						}
					}
				}
			}
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x000410A4 File Offset: 0x0003F2A4
		public void GenerateGeometry(SubsystemMovingBlocks.MovingBlockSet movingBlockSet)
		{
			Point3 point = default(Point3);
			point.X = ((movingBlockSet.CurrentVelocity.X > 0f) ? ((int)MathUtils.Floor(movingBlockSet.Position.X)) : (point.X = (int)MathUtils.Ceiling(movingBlockSet.Position.X)));
			point.Y = ((movingBlockSet.CurrentVelocity.Y > 0f) ? ((int)MathUtils.Floor(movingBlockSet.Position.Y)) : (point.Y = (int)MathUtils.Ceiling(movingBlockSet.Position.Y)));
			point.Z = ((movingBlockSet.CurrentVelocity.Z > 0f) ? ((int)MathUtils.Floor(movingBlockSet.Position.Z)) : (point.Z = (int)MathUtils.Ceiling(movingBlockSet.Position.Z)));
			if (!(point != movingBlockSet.GeometryGenerationPosition))
			{
				return;
			}
			Point3 point2 = new Point3(movingBlockSet.Box.Left, movingBlockSet.Box.Top, movingBlockSet.Box.Near);
			Point3 point3 = new Point3(movingBlockSet.Box.Width, movingBlockSet.Box.Height, movingBlockSet.Box.Depth);
			point3.Y = MathUtils.Min(point3.Y, 254);
			if (this.m_blockGeometryGenerator == null)
			{
				int num = 2;
				num = (int)MathUtils.NextPowerOf2((uint)num);
				this.m_blockGeometryGenerator = new BlockGeometryGenerator(new Terrain(), this.m_subsystemTerrain, null, base.Project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true), null, base.Project.FindSubsystem<SubsystemPalette>(true));
				for (int i = 0; i < num; i++)
				{
					for (int j = 0; j < num; j++)
					{
						this.m_blockGeometryGenerator.Terrain.AllocateChunk(i, j);
					}
				}
			}
			Terrain terrain = this.m_subsystemTerrain.Terrain;
			for (int k = 0; k < point3.X + 2; k++)
			{
				for (int l = 0; l < point3.Z + 2; l++)
				{
					int x = k + point2.X + point.X - 1;
					int z = l + point2.Z + point.Z - 1;
					int shaftValue = terrain.GetShaftValue(x, z);
					this.m_blockGeometryGenerator.Terrain.SetTemperature(k, l, Terrain.ExtractTemperature(shaftValue));
					this.m_blockGeometryGenerator.Terrain.SetHumidity(k, l, Terrain.ExtractHumidity(shaftValue));
					for (int m = 0; m < point3.Y + 2; m++)
					{
						int y = m + point2.Y + point.Y - 1;
						int light = Terrain.ExtractLight(terrain.GetCellValue(x, y, z));
						this.m_blockGeometryGenerator.Terrain.SetCellValueFast(k, m, l, Terrain.MakeBlockValue(0, light, 0));
					}
				}
			}
			this.m_blockGeometryGenerator.Terrain.SeasonTemperature = terrain.SeasonTemperature;
			this.m_blockGeometryGenerator.Terrain.SeasonHumidity = terrain.SeasonHumidity;
			foreach (MovingBlock movingBlock in movingBlockSet.Blocks)
			{
				int x2 = movingBlock.Offset.X - point2.X + 1;
				int y2 = movingBlock.Offset.Y - point2.Y + 1;
				int z2 = movingBlock.Offset.Z - point2.Z + 1;
				int cellLightFast = this.m_blockGeometryGenerator.Terrain.GetCellLightFast(x2, y2, z2);
				int value = Terrain.ReplaceLight(movingBlock.Value, cellLightFast);
				this.m_blockGeometryGenerator.Terrain.SetCellValueFast(x2, y2, z2, value);
			}
			this.m_blockGeometryGenerator.ResetCache();
			movingBlockSet.Vertices.Clear();
			movingBlockSet.Indices.Clear();
			for (int n = 1; n < point3.X + 1; n++)
			{
				for (int num2 = 1; num2 < point3.Y + 1; num2++)
				{
					for (int num3 = 1; num3 < point3.Z + 1; num3++)
					{
						int cellValueFast = this.m_blockGeometryGenerator.Terrain.GetCellValueFast(n, num2, num3);
						int num4 = Terrain.ExtractContents(cellValueFast);
						if (num4 != 0)
						{
							BlocksManager.Blocks[num4].GenerateTerrainVertices(this.m_blockGeometryGenerator, movingBlockSet.Geometry, cellValueFast, n, num2, num3);
						}
					}
				}
			}
			movingBlockSet.GeometryOffset = new Vector3(point2) - new Vector3(1f);
			movingBlockSet.GeometryGenerationPosition = point;
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x00041554 File Offset: 0x0003F754
		public void DrawMovingBlockSet(Camera camera, SubsystemMovingBlocks.MovingBlockSet movingBlockSet)
		{
			if (this.m_vertices.Count <= 20000 && camera.ViewFrustum.Intersection(movingBlockSet.BoundingBox(false)))
			{
				this.GenerateGeometry(movingBlockSet);
				int count = this.m_vertices.Count;
				ushort[] array = movingBlockSet.Indices.Array;
				int count2 = movingBlockSet.Indices.Count;
				Vector3 vector = movingBlockSet.Position + movingBlockSet.GeometryOffset;
				TerrainVertex[] array2 = movingBlockSet.Vertices.Array;
				int count3 = movingBlockSet.Vertices.Count;
				for (int i = 0; i < count3; i++)
				{
					TerrainVertex item = array2[i];
					item.X += vector.X;
					item.Y += vector.Y;
					item.Z += vector.Z;
					this.m_vertices.Add(item);
				}
				for (int j = 0; j < movingBlockSet.Indices.Count; j++)
				{
					this.m_indices.Add((ushort)((int)array[j] + count));
				}
			}
		}

		// Token: 0x0600095A RID: 2394 RVA: 0x0004166C File Offset: 0x0003F86C
		public static bool ExclusiveBoxIntersection(BoundingBox b1, BoundingBox b2)
		{
			return b1.Max.X > b2.Min.X && b1.Min.X < b2.Max.X && b1.Max.Y > b2.Min.Y && b1.Min.Y < b2.Max.Y && b1.Max.Z > b2.Min.Z && b1.Min.Z < b2.Max.Z;
		}

		// Token: 0x040004DE RID: 1246
		public SubsystemTime m_subsystemTime;

		// Token: 0x040004DF RID: 1247
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040004E0 RID: 1248
		public SubsystemSky m_subsystemSky;

		// Token: 0x040004E1 RID: 1249
		public SubsystemAnimatedTextures m_subsystemAnimatedTextures;

		// Token: 0x040004E2 RID: 1250
		public List<SubsystemMovingBlocks.MovingBlockSet> m_movingBlockSets = new List<SubsystemMovingBlocks.MovingBlockSet>();

		// Token: 0x040004E3 RID: 1251
		public List<SubsystemMovingBlocks.MovingBlockSet> m_stopped = new List<SubsystemMovingBlocks.MovingBlockSet>();

		// Token: 0x040004E4 RID: 1252
		public List<SubsystemMovingBlocks.MovingBlockSet> m_removing = new List<SubsystemMovingBlocks.MovingBlockSet>();

		// Token: 0x040004E5 RID: 1253
		public DynamicArray<TerrainVertex> m_vertices = new DynamicArray<TerrainVertex>();

		// Token: 0x040004E6 RID: 1254
		public DynamicArray<ushort> m_indices = new DynamicArray<ushort>();

		// Token: 0x040004E7 RID: 1255
		public DynamicArray<IMovingBlockSet> m_result = new DynamicArray<IMovingBlockSet>();

		// Token: 0x040004E8 RID: 1256
		public Shader m_shader;

		// Token: 0x040004E9 RID: 1257
		public BlockGeometryGenerator m_blockGeometryGenerator;

		// Token: 0x040004EA RID: 1258
		public bool m_canGenerateGeometry;

		// Token: 0x040004EB RID: 1259
		public static bool DebugDrawMovingBlocks = false;

		// Token: 0x040004EC RID: 1260
		public static int[] m_drawOrders = new int[]
		{
			10
		};

		// Token: 0x02000430 RID: 1072
		public class MovingBlockSet : IMovingBlockSet
		{
			// Token: 0x1700054D RID: 1357
			// (get) Token: 0x06001E7B RID: 7803 RVA: 0x000DF13C File Offset: 0x000DD33C
			Vector3 IMovingBlockSet.Position
			{
				get
				{
					return this.Position;
				}
			}

			// Token: 0x1700054E RID: 1358
			// (get) Token: 0x06001E7C RID: 7804 RVA: 0x000DF144 File Offset: 0x000DD344
			string IMovingBlockSet.Id
			{
				get
				{
					return this.Id;
				}
			}

			// Token: 0x1700054F RID: 1359
			// (get) Token: 0x06001E7D RID: 7805 RVA: 0x000DF14C File Offset: 0x000DD34C
			object IMovingBlockSet.Tag
			{
				get
				{
					return this.Tag;
				}
			}

			// Token: 0x17000550 RID: 1360
			// (get) Token: 0x06001E7E RID: 7806 RVA: 0x000DF154 File Offset: 0x000DD354
			Vector3 IMovingBlockSet.CurrentVelocity
			{
				get
				{
					return this.CurrentVelocity;
				}
			}

			// Token: 0x17000551 RID: 1361
			// (get) Token: 0x06001E7F RID: 7807 RVA: 0x000DF15C File Offset: 0x000DD35C
			ReadOnlyList<MovingBlock> IMovingBlockSet.Blocks
			{
				get
				{
					return new ReadOnlyList<MovingBlock>(this.Blocks);
				}
			}

			// Token: 0x06001E80 RID: 7808 RVA: 0x000DF16C File Offset: 0x000DD36C
			public MovingBlockSet()
			{
				TerrainGeometrySubset terrainGeometrySubset = new TerrainGeometrySubset(this.Vertices, this.Indices);
				this.Geometry = new TerrainGeometry();
				this.Geometry.SubsetOpaque = terrainGeometrySubset;
				this.Geometry.SubsetAlphaTest = terrainGeometrySubset;
				this.Geometry.SubsetTransparent = terrainGeometrySubset;
				this.Geometry.OpaqueSubsetsByFace = new TerrainGeometrySubset[]
				{
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset
				};
				this.Geometry.AlphaTestSubsetsByFace = new TerrainGeometrySubset[]
				{
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset
				};
				this.Geometry.TransparentSubsetsByFace = new TerrainGeometrySubset[]
				{
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset
				};
			}

			// Token: 0x06001E81 RID: 7809 RVA: 0x000DF264 File Offset: 0x000DD464
			public void UpdateBox()
			{
				Point3? point = null;
				Point3? point2 = null;
				foreach (MovingBlock movingBlock in this.Blocks)
				{
					point = new Point3?((point != null) ? Point3.Min(point.Value, movingBlock.Offset) : movingBlock.Offset);
					point2 = new Point3?((point2 != null) ? Point3.Max(point2.Value, movingBlock.Offset) : movingBlock.Offset);
				}
				if (point != null)
				{
					this.Box = new Box(point.Value.X, point.Value.Y, point.Value.Z, point2.Value.X - point.Value.X + 1, point2.Value.Y - point.Value.Y + 1, point2.Value.Z - point.Value.Z + 1);
					return;
				}
				this.Box = default(Box);
			}

			// Token: 0x06001E82 RID: 7810 RVA: 0x000DF3AC File Offset: 0x000DD5AC
			public BoundingBox BoundingBox(bool extendToFillCells)
			{
				Vector3 vector = new Vector3(this.Position.X + (float)this.Box.Left, this.Position.Y + (float)this.Box.Top, this.Position.Z + (float)this.Box.Near);
				Vector3 vector2 = new Vector3(this.Position.X + (float)this.Box.Right, this.Position.Y + (float)this.Box.Bottom, this.Position.Z + (float)this.Box.Far);
				if (extendToFillCells)
				{
					vector.X = MathUtils.Floor(vector.X);
					vector.Y = MathUtils.Floor(vector.Y);
					vector.Z = MathUtils.Floor(vector.Z);
					vector2.X = MathUtils.Ceiling(vector2.X);
					vector2.Y = MathUtils.Ceiling(vector2.Y);
					vector2.Z = MathUtils.Ceiling(vector2.Z);
				}
				return new BoundingBox(vector, vector2);
			}

			// Token: 0x06001E83 RID: 7811 RVA: 0x000DF4D0 File Offset: 0x000DD6D0
			void IMovingBlockSet.SetBlock(Point3 offset, int value)
			{
				this.Blocks.RemoveAll((MovingBlock b) => b.Offset == offset);
				if (value != 0)
				{
					this.Blocks.Add(new MovingBlock
					{
						Offset = offset,
						Value = value
					});
				}
				this.UpdateBox();
				this.GeometryGenerationPosition = new Point3(int.MaxValue);
			}

			// Token: 0x06001E84 RID: 7812 RVA: 0x000DF544 File Offset: 0x000DD744
			void IMovingBlockSet.Stop()
			{
				this.Stop = true;
			}

			// Token: 0x040015B3 RID: 5555
			public string Id;

			// Token: 0x040015B4 RID: 5556
			public object Tag;

			// Token: 0x040015B5 RID: 5557
			public bool Stop;

			// Token: 0x040015B6 RID: 5558
			public int RemainCounter;

			// Token: 0x040015B7 RID: 5559
			public Vector3 Position;

			// Token: 0x040015B8 RID: 5560
			public Vector3 StartPosition;

			// Token: 0x040015B9 RID: 5561
			public Vector3 TargetPosition;

			// Token: 0x040015BA RID: 5562
			public float Speed;

			// Token: 0x040015BB RID: 5563
			public float Acceleration;

			// Token: 0x040015BC RID: 5564
			public float Drag;

			// Token: 0x040015BD RID: 5565
			public Vector2 Smoothness;

			// Token: 0x040015BE RID: 5566
			public List<MovingBlock> Blocks;

			// Token: 0x040015BF RID: 5567
			public Box Box;

			// Token: 0x040015C0 RID: 5568
			public Vector3 CurrentVelocity;

			// Token: 0x040015C1 RID: 5569
			public TerrainGeometry Geometry;

			// Token: 0x040015C2 RID: 5570
			public DynamicArray<TerrainVertex> Vertices = new DynamicArray<TerrainVertex>();

			// Token: 0x040015C3 RID: 5571
			public DynamicArray<ushort> Indices = new DynamicArray<ushort>();

			// Token: 0x040015C4 RID: 5572
			public Vector3 GeometryOffset;

			// Token: 0x040015C5 RID: 5573
			public Point3 GeometryGenerationPosition = new Point3(int.MaxValue);
		}
	}
}
