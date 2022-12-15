using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000B9 RID: 185
	public class MusketBlock : Block
	{
		// Token: 0x06000376 RID: 886 RVA: 0x00013778 File Offset: 0x00011978
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Musket");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Musket", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Hammer", true).ParentBone);
			this.m_standaloneBlockMeshUnloaded = new BlockMesh();
			this.m_standaloneBlockMeshUnloaded.AppendModelMeshPart(model.FindMesh("Musket", true).MeshParts[0], boneAbsoluteTransform, false, false, false, false, Color.White);
			this.m_standaloneBlockMeshUnloaded.AppendModelMeshPart(model.FindMesh("Hammer", true).MeshParts[0], boneAbsoluteTransform2, false, false, false, false, Color.White);
			this.m_standaloneBlockMeshLoaded = new BlockMesh();
			this.m_standaloneBlockMeshLoaded.AppendModelMeshPart(model.FindMesh("Musket", true).MeshParts[0], boneAbsoluteTransform, false, false, false, false, Color.White);
			this.m_standaloneBlockMeshLoaded.AppendModelMeshPart(model.FindMesh("Hammer", true).MeshParts[0], Matrix.CreateRotationX(0.7f) * boneAbsoluteTransform2, false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000377 RID: 887 RVA: 0x000138A5 File Offset: 0x00011AA5
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000378 RID: 888 RVA: 0x000138A8 File Offset: 0x00011AA8
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			if (MusketBlock.GetHammerState(Terrain.ExtractData(value)))
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshLoaded, color, 2f * size, ref matrix, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshUnloaded, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000379 RID: 889 RVA: 0x000138F8 File Offset: 0x00011AF8
		public override bool IsSwapAnimationNeeded(int oldValue, int newValue)
		{
			if (Terrain.ExtractContents(oldValue) != 212)
			{
				return true;
			}
			int data = Terrain.ExtractData(oldValue);
			return MusketBlock.SetHammerState(Terrain.ExtractData(newValue), true) != MusketBlock.SetHammerState(data, true);
		}

		// Token: 0x0600037A RID: 890 RVA: 0x00013933 File Offset: 0x00011B33
		public override int GetDamage(int value)
		{
			return Terrain.ExtractData(value) >> 8 & 255;
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00013944 File Offset: 0x00011B44
		public override int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			num &= -65281;
			num |= MathUtils.Clamp(damage, 0, 255) << 8;
			return Terrain.ReplaceData(value, num);
		}

		// Token: 0x0600037C RID: 892 RVA: 0x00013978 File Offset: 0x00011B78
		public static MusketBlock.LoadState GetLoadState(int data)
		{
			return (MusketBlock.LoadState)(data & 3);
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0001397D File Offset: 0x00011B7D
		public static int SetLoadState(int data, MusketBlock.LoadState loadState)
		{
			return (data & -4) | (int)(loadState & MusketBlock.LoadState.Loaded);
		}

		// Token: 0x0600037E RID: 894 RVA: 0x00013987 File Offset: 0x00011B87
		public static bool GetHammerState(int data)
		{
			return (data & 4) != 0;
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0001398F File Offset: 0x00011B8F
		public static int SetHammerState(int data, bool state)
		{
			return (data & -5) | (state ? 1 : 0) << 2;
		}

		// Token: 0x06000380 RID: 896 RVA: 0x000139A0 File Offset: 0x00011BA0
		public static BulletBlock.BulletType? GetBulletType(int data)
		{
			int num = data >> 4 & 15;
			if (num != 0)
			{
				return new BulletBlock.BulletType?((BulletBlock.BulletType)(num - 1));
			}
			return null;
		}

		// Token: 0x06000381 RID: 897 RVA: 0x000139CC File Offset: 0x00011BCC
		public static int SetBulletType(int data, BulletBlock.BulletType? bulletType)
		{
			int num = (int)((bulletType != null) ? (bulletType.Value + 1) : BulletBlock.BulletType.MusketBall);
			return (data & -241) | (num & 15) << 4;
		}

		// Token: 0x0400019C RID: 412
		public const int Index = 212;

		// Token: 0x0400019D RID: 413
		public BlockMesh m_standaloneBlockMeshUnloaded;

		// Token: 0x0400019E RID: 414
		public BlockMesh m_standaloneBlockMeshLoaded;

		// Token: 0x020003D9 RID: 985
		public enum LoadState
		{
			// Token: 0x04001462 RID: 5218
			Empty,
			// Token: 0x04001463 RID: 5219
			Gunpowder,
			// Token: 0x04001464 RID: 5220
			Wad,
			// Token: 0x04001465 RID: 5221
			Loaded
		}
	}
}
