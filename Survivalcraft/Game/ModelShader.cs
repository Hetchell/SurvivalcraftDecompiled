using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020002B0 RID: 688
	public class ModelShader : Shader
	{
		// Token: 0x170002DF RID: 735
		// (set) Token: 0x060013B5 RID: 5045 RVA: 0x00098F95 File Offset: 0x00097195
		public Texture2D Texture
		{
			set
			{
				this.m_textureParameter.SetValue(value);
			}
		}

		// Token: 0x170002E0 RID: 736
		// (set) Token: 0x060013B6 RID: 5046 RVA: 0x00098FA3 File Offset: 0x000971A3
		public SamplerState SamplerState
		{
			set
			{
				this.m_samplerStateParameter.SetValue(value);
			}
		}

		// Token: 0x170002E1 RID: 737
		// (set) Token: 0x060013B7 RID: 5047 RVA: 0x00098FB1 File Offset: 0x000971B1
		public Vector4 MaterialColor
		{
			set
			{
				this.m_materialColorParameter.SetValue(value);
			}
		}

		// Token: 0x170002E2 RID: 738
		// (set) Token: 0x060013B8 RID: 5048 RVA: 0x00098FBF File Offset: 0x000971BF
		public Vector4 EmissionColor
		{
			set
			{
				this.m_emissionColorParameter.SetValue(value);
			}
		}

		// Token: 0x170002E3 RID: 739
		// (set) Token: 0x060013B9 RID: 5049 RVA: 0x00098FCD File Offset: 0x000971CD
		public float AlphaThreshold
		{
			set
			{
				this.m_alphaThresholdParameter.SetValue(value);
			}
		}

		// Token: 0x170002E4 RID: 740
		// (set) Token: 0x060013BA RID: 5050 RVA: 0x00098FDB File Offset: 0x000971DB
		public Vector3 AmbientLightColor
		{
			set
			{
				this.m_ambientLightColorParameter.SetValue(value);
			}
		}

		// Token: 0x170002E5 RID: 741
		// (set) Token: 0x060013BB RID: 5051 RVA: 0x00098FE9 File Offset: 0x000971E9
		public Vector3 DiffuseLightColor1
		{
			set
			{
				this.m_diffuseLightColor1Parameter.SetValue(value);
			}
		}

		// Token: 0x170002E6 RID: 742
		// (set) Token: 0x060013BC RID: 5052 RVA: 0x00098FF7 File Offset: 0x000971F7
		public Vector3 DiffuseLightColor2
		{
			set
			{
				this.m_diffuseLightColor2Parameter.SetValue(value);
			}
		}

		// Token: 0x170002E7 RID: 743
		// (set) Token: 0x060013BD RID: 5053 RVA: 0x00099005 File Offset: 0x00097205
		public Vector3 LightDirection1
		{
			set
			{
				this.m_directionToLight1Parameter.SetValue(-value);
			}
		}

		// Token: 0x170002E8 RID: 744
		// (set) Token: 0x060013BE RID: 5054 RVA: 0x00099018 File Offset: 0x00097218
		public Vector3 LightDirection2
		{
			set
			{
				this.m_directionToLight2Parameter.SetValue(-value);
			}
		}

		// Token: 0x170002E9 RID: 745
		// (set) Token: 0x060013BF RID: 5055 RVA: 0x0009902B File Offset: 0x0009722B
		public Vector3 FogColor
		{
			set
			{
				this.m_fogColorParameter.SetValue(value);
			}
		}

		// Token: 0x170002EA RID: 746
		// (set) Token: 0x060013C0 RID: 5056 RVA: 0x00099039 File Offset: 0x00097239
		public Vector2 FogStartInvLength
		{
			set
			{
				this.m_fogStartInvLengthParameter.SetValue(value);
			}
		}

		// Token: 0x170002EB RID: 747
		// (set) Token: 0x060013C1 RID: 5057 RVA: 0x00099047 File Offset: 0x00097247
		public float FogYMultiplier
		{
			set
			{
				this.m_fogYMultiplierParameter.SetValue(value);
			}
		}

		// Token: 0x170002EC RID: 748
		// (set) Token: 0x060013C2 RID: 5058 RVA: 0x00099055 File Offset: 0x00097255
		public Vector3 WorldUp
		{
			set
			{
				this.m_worldUpParameter.SetValue(value);
			}
		}

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x060013C3 RID: 5059 RVA: 0x00099063 File Offset: 0x00097263
		// (set) Token: 0x060013C4 RID: 5060 RVA: 0x0009906B File Offset: 0x0009726B
		public int InstancesCount
		{
			get
			{
				return this.m_instancesCount;
			}
			set
			{
				if (value < 0 || value > this.Transforms.MaxWorldMatrices)
				{
					throw new InvalidOperationException("Invalid instances count.");
				}
				this.m_instancesCount = value;
			}
		}

		// Token: 0x060013C5 RID: 5061 RVA: 0x00099094 File Offset: 0x00097294
		public ModelShader(bool useAlphaThreshold, int maxInstancesCount = 1) : base(ContentManager.Get<string>("Shaders/ModelVsh"), ContentManager.Get<string>("Shaders/ModelPsh"), ModelShader.PrepareShaderMacros(useAlphaThreshold, maxInstancesCount))
		{
			this.m_worldMatrixParameter = base.GetParameter("u_worldMatrix", false);
			this.m_worldViewProjectionMatrixParameter = base.GetParameter("u_worldViewProjectionMatrix", false);
			this.m_textureParameter = base.GetParameter("u_texture", false);
			this.m_samplerStateParameter = base.GetParameter("u_samplerState", false);
			this.m_materialColorParameter = base.GetParameter("u_materialColor", false);
			this.m_emissionColorParameter = base.GetParameter("u_emissionColor", false);
			this.m_alphaThresholdParameter = base.GetParameter("u_alphaThreshold", true);
			this.m_ambientLightColorParameter = base.GetParameter("u_ambientLightColor", false);
			this.m_diffuseLightColor1Parameter = base.GetParameter("u_diffuseLightColor1", false);
			this.m_directionToLight1Parameter = base.GetParameter("u_directionToLight1", false);
			this.m_diffuseLightColor2Parameter = base.GetParameter("u_diffuseLightColor2", false);
			this.m_directionToLight2Parameter = base.GetParameter("u_directionToLight2", false);
			this.m_fogColorParameter = base.GetParameter("u_fogColor", false);
			this.m_fogStartInvLengthParameter = base.GetParameter("u_fogStartInvLength", false);
			this.m_fogYMultiplierParameter = base.GetParameter("u_fogYMultiplier", false);
			this.m_worldUpParameter = base.GetParameter("u_worldUp", false);
			this.Transforms = new ShaderTransforms(maxInstancesCount);
		}

		// Token: 0x060013C6 RID: 5062 RVA: 0x000991F0 File Offset: 0x000973F0
		protected override void PrepareForDrawingOverride()
		{
			this.Transforms.UpdateMatrices(this.m_instancesCount, false, false, true);
			this.m_worldViewProjectionMatrixParameter.SetValue(this.Transforms.WorldViewProjection, this.InstancesCount);
			this.m_worldMatrixParameter.SetValue(this.Transforms.World, this.InstancesCount);
		}

		// Token: 0x060013C7 RID: 5063 RVA: 0x0009924C File Offset: 0x0009744C
		public static ShaderMacro[] PrepareShaderMacros(bool useAlphaThreshold, int maxInstancesCount)
		{
			List<ShaderMacro> list = new List<ShaderMacro>();
			if (useAlphaThreshold)
			{
				list.Add(new ShaderMacro("ALPHATESTED"));
			}
			list.Add(new ShaderMacro("MAX_INSTANCES_COUNT", maxInstancesCount.ToString(CultureInfo.InvariantCulture)));
			return list.ToArray();
		}

		// Token: 0x04000D8B RID: 3467
		public ShaderParameter m_worldMatrixParameter;

		// Token: 0x04000D8C RID: 3468
		public ShaderParameter m_worldViewProjectionMatrixParameter;

		// Token: 0x04000D8D RID: 3469
		public ShaderParameter m_textureParameter;

		// Token: 0x04000D8E RID: 3470
		public ShaderParameter m_samplerStateParameter;

		// Token: 0x04000D8F RID: 3471
		public ShaderParameter m_materialColorParameter;

		// Token: 0x04000D90 RID: 3472
		public ShaderParameter m_emissionColorParameter;

		// Token: 0x04000D91 RID: 3473
		public ShaderParameter m_alphaThresholdParameter;

		// Token: 0x04000D92 RID: 3474
		public ShaderParameter m_ambientLightColorParameter;

		// Token: 0x04000D93 RID: 3475
		public ShaderParameter m_diffuseLightColor1Parameter;

		// Token: 0x04000D94 RID: 3476
		public ShaderParameter m_directionToLight1Parameter;

		// Token: 0x04000D95 RID: 3477
		public ShaderParameter m_diffuseLightColor2Parameter;

		// Token: 0x04000D96 RID: 3478
		public ShaderParameter m_directionToLight2Parameter;

		// Token: 0x04000D97 RID: 3479
		public ShaderParameter m_fogColorParameter;

		// Token: 0x04000D98 RID: 3480
		public ShaderParameter m_fogStartInvLengthParameter;

		// Token: 0x04000D99 RID: 3481
		public ShaderParameter m_fogYMultiplierParameter;

		// Token: 0x04000D9A RID: 3482
		public ShaderParameter m_worldUpParameter;

		// Token: 0x04000D9B RID: 3483
		public int m_instancesCount;

		// Token: 0x04000D9C RID: 3484
		public readonly ShaderTransforms Transforms;
	}
}
