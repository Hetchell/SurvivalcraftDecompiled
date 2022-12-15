using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000071 RID: 113
	public class FurnitureBlock : Block, IPaintableBlock, IElectricElementBlock
	{
		// Token: 0x06000266 RID: 614 RVA: 0x0000E324 File Offset: 0x0000C524
		public override void Initialize()
		{
			for (int i = 0; i < 4; i++)
			{
				this.m_matrices[i] = Matrix.CreateTranslation(new Vector3(-0.5f, 0f, -0.5f)) * Matrix.CreateRotationY((float)i * 3.1415927f / 2f) * Matrix.CreateTranslation(new Vector3(0.5f, 0f, 0.5f));
			}
			base.Initialize();
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000E3A0 File Offset: 0x0000C5A0
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			if (generator.SubsystemFurnitureBlockBehavior == null)
			{
				return;
			}
			int data = Terrain.ExtractData(value);
			int designIndex = FurnitureBlock.GetDesignIndex(data);
			int rotation = FurnitureBlock.GetRotation(data);
			FurnitureDesign design = generator.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
			if (design == null)
			{
				return;
			}
			FurnitureGeometry geometry2 = design.Geometry;
			int mountingFacesMask = design.MountingFacesMask;
			for (int i = 0; i < 6; i++)
			{
				int num = CellFace.OppositeFace((i < 4) ? ((i + rotation) % 4) : i);
				byte b = (byte)(LightingManager.LightIntensityByLightValueAndFace[15 + 16 * num] * 255f);
				Color color = new Color(b, b, b);
				if (geometry2.SubsetOpaqueByFace[i] != null)
				{
					generator.GenerateShadedMeshVertices(this, x, y, z, geometry2.SubsetOpaqueByFace[i], color, new Matrix?(this.m_matrices[rotation]), this.m_facesMaps[rotation], geometry.OpaqueSubsetsByFace[num]);
				}
				if (geometry2.SubsetAlphaTestByFace[i] != null)
				{
					generator.GenerateShadedMeshVertices(this, x, y, z, geometry2.SubsetAlphaTestByFace[i], color, new Matrix?(this.m_matrices[rotation]), this.m_facesMaps[rotation], geometry.AlphaTestSubsetsByFace[num]);
				}
				int num2 = CellFace.OppositeFace((i < 4) ? ((i - rotation + 4) % 4) : i);
				if ((mountingFacesMask & 1 << num2) != 0)
				{
					generator.GenerateWireVertices(value, x, y, z, i, 0f, Vector2.Zero, geometry.SubsetOpaque);
				}
			}
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000E508 File Offset: 0x0000C708
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			if (environmentData.SubsystemTerrain == null)
			{
				return;
			}
			int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
			FurnitureDesign design = environmentData.SubsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
			if (design == null)
			{
				return;
			}
			Matrix matrix2 = Matrix.CreateTranslation(new Vector3
			{
				X = -0.5f * (float)(design.Box.Left + design.Box.Right) / (float)design.Resolution,
				Y = -0.5f * (float)(design.Box.Top + design.Box.Bottom) / (float)design.Resolution,
				Z = -0.5f * (float)(design.Box.Near + design.Box.Far) / (float)design.Resolution
			} * size) * matrix;
			FurnitureGeometry geometry = design.Geometry;
			for (int i = 0; i < 6; i++)
			{
				float s = LightingManager.LightIntensityByLightValueAndFace[environmentData.Light + 16 * CellFace.OppositeFace(i)];
				Color color2 = Color.MultiplyColorOnly(color, s);
				if (geometry.SubsetOpaqueByFace[i] != null)
				{
					BlocksManager.DrawMeshBlock(primitivesRenderer, geometry.SubsetOpaqueByFace[i], color2, size, ref matrix2, environmentData);
				}
				if (geometry.SubsetAlphaTestByFace[i] != null)
				{
					BlocksManager.DrawMeshBlock(primitivesRenderer, geometry.SubsetAlphaTestByFace[i], color2, size, ref matrix2, environmentData);
				}
			}
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000E67C File Offset: 0x0000C87C
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			if (subsystemTerrain != null)
			{
				int data = Terrain.ExtractData(value);
				int rotation = FurnitureBlock.GetRotation(data);
				int designIndex = FurnitureBlock.GetDesignIndex(data);
				FurnitureDesign design = subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				if (design != null)
				{
					return (1 << this.m_reverseFacesMaps[rotation][face] & design.TransparentFacesMask) != 0;
				}
			}
			return false;
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000E6CC File Offset: 0x0000C8CC
		public override int GetShadowStrength(int value)
		{
			int data = Terrain.ExtractData(value);
			if (FurnitureBlock.GetIsLightEmitter(data))
			{
				return -99;
			}
			return FurnitureBlock.GetShadowStrengthFactor(data) * 3 + 1;
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000E6F5 File Offset: 0x0000C8F5
		public override int GetEmittedLightAmount(int value)
		{
			if (!FurnitureBlock.GetIsLightEmitter(Terrain.ExtractData(value)))
			{
				return 0;
			}
			return 15;
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000E708 File Offset: 0x0000C908
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			if (subsystemTerrain != null)
			{
				int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
				FurnitureDesign design = subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				if (design != null)
				{
					if (!string.IsNullOrEmpty(design.Name))
					{
						return design.Name;
					}
					return design.GetDefaultName();
				}
			}
			return "家具";
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000E754 File Offset: 0x0000C954
		public override bool IsInteractive(SubsystemTerrain subsystemTerrain, int value)
		{
			if (subsystemTerrain != null)
			{
				int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
				FurnitureDesign design = subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				if (design != null)
				{
					return design.InteractionMode == FurnitureInteractionMode.Multistate || design.InteractionMode == FurnitureInteractionMode.ElectricButton || design.InteractionMode == FurnitureInteractionMode.ElectricSwitch || design.InteractionMode == FurnitureInteractionMode.ConnectedMultistate;
				}
			}
			return base.IsInteractive(subsystemTerrain, value);
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000E7B0 File Offset: 0x0000C9B0
		public override string GetSoundMaterialName(SubsystemTerrain subsystemTerrain, int value)
		{
			if (subsystemTerrain != null)
			{
				int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
				FurnitureDesign design = subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				if (design != null)
				{
					int mainValue = design.MainValue;
					int num = Terrain.ExtractContents(mainValue);
					return BlocksManager.Blocks[num].GetSoundMaterialName(subsystemTerrain, mainValue);
				}
			}
			return base.GetSoundMaterialName(subsystemTerrain, value);
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000E804 File Offset: 0x0000CA04
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain subsystemTerrain, int value)
		{
			if (subsystemTerrain != null)
			{
				int data = Terrain.ExtractData(value);
				int designIndex = FurnitureBlock.GetDesignIndex(data);
				int rotation = FurnitureBlock.GetRotation(data);
				FurnitureDesign design = subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				if (design != null)
				{
					return design.GetCollisionBoxes(rotation);
				}
			}
			return base.GetCustomCollisionBoxes(subsystemTerrain, value);
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000E848 File Offset: 0x0000CA48
		public override BoundingBox[] GetCustomInteractionBoxes(SubsystemTerrain subsystemTerrain, int value)
		{
			if (subsystemTerrain != null)
			{
				int data = Terrain.ExtractData(value);
				int designIndex = FurnitureBlock.GetDesignIndex(data);
				int rotation = FurnitureBlock.GetRotation(data);
				FurnitureDesign design = subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				if (design != null)
				{
					return design.GetInteractionBoxes(rotation);
				}
			}
			return base.GetCustomInteractionBoxes(subsystemTerrain, value);
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000E88C File Offset: 0x0000CA8C
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int faceTextureSlot = this.GetFaceTextureSlot(4, value);
			int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
			FurnitureDesign design = subsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
			if (design != null)
			{
				int mainValue = design.MainValue;
				int num = Terrain.ExtractContents(mainValue);
				return BlocksManager.Blocks[num].CreateDebrisParticleSystem(subsystemTerrain, position, mainValue, strength);
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, faceTextureSlot);
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000E8F8 File Offset: 0x0000CAF8
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int rotation = 0;
			if (raycastResult.CellFace.Face < 4)
			{
				rotation = CellFace.OppositeFace(raycastResult.CellFace.Face);
			}
			else
			{
				Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
				float num = Vector3.Dot(forward, Vector3.UnitZ);
				float num2 = Vector3.Dot(forward, Vector3.UnitX);
				float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
				float num4 = Vector3.Dot(forward, -Vector3.UnitX);
				if (num == MathUtils.Max(num, num2, num3, num4))
				{
					rotation = 0;
				}
				else if (num2 == MathUtils.Max(num, num2, num3, num4))
				{
					rotation = 1;
				}
				else if (num3 == MathUtils.Max(num, num2, num3, num4))
				{
					rotation = 2;
				}
				else if (num4 == MathUtils.Max(num, num2, num3, num4))
				{
					rotation = 3;
				}
			}
			int data = FurnitureBlock.SetRotation(Terrain.ExtractData(value), rotation);
			return new BlockPlacementData
			{
				CellFace = raycastResult.CellFace,
				Value = Terrain.ReplaceData(value, data)
			};
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000EA08 File Offset: 0x0000CC08
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			int data = Terrain.ExtractData(oldValue);
			data = FurnitureBlock.SetRotation(data, 0);
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(227, 0, data),
				Count = 1
			});
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000EA54 File Offset: 0x0000CC54
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			if (environmentData.SubsystemTerrain != null)
			{
				int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
				FurnitureDesign design = environmentData.SubsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
				if (design != null)
				{
					float num = (float)design.Resolution / (float)MathUtils.Max(design.Box.Width, design.Box.Height, design.Box.Depth);
					return this.DefaultIconViewScale * num;
				}
			}
			return base.GetIconViewScale(value, environmentData);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000EACC File Offset: 0x0000CCCC
		public int? GetPaintColor(int value)
		{
			return null;
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000EAE4 File Offset: 0x0000CCE4
		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			int designIndex = FurnitureBlock.GetDesignIndex(data);
			FurnitureDesign design = terrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
			if (design != null)
			{
				List<FurnitureDesign> list = design.CloneChain();
				foreach (FurnitureDesign furnitureDesign in list)
				{
					furnitureDesign.Paint(color);
				}
				FurnitureDesign furnitureDesign2 = terrain.SubsystemFurnitureBlockBehavior.TryAddDesignChain(list[0], true);
				if (furnitureDesign2 != null)
				{
					int data2 = FurnitureBlock.SetDesignIndex(data, furnitureDesign2.Index, furnitureDesign2.ShadowStrengthFactor, furnitureDesign2.IsLightEmitter);
					return Terrain.ReplaceData(value, data2);
				}
				this.DisplayError();
			}
			return value;
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000EB9C File Offset: 0x0000CD9C
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain terrain, string[] ingredients, float heatLevel, float playerLevel)
		{
			if (heatLevel != 0f)
			{
				return null;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			List<FurnitureDesign> list = new List<FurnitureDesign>();
			for (int i = 0; i < ingredients.Length; i++)
			{
				if (!string.IsNullOrEmpty(ingredients[i]))
				{
					string a;
					int? num4;
					CraftingRecipesManager.DecodeIngredient(ingredients[i], out a, out num4);
					if (a == BlocksManager.Blocks[227].CraftingId)
					{
						FurnitureDesign design = terrain.SubsystemFurnitureBlockBehavior.GetDesign(FurnitureBlock.GetDesignIndex(num4.GetValueOrDefault()));
						if (design == null)
						{
							return null;
						}
						list.Add(design);
					}
					else if (a == BlocksManager.Blocks[142].CraftingId)
					{
						num++;
					}
					else if (a == BlocksManager.Blocks[141].CraftingId)
					{
						num2++;
					}
					else
					{
						if (!(a == BlocksManager.Blocks[133].CraftingId))
						{
							return null;
						}
						num3++;
					}
				}
			}
			if (list.Count == 1 && num == 1 && num2 == 0 && num3 == 0)
			{
				FurnitureDesign furnitureDesign = list[0].Clone();
				furnitureDesign.InteractionMode = FurnitureInteractionMode.ElectricButton;
				FurnitureDesign furnitureDesign2 = terrain.SubsystemFurnitureBlockBehavior.TryAddDesignChain(furnitureDesign, true);
				if (furnitureDesign2 == null)
				{
					this.DisplayError();
					return null;
				}
				return new CraftingRecipe
				{
					ResultValue = Terrain.MakeBlockValue(227, 0, FurnitureBlock.SetDesignIndex(0, furnitureDesign2.Index, furnitureDesign2.ShadowStrengthFactor, furnitureDesign2.IsLightEmitter)),
					ResultCount = 1,
					Description = "Combine furniture into interactive design",
					Ingredients = (string[])ingredients.Clone()
				};
			}
			else if (list.Count == 2 && num == 0 && num2 == 1 && num3 == 0)
			{
				List<FurnitureDesign> list2 = (from d in list
				select d.Clone()).ToList<FurnitureDesign>();
				for (int j = 0; j < list2.Count; j++)
				{
					list2[j].InteractionMode = FurnitureInteractionMode.ElectricSwitch;
					list2[j].LinkedDesign = list2[(j + 1) % list2.Count];
				}
				FurnitureDesign furnitureDesign3 = terrain.SubsystemFurnitureBlockBehavior.TryAddDesignChain(list2[0], true);
				if (furnitureDesign3 == null)
				{
					this.DisplayError();
					return null;
				}
				return new CraftingRecipe
				{
					ResultValue = Terrain.MakeBlockValue(227, 0, FurnitureBlock.SetDesignIndex(0, furnitureDesign3.Index, furnitureDesign3.ShadowStrengthFactor, furnitureDesign3.IsLightEmitter)),
					ResultCount = 1,
					Description = "Combine furniture into interactive design",
					Ingredients = (string[])ingredients.Clone()
				};
			}
			else
			{
				if (list.Count < 2 || num != 0 || num2 != 0 || num3 > 1)
				{
					return null;
				}
				List<FurnitureDesign> list3 = (from d in list
				select d.Clone()).ToList<FurnitureDesign>();
				for (int k = 0; k < list3.Count; k++)
				{
					list3[k].InteractionMode = ((num3 == 0) ? FurnitureInteractionMode.Multistate : FurnitureInteractionMode.ConnectedMultistate);
					list3[k].LinkedDesign = list3[(k + 1) % list3.Count];
				}
				FurnitureDesign furnitureDesign4 = terrain.SubsystemFurnitureBlockBehavior.TryAddDesignChain(list3[0], true);
				if (furnitureDesign4 == null)
				{
					this.DisplayError();
					return null;
				}
				return new CraftingRecipe
				{
					ResultValue = Terrain.MakeBlockValue(227, 0, FurnitureBlock.SetDesignIndex(0, furnitureDesign4.Index, furnitureDesign4.ShadowStrengthFactor, furnitureDesign4.IsLightEmitter)),
					ResultCount = 1,
					Description = "Combine furniture into interactive design",
					Ingredients = (string[])ingredients.Clone()
				};
			}
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000EF50 File Offset: 0x0000D150
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
			FurnitureDesign design = subsystemElectricity.SubsystemTerrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
			if (design != null)
			{
				if (design.InteractionMode == FurnitureInteractionMode.Multistate || design.InteractionMode == FurnitureInteractionMode.ConnectedMultistate)
				{
					return new MultistateFurnitureElectricElement(subsystemElectricity, new Point3(x, y, z));
				}
				if (design.InteractionMode == FurnitureInteractionMode.ElectricButton)
				{
					return new ButtonFurnitureElectricElement(subsystemElectricity, new Point3(x, y, z));
				}
				if (design.InteractionMode == FurnitureInteractionMode.ElectricSwitch)
				{
					return new SwitchFurnitureElectricElement(subsystemElectricity, new Point3(x, y, z), value);
				}
			}
			return null;
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000EFD8 File Offset: 0x0000D1D8
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int rotation = FurnitureBlock.GetRotation(data);
			int designIndex = FurnitureBlock.GetDesignIndex(data);
			FurnitureDesign design = terrain.SubsystemFurnitureBlockBehavior.GetDesign(designIndex);
			if (design != null)
			{
				int num = CellFace.OppositeFace((face < 4) ? ((face - rotation + 4) % 4) : face);
				if ((design.MountingFacesMask & 1 << num) != 0 && SubsystemElectricity.GetConnectorDirection(face, 0, connectorFace) != null)
				{
					Point3 point = CellFace.FaceToPoint3(face);
					int cellValue = terrain.Terrain.GetCellValue(x - point.X, y - point.Y, z - point.Z);
					if (!BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].IsFaceTransparent(terrain, CellFace.OppositeFace(num), cellValue))
					{
						if (design.InteractionMode == FurnitureInteractionMode.Multistate || design.InteractionMode == FurnitureInteractionMode.ConnectedMultistate)
						{
							return new ElectricConnectorType?(ElectricConnectorType.Input);
						}
						if (design.InteractionMode == FurnitureInteractionMode.ElectricButton || design.InteractionMode == FurnitureInteractionMode.ElectricSwitch)
						{
							return new ElectricConnectorType?(ElectricConnectorType.Output);
						}
					}
				}
			}
			return null;
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000F0D4 File Offset: 0x0000D2D4
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000F0DB File Offset: 0x0000D2DB
		public void DisplayError()
		{
			DialogsManager.ShowDialog(null, new MessageDialog("Error", "Too many different furniture designs", "确定", null, null));
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000F0F9 File Offset: 0x0000D2F9
		public static int GetRotation(int data)
		{
			return data & 3;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000F0FE File Offset: 0x0000D2FE
		public static int SetRotation(int data, int rotation)
		{
			return (data & -4) | (rotation & 3);
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000F108 File Offset: 0x0000D308
		public static int GetDesignIndex(int data)
		{
			return data >> 2 & 1023;
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000F113 File Offset: 0x0000D313
		public static int SetDesignIndex(int data, int designIndex, int shadowStrengthFactor, bool isLightEmitter)
		{
			data = ((data & -4093) | (designIndex & 1023) << 2);
			data = ((data & -12289) | (shadowStrengthFactor & 3) << 12);
			data = ((data & -16385) | (isLightEmitter ? 1 : 0) << 14);
			return data;
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000F150 File Offset: 0x0000D350
		public static FurnitureDesign GetDesign(SubsystemFurnitureBlockBehavior subsystemFurnitureBlockBehavior, int value)
		{
			int designIndex = FurnitureBlock.GetDesignIndex(Terrain.ExtractData(value));
			return subsystemFurnitureBlockBehavior.GetDesign(designIndex);
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000F170 File Offset: 0x0000D370
		public static int GetShadowStrengthFactor(int data)
		{
			return data >> 12 & 3;
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000F178 File Offset: 0x0000D378
		public static bool GetIsLightEmitter(int data)
		{
			return (data >> 14 & 1) != 0;
		}

		// Token: 0x0400011A RID: 282
		public const int Index = 227;

		// Token: 0x0400011B RID: 283
		public Matrix[] m_matrices = new Matrix[4];

		// Token: 0x0400011C RID: 284
		public int[][] m_facesMaps = new int[][]
		{
			new int[]
			{
				0,
				1,
				2,
				3,
				4,
				5
			},
			new int[]
			{
				1,
				2,
				3,
				0,
				4,
				5
			},
			new int[]
			{
				2,
				3,
				0,
				1,
				4,
				5
			},
			new int[]
			{
				3,
				0,
				1,
				2,
				4,
				5
			}
		};

		// Token: 0x0400011D RID: 285
		public int[][] m_reverseFacesMaps = new int[][]
		{
			new int[]
			{
				0,
				1,
				2,
				3,
				4,
				5
			},
			new int[]
			{
				3,
				0,
				1,
				2,
				4,
				5
			},
			new int[]
			{
				2,
				3,
				0,
				1,
				4,
				5
			},
			new int[]
			{
				1,
				2,
				3,
				0,
				4,
				5
			}
		};
	}
}
