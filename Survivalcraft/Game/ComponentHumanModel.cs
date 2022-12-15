using System;
using Engine;
using Engine.Graphics;
using Engine.Media;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E8 RID: 488
	public class ComponentHumanModel : ComponentCreatureModel
	{
		// Token: 0x06000DBB RID: 3515 RVA: 0x0006834C File Offset: 0x0006654C
		public override void Update(float dt)
		{
			if (this.m_componentCreature.ComponentBody.IsSneaking)
			{
				this.m_sneakFactor = MathUtils.Min(this.m_sneakFactor + 2f * dt, 1f);
			}
			else
			{
				this.m_sneakFactor = MathUtils.Max(this.m_sneakFactor - 2f * dt, 0f);
			}
			if ((this.m_componentSleep != null && this.m_componentSleep.IsSleeping) || this.m_componentCreature.ComponentHealth.Health <= 0f)
			{
				this.m_lieDownFactorEye = MathUtils.Min(this.m_lieDownFactorEye + 1f * dt, 1f);
				this.m_lieDownFactorModel = MathUtils.Min(this.m_lieDownFactorModel + 3f * dt, 1f);
			}
			else
			{
				this.m_lieDownFactorEye = MathUtils.Max(this.m_lieDownFactorEye - 1f * dt, 0f);
				this.m_lieDownFactorModel = MathUtils.Max(this.m_lieDownFactorModel - 3f * dt, 0f);
			}
			bool flag = true;
			bool flag2 = true;
			float footstepsPhase = this.m_footstepsPhase;
			if (this.m_componentCreature.ComponentLocomotion.LadderValue != null)
			{
				this.m_footstepsPhase += 1.5f * this.m_walkAnimationSpeed * this.m_componentCreature.ComponentBody.Velocity.Length() * dt;
				flag2 = false;
			}
			else if (!this.m_componentCreature.ComponentLocomotion.IsCreativeFlyEnabled)
			{
				float num = this.m_componentCreature.ComponentLocomotion.SlipSpeed ?? (this.m_componentCreature.ComponentBody.Velocity.XZ - this.m_componentCreature.ComponentBody.StandingOnVelocity.XZ).Length();
				if (num > 0.5f)
				{
					base.MovementAnimationPhase += num * dt * this.m_walkAnimationSpeed;
					this.m_footstepsPhase += 1f * this.m_walkAnimationSpeed * num * dt;
					flag = false;
					flag2 = false;
				}
			}
			if (flag)
			{
				float num2 = 0.5f * MathUtils.Floor(2f * base.MovementAnimationPhase);
				if (base.MovementAnimationPhase != num2)
				{
					if (base.MovementAnimationPhase - num2 > 0.25f)
					{
						base.MovementAnimationPhase = MathUtils.Min(base.MovementAnimationPhase + 2f * dt, num2 + 0.5f);
					}
					else
					{
						base.MovementAnimationPhase = MathUtils.Max(base.MovementAnimationPhase - 2f * dt, num2);
					}
				}
			}
			if (flag2)
			{
				this.m_footstepsPhase = 0f;
			}
			float num3 = 0f;
			ComponentMount componentMount = (this.m_componentRider != null) ? this.m_componentRider.Mount : null;
			if (componentMount != null)
			{
				ComponentCreatureModel componentCreatureModel = componentMount.Entity.FindComponent<ComponentCreatureModel>();
				if (componentCreatureModel != null)
				{
					base.Bob = componentCreatureModel.Bob;
					num3 = base.Bob;
				}
				this.m_headingOffset = 0f;
			}
			else
			{
				float x = MathUtils.Sin(6.2831855f * base.MovementAnimationPhase);
				num3 = this.m_walkBobHeight * MathUtils.Sqr(x);
				float num4 = 0f;
				if (this.m_componentCreature.ComponentLocomotion.LastWalkOrder != null)
				{
					Vector2? lastWalkOrder = this.m_componentCreature.ComponentLocomotion.LastWalkOrder;
					Vector2 zero = Vector2.Zero;
					if (lastWalkOrder == null || (lastWalkOrder != null && lastWalkOrder.GetValueOrDefault() != zero))
					{
						num4 = Vector2.Angle(Vector2.UnitY, this.m_componentCreature.ComponentLocomotion.LastWalkOrder.Value);
					}
				}
				this.m_headingOffset += MathUtils.NormalizeAngle(num4 - this.m_headingOffset) * MathUtils.Saturate(8f * this.m_subsystemTime.GameTimeDelta);
				this.m_headingOffset = MathUtils.NormalizeAngle(this.m_headingOffset);
			}
			float num5 = MathUtils.Min(12f * this.m_subsystemTime.GameTimeDelta, 1f);
			base.Bob += num5 * (num3 - base.Bob);
			base.IsAttackHitMoment = false;
			if (base.AttackOrder)
			{
				this.m_punchFactor = MathUtils.Min(this.m_punchFactor + 4f * dt, 1f);
				float punchPhase = this.m_punchPhase;
				this.m_punchPhase = MathUtils.Remainder(this.m_punchPhase + dt * 2f, 1f);
				if (punchPhase < 0.5f && this.m_punchPhase >= 0.5f)
				{
					base.IsAttackHitMoment = true;
					this.m_punchCounter++;
				}
			}
			else
			{
				this.m_punchFactor = MathUtils.Max(this.m_punchFactor - 4f * dt, 0f);
				if (this.m_punchPhase != 0f)
				{
					if (this.m_punchPhase > 0.5f)
					{
						this.m_punchPhase = MathUtils.Remainder(MathUtils.Min(this.m_punchPhase + dt * 2f, 1f), 1f);
					}
					else if (this.m_punchPhase > 0f)
					{
						this.m_punchPhase = MathUtils.Max(this.m_punchPhase - dt * this.m_punchPhase, 0f);
					}
				}
			}
			this.m_rowLeft = base.RowLeftOrder;
			this.m_rowRight = base.RowRightOrder;
			if ((this.m_rowLeft || this.m_rowRight) && componentMount != null && componentMount.ComponentBody.ImmersionFactor > 0f && MathUtils.Floor(1.100000023841858 * this.m_subsystemTime.GameTime) != MathUtils.Floor(1.100000023841858 * (this.m_subsystemTime.GameTime - (double)this.m_subsystemTime.GameTimeDelta)))
			{
				this.m_subsystemAudio.PlayRandomSound("Audio/Rowing", this.m_random.Float(0.4f, 0.6f), this.m_random.Float(-0.3f, 0.2f), this.m_componentCreature.ComponentBody.Position, 3f, true);
			}
			float num6 = MathUtils.Floor(this.m_footstepsPhase);
			if (this.m_footstepsPhase > num6 && footstepsPhase <= num6)
			{
				if (!this.m_componentCreature.ComponentBody.IsSneaking)
				{
					this.m_subsystemNoise.MakeNoise(this.m_componentCreature.ComponentBody, 0.25f, 8f);
				}
				if (!this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(1f))
				{
					this.m_footstepsPhase = 0f;
				}
			}
			this.m_aimHandAngle = base.AimHandAngleOrder;
			this.m_inHandItemOffset = Vector3.Lerp(this.m_inHandItemOffset, base.InHandItemOffsetOrder, 10f * dt);
			this.m_inHandItemRotation = Vector3.Lerp(this.m_inHandItemRotation, base.InHandItemRotationOrder, 10f * dt);
			base.AttackOrder = false;
			base.RowLeftOrder = false;
			base.RowRightOrder = false;
			base.AimHandAngleOrder = 0f;
			base.InHandItemOffsetOrder = Vector3.Zero;
			base.InHandItemRotationOrder = Vector3.Zero;
			base.Update(dt);
		}

		// Token: 0x06000DBC RID: 3516 RVA: 0x00068A50 File Offset: 0x00066C50
		public override void Animate()
		{
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			Vector3 vector = this.m_componentCreature.ComponentBody.Rotation.ToYawPitchRoll();
			if (this.m_lieDownFactorModel == 0f)
			{
				ComponentMount componentMount = (this.m_componentRider != null) ? this.m_componentRider.Mount : null;
				float num = MathUtils.Sin(6.2831855f * base.MovementAnimationPhase);
				position.Y += base.Bob;
				vector.X += this.m_headingOffset;
				float num2 = (float)MathUtils.Remainder(0.75 * this.m_subsystemGameInfo.TotalElapsedGameTime + (double)(this.GetHashCode() & 65535), 10000.0);
				float x = MathUtils.Clamp(MathUtils.Lerp(-0.3f, 0.3f, SimplexNoise.Noise(1.02f * num2 - 100f)) + this.m_componentCreature.ComponentLocomotion.LookAngles.X + 1f * this.m_componentCreature.ComponentLocomotion.LastTurnOrder.X + this.m_headingOffset, 0f - MathUtils.DegToRad(80f), MathUtils.DegToRad(80f));
				float y = MathUtils.Clamp(MathUtils.Lerp(-0.3f, 0.3f, SimplexNoise.Noise(0.96f * num2 - 200f)) + this.m_componentCreature.ComponentLocomotion.LookAngles.Y, 0f - MathUtils.DegToRad(45f), MathUtils.DegToRad(45f));
				float num3 = 0f;
				float y2 = 0f;
				float x2 = 0f;
				float y3 = 0f;
				float num4 = 0f;
				float num5 = 0f;
				float num6 = 0f;
				float num7 = 0f;
				if (componentMount != null)
				{
					if (componentMount.Entity.ValuesDictionary.DatabaseObject.Name == "Boat")
					{
						position.Y -= 0.2f;
						vector.X += 3.1415927f;
						num4 = 0.4f;
						num6 = 0.4f;
						num5 = 0.2f;
						num7 = -0.2f;
						num3 = 1.1f;
						x2 = 1.1f;
						y2 = 0.2f;
						y3 = -0.2f;
					}
					else
					{
						num4 = 0.5f;
						num6 = 0.5f;
						num5 = 0.15f;
						num7 = -0.15f;
						y2 = 0.55f;
						y3 = -0.55f;
					}
				}
				else if (this.m_componentCreature.ComponentLocomotion.IsCreativeFlyEnabled)
				{
					float num8 = (this.m_componentCreature.ComponentLocomotion.LastWalkOrder != null) ? MathUtils.Min(0.03f * this.m_componentCreature.ComponentBody.Velocity.XZ.LengthSquared(), 0.5f) : 0f;
					num3 = -0.1f - num8;
					x2 = num3;
					y2 = MathUtils.Lerp(0f, 0.25f, SimplexNoise.Noise(1.07f * num2 + 400f));
					y3 = 0f - MathUtils.Lerp(0f, 0.25f, SimplexNoise.Noise(0.93f * num2 + 500f));
				}
				else if (base.MovementAnimationPhase != 0f)
				{
					num4 = -0.5f * num;
					num6 = 0.5f * num;
					num3 = this.m_walkLegsAngle * num;
					x2 = 0f - num3;
				}
				float num9 = 0f;
				if (this.m_componentMiner != null)
				{
					float num10 = MathUtils.Sin(MathUtils.Sqrt(this.m_componentMiner.PokingPhase) * 3.1415927f);
					num9 = ((this.m_componentMiner.ActiveBlockValue == 0) ? (1f * num10) : (0.3f + 1f * num10));
				}
				float num11 = (this.m_punchPhase != 0f) ? ((0f - MathUtils.DegToRad(90f)) * MathUtils.Sin(6.2831855f * MathUtils.Sigmoid(this.m_punchPhase, 4f))) : 0f;
				float num12 = ((this.m_punchCounter & 1) == 0) ? num11 : 0f;
				float num13 = ((this.m_punchCounter & 1) != 0) ? num11 : 0f;
				float num14 = 0f;
				float num15 = 0f;
				float num16 = 0f;
				float num17 = 0f;
				if (this.m_rowLeft || this.m_rowRight)
				{
					float num18 = 0.6f * (float)MathUtils.Sin(6.91150426864624 * this.m_subsystemTime.GameTime);
					float num19 = 0.2f + 0.2f * (float)MathUtils.Cos(6.91150426864624 * (this.m_subsystemTime.GameTime + 0.5));
					if (this.m_rowLeft)
					{
						num14 = num18;
						num15 = num19;
					}
					if (this.m_rowRight)
					{
						num16 = num18;
						num17 = 0f - num19;
					}
				}
				float num20 = 0f;
				float num21 = 0f;
				float num22 = 0f;
				float num23 = 0f;
				if (this.m_aimHandAngle != 0f)
				{
					num20 = 1.5f;
					num21 = -0.7f;
					num22 = this.m_aimHandAngle * 1f;
					num23 = 0f;
				}
				float num24 = (float)((!this.m_componentCreature.ComponentLocomotion.IsCreativeFlyEnabled) ? 1 : 4);
				num4 += MathUtils.Lerp(-0.1f, 0.1f, SimplexNoise.Noise(num2)) + num12 + num14 + num20;
				num5 += MathUtils.Lerp(0f, num24 * 0.15f, SimplexNoise.Noise(1.1f * num2 + 100f)) + num15 + num21;
				num6 += num9 + MathUtils.Lerp(-0.1f, 0.1f, SimplexNoise.Noise(0.9f * num2 + 200f)) + num13 + num16 + num22;
				num7 += 0f - MathUtils.Lerp(0f, num24 * 0.15f, SimplexNoise.Noise(1.05f * num2 + 300f)) + num17 + num23;
				float s = MathUtils.Min(12f * this.m_subsystemTime.GameTimeDelta, 1f);
				this.m_headAngles += s * (new Vector2(x, y) - this.m_headAngles);
				this.m_handAngles1 += s * (new Vector2(num4, num5) - this.m_handAngles1);
				this.m_handAngles2 += s * (new Vector2(num6, num7) - this.m_handAngles2);
				this.m_legAngles1 += s * (new Vector2(num3, y2) - this.m_legAngles1);
				this.m_legAngles2 += s * (new Vector2(x2, y3) - this.m_legAngles2);
				base.SetBoneTransform(this.m_bodyBone.Index, new Matrix?(Matrix.CreateRotationY(vector.X) * Matrix.CreateTranslation(position)));
				base.SetBoneTransform(this.m_headBone.Index, new Matrix?(Matrix.CreateRotationX(this.m_headAngles.Y) * Matrix.CreateRotationZ(0f - this.m_headAngles.X)));
				base.SetBoneTransform(this.m_hand1Bone.Index, new Matrix?(Matrix.CreateRotationY(this.m_handAngles1.Y) * Matrix.CreateRotationX(this.m_handAngles1.X)));
				base.SetBoneTransform(this.m_hand2Bone.Index, new Matrix?(Matrix.CreateRotationY(this.m_handAngles2.Y) * Matrix.CreateRotationX(this.m_handAngles2.X)));
				base.SetBoneTransform(this.m_leg1Bone.Index, new Matrix?(Matrix.CreateRotationY(this.m_legAngles1.Y) * Matrix.CreateRotationX(this.m_legAngles1.X)));
				base.SetBoneTransform(this.m_leg2Bone.Index, new Matrix?(Matrix.CreateRotationY(this.m_legAngles2.Y) * Matrix.CreateRotationX(this.m_legAngles2.X)));
			}
			else
			{
				float num25 = MathUtils.Max(base.DeathPhase, this.m_lieDownFactorModel);
				float num26 = 1f - num25;
				Vector3 position2 = position + num25 * 0.5f * this.m_componentCreature.ComponentBody.BoxSize.Y * Vector3.Normalize(this.m_componentCreature.ComponentBody.Matrix.Forward * new Vector3(1f, 0f, 1f)) + num25 * Vector3.UnitY * this.m_componentCreature.ComponentBody.BoxSize.Z * 0.1f;
				base.SetBoneTransform(this.m_bodyBone.Index, new Matrix?(Matrix.CreateFromYawPitchRoll(vector.X, 1.5707964f * num25, 0f) * Matrix.CreateTranslation(position2)));
				base.SetBoneTransform(this.m_headBone.Index, new Matrix?(Matrix.Identity));
				base.SetBoneTransform(this.m_hand1Bone.Index, new Matrix?(Matrix.CreateRotationY(this.m_handAngles1.Y * num26) * Matrix.CreateRotationX(this.m_handAngles1.X * num26)));
				base.SetBoneTransform(this.m_hand2Bone.Index, new Matrix?(Matrix.CreateRotationY(this.m_handAngles2.Y * num26) * Matrix.CreateRotationX(this.m_handAngles2.X * num26)));
				base.SetBoneTransform(this.m_leg1Bone.Index, new Matrix?(Matrix.CreateRotationY(this.m_legAngles1.Y * num26) * Matrix.CreateRotationX(this.m_legAngles1.X * num26)));
				base.SetBoneTransform(this.m_leg2Bone.Index, new Matrix?(Matrix.CreateRotationY(this.m_legAngles2.Y * num26) * Matrix.CreateRotationX(this.m_legAngles2.X * num26)));
			}
			base.Animate();
		}

		// Token: 0x06000DBD RID: 3517 RVA: 0x000694CC File Offset: 0x000676CC
		public override void DrawExtras(Camera camera)
		{
			if (this.m_componentCreature.ComponentHealth.Health > 0f && this.m_componentMiner != null && this.m_componentMiner.ActiveBlockValue != 0)
			{
				int num = Terrain.ExtractContents(this.m_componentMiner.ActiveBlockValue);
				Block block = BlocksManager.Blocks[num];
				Matrix matrix = base.AbsoluteBoneTransformsForCamera[this.m_hand2Bone.Index];
				matrix *= camera.InvertedViewMatrix;
				matrix.Right = Vector3.Normalize(matrix.Right);
				matrix.Up = Vector3.Normalize(matrix.Up);
				matrix.Forward = Vector3.Normalize(matrix.Forward);
				Matrix matrix2 = Matrix.CreateRotationY(MathUtils.DegToRad(block.InHandRotation.Y) + this.m_inHandItemRotation.Y) * Matrix.CreateRotationZ(MathUtils.DegToRad(block.InHandRotation.Z) + this.m_inHandItemRotation.Z) * Matrix.CreateRotationX(MathUtils.DegToRad(block.InHandRotation.X) + this.m_inHandItemRotation.X) * Matrix.CreateTranslation(block.InHandOffset + this.m_inHandItemOffset) * Matrix.CreateTranslation(new Vector3(0.05f, 0.05f, -0.56f) * (this.m_componentCreature.ComponentBody.BoxSize.Y / 1.77f)) * matrix;
				int x = Terrain.ToCell(matrix2.Translation.X);
				int y = Terrain.ToCell(matrix2.Translation.Y);
				int z = Terrain.ToCell(matrix2.Translation.Z);
				this.m_drawBlockEnvironmentData.InWorldMatrix = matrix2;
				this.m_drawBlockEnvironmentData.Humidity = this.m_subsystemTerrain.Terrain.GetSeasonalHumidity(x, z);
				this.m_drawBlockEnvironmentData.Temperature = this.m_subsystemTerrain.Terrain.GetSeasonalTemperature(x, z) + SubsystemWeather.GetTemperatureAdjustmentAtHeight(y);
				this.m_drawBlockEnvironmentData.Light = this.m_subsystemTerrain.Terrain.GetCellLight(x, y, z);
				this.m_drawBlockEnvironmentData.BillboardDirection = new Vector3?(-Vector3.UnitZ);
				this.m_drawBlockEnvironmentData.SubsystemTerrain = this.m_subsystemTerrain;
				Matrix matrix3 = matrix2 * camera.ViewMatrix;
				block.DrawBlock(this.m_subsystemModelsRenderer.PrimitivesRenderer, this.m_componentMiner.ActiveBlockValue, Color.White, block.InHandScale, ref matrix3, this.m_drawBlockEnvironmentData);
			}
			if (this.m_componentPlayer != null && camera.GameWidget.PlayerData != this.m_componentPlayer.PlayerData)
			{
				Vector3 vector = Vector3.Transform(this.m_componentCreature.ComponentBody.Position + 1.02f * Vector3.UnitY * this.m_componentCreature.ComponentBody.BoxSize.Y, camera.ViewMatrix);
				if (vector.Z < 0f)
				{
					Color color = Color.Lerp(Color.White, Color.Transparent, MathUtils.Saturate((vector.Length() - 4f) / 3f));
					if (color.A > 8)
					{
						Vector3 vector2 = Vector3.TransformNormal(0.005f * Vector3.Normalize(Vector3.Cross(camera.ViewDirection, Vector3.UnitY)), camera.ViewMatrix);
						Vector3 vector3 = Vector3.TransformNormal(-0.005f * Vector3.UnitY, camera.ViewMatrix);
						BitmapFont font = ContentManager.Get<BitmapFont>("Fonts/Pericles");
						this.m_subsystemModelsRenderer.PrimitivesRenderer.FontBatch(font, 1, DepthStencilState.DepthRead, RasterizerState.CullNoneScissor, BlendState.AlphaBlend, SamplerState.LinearClamp).QueueText(this.m_componentPlayer.PlayerData.Name, vector, vector2, vector3, color, TextAnchor.HorizontalCenter | TextAnchor.Bottom);
					}
				}
			}
			base.DrawExtras(camera);
		}

        // Token: 0x06000DBE RID: 3518 RVA: 0x000698C0 File Offset: 0x00067AC0
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemModelsRenderer = base.Project.FindSubsystem<SubsystemModelsRenderer>(true);
			this.m_subsystemNoise = base.Project.FindSubsystem<SubsystemNoise>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_componentMiner = base.Entity.FindComponent<ComponentMiner>();
			this.m_componentRider = base.Entity.FindComponent<ComponentRider>();
			this.m_componentSleep = base.Entity.FindComponent<ComponentSleep>();
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>();
			this.m_walkAnimationSpeed = valuesDictionary.GetValue<float>("WalkAnimationSpeed");
			this.m_walkBobHeight = valuesDictionary.GetValue<float>("WalkBobHeight");
			this.m_walkLegsAngle = valuesDictionary.GetValue<float>("WalkLegsAngle");
		}

		// Token: 0x06000DBF RID: 3519 RVA: 0x00069994 File Offset: 0x00067B94
		public override void SetModel(Model model)
		{
			base.SetModel(model);
			if (base.Model != null)
			{
				this.m_bodyBone = base.Model.FindBone("Body", true);
				this.m_headBone = base.Model.FindBone("Head", true);
				this.m_leg1Bone = base.Model.FindBone("Leg1", true);
				this.m_leg2Bone = base.Model.FindBone("Leg2", true);
				this.m_hand1Bone = base.Model.FindBone("Hand1", true);
				this.m_hand2Bone = base.Model.FindBone("Hand2", true);
				return;
			}
			this.m_bodyBone = null;
			this.m_headBone = null;
			this.m_leg1Bone = null;
			this.m_leg2Bone = null;
			this.m_hand1Bone = null;
			this.m_hand2Bone = null;
		}

		// Token: 0x06000DC0 RID: 3520 RVA: 0x00069A68 File Offset: 0x00067C68
		public override Vector3 CalculateEyePosition()
		{
			float f = MathUtils.Sigmoid(this.m_lieDownFactorEye, 1f);
			float num = MathUtils.Sigmoid(this.m_sneakFactor, 4f);
			float num2 = 0.875f * this.m_componentCreature.ComponentBody.BoxSize.Y;
			float num3 = MathUtils.Lerp(MathUtils.Lerp(num2, 0.45f * num2, num), 0.2f * num2, f);
			Matrix matrix = this.m_componentCreature.ComponentBody.Matrix;
			return this.m_componentCreature.ComponentBody.Position + matrix.Up * (num3 + 2f * base.Bob) + matrix.Forward * -0.2f * num;
		}

		// Token: 0x06000DC1 RID: 3521 RVA: 0x00069B2C File Offset: 0x00067D2C
		public override Quaternion CalculateEyeRotation()
		{
			float num = 0f;
			if (this.m_lieDownFactorEye != 0f)
			{
				num += MathUtils.DegToRad(80f) * MathUtils.Sigmoid(MathUtils.Max(this.m_lieDownFactorEye - 0.2f, 0f) / 0.8f, 4f);
			}
			return this.m_componentCreature.ComponentBody.Rotation * Quaternion.CreateFromYawPitchRoll(0f - this.m_componentCreature.ComponentLocomotion.LookAngles.X, this.m_componentCreature.ComponentLocomotion.LookAngles.Y, num);
		}

		// Token: 0x0400088B RID: 2187
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400088C RID: 2188
		public SubsystemModelsRenderer m_subsystemModelsRenderer;

		// Token: 0x0400088D RID: 2189
		public SubsystemNoise m_subsystemNoise;

		// Token: 0x0400088E RID: 2190
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400088F RID: 2191
		public ComponentMiner m_componentMiner;

		// Token: 0x04000890 RID: 2192
		public ComponentRider m_componentRider;

		// Token: 0x04000891 RID: 2193
		public ComponentSleep m_componentSleep;

		// Token: 0x04000892 RID: 2194
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000893 RID: 2195
		public new Game.Random m_random = new Game.Random();

		// Token: 0x04000894 RID: 2196
		public DrawBlockEnvironmentData m_drawBlockEnvironmentData = new DrawBlockEnvironmentData();

		// Token: 0x04000895 RID: 2197
		public ModelBone m_bodyBone;

		// Token: 0x04000896 RID: 2198
		public ModelBone m_headBone;

		// Token: 0x04000897 RID: 2199
		public ModelBone m_leg1Bone;

		// Token: 0x04000898 RID: 2200
		public ModelBone m_leg2Bone;

		// Token: 0x04000899 RID: 2201
		public ModelBone m_hand1Bone;

		// Token: 0x0400089A RID: 2202
		public ModelBone m_hand2Bone;

		// Token: 0x0400089B RID: 2203
		public float m_sneakFactor;

		// Token: 0x0400089C RID: 2204
		public float m_lieDownFactorEye;

		// Token: 0x0400089D RID: 2205
		public float m_lieDownFactorModel;

		// Token: 0x0400089E RID: 2206
		public float m_walkAnimationSpeed;

		// Token: 0x0400089F RID: 2207
		public float m_walkLegsAngle;

		// Token: 0x040008A0 RID: 2208
		public float m_walkBobHeight;

		// Token: 0x040008A1 RID: 2209
		public float m_headingOffset;

		// Token: 0x040008A2 RID: 2210
		public float m_punchFactor;

		// Token: 0x040008A3 RID: 2211
		public float m_punchPhase;

		// Token: 0x040008A4 RID: 2212
		public int m_punchCounter;

		// Token: 0x040008A5 RID: 2213
		public float m_footstepsPhase;

		// Token: 0x040008A6 RID: 2214
		public bool m_rowLeft;

		// Token: 0x040008A7 RID: 2215
		public bool m_rowRight;

		// Token: 0x040008A8 RID: 2216
		public float m_aimHandAngle;

		// Token: 0x040008A9 RID: 2217
		public Vector3 m_inHandItemOffset;

		// Token: 0x040008AA RID: 2218
		public Vector3 m_inHandItemRotation;

		// Token: 0x040008AB RID: 2219
		public Vector2 m_headAngles;

		// Token: 0x040008AC RID: 2220
		public Vector2 m_handAngles1;

		// Token: 0x040008AD RID: 2221
		public Vector2 m_handAngles2;

		// Token: 0x040008AE RID: 2222
		public Vector2 m_legAngles1;

		// Token: 0x040008AF RID: 2223
		public Vector2 m_legAngles2;
	}
}
