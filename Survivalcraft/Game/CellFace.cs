using System;
using Engine;

namespace Game
{
	// Token: 0x02000238 RID: 568
	public struct CellFace : IEquatable<CellFace>
	{
		// Token: 0x17000288 RID: 648
		// (get) Token: 0x06001178 RID: 4472 RVA: 0x000876CA File Offset: 0x000858CA
		// (set) Token: 0x06001179 RID: 4473 RVA: 0x000876E3 File Offset: 0x000858E3
		public Point3 Point
		{
			get
			{
				return new Point3(this.X, this.Y, this.Z);
			}
			set
			{
				this.X = value.X;
				this.Y = value.Y;
				this.Z = value.Z;
			}
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x00087709 File Offset: 0x00085909
		public CellFace(int x, int y, int z, int face)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.Face = face;
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x00087728 File Offset: 0x00085928
		public static int OppositeFace(int face)
		{
			return CellFace.m_oppositeFaces[face];
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x00087731 File Offset: 0x00085931
		public static Point3 FaceToPoint3(int face)
		{
			return CellFace.m_faceToPoint3[face];
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x0008773E File Offset: 0x0008593E
		public static Vector3 FaceToVector3(int face)
		{
			return CellFace.m_faceToVector3[face];
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x0008774C File Offset: 0x0008594C
		public static int Point3ToFace(Point3 p, int maxFace = 5)
		{
			maxFace = MathUtils.Clamp(maxFace, 0, 5);
			for (int i = 0; i < maxFace; i++)
			{
				if (CellFace.m_faceToPoint3[i] == p)
				{
					return i;
				}
			}
			throw new InvalidOperationException("Invalid Point3.");
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x00087790 File Offset: 0x00085990
		public static int Vector3ToFace(Vector3 v, int maxFace = 5)
		{
			maxFace = MathUtils.Clamp(maxFace, 0, 5);
			float num = float.NegativeInfinity;
			int result = 0;
			for (int i = 0; i <= maxFace; i++)
			{
				float num2 = Vector3.Dot(CellFace.m_faceToVector3[i], v);
				if (num2 > num)
				{
					result = i;
					num = num2;
				}
			}
			return result;
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x000877D8 File Offset: 0x000859D8
		public static CellFace FromAxisAndDirection(int x, int y, int z, int axis, float direction)
		{
			CellFace result = default(CellFace);
			result.X = x;
			result.Y = y;
			result.Z = z;
			switch (axis)
			{
			case 0:
				result.Face = ((direction > 0f) ? 1 : 3);
				break;
			case 1:
				result.Face = ((direction > 0f) ? 4 : 5);
				break;
			case 2:
				result.Face = ((direction <= 0f) ? 2 : 0);
				break;
			}
			return result;
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x0008785C File Offset: 0x00085A5C
		public Plane CalculatePlane()
		{
			switch (this.Face)
			{
			case 0:
				return new Plane(new Vector3(0f, 0f, 1f), (float)(-(float)(this.Z + 1)));
			case 1:
				return new Plane(new Vector3(-1f, 0f, 0f), (float)(this.X + 1));
			case 2:
				return new Plane(new Vector3(0f, 0f, -1f), (float)this.Z);
			case 3:
				return new Plane(new Vector3(1f, 0f, 0f), (float)(-(float)this.X));
			case 4:
				return new Plane(new Vector3(0f, 1f, 0f), (float)(-(float)(this.Y + 1)));
			default:
				return new Plane(new Vector3(0f, -1f, 0f), (float)this.Y);
			}
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x0008795D File Offset: 0x00085B5D
		public override int GetHashCode()
		{
			return (this.X << 11) + (this.Y << 7) + (this.Z << 3) + this.Face;
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x00087981 File Offset: 0x00085B81
		public override bool Equals(object obj)
		{
			return obj is CellFace && this.Equals((CellFace)obj);
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x00087999 File Offset: 0x00085B99
		public bool Equals(CellFace other)
		{
			return other.X == this.X && other.Y == this.Y && other.Z == this.Z && other.Face == this.Face;
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x000879D8 File Offset: 0x00085BD8
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.X.ToString(),
				", ",
				this.Y.ToString(),
				", ",
				this.Z.ToString(),
				", face ",
				this.Face.ToString()
			});
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x00087A40 File Offset: 0x00085C40
		public static bool operator ==(CellFace c1, CellFace c2)
		{
			return c1.Equals(c2);
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x00087A4A File Offset: 0x00085C4A
		public static bool operator !=(CellFace c1, CellFace c2)
		{
			return !c1.Equals(c2);
		}

		// Token: 0x04000BA6 RID: 2982
		public int X;

		// Token: 0x04000BA7 RID: 2983
		public int Y;

		// Token: 0x04000BA8 RID: 2984
		public int Z;

		// Token: 0x04000BA9 RID: 2985
		public int Face;

		// Token: 0x04000BAA RID: 2986
		public static readonly int[] m_oppositeFaces = new int[]
		{
			2,
			3,
			0,
			1,
			5,
			4
		};

		// Token: 0x04000BAB RID: 2987
		public static readonly Point3[] m_faceToPoint3 = new Point3[]
		{
			new Point3(0, 0, 1),
			new Point3(1, 0, 0),
			new Point3(0, 0, -1),
			new Point3(-1, 0, 0),
			new Point3(0, 1, 0),
			new Point3(0, -1, 0)
		};

		// Token: 0x04000BAC RID: 2988
		public static readonly Vector3[] m_faceToVector3 = new Vector3[]
		{
			new Vector3(0f, 0f, 1f),
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 0f, -1f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, -1f, 0f)
		};
	}
}
