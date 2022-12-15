using System;
using Engine;
using Engine.Graphics;
using Engine.Input;

namespace Game
{
	// Token: 0x02000249 RID: 585
	public class DebugCamera : BasePerspectiveCamera
	{
		// Token: 0x1700028E RID: 654
		// (get) Token: 0x060011CA RID: 4554 RVA: 0x00089C73 File Offset: 0x00087E73
		public override bool UsesMovementControls
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x060011CB RID: 4555 RVA: 0x00089C76 File Offset: 0x00087E76
		public override bool IsEntityControlEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060011CC RID: 4556 RVA: 0x00089C79 File Offset: 0x00087E79
		public DebugCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x060011CD RID: 4557 RVA: 0x00089C8D File Offset: 0x00087E8D
		public override void Activate(Camera previousCamera)
		{
			this.m_position = previousCamera.ViewPosition;
			this.m_direction = previousCamera.ViewDirection;
			base.SetupPerspectiveCamera(this.m_position, this.m_direction, Vector3.UnitY);
		}

		// Token: 0x060011CE RID: 4558 RVA: 0x00089CC0 File Offset: 0x00087EC0
		public override void Update(float dt)
		{
			dt = MathUtils.Min(dt, 0.1f);
			Vector3 zero = Vector3.Zero;
			if (Keyboard.IsKeyDown(Key.CapsLock))
			{
				zero.X = -1f;
			}
			if (Keyboard.IsKeyDown(Key.C))
			{
				zero.X = 1f;
			}
			if (Keyboard.IsKeyDown(Key.V))
			{
				zero.Z = 1f;
			}
			if (Keyboard.IsKeyDown(Key.R))
			{
				zero.Z = -1f;
			}
			Vector2 vector = 0.03f * new Vector2((float)Mouse.MouseMovement.X, (float)(-(float)Mouse.MouseMovement.Y));
			bool flag = Keyboard.IsKeyDown(Key.Shift);
			bool flag2 = Keyboard.IsKeyDown(Key.Control);
			Vector3 direction = this.m_direction;
			Vector3 unitY = Vector3.UnitY;
			Vector3 vector2 = Vector3.Normalize(Vector3.Cross(direction, unitY));
			float num = 8f;
			if (flag)
			{
				num *= 10f;
			}
			if (flag2)
			{
				num /= 10f;
			}
			Vector3 vector3 = Vector3.Zero;
			vector3 += num * zero.X * vector2;
			vector3 += num * zero.Y * unitY;
			vector3 += num * zero.Z * direction;
			this.m_position += vector3 * dt;
			this.m_direction = Vector3.Transform(this.m_direction, Matrix.CreateFromAxisAngle(unitY, -4f * vector.X * dt));
			this.m_direction = Vector3.Transform(this.m_direction, Matrix.CreateFromAxisAngle(vector2, 4f * vector.Y * dt));
			base.SetupPerspectiveCamera(this.m_position, this.m_direction, Vector3.UnitY);
			Vector2 v = this.ViewportSize / 2f;
			FlatBatch2D flatBatch2D = this.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null);
			int count = flatBatch2D.LineVertices.Count;
			flatBatch2D.QueueLine(v - new Vector2(5f, 0f), v + new Vector2(5f, 0f), 0f, Color.White);
			flatBatch2D.QueueLine(v - new Vector2(0f, 5f), v + new Vector2(0f, 5f), 0f, Color.White);
			flatBatch2D.TransformLines(this.ViewportMatrix, count, -1);
			this.PrimitivesRenderer2D.Flush(true, int.MaxValue);
		}

		// Token: 0x04000BF9 RID: 3065
		public static string AmbientParameters = string.Empty;

		// Token: 0x04000BFA RID: 3066
		public static string PlantParameters = string.Empty;

		// Token: 0x04000BFB RID: 3067
		public Vector3 m_position;

		// Token: 0x04000BFC RID: 3068
		public Vector3 m_direction;

		// Token: 0x04000BFD RID: 3069
		public PrimitivesRenderer2D PrimitivesRenderer2D = new PrimitivesRenderer2D();
	}
}
