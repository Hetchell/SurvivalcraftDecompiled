using System;
using Engine;

namespace Game
{
	// Token: 0x020002E1 RID: 737
	public class RandomJumpCamera : BasePerspectiveCamera
	{
		// Token: 0x1700031C RID: 796
		// (get) Token: 0x060014D1 RID: 5329 RVA: 0x000A1466 File Offset: 0x0009F666
		public override bool UsesMovementControls
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x060014D2 RID: 5330 RVA: 0x000A1469 File Offset: 0x0009F669
		public override bool IsEntityControlEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060014D3 RID: 5331 RVA: 0x000A146C File Offset: 0x0009F66C
		public RandomJumpCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x000A148B File Offset: 0x0009F68B
		public override void Activate(Camera previousCamera)
		{
			base.SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);
		}

		// Token: 0x060014D5 RID: 5333 RVA: 0x000A14A8 File Offset: 0x0009F6A8
		public override void Update(float dt)
		{
			if (this.m_random.Float(0f, 1f) < 0.1f * dt)
			{
				this.m_frequency = this.m_random.Float(0.33f, 5f) * 0.5f;
			}
			if (this.m_random.Float(0f, 1f) < this.m_frequency * dt)
			{
				SubsystemPlayers subsystemPlayers = base.GameWidget.SubsystemGameWidgets.Project.FindSubsystem<SubsystemPlayers>(true);
				if (subsystemPlayers.PlayersData.Count > 0)
				{
					Vector3 spawnPosition = subsystemPlayers.PlayersData[0].SpawnPosition;
					spawnPosition.X += this.m_random.Float(-150f, 150f);
					spawnPosition.Y = this.m_random.Float(70f, 120f);
					spawnPosition.Z += this.m_random.Float(-150f, 150f);
					Vector3 direction = this.m_random.Vector3(1f);
					base.SetupPerspectiveCamera(spawnPosition, direction, Vector3.UnitY);
				}
			}
			if (this.m_random.Float(0f, 1f) < 0.5f * this.m_frequency * dt)
			{
				base.GameWidget.SubsystemGameWidgets.Project.FindSubsystem<SubsystemTimeOfDay>(true).TimeOfDayOffset = (double)this.m_random.Float(0f, 1f);
			}
			if (this.m_random.Float(0f, 1f) < 1f * dt * 0.5f)
			{
				GameManager.SaveProject(false, false);
			}
		}

		// Token: 0x04000EC7 RID: 3783
		public const float frequencyFactor = 0.5f;

		// Token: 0x04000EC8 RID: 3784
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000EC9 RID: 3785
		public float m_frequency = 0.5f;
	}
}
