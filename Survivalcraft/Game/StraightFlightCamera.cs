using System;
using Engine;

namespace Game
{
	// Token: 0x02000302 RID: 770
	public class StraightFlightCamera : BasePerspectiveCamera
	{
		// Token: 0x17000365 RID: 869
		// (get) Token: 0x060015B5 RID: 5557 RVA: 0x000A589D File Offset: 0x000A3A9D
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x060015B6 RID: 5558 RVA: 0x000A58A0 File Offset: 0x000A3AA0
		public override bool IsEntityControlEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060015B7 RID: 5559 RVA: 0x000A58A3 File Offset: 0x000A3AA3
		public StraightFlightCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x060015B8 RID: 5560 RVA: 0x000A58AC File Offset: 0x000A3AAC
		public override void Activate(Camera previousCamera)
		{
			this.m_position = previousCamera.ViewPosition;
			base.SetupPerspectiveCamera(this.m_position, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		// Token: 0x060015B9 RID: 5561 RVA: 0x000A58D4 File Offset: 0x000A3AD4
		public override void Update(float dt)
		{
			Vector3 vector = 10f * (Vector3.UnitX + (float)MathUtils.Sin(0.20000000298023224 * Time.FrameStartTime) * Vector3.UnitZ);
			this.m_position.Y = 120f;
			this.m_position += vector * dt;
			base.SetupPerspectiveCamera(this.m_position, vector, Vector3.UnitY);
		}

		// Token: 0x04000F7B RID: 3963
		public Vector3 m_position;
	}
}
