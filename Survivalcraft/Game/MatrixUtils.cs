using System;
using Engine;

namespace Game
{
	// Token: 0x020002AA RID: 682
	internal class MatrixUtils
	{
		// Token: 0x060013A0 RID: 5024 RVA: 0x00098698 File Offset: 0x00096898
		public static Matrix CreateScaleTranslation(float sx, float sy, float tx, float ty)
		{
			return new Matrix(sx, 0f, 0f, 0f, 0f, sy, 0f, 0f, 0f, 0f, 1f, 0f, tx, ty, 0f, 1f);
		}
	}
}
