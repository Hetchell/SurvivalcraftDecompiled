using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E3 RID: 483
	public class ComponentGlowingEyes : Component, IDrawable
	{
		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06000D4A RID: 3402 RVA: 0x00064D7C File Offset: 0x00062F7C
		// (set) Token: 0x06000D4B RID: 3403 RVA: 0x00064D84 File Offset: 0x00062F84
		public Vector3 GlowingEyesOffset { get; set; }

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06000D4C RID: 3404 RVA: 0x00064D8D File Offset: 0x00062F8D
		// (set) Token: 0x06000D4D RID: 3405 RVA: 0x00064D95 File Offset: 0x00062F95
		public Color GlowingEyesColor { get; set; }

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000D4E RID: 3406 RVA: 0x00064D9E File Offset: 0x00062F9E
		public int[] DrawOrders
		{
			get
			{
				return ComponentGlowingEyes.m_drawOrders;
			}
		}

        // Token: 0x06000D4F RID: 3407 RVA: 0x00064DA8 File Offset: 0x00062FA8
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGlow = base.Project.FindSubsystem<SubsystemGlow>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentCreatureModel = base.Entity.FindComponent<ComponentCreatureModel>(true);
			this.GlowingEyesOffset = valuesDictionary.GetValue<Vector3>("GlowingEyesOffset");
			this.GlowingEyesColor = valuesDictionary.GetValue<Color>("GlowingEyesColor");
		}

        // Token: 0x06000D50 RID: 3408 RVA: 0x00064E10 File Offset: 0x00063010
        public override void OnEntityAdded()
		{
			for (int i = 0; i < this.m_eyeGlowPoints.Length; i++)
			{
				this.m_eyeGlowPoints[i] = this.m_subsystemGlow.AddGlowPoint();
			}
		}

        // Token: 0x06000D51 RID: 3409 RVA: 0x00064E44 File Offset: 0x00063044
        public override void OnEntityRemoved()
		{
			for (int i = 0; i < this.m_eyeGlowPoints.Length; i++)
			{
				this.m_subsystemGlow.RemoveGlowPoint(this.m_eyeGlowPoints[i]);
			}
		}

		// Token: 0x06000D52 RID: 3410 RVA: 0x00064E78 File Offset: 0x00063078
		public void Draw(Camera camera, int drawOrder)
		{
			if (this.m_eyeGlowPoints[0] == null || !this.m_componentCreatureModel.IsVisibleForCamera)
			{
				return;
			}
			this.m_eyeGlowPoints[0].Color = Color.Transparent;
			this.m_eyeGlowPoints[1].Color = Color.Transparent;
			ModelBone modelBone = this.m_componentCreatureModel.Model.FindBone("Head", false);
			if (modelBone == null)
			{
				return;
			}
			Matrix m = this.m_componentCreatureModel.AbsoluteBoneTransformsForCamera[modelBone.Index];
			m *= camera.InvertedViewMatrix;
			Vector3 vector = Vector3.Normalize(m.Up);
			float num = Vector3.Dot(m.Translation - camera.ViewPosition, camera.ViewDirection);
			if (num > 0f)
			{
				Vector3 translation = m.Translation;
				int cellLight = this.m_subsystemTerrain.Terrain.GetCellLight(Terrain.ToCell(translation.X), Terrain.ToCell(translation.Y), Terrain.ToCell(translation.Z));
				float num2 = LightingManager.LightIntensityByLightValue[cellLight];
				float num3 = (float)((0f - Vector3.Dot(vector, Vector3.Normalize(m.Translation - camera.ViewPosition)) > 0.7f) ? 1 : 0);
				num3 *= MathUtils.Saturate(1f * (num - 1f));
				num3 *= MathUtils.Saturate((1f - num2 - 0.5f) / 0.5f);
				if (num3 > 0.25f)
				{
					Vector3 vector2 = Vector3.Normalize(m.Right);
					Vector3 vector3 = -Vector3.Normalize(m.Forward);
					Color color = this.GlowingEyesColor * num3;
					this.m_eyeGlowPoints[0].Position = translation + vector2 * this.GlowingEyesOffset.X + vector3 * this.GlowingEyesOffset.Y + vector * this.GlowingEyesOffset.Z;
					this.m_eyeGlowPoints[0].Right = vector2;
					this.m_eyeGlowPoints[0].Up = vector3;
					this.m_eyeGlowPoints[0].Forward = vector;
					this.m_eyeGlowPoints[0].Size = 0.01f;
					this.m_eyeGlowPoints[0].FarSize = 0.06f;
					this.m_eyeGlowPoints[0].FarDistance = 14f;
					this.m_eyeGlowPoints[0].Color = color;
					this.m_eyeGlowPoints[1].Position = translation - vector2 * this.GlowingEyesOffset.X + vector3 * this.GlowingEyesOffset.Y + vector * this.GlowingEyesOffset.Z;
					this.m_eyeGlowPoints[1].Right = vector2;
					this.m_eyeGlowPoints[1].Up = vector3;
					this.m_eyeGlowPoints[1].Forward = vector;
					this.m_eyeGlowPoints[1].Size = 0.01f;
					this.m_eyeGlowPoints[1].FarSize = 0.06f;
					this.m_eyeGlowPoints[1].FarDistance = 14f;
					this.m_eyeGlowPoints[1].Color = color;
				}
			}
		}

		// Token: 0x0400081D RID: 2077
		public SubsystemGlow m_subsystemGlow;

		// Token: 0x0400081E RID: 2078
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400081F RID: 2079
		public ComponentCreatureModel m_componentCreatureModel;

		// Token: 0x04000820 RID: 2080
		public GlowPoint[] m_eyeGlowPoints = new GlowPoint[2];

		// Token: 0x04000821 RID: 2081
		public static int[] m_drawOrders = new int[1];
	}
}
