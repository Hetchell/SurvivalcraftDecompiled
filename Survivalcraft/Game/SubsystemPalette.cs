using System;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000196 RID: 406
	public class SubsystemPalette : Subsystem
	{
		// Token: 0x06000971 RID: 2417 RVA: 0x000423CB File Offset: 0x000405CB
		static SubsystemPalette()
		{
			SubsystemPalette.m_defaultFabricColors = SubsystemPalette.CreateFabricColors(WorldPalette.DefaultColors);
		}

		// Token: 0x06000972 RID: 2418 RVA: 0x000423E8 File Offset: 0x000405E8
		public Color GetColor(int index)
		{
			return this.m_colors[index];
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x000423F6 File Offset: 0x000405F6
		public string GetName(int index)
		{
			return this.m_names[index];
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x00042400 File Offset: 0x00040600
		public Color GetFabricColor(int index)
		{
			return this.m_fabricColors[index];
		}

		// Token: 0x06000975 RID: 2421 RVA: 0x0004240E File Offset: 0x0004060E
		public static Color GetColor(BlockGeometryGenerator generator, int? index)
		{
			if (index == null)
			{
				return Color.White;
			}
			if (generator.SubsystemPalette != null)
			{
				return generator.SubsystemPalette.GetColor(index.Value);
			}
			return WorldPalette.DefaultColors[index.Value];
		}

		// Token: 0x06000976 RID: 2422 RVA: 0x0004244B File Offset: 0x0004064B
		public static Color GetColor(DrawBlockEnvironmentData environmentData, int? index)
		{
			return SubsystemPalette.GetColor(environmentData.SubsystemTerrain, index);
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x00042459 File Offset: 0x00040659
		public static Color GetColor(SubsystemTerrain subsystemTerrain, int? index)
		{
			if (index == null)
			{
				return Color.White;
			}
			if (subsystemTerrain != null && subsystemTerrain.SubsystemPalette != null)
			{
				return subsystemTerrain.SubsystemPalette.GetColor(index.Value);
			}
			return WorldPalette.DefaultColors[index.Value];
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x00042499 File Offset: 0x00040699
		public static Color GetFabricColor(BlockGeometryGenerator generator, int? index)
		{
			if (index == null)
			{
				return Color.White;
			}
			if (generator.SubsystemPalette != null)
			{
				return generator.SubsystemPalette.GetFabricColor(index.Value);
			}
			return SubsystemPalette.m_defaultFabricColors[index.Value];
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x000424D6 File Offset: 0x000406D6
		public static Color GetFabricColor(DrawBlockEnvironmentData environmentData, int? index)
		{
			return SubsystemPalette.GetFabricColor(environmentData.SubsystemTerrain, index);
		}

		// Token: 0x0600097A RID: 2426 RVA: 0x000424E4 File Offset: 0x000406E4
		public static Color GetFabricColor(SubsystemTerrain subsystemTerrain, int? index)
		{
			if (index == null)
			{
				return Color.White;
			}
			if (subsystemTerrain != null && subsystemTerrain.SubsystemPalette != null)
			{
				return subsystemTerrain.SubsystemPalette.GetFabricColor(index.Value);
			}
			return SubsystemPalette.m_defaultFabricColors[index.Value];
		}

		// Token: 0x0600097B RID: 2427 RVA: 0x00042524 File Offset: 0x00040724
		public static string GetName(SubsystemTerrain subsystemTerrain, int? index, string suffix)
		{
			if (index == null)
			{
				return suffix ?? string.Empty;
			}
			string text = LanguageControl.Get("WorldPalette", index.Value);
			if (!string.IsNullOrEmpty(suffix))
			{
				return text + " " + suffix;
			}
			return text;
		}

        // Token: 0x0600097C RID: 2428 RVA: 0x00042570 File Offset: 0x00040770
        public override void Load(ValuesDictionary valuesDictionary)
		{
			SubsystemGameInfo subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_colors = subsystemGameInfo.WorldSettings.Palette.Colors.ToArray<Color>();
			this.m_names = subsystemGameInfo.WorldSettings.Palette.Names.ToArray<string>();
			this.m_fabricColors = SubsystemPalette.CreateFabricColors(this.m_colors);
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x000425D4 File Offset: 0x000407D4
		public static Color[] CreateFabricColors(Color[] colors)
		{
			Color[] array = new Color[16];
			for (int i = 0; i < 16; i++)
			{
				Vector3 rgb = new Vector3(colors[i]);
				Vector3 hsv = Color.RgbToHsv(rgb);
				hsv.Y *= 0.85f;
				rgb = Color.HsvToRgb(hsv);
				array[i] = new Color(rgb);
			}
			return array;
		}

		// Token: 0x040004FB RID: 1275
		public static readonly Color[] m_defaultFabricColors = new Color[16];

		// Token: 0x040004FC RID: 1276
		public string[] m_names;

		// Token: 0x040004FD RID: 1277
		public Color[] m_colors;

		// Token: 0x040004FE RID: 1278
		public Color[] m_fabricColors;
	}
}
