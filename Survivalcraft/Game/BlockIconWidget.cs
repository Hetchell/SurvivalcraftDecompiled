using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200036C RID: 876
	public class BlockIconWidget : Widget
	{
		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x060018F3 RID: 6387 RVA: 0x000C56D6 File Offset: 0x000C38D6
		// (set) Token: 0x060018F4 RID: 6388 RVA: 0x000C56DE File Offset: 0x000C38DE
		public DrawBlockEnvironmentData DrawBlockEnvironmentData { get; set; }

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x060018F5 RID: 6389 RVA: 0x000C56E7 File Offset: 0x000C38E7
		// (set) Token: 0x060018F6 RID: 6390 RVA: 0x000C56EF File Offset: 0x000C38EF
		public Vector2 Size { get; set; }

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x060018F7 RID: 6391 RVA: 0x000C56F8 File Offset: 0x000C38F8
		// (set) Token: 0x060018F8 RID: 6392 RVA: 0x000C5700 File Offset: 0x000C3900
		public float Depth { get; set; }

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x060018F9 RID: 6393 RVA: 0x000C5709 File Offset: 0x000C3909
		// (set) Token: 0x060018FA RID: 6394 RVA: 0x000C5711 File Offset: 0x000C3911
		public Color Color { get; set; }

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x060018FB RID: 6395 RVA: 0x000C571A File Offset: 0x000C391A
		// (set) Token: 0x060018FC RID: 6396 RVA: 0x000C5722 File Offset: 0x000C3922
		public Matrix? CustomViewMatrix { get; set; }

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x060018FD RID: 6397 RVA: 0x000C572B File Offset: 0x000C392B
		// (set) Token: 0x060018FE RID: 6398 RVA: 0x000C5734 File Offset: 0x000C3934
		public int Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				if (this.m_value == 0 || value != this.m_value)
				{
					this.m_value = value;
					Block block = BlocksManager.Blocks[this.Contents];
					this.m_viewMatrix = Matrix.CreateLookAt(block.GetIconViewOffset(this.Value, this.DrawBlockEnvironmentData), new Vector3(0f, 0f, 0f), Vector3.UnitY);
				}
			}
		}

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x060018FF RID: 6399 RVA: 0x000C579C File Offset: 0x000C399C
		// (set) Token: 0x06001900 RID: 6400 RVA: 0x000C57A9 File Offset: 0x000C39A9
		public int Contents
		{
			get
			{
				return Terrain.ExtractContents(this.Value);
			}
			set
			{
				this.Value = Terrain.ReplaceContents(this.Value, value);
			}
		}

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06001901 RID: 6401 RVA: 0x000C57BD File Offset: 0x000C39BD
		// (set) Token: 0x06001902 RID: 6402 RVA: 0x000C57CA File Offset: 0x000C39CA
		public int Light
		{
			get
			{
				return Terrain.ExtractLight(this.Value);
			}
			set
			{
				this.Value = Terrain.ReplaceLight(this.Value, value);
			}
		}

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x06001903 RID: 6403 RVA: 0x000C57DE File Offset: 0x000C39DE
		// (set) Token: 0x06001904 RID: 6404 RVA: 0x000C57EB File Offset: 0x000C39EB
		public int Data
		{
			get
			{
				return Terrain.ExtractData(this.Value);
			}
			set
			{
				this.Value = Terrain.ReplaceData(this.Value, value);
			}
		}

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06001905 RID: 6405 RVA: 0x000C57FF File Offset: 0x000C39FF
		// (set) Token: 0x06001906 RID: 6406 RVA: 0x000C5807 File Offset: 0x000C3A07
		public float Scale { get; set; }

		// Token: 0x06001907 RID: 6407 RVA: 0x000C5810 File Offset: 0x000C3A10
		public BlockIconWidget()
		{
			this.Size = new Vector2(float.PositiveInfinity);
			this.IsHitTestVisible = false;
			this.Light = 15;
			this.Depth = 1f;
			this.Color = Color.White;
			this.DrawBlockEnvironmentData = new DrawBlockEnvironmentData();
			this.Scale = 1f;
		}

		// Token: 0x06001908 RID: 6408 RVA: 0x000C5870 File Offset: 0x000C3A70
		public override void Draw(Widget.DrawContext dc)
		{
			Block block = BlocksManager.Blocks[this.Contents];
			if (this.DrawBlockEnvironmentData.SubsystemTerrain != null)
			{
				Texture2D animatedBlocksTexture = this.DrawBlockEnvironmentData.SubsystemTerrain.SubsystemAnimatedTextures.AnimatedBlocksTexture;
			}
			else
			{
				Texture2D defaultBlocksTexture = BlocksTexturesManager.DefaultBlocksTexture;
			}
			Viewport viewport = Display.Viewport;
			float num = MathUtils.Min(base.ActualSize.X, base.ActualSize.Y) * this.Scale;
			Matrix m = Matrix.CreateOrthographic(3.6f, 3.6f, -10f - 1f * this.Depth, 10f - 1f * this.Depth);
			Matrix m2 = MatrixUtils.CreateScaleTranslation(num, 0f - num, base.ActualSize.X / 2f, base.ActualSize.Y / 2f) * base.GlobalTransform * MatrixUtils.CreateScaleTranslation(2f / (float)viewport.Width, -2f / (float)viewport.Height, -1f, 1f);
			this.DrawBlockEnvironmentData.ViewProjectionMatrix = new Matrix?(((this.CustomViewMatrix != null) ? this.CustomViewMatrix.Value : this.m_viewMatrix) * m * m2);
			float iconViewScale = BlocksManager.Blocks[this.Contents].GetIconViewScale(this.Value, this.DrawBlockEnvironmentData);
			Matrix matrix = (this.CustomViewMatrix != null) ? Matrix.Identity : Matrix.CreateTranslation(BlocksManager.Blocks[this.Contents].GetIconBlockOffset(this.Value, this.DrawBlockEnvironmentData));
			block.DrawBlock(dc.PrimitivesRenderer3D, this.Value, base.GlobalColorTransform, iconViewScale, ref matrix, this.DrawBlockEnvironmentData);
		}

		// Token: 0x06001909 RID: 6409 RVA: 0x000C5A3A File Offset: 0x000C3C3A
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			base.DesiredSize = this.Size;
		}

		// Token: 0x04001187 RID: 4487
		public Matrix m_viewMatrix;

		// Token: 0x04001188 RID: 4488
		public int m_value;
	}
}
