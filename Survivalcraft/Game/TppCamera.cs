using System;
using Engine;

namespace Game
{
	// Token: 0x02000325 RID: 805
	public class TppCamera : BasePerspectiveCamera
	{
		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06001709 RID: 5897 RVA: 0x000B835D File Offset: 0x000B655D
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x0600170A RID: 5898 RVA: 0x000B8360 File Offset: 0x000B6560
		public override bool IsEntityControlEnabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600170B RID: 5899 RVA: 0x000B8363 File Offset: 0x000B6563
		public TppCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x000B836C File Offset: 0x000B656C
		public override void Activate(Camera previousCamera)
		{
			this.m_position = previousCamera.ViewPosition;
			base.SetupPerspectiveCamera(this.m_position, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x000B8394 File Offset: 0x000B6594
		public override void Update(float dt)
		{
			if (base.GameWidget.Target == null)
			{
				return;
			}
			Matrix matrix = Matrix.CreateFromQuaternion(base.GameWidget.Target.ComponentCreatureModel.EyeRotation);
			matrix.Translation = base.GameWidget.Target.ComponentBody.Position + 0.5f * base.GameWidget.Target.ComponentBody.BoxSize.Y * Vector3.UnitY;
			Vector3 v = -2.25f * matrix.Forward + 1.75f * matrix.Up;
			Vector3 vector = matrix.Translation + v;
			if (Vector3.Distance(vector, this.m_position) < 10f)
			{
				Vector3 v2 = vector - this.m_position;
				float s = 3f * dt;
				this.m_position += s * v2;
			}
			else
			{
				this.m_position = vector;
			}
			Vector3 vector2 = this.m_position - matrix.Translation;
			float? num = null;
			Vector3 vector3 = Vector3.Normalize(Vector3.Cross(vector2, Vector3.UnitY));
			Vector3 v3 = Vector3.Normalize(Vector3.Cross(vector2, vector3));
			for (int i = 0; i <= 0; i++)
			{
				for (int j = 0; j <= 0; j++)
				{
					Vector3 v4 = 0.5f * (vector3 * (float)i + v3 * (float)j);
					Vector3 vector4 = matrix.Translation + v4;
					Vector3 end = vector4 + vector2 + Vector3.Normalize(vector2) * 0.5f;
					TerrainRaycastResult? terrainRaycastResult = base.GameWidget.SubsystemGameWidgets.SubsystemTerrain.Raycast(vector4, end, false, true, (int value, float distance) => !BlocksManager.Blocks[Terrain.ExtractContents(value)].IsTransparent);
					if (terrainRaycastResult != null)
					{
						num = new float?((num != null) ? MathUtils.Min(num.Value, terrainRaycastResult.Value.Distance) : terrainRaycastResult.Value.Distance);
					}
				}
			}
			Vector3 vector5 = (num == null) ? (matrix.Translation + vector2) : (matrix.Translation + Vector3.Normalize(vector2) * MathUtils.Max(num.Value - 0.5f, 0.2f));
			base.SetupPerspectiveCamera(vector5, matrix.Translation - vector5, Vector3.UnitY);
		}

		// Token: 0x040010C4 RID: 4292
		public Vector3 m_position;
	}
}
