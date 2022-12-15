using System;
using Engine;

namespace Game
{
	// Token: 0x02000299 RID: 665
	public class IntroCamera : BasePerspectiveCamera
	{
		// Token: 0x170002CF RID: 719
		// (get) Token: 0x0600135E RID: 4958 RVA: 0x00096FBC File Offset: 0x000951BC
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x0600135F RID: 4959 RVA: 0x00096FBF File Offset: 0x000951BF
		public override bool IsEntityControlEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06001360 RID: 4960 RVA: 0x00096FC2 File Offset: 0x000951C2
		// (set) Token: 0x06001361 RID: 4961 RVA: 0x00096FCA File Offset: 0x000951CA
		public Vector3 CameraPosition { get; set; }

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06001362 RID: 4962 RVA: 0x00096FD3 File Offset: 0x000951D3
		// (set) Token: 0x06001363 RID: 4963 RVA: 0x00096FDB File Offset: 0x000951DB
		public Vector3 TargetPosition { get; set; }

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06001364 RID: 4964 RVA: 0x00096FE4 File Offset: 0x000951E4
		// (set) Token: 0x06001365 RID: 4965 RVA: 0x00096FEC File Offset: 0x000951EC
		public Vector3 TargetCameraPosition { get; set; }

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06001366 RID: 4966 RVA: 0x00096FF5 File Offset: 0x000951F5
		// (set) Token: 0x06001367 RID: 4967 RVA: 0x00096FFD File Offset: 0x000951FD
		public float Speed { get; set; }

		// Token: 0x06001368 RID: 4968 RVA: 0x00097006 File Offset: 0x00095206
		public IntroCamera(GameWidget gameWidget) : base(gameWidget)
		{
			this.Speed = 1f;
		}

		// Token: 0x06001369 RID: 4969 RVA: 0x0009701A File Offset: 0x0009521A
		public override void Activate(Camera previousCamera)
		{
			base.SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		// Token: 0x0600136A RID: 4970 RVA: 0x00097034 File Offset: 0x00095234
		public override void Update(float dt)
		{
			float x = Vector3.Distance(this.TargetCameraPosition, this.CameraPosition);
			this.CameraPosition += MathUtils.Min(dt * this.Speed, x) * Vector3.Normalize(this.TargetCameraPosition - this.CameraPosition);
			base.SetupPerspectiveCamera(this.CameraPosition, this.TargetPosition - this.CameraPosition, Vector3.UnitY);
		}
	}
}
