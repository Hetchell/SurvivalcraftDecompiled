using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000158 RID: 344
	public class SubsystemAnimatedTextures : Subsystem, IUpdateable
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000692 RID: 1682 RVA: 0x00029CA8 File Offset: 0x00027EA8
		public Texture2D AnimatedBlocksTexture
		{
			get
			{
				if (this.DisableTextureAnimation || this.m_animatedBlocksTexture == null)
				{
					return this.m_subsystemBlocksTexture.BlocksTexture;
				}
				return this.m_animatedBlocksTexture;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000693 RID: 1683 RVA: 0x00029CCC File Offset: 0x00027ECC
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x00029CD0 File Offset: 0x00027ED0
		public void Update(float dt)
		{
			if (!this.DisableTextureAnimation && this.m_subsystemTime.FixedTimeStep == null)
			{
				float dt2 = (float)MathUtils.Min(this.m_subsystemTime.GameTime - this.m_lastAnimateGameTime, 1.0);
				this.m_lastAnimateGameTime = this.m_subsystemTime.GameTime;
				Texture2D blocksTexture = this.m_subsystemBlocksTexture.BlocksTexture;
				if (this.m_animatedBlocksTexture == null || this.m_animatedBlocksTexture.Width != blocksTexture.Width || this.m_animatedBlocksTexture.Height != blocksTexture.Height || this.m_animatedBlocksTexture.MipLevelsCount > 1 != SettingsManager.TerrainMipmapsEnabled)
				{
					Utilities.Dispose<RenderTarget2D>(ref this.m_animatedBlocksTexture);
					this.m_animatedBlocksTexture = new RenderTarget2D(blocksTexture.Width, blocksTexture.Height, (!SettingsManager.TerrainMipmapsEnabled) ? 1 : 4, ColorFormat.Rgba8888, DepthFormat.None);
				}
				Rectangle scissorRectangle = Display.ScissorRectangle;
				RenderTarget2D renderTarget = Display.RenderTarget;
				Display.RenderTarget = this.m_animatedBlocksTexture;
				try
				{
					Display.Clear(new Vector4?(new Vector4(Color.Transparent)), null, null);
					this.m_primitivesRenderer.TexturedBatch(blocksTexture, false, -1, DepthStencilState.None, RasterizerState.CullNone, BlendState.Opaque, SamplerState.PointClamp).QueueQuad(new Vector2(0f, 0f), new Vector2((float)this.m_animatedBlocksTexture.Width, (float)this.m_animatedBlocksTexture.Height), 0f, Vector2.Zero, Vector2.One, Color.White);
					this.AnimateWaterBlocksTexture();
					this.AnimateMagmaBlocksTexture();
					this.m_primitivesRenderer.Flush(true, int.MaxValue);
					Display.ScissorRectangle = this.AnimateFireBlocksTexture(dt2);
					this.m_primitivesRenderer.Flush(true, int.MaxValue);
				}
				finally
				{
					Display.RenderTarget = renderTarget;
					Display.ScissorRectangle = scissorRectangle;
				}
				if (SettingsManager.TerrainMipmapsEnabled && Time.FrameIndex % 2 == 0)
				{
					this.m_animatedBlocksTexture.GenerateMipMaps();
				}
			}
		}

        // Token: 0x06000695 RID: 1685 RVA: 0x00029ECC File Offset: 0x000280CC
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemBlocksTexture = base.Project.FindSubsystem<SubsystemBlocksTexture>(true);
			Display.DeviceReset += this.Display_DeviceReset;
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x00029F03 File Offset: 0x00028103
		public override void Dispose()
		{
			Utilities.Dispose<RenderTarget2D>(ref this.m_animatedBlocksTexture);
			Display.DeviceReset -= this.Display_DeviceReset;
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x00029F21 File Offset: 0x00028121
		public void Display_DeviceReset()
		{
			this.m_animatedBlocksTexture = null;
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x00029F2C File Offset: 0x0002812C
		public void AnimateWaterBlocksTexture()
		{
			TexturedBatch2D batch = this.m_primitivesRenderer.TexturedBatch(this.m_subsystemBlocksTexture.BlocksTexture, false, 0, DepthStencilState.None, null, BlendState.AlphaBlend, SamplerState.PointClamp);
			int num = BlocksManager.Blocks[18].DefaultTextureSlot % 16;
			int num2 = BlocksManager.Blocks[18].DefaultTextureSlot / 16;
			double num3 = 1.0 * this.m_subsystemTime.GameTime;
			double num4 = 1.0 * (this.m_subsystemTime.GameTime - (double)this.m_subsystemTime.GameTimeDelta);
			float num5 = MathUtils.Min((float)MathUtils.Remainder(num3, 2.0), 1f);
			float num6 = MathUtils.Min((float)MathUtils.Remainder(num3 + 1.0, 2.0), 1f);
			byte b = (byte)(255f * num5);
			byte b2 = (byte)(255f * num6);
			if (MathUtils.Remainder(num3, 2.0) >= 1.0 && MathUtils.Remainder(num4, 2.0) < 1.0)
			{
				this.m_waterOrder = true;
				this.m_waterOffset2 = new Vector2(this.m_random.Float(0f, 1f), this.m_random.Float(0f, 1f));
			}
			else if (MathUtils.Remainder(num3 + 1.0, 2.0) >= 1.0 && MathUtils.Remainder(num4 + 1.0, 2.0) < 1.0)
			{
				this.m_waterOrder = false;
				this.m_waterOffset1 = new Vector2(this.m_random.Float(0f, 1f), this.m_random.Float(0f, 1f));
			}
			Vector2 tcOffset = new Vector2((float)num, (float)num2) - (this.m_waterOrder ? this.m_waterOffset1 : this.m_waterOffset2);
			Vector2 tcOffset2 = new Vector2((float)num, (float)num2) - (this.m_waterOrder ? this.m_waterOffset2 : this.m_waterOffset1);
			Color color = this.m_waterOrder ? new Color(b, b, b, b) : new Color(b2, b2, b2, b2);
			Color color2 = this.m_waterOrder ? new Color(b2, b2, b2, b2) : new Color(b, b, b, b);
			float num7 = MathUtils.Floor((float)MathUtils.Remainder(1.75 * this.m_subsystemTime.GameTime, 1.0) * 16f) / 16f;
			float num8 = 0f - num7 + 1f;
			float num9 = MathUtils.Floor((float)MathUtils.Remainder((double)(1.75f / MathUtils.Sqrt(2f)) * this.m_subsystemTime.GameTime, 1.0) * 16f) / 16f;
			float num10 = 0f - num9 + 1f;
			Vector2 tc = new Vector2(0f, 0f);
			Vector2 tc2 = new Vector2(1f, 1f);
			this.DrawBlocksTextureSlot(batch, num, num2, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num, num2, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num7, 0f);
			tc2 = new Vector2(num7 + 1f, 1f);
			this.DrawBlocksTextureSlot(batch, num - 1, num2, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num - 1, num2, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num8, 0f);
			tc2 = new Vector2(num8 + 1f, 1f);
			this.DrawBlocksTextureSlot(batch, num + 1, num2, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num + 1, num2, tc, tc2, tcOffset2, color2);
			tc = new Vector2(0f, num7);
			tc2 = new Vector2(1f, num7 + 1f);
			this.DrawBlocksTextureSlot(batch, num, num2 - 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num, num2 - 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(0f, num8);
			tc2 = new Vector2(1f, num8 + 1f);
			this.DrawBlocksTextureSlot(batch, num, num2 + 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num, num2 + 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num9, num10);
			tc2 = new Vector2(num9 + 1f, num10 + 1f);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 + 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 + 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num10, num10);
			tc2 = new Vector2(num10 + 1f, num10 + 1f);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 + 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 + 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num9, num9);
			tc2 = new Vector2(num9 + 1f, num9 + 1f);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 - 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 - 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num10, num9);
			tc2 = new Vector2(num10 + 1f, num9 + 1f);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 - 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 - 1, tc, tc2, tcOffset2, color2);
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x0002A4D4 File Offset: 0x000286D4
		public void AnimateMagmaBlocksTexture()
		{
			TexturedBatch2D batch = this.m_primitivesRenderer.TexturedBatch(this.m_subsystemBlocksTexture.BlocksTexture, false, 0, DepthStencilState.None, null, BlendState.AlphaBlend, SamplerState.PointClamp);
			int num = BlocksManager.Blocks[92].DefaultTextureSlot % 16;
			int num2 = BlocksManager.Blocks[92].DefaultTextureSlot / 16;
			double num3 = 0.5 * this.m_subsystemTime.GameTime;
			double num4 = 0.5 * (this.m_subsystemTime.GameTime - (double)this.m_subsystemTime.GameTimeDelta);
			float num5 = MathUtils.Min((float)MathUtils.Remainder(num3, 2.0), 1f);
			float num6 = MathUtils.Min((float)MathUtils.Remainder(num3 + 1.0, 2.0), 1f);
			byte b = (byte)(255f * num5);
			byte b2 = (byte)(255f * num6);
			if (MathUtils.Remainder(num3, 2.0) >= 1.0 && MathUtils.Remainder(num4, 2.0) < 1.0)
			{
				this.m_magmaOrder = true;
				this.m_magmaOffset2 = new Vector2(this.m_random.Float(0f, 1f), this.m_random.Float(0f, 1f));
			}
			else if (MathUtils.Remainder(num3 + 1.0, 2.0) >= 1.0 && MathUtils.Remainder(num4 + 1.0, 2.0) < 1.0)
			{
				this.m_magmaOrder = false;
				this.m_magmaOffset1 = new Vector2(this.m_random.Float(0f, 1f), this.m_random.Float(0f, 1f));
			}
			Vector2 tcOffset = new Vector2((float)num, (float)num2) - (this.m_magmaOrder ? this.m_magmaOffset1 : this.m_magmaOffset2);
			Vector2 tcOffset2 = new Vector2((float)num, (float)num2) - (this.m_magmaOrder ? this.m_magmaOffset2 : this.m_magmaOffset1);
			Color color = this.m_magmaOrder ? new Color(b, b, b, b) : new Color(b2, b2, b2, b2);
			Color color2 = this.m_magmaOrder ? new Color(b2, b2, b2, b2) : new Color(b, b, b, b);
			float num7 = MathUtils.Floor((float)MathUtils.Remainder(0.4000000059604645 * this.m_subsystemTime.GameTime, 1.0) * 16f) / 16f;
			float num8 = 0f - num7 + 1f;
			float num9 = MathUtils.Floor((float)MathUtils.Remainder((double)(0.4f / MathUtils.Sqrt(2f)) * this.m_subsystemTime.GameTime, 1.0) * 16f) / 16f;
			float num10 = 0f - num9 + 1f;
			Vector2 tc = new Vector2(0f, 0f);
			Vector2 tc2 = new Vector2(1f, 1f);
			this.DrawBlocksTextureSlot(batch, num, num2, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num, num2, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num7, 0f);
			tc2 = new Vector2(num7 + 1f, 1f);
			this.DrawBlocksTextureSlot(batch, num - 1, num2, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num - 1, num2, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num8, 0f);
			tc2 = new Vector2(num8 + 1f, 1f);
			this.DrawBlocksTextureSlot(batch, num + 1, num2, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num + 1, num2, tc, tc2, tcOffset2, color2);
			tc = new Vector2(0f, num7);
			tc2 = new Vector2(1f, num7 + 1f);
			this.DrawBlocksTextureSlot(batch, num, num2 - 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num, num2 - 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(0f, num8);
			tc2 = new Vector2(1f, num8 + 1f);
			this.DrawBlocksTextureSlot(batch, num, num2 + 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num, num2 + 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num9, num10);
			tc2 = new Vector2(num9 + 1f, num10 + 1f);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 + 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 + 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num10, num10);
			tc2 = new Vector2(num10 + 1f, num10 + 1f);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 + 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 + 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num9, num9);
			tc2 = new Vector2(num9 + 1f, num9 + 1f);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 - 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num - 1, num2 - 1, tc, tc2, tcOffset2, color2);
			tc = new Vector2(num10, num9);
			tc2 = new Vector2(num10 + 1f, num9 + 1f);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 - 1, tc, tc2, tcOffset, color);
			this.DrawBlocksTextureSlot(batch, num + 1, num2 - 1, tc, tc2, tcOffset2, color2);
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x0002AA7C File Offset: 0x00028C7C
		public Rectangle AnimateFireBlocksTexture(float dt)
		{
			int defaultTextureSlot = BlocksManager.Blocks[104].DefaultTextureSlot;
			float num = (float)(this.m_animatedBlocksTexture.Width / 16);
			int num2 = defaultTextureSlot % 16;
			int num3 = defaultTextureSlot / 16;
			this.m_screenSpaceFireRenderer.ParticleSize = 1f * num;
			this.m_screenSpaceFireRenderer.ParticleSpeed = 1.9f * num;
			this.m_screenSpaceFireRenderer.ParticlesPerSecond = 24f;
			this.m_screenSpaceFireRenderer.MinTimeToLive = float.PositiveInfinity;
			this.m_screenSpaceFireRenderer.MaxTimeToLive = float.PositiveInfinity;
			this.m_screenSpaceFireRenderer.ParticleAnimationOffset = 1f;
			this.m_screenSpaceFireRenderer.ParticleAnimationPeriod = 3f;
			this.m_screenSpaceFireRenderer.Origin = new Vector2((float)num2, (float)(num3 + 3)) * num + new Vector2(0f, 0.5f * this.m_screenSpaceFireRenderer.ParticleSize);
			this.m_screenSpaceFireRenderer.Width = num;
			this.m_screenSpaceFireRenderer.CutoffPosition = (float)num3 * num;
			this.m_screenSpaceFireRenderer.Update(dt);
			this.m_screenSpaceFireRenderer.Draw(this.m_primitivesRenderer, 0f, Matrix.Identity, Color.White);
			return new Rectangle((int)((float)num2 * num), (int)((float)num3 * num), (int)num, (int)(num * 3f));
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x0002ABC0 File Offset: 0x00028DC0
		public void DrawBlocksTextureSlot(TexturedBatch2D batch, int slotX, int slotY, Vector2 tc1, Vector2 tc2, Vector2 tcOffset, Color color)
		{
			float s = (float)this.m_animatedBlocksTexture.Width / 16f;
			batch.QueueQuad(new Vector2((float)slotX, (float)slotY) * s, new Vector2((float)(slotX + 1), (float)(slotY + 1)) * s, 0f, (tc1 + tcOffset) / 16f, (tc2 + tcOffset) / 16f, color);
		}

		// Token: 0x040003AF RID: 943
		public SubsystemTime m_subsystemTime;

		// Token: 0x040003B0 RID: 944
		public SubsystemBlocksTexture m_subsystemBlocksTexture;

		// Token: 0x040003B1 RID: 945
		public RenderTarget2D m_animatedBlocksTexture;

		// Token: 0x040003B2 RID: 946
		public PrimitivesRenderer2D m_primitivesRenderer = new PrimitivesRenderer2D();

		// Token: 0x040003B3 RID: 947
		public ScreenSpaceFireRenderer m_screenSpaceFireRenderer = new ScreenSpaceFireRenderer(200);

		// Token: 0x040003B4 RID: 948
		public Game.Random m_random = new Game.Random();

		// Token: 0x040003B5 RID: 949
		public bool m_waterOrder;

		// Token: 0x040003B6 RID: 950
		public Vector2 m_waterOffset1;

		// Token: 0x040003B7 RID: 951
		public Vector2 m_waterOffset2;

		// Token: 0x040003B8 RID: 952
		public bool m_magmaOrder;

		// Token: 0x040003B9 RID: 953
		public Vector2 m_magmaOffset1;

		// Token: 0x040003BA RID: 954
		public Vector2 m_magmaOffset2;

		// Token: 0x040003BB RID: 955
		public double m_lastAnimateGameTime;

		// Token: 0x040003BC RID: 956
		public bool DisableTextureAnimation;

		// Token: 0x040003BD RID: 957
		public bool ShowAnimatedTexture;
	}
}
