using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200039E RID: 926
	public class SubsystemGameWidgets : Subsystem, IUpdateable
	{
		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x06001B18 RID: 6936 RVA: 0x000D43FF File Offset: 0x000D25FF
		// (set) Token: 0x06001B19 RID: 6937 RVA: 0x000D4407 File Offset: 0x000D2607
		public GamesWidget GamesWidget { get; set; }

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x06001B1A RID: 6938 RVA: 0x000D4410 File Offset: 0x000D2610
		public ReadOnlyList<GameWidget> GameWidgets
		{
			get
			{
				return new ReadOnlyList<GameWidget>(this.m_gameWidgets);
			}
		}

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06001B1B RID: 6939 RVA: 0x000D441D File Offset: 0x000D261D
		// (set) Token: 0x06001B1C RID: 6940 RVA: 0x000D4425 File Offset: 0x000D2625
		public SubsystemTerrain SubsystemTerrain { get; set; }

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x06001B1D RID: 6941 RVA: 0x000D442E File Offset: 0x000D262E
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Views;
			}
		}

		// Token: 0x06001B1E RID: 6942 RVA: 0x000D4438 File Offset: 0x000D2638
		public float CalculateSquaredDistanceFromNearestView(Vector3 p)
		{
			float num = float.MaxValue;
			foreach (GameWidget gameWidget in this.m_gameWidgets)
			{
				float num2 = Vector3.DistanceSquared(p, gameWidget.ActiveCamera.ViewPosition);
				if (num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x06001B1F RID: 6943 RVA: 0x000D44A4 File Offset: 0x000D26A4
		public float CalculateDistanceFromNearestView(Vector3 p)
		{
			return MathUtils.Sqrt(this.CalculateSquaredDistanceFromNearestView(p));
		}

		// Token: 0x06001B20 RID: 6944 RVA: 0x000D44B4 File Offset: 0x000D26B4
		public void Update(float dt)
		{
			foreach (GameWidget gameWidget in this.GameWidgets)
			{
				gameWidget.ActiveCamera.Update(Time.FrameDuration);
			}
		}

        // Token: 0x06001B21 RID: 6945 RVA: 0x000D4514 File Offset: 0x000D2714
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			this.SubsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemPlayers.PlayerAdded += delegate(PlayerData playerData)
			{
				this.AddGameWidgetForPlayer(playerData);
			};
			this.m_subsystemPlayers.PlayerRemoved += delegate(PlayerData playerData)
			{
				if (playerData.GameWidget != null)
				{
					this.RemoveGameWidget(playerData.GameWidget);
				}
			};
			this.GamesWidget = valuesDictionary.GetValue<GamesWidget>("GamesWidget");
			foreach (PlayerData playerData2 in this.m_subsystemPlayers.PlayersData)
			{
				this.AddGameWidgetForPlayer(playerData2);
			}
		}

		// Token: 0x06001B22 RID: 6946 RVA: 0x000D45D4 File Offset: 0x000D27D4
		public override void Dispose()
		{
			foreach (GameWidget gameWidget in this.GameWidgets.ToArray<GameWidget>())
			{
				this.RemoveGameWidget(gameWidget);
				gameWidget.Dispose();
			}
		}

		// Token: 0x06001B23 RID: 6947 RVA: 0x000D4614 File Offset: 0x000D2814
		public void AddGameWidgetForPlayer(PlayerData playerData)
		{
			int index = 0;
			while (index < 4 && this.m_gameWidgets.FirstOrDefault((GameWidget v) => v.GameWidgetIndex == index) != null)
			{
				int index2 = index + 1;
				index = index2;
			}
			if (index >= 4)
			{
				throw new InvalidOperationException("Too many GameWidgets.");
			}
			GameWidget gameWidget = new GameWidget(playerData, playerData.PlayerIndex);
			this.m_gameWidgets.Add(gameWidget);
			this.GamesWidget.Children.Add(gameWidget);
		}

		// Token: 0x06001B24 RID: 6948 RVA: 0x000D46A0 File Offset: 0x000D28A0
		public void RemoveGameWidget(GameWidget gameWidget)
		{
			this.m_gameWidgets.Remove(gameWidget);
			this.GamesWidget.Children.Remove(gameWidget);
		}

		// Token: 0x040012D5 RID: 4821
		public const int MaxGameWidgets = 4;

		// Token: 0x040012D6 RID: 4822
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x040012D7 RID: 4823
		public List<GameWidget> m_gameWidgets = new List<GameWidget>();
	}
}
