using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000235 RID: 565
	public abstract class Camera
	{
		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06001151 RID: 4433 RVA: 0x000872F2 File Offset: 0x000854F2
		// (set) Token: 0x06001152 RID: 4434 RVA: 0x000872FA File Offset: 0x000854FA
		public GameWidget GameWidget { get; set; }

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06001153 RID: 4435 RVA: 0x00087303 File Offset: 0x00085503
		// (set) Token: 0x06001154 RID: 4436 RVA: 0x0008730B File Offset: 0x0008550B
		public VrEye? Eye { get; set; }

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06001155 RID: 4437
		public abstract Vector3 ViewPosition { get; }

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06001156 RID: 4438
		public abstract Vector3 ViewDirection { get; }

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06001157 RID: 4439
		public abstract Vector3 ViewUp { get; }

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06001158 RID: 4440
		public abstract Vector3 ViewRight { get; }

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06001159 RID: 4441
		public abstract Matrix ViewMatrix { get; }

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x0600115A RID: 4442
		public abstract Matrix InvertedViewMatrix { get; }

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x0600115B RID: 4443
		public abstract Matrix ProjectionMatrix { get; }

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x0600115C RID: 4444
		public abstract Matrix ScreenProjectionMatrix { get; }

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x0600115D RID: 4445
		public abstract Matrix InvertedProjectionMatrix { get; }

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x0600115E RID: 4446
		public abstract Matrix ViewProjectionMatrix { get; }

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x0600115F RID: 4447
		public abstract Vector2 ViewportSize { get; }

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06001160 RID: 4448
		public abstract Matrix ViewportMatrix { get; }

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06001161 RID: 4449
		public abstract BoundingFrustum ViewFrustum { get; }

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06001162 RID: 4450
		public abstract bool UsesMovementControls { get; }

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06001163 RID: 4451
		public abstract bool IsEntityControlEnabled { get; }

		// Token: 0x06001164 RID: 4452 RVA: 0x00087314 File Offset: 0x00085514
		public Camera(GameWidget gameWidget)
		{
			this.GameWidget = gameWidget;
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x00087324 File Offset: 0x00085524
		public Vector3 WorldToScreen(Vector3 worldPoint, Matrix worldMatrix)
		{
			if (this.Eye == null)
			{
				return new Viewport(0, 0, Window.Size.X, Window.Size.Y, 0f, 1f).Project(worldPoint, this.ScreenProjectionMatrix, this.ViewMatrix, worldMatrix);
			}
			return new Viewport(0, 0, (int)this.ViewportSize.X, (int)this.ViewportSize.Y, 0f, 1f).Project(worldPoint, this.ScreenProjectionMatrix, this.ViewMatrix, worldMatrix);
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x000873BC File Offset: 0x000855BC
		public Vector3 ScreenToWorld(Vector3 screenPoint, Matrix worldMatrix)
		{
			if (this.Eye == null)
			{
				return new Viewport(0, 0, Window.Size.X, Window.Size.Y, 0f, 1f).Unproject(screenPoint, this.ScreenProjectionMatrix, this.ViewMatrix, worldMatrix);
			}
			return new Viewport(0, 0, (int)this.ViewportSize.X, (int)this.ViewportSize.Y, 0f, 1f).Unproject(screenPoint, this.ScreenProjectionMatrix, this.ViewMatrix, worldMatrix);
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x00087454 File Offset: 0x00085654
		public virtual void Activate(Camera previousCamera)
		{
		}

		// Token: 0x06001168 RID: 4456
		public abstract void Update(float dt);

		// Token: 0x06001169 RID: 4457 RVA: 0x00087456 File Offset: 0x00085656
		public virtual void PrepareForDrawing(VrEye? eye)
		{
			this.Eye = eye;
		}
	}
}
