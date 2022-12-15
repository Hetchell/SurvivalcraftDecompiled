using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000317 RID: 791
	public class TerrainRenderer : IDisposable
	{
		// Token: 0x1700036C RID: 876
		// (get) Token: 0x060016A5 RID: 5797 RVA: 0x000B37A0 File Offset: 0x000B19A0
		public string ChunksGpuMemoryUsage
		{
			get
			{
				long num = 0L;
				foreach (TerrainChunk terrainChunk in this.m_subsystemTerrain.Terrain.AllocatedChunks)
				{
					if (terrainChunk.Geometry != null)
					{
						foreach (TerrainChunkGeometry.Buffer buffer in terrainChunk.Geometry.Buffers)
						{
							long num2 = num;
							VertexBuffer vertexBuffer = buffer.VertexBuffer;
							num = num2 + (long)((vertexBuffer != null) ? vertexBuffer.GetGpuMemoryUsage() : 0);
							long num3 = num;
							IndexBuffer indexBuffer = buffer.IndexBuffer;
							num = num3 + (long)((indexBuffer != null) ? indexBuffer.GetGpuMemoryUsage() : 0);
						}
					}
				}
				return string.Format("{0:0.0}MB", num / 1024L / 1024L);
			}
		}

		// Token: 0x060016A6 RID: 5798 RVA: 0x000B3870 File Offset: 0x000B1A70
		public TerrainRenderer(SubsystemTerrain subsystemTerrain)
		{
			this.m_subsystemTerrain = subsystemTerrain;
			this.m_subsystemSky = subsystemTerrain.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemAnimatedTextures = subsystemTerrain.SubsystemAnimatedTextures;
			this.m_opaqueShader = ContentManager.Get<Shader>("Shaders/Opaque");
			this.m_alphaTestedShader = ContentManager.Get<Shader>("Shaders/AlphaTested");
			this.m_transparentShader = ContentManager.Get<Shader>("Shaders/Transparent");
			Display.DeviceReset += this.Display_DeviceReset;
		}

		// Token: 0x060016A7 RID: 5799 RVA: 0x000B394C File Offset: 0x000B1B4C
		public void DisposeTerrainChunkGeometryVertexIndexBuffers(TerrainChunkGeometry geometry)
		{
			foreach (TerrainChunkGeometry.Buffer buffer in geometry.Buffers)
			{
				buffer.Dispose();
			}
			geometry.Buffers.Clear();
			geometry.InvalidateSliceContentsHashes();
		}

		// Token: 0x060016A8 RID: 5800 RVA: 0x000B39B0 File Offset: 0x000B1BB0
		public void PrepareForDrawing(Camera camera)
		{
			Vector2 xz = camera.ViewPosition.XZ;
			float num = MathUtils.Sqr(this.m_subsystemSky.VisibilityRange);
			BoundingFrustum viewFrustum = camera.ViewFrustum;
			int gameWidgetIndex = camera.GameWidget.GameWidgetIndex;
			this.m_chunksToDraw.Clear();
			foreach (TerrainChunk terrainChunk in this.m_subsystemTerrain.Terrain.AllocatedChunks)
			{
				if (terrainChunk.NewGeometryData)
				{
					TerrainChunkGeometry geometry = terrainChunk.Geometry;
					lock (geometry)
					{
						if (terrainChunk.NewGeometryData)
						{
							terrainChunk.NewGeometryData = false;
							this.SetupTerrainChunkGeometryVertexIndexBuffers(terrainChunk);
						}
					}
				}
				terrainChunk.DrawDistanceSquared = Vector2.DistanceSquared(xz, terrainChunk.Center);
				if (terrainChunk.DrawDistanceSquared <= num)
				{
					if (viewFrustum.Intersection(terrainChunk.BoundingBox))
					{
						this.m_chunksToDraw.Add(terrainChunk);
					}
					if (terrainChunk.State == TerrainChunkState.Valid)
					{
						float num2 = terrainChunk.FogEnds[gameWidgetIndex];
						if (num2 != 3.4028235E+38f)
						{
							if (num2 == 0f)
							{
								this.StartChunkFadeIn(camera, terrainChunk);
							}
							else
							{
								this.RunChunkFadeIn(camera, terrainChunk);
							}
						}
					}
				}
				else
				{
					terrainChunk.FogEnds[gameWidgetIndex] = 0f;
				}
			}
			TerrainRenderer.ChunksDrawn = 0;
			TerrainRenderer.ChunkDrawCalls = 0;
			TerrainRenderer.ChunkTrianglesDrawn = 0;
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x000B3B24 File Offset: 0x000B1D24
		public void DrawOpaque(Camera camera)
		{
			int gameWidgetIndex = camera.GameWidget.GameWidgetIndex;
			Vector3 viewPosition = camera.ViewPosition;
			Vector3 v = new Vector3(MathUtils.Floor(viewPosition.X), 0f, MathUtils.Floor(viewPosition.Z));
			Matrix value = Matrix.CreateTranslation(v - viewPosition) * camera.ViewMatrix.OrientationMatrix * camera.ProjectionMatrix;
			Display.BlendState = BlendState.Opaque;
			Display.DepthStencilState = DepthStencilState.Default;
			Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
			this.m_opaqueShader.GetParameter("u_origin", false).SetValue(v.XZ);
			this.m_opaqueShader.GetParameter("u_viewProjectionMatrix", false).SetValue(value);
			this.m_opaqueShader.GetParameter("u_viewPosition", false).SetValue(viewPosition);
			this.m_opaqueShader.GetParameter("u_texture", false).SetValue(this.m_subsystemAnimatedTextures.AnimatedBlocksTexture);
			this.m_opaqueShader.GetParameter("u_samplerState", false).SetValue(SettingsManager.TerrainMipmapsEnabled ? this.m_samplerStateMips : this.m_samplerState);
			this.m_opaqueShader.GetParameter("u_fogYMultiplier", false).SetValue(this.m_subsystemSky.VisibilityRangeYMultiplier);
			this.m_opaqueShader.GetParameter("u_fogColor", false).SetValue(new Vector3(this.m_subsystemSky.ViewFogColor));
			ShaderParameter parameter = this.m_opaqueShader.GetParameter("u_fogStartInvLength", false);
			for (int i = 0; i < this.m_chunksToDraw.Count; i++)
			{
				TerrainChunk terrainChunk = this.m_chunksToDraw[i];
				float num = MathUtils.Min(terrainChunk.FogEnds[gameWidgetIndex], this.m_subsystemSky.ViewFogRange.Y);
				float num2 = MathUtils.Min(this.m_subsystemSky.ViewFogRange.X, num - 1f);
				parameter.SetValue(new Vector2(num2, 1f / (num - num2)));
				int num3 = 16;
				if (viewPosition.Z > terrainChunk.BoundingBox.Min.Z)
				{
					num3 |= 1;
				}
				if (viewPosition.X > terrainChunk.BoundingBox.Min.X)
				{
					num3 |= 2;
				}
				if (viewPosition.Z < terrainChunk.BoundingBox.Max.Z)
				{
					num3 |= 4;
				}
				if (viewPosition.X < terrainChunk.BoundingBox.Max.X)
				{
					num3 |= 8;
				}
				this.DrawTerrainChunkGeometrySubsets(this.m_opaqueShader, terrainChunk.Geometry, num3);
				TerrainRenderer.ChunksDrawn++;
			}
		}

		// Token: 0x060016AA RID: 5802 RVA: 0x000B3DCC File Offset: 0x000B1FCC
		public void DrawAlphaTested(Camera camera)
		{
			int gameWidgetIndex = camera.GameWidget.GameWidgetIndex;
			Vector3 viewPosition = camera.ViewPosition;
			Vector3 v = new Vector3(MathUtils.Floor(viewPosition.X), 0f, MathUtils.Floor(viewPosition.Z));
			Matrix value = Matrix.CreateTranslation(v - viewPosition) * camera.ViewMatrix.OrientationMatrix * camera.ProjectionMatrix;
			Display.BlendState = BlendState.Opaque;
			Display.DepthStencilState = DepthStencilState.Default;
			Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
			this.m_alphaTestedShader.GetParameter("u_origin", false).SetValue(v.XZ);
			this.m_alphaTestedShader.GetParameter("u_viewProjectionMatrix", false).SetValue(value);
			this.m_alphaTestedShader.GetParameter("u_viewPosition", false).SetValue(viewPosition);
			this.m_alphaTestedShader.GetParameter("u_texture", false).SetValue(this.m_subsystemAnimatedTextures.AnimatedBlocksTexture);
			this.m_alphaTestedShader.GetParameter("u_samplerState", false).SetValue(SettingsManager.TerrainMipmapsEnabled ? this.m_samplerStateMips : this.m_samplerState);
			this.m_alphaTestedShader.GetParameter("u_fogYMultiplier", false).SetValue(this.m_subsystemSky.VisibilityRangeYMultiplier);
			this.m_alphaTestedShader.GetParameter("u_fogColor", false).SetValue(new Vector3(this.m_subsystemSky.ViewFogColor));
			ShaderParameter parameter = this.m_alphaTestedShader.GetParameter("u_fogStartInvLength", false);
			for (int i = 0; i < this.m_chunksToDraw.Count; i++)
			{
				TerrainChunk terrainChunk = this.m_chunksToDraw[i];
				float num = MathUtils.Min(terrainChunk.FogEnds[gameWidgetIndex], this.m_subsystemSky.ViewFogRange.Y);
				float num2 = MathUtils.Min(this.m_subsystemSky.ViewFogRange.X, num - 1f);
				parameter.SetValue(new Vector2(num2, 1f / (num - num2)));
				int subsetsMask = 32;
				this.DrawTerrainChunkGeometrySubsets(this.m_alphaTestedShader, terrainChunk.Geometry, subsetsMask);
			}
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x000B3FEC File Offset: 0x000B21EC
		public void DrawTransparent(Camera camera)
		{
			int gameWidgetIndex = camera.GameWidget.GameWidgetIndex;
			Vector3 viewPosition = camera.ViewPosition;
			Vector3 v = new Vector3(MathUtils.Floor(viewPosition.X), 0f, MathUtils.Floor(viewPosition.Z));
			Matrix value = Matrix.CreateTranslation(v - viewPosition) * camera.ViewMatrix.OrientationMatrix * camera.ProjectionMatrix;
			Display.BlendState = BlendState.AlphaBlend;
			Display.DepthStencilState = DepthStencilState.Default;
			Display.RasterizerState = ((this.m_subsystemSky.ViewUnderWaterDepth > 0f) ? RasterizerState.CullClockwiseScissor : RasterizerState.CullCounterClockwiseScissor);
			this.m_transparentShader.GetParameter("u_origin", false).SetValue(v.XZ);
			this.m_transparentShader.GetParameter("u_viewProjectionMatrix", false).SetValue(value);
			this.m_transparentShader.GetParameter("u_viewPosition", false).SetValue(viewPosition);
			this.m_transparentShader.GetParameter("u_texture", false).SetValue(this.m_subsystemAnimatedTextures.AnimatedBlocksTexture);
			this.m_transparentShader.GetParameter("u_samplerState", false).SetValue(SettingsManager.TerrainMipmapsEnabled ? this.m_samplerStateMips : this.m_samplerState);
			this.m_transparentShader.GetParameter("u_fogYMultiplier", false).SetValue(this.m_subsystemSky.VisibilityRangeYMultiplier);
			this.m_transparentShader.GetParameter("u_fogColor", false).SetValue(new Vector3(this.m_subsystemSky.ViewFogColor));
			ShaderParameter parameter = this.m_transparentShader.GetParameter("u_fogStartInvLength", false);
			for (int i = 0; i < this.m_chunksToDraw.Count; i++)
			{
				TerrainChunk terrainChunk = this.m_chunksToDraw[i];
				float num = MathUtils.Min(terrainChunk.FogEnds[gameWidgetIndex], this.m_subsystemSky.ViewFogRange.Y);
				float num2 = MathUtils.Min(this.m_subsystemSky.ViewFogRange.X, num - 1f);
				parameter.SetValue(new Vector2(num2, 1f / (num - num2)));
				int subsetsMask = 64;
				this.DrawTerrainChunkGeometrySubsets(this.m_transparentShader, terrainChunk.Geometry, subsetsMask);
			}
		}

		// Token: 0x060016AC RID: 5804 RVA: 0x000B4222 File Offset: 0x000B2422
		public void Dispose()
		{
			Display.DeviceReset -= this.Display_DeviceReset;
		}

		// Token: 0x060016AD RID: 5805 RVA: 0x000B4238 File Offset: 0x000B2438
		public void Display_DeviceReset()
		{
			this.m_subsystemTerrain.TerrainUpdater.DowngradeAllChunksState(TerrainChunkState.InvalidVertices1, false);
			foreach (TerrainChunk terrainChunk in this.m_subsystemTerrain.Terrain.AllocatedChunks)
			{
				this.DisposeTerrainChunkGeometryVertexIndexBuffers(terrainChunk.Geometry);
			}
		}

		// Token: 0x060016AE RID: 5806 RVA: 0x000B4288 File Offset: 0x000B2488
		public void SetupTerrainChunkGeometryVertexIndexBuffers(TerrainChunk chunk)
		{
			TerrainChunkGeometry geometry = chunk.Geometry;
			this.DisposeTerrainChunkGeometryVertexIndexBuffers(geometry);
			int j;
			for (int i = 0; i < 112; i = j)
			{
				int num = 0;
				int num2 = 0;
				for (j = i; j < 112; j++)
				{
					int num3 = j / 16;
					int num4 = j % 16;
					TerrainGeometrySubset terrainGeometrySubset = geometry.Slices[num4].Subsets[num3];
					if (num + terrainGeometrySubset.Vertices.Count > 65535 && j > i)
					{
						break;
					}
					num += terrainGeometrySubset.Vertices.Count;
					num2 += terrainGeometrySubset.Indices.Count;
				}
				if (num > 65535)
				{
					Log.Warning("Max vertices count exceeded around ({0},{1},{2}), geometry will be corrupted ({3}/{4} vertices).", new object[]
					{
						chunk.Origin.X,
						j % 16 * 16,
						chunk.Origin.Y,
						num,
						65535
					});
				}
				if (num > 0 && num2 > 0)
				{
					TerrainChunkGeometry.Buffer buffer = new TerrainChunkGeometry.Buffer();
					geometry.Buffers.Add(buffer);
					buffer.VertexBuffer = new VertexBuffer(TerrainVertex.VertexDeclaration, num);
					buffer.IndexBuffer = new IndexBuffer(IndexFormat.SixteenBits, num2);
					int num5 = 0;
					int num6 = 0;
					for (int k = i; k < j; k++)
					{
						int num7 = k / 16;
						int num8 = k % 16;
						TerrainGeometrySubset terrainGeometrySubset2 = geometry.Slices[num8].Subsets[num7];
						if (num8 == 0 || k == i)
						{
							buffer.SubsetIndexBufferStarts[num7] = num6;
						}
						if (terrainGeometrySubset2.Indices.Count > 0)
						{
							TerrainRenderer.m_tmpIndices.Count = terrainGeometrySubset2.Indices.Count;
							TerrainRenderer.ShiftIndices(terrainGeometrySubset2.Indices.Array, TerrainRenderer.m_tmpIndices.Array, num5, terrainGeometrySubset2.Indices.Count);
							buffer.IndexBuffer.SetData<ushort>(TerrainRenderer.m_tmpIndices.Array, 0, TerrainRenderer.m_tmpIndices.Count, num6);
							num6 += TerrainRenderer.m_tmpIndices.Count;
						}
						if (terrainGeometrySubset2.Vertices.Count > 0)
						{
							buffer.VertexBuffer.SetData<TerrainVertex>(terrainGeometrySubset2.Vertices.Array, 0, terrainGeometrySubset2.Vertices.Count, num5);
							num5 += terrainGeometrySubset2.Vertices.Count;
						}
						if (num8 == 15 || k == j - 1)
						{
							buffer.SubsetIndexBufferEnds[num7] = num6;
						}
					}
				}
			}
			geometry.CopySliceContentsHashes(chunk);
		}

		// Token: 0x060016AF RID: 5807 RVA: 0x000B4504 File Offset: 0x000B2704
		public void DrawTerrainChunkGeometrySubsets(Shader shader, TerrainChunkGeometry geometry, int subsetsMask)
		{
			foreach (TerrainChunkGeometry.Buffer buffer in geometry.Buffers)
			{
				int num = int.MaxValue;
				int num2 = 0;
				for (int i = 0; i < 8; i++)
				{
					if (i < 7 && (subsetsMask & 1 << i) != 0)
					{
						if (buffer.SubsetIndexBufferEnds[i] > 0)
						{
							if (num == 2147483647)
							{
								num = buffer.SubsetIndexBufferStarts[i];
							}
							num2 = buffer.SubsetIndexBufferEnds[i];
						}
					}
					else
					{
						if (num2 > num)
						{
							Display.DrawIndexed(PrimitiveType.TriangleList, shader, buffer.VertexBuffer, buffer.IndexBuffer, num, num2 - num);
							TerrainRenderer.ChunkTrianglesDrawn += (num2 - num) / 3;
							TerrainRenderer.ChunkDrawCalls++;
						}
						num = int.MaxValue;
					}
				}
			}
		}

		// Token: 0x060016B0 RID: 5808 RVA: 0x000B45E8 File Offset: 0x000B27E8
		public void StartChunkFadeIn(Camera camera, TerrainChunk chunk)
		{
			Vector3 viewPosition = camera.ViewPosition;
			Vector2 v = new Vector2((float)chunk.Origin.X, (float)chunk.Origin.Y);
			Vector2 v2 = new Vector2((float)(chunk.Origin.X + 16), (float)chunk.Origin.Y);
			Vector2 v3 = new Vector2((float)chunk.Origin.X, (float)(chunk.Origin.Y + 16));
			Vector2 v4 = new Vector2((float)(chunk.Origin.X + 16), (float)(chunk.Origin.Y + 16));
			float x = Vector2.Distance(viewPosition.XZ, v);
			float x2 = Vector2.Distance(viewPosition.XZ, v2);
			float x3 = Vector2.Distance(viewPosition.XZ, v3);
			float x4 = Vector2.Distance(viewPosition.XZ, v4);
			chunk.FogEnds[camera.GameWidget.GameWidgetIndex] = MathUtils.Max(MathUtils.Min(x, x2, x3, x4), 0.001f);
		}

		// Token: 0x060016B1 RID: 5809 RVA: 0x000B46EC File Offset: 0x000B28EC
		public void RunChunkFadeIn(Camera camera, TerrainChunk chunk)
		{
			chunk.FogEnds[camera.GameWidget.GameWidgetIndex] += 32f * Time.FrameDuration;
			if (chunk.FogEnds[camera.GameWidget.GameWidgetIndex] >= this.m_subsystemSky.ViewFogRange.Y)
			{
				chunk.FogEnds[camera.GameWidget.GameWidgetIndex] = float.MaxValue;
			}
		}

		// Token: 0x060016B2 RID: 5810 RVA: 0x000B475C File Offset: 0x000B295C
		public static void ShiftIndices(ushort[] source, ushort[] destination, int shift, int count)
		{
			for (int i = 0; i < count; i++)
			{
				destination[i] = (ushort)((int)source[i] + shift);
			}
		}

		// Token: 0x04001056 RID: 4182
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04001057 RID: 4183
		public SubsystemSky m_subsystemSky;

		// Token: 0x04001058 RID: 4184
		public SubsystemAnimatedTextures m_subsystemAnimatedTextures;

		// Token: 0x04001059 RID: 4185
		public Shader m_opaqueShader;

		// Token: 0x0400105A RID: 4186
		public Shader m_alphaTestedShader;

		// Token: 0x0400105B RID: 4187
		public Shader m_transparentShader;

		// Token: 0x0400105C RID: 4188
		public SamplerState m_samplerState = new SamplerState
		{
			AddressModeU = TextureAddressMode.Clamp,
			AddressModeV = TextureAddressMode.Clamp,
			FilterMode = TextureFilterMode.Point,
			MaxLod = 0f
		};

		// Token: 0x0400105D RID: 4189
		public SamplerState m_samplerStateMips = new SamplerState
		{
			AddressModeU = TextureAddressMode.Clamp,
			AddressModeV = TextureAddressMode.Clamp,
			FilterMode = TextureFilterMode.PointMipLinear,
			MaxLod = 4f
		};

		// Token: 0x0400105E RID: 4190
		public DynamicArray<TerrainChunk> m_chunksToDraw = new DynamicArray<TerrainChunk>();

		// Token: 0x0400105F RID: 4191
		public static DynamicArray<ushort> m_tmpIndices = new DynamicArray<ushort>();

		// Token: 0x04001060 RID: 4192
		public static bool DrawChunksMap;

		// Token: 0x04001061 RID: 4193
		public static int ChunksDrawn;

		// Token: 0x04001062 RID: 4194
		public static int ChunkDrawCalls;

		// Token: 0x04001063 RID: 4195
		public static int ChunkTrianglesDrawn;
	}
}
