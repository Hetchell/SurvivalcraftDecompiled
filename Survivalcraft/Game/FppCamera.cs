using System;
using Engine;

namespace Game
{
	// Token: 0x0200027A RID: 634
	public class FppCamera : BasePerspectiveCamera
	{
		// Token: 0x170002AD RID: 685
		// (get) Token: 0x0600129B RID: 4763 RVA: 0x00090118 File Offset: 0x0008E318
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x0600129C RID: 4764 RVA: 0x0009011B File Offset: 0x0008E31B
		public override bool IsEntityControlEnabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600129D RID: 4765 RVA: 0x0009011E File Offset: 0x0008E31E
		public FppCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x0600129E RID: 4766 RVA: 0x00090127 File Offset: 0x0008E327
		public override void Activate(Camera previousCamera)
		{
			base.SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		// Token: 0x0600129F RID: 4767 RVA: 0x00090144 File Offset: 0x0008E344
		public override void Update(float dt)
		{
			if (base.GameWidget.Target != null)
			{
				if (base.Eye == null)
				{
					Matrix matrix = Matrix.CreateFromQuaternion(base.GameWidget.Target.ComponentCreatureModel.EyeRotation);
					matrix.Translation = base.GameWidget.Target.ComponentCreatureModel.EyePosition;
					base.SetupPerspectiveCamera(matrix.Translation, matrix.Forward, matrix.Up);
					return;
				}
				Vector3 translation = VrManager.HmdMatrix.Translation;
				Vector3 position = base.GameWidget.Target.ComponentBody.Position;
				float y = position.Y + MathUtils.Clamp(translation.Y, 0.2f, base.GameWidget.Target.ComponentBody.BoxSize.Y - 0.1f);
				Vector3 hmdMatrixYpr = VrManager.HmdMatrixYpr;
				Vector3 vector = base.GameWidget.Target.ComponentCreatureModel.EyeRotation.ToYawPitchRoll();
				float radians = vector.X - hmdMatrixYpr.X;
				Matrix identity = Matrix.Identity;
				identity.Translation = new Vector3(position.X, y, position.Z);
				identity.OrientationMatrix = VrManager.HmdMatrix * Matrix.CreateRotationY(radians);
				identity.OrientationMatrix *= Matrix.CreateFromAxisAngle(identity.OrientationMatrix.Forward, vector.Z);
				base.SetupPerspectiveCamera(identity.Translation, identity.Forward, identity.Up);
			}
		}
	}
}
