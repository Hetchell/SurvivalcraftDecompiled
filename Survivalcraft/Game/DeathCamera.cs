using System;
using Engine;

namespace Game
{
	// Token: 0x02000248 RID: 584
	public class DeathCamera : BasePerspectiveCamera
	{
		// Token: 0x1700028C RID: 652
		// (get) Token: 0x060011C4 RID: 4548 RVA: 0x00089918 File Offset: 0x00087B18
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x060011C5 RID: 4549 RVA: 0x0008991B File Offset: 0x00087B1B
		public override bool IsEntityControlEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060011C6 RID: 4550 RVA: 0x0008991E File Offset: 0x00087B1E
		public DeathCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x00089928 File Offset: 0x00087B28
		public override void Activate(Camera previousCamera)
		{
			this.m_position = previousCamera.ViewPosition;
			ComponentCreature target = base.GameWidget.Target;
			Vector3 vector = (target != null) ? target.ComponentBody.BoundingBox.Center() : this.m_position;
			this.m_bestPosition = new Vector3?(this.FindBestCameraPosition(vector, 6f));
			base.SetupPerspectiveCamera(this.m_position, vector - this.m_position, Vector3.UnitY);
			ComponentPlayer componentPlayer = base.GameWidget.Target as ComponentPlayer;
			if (componentPlayer != null && componentPlayer.ComponentInput.IsControlledByVr && this.m_bestPosition != null)
			{
				Vector3 vector2 = Matrix.CreateWorld(Vector3.Zero, vector - this.m_bestPosition.Value, Vector3.UnitY).ToYawPitchRoll();
				this.m_vrDeltaYaw = vector2.X - VrManager.HmdMatrixYpr.X;
			}
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x00089A10 File Offset: 0x00087C10
		public override void Update(float dt)
		{
			ComponentCreature target = base.GameWidget.Target;
			Vector3 v = (target != null) ? target.ComponentBody.BoundingBox.Center() : this.m_position;
			if (this.m_bestPosition != null)
			{
				if (Vector3.Distance(this.m_bestPosition.Value, this.m_position) > 20f)
				{
					this.m_position = this.m_bestPosition.Value;
				}
				this.m_position += 1.5f * dt * (this.m_bestPosition.Value - this.m_position);
			}
			if (base.Eye == null)
			{
				base.SetupPerspectiveCamera(this.m_position, v - this.m_position, Vector3.UnitY);
				return;
			}
			Matrix identity = Matrix.Identity;
			identity.Translation = this.m_position;
			identity.OrientationMatrix = VrManager.HmdMatrix * Matrix.CreateRotationY(this.m_vrDeltaYaw);
			base.SetupPerspectiveCamera(identity.Translation, identity.Forward, identity.Up);
		}

		// Token: 0x060011C9 RID: 4553 RVA: 0x00089B30 File Offset: 0x00087D30
		public Vector3 FindBestCameraPosition(Vector3 targetPosition, float distance)
		{
			Vector3? vector = null;
			for (int i = 0; i < 36; i++)
			{
				float x = 1f + 6.2831855f * (float)i / 36f;
				Vector3 v2 = Vector3.Normalize(new Vector3(MathUtils.Sin(x), 0.5f, MathUtils.Cos(x)));
				Vector3 vector2 = targetPosition + v2 * distance;
				TerrainRaycastResult? terrainRaycastResult = base.GameWidget.SubsystemGameWidgets.SubsystemTerrain.Raycast(targetPosition, vector2, false, true, (int v, float d) => !BlocksManager.Blocks[Terrain.ExtractContents(v)].IsTransparent);
				Vector3 vector3 = Vector3.Zero;
				if (terrainRaycastResult != null)
				{
					CellFace cellFace = terrainRaycastResult.Value.CellFace;
					vector3 = new Vector3((float)cellFace.X + 0.5f, (float)cellFace.Y + 0.5f, (float)cellFace.Z + 0.5f) - 1f * v2;
				}
				else
				{
					vector3 = vector2;
				}
				if (vector == null || Vector3.Distance(vector3, targetPosition) > Vector3.Distance(vector.Value, targetPosition))
				{
					vector = new Vector3?(vector3);
				}
			}
			if (vector != null)
			{
				return vector.Value;
			}
			return targetPosition;
		}

		// Token: 0x04000BF6 RID: 3062
		public Vector3 m_position;

		// Token: 0x04000BF7 RID: 3063
		public Vector3? m_bestPosition;

		// Token: 0x04000BF8 RID: 3064
		public float m_vrDeltaYaw;
	}
}
