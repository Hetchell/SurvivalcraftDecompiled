using System;
using Engine;

namespace Game
{
	// Token: 0x020002D0 RID: 720
	public struct PlayerInput
	{
		// Token: 0x04000E2E RID: 3630
		public Vector2 Look;

		// Token: 0x04000E2F RID: 3631
		public Vector3 Move;

		// Token: 0x04000E30 RID: 3632
		public Vector3 SneakMove;

		// Token: 0x04000E31 RID: 3633
		public Vector3? VrMove;

		// Token: 0x04000E32 RID: 3634
		public Vector2? VrLook;

		// Token: 0x04000E33 RID: 3635
		public Vector2 CameraLook;

		// Token: 0x04000E34 RID: 3636
		public Vector3 CameraMove;

		// Token: 0x04000E35 RID: 3637
		public Vector3 CameraSneakMove;

		// Token: 0x04000E36 RID: 3638
		public bool ToggleCreativeFly;

		// Token: 0x04000E37 RID: 3639
		public bool ToggleSneak;

		// Token: 0x04000E38 RID: 3640
		public bool ToggleMount;

		// Token: 0x04000E39 RID: 3641
		public bool EditItem;

		// Token: 0x04000E3A RID: 3642
		public bool Jump;

		// Token: 0x04000E3B RID: 3643
		public int ScrollInventory;

		// Token: 0x04000E3C RID: 3644
		public bool ToggleInventory;

		// Token: 0x04000E3D RID: 3645
		public bool ToggleClothing;

		// Token: 0x04000E3E RID: 3646
		public bool TakeScreenshot;

		// Token: 0x04000E3F RID: 3647
		public bool SwitchCameraMode;

		// Token: 0x04000E40 RID: 3648
		public bool TimeOfDay;

		// Token: 0x04000E41 RID: 3649
		public bool Lighting;

		// Token: 0x04000E42 RID: 3650
		public bool KeyboardHelp;

		// Token: 0x04000E43 RID: 3651
		public bool GamepadHelp;

		// Token: 0x04000E44 RID: 3652
		public Ray3? Dig;

		// Token: 0x04000E45 RID: 3653
		public Ray3? Hit;

		// Token: 0x04000E46 RID: 3654
		public Ray3? Aim;

		// Token: 0x04000E47 RID: 3655
		public Ray3? Interact;

		// Token: 0x04000E48 RID: 3656
		public Ray3? PickBlockType;

		// Token: 0x04000E49 RID: 3657
		public bool Drop;

		// Token: 0x04000E4A RID: 3658
		public int? SelectInventorySlot;
	}
}
