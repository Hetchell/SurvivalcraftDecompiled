using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000191 RID: 401
	public class SubsystemModelsRenderer : Subsystem, IDrawable
	{
		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000937 RID: 2359 RVA: 0x0003F6E9 File Offset: 0x0003D8E9
		public PrimitivesRenderer3D PrimitivesRenderer
		{
			get
			{
				return this.m_primitivesRenderer;
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000938 RID: 2360 RVA: 0x0003F6F1 File Offset: 0x0003D8F1
		public int[] DrawOrders
		{
			get
			{
				return this.m_drawOrders;
			}
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x0003F6FC File Offset: 0x0003D8FC
		public void Draw(Camera camera, int drawOrder)
		{
			if (drawOrder == this.m_drawOrders[0])
			{
				this.ModelsDrawn = 0;
				List<SubsystemModelsRenderer.ModelData>[] modelsToDraw = this.m_modelsToDraw;
				for (int i = 0; i < modelsToDraw.Length; i++)
				{
					modelsToDraw[i].Clear();
				}
				this.m_modelsToPrepare.Clear();
				foreach (SubsystemModelsRenderer.ModelData modelData in this.m_componentModels.Values)
				{
					if (modelData.ComponentModel.Model != null)
					{
						modelData.ComponentModel.CalculateIsVisible(camera);
						if (modelData.ComponentModel.IsVisibleForCamera)
						{
							this.m_modelsToPrepare.Add(modelData);
						}
					}
				}
				this.m_modelsToPrepare.Sort();
				using (List<SubsystemModelsRenderer.ModelData>.Enumerator enumerator2 = this.m_modelsToPrepare.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SubsystemModelsRenderer.ModelData modelData2 = enumerator2.Current;
						this.PrepareModel(modelData2, camera);
						this.m_modelsToDraw[(int)modelData2.ComponentModel.RenderingMode].Add(modelData2);
					}
					return;
				}
			}
			if (!SubsystemModelsRenderer.DisableDrawingModels)
			{
				if (drawOrder == this.m_drawOrders[1])
				{
					Display.DepthStencilState = DepthStencilState.Default;
					Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
					Display.BlendState = BlendState.Opaque;
					this.DrawModels(camera, this.m_modelsToDraw[0], null);
					Display.RasterizerState = RasterizerState.CullNoneScissor;
					this.DrawModels(camera, this.m_modelsToDraw[1], new float?(0f));
					Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
					this.m_primitivesRenderer.Flush(camera.ProjectionMatrix, true, 0);
					return;
				}
				if (drawOrder == this.m_drawOrders[2])
				{
					Display.DepthStencilState = DepthStencilState.Default;
					Display.RasterizerState = RasterizerState.CullNoneScissor;
					Display.BlendState = BlendState.AlphaBlend;
					this.DrawModels(camera, this.m_modelsToDraw[2], null);
					return;
				}
				if (drawOrder == this.m_drawOrders[3])
				{
					Display.DepthStencilState = DepthStencilState.Default;
					Display.RasterizerState = RasterizerState.CullNoneScissor;
					Display.BlendState = BlendState.AlphaBlend;
					this.DrawModels(camera, this.m_modelsToDraw[3], null);
					this.m_primitivesRenderer.Flush(camera.ProjectionMatrix, true, int.MaxValue);
					return;
				}
			}
			else
			{
				this.m_primitivesRenderer.Clear();
			}
		}

        // Token: 0x0600093A RID: 2362 RVA: 0x0003F960 File Offset: 0x0003DB60
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_subsystemShadows = base.Project.FindSubsystem<SubsystemShadows>(true);
		}

        // Token: 0x0600093B RID: 2363 RVA: 0x0003F998 File Offset: 0x0003DB98
        public override void OnEntityAdded(Entity entity)
		{
			foreach (ComponentModel componentModel in entity.FindComponents<ComponentModel>())
			{
				SubsystemModelsRenderer.ModelData value = new SubsystemModelsRenderer.ModelData
				{
					ComponentModel = componentModel,
					ComponentBody = componentModel.Entity.FindComponent<ComponentBody>(),
					Light = this.m_subsystemSky.SkyLightIntensity
				};
				this.m_componentModels.Add(componentModel, value);
			}
		}

        // Token: 0x0600093C RID: 2364 RVA: 0x0003FA24 File Offset: 0x0003DC24
        public override void OnEntityRemoved(Entity entity)
		{
			foreach (ComponentModel key in entity.FindComponents<ComponentModel>())
			{
				this.m_componentModels.Remove(key);
			}
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x0003FA80 File Offset: 0x0003DC80
		public void PrepareModel(SubsystemModelsRenderer.ModelData modelData, Camera camera)
		{
			if (Time.FrameIndex > modelData.LastAnimateFrame)
			{
				modelData.ComponentModel.Animate();
				modelData.LastAnimateFrame = Time.FrameIndex;
			}
			if (Time.FrameStartTime >= modelData.NextLightTime)
			{
				float? num = this.CalculateModelLight(modelData);
				if (num != null)
				{
					modelData.Light = num.Value;
				}
				modelData.NextLightTime = Time.FrameStartTime + 0.1;
			}
			modelData.ComponentModel.CalculateAbsoluteBonesTransforms(camera);
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x0003FAFC File Offset: 0x0003DCFC
		public void DrawModels(Camera camera, List<SubsystemModelsRenderer.ModelData> modelsData, float? alphaThreshold)
		{
			this.DrawInstancedModels(camera, modelsData, alphaThreshold);
			this.DrawModelsExtras(camera, modelsData);
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x0003FB10 File Offset: 0x0003DD10
		public void DrawInstancedModels(Camera camera, List<SubsystemModelsRenderer.ModelData> modelsData, float? alphaThreshold)
		{
			ModelShader modelShader = (alphaThreshold != null) ? SubsystemModelsRenderer.m_shaderAlphaTested : SubsystemModelsRenderer.m_shaderOpaque;
			modelShader.LightDirection1 = -Vector3.TransformNormal(LightingManager.DirectionToLight1, camera.ViewMatrix);
			modelShader.LightDirection2 = -Vector3.TransformNormal(LightingManager.DirectionToLight2, camera.ViewMatrix);
			modelShader.FogColor = new Vector3(this.m_subsystemSky.ViewFogColor);
			modelShader.FogStartInvLength = new Vector2(this.m_subsystemSky.ViewFogRange.X, 1f / (this.m_subsystemSky.ViewFogRange.Y - this.m_subsystemSky.ViewFogRange.X));
			modelShader.FogYMultiplier = this.m_subsystemSky.VisibilityRangeYMultiplier;
			modelShader.WorldUp = Vector3.TransformNormal(Vector3.UnitY, camera.ViewMatrix);
			modelShader.Transforms.View = Matrix.Identity;
			modelShader.Transforms.Projection = camera.ProjectionMatrix;
			modelShader.SamplerState = SamplerState.PointClamp;
			if (alphaThreshold != null)
			{
				modelShader.AlphaThreshold = alphaThreshold.Value;
			}
			foreach (SubsystemModelsRenderer.ModelData modelData in modelsData)
			{
				ComponentModel componentModel = modelData.ComponentModel;
				Vector3 v = (componentModel.DiffuseColor != null) ? componentModel.DiffuseColor.Value : Vector3.One;
				float num = (componentModel.Opacity != null) ? componentModel.Opacity.Value : 1f;
				modelShader.InstancesCount = componentModel.AbsoluteBoneTransformsForCamera.Length;
				modelShader.MaterialColor = new Vector4(v * num, num);
				modelShader.EmissionColor = ((componentModel.EmissionColor != null) ? componentModel.EmissionColor.Value : Vector4.Zero);
				modelShader.AmbientLightColor = new Vector3(LightingManager.LightAmbient * modelData.Light);
				modelShader.DiffuseLightColor1 = new Vector3(modelData.Light);
				modelShader.DiffuseLightColor2 = new Vector3(modelData.Light);
				modelShader.Texture = componentModel.TextureOverride;
				Array.Copy(componentModel.AbsoluteBoneTransformsForCamera, modelShader.Transforms.World, componentModel.AbsoluteBoneTransformsForCamera.Length);
				InstancedModelData instancedModelData = InstancedModelsManager.GetInstancedModelData(componentModel.Model, componentModel.MeshDrawOrders);
				Display.DrawIndexed(PrimitiveType.TriangleList, modelShader, instancedModelData.VertexBuffer, instancedModelData.IndexBuffer, 0, instancedModelData.IndexBuffer.IndicesCount);
				this.ModelsDrawn++;
			}
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x0003FDC8 File Offset: 0x0003DFC8
		public void DrawModelsExtras(Camera camera, List<SubsystemModelsRenderer.ModelData> modelsData)
		{
			foreach (SubsystemModelsRenderer.ModelData modelData in modelsData)
			{
				if (modelData.ComponentBody != null && modelData.ComponentModel.CastsShadow)
				{
					Vector3 shadowPosition = modelData.ComponentBody.Position + new Vector3(0f, 0.1f, 0f);
					BoundingBox boundingBox = modelData.ComponentBody.BoundingBox;
					float shadowDiameter = 2.25f * (boundingBox.Max.X - boundingBox.Min.X);
					this.m_subsystemShadows.QueueShadow(camera, shadowPosition, shadowDiameter, modelData.ComponentModel.Opacity ?? 1f);
				}
				modelData.ComponentModel.DrawExtras(camera);
			}
		}

		// Token: 0x06000941 RID: 2369 RVA: 0x0003FEC0 File Offset: 0x0003E0C0
		public float? CalculateModelLight(SubsystemModelsRenderer.ModelData modelData)
		{
			Vector3 p;
			if (modelData.ComponentBody != null)
			{
				p = modelData.ComponentBody.Position;
				p.Y += 0.95f * (modelData.ComponentBody.BoundingBox.Max.Y - modelData.ComponentBody.BoundingBox.Min.Y);
			}
			else
			{
				Matrix? boneTransform = modelData.ComponentModel.GetBoneTransform(modelData.ComponentModel.Model.RootBone.Index);
				p = ((boneTransform == null) ? Vector3.Zero : (boneTransform.Value.Translation + new Vector3(0f, 0.9f, 0f)));
			}
			return LightingManager.CalculateSmoothLight(this.m_subsystemTerrain, p);
		}

		// Token: 0x040004D2 RID: 1234
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040004D3 RID: 1235
		public SubsystemSky m_subsystemSky;

		// Token: 0x040004D4 RID: 1236
		public SubsystemShadows m_subsystemShadows;

		// Token: 0x040004D5 RID: 1237
		public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

		// Token: 0x040004D6 RID: 1238
		public static ModelShader m_shaderOpaque = new ModelShader(false, 7);

		// Token: 0x040004D7 RID: 1239
		public static ModelShader m_shaderAlphaTested = new ModelShader(true, 7);

		// Token: 0x040004D8 RID: 1240
		public Dictionary<ComponentModel, SubsystemModelsRenderer.ModelData> m_componentModels = new Dictionary<ComponentModel, SubsystemModelsRenderer.ModelData>();

		// Token: 0x040004D9 RID: 1241
		public List<SubsystemModelsRenderer.ModelData> m_modelsToPrepare = new List<SubsystemModelsRenderer.ModelData>();

		// Token: 0x040004DA RID: 1242
		public List<SubsystemModelsRenderer.ModelData>[] m_modelsToDraw = new List<SubsystemModelsRenderer.ModelData>[]
		{
			new List<SubsystemModelsRenderer.ModelData>(),
			new List<SubsystemModelsRenderer.ModelData>(),
			new List<SubsystemModelsRenderer.ModelData>(),
			new List<SubsystemModelsRenderer.ModelData>()
		};

		// Token: 0x040004DB RID: 1243
		public static bool DisableDrawingModels = false;

		// Token: 0x040004DC RID: 1244
		public int ModelsDrawn;

		// Token: 0x040004DD RID: 1245
		public int[] m_drawOrders = new int[]
		{
			-10000,
			1,
			99,
			201
		};

		// Token: 0x0200042F RID: 1071
		public class ModelData : IComparable<SubsystemModelsRenderer.ModelData>
		{
			// Token: 0x06001E79 RID: 7801 RVA: 0x000DF0F8 File Offset: 0x000DD2F8
			public int CompareTo(SubsystemModelsRenderer.ModelData other)
			{
				int num = (this.ComponentModel != null) ? this.ComponentModel.PrepareOrder : 0;
				int num2 = (other.ComponentModel != null) ? other.ComponentModel.PrepareOrder : 0;
				return num - num2;
			}

			// Token: 0x040015AE RID: 5550
			public ComponentModel ComponentModel;

			// Token: 0x040015AF RID: 5551
			public ComponentBody ComponentBody;

			// Token: 0x040015B0 RID: 5552
			public float Light;

			// Token: 0x040015B1 RID: 5553
			public double NextLightTime;

			// Token: 0x040015B2 RID: 5554
			public int LastAnimateFrame;
		}
	}
}
