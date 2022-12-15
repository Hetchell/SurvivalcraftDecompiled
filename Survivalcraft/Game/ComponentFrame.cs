using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E0 RID: 480
	public class ComponentFrame : Component
	{
		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06000D19 RID: 3353 RVA: 0x000641B9 File Offset: 0x000623B9
		// (set) Token: 0x06000D1A RID: 3354 RVA: 0x000641C1 File Offset: 0x000623C1
		public Vector3 Position
		{
			get
			{
				return this.m_position;
			}
			set
			{
				if (value != this.m_position)
				{
					this.m_cachedMatrixValid = false;
					this.m_position = value;
					Action<ComponentFrame> positionChanged = this.PositionChanged;
					if (positionChanged == null)
					{
						return;
					}
					positionChanged(this);
				}
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06000D1B RID: 3355 RVA: 0x000641F0 File Offset: 0x000623F0
		// (set) Token: 0x06000D1C RID: 3356 RVA: 0x000641F8 File Offset: 0x000623F8
		public Quaternion Rotation
		{
			get
			{
				return this.m_rotation;
			}
			set
			{
				value = Quaternion.Normalize(value);
				if (value != this.m_rotation)
				{
					this.m_cachedMatrixValid = false;
					this.m_rotation = value;
					Action<ComponentFrame> rotationChanged = this.RotationChanged;
					if (rotationChanged == null)
					{
						return;
					}
					rotationChanged(this);
				}
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000D1D RID: 3357 RVA: 0x0006422F File Offset: 0x0006242F
		public Matrix Matrix
		{
			get
			{
				if (!this.m_cachedMatrixValid)
				{
					this.m_cachedMatrix = Matrix.CreateFromQuaternion(this.Rotation);
					this.m_cachedMatrix.Translation = this.Position;
				}
				return this.m_cachedMatrix;
			}
		}

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x06000D1E RID: 3358 RVA: 0x00064264 File Offset: 0x00062464
		// (remove) Token: 0x06000D1F RID: 3359 RVA: 0x0006429C File Offset: 0x0006249C
		public event Action<ComponentFrame> PositionChanged;

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x06000D20 RID: 3360 RVA: 0x000642D4 File Offset: 0x000624D4
		// (remove) Token: 0x06000D21 RID: 3361 RVA: 0x0006430C File Offset: 0x0006250C
		public event Action<ComponentFrame> RotationChanged;

        // Token: 0x06000D22 RID: 3362 RVA: 0x00064341 File Offset: 0x00062541
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.Position = valuesDictionary.GetValue<Vector3>("Position");
			this.Rotation = valuesDictionary.GetValue<Quaternion>("Rotation");
		}

        // Token: 0x06000D23 RID: 3363 RVA: 0x00064365 File Offset: 0x00062565
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<Vector3>("Position", this.Position);
			valuesDictionary.SetValue<Quaternion>("Rotation", this.Rotation);
		}

		// Token: 0x04000807 RID: 2055
		public Vector3 m_position;

		// Token: 0x04000808 RID: 2056
		public Quaternion m_rotation;

		// Token: 0x04000809 RID: 2057
		public bool m_cachedMatrixValid;

		// Token: 0x0400080A RID: 2058
		public Matrix m_cachedMatrix;
	}
}
