using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001B0 RID: 432
	public class SubsystemTime : Subsystem
	{
		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000AAD RID: 2733 RVA: 0x0004F654 File Offset: 0x0004D854
		public double GameTime
		{
			get
			{
				return this.m_gameTime;
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000AAE RID: 2734 RVA: 0x0004F65C File Offset: 0x0004D85C
		public float GameTimeDelta
		{
			get
			{
				return this.m_gameTimeDelta;
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000AAF RID: 2735 RVA: 0x0004F664 File Offset: 0x0004D864
		public float PreviousGameTimeDelta
		{
			get
			{
				return this.m_prevGameTimeDelta;
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000AB0 RID: 2736 RVA: 0x0004F66C File Offset: 0x0004D86C
		// (set) Token: 0x06000AB1 RID: 2737 RVA: 0x0004F674 File Offset: 0x0004D874
		public float GameTimeFactor
		{
			get
			{
				return this.m_gameTimeFactor;
			}
			set
			{
				this.m_gameTimeFactor = MathUtils.Clamp(value, 0f, 256f);
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000AB2 RID: 2738 RVA: 0x0004F68C File Offset: 0x0004D88C
		// (set) Token: 0x06000AB3 RID: 2739 RVA: 0x0004F694 File Offset: 0x0004D894
		public float? FixedTimeStep { get; set; }

		// Token: 0x06000AB4 RID: 2740 RVA: 0x0004F6A0 File Offset: 0x0004D8A0
		public void NextFrame()
		{
			this.m_prevGameTimeDelta = this.m_gameTimeDelta;
			if (this.FixedTimeStep == null)
			{
				this.m_gameTimeDelta = MathUtils.Min(Time.FrameDuration * this.m_gameTimeFactor, 0.1f);
			}
			else
			{
				this.m_gameTimeDelta = MathUtils.Min(this.FixedTimeStep.Value * this.m_gameTimeFactor, 0.1f);
			}
			this.m_gameTime += (double)this.m_gameTimeDelta;
			int i = 0;
			while (i < this.m_delayedExecutionsRequests.Count)
			{
				SubsystemTime.DelayedExecutionRequest delayedExecutionRequest = this.m_delayedExecutionsRequests[i];
				if (delayedExecutionRequest.GameTime >= 0.0 && this.GameTime >= delayedExecutionRequest.GameTime)
				{
					this.m_delayedExecutionsRequests.RemoveAt(i);
					delayedExecutionRequest.Action();
				}
				else
				{
					i++;
				}
			}
			int num = 0;
			int num2 = 0;
			foreach (ComponentPlayer componentPlayer in this.m_subsystemPlayers.ComponentPlayers)
			{
				if (componentPlayer.ComponentHealth.Health == 0f)
				{
					num2++;
				}
				else if (componentPlayer.ComponentSleep.SleepFactor == 1f)
				{
					num++;
				}
			}
			if (num + num2 == this.m_subsystemPlayers.ComponentPlayers.Count && num >= 1)
			{
				this.FixedTimeStep = new float?(0.05f);
				this.m_subsystemUpdate.UpdatesPerFrame = 20;
			}
			else
			{
				this.FixedTimeStep = null;
				this.m_subsystemUpdate.UpdatesPerFrame = 1;
			}
			bool flag = true;
			using (ReadOnlyList<ComponentPlayer>.Enumerator enumerator = this.m_subsystemPlayers.ComponentPlayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.ComponentGui.IsGameMenuDialogVisible())
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.GameTimeFactor = 0f;
				return;
			}
			if (this.GameTimeFactor == 0f)
			{
				this.GameTimeFactor = 1f;
			}
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x0004F8D4 File Offset: 0x0004DAD4
		public void QueueGameTimeDelayedExecution(double gameTime, Action action)
		{
			this.m_delayedExecutionsRequests.Add(new SubsystemTime.DelayedExecutionRequest
			{
				GameTime = gameTime,
				Action = action
			});
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x0004F908 File Offset: 0x0004DB08
		public bool PeriodicGameTimeEvent(double period, double offset)
		{
			double num = this.GameTime - offset;
			double num2 = MathUtils.Floor(num / period) * period;
			return num >= num2 && num - (double)this.GameTimeDelta < num2;
		}

        // Token: 0x06000AB7 RID: 2743 RVA: 0x0004F93B File Offset: 0x0004DB3B
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			this.m_subsystemUpdate = base.Project.FindSubsystem<SubsystemUpdate>(true);
		}

		// Token: 0x040005DC RID: 1500
		public const float MaxGameTimeDelta = 0.1f;

		// Token: 0x040005DD RID: 1501
		public double m_gameTime;

		// Token: 0x040005DE RID: 1502
		public float m_gameTimeDelta;

		// Token: 0x040005DF RID: 1503
		public float m_prevGameTimeDelta;

		// Token: 0x040005E0 RID: 1504
		public float m_gameTimeFactor = 1f;

		// Token: 0x040005E1 RID: 1505
		public List<SubsystemTime.DelayedExecutionRequest> m_delayedExecutionsRequests = new List<SubsystemTime.DelayedExecutionRequest>();

		// Token: 0x040005E2 RID: 1506
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x040005E3 RID: 1507
		public SubsystemUpdate m_subsystemUpdate;

		// Token: 0x02000441 RID: 1089
		public struct DelayedExecutionRequest
		{
			// Token: 0x0400160F RID: 5647
			public double GameTime;

			// Token: 0x04001610 RID: 5648
			public Action Action;
		}
	}
}
