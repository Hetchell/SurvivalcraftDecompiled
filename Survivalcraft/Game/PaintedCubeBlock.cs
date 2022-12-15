using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000C2 RID: 194
	public abstract class PaintedCubeBlock : CubeBlock, IPaintableBlock
	{
		// Token: 0x060003AB RID: 939 RVA: 0x000143C2 File Offset: 0x000125C2
		public PaintedCubeBlock(int coloredTextureSlot)
		{
			this.m_coloredTextureSlot = coloredTextureSlot;
		}

		// Token: 0x060003AC RID: 940 RVA: 0x000143D1 File Offset: 0x000125D1
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (!PaintedCubeBlock.IsColored(Terrain.ExtractData(value)))
			{
				return this.DefaultTextureSlot;
			}
			return this.m_coloredTextureSlot;
		}

		// Token: 0x060003AD RID: 941 RVA: 0x000143F0 File Offset: 0x000125F0
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			Color color = SubsystemPalette.GetColor(generator, PaintedCubeBlock.GetColor(data));
			generator.GenerateCubeVertices(this, value, x, y, z, color, geometry.OpaqueSubsetsByFace);
		}

		// Token: 0x060003AE RID: 942 RVA: 0x00014428 File Offset: 0x00012628
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int data = Terrain.ExtractData(value);
			color *= SubsystemPalette.GetColor(environmentData, PaintedCubeBlock.GetColor(data));
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}

		// Token: 0x060003AF RID: 943 RVA: 0x00014465 File Offset: 0x00012665
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, PaintedCubeBlock.SetColor(0, null));
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(this.BlockIndex, 0, PaintedCubeBlock.SetColor(0, new int?(i)));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x00014478 File Offset: 0x00012678
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			if (PaintedCubeBlock.GetColor(data) != null)
			{
				showDebris = true;
				if (toolLevel >= this.RequiredToolLevel)
				{
					dropValues.Add(new BlockDropValue
					{
						Value = Terrain.MakeBlockValue(this.DefaultDropContent, 0, data),
						Count = (int)this.DefaultDropCount
					});
					return;
				}
			}
			else
			{
				base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
			}
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x000144EC File Offset: 0x000126EC
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int data = Terrain.ExtractData(value);
			Color color = SubsystemPalette.GetColor(subsystemTerrain, PaintedCubeBlock.GetColor(data));
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, color, this.GetFaceTextureSlot(0, value));
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x00014528 File Offset: 0x00012728
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			return SubsystemPalette.GetName(subsystemTerrain, PaintedCubeBlock.GetColor(data), LanguageControl.GetBlock(string.Format("{0}:{1}", base.GetType().Name, data.ToString()), "DisplayName"));
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x00014570 File Offset: 0x00012770
		public override string GetCategory(int value)
		{
			if (PaintedCubeBlock.GetColor(Terrain.ExtractData(value)) == null)
			{
				return base.GetCategory(value);
			}
			return LanguageControl.Get("BlocksManager", "Painted");
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x000145A9 File Offset: 0x000127A9
		public int? GetPaintColor(int value)
		{
			return PaintedCubeBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x000145B8 File Offset: 0x000127B8
		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, PaintedCubeBlock.SetColor(data, color));
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x000145D9 File Offset: 0x000127D9
		public static bool IsColored(int data)
		{
			return (data & 1) != 0;
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x000145E4 File Offset: 0x000127E4
		public static int? GetColor(int data)
		{
			if ((data & 1) != 0)
			{
				return new int?(data >> 1 & 15);
			}
			return null;
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x0001460B File Offset: 0x0001280B
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -32) | 1 | color.Value << 1;
			}
			return data & -32;
		}

		// Token: 0x040001AC RID: 428
		public int m_coloredTextureSlot;
	}
}
