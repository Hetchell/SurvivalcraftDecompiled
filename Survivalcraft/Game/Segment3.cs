using System;
using Engine;

namespace Game
{
	// Token: 0x020002EB RID: 747
	public struct Segment3
	{
		// Token: 0x060014EB RID: 5355 RVA: 0x000A22B4 File Offset: 0x000A04B4
		public Segment3(Vector3 start, Vector3 end)
		{
			this.Start = start;
			this.End = end;
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x000A22C4 File Offset: 0x000A04C4
		public override string ToString()
		{
			return string.Format("{0}, {1}, {2},  {3}, {4}, {5}", new object[]
			{
				this.Start.X,
				this.Start.Y,
				this.Start.Z,
				this.End.X,
				this.End.Y,
				this.End.Z
			});
		}

		// Token: 0x060014ED RID: 5357 RVA: 0x000A2354 File Offset: 0x000A0554
		public static float Distance(Segment3 s, Vector3 p)
		{
			Vector3 v = s.End - s.Start;
			Vector3 v2 = s.Start - p;
			Vector3 v3 = s.End - p;
			float num = Vector3.Dot(v2, v);
			if (num * Vector3.Dot(v3, v) <= 0f)
			{
				float num2 = v.LengthSquared();
				if (num2 == 0f)
				{
					return Vector3.Distance(p, s.Start);
				}
				return MathUtils.Sqrt(Vector3.Cross(p - s.Start, v).LengthSquared() / num2);
			}
			else
			{
				if (num <= 0f)
				{
					return v3.Length();
				}
				return v2.Length();
			}
		}

		// Token: 0x060014EE RID: 5358 RVA: 0x000A2400 File Offset: 0x000A0600
		public static Vector3 NearestPoint(Segment3 s, Vector3 p)
		{
			Vector3 v = s.End - s.Start;
			Vector3 v2 = s.Start - p;
			Vector3 v3 = s.End - p;
			float num = Vector3.Dot(v2, v);
			if (num * Vector3.Dot(v3, v) <= 0f)
			{
				float num2 = v.LengthSquared();
				if (num2 == 0f)
				{
					return s.Start;
				}
				float num3 = MathUtils.Sqrt(v2.LengthSquared() - Vector3.Cross(p - s.Start, v).LengthSquared() / num2);
				return Vector3.Lerp(s.Start, s.End, num3 / MathUtils.Sqrt(num2));
			}
			else
			{
				if (num <= 0f)
				{
					return s.End;
				}
				return s.Start;
			}
		}

		// Token: 0x04000EEF RID: 3823
		public Vector3 Start;

		// Token: 0x04000EF0 RID: 3824
		public Vector3 End;
	}
}
