using System;
using System.Collections.Generic;

namespace Game
{
	// Token: 0x02000301 RID: 769
	public class StateMachine
	{
		// Token: 0x17000363 RID: 867
		// (get) Token: 0x060015AE RID: 5550 RVA: 0x000A5728 File Offset: 0x000A3928
		public string PreviousState
		{
			get
			{
				if (this.m_previousState == null)
				{
					return null;
				}
				return this.m_previousState.Name;
			}
		}

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x060015AF RID: 5551 RVA: 0x000A573F File Offset: 0x000A393F
		public string CurrentState
		{
			get
			{
				if (this.m_currentState == null)
				{
					return null;
				}
				return this.m_currentState.Name;
			}
		}

		// Token: 0x060015B0 RID: 5552 RVA: 0x000A5758 File Offset: 0x000A3958
		public void AddState(string name, Action enter, Action update, Action leave)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new Exception("State name must not be empty or null.");
			}
			this.m_states.Add(name, new StateMachine.State
			{
				Name = name,
				Enter = enter,
				Update = update,
				Leave = leave
			});
		}

		// Token: 0x060015B1 RID: 5553 RVA: 0x000A57A8 File Offset: 0x000A39A8
		public void TransitionTo(string stateName)
		{
			StateMachine.State state = this.FindState(stateName);
			if (state != this.m_currentState)
			{
				if (this.m_currentState != null && this.m_currentState.Leave != null)
				{
					this.m_currentState.Leave();
				}
				this.m_previousState = this.m_currentState;
				this.m_currentState = state;
				if (this.m_currentState != null && this.m_currentState.Enter != null)
				{
					this.m_currentState.Enter();
				}
			}
		}

		// Token: 0x060015B2 RID: 5554 RVA: 0x000A5823 File Offset: 0x000A3A23
		public void Update()
		{
			if (this.m_currentState != null && this.m_currentState.Update != null)
			{
				this.m_currentState.Update();
			}
		}

		// Token: 0x060015B3 RID: 5555 RVA: 0x000A584C File Offset: 0x000A3A4C
		public StateMachine.State FindState(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			StateMachine.State result;
			if (!this.m_states.TryGetValue(name, out result))
			{
				throw new InvalidOperationException("State \"" + name + "\" not found.");
			}
			return result;
		}

		// Token: 0x04000F78 RID: 3960
		public Dictionary<string, StateMachine.State> m_states = new Dictionary<string, StateMachine.State>();

		// Token: 0x04000F79 RID: 3961
		public StateMachine.State m_currentState;

		// Token: 0x04000F7A RID: 3962
		public StateMachine.State m_previousState;

		// Token: 0x020004E5 RID: 1253
		public class State
		{
			// Token: 0x040017ED RID: 6125
			public string Name;

			// Token: 0x040017EE RID: 6126
			public Action Enter;

			// Token: 0x040017EF RID: 6127
			public Action Update;

			// Token: 0x040017F0 RID: 6128
			public Action Leave;
		}
	}
}
