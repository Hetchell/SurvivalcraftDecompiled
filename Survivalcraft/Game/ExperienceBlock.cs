using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000062 RID: 98
	public class ExperienceBlock : Block
	{
		// Token: 0x060001E1 RID: 481 RVA: 0x0000B04F File Offset: 0x0000924F
		public override void Initialize()
		{
			base.Initialize();
			this.m_texture = ContentManager.Get<Texture2D>("Textures/Experience");
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000B067 File Offset: 0x00009267
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000B069 File Offset: 0x00009269
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size * 0.18f, ref matrix, this.m_texture, Color.White, true, environmentData);
		}

		// Token: 0x040000E4 RID: 228
		public const int Index = 248;

		// Token: 0x040000E5 RID: 229
		public Texture2D m_texture;
	}
}
