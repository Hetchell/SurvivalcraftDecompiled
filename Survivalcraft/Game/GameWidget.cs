using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;
using GameEntitySystem;

namespace Game
{
	// Token: 0x02000387 RID: 903
	public class GameWidget : CanvasWidget
	{
		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x060019C8 RID: 6600 RVA: 0x000CC553 File Offset: 0x000CA753
		// (set) Token: 0x060019C9 RID: 6601 RVA: 0x000CC55B File Offset: 0x000CA75B
		public ViewWidget ViewWidget { get; set; }

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x060019CA RID: 6602 RVA: 0x000CC564 File Offset: 0x000CA764
		// (set) Token: 0x060019CB RID: 6603 RVA: 0x000CC56C File Offset: 0x000CA76C
		public ContainerWidget GuiWidget { get; set; }

		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x060019CC RID: 6604 RVA: 0x000CC575 File Offset: 0x000CA775
		// (set) Token: 0x060019CD RID: 6605 RVA: 0x000CC57D File Offset: 0x000CA77D
		public int GameWidgetIndex { get; set; }

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x060019CE RID: 6606 RVA: 0x000CC586 File Offset: 0x000CA786
		// (set) Token: 0x060019CF RID: 6607 RVA: 0x000CC58E File Offset: 0x000CA78E
		public SubsystemGameWidgets SubsystemGameWidgets { get; set; }

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x060019D0 RID: 6608 RVA: 0x000CC597 File Offset: 0x000CA797
		// (set) Token: 0x060019D1 RID: 6609 RVA: 0x000CC59F File Offset: 0x000CA79F
		public PlayerData PlayerData { get; set; }

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x060019D2 RID: 6610 RVA: 0x000CC5A8 File Offset: 0x000CA7A8
		public ReadOnlyList<Camera> Cameras
		{
			get
			{
				return new ReadOnlyList<Camera>(this.m_cameras);
			}
		}

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x060019D3 RID: 6611 RVA: 0x000CC5B5 File Offset: 0x000CA7B5
		// (set) Token: 0x060019D4 RID: 6612 RVA: 0x000CC5C0 File Offset: 0x000CA7C0
		public Camera ActiveCamera
		{
			get
			{
				return this.m_activeCamera;
			}
			set
			{
				if (value == null || value.GameWidget != this)
				{
					throw new InvalidOperationException("Invalid camera.");
				}
				if (!this.IsCameraAllowed(value))
				{
					value = this.FindCamera<FppCamera>(true);
				}
				if (value != this.m_activeCamera)
				{
					Camera activeCamera = this.m_activeCamera;
					this.m_activeCamera = value;
					this.m_activeCamera.Activate(activeCamera);
				}
			}
		}

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x060019D5 RID: 6613 RVA: 0x000CC619 File Offset: 0x000CA819
		// (set) Token: 0x060019D6 RID: 6614 RVA: 0x000CC621 File Offset: 0x000CA821
		public ComponentCreature Target { get; set; }

		// Token: 0x060019D7 RID: 6615 RVA: 0x000CC62C File Offset: 0x000CA82C
		public GameWidget(PlayerData playerData, int gameViewIndex)
		{
			this.PlayerData = playerData;
			this.GameWidgetIndex = gameViewIndex;
			this.SubsystemGameWidgets = playerData.SubsystemGameWidgets;
			base.LoadContents(this, ContentManager.Get<XElement>("Widgets/GameWidget"));
			this.ViewWidget = this.Children.Find<ViewWidget>("View", true);
			this.GuiWidget = this.Children.Find<ContainerWidget>("Gui", true);
			this.m_cameras.Add(new FppCamera(this));
			this.m_cameras.Add(new DeathCamera(this));
			this.m_cameras.Add(new IntroCamera(this));
			this.m_cameras.Add(new TppCamera(this));
			this.m_cameras.Add(new OrbitCamera(this));
			this.m_cameras.Add(new FixedCamera(this));
			this.m_cameras.Add(new LoadingCamera(this));
			this.m_activeCamera = this.FindCamera<LoadingCamera>(true);
		}

		// Token: 0x060019D8 RID: 6616 RVA: 0x000CC728 File Offset: 0x000CA928
		public T FindCamera<T>(bool throwOnError = true) where T : Camera
		{
			T t = (T)((object)this.m_cameras.FirstOrDefault((Camera c) => c is T));
			if (t != null || !throwOnError)
			{
				return t;
			}
			throw new InvalidOperationException("Camera with type \"" + typeof(T).Name + "\" not found.");
		}

		// Token: 0x060019D9 RID: 6617 RVA: 0x000CC795 File Offset: 0x000CA995
		public bool IsEntityTarget(Entity entity)
		{
			return this.Target != null && this.Target.Entity == entity;
		}

		// Token: 0x060019DA RID: 6618 RVA: 0x000CC7AF File Offset: 0x000CA9AF
		public bool IsEntityFirstPersonTarget(Entity entity)
		{
			return this.IsEntityTarget(entity) && this.ActiveCamera is FppCamera;
		}

		// Token: 0x060019DB RID: 6619 RVA: 0x000CC7CC File Offset: 0x000CA9CC
		public override void Update()
		{
			WidgetInputDevice widgetInputDevice = this.DetermineInputDevices();
			if (base.WidgetsHierarchyInput == null || base.WidgetsHierarchyInput.Devices != widgetInputDevice)
			{
				base.WidgetsHierarchyInput = new WidgetInput(widgetInputDevice);
			}
			if (this.GuiWidget.ParentWidget == null)
			{
				Widget.UpdateWidgetsHierarchy(this.GuiWidget);
			}
		}

		// Token: 0x060019DC RID: 6620 RVA: 0x000CC81C File Offset: 0x000CAA1C
		public WidgetInputDevice DetermineInputDevices()
		{
			if (this.PlayerData.SubsystemPlayers.PlayersData.Count > 0 && this.PlayerData == this.PlayerData.SubsystemPlayers.PlayersData[0])
			{
				WidgetInputDevice widgetInputDevice = WidgetInputDevice.None;
				foreach (PlayerData playerData in this.PlayerData.SubsystemPlayers.PlayersData)
				{
					if (playerData != this.PlayerData)
					{
						widgetInputDevice |= playerData.InputDevice;
					}
				}
				return (WidgetInputDevice.All & ~widgetInputDevice) | WidgetInputDevice.Touch | this.PlayerData.InputDevice;
			}
			WidgetInputDevice widgetInputDevice2 = WidgetInputDevice.None;
			foreach (PlayerData playerData2 in this.PlayerData.SubsystemPlayers.PlayersData)
			{
				if (playerData2 == this.PlayerData)
				{
					break;
				}
				widgetInputDevice2 |= playerData2.InputDevice;
			}
			return (this.PlayerData.InputDevice & ~widgetInputDevice2) | WidgetInputDevice.Touch;
		}

		// Token: 0x060019DD RID: 6621 RVA: 0x000CC954 File Offset: 0x000CAB54
		public bool IsCameraAllowed(Camera camera)
		{
			ComponentPlayer componentPlayer = this.PlayerData.ComponentPlayer;
			return componentPlayer == null || !componentPlayer.ComponentInput.IsControlledByVr || camera is FppCamera || camera is LoadingCamera || camera is DeathCamera;
		}

		// Token: 0x0400121A RID: 4634
		public List<Camera> m_cameras = new List<Camera>();

		// Token: 0x0400121B RID: 4635
		public Camera m_activeCamera;
	}
}
