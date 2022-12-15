using System;
using Engine;

namespace Game
{
	// Token: 0x020002A6 RID: 678
	public class LoadingCamera : BasePerspectiveCamera
	{
		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06001395 RID: 5013 RVA: 0x00098106 File Offset: 0x00096306
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06001396 RID: 5014 RVA: 0x00098109 File Offset: 0x00096309
		public override bool IsEntityControlEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001397 RID: 5015 RVA: 0x0009810C File Offset: 0x0009630C
		public LoadingCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x00098115 File Offset: 0x00096315
		public override void Activate(Camera previousCamera)
		{
			base.SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		// Token: 0x06001399 RID: 5017 RVA: 0x0009812F File Offset: 0x0009632F
		public override void Update(float dt)
		{
			base.SetupPerspectiveCamera(base.GameWidget.PlayerData.SpawnPosition, Vector3.UnitX, Vector3.UnitY);
		}
	}
}
