using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200020F RID: 527
	public class ComponentVrHandsModel : Component, IDrawable, IUpdateable
	{
		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06001048 RID: 4168 RVA: 0x0007C8F6 File Offset: 0x0007AAF6
		// (set) Token: 0x06001049 RID: 4169 RVA: 0x0007C8FE File Offset: 0x0007AAFE
		public Vector3 ItemOffsetOrder { get; set; }

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x0600104A RID: 4170 RVA: 0x0007C907 File Offset: 0x0007AB07
		// (set) Token: 0x0600104B RID: 4171 RVA: 0x0007C90F File Offset: 0x0007AB0F
		public Vector3 ItemRotationOrder { get; set; }

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x0600104C RID: 4172 RVA: 0x0007C918 File Offset: 0x0007AB18
		public int[] DrawOrders
		{
			get
			{
				return ComponentVrHandsModel.m_drawOrders;
			}
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x0600104D RID: 4173 RVA: 0x0007C91F File Offset: 0x0007AB1F
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.FirstPersonModels;
			}
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x0007C924 File Offset: 0x0007AB24
		public void Draw(Camera camera, int drawOrder)
		{
			if (this.m_componentPlayer.ComponentHealth.Health <= 0f || !camera.GameWidget.IsEntityFirstPersonTarget(base.Entity) || !this.m_componentPlayer.ComponentInput.IsControlledByVr)
			{
				return;
			}
			Vector3 eyePosition = this.m_componentPlayer.ComponentCreatureModel.EyePosition;
			int x = Terrain.ToCell(eyePosition.X);
			int num = Terrain.ToCell(eyePosition.Y);
			int z = Terrain.ToCell(eyePosition.Z);
			int activeBlockValue = this.m_componentMiner.ActiveBlockValue;
			if (Time.FrameStartTime >= this.m_nextHandLightTime)
			{
				float? num2 = LightingManager.CalculateSmoothLight(this.m_subsystemTerrain, eyePosition);
				if (num2 != null)
				{
					this.m_nextHandLightTime = Time.FrameStartTime + 0.1;
					this.m_handLight = num2.Value;
				}
			}
			Matrix matrix = Matrix.Identity;
			if (this.m_pokeAnimationTime > 0f)
			{
				float num3 = MathUtils.Sin(MathUtils.Sqrt(this.m_pokeAnimationTime) * 3.1415927f);
				if (activeBlockValue != 0)
				{
					matrix *= Matrix.CreateRotationX((0f - MathUtils.DegToRad(90f)) * num3);
				}
				else
				{
					matrix *= Matrix.CreateRotationX((0f - MathUtils.DegToRad(45f)) * num3);
				}
			}
			if (!VrManager.IsControllerPresent(VrController.Right))
			{
				return;
			}
			Matrix m = VrManager.HmdMatrixInverted * Matrix.CreateWorld(camera.ViewPosition, camera.ViewDirection, camera.ViewUp) * camera.ViewMatrix;
			Matrix controllerMatrix = VrManager.GetControllerMatrix(VrController.Right);
			if (activeBlockValue == 0)
			{
				Display.DepthStencilState = DepthStencilState.Default;
				Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
				ComponentVrHandsModel.m_shader.Texture = this.m_componentPlayer.ComponentCreatureModel.TextureOverride;
				ComponentVrHandsModel.m_shader.SamplerState = SamplerState.PointClamp;
				ComponentVrHandsModel.m_shader.MaterialColor = Vector4.One;
				ComponentVrHandsModel.m_shader.AmbientLightColor = new Vector3(this.m_handLight * LightingManager.LightAmbient);
				ComponentVrHandsModel.m_shader.DiffuseLightColor1 = new Vector3(this.m_handLight);
				ComponentVrHandsModel.m_shader.DiffuseLightColor2 = new Vector3(this.m_handLight);
				ComponentVrHandsModel.m_shader.LightDirection1 = -Vector3.TransformNormal(LightingManager.DirectionToLight1, camera.ViewMatrix);
				ComponentVrHandsModel.m_shader.LightDirection2 = -Vector3.TransformNormal(LightingManager.DirectionToLight2, camera.ViewMatrix);
				ComponentVrHandsModel.m_shader.Transforms.View = Matrix.Identity;
				ComponentVrHandsModel.m_shader.Transforms.Projection = camera.ProjectionMatrix;
				ComponentVrHandsModel.m_shader.Transforms.World[0] = Matrix.CreateScale(0.01f) * matrix * controllerMatrix * m;
				using (ReadOnlyList<ModelMesh>.Enumerator enumerator = this.m_vrHandModel.Meshes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ModelMesh modelMesh = enumerator.Current;
						foreach (ModelMeshPart modelMeshPart in modelMesh.MeshParts)
						{
							Display.DrawIndexed(PrimitiveType.TriangleList, ComponentVrHandsModel.m_shader, modelMeshPart.VertexBuffer, modelMeshPart.IndexBuffer, modelMeshPart.StartIndex, modelMeshPart.IndicesCount);
						}
					}
					goto IL_49E;
				}
			}
			if (num >= 0 && num <= 255)
			{
				TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(x, z);
				if (chunkAtCell != null && chunkAtCell.State >= TerrainChunkState.InvalidVertices1)
				{
					this.m_itemLight = this.m_subsystemTerrain.Terrain.GetCellLightFast(x, num, z);
				}
			}
			int num4 = Terrain.ExtractContents(activeBlockValue);
			Block block = BlocksManager.Blocks[num4];
			Vector3 vector = block.InHandRotation * 0.017453292f + this.m_itemRotation;
			Matrix inWorldMatrix = Matrix.CreateFromYawPitchRoll(vector.Y, vector.X, vector.Z) * Matrix.CreateTranslation(block.InHandOffset) * matrix * Matrix.CreateTranslation(this.m_itemOffset) * controllerMatrix * m;
			this.m_drawBlockEnvironmentData.SubsystemTerrain = this.m_subsystemTerrain;
			this.m_drawBlockEnvironmentData.InWorldMatrix = inWorldMatrix;
			this.m_drawBlockEnvironmentData.Light = this.m_itemLight;
			this.m_drawBlockEnvironmentData.Humidity = this.m_subsystemTerrain.Terrain.GetHumidity(x, z);
			this.m_drawBlockEnvironmentData.Temperature = this.m_subsystemTerrain.Terrain.GetTemperature(x, z) + SubsystemWeather.GetTemperatureAdjustmentAtHeight(num);
			block.DrawBlock(this.m_primitivesRenderer, activeBlockValue, Color.White, block.InHandScale, ref inWorldMatrix, this.m_drawBlockEnvironmentData);
			IL_49E:
			this.m_primitivesRenderer.Flush(camera.ProjectionMatrix, true, int.MaxValue);
		}

		// Token: 0x0600104F RID: 4175 RVA: 0x0007CE04 File Offset: 0x0007B004
		public void Update(float dt)
		{
			this.m_pokeAnimationTime = this.m_componentMiner.PokingPhase;
			this.m_itemOffset = Vector3.Lerp(this.m_itemOffset, this.ItemOffsetOrder, MathUtils.Saturate(10f * dt));
			this.m_itemRotation = Vector3.Lerp(this.m_itemRotation, this.ItemRotationOrder, MathUtils.Saturate(10f * dt));
			this.ItemOffsetOrder = Vector3.Zero;
			this.ItemRotationOrder = Vector3.Zero;
		}

        // Token: 0x06001050 RID: 4176 RVA: 0x0007CE80 File Offset: 0x0007B080
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.m_componentMiner = base.Entity.FindComponent<ComponentMiner>(true);
			this.m_vrHandModel = ContentManager.Get<Model>(valuesDictionary.GetValue<string>("VrHandModelName"));
		}

		// Token: 0x04000AA7 RID: 2727
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000AA8 RID: 2728
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000AA9 RID: 2729
		public ComponentMiner m_componentMiner;

		// Token: 0x04000AAA RID: 2730
		public Model m_vrHandModel;

		// Token: 0x04000AAB RID: 2731
		public Vector3 m_itemOffset;

		// Token: 0x04000AAC RID: 2732
		public Vector3 m_itemRotation;

		// Token: 0x04000AAD RID: 2733
		public float m_pokeAnimationTime;

		// Token: 0x04000AAE RID: 2734
		public double m_nextHandLightTime;

		// Token: 0x04000AAF RID: 2735
		public float m_handLight;

		// Token: 0x04000AB0 RID: 2736
		public int m_itemLight;

		// Token: 0x04000AB1 RID: 2737
		public DrawBlockEnvironmentData m_drawBlockEnvironmentData = new DrawBlockEnvironmentData();

		// Token: 0x04000AB2 RID: 2738
		public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

		// Token: 0x04000AB3 RID: 2739
		public static LitShader m_shader = new LitShader(2, false, false, true, false, false, 1);

		// Token: 0x04000AB4 RID: 2740
		public static int[] m_drawOrders = new int[]
		{
			1
		};
	}
}
