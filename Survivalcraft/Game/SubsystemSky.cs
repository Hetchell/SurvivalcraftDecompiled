using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using Survivalcraft.Game.ModificationHolder;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001A7 RID: 423
	public class SubsystemSky : Subsystem, IDrawable, IUpdateable
	{
		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000A2A RID: 2602 RVA: 0x0004A6A9 File Offset: 0x000488A9
		// (set) Token: 0x06000A2B RID: 2603 RVA: 0x0004A6B1 File Offset: 0x000488B1
		public float SkyLightIntensity { get; set; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000A2C RID: 2604 RVA: 0x0004A6BA File Offset: 0x000488BA
		// (set) Token: 0x06000A2D RID: 2605 RVA: 0x0004A6C2 File Offset: 0x000488C2
		public int MoonPhase { get; set; }

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000A2E RID: 2606 RVA: 0x0004A6CB File Offset: 0x000488CB
		// (set) Token: 0x06000A2F RID: 2607 RVA: 0x0004A6D3 File Offset: 0x000488D3
		public int SkyLightValue { get; set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000A30 RID: 2608 RVA: 0x0004A6DC File Offset: 0x000488DC
		// (set) Token: 0x06000A31 RID: 2609 RVA: 0x0004A6E4 File Offset: 0x000488E4
		public float VisibilityRange { get; set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000A32 RID: 2610 RVA: 0x0004A6ED File Offset: 0x000488ED
		// (set) Token: 0x06000A33 RID: 2611 RVA: 0x0004A6F5 File Offset: 0x000488F5
		public float VisibilityRangeYMultiplier { get; set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000A34 RID: 2612 RVA: 0x0004A6FE File Offset: 0x000488FE
		// (set) Token: 0x06000A35 RID: 2613 RVA: 0x0004A706 File Offset: 0x00048906
		public float ViewUnderWaterDepth { get; set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000A36 RID: 2614 RVA: 0x0004A70F File Offset: 0x0004890F
		// (set) Token: 0x06000A37 RID: 2615 RVA: 0x0004A717 File Offset: 0x00048917
		public float ViewUnderMagmaDepth { get; set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000A38 RID: 2616 RVA: 0x0004A720 File Offset: 0x00048920
		public Color ViewFogColor
		{
			get
			{
				return this.m_viewFogColor;
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000A39 RID: 2617 RVA: 0x0004A728 File Offset: 0x00048928
		public Vector2 ViewFogRange
		{
			get
			{
				return this.m_viewFogRange;
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000A3A RID: 2618 RVA: 0x0004A730 File Offset: 0x00048930
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000A3B RID: 2619 RVA: 0x0004A733 File Offset: 0x00048933
		public int[] DrawOrders
		{
			get
			{
				return this.m_drawOrders;
			}
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x0004A73C File Offset: 0x0004893C
		public void MakeLightningStrike(Vector3 targetPosition)
		{
			if (this.m_lightningStrikePosition != null || this.m_subsystemTime.GameTime - this.m_lastLightningStrikeTime <= 1.0)
			{
				return;
			}
			this.m_lastLightningStrikeTime = this.m_subsystemTime.GameTime;
			this.m_lightningStrikePosition = new Vector3?(targetPosition);
			this.m_lightningStrikeBrightness = 1f;
			float num = float.MaxValue;
			foreach (Vector3 vector in this.m_subsystemAudio.ListenerPositions)
			{
				float num2 = Vector2.Distance(new Vector2(vector.X, vector.Z), new Vector2(targetPosition.X, targetPosition.Z));
				if (num2 < num)
				{
					num = num2;
				}
			}
			float delay = this.m_subsystemAudio.CalculateDelay(num);
			if (num < 40f)
			{
				this.m_subsystemAudio.PlayRandomSound("Audio/ThunderNear", 1f, this.m_random.Float(-0.2f, 0.2f), 0f, delay);
			}
			else if (num < 200f)
			{
				this.m_subsystemAudio.PlayRandomSound("Audio/ThunderFar", 0.8f, this.m_random.Float(-0.2f, 0.2f), 0f, delay);
			}
			if (this.m_subsystemGameInfo.WorldSettings.EnvironmentBehaviorMode != EnvironmentBehaviorMode.Living)
			{
				return;
			}
			DynamicArray<ComponentBody> dynamicArray = new DynamicArray<ComponentBody>();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(targetPosition.X, targetPosition.Z), 4f, dynamicArray);
			for (int i = 0; i < dynamicArray.Count; i++)
			{
				ComponentBody componentBody = dynamicArray.Array[i];
				if (componentBody.Position.Y > targetPosition.Y - 1.5f && Vector2.Distance(new Vector2(componentBody.Position.X, componentBody.Position.Z), new Vector2(targetPosition.X, targetPosition.Z)) < 4f)
				{
					ComponentOnFire componentOnFire = componentBody.Entity.FindComponent<ComponentOnFire>();
					if (componentOnFire != null)
					{
						componentOnFire.SetOnFire(null, this.m_random.Float(12f, 15f));
					}
				}
				ComponentCreature componentCreature = componentBody.Entity.FindComponent<ComponentCreature>();
				if (componentCreature != null && componentCreature.PlayerStats != null)
				{
					componentCreature.PlayerStats.StruckByLightning += 1L;
				}
			}
			int x = Terrain.ToCell(targetPosition.X);
			int num3 = Terrain.ToCell(targetPosition.Y);
			int z = Terrain.ToCell(targetPosition.Z);
			float pressure = (float)((this.m_random.Float(0f, 1f) < 0.2f) ? 39 : 19);
			base.Project.FindSubsystem<SubsystemExplosions>(true).AddExplosion(x, num3 + 1, z, pressure, false, true);
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x0004AA18 File Offset: 0x00048C18
		public void Update(float dt)
		{
			this.MoonPhase = ((int)MathUtils.Floor(this.m_subsystemTimeOfDay.Day - 0.5 + 5.0) % 8 + 8) % 8;
			this.UpdateLightAndViewParameters();
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x0004AA54 File Offset: 0x00048C54
		public void Draw(Camera camera, int drawOrder)
		{
			if (drawOrder == this.m_drawOrders[0])
			{
				this.ViewUnderWaterDepth = 0f;
				this.ViewUnderMagmaDepth = 0f;
				Vector3 viewPosition = camera.ViewPosition;
				int x = Terrain.ToCell(viewPosition.X);
				int y = Terrain.ToCell(viewPosition.Y);
				int z = Terrain.ToCell(viewPosition.Z);
				FluidBlock fluidBlock;
				float? surfaceHeight = this.m_subsystemFluidBlockBehavior.GetSurfaceHeight(x, y, z, out fluidBlock);
				if (surfaceHeight != null)
				{
					if (fluidBlock is WaterBlock)
					{
						this.ViewUnderWaterDepth = surfaceHeight.Value + 0.1f - viewPosition.Y;
					}
					else if (fluidBlock is MagmaBlock)
					{
						this.ViewUnderMagmaDepth = surfaceHeight.Value + 1f - viewPosition.Y;
					}
				}
				if (this.ViewUnderWaterDepth > 0f)
				{
					int seasonalHumidity = this.m_subsystemTerrain.Terrain.GetSeasonalHumidity(x, z);
					int temperature = this.m_subsystemTerrain.Terrain.GetSeasonalTemperature(x, z) + SubsystemWeather.GetTemperatureAdjustmentAtHeight(y);
					Color c = BlockColorsMap.WaterColorsMap.Lookup(temperature, seasonalHumidity);
					float num = MathUtils.Lerp(1f, 0.5f, (float)seasonalHumidity / 15f);
					float num2 = MathUtils.Lerp(1f, 0.2f, MathUtils.Saturate(0.075f * (this.ViewUnderWaterDepth - 2f)));
					float num3 = MathUtils.Lerp(0.33f, 1f, this.SkyLightIntensity);
					this.m_viewFogRange.X = 0f;
					this.m_viewFogRange.Y = MathUtils.Lerp(4f, 10f, num * num2 * num3);
					this.m_viewFogColor = Color.MultiplyColorOnly(c, 0.66f * num2 * num3);
					this.VisibilityRangeYMultiplier = 1f;
					this.m_viewIsSkyVisible = false;
				}
				else if (this.ViewUnderMagmaDepth > 0f)
				{
					this.m_viewFogRange.X = 10f;
					this.m_viewFogRange.Y = 90.1f;
                    this.m_viewFogColor = Color.Gray;
					this.VisibilityRangeYMultiplier = 45f;
					this.m_viewIsSkyVisible = true;
				}
				else
				{
					float num4 = 1024f;
					float num5 = 128f;
					int seasonalTemperature = this.m_subsystemTerrain.Terrain.GetSeasonalTemperature(Terrain.ToCell(viewPosition.X), Terrain.ToCell(viewPosition.Z));
					float num6 = MathUtils.Lerp(0.5f, 0f, this.m_subsystemWeather.GlobalPrecipitationIntensity);
					float num7 = MathUtils.Lerp(1f, 0.8f, this.m_subsystemWeather.GlobalPrecipitationIntensity);
					this.m_viewFogRange.X = this.VisibilityRange * num6;
					this.m_viewFogRange.Y = this.VisibilityRange * num7;
					this.m_viewFogColor = this.CalculateSkyColor(new Vector3(camera.ViewDirection.X, 0f, camera.ViewDirection.Z), this.m_subsystemTimeOfDay.TimeOfDay, this.m_subsystemWeather.GlobalPrecipitationIntensity, seasonalTemperature);
					this.VisibilityRangeYMultiplier = MathUtils.Lerp(this.VisibilityRange / num4, this.VisibilityRange / num5, MathUtils.Pow(this.m_subsystemWeather.GlobalPrecipitationIntensity, 4f));
					this.m_viewIsSkyVisible = true;
				}
				if (!this.FogEnabled)
				{
					this.m_viewFogRange = new Vector2(100000f, 100000f);
				}
				if (!this.DrawSkyEnabled || !this.m_viewIsSkyVisible || SettingsManager.SkyRenderingMode == SkyRenderingMode.Disabled)
				{
					FlatBatch2D flatBatch2D = this.m_primitivesRenderer2d.FlatBatch(-1, DepthStencilState.None, RasterizerState.CullNoneScissor, BlendState.Opaque);
					int count = flatBatch2D.TriangleVertices.Count;
					flatBatch2D.QueueQuad(Vector2.Zero, camera.ViewportSize, 0f, this.m_viewFogColor);
					flatBatch2D.TransformTriangles(camera.ViewportMatrix, count, -1);
					this.m_primitivesRenderer2d.Flush(true, int.MaxValue);
					return;
				}
			}
			else if (drawOrder == this.m_drawOrders[1])
			{
				if (this.DrawSkyEnabled && this.m_viewIsSkyVisible && SettingsManager.SkyRenderingMode != SkyRenderingMode.Disabled)
				{
					this.DrawSkydome(camera);
					this.DrawStars(camera);
					this.DrawSunAndMoon(camera);
					this.DrawClouds(camera);
					this.m_primitivesRenderer3d.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
					return;
				}
			}
			else
			{
				this.DrawLightning(camera);
				this.m_primitivesRenderer3d.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
			}
		}

        // Token: 0x06000A3F RID: 2623 RVA: 0x0004AE98 File Offset: 0x00049098
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTimeOfDay = base.Project.FindSubsystem<SubsystemTimeOfDay>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemWeather = base.Project.FindSubsystem<SubsystemWeather>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemFluidBlockBehavior = base.Project.FindSubsystem<SubsystemFluidBlockBehavior>(true);
			this.m_sunTexture = ContentManager.Get<Texture2D>("Textures/Sun");
			this.m_glowTexture = ContentManager.Get<Texture2D>("Textures/SkyGlow");
			this.m_cloudsTexture = ContentManager.Get<Texture2D>("Textures/Clouds");
			for (int i = 0; i < 8; i++)
			{
				this.m_moonTextures[i] = ContentManager.Get<Texture2D>("Textures/Moon" + (i + 1).ToString(CultureInfo.InvariantCulture));
			}
			this.UpdateLightAndViewParameters();
			Display.DeviceReset += this.Display_DeviceReset;
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x0004AFB0 File Offset: 0x000491B0
		public override void Dispose()
		{
			Display.DeviceReset -= this.Display_DeviceReset;
			Utilities.Dispose<VertexBuffer>(ref this.m_starsVertexBuffer);
			Utilities.Dispose<IndexBuffer>(ref this.m_starsIndexBuffer);
			foreach (SubsystemSky.SkyDome skyDome in this.m_skyDomes.Values)
			{
				skyDome.Dispose();
			}
			this.m_skyDomes.Clear();
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x0004B038 File Offset: 0x00049238
		public void Display_DeviceReset()
		{
			Utilities.Dispose<VertexBuffer>(ref this.m_starsVertexBuffer);
			Utilities.Dispose<IndexBuffer>(ref this.m_starsIndexBuffer);
			foreach (SubsystemSky.SkyDome skyDome in this.m_skyDomes.Values)
			{
				skyDome.Dispose();
			}
			this.m_skyDomes.Clear();
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x0004B0B0 File Offset: 0x000492B0
		public void DrawSkydome(Camera camera)
		{
			SubsystemSky.SkyDome skyDome;
			if (!this.m_skyDomes.TryGetValue(camera.GameWidget, out skyDome))
			{
				skyDome = new SubsystemSky.SkyDome();
				this.m_skyDomes.Add(camera.GameWidget, skyDome);
			}
			if (skyDome.VertexBuffer == null || skyDome.IndexBuffer == null)
			{
				Utilities.Dispose<VertexBuffer>(ref skyDome.VertexBuffer);
				Utilities.Dispose<IndexBuffer>(ref skyDome.IndexBuffer);
				skyDome.VertexBuffer = new VertexBuffer(this.m_skyVertexDeclaration, skyDome.Vertices.Length);
				skyDome.IndexBuffer = new IndexBuffer(IndexFormat.SixteenBits, skyDome.Indices.Length);
				this.FillSkyIndexBuffer(skyDome);
				skyDome.LastUpdateTimeOfDay = null;
			}
			int x = Terrain.ToCell(camera.ViewPosition.X);
			int z = Terrain.ToCell(camera.ViewPosition.Z);
			float globalPrecipitationIntensity = this.m_subsystemWeather.GlobalPrecipitationIntensity;
			float timeOfDay = this.m_subsystemTimeOfDay.TimeOfDay;
			int seasonalTemperature = this.m_subsystemTerrain.Terrain.GetSeasonalTemperature(x, z);
			if (skyDome.LastUpdateTimeOfDay != null && MathUtils.Abs(timeOfDay - skyDome.LastUpdateTimeOfDay.Value) <= 0.001f && skyDome.LastUpdatePrecipitationIntensity != null && MathUtils.Abs(globalPrecipitationIntensity - skyDome.LastUpdatePrecipitationIntensity.Value) <= 0.02f && ((globalPrecipitationIntensity != 0f && globalPrecipitationIntensity != 1f) || skyDome.LastUpdatePrecipitationIntensity.Value == globalPrecipitationIntensity) && this.m_lightningStrikeBrightness == skyDome.LastUpdateLightningStrikeBrightness && skyDome.LastUpdateTemperature != null)
			{
				int num = seasonalTemperature;
				int? lastUpdateTemperature = skyDome.LastUpdateTemperature;
				if (num == lastUpdateTemperature.GetValueOrDefault() & lastUpdateTemperature != null)
				{
					goto IL_1C6;
				}
			}
			skyDome.LastUpdateTimeOfDay = new float?(timeOfDay);
			skyDome.LastUpdatePrecipitationIntensity = new float?(globalPrecipitationIntensity);
			skyDome.LastUpdateLightningStrikeBrightness = this.m_lightningStrikeBrightness;
			skyDome.LastUpdateTemperature = new int?(seasonalTemperature);
			this.FillSkyVertexBuffer(skyDome, timeOfDay, globalPrecipitationIntensity, seasonalTemperature);
			IL_1C6:
			Display.DepthStencilState = DepthStencilState.DepthRead;
			Display.RasterizerState = RasterizerState.CullNoneScissor;
			Display.BlendState = BlendState.Opaque;
			SubsystemSky.m_shaderFlat.Transforms.World[0] = Matrix.CreateTranslation(camera.ViewPosition) * camera.ViewProjectionMatrix;
			SubsystemSky.m_shaderFlat.Color = Vector4.One;
			Display.DrawIndexed(PrimitiveType.TriangleList, SubsystemSky.m_shaderFlat, skyDome.VertexBuffer, skyDome.IndexBuffer, 0, skyDome.IndexBuffer.IndicesCount);
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x0004B300 File Offset: 0x00049500
		public void DrawStars(Camera camera)
		{
			float globalPrecipitationIntensity = this.m_subsystemWeather.GlobalPrecipitationIntensity;
			float timeOfDay = this.m_subsystemTimeOfDay.TimeOfDay;
			if (this.m_starsVertexBuffer == null || this.m_starsIndexBuffer == null)
			{
				Utilities.Dispose<VertexBuffer>(ref this.m_starsVertexBuffer);
				Utilities.Dispose<IndexBuffer>(ref this.m_starsIndexBuffer);
				this.m_starsVertexBuffer = new VertexBuffer(this.m_starsVertexDeclaration, 600);
				this.m_starsIndexBuffer = new IndexBuffer(IndexFormat.SixteenBits, 900);
				this.FillStarsBuffers();
			}
			Display.DepthStencilState = DepthStencilState.DepthRead;
			Display.RasterizerState = RasterizerState.CullNoneScissor;
			float num = MathUtils.Sqr((1f - this.CalculateLightIntensity(timeOfDay)) * (1f - globalPrecipitationIntensity));
			if (num > 0.01f)
			{
				Display.BlendState = BlendState.Additive;
				SubsystemSky.m_shaderTextured.Transforms.World[0] = Matrix.CreateRotationZ(-2f * timeOfDay * 3.1415927f) * Matrix.CreateTranslation(camera.ViewPosition) * camera.ViewProjectionMatrix;
				SubsystemSky.m_shaderTextured.Color = new Vector4(1f, 1f, 1f, num);
				SubsystemSky.m_shaderTextured.Texture = ContentManager.Get<Texture2D>("Textures/Star");
				SubsystemSky.m_shaderTextured.SamplerState = SamplerState.LinearClamp;
				Display.DrawIndexed(PrimitiveType.TriangleList, SubsystemSky.m_shaderTextured, this.m_starsVertexBuffer, this.m_starsIndexBuffer, 0, this.m_starsIndexBuffer.IndicesCount);
			}
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x0004B464 File Offset: 0x00049664
		public void DrawSunAndMoon(Camera camera)
		{
			float globalPrecipitationIntensity = this.m_subsystemWeather.GlobalPrecipitationIntensity;
			float timeOfDay = this.m_subsystemTimeOfDay.TimeOfDay;
			float f = MathUtils.Max(SubsystemSky.CalculateDawnGlowIntensity(timeOfDay), SubsystemSky.CalculateDuskGlowIntensity(timeOfDay));
			float num = 2f * timeOfDay * 3.1415927f;
			float angle = num + 3.1415927f;
			float num2 = MathUtils.Lerp(90f, 160f, f);
			float num3 = MathUtils.Lerp(60f, 80f, f);
			Color color = Color.Lerp(new Color(255, 255, 255), new Color(255, 255, 160), f);
			Color color2 = Color.White;
			color2 *= 1f - this.SkyLightIntensity;
			color *= MathUtils.Lerp(1f, 0f, globalPrecipitationIntensity);
			color2 *= MathUtils.Lerp(1f, 0f, globalPrecipitationIntensity);
			Color color3 = color * 0.6f * MathUtils.Lerp(1f, 0f, globalPrecipitationIntensity);
			Color color4 = color * 0.2f * MathUtils.Lerp(1f, 0f, globalPrecipitationIntensity);
			TexturedBatch3D batch = this.m_primitivesRenderer3d.TexturedBatch(this.m_glowTexture, false, 0, DepthStencilState.DepthRead, null, BlendState.Additive, null);
			TexturedBatch3D batch2 = this.m_primitivesRenderer3d.TexturedBatch(this.m_sunTexture, false, 1, DepthStencilState.DepthRead, null, BlendState.AlphaBlend, null);
			TexturedBatch3D batch3 = this.m_primitivesRenderer3d.TexturedBatch(this.m_moonTextures[this.MoonPhase], false, 1, DepthStencilState.DepthRead, null, BlendState.AlphaBlend, null);
			this.QueueCelestialBody(batch, camera.ViewPosition, color3, 900f, 3.5f * num2, num);
			this.QueueCelestialBody(batch, camera.ViewPosition, color4, 900f, 3.5f * num3, angle);
			this.QueueCelestialBody(batch2, camera.ViewPosition, color, 900f, num2, num);
			this.QueueCelestialBody(batch3, camera.ViewPosition, color2, 900f, num3, angle);
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x0004B674 File Offset: 0x00049874
		public void DrawLightning(Camera camera)
		{
			if (this.m_lightningStrikePosition == null)
			{
				return;
			}
			FlatBatch3D flatBatch3D = this.m_primitivesRenderer3d.FlatBatch(0, DepthStencilState.DepthRead, null, BlendState.Additive);
			Vector3 value = this.m_lightningStrikePosition.Value;
			Vector3 unitY = Vector3.UnitY;
			Vector3 v = Vector3.Normalize(Vector3.Cross(camera.ViewDirection, unitY));
			Viewport viewport = Display.Viewport;
			float num = Vector4.Transform(new Vector4(value, 1f), camera.ViewProjectionMatrix).W * 2f / ((float)viewport.Width * camera.ProjectionMatrix.M11);
			for (int i = 0; i < (int)(this.m_lightningStrikeBrightness * 30f); i++)
			{
				float s = this.m_random.NormalFloat(0f, 1f * num);
				float s2 = this.m_random.NormalFloat(0f, 1f * num);
				Vector3 v2 = s * v + s2 * unitY;
				float num4;
				for (float num2 = 260f; num2 > value.Y; num2 -= num4)
				{
					uint num3 = MathUtils.Hash((uint)(this.m_lightningStrikePosition.Value.X + 100f * this.m_lightningStrikePosition.Value.Z + 200f * num2));
					num4 = MathUtils.Lerp(4f, 10f, (float)(num3 & 255U) / 255f);
					float s3 = (float)(((num3 & 1U) == 0U) ? 1 : -1);
					float s4 = MathUtils.Lerp(0.05f, 0.2f, (float)(num3 >> 8 & 255U) / 255f);
					float num5 = num2;
					float num6 = num5 - num4 * MathUtils.Lerp(0.45f, 0.55f, (float)(num3 >> 16 & 255U) / 255f);
					float num7 = num5 - num4 * MathUtils.Lerp(0.45f, 0.55f, (float)(num3 >> 24 & 255U) / 255f);
					float num8 = num5 - num4;
					Vector3 p = new Vector3(value.X, num5, value.Z) + v2;
					Vector3 vector = new Vector3(value.X, num6, value.Z) + v2 - num4 * v * s3 * s4;
					Vector3 vector2 = new Vector3(value.X, num7, value.Z) + v2 + num4 * v * s3 * s4;
					Vector3 p2 = new Vector3(value.X, num8, value.Z) + v2;
					Color color = Color.White * 0.2f * MathUtils.Saturate((260f - num5) * 0.2f);
					Color color2 = Color.White * 0.2f * MathUtils.Saturate((260f - num6) * 0.2f);
					Color color3 = Color.White * 0.2f * MathUtils.Saturate((260f - num7) * 0.2f);
					Color color4 = Color.White * 0.2f * MathUtils.Saturate((260f - num8) * 0.2f);
					flatBatch3D.QueueLine(p, vector, color, color2);
					flatBatch3D.QueueLine(vector, vector2, color2, color3);
					flatBatch3D.QueueLine(vector2, p2, color3, color4);
				}
			}
			float num9 = MathUtils.Lerp(0.3f, 0.75f, 0.5f * (float)MathUtils.Sin(MathUtils.Remainder(1.0 * this.m_subsystemTime.GameTime, 6.2831854820251465)) + 0.5f);
			this.m_lightningStrikeBrightness -= this.m_subsystemTime.GameTimeDelta / num9;
			if (this.m_lightningStrikeBrightness <= 0f)
			{
				this.m_lightningStrikePosition = null;
				this.m_lightningStrikeBrightness = 0f;
			}
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x0004BA78 File Offset: 0x00049C78
		public void DrawClouds(Camera camera)
		{
			if (SettingsManager.SkyRenderingMode == SkyRenderingMode.NoClouds)
			{
				return;
			}
			float globalPrecipitationIntensity = this.m_subsystemWeather.GlobalPrecipitationIntensity;
			float num = MathUtils.Lerp(0.03f, 1f, MathUtils.Sqr(this.SkyLightIntensity)) * MathUtils.Lerp(1f, 0.2f, globalPrecipitationIntensity);
			this.m_cloudsLayerColors[0] = Color.White * (num * 0.75f);
			this.m_cloudsLayerColors[1] = Color.White * (num * 0.66f);
			this.m_cloudsLayerColors[2] = this.ViewFogColor;
			this.m_cloudsLayerColors[3] = Color.Transparent;
			double gameTime = this.m_subsystemTime.GameTime;
			Vector3 viewPosition = camera.ViewPosition;
			Vector2 v = new Vector2((float)MathUtils.Remainder(0.0020000000949949026 * gameTime - (double)(viewPosition.X / 1900f * 1.75f), 1.0) + viewPosition.X / 1900f * 1.75f, (float)MathUtils.Remainder(0.0020000000949949026 * gameTime - (double)(viewPosition.Z / 1900f * 1.75f), 1.0) + viewPosition.Z / 1900f * 1.75f);
			TexturedBatch3D texturedBatch3D = this.m_primitivesRenderer3d.TexturedBatch(this.m_cloudsTexture, false, 2, DepthStencilState.DepthRead, null, BlendState.AlphaBlend, SamplerState.LinearWrap);
			DynamicArray<VertexPositionColorTexture> triangleVertices = texturedBatch3D.TriangleVertices;
			DynamicArray<ushort> triangleIndices = texturedBatch3D.TriangleIndices;
			int count = triangleVertices.Count;
			int count2 = triangleVertices.Count;
			int count3 = triangleIndices.Count;
			triangleVertices.Count += 49;
			triangleIndices.Count += 216;
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					int num2 = j - 3;
					int num3 = i - 3;
					int num4 = MathUtils.Max(MathUtils.Abs(num2), MathUtils.Abs(num3));
					float num5 = this.m_cloudsLayerRadii[num4];
					float num6 = (num4 > 0) ? (num5 / MathUtils.Sqrt((float)(num2 * num2 + num3 * num3))) : 0f;
					float num7 = (float)num2 * num6;
					float num8 = (float)num3 * num6;
					float y = MathUtils.Lerp(600f, 60f, num5 * num5);
					Vector3 vector = new Vector3(viewPosition.X + num7 * 1900f, y, viewPosition.Z + num8 * 1900f);
					Vector2 texCoord = new Vector2(vector.X, vector.Z) / 1900f * 1.75f - v;
					Color color = this.m_cloudsLayerColors[num4];
					texturedBatch3D.TriangleVertices.Array[count2++] = new VertexPositionColorTexture(vector, color, texCoord);
					if (j > 0 && i > 0)
					{
						ushort num9 = (ushort)(count + j + i * 7);
						ushort num10 = (ushort)(count + (j - 1) + i * 7);
						ushort num11 = (ushort)(count + (j - 1) + (i - 1) * 7);
						ushort num12 = (ushort)(count + j + (i - 1) * 7);
						if ((num2 <= 0 && num3 <= 0) || (num2 > 0 && num3 > 0))
						{
							texturedBatch3D.TriangleIndices.Array[count3++] = num9;
							texturedBatch3D.TriangleIndices.Array[count3++] = num10;
							texturedBatch3D.TriangleIndices.Array[count3++] = num11;
							texturedBatch3D.TriangleIndices.Array[count3++] = num11;
							texturedBatch3D.TriangleIndices.Array[count3++] = num12;
							texturedBatch3D.TriangleIndices.Array[count3++] = num9;
						}
						else
						{
							texturedBatch3D.TriangleIndices.Array[count3++] = num9;
							texturedBatch3D.TriangleIndices.Array[count3++] = num10;
							texturedBatch3D.TriangleIndices.Array[count3++] = num12;
							texturedBatch3D.TriangleIndices.Array[count3++] = num10;
							texturedBatch3D.TriangleIndices.Array[count3++] = num11;
							texturedBatch3D.TriangleIndices.Array[count3++] = num12;
						}
					}
				}
			}
			bool drawCloudsWireframe = this.DrawCloudsWireframe;
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x0004BED4 File Offset: 0x0004A0D4
		public void QueueCelestialBody(TexturedBatch3D batch, Vector3 viewPosition, Color color, float distance, float radius, float angle)
		{
			if (color.A > 0)
			{
				Vector3 vector = new Vector3
				{
					X = 0f - MathUtils.Sin(angle),
					Y = 0f - MathUtils.Cos(angle),
					Z = 0f
				};
				Vector3 unitZ = Vector3.UnitZ;
				Vector3 v = Vector3.Cross(unitZ, vector);
				Vector3 p = viewPosition + vector * distance - radius * unitZ - radius * v;
				Vector3 p2 = viewPosition + vector * distance + radius * unitZ - radius * v;
				Vector3 p3 = viewPosition + vector * distance + radius * unitZ + radius * v;
				Vector3 p4 = viewPosition + vector * distance - radius * unitZ + radius * v;
				batch.QueueQuad(p, p2, p3, p4, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f), color);
			}
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x0004C030 File Offset: 0x0004A230
		public void UpdateLightAndViewParameters()
		{
			this.VisibilityRange = (float)SettingsManager.VisibilityRange;
			this.SkyLightIntensity = this.CalculateLightIntensity(this.m_subsystemTimeOfDay.TimeOfDay);
			if (this.MoonPhase == 4)
			{
				this.SkyLightValue = SubsystemSky.m_lightValuesMoonless[(int)MathUtils.Round(MathUtils.Lerp(0f, 5f, this.SkyLightIntensity))];
				return;
			}
			this.SkyLightValue = SubsystemSky.m_lightValuesNormal[(int)MathUtils.Round(MathUtils.Lerp(0f, 5f, this.SkyLightIntensity))];
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x0004C0B8 File Offset: 0x0004A2B8
		public float CalculateLightIntensity(float timeOfDay)
		{
			if (timeOfDay <= 0.2f || timeOfDay > 0.8f)
			{
				return 0f;
			}
			if (timeOfDay > 0.2f && timeOfDay <= 0.3f)
			{
				return (timeOfDay - 0.2f) / 0.10000001f;
			}
			if (timeOfDay > 0.3f && timeOfDay <= 0.7f)
			{
				return 1f;
			}
			return 1f - (timeOfDay - 0.7f) / 0.100000024f;
		}

		// Token: 0x06000A4A RID: 2634 RVA: 0x0004C124 File Offset: 0x0004A324
		public Color CalculateSkyColor(Vector3 direction, float timeOfDay, float precipitationIntensity, int temperature)
		{
			direction = Vector3.Normalize(direction);
			Vector2 vector = Vector2.Normalize(new Vector2(direction.X, direction.Z));
			float s = this.CalculateLightIntensity(timeOfDay);
			float f = (float)temperature / 15f;
			Vector3 v = new Vector3(0.65f, 0.68f, 0.7f) * s;
			Vector3 v2 = Vector3.Lerp(new Vector3(0.28f, 0.38f, 0.52f), new Vector3(0.15f, 0.3f, 0.56f), f);
			Vector3 v3 = Vector3.Lerp(new Vector3(0.7f, 0.79f, 0.88f), new Vector3(0.64f, 0.77f, 0.91f), f);
			Vector3 v4 = Vector3.Lerp(v2, v, precipitationIntensity) * s;
			Vector3 v5 = Vector3.Lerp(v3, v, precipitationIntensity) * s;
			Vector3 v6 = new Vector3(1f, 0.3f, -0.2f);
			Vector3 v7 = new Vector3(1f, 0.3f, -0.2f);
			if (this.m_lightningStrikePosition != null)
			{
				v4 = Vector3.Max(new Vector3(this.m_lightningStrikeBrightness), v4);
			}
			float num = MathUtils.Lerp(SubsystemSky.CalculateDawnGlowIntensity(timeOfDay), 0f, precipitationIntensity);
			float num2 = MathUtils.Lerp(SubsystemSky.CalculateDuskGlowIntensity(timeOfDay), 0f, precipitationIntensity);
			float f2 = MathUtils.Saturate((direction.Y - 0.1f) / 0.4f);
			float s2 = num * MathUtils.Sqr(MathUtils.Saturate(0f - vector.X));
			float s3 = num2 * MathUtils.Sqr(MathUtils.Saturate(vector.X));
			return new Color(Vector3.Lerp(v5 + v6 * s2 + v7 * s3, v4, f2));
		}

		// Token: 0x06000A4B RID: 2635 RVA: 0x0004C2DC File Offset: 0x0004A4DC
		public void FillSkyVertexBuffer(SubsystemSky.SkyDome skyDome, float timeOfDay, float precipitationIntensity, int temperature)
		{
			for (int i = 0; i < 8; i++)
			{
				float x = 1.5707964f * MathUtils.Sqr((float)i / 7f);
				for (int j = 0; j < 10; j++)
				{
					int num = j + i * 10;
					float x2 = 6.2831855f * (float)j / 10f;
					float num2 = 1800f * MathUtils.Cos(x);
					skyDome.Vertices[num].Position.X = num2 * MathUtils.Sin(x2);
					skyDome.Vertices[num].Position.Z = num2 * MathUtils.Cos(x2);
					skyDome.Vertices[num].Position.Y = 1800f * MathUtils.Sin(x) - ((i == 0) ? 450f : 0f);
					skyDome.Vertices[num].Color = this.CalculateSkyColor(skyDome.Vertices[num].Position, timeOfDay, precipitationIntensity, temperature);
				}
			}
			skyDome.VertexBuffer.SetData<SubsystemSky.SkyVertex>(skyDome.Vertices, 0, skyDome.Vertices.Length, 0);
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x0004C400 File Offset: 0x0004A600
		public void FillSkyIndexBuffer(SubsystemSky.SkyDome skyDome)
		{
			int num = 0;
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					int num2 = j;
					int num3 = (j + 1) % 10;
					int num4 = i;
					int num5 = i + 1;
					skyDome.Indices[num++] = (ushort)(num2 + num4 * 10);
					skyDome.Indices[num++] = (ushort)(num3 + num4 * 10);
					skyDome.Indices[num++] = (ushort)(num3 + num5 * 10);
					skyDome.Indices[num++] = (ushort)(num3 + num5 * 10);
					skyDome.Indices[num++] = (ushort)(num2 + num5 * 10);
					skyDome.Indices[num++] = (ushort)(num2 + num4 * 10);
				}
			}
			for (int k = 2; k < 10; k++)
			{
				skyDome.Indices[num++] = 0;
				skyDome.Indices[num++] = (ushort)(k - 1);
				skyDome.Indices[num++] = (ushort)k;
			}
			skyDome.IndexBuffer.SetData<ushort>(skyDome.Indices, 0, skyDome.Indices.Length, 0);
		}

		// Token: 0x06000A4D RID: 2637 RVA: 0x0004C51C File Offset: 0x0004A71C
		public void FillStarsBuffers()
		{
			Game.Random random = new Game.Random(7);
			SubsystemSky.StarVertex[] array = new SubsystemSky.StarVertex[600];
			for (int i = 0; i < 150; i++)
			{
				Vector3 vector;
				do
				{
					vector = new Vector3(random.Float(-1f, 1f), random.Float(-1f, 1f), random.Float(-1f, 1f));
				}
				while (vector.LengthSquared() > 1f);
				vector = Vector3.Normalize(vector);
				float s = 9f * random.NormalFloat(1f, 0.1f);
				float w = MathUtils.Saturate(random.NormalFloat(0.6f, 0.4f));
				Color color = new Color(new Vector4(random.Float(0.6f, 1f), 0.7f, random.Float(0.8f, 1f), w));
				Vector3 v = 900f * vector;
				Vector3 vector2 = Vector3.Normalize(Vector3.Cross((vector.X > vector.Y) ? Vector3.UnitY : Vector3.UnitX, vector));
				Vector3 v2 = Vector3.Normalize(Vector3.Cross(vector2, vector));
				Vector3 position = v + s * (-vector2 - v2);
				Vector3 position2 = v + s * (vector2 - v2);
				Vector3 position3 = v + s * (vector2 + v2);
				Vector3 position4 = v + s * (-vector2 + v2);
				array[i * 4] = new SubsystemSky.StarVertex
				{
					Position = position,
					TextureCoordinate = new Vector2(0f, 0f),
					Color = color
				};
				array[i * 4 + 1] = new SubsystemSky.StarVertex
				{
					Position = position2,
					TextureCoordinate = new Vector2(1f, 0f),
					Color = color
				};
				array[i * 4 + 2] = new SubsystemSky.StarVertex
				{
					Position = position3,
					TextureCoordinate = new Vector2(1f, 1f),
					Color = color
				};
				array[i * 4 + 3] = new SubsystemSky.StarVertex
				{
					Position = position4,
					TextureCoordinate = new Vector2(0f, 1f),
					Color = color
				};
			}
			this.m_starsVertexBuffer.SetData<SubsystemSky.StarVertex>(array, 0, array.Length, 0);
			ushort[] array2 = new ushort[900];
			for (int j = 0; j < 150; j++)
			{
				array2[j * 6] = (ushort)(j * 4);
				array2[j * 6 + 1] = (ushort)(j * 4 + 1);
				array2[j * 6 + 2] = (ushort)(j * 4 + 2);
				array2[j * 6 + 3] = (ushort)(j * 4 + 2);
				array2[j * 6 + 4] = (ushort)(j * 4 + 3);
				array2[j * 6 + 5] = (ushort)(j * 4);
			}
			this.m_starsIndexBuffer.SetData<ushort>(array2, 0, array2.Length, 0);
		}

		// Token: 0x06000A4E RID: 2638 RVA: 0x0004C837 File Offset: 0x0004AA37
		public static float CalculateDawnGlowIntensity(float timeOfDay)
		{
			return MathUtils.Max(1f - MathUtils.Abs(timeOfDay - 0.25f) / 0.10000001f * 2f, 0f);
		}

		// Token: 0x06000A4F RID: 2639 RVA: 0x0004C861 File Offset: 0x0004AA61
		public static float CalculateDuskGlowIntensity(float timeOfDay)
		{
			return MathUtils.Max(1f - MathUtils.Abs(timeOfDay - 0.75f) / 0.100000024f * 2f, 0f);
		}

		// Token: 0x04000575 RID: 1397
		public SubsystemTimeOfDay m_subsystemTimeOfDay;

		// Token: 0x04000576 RID: 1398
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000577 RID: 1399
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000578 RID: 1400
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000579 RID: 1401
		public SubsystemWeather m_subsystemWeather;

		// Token: 0x0400057A RID: 1402
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400057B RID: 1403
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x0400057C RID: 1404
		public SubsystemFluidBlockBehavior m_subsystemFluidBlockBehavior;

		// Token: 0x0400057D RID: 1405
		public PrimitivesRenderer2D m_primitivesRenderer2d = new PrimitivesRenderer2D();

		// Token: 0x0400057E RID: 1406
		public PrimitivesRenderer3D m_primitivesRenderer3d = new PrimitivesRenderer3D();

		// Token: 0x0400057F RID: 1407
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000580 RID: 1408
		public Color m_viewFogColor;

		// Token: 0x04000581 RID: 1409
		public Vector2 m_viewFogRange;

		// Token: 0x04000582 RID: 1410
		public bool m_viewIsSkyVisible;

		// Token: 0x04000583 RID: 1411
		public Texture2D m_sunTexture;

		// Token: 0x04000584 RID: 1412
		public Texture2D m_glowTexture;

		// Token: 0x04000585 RID: 1413
		public Texture2D m_cloudsTexture;

		// Token: 0x04000586 RID: 1414
		public Texture2D[] m_moonTextures = new Texture2D[8];

		// Token: 0x04000587 RID: 1415
		public static UnlitShader m_shaderFlat = new UnlitShader(true, false, false);

		// Token: 0x04000588 RID: 1416
		public static UnlitShader m_shaderTextured = new UnlitShader(true, true, false);

		// Token: 0x04000589 RID: 1417
		public VertexDeclaration m_skyVertexDeclaration = new VertexDeclaration(new VertexElement[]
		{
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementSemantic.Position),
			new VertexElement(12, VertexElementFormat.NormalizedByte4, VertexElementSemantic.Color)
		});

		// Token: 0x0400058A RID: 1418
		public Dictionary<GameWidget, SubsystemSky.SkyDome> m_skyDomes = new Dictionary<GameWidget, SubsystemSky.SkyDome>();

		// Token: 0x0400058B RID: 1419
		public VertexBuffer m_starsVertexBuffer;

		// Token: 0x0400058C RID: 1420
		public IndexBuffer m_starsIndexBuffer;

		// Token: 0x0400058D RID: 1421
		public VertexDeclaration m_starsVertexDeclaration = new VertexDeclaration(new VertexElement[]
		{
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementSemantic.Position),
			new VertexElement(12, VertexElementFormat.Vector2, VertexElementSemantic.TextureCoordinate),
			new VertexElement(20, VertexElementFormat.NormalizedByte4, VertexElementSemantic.Color)
		});

		// Token: 0x0400058E RID: 1422
		public const int m_starsCount = 150;

		// Token: 0x0400058F RID: 1423
		public Vector3? m_lightningStrikePosition;

		// Token: 0x04000590 RID: 1424
		public float m_lightningStrikeBrightness;

		// Token: 0x04000591 RID: 1425
		public double m_lastLightningStrikeTime;

		// Token: 0x04000592 RID: 1426
		public const float DawnStart = 0.2f;

		// Token: 0x04000593 RID: 1427
		public const float DayStart = 0.3f;

		// Token: 0x04000594 RID: 1428
		public const float DuskStart = 0.7f;

		// Token: 0x04000595 RID: 1429
		public const float NightStart = 0.8f;

		// Token: 0x04000596 RID: 1430
		public bool DrawSkyEnabled = true;

		// Token: 0x04000597 RID: 1431
		public bool DrawCloudsWireframe;

		// Token: 0x04000598 RID: 1432
		public bool FogEnabled = ModificationsHolder.fogEnable;

		// Token: 0x04000599 RID: 1433
		public int[] m_drawOrders = new int[]
		{
			-100,
			5,
			105
		};

		// Token: 0x0400059A RID: 1434
		public float[] m_cloudsLayerRadii = new float[]
		{
			0f,
			0.8f,
			0.95f,
			1f
		};

		// Token: 0x0400059B RID: 1435
		public Color[] m_cloudsLayerColors = new Color[5];

		// Token: 0x0400059C RID: 1436
		public static int[] m_lightValuesMoonless = new int[]
		{
			0,
			3,
			6,
			9,
			12,
			15
		};

		// Token: 0x0400059D RID: 1437
		public static int[] m_lightValuesNormal = new int[]
		{
			3,
			5,
			8,
			10,
			13,
			15
		};

		// Token: 0x0200043D RID: 1085
		public struct SkyVertex
		{
			// Token: 0x040015FD RID: 5629
			public Vector3 Position;

			// Token: 0x040015FE RID: 5630
			public Color Color;
		}

		// Token: 0x0200043E RID: 1086
		public class SkyDome : IDisposable
		{
			// Token: 0x06001EAD RID: 7853 RVA: 0x000DFDF7 File Offset: 0x000DDFF7
			public void Dispose()
			{
				Utilities.Dispose<VertexBuffer>(ref this.VertexBuffer);
				Utilities.Dispose<IndexBuffer>(ref this.IndexBuffer);
			}

			// Token: 0x040015FF RID: 5631
			public const int VerticesCountX = 10;

			// Token: 0x04001600 RID: 5632
			public const int VerticesCountY = 8;

			// Token: 0x04001601 RID: 5633
			public float? LastUpdateTimeOfDay;

			// Token: 0x04001602 RID: 5634
			public float? LastUpdatePrecipitationIntensity;

			// Token: 0x04001603 RID: 5635
			public int? LastUpdateTemperature;

			// Token: 0x04001604 RID: 5636
			public float LastUpdateLightningStrikeBrightness;

			// Token: 0x04001605 RID: 5637
			public SubsystemSky.SkyVertex[] Vertices = new SubsystemSky.SkyVertex[80];

			// Token: 0x04001606 RID: 5638
			public ushort[] Indices = new ushort[444];

			// Token: 0x04001607 RID: 5639
			public VertexBuffer VertexBuffer;

			// Token: 0x04001608 RID: 5640
			public IndexBuffer IndexBuffer;
		}

		// Token: 0x0200043F RID: 1087
		public struct StarVertex
		{
			// Token: 0x04001609 RID: 5641
			public Vector3 Position;

			// Token: 0x0400160A RID: 5642
			public Vector2 TextureCoordinate;

			// Token: 0x0400160B RID: 5643
			public Color Color;
		}
	}
}
