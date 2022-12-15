using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000353 RID: 851
	public static class VrManager
	{
		// Token: 0x170003BC RID: 956
		// (get) Token: 0x060017EB RID: 6123 RVA: 0x000BDA8E File Offset: 0x000BBC8E
		public static bool IsVrAvailable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x060017EC RID: 6124 RVA: 0x000BDA91 File Offset: 0x000BBC91
		public static bool IsVrStarted
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x060017ED RID: 6125 RVA: 0x000BDA94 File Offset: 0x000BBC94
		public static RenderTarget2D VrRenderTarget
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x060017EE RID: 6126 RVA: 0x000BDA98 File Offset: 0x000BBC98
		public static Matrix HmdMatrix
		{
			get
			{
				return default(Matrix);
			}
		}

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x060017EF RID: 6127 RVA: 0x000BDAB0 File Offset: 0x000BBCB0
		public static Matrix HmdMatrixInverted
		{
			get
			{
				return default(Matrix);
			}
		}

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x060017F0 RID: 6128 RVA: 0x000BDAC8 File Offset: 0x000BBCC8
		public static Vector3 HmdMatrixYpr
		{
			get
			{
				return default(Vector3);
			}
		}

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x060017F1 RID: 6129 RVA: 0x000BDAE0 File Offset: 0x000BBCE0
		public static Matrix HmdLastMatrix
		{
			get
			{
				return default(Matrix);
			}
		}

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x060017F2 RID: 6130 RVA: 0x000BDAF8 File Offset: 0x000BBCF8
		public static Matrix HmdLastMatrixInverted
		{
			get
			{
				return default(Matrix);
			}
		}

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x060017F3 RID: 6131 RVA: 0x000BDB10 File Offset: 0x000BBD10
		public static Vector3 HmdLastMatrixYpr
		{
			get
			{
				return default(Vector3);
			}
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x060017F4 RID: 6132 RVA: 0x000BDB28 File Offset: 0x000BBD28
		public static Vector2 HeadMove
		{
			get
			{
				return default(Vector2);
			}
		}

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x060017F5 RID: 6133 RVA: 0x000BDB40 File Offset: 0x000BBD40
		public static Vector2 WalkingVelocity
		{
			get
			{
				return default(Vector2);
			}
		}

		// Token: 0x060017F6 RID: 6134 RVA: 0x000BDB56 File Offset: 0x000BBD56
		public static void Initialize()
		{
		}

		// Token: 0x060017F7 RID: 6135 RVA: 0x000BDB58 File Offset: 0x000BBD58
		public static void StartVr()
		{
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x000BDB5A File Offset: 0x000BBD5A
		public static void StopVr()
		{
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x000BDB5C File Offset: 0x000BBD5C
		public static void WaitGetPoses()
		{
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x000BDB5E File Offset: 0x000BBD5E
		public static void SubmitEyeTexture(VrEye eye, Texture2D texture)
		{
		}

		// Token: 0x060017FB RID: 6139 RVA: 0x000BDB60 File Offset: 0x000BBD60
		public static Matrix GetEyeToHeadTransform(VrEye eye)
		{
			return default(Matrix);
		}

		// Token: 0x060017FC RID: 6140 RVA: 0x000BDB78 File Offset: 0x000BBD78
		public static Matrix GetProjectionMatrix(VrEye eye, float near, float far)
		{
			return default(Matrix);
		}

		// Token: 0x060017FD RID: 6141 RVA: 0x000BDB8E File Offset: 0x000BBD8E
		public static bool IsControllerPresent(VrController controller)
		{
			return false;
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x000BDB94 File Offset: 0x000BBD94
		public static Matrix GetControllerMatrix(VrController controller)
		{
			return default(Matrix);
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x000BDBAC File Offset: 0x000BBDAC
		public static Vector2 GetStickPosition(VrController controller, float deadZone = 0f)
		{
			return default(Vector2);
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x000BDBC4 File Offset: 0x000BBDC4
		public static Vector2? GetTouchpadPosition(VrController controller, float deadZone = 0f)
		{
			return new Vector2?(default(Vector2));
		}

		// Token: 0x06001801 RID: 6145 RVA: 0x000BDBDF File Offset: 0x000BBDDF
		public static float GetTriggerPosition(VrController controller, float deadZone = 0f)
		{
			return 0f;
		}

		// Token: 0x06001802 RID: 6146 RVA: 0x000BDBE6 File Offset: 0x000BBDE6
		public static bool IsButtonDown(VrController controller, VrControllerButton button)
		{
			return false;
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x000BDBE9 File Offset: 0x000BBDE9
		public static bool IsButtonDownOnce(VrController controller, VrControllerButton button)
		{
			return false;
		}

		// Token: 0x06001804 RID: 6148 RVA: 0x000BDBEC File Offset: 0x000BBDEC
		public static TouchInput? GetTouchInput(VrController controller)
		{
			return null;
		}
	}
}
