using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000395 RID: 917
	public class PlayerModelWidget : CanvasWidget
	{
		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x06001A9E RID: 6814 RVA: 0x000D19AD File Offset: 0x000CFBAD
		// (set) Token: 0x06001A9F RID: 6815 RVA: 0x000D19B5 File Offset: 0x000CFBB5
		public CharacterSkinsCache CharacterSkinsCache
		{
			get
			{
				return this.m_characterSkinsCache;
			}
			set
			{
				if (value != null)
				{
					this.m_publicCharacterSkinsCache.Clear();
					this.m_characterSkinsCache = value;
					return;
				}
				this.m_characterSkinsCache = this.m_publicCharacterSkinsCache;
			}
		}

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x06001AA0 RID: 6816 RVA: 0x000D19D9 File Offset: 0x000CFBD9
		// (set) Token: 0x06001AA1 RID: 6817 RVA: 0x000D19E1 File Offset: 0x000CFBE1
		public PlayerModelWidget.Shot CameraShot { get; set; }

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06001AA2 RID: 6818 RVA: 0x000D19EA File Offset: 0x000CFBEA
		// (set) Token: 0x06001AA3 RID: 6819 RVA: 0x000D19F2 File Offset: 0x000CFBF2
		public int AnimateHeadSeed { get; set; }

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06001AA4 RID: 6820 RVA: 0x000D19FB File Offset: 0x000CFBFB
		// (set) Token: 0x06001AA5 RID: 6821 RVA: 0x000D1A03 File Offset: 0x000CFC03
		public int AnimateHandsSeed { get; set; }

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06001AA6 RID: 6822 RVA: 0x000D1A0C File Offset: 0x000CFC0C
		// (set) Token: 0x06001AA7 RID: 6823 RVA: 0x000D1A14 File Offset: 0x000CFC14
		public bool OuterClothing { get; set; }

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06001AA8 RID: 6824 RVA: 0x000D1A1D File Offset: 0x000CFC1D
		// (set) Token: 0x06001AA9 RID: 6825 RVA: 0x000D1A25 File Offset: 0x000CFC25
		public PlayerClass PlayerClass { get; set; }

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06001AAA RID: 6826 RVA: 0x000D1A2E File Offset: 0x000CFC2E
		// (set) Token: 0x06001AAB RID: 6827 RVA: 0x000D1A36 File Offset: 0x000CFC36
		public string CharacterSkinName { get; set; }

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06001AAC RID: 6828 RVA: 0x000D1A3F File Offset: 0x000CFC3F
		// (set) Token: 0x06001AAD RID: 6829 RVA: 0x000D1A47 File Offset: 0x000CFC47
		public Texture2D CharacterSkinTexture { get; set; }

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06001AAE RID: 6830 RVA: 0x000D1A50 File Offset: 0x000CFC50
		// (set) Token: 0x06001AAF RID: 6831 RVA: 0x000D1A58 File Offset: 0x000CFC58
		public Texture2D OuterClothingTexture { get; set; }

		// Token: 0x06001AB0 RID: 6832 RVA: 0x000D1A64 File Offset: 0x000CFC64
		public PlayerModelWidget()
		{
			this.m_modelWidget = new ModelWidget
			{
				UseAlphaThreshold = true,
				IsPerspective = true
			};
			this.Children.Add(this.m_modelWidget);
			this.IsHitTestVisible = false;
			this.m_publicCharacterSkinsCache = new CharacterSkinsCache();
			this.m_characterSkinsCache = this.m_publicCharacterSkinsCache;
		}

		// Token: 0x06001AB1 RID: 6833 RVA: 0x000D1AC0 File Offset: 0x000CFCC0
		public override void Update()
		{
			if (base.Input.Press != null)
			{
				if (this.m_lastDrag != null)
				{
					this.m_rotation += 0.01f * (base.Input.Press.Value.X - this.m_lastDrag.Value.X);
					this.m_lastDrag = new Vector2?(base.Input.Press.Value);
					base.Input.Clear();
				}
				else if (base.HitTestGlobal(base.Input.Press.Value, null) == this)
				{
					this.m_lastDrag = new Vector2?(base.Input.Press.Value);
				}
			}
			else
			{
				this.m_lastDrag = null;
				this.m_rotation = MathUtils.NormalizeAngle(this.m_rotation);
				if (MathUtils.Abs(this.m_rotation) > 0.01f)
				{
					this.m_rotation *= MathUtils.PowSign(0.1f, Time.FrameDuration);
				}
				else
				{
					this.m_rotation = 0f;
				}
			}
			this.m_modelWidget.ModelMatrix = ((this.m_rotation != 0f) ? Matrix.CreateRotationY(this.m_rotation) : Matrix.Identity);
		}

		// Token: 0x06001AB2 RID: 6834 RVA: 0x000D1C1C File Offset: 0x000CFE1C
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			if (this.OuterClothing)
			{
				this.m_modelWidget.Model = CharacterSkinsManager.GetOuterClothingModel(this.PlayerClass);
			}
			else
			{
				this.m_modelWidget.Model = CharacterSkinsManager.GetPlayerModel(this.PlayerClass);
			}
			if (this.CameraShot == PlayerModelWidget.Shot.Body)
			{
				this.m_modelWidget.ViewPosition = ((this.PlayerClass == PlayerClass.Male) ? new Vector3(0f, 1.46f, -3.2f) : new Vector3(0f, 1.39f, -3.04f));
				this.m_modelWidget.ViewTarget = ((this.PlayerClass == PlayerClass.Male) ? new Vector3(0f, 0.9f, 0f) : new Vector3(0f, 0.86f, 0f));
				this.m_modelWidget.ViewFov = 0.57f;
			}
			else
			{
				if (this.CameraShot != PlayerModelWidget.Shot.Bust)
				{
					throw new InvalidOperationException("Unknown shot.");
				}
				this.m_modelWidget.ViewPosition = ((this.PlayerClass == PlayerClass.Male) ? new Vector3(0f, 1.5f, -1.05f) : new Vector3(0f, 1.43f, -1f));
				this.m_modelWidget.ViewTarget = ((this.PlayerClass == PlayerClass.Male) ? new Vector3(0f, 1.5f, 0f) : new Vector3(0f, 1.43f, 0f));
				this.m_modelWidget.ViewFov = 0.57f;
			}
			if (this.OuterClothing)
			{
				this.m_modelWidget.TextureOverride = this.OuterClothingTexture;
			}
			else
			{
				this.m_modelWidget.TextureOverride = ((this.CharacterSkinName != null) ? this.CharacterSkinsCache.GetTexture(this.CharacterSkinName) : this.CharacterSkinTexture);
			}
			if (this.AnimateHeadSeed != 0)
			{
				int num = (this.AnimateHeadSeed < 0) ? this.GetHashCode() : this.AnimateHeadSeed;
				float num2 = (float)MathUtils.Remainder(Time.FrameStartTime + 1000.0 * (double)num, 10000.0);
				Vector2 vector = default(Vector2);
				vector.X = MathUtils.Lerp(-0.75f, 0.75f, SimplexNoise.OctavedNoise(num2 + 100f, 0.2f, 1, 2f, 0.5f, false));
				vector.Y = MathUtils.Lerp(-0.5f, 0.5f, SimplexNoise.OctavedNoise(num2 + 200f, 0.17f, 1, 2f, 0.5f, false));
				Matrix value = Matrix.CreateRotationX(vector.Y) * Matrix.CreateRotationZ(vector.X);
				this.m_modelWidget.SetBoneTransform(this.m_modelWidget.Model.FindBone("Head", true).Index, new Matrix?(value));
			}
			if (!this.OuterClothing && this.AnimateHandsSeed != 0)
			{
				int num3 = (this.AnimateHandsSeed < 0) ? this.GetHashCode() : this.AnimateHandsSeed;
				float num4 = (float)MathUtils.Remainder(Time.FrameStartTime + 1000.0 * (double)num3, 10000.0);
				Vector2 vector2 = default(Vector2);
				vector2.X = MathUtils.Lerp(0.2f, 0f, SimplexNoise.OctavedNoise(num4 + 100f, 0.7f, 1, 2f, 0.5f, false));
				vector2.Y = MathUtils.Lerp(-0.3f, 0.3f, SimplexNoise.OctavedNoise(num4 + 200f, 0.7f, 1, 2f, 0.5f, false));
				Vector2 vector3 = default(Vector2);
				vector3.X = MathUtils.Lerp(-0.2f, 0f, SimplexNoise.OctavedNoise(num4 + 300f, 0.7f, 1, 2f, 0.5f, false));
				vector3.Y = MathUtils.Lerp(-0.3f, 0.3f, SimplexNoise.OctavedNoise(num4 + 400f, 0.7f, 1, 2f, 0.5f, false));
				Matrix value2 = Matrix.CreateRotationX(vector2.Y) * Matrix.CreateRotationY(vector2.X);
				Matrix value3 = Matrix.CreateRotationX(vector3.Y) * Matrix.CreateRotationY(vector3.X);
				this.m_modelWidget.SetBoneTransform(this.m_modelWidget.Model.FindBone("Hand1", true).Index, new Matrix?(value2));
				this.m_modelWidget.SetBoneTransform(this.m_modelWidget.Model.FindBone("Hand2", true).Index, new Matrix?(value3));
			}
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x06001AB3 RID: 6835 RVA: 0x000D20AB File Offset: 0x000D02AB
		public override void UpdateCeases()
		{
			if (base.RootWidget == null)
			{
				if (this.m_publicCharacterSkinsCache.ContainsTexture(this.m_modelWidget.TextureOverride))
				{
					this.m_modelWidget.TextureOverride = null;
				}
				this.m_publicCharacterSkinsCache.Clear();
			}
			base.UpdateCeases();
		}

		// Token: 0x04001290 RID: 4752
		public ModelWidget m_modelWidget;

		// Token: 0x04001291 RID: 4753
		public CharacterSkinsCache m_publicCharacterSkinsCache;

		// Token: 0x04001292 RID: 4754
		public CharacterSkinsCache m_characterSkinsCache;

		// Token: 0x04001293 RID: 4755
		public Vector2? m_lastDrag;

		// Token: 0x04001294 RID: 4756
		public float m_rotation;

		// Token: 0x02000525 RID: 1317
		public enum Shot
		{
			// Token: 0x040018F9 RID: 6393
			Body,
			// Token: 0x040018FA RID: 6394
			Bust
		}
	}
}
