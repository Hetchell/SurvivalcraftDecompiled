using System;
using System.Collections.Generic;
using System.IO;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
	// Token: 0x0200022C RID: 556
	public static class BlocksTexturesManager
	{
		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06001119 RID: 4377 RVA: 0x00085C4B File Offset: 0x00083E4B
		// (set) Token: 0x0600111A RID: 4378 RVA: 0x00085C52 File Offset: 0x00083E52
		public static Texture2D DefaultBlocksTexture { get; set; }

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x0600111B RID: 4379 RVA: 0x00085C5A File Offset: 0x00083E5A
		public static ReadOnlyList<string> BlockTexturesNames
		{
			get
			{
				return new ReadOnlyList<string>(BlocksTexturesManager.m_blockTextureNames);
			}
		}

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x0600111C RID: 4380 RVA: 0x00085C66 File Offset: 0x00083E66
		public static string BlockTexturesDirectoryName
		{
			get
			{
				return "app:/TexturePacks";
			}
		}

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x0600111D RID: 4381 RVA: 0x00085C70 File Offset: 0x00083E70
		// (remove) Token: 0x0600111E RID: 4382 RVA: 0x00085CA4 File Offset: 0x00083EA4
		public static event Action<string> BlocksTextureDeleted;

		// Token: 0x0600111F RID: 4383 RVA: 0x00085CD7 File Offset: 0x00083ED7
		public static void Initialize()
		{
			Storage.CreateDirectory(BlocksTexturesManager.BlockTexturesDirectoryName);
			BlocksTexturesManager.DefaultBlocksTexture = ContentManager.Get<Texture2D>("Textures/Blocks");
		}

		// Token: 0x06001120 RID: 4384 RVA: 0x00085CF2 File Offset: 0x00083EF2
		public static bool IsBuiltIn(string name)
		{
			return string.IsNullOrEmpty(name);
		}

		// Token: 0x06001121 RID: 4385 RVA: 0x00085CFA File Offset: 0x00083EFA
		public static string GetFileName(string name)
		{
			if (BlocksTexturesManager.IsBuiltIn(name))
			{
				return null;
			}
			return Storage.CombinePaths(new string[]
			{
				BlocksTexturesManager.BlockTexturesDirectoryName,
				name
			});
		}

		// Token: 0x06001122 RID: 4386 RVA: 0x00085D1D File Offset: 0x00083F1D
		public static string GetDisplayName(string name)
		{
			if (BlocksTexturesManager.IsBuiltIn(name))
			{
				return "Survivalcraft";
			}
			return Storage.GetFileNameWithoutExtension(name);
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x00085D34 File Offset: 0x00083F34
		public static DateTime GetCreationDate(string name)
		{
			try
			{
				if (!BlocksTexturesManager.IsBuiltIn(name))
				{
					return Storage.GetFileLastWriteTime(BlocksTexturesManager.GetFileName(name));
				}
			}
			catch
			{
			}
			return new DateTime(2000, 1, 1);
		}

		// Token: 0x06001124 RID: 4388 RVA: 0x00085D7C File Offset: 0x00083F7C
		public static Texture2D LoadTexture(string name)
		{
			Texture2D texture2D = null;
			if (!BlocksTexturesManager.IsBuiltIn(name))
			{
				try
				{
					using (Stream stream = Storage.OpenFile(BlocksTexturesManager.GetFileName(name), OpenFileMode.Read))
					{
						BlocksTexturesManager.ValidateBlocksTexture(stream);
						stream.Position = 0L;
						texture2D = Texture2D.Load(stream, false, 1);
					}
				}
				catch (Exception ex)
				{
					Log.Warning(string.Concat(new string[]
					{
						"Could not load blocks texture \"",
						name,
						"\". Reason: ",
						ex.Message,
						"."
					}));
				}
			}
			if (texture2D == null)
			{
				texture2D = BlocksTexturesManager.DefaultBlocksTexture;
			}
			return texture2D;
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x00085E24 File Offset: 0x00084024
		public static string ImportBlocksTexture(string name, Stream stream)
		{
			Exception ex = ExternalContentManager.VerifyExternalContentName(name);
			if (ex != null)
			{
				throw ex;
			}
			if (Storage.GetExtension(name) != ".scbtex")
			{
				name += ".scbtex";
			}
			BlocksTexturesManager.ValidateBlocksTexture(stream);
			stream.Position = 0L;
			string result;
			using (Stream stream2 = Storage.OpenFile(BlocksTexturesManager.GetFileName(name), OpenFileMode.Create))
			{
				stream.CopyTo(stream2);
				result = name;
			}
			return result;
		}

		// Token: 0x06001126 RID: 4390 RVA: 0x00085EA0 File Offset: 0x000840A0
		public static void DeleteBlocksTexture(string name)
		{
			try
			{
				string fileName = BlocksTexturesManager.GetFileName(name);
				if (!string.IsNullOrEmpty(fileName))
				{
					Storage.DeleteFile(fileName);
					Action<string> blocksTextureDeleted = BlocksTexturesManager.BlocksTextureDeleted;
					if (blocksTextureDeleted != null)
					{
						blocksTextureDeleted(name);
					}
				}
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("Unable to delete blocks texture \"" + name + "\"", e);
			}
		}

		// Token: 0x06001127 RID: 4391 RVA: 0x00085F00 File Offset: 0x00084100
		public static void UpdateBlocksTexturesList()
		{
			BlocksTexturesManager.m_blockTextureNames.Clear();
			BlocksTexturesManager.m_blockTextureNames.Add(string.Empty);
			foreach (string item in Storage.ListFileNames(BlocksTexturesManager.BlockTexturesDirectoryName))
			{
				BlocksTexturesManager.m_blockTextureNames.Add(item);
			}
		}

		// Token: 0x06001128 RID: 4392 RVA: 0x00085F70 File Offset: 0x00084170
		public static void ValidateBlocksTexture(Stream stream)
		{
			Image image = Image.Load(stream);
			if (image.Width > 65535 || image.Height > 65535)
			{
				throw new InvalidOperationException(string.Format("Blocks texture is larger than 65535x65535 pixels (size={0}x{1})", image.Width, image.Height));
			}
			if (!MathUtils.IsPowerOf2((long)image.Width) || !MathUtils.IsPowerOf2((long)image.Height))
			{
				throw new InvalidOperationException(string.Format("Blocks texture does not have power-of-two size (size={0}x{1})", image.Width, image.Height));
			}
		}

		// Token: 0x04000B71 RID: 2929
		public static List<string> m_blockTextureNames = new List<string>();
	}
}
