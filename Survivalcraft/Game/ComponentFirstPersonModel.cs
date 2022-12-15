using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D8 RID: 472
	public class ComponentFirstPersonModel : Component, IDrawable, IUpdateable
	{
		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000CC8 RID: 3272 RVA: 0x0005FE18 File Offset: 0x0005E018
		// (set) Token: 0x06000CC9 RID: 3273 RVA: 0x0005FE20 File Offset: 0x0005E020
		public Vector3 ItemOffsetOrder { get; set; }

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000CCA RID: 3274 RVA: 0x0005FE29 File Offset: 0x0005E029
		// (set) Token: 0x06000CCB RID: 3275 RVA: 0x0005FE31 File Offset: 0x0005E031
		public Vector3 ItemRotationOrder { get; set; }

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06000CCC RID: 3276 RVA: 0x0005FE3A File Offset: 0x0005E03A
		public int[] DrawOrders
		{
			get
			{
				return ComponentFirstPersonModel.m_drawOrders;
			}
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06000CCD RID: 3277 RVA: 0x0005FE41 File Offset: 0x0005E041
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.FirstPersonModels;
			}
		}

		// Token: 0x06000CCE RID: 3278 RVA: 0x0005FE48 File Offset: 0x0005E048
		public void Draw(Camera camera, int drawOrder)
		{
			if (this.m_componentPlayer.ComponentHealth.Health > 0f && camera.GameWidget.IsEntityFirstPersonTarget(base.Entity) && !this.m_componentPlayer.ComponentInput.IsControlledByVr)
			{
				Viewport viewport = Display.Viewport;
				Viewport viewport2 = viewport;
				viewport2.MaxDepth *= 0.1f;
				Display.Viewport = viewport2;
				try
				{
					Matrix matrix = Matrix.Identity;
					if (this.m_swapAnimationTime > 0f)
					{
						float num = MathUtils.Pow(MathUtils.Sin(this.m_swapAnimationTime * 3.1415927f), 3f);
						matrix *= Matrix.CreateTranslation(0f, -0.8f * num, 0.2f * num);
					}
					if (this.m_pokeAnimationTime > 0f)
					{
						float num2 = MathUtils.Sin(MathUtils.Sqrt(this.m_pokeAnimationTime) * 3.1415927f);
						if (this.m_value != 0)
						{
							matrix *= Matrix.CreateRotationX((0f - MathUtils.DegToRad(90f)) * num2);
							matrix *= Matrix.CreateTranslation(-0.5f * num2, 0.1f * num2, 0f * num2);
						}
						else
						{
							matrix *= Matrix.CreateRotationX((0f - MathUtils.DegToRad(45f)) * num2);
							matrix *= Matrix.CreateTranslation(-0.1f * num2, 0.2f * num2, -0.05f * num2);
						}
					}
					if (this.m_componentRider.Mount != null)
					{
						ComponentCreatureModel componentCreatureModel = this.m_componentRider.Mount.Entity.FindComponent<ComponentCreatureModel>();
						if (componentCreatureModel != null)
						{
							float num3 = componentCreatureModel.MovementAnimationPhase * 3.1415927f * 2f + 0.5f;
							Vector3 position = default(Vector3);
							position.Y = 0.02f * MathUtils.Sin(num3);
							position.Z = 0.02f * MathUtils.Sin(num3);
							matrix *= Matrix.CreateRotationX(0.05f * MathUtils.Sin(num3 * 1f)) * Matrix.CreateTranslation(position);
						}
					}
					else
					{
						float num4 = this.m_componentPlayer.ComponentCreatureModel.MovementAnimationPhase * 3.1415927f * 2f;
						Vector3 vector = default(Vector3);
						vector.X = 0.03f * MathUtils.Sin(num4 * 1f);
						vector.Y = 0.02f * MathUtils.Sin(num4 * 2f);
						vector.Z = 0.02f * MathUtils.Sin(num4 * 1f);
						matrix *= Matrix.CreateRotationZ(1f * vector.X) * Matrix.CreateTranslation(vector);
					}
					Vector3 eyePosition = this.m_componentPlayer.ComponentCreatureModel.EyePosition;
					int x = Terrain.ToCell(eyePosition.X);
					int num5 = Terrain.ToCell(eyePosition.Y);
					int z = Terrain.ToCell(eyePosition.Z);
					Matrix m = Matrix.CreateFromQuaternion(this.m_componentPlayer.ComponentCreatureModel.EyeRotation);
					m.Translation = this.m_componentPlayer.ComponentCreatureModel.EyePosition;
					if (this.m_value != 0)
					{
						if (num5 >= 0 && num5 <= 255)
						{
							TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(x, z);
							if (chunkAtCell != null && chunkAtCell.State >= TerrainChunkState.InvalidVertices1)
							{
								this.m_itemLight = this.m_subsystemTerrain.Terrain.GetCellLightFast(x, num5, z);
							}
						}
						int num6 = Terrain.ExtractContents(this.m_value);
						Block block = BlocksManager.Blocks[num6];
						Vector3 vector2 = block.FirstPersonRotation * 0.017453292f + this.m_itemRotation;
						Vector3 vector3 = block.FirstPersonOffset + this.m_itemOffset;
						vector3 += this.m_itemOffset;
						Matrix inWorldMatrix = Matrix.CreateFromYawPitchRoll(vector2.Y, vector2.X, vector2.Z) * matrix * Matrix.CreateTranslation(vector3) * Matrix.CreateFromYawPitchRoll(this.m_lagAngles.X, this.m_lagAngles.Y, 0f) * m;
						this.m_drawBlockEnvironmentData.SubsystemTerrain = this.m_subsystemTerrain;
						this.m_drawBlockEnvironmentData.InWorldMatrix = inWorldMatrix;
						this.m_drawBlockEnvironmentData.Light = this.m_itemLight;
						this.m_drawBlockEnvironmentData.Humidity = this.m_subsystemTerrain.Terrain.GetSeasonalHumidity(x, z);
						this.m_drawBlockEnvironmentData.Temperature = this.m_subsystemTerrain.Terrain.GetSeasonalTemperature(x, z) + SubsystemWeather.GetTemperatureAdjustmentAtHeight(num5);
						block.DrawBlock(this.m_primitivesRenderer, this.m_value, Color.White, block.FirstPersonScale, ref inWorldMatrix, this.m_drawBlockEnvironmentData);
						this.m_primitivesRenderer.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
					}
					else
					{
						if (Time.FrameStartTime >= this.m_nextHandLightTime)
						{
							float? num7 = LightingManager.CalculateSmoothLight(this.m_subsystemTerrain, eyePosition);
							if (num7 != null)
							{
								this.m_nextHandLightTime = Time.FrameStartTime + 0.1;
								this.m_handLight = num7.Value;
							}
						}
						Vector3 position2 = new Vector3(0.25f, -0.3f, -0.05f);
						Matrix matrix2 = Matrix.CreateScale(0.01f) * Matrix.CreateRotationX(0.8f) * Matrix.CreateRotationY(0.4f) * matrix * Matrix.CreateTranslation(position2) * Matrix.CreateFromYawPitchRoll(this.m_lagAngles.X, this.m_lagAngles.Y, 0f) * m * camera.ViewMatrix;
						Display.DepthStencilState = DepthStencilState.Default;
						Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
						ComponentFirstPersonModel.m_shader.Texture = this.m_componentPlayer.ComponentCreatureModel.TextureOverride;
						ComponentFirstPersonModel.m_shader.SamplerState = SamplerState.PointClamp;
						ComponentFirstPersonModel.m_shader.MaterialColor = Vector4.One;
						ComponentFirstPersonModel.m_shader.AmbientLightColor = new Vector3(this.m_handLight * LightingManager.LightAmbient);
						ComponentFirstPersonModel.m_shader.DiffuseLightColor1 = new Vector3(this.m_handLight);
						ComponentFirstPersonModel.m_shader.DiffuseLightColor2 = new Vector3(this.m_handLight);
						ComponentFirstPersonModel.m_shader.LightDirection1 = Vector3.TransformNormal(LightingManager.DirectionToLight1, camera.ViewMatrix);
						ComponentFirstPersonModel.m_shader.LightDirection2 = Vector3.TransformNormal(LightingManager.DirectionToLight2, camera.ViewMatrix);
						ComponentFirstPersonModel.m_shader.Transforms.World[0] = matrix2;
						ComponentFirstPersonModel.m_shader.Transforms.View = Matrix.Identity;
						ComponentFirstPersonModel.m_shader.Transforms.Projection = camera.ProjectionMatrix;
						foreach (ModelMesh modelMesh in this.m_handModel.Meshes)
						{
							foreach (ModelMeshPart modelMeshPart in modelMesh.MeshParts)
							{
								Display.DrawIndexed(PrimitiveType.TriangleList, ComponentFirstPersonModel.m_shader, modelMeshPart.VertexBuffer, modelMeshPart.IndexBuffer, modelMeshPart.StartIndex, modelMeshPart.IndicesCount);
							}
						}
					}
				}
				finally
				{
					Display.Viewport = viewport;
				}
			}
		}

		// Token: 0x06000CCF RID: 3279 RVA: 0x000605E8 File Offset: 0x0005E7E8
		public void Update(float dt)
		{
			Vector3 vector = this.m_componentPlayer.ComponentCreatureModel.EyeRotation.ToYawPitchRoll();
			this.m_lagAngles *= MathUtils.Pow(0.2f, dt);
			if (this.m_lastYpr != null)
			{
				Vector3 vector2 = vector - this.m_lastYpr.Value;
				this.m_lagAngles.X = MathUtils.Clamp(this.m_lagAngles.X - 0.08f * MathUtils.NormalizeAngle(vector2.X), -0.1f, 0.1f);
				this.m_lagAngles.Y = MathUtils.Clamp(this.m_lagAngles.Y - 0.08f * MathUtils.NormalizeAngle(vector2.Y), -0.1f, 0.1f);
			}
			this.m_lastYpr = new Vector3?(vector);
			int activeBlockValue = this.m_componentMiner.ActiveBlockValue;
			if (this.m_swapAnimationTime == 0f && activeBlockValue != this.m_value)
			{
				if (BlocksManager.Blocks[Terrain.ExtractContents(activeBlockValue)].IsSwapAnimationNeeded(this.m_value, activeBlockValue))
				{
					this.m_swapAnimationTime = 0.0001f;
				}
				else
				{
					this.m_value = activeBlockValue;
				}
			}
			if (this.m_swapAnimationTime > 0f)
			{
				float swapAnimationTime = this.m_swapAnimationTime;
				this.m_swapAnimationTime += 2f * dt;
				if (swapAnimationTime < 0.5f && this.m_swapAnimationTime >= 0.5f)
				{
					this.m_value = activeBlockValue;
				}
				if (this.m_swapAnimationTime > 1f)
				{
					this.m_swapAnimationTime = 0f;
				}
			}
			this.m_pokeAnimationTime = this.m_componentMiner.PokingPhase;
			this.m_itemOffset = Vector3.Lerp(this.m_itemOffset, this.ItemOffsetOrder, MathUtils.Saturate(10f * dt));
			this.m_itemRotation = Vector3.Lerp(this.m_itemRotation, this.ItemRotationOrder, MathUtils.Saturate(10f * dt));
			this.ItemOffsetOrder = Vector3.Zero;
			this.ItemRotationOrder = Vector3.Zero;
		}

        // Token: 0x06000CD0 RID: 3280 RVA: 0x000607E0 File Offset: 0x0005E9E0
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.m_componentRider = base.Entity.FindComponent<ComponentRider>(true);
			this.m_componentMiner = base.Entity.FindComponent<ComponentMiner>(true);
			this.m_handModel = ContentManager.Get<Model>(valuesDictionary.GetValue<string>("HandModelName"));
		}

		// Token: 0x0400078C RID: 1932
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400078D RID: 1933
		public ComponentMiner m_componentMiner;

		// Token: 0x0400078E RID: 1934
		public ComponentPlayer m_componentPlayer;

		// Token: 0x0400078F RID: 1935
		public ComponentRider m_componentRider;

		// Token: 0x04000790 RID: 1936
		public int m_value;

		// Token: 0x04000791 RID: 1937
		public Model m_handModel;

		// Token: 0x04000792 RID: 1938
		public Vector3? m_lastYpr;

		// Token: 0x04000793 RID: 1939
		public Vector2 m_lagAngles;

		// Token: 0x04000794 RID: 1940
		public float m_swapAnimationTime;

		// Token: 0x04000795 RID: 1941
		public float m_pokeAnimationTime;

		// Token: 0x04000796 RID: 1942
		public Vector3 m_itemOffset;

		// Token: 0x04000797 RID: 1943
		public Vector3 m_itemRotation;

		// Token: 0x04000798 RID: 1944
		public double m_nextHandLightTime;

		// Token: 0x04000799 RID: 1945
		public float m_handLight;

		// Token: 0x0400079A RID: 1946
		public int m_itemLight;

		// Token: 0x0400079B RID: 1947
		public DrawBlockEnvironmentData m_drawBlockEnvironmentData = new DrawBlockEnvironmentData();

		// Token: 0x0400079C RID: 1948
		public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

		// Token: 0x0400079D RID: 1949
		public static LitShader m_shader = new LitShader(2, false, false, true, false, false, 1);

		// Token: 0x0400079E RID: 1950
		public static int[] m_drawOrders = new int[]
		{
			1
		};
	}
}
