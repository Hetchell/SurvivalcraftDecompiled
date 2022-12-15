using System;

namespace Game
{
	// Token: 0x02000274 RID: 628
	public class FixedCamera : BasePerspectiveCamera
	{
		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x0600128C RID: 4748 RVA: 0x0008FAFD File Offset: 0x0008DCFD
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x0600128D RID: 4749 RVA: 0x0008FB00 File Offset: 0x0008DD00
		public override bool IsEntityControlEnabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600128E RID: 4750 RVA: 0x0008FB03 File Offset: 0x0008DD03
		public FixedCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x0600128F RID: 4751 RVA: 0x0008FB0C File Offset: 0x0008DD0C
		public override void Activate(Camera previousCamera)
		{
			base.SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		// Token: 0x06001290 RID: 4752 RVA: 0x0008FB26 File Offset: 0x0008DD26
		public override void Update(float dt)
		{
		}
	}
}
