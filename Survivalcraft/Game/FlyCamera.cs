using System;
using Engine;
using Engine.Input;

namespace Game
{
	// Token: 0x02000277 RID: 631
	public class FlyCamera : BasePerspectiveCamera
	{
		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06001292 RID: 4754 RVA: 0x0008FB4E File Offset: 0x0008DD4E
		public override bool UsesMovementControls
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06001293 RID: 4755 RVA: 0x0008FB51 File Offset: 0x0008DD51
		public override bool IsEntityControlEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001294 RID: 4756 RVA: 0x0008FB54 File Offset: 0x0008DD54
		public FlyCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x06001295 RID: 4757 RVA: 0x0008FB5D File Offset: 0x0008DD5D
		public override void Activate(Camera previousCamera)
		{
			this.m_position = previousCamera.ViewPosition;
			this.m_direction = previousCamera.ViewDirection;
			base.SetupPerspectiveCamera(this.m_position, this.m_direction, Vector3.UnitY);
		}

		// Token: 0x06001296 RID: 4758 RVA: 0x0008FB90 File Offset: 0x0008DD90
		public override void Update(float dt)
		{
			Vector3 vector = Vector3.Zero;
			Vector2 vector2 = Vector2.Zero;
			ComponentPlayer componentPlayer = base.GameWidget.PlayerData.ComponentPlayer;
			ComponentInput componentInput = (componentPlayer != null) ? componentPlayer.ComponentInput : null;
			if (componentInput != null)
			{
				vector = componentInput.PlayerInput.CameraMove * new Vector3(1f, 0f, 1f);
				vector2 = componentInput.PlayerInput.CameraLook;
			}
			bool flag = Keyboard.IsKeyDown(Key.Shift);
			bool flag2 = Keyboard.IsKeyDown(Key.Control);
			Vector3 direction = this.m_direction;
			Vector3 unitY = Vector3.UnitY;
			Vector3 vector3 = Vector3.Normalize(Vector3.Cross(direction, unitY));
			float num = 10f;
			if (flag)
			{
				num *= 5f;
			}
			if (flag2)
			{
				num /= 5f;
			}
			Vector3 v = Vector3.Zero;
			v += num * vector.X * vector3;
			v += num * vector.Y * unitY;
			v += num * vector.Z * direction;
			this.m_rollSpeed = MathUtils.Lerp(this.m_rollSpeed, -1.5f * vector2.X, 3f * dt);
			this.m_rollAngle += this.m_rollSpeed * dt;
			this.m_rollAngle *= MathUtils.Pow(0.33f, dt);
			this.m_pitchSpeed = MathUtils.Lerp(this.m_pitchSpeed, -0.2f * vector2.Y, 3f * dt);
			this.m_pitchSpeed *= MathUtils.Pow(0.33f, dt);
			this.m_velocity += 1.5f * (v - this.m_velocity) * dt;
			this.m_position += this.m_velocity * dt;
			this.m_direction = Vector3.Transform(this.m_direction, Matrix.CreateFromAxisAngle(unitY, 0.05f * this.m_rollAngle));
			this.m_direction = Vector3.Transform(this.m_direction, Matrix.CreateFromAxisAngle(vector3, 0.2f * this.m_pitchSpeed));
			Vector3 up = Vector3.TransformNormal(Vector3.UnitY, Matrix.CreateFromAxisAngle(this.m_direction, 0f - this.m_rollAngle));
			base.SetupPerspectiveCamera(this.m_position, this.m_direction, up);
		}

		// Token: 0x04000CC9 RID: 3273
		public Vector3 m_position;

		// Token: 0x04000CCA RID: 3274
		public Vector3 m_direction;

		// Token: 0x04000CCB RID: 3275
		public Vector3 m_velocity;

		// Token: 0x04000CCC RID: 3276
		public float m_rollSpeed;

		// Token: 0x04000CCD RID: 3277
		public float m_pitchSpeed;

		// Token: 0x04000CCE RID: 3278
		public float m_rollAngle;
	}
}
