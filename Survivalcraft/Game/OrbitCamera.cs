using System;
using Engine;

namespace Game
{
	// Token: 0x020002BE RID: 702
	public class OrbitCamera : BasePerspectiveCamera
	{
		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x060013FE RID: 5118 RVA: 0x0009AB14 File Offset: 0x00098D14
		public override bool UsesMovementControls
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x060013FF RID: 5119 RVA: 0x0009AB17 File Offset: 0x00098D17
		public override bool IsEntityControlEnabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x0009AB1A File Offset: 0x00098D1A
		public OrbitCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x0009AB48 File Offset: 0x00098D48
		public override void Activate(Camera previousCamera)
		{
			base.SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		// Token: 0x06001402 RID: 5122 RVA: 0x0009AB64 File Offset: 0x00098D64
		public override void Update(float dt)
		{
			ComponentPlayer componentPlayer = base.GameWidget.PlayerData.ComponentPlayer;
			if (componentPlayer == null || base.GameWidget.Target == null)
			{
				return;
			}
			ComponentInput componentInput = componentPlayer.ComponentInput;
			Vector3 cameraSneakMove = componentInput.PlayerInput.CameraSneakMove;
			Vector2 cameraLook = componentInput.PlayerInput.CameraLook;
			this.m_angles.X = MathUtils.NormalizeAngle(this.m_angles.X + 4f * cameraLook.X * dt + 0.5f * cameraSneakMove.X * dt);
			this.m_angles.Y = MathUtils.Clamp(MathUtils.NormalizeAngle(this.m_angles.Y + 4f * cameraLook.Y * dt), MathUtils.DegToRad(-20f), MathUtils.DegToRad(70f));
			this.m_distance = MathUtils.Clamp(this.m_distance - 10f * cameraSneakMove.Z * dt, 2f, 16f);
			Vector3 v = Vector3.Transform(new Vector3(this.m_distance, 0f, 0f), Matrix.CreateFromYawPitchRoll(this.m_angles.X, 0f, this.m_angles.Y));
			Vector3 vector = base.GameWidget.Target.ComponentBody.BoundingBox.Center();
			Vector3 vector2 = vector + v;
			if (Vector3.Distance(vector2, this.m_position) < 10f)
			{
				Vector3 v2 = vector2 - this.m_position;
				float s = MathUtils.Saturate(10f * dt);
				this.m_position += s * v2;
			}
			else
			{
				this.m_position = vector2;
			}
			Vector3 vector3 = this.m_position - vector;
			float? num = null;
			Vector3 vector4 = Vector3.Normalize(Vector3.Cross(vector3, Vector3.UnitY));
			Vector3 v3 = Vector3.Normalize(Vector3.Cross(vector3, vector4));
			for (int i = 0; i <= 0; i++)
			{
				for (int j = 0; j <= 0; j++)
				{
					Vector3 v4 = 0.5f * (vector4 * (float)i + v3 * (float)j);
					Vector3 vector5 = vector + v4;
					Vector3 end = vector5 + vector3 + Vector3.Normalize(vector3) * 0.5f;
					TerrainRaycastResult? terrainRaycastResult = base.GameWidget.SubsystemGameWidgets.SubsystemTerrain.Raycast(vector5, end, false, true, (int value, float distance) => !BlocksManager.Blocks[Terrain.ExtractContents(value)].IsTransparent);
					if (terrainRaycastResult != null)
					{
						num = new float?((num != null) ? MathUtils.Min(num.Value, terrainRaycastResult.Value.Distance) : terrainRaycastResult.Value.Distance);
					}
				}
			}
			Vector3 vector6 = (num == null) ? (vector + vector3) : (vector + Vector3.Normalize(vector3) * MathUtils.Max(num.Value - 0.5f, 0.2f));
			base.SetupPerspectiveCamera(vector6, vector - vector6, Vector3.UnitY);
		}

		// Token: 0x04000DCC RID: 3532
		public Vector3 m_position;

		// Token: 0x04000DCD RID: 3533
		public Vector2 m_angles = new Vector2(0f, MathUtils.DegToRad(30f));

		// Token: 0x04000DCE RID: 3534
		public float m_distance = 6f;
	}
}
