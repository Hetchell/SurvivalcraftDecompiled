using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200019C RID: 412
	public class SubsystemPlayers : Subsystem, IUpdateable
	{
		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060009BF RID: 2495 RVA: 0x00046333 File Offset: 0x00044533
		public ReadOnlyList<PlayerData> PlayersData
		{
			get
			{
				return new ReadOnlyList<PlayerData>(this.m_playersData);
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060009C0 RID: 2496 RVA: 0x00046340 File Offset: 0x00044540
		public ReadOnlyList<ComponentPlayer> ComponentPlayers
		{
			get
			{
				return new ReadOnlyList<ComponentPlayer>(this.m_componentPlayers);
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060009C1 RID: 2497 RVA: 0x0004634D File Offset: 0x0004454D
		// (set) Token: 0x060009C2 RID: 2498 RVA: 0x00046355 File Offset: 0x00044555
		public Vector3 GlobalSpawnPosition { get; set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060009C3 RID: 2499 RVA: 0x0004635E File Offset: 0x0004455E
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.SubsystemPlayers;
			}
		}

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x060009C4 RID: 2500 RVA: 0x00046364 File Offset: 0x00044564
		// (remove) Token: 0x060009C5 RID: 2501 RVA: 0x0004639C File Offset: 0x0004459C
		public event Action<PlayerData> PlayerAdded;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x060009C6 RID: 2502 RVA: 0x000463D4 File Offset: 0x000445D4
		// (remove) Token: 0x060009C7 RID: 2503 RVA: 0x0004640C File Offset: 0x0004460C
		public event Action<PlayerData> PlayerRemoved;

		// Token: 0x060009C8 RID: 2504 RVA: 0x00046444 File Offset: 0x00044644
		public bool IsPlayer(Entity entity)
		{
			foreach (ComponentPlayer componentPlayer in this.m_componentPlayers)
			{
				if (entity == componentPlayer.Entity)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060009C9 RID: 2505 RVA: 0x000464A0 File Offset: 0x000446A0
		public ComponentPlayer FindNearestPlayer(Vector3 position)
		{
			ComponentPlayer result = null;
			float num = float.MaxValue;
			foreach (ComponentPlayer componentPlayer in this.ComponentPlayers)
			{
				float num2 = Vector3.DistanceSquared(componentPlayer.ComponentBody.Position, position);
				if (num2 < num)
				{
					num = num2;
					result = componentPlayer;
				}
			}
			return result;
		}

		// Token: 0x060009CA RID: 2506 RVA: 0x00046518 File Offset: 0x00044718
		public void AddPlayerData(PlayerData playerData)
		{
			if (this.m_playersData.Count >= 4)
			{
				throw new InvalidOperationException("Too many players.");
			}
			if (this.m_playersData.Contains(playerData))
			{
				throw new InvalidOperationException("Player already added.");
			}
			this.m_playersData.Add(playerData);
			int nextPlayerIndex = this.m_nextPlayerIndex;
			this.m_nextPlayerIndex = nextPlayerIndex + 1;
			playerData.PlayerIndex = nextPlayerIndex;
			Action<PlayerData> playerAdded = this.PlayerAdded;
			if (playerAdded == null)
			{
				return;
			}
			playerAdded(playerData);
		}

		// Token: 0x060009CB RID: 2507 RVA: 0x0004658C File Offset: 0x0004478C
		public void RemovePlayerData(PlayerData playerData)
		{
			if (!this.m_playersData.Contains(playerData))
			{
				throw new InvalidOperationException("Player does not exist.");
			}
			this.m_playersData.Remove(playerData);
			if (playerData.ComponentPlayer != null)
			{
				base.Project.RemoveEntity(playerData.ComponentPlayer.Entity, true);
			}
			Action<PlayerData> playerRemoved = this.PlayerRemoved;
			if (playerRemoved != null)
			{
				playerRemoved(playerData);
			}
			playerData.Dispose();
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x000465F8 File Offset: 0x000447F8
		public void Update(float dt)
		{
			if (this.m_playersData.Count == 0)
			{
				ScreensManager.SwitchScreen("Player", new object[]
				{
					PlayerScreen.Mode.Initial,
					base.Project
				});
			}
			foreach (PlayerData playerData in this.m_playersData)
			{
				playerData.Update();
			}
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x00046678 File Offset: 0x00044878
		public override void Dispose()
		{
			foreach (PlayerData playerData in this.m_playersData)
			{
				playerData.Dispose();
			}
		}

        // Token: 0x060009CE RID: 2510 RVA: 0x000466C8 File Offset: 0x000448C8
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_nextPlayerIndex = valuesDictionary.GetValue<int>("NextPlayerIndex");
			this.GlobalSpawnPosition = valuesDictionary.GetValue<Vector3>("GlobalSpawnPosition");
			foreach (KeyValuePair<string, object> keyValuePair in valuesDictionary.GetValue<ValuesDictionary>("Players"))
			{
				PlayerData playerData = new PlayerData(base.Project);
				playerData.Load((ValuesDictionary)keyValuePair.Value);
				playerData.PlayerIndex = int.Parse(keyValuePair.Key, CultureInfo.InvariantCulture);
				this.m_playersData.Add(playerData);
			}
		}

        // Token: 0x060009CF RID: 2511 RVA: 0x00046788 File Offset: 0x00044988
        public override void Save(ValuesDictionary valuesDictionary)
		{
			valuesDictionary.SetValue<int>("NextPlayerIndex", this.m_nextPlayerIndex);
			valuesDictionary.SetValue<Vector3>("GlobalSpawnPosition", this.GlobalSpawnPosition);
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Players", valuesDictionary2);
			foreach (PlayerData playerData in this.m_playersData)
			{
				ValuesDictionary valuesDictionary3 = new ValuesDictionary();
				valuesDictionary2.SetValue<ValuesDictionary>(playerData.PlayerIndex.ToString(CultureInfo.InvariantCulture), valuesDictionary3);
				playerData.Save(valuesDictionary3);
			}
		}

        // Token: 0x060009D0 RID: 2512 RVA: 0x00046830 File Offset: 0x00044A30
        public override void OnEntityAdded(Entity entity)
		{
			foreach (PlayerData playerData in this.m_playersData)
			{
				playerData.OnEntityAdded(entity);
			}
			this.UpdateComponentPlayers();
		}

        // Token: 0x060009D1 RID: 2513 RVA: 0x00046888 File Offset: 0x00044A88
        public override void OnEntityRemoved(Entity entity)
		{
			foreach (PlayerData playerData in this.m_playersData)
			{
				playerData.OnEntityRemoved(entity);
			}
			this.UpdateComponentPlayers();
		}

		// Token: 0x060009D2 RID: 2514 RVA: 0x000468E0 File Offset: 0x00044AE0
		public void UpdateComponentPlayers()
		{
			this.m_componentPlayers.Clear();
			foreach (PlayerData playerData in this.m_playersData)
			{
				if (playerData.ComponentPlayer != null)
				{
					this.m_componentPlayers.Add(playerData.ComponentPlayer);
				}
			}
		}

		// Token: 0x0400052D RID: 1325
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400052E RID: 1326
		public List<PlayerData> m_playersData = new List<PlayerData>();

		// Token: 0x0400052F RID: 1327
		public List<ComponentPlayer> m_componentPlayers = new List<ComponentPlayer>();

		// Token: 0x04000530 RID: 1328
		public int m_nextPlayerIndex;

		// Token: 0x04000531 RID: 1329
		public const int MaxPlayers = 4;
	}
}
