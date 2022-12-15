using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Engine;
using Engine.Graphics;
using Engine.Serialization;

namespace Game
{
	// Token: 0x02000362 RID: 866
	public static class BlocksManager
	{
		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06001863 RID: 6243 RVA: 0x000C1480 File Offset: 0x000BF680
		public static Block[] Blocks
		{
			get
			{
				return BlocksManager.m_blocks;
			}
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06001864 RID: 6244 RVA: 0x000C1487 File Offset: 0x000BF687
		public static FluidBlock[] FluidBlocks
		{
			get
			{
				return BlocksManager.m_fluidBlocks;
			}
		}

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06001865 RID: 6245 RVA: 0x000C148E File Offset: 0x000BF68E
		public static ReadOnlyList<string> Categories
		{
			get
			{
				return new ReadOnlyList<string>(BlocksManager.m_categories);
			}
		}

		// Token: 0x06001866 RID: 6246 RVA: 0x000C149C File Offset: 0x000BF69C
		public static IEnumerable<TypeInfo> GetBlockTypes()
		{
			Func<IEnumerable<TypeInfo>> getBlockTypes = BlocksManager.GetBlockTypes1;
			if (getBlockTypes != null)
			{
				return getBlockTypes();
			}
			List<TypeInfo> list = new List<TypeInfo>();
			list.AddRange(typeof(BlocksManager).Assembly.DefinedTypes);
			foreach (Assembly assembly in TypeCache.LoadedAssemblies)
			{
				list.AddRange(assembly.DefinedTypes);
			}
			return list;
		}

		// Token: 0x06001867 RID: 6247 RVA: 0x000C1508 File Offset: 0x000BF708
		public static void Initialize()
		{
			if (BlocksManager.Initialize1 != null)
			{
				BlocksManager.Initialize1();
				return;
			}
			BlocksManager.CalculateSlotTexCoordTables();
			int i = 0;
			Dictionary<int, Block> dictionary = new Dictionary<int, Block>();
			foreach (TypeInfo typeInfo in BlocksManager.GetBlockTypes())
			{
				if (typeInfo.IsSubclassOf(typeof(Block)) && !typeInfo.IsAbstract)
				{
					FieldInfo fieldInfo = typeInfo.AsType().GetRuntimeFields().FirstOrDefault(new Func<FieldInfo, bool>(BlocksManager.c._.Initialize_b__11_0));
					if (!(fieldInfo != null) || !(fieldInfo.FieldType == typeof(int)))
					{
						throw new InvalidOperationException("Block type \"" + typeInfo.FullName + "\" does not have static field Index of type int.");
					}
					int num = (int)fieldInfo.GetValue(null);
					Block block = (Block)Activator.CreateInstance(typeInfo.AsType());
					Dictionary<int, Block> dictionary2 = dictionary;
					int j = block.BlockIndex = num;
					dictionary2[j] = block;
					if (num > i)
					{
						i = num;
					}
				}
			}
			BlocksManager.m_blocks = new Block[1024];
			BlocksManager.m_fluidBlocks = new FluidBlock[i + 1];
			foreach (KeyValuePair<int, Block> keyValuePair in dictionary)
			{
				BlocksManager.m_blocks[keyValuePair.Key] = keyValuePair.Value;
				BlocksManager.m_fluidBlocks[keyValuePair.Key] = (keyValuePair.Value as FluidBlock);
			}
			for (i = 0; i < BlocksManager.m_blocks.Length; i++)
			{
				if (BlocksManager.m_blocks[i] == null)
				{
					BlocksManager.m_blocks[i] = BlocksManager.Blocks[0];
				}
			}
			string data = ContentManager.Get<string>("BlocksData");
			ContentManager.Dispose("BlocksData");
			BlocksManager.LoadBlocksData(data);
			foreach (FileEntry fileEntry in ModsManager.GetEntries(".csv"))
			{
				fileEntry.Stream.Position = 0L;
				BlocksManager.LoadBlocksData(new StreamReader(fileEntry.Stream).ReadToEnd());
			}
			Block[] blocks = BlocksManager.Blocks;
			for (int k = 0; k < blocks.Length; k++)
			{
				blocks[k].Initialize();
			}
			BlocksManager.m_categories.Add(LanguageControl.Get("BlocksManager", "Terrain"));
			BlocksManager.m_categories.Add(LanguageControl.Get("BlocksManager", "Plants"));
			BlocksManager.m_categories.Add(LanguageControl.Get("BlocksManager", "Construction"));
			BlocksManager.m_categories.Add(LanguageControl.Get("BlocksManager", "Items"));
			BlocksManager.m_categories.Add(LanguageControl.Get("BlocksManager", "Tools"));
			BlocksManager.m_categories.Add(LanguageControl.Get("BlocksManager", "Weapons"));
			BlocksManager.m_categories.Add(LanguageControl.Get("BlocksManager", "Clothes"));
			BlocksManager.m_categories.Add(LanguageControl.Get("BlocksManager", "Electrics"));
			BlocksManager.m_categories.Add(LanguageControl.Get("BlocksManager", "Food"));
			BlocksManager.m_categories.Add(LanguageControl.Get("BlocksManager", "Spawner Eggs"));
			BlocksManager.m_categories.Add(LanguageControl.Get("BlocksManager", "Painted"));
			BlocksManager.m_categories.Add(LanguageControl.Get("BlocksManager", "Dyed"));
			BlocksManager.m_categories.Add(LanguageControl.Get("BlocksManager", "Fireworks"));
			blocks = BlocksManager.Blocks;
			Action initialized = BlocksManager.Initialized;
			if (initialized != null)
			{
				initialized();
			}
			foreach (Block block2 in blocks)
			{
				foreach (int value in block2.GetCreativeValues())
				{
					string category = block2.GetCategory(value);
					string text = LanguageControl.Get("BlocksManager", category);
					if (string.IsNullOrEmpty(text))
					{
						if (!BlocksManager.m_categories.Contains(category))
						{
							BlocksManager.m_categories.Add(category);
						}
					}
					else if (!BlocksManager.m_categories.Contains(text))
					{
						BlocksManager.m_categories.Add(text);
					}
				}
			}
		}

		// Token: 0x06001868 RID: 6248 RVA: 0x000C198C File Offset: 0x000BFB8C
		public static Block FindBlockByTypeName(string typeName, bool throwIfNotFound)
		{
			Block block = BlocksManager.Blocks.FirstOrDefault((Block b) => b.GetType().Name == typeName);
			if (block == null && throwIfNotFound)
			{
				throw new InvalidOperationException(string.Format(LanguageControl.Get("BlocksManager", 1), typeName));
			}
			return block;
		}

		// Token: 0x06001869 RID: 6249 RVA: 0x000C19E0 File Offset: 0x000BFBE0
		public static Block[] FindBlocksByCraftingId(string craftingId)
		{
			return (from b in BlocksManager.Blocks
			where b.CraftingId == craftingId
			select b).ToArray<Block>();
		}

		// Token: 0x0600186A RID: 6250 RVA: 0x000C1A18 File Offset: 0x000BFC18
		public static void DrawCubeBlock(PrimitivesRenderer3D primitivesRenderer, int value, Vector3 size, ref Matrix matrix, Color color, Color topColor, DrawBlockEnvironmentData environmentData)
		{
			environmentData = (environmentData ?? BlocksManager.m_defaultEnvironmentData);
			Texture2D texture = (environmentData.SubsystemTerrain != null) ? environmentData.SubsystemTerrain.SubsystemAnimatedTextures.AnimatedBlocksTexture : BlocksTexturesManager.DefaultBlocksTexture;
			TexturedBatch3D texturedBatch3D = primitivesRenderer.TexturedBatch(texture, true, 0, null, RasterizerState.CullCounterClockwiseScissor, null, SamplerState.PointClamp);
			float s = LightingManager.LightIntensityByLightValue[environmentData.Light];
			color = Color.MultiplyColorOnly(color, s);
			topColor = Color.MultiplyColorOnly(topColor, s);
			Vector3 translation = matrix.Translation;
			Vector3 vector = matrix.Right * size.X;
			Vector3 v = matrix.Up * size.Y;
			Vector3 v2 = matrix.Forward * size.Z;
			Vector3 p = translation + 0.5f * (-vector - v - v2);
			Vector3 vector2 = translation + 0.5f * (vector - v - v2);
			Vector3 vector3 = translation + 0.5f * (-vector + v - v2);
			Vector3 vector4 = translation + 0.5f * (vector + v - v2);
			Vector3 vector5 = translation + 0.5f * (-vector - v + v2);
			Vector3 vector6 = translation + 0.5f * (vector - v + v2);
			Vector3 vector7 = translation + 0.5f * (-vector + v + v2);
			Vector3 p2 = translation + 0.5f * (vector + v + v2);
			if (environmentData.ViewProjectionMatrix != null)
			{
				Matrix value2 = environmentData.ViewProjectionMatrix.Value;
				Vector3.Transform(ref p, ref value2, out p);
				Vector3.Transform(ref vector2, ref value2, out vector2);
				Vector3.Transform(ref vector3, ref value2, out vector3);
				Vector3.Transform(ref vector4, ref value2, out vector4);
				Vector3.Transform(ref vector5, ref value2, out vector5);
				Vector3.Transform(ref vector6, ref value2, out vector6);
				Vector3.Transform(ref vector7, ref value2, out vector7);
				Vector3.Transform(ref p2, ref value2, out p2);
			}
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			Vector4 vector8 = BlocksManager.m_slotTexCoords[block.GetFaceTextureSlot(0, value)];
			Color color2 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(-matrix.Forward));
			texturedBatch3D.QueueQuad(p, vector3, vector4, vector2, new Vector2(vector8.X, vector8.W), new Vector2(vector8.X, vector8.Y), new Vector2(vector8.Z, vector8.Y), new Vector2(vector8.Z, vector8.W), color2);
			vector8 = BlocksManager.m_slotTexCoords[block.GetFaceTextureSlot(2, value)];
			color2 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(matrix.Forward));
			texturedBatch3D.QueueQuad(vector5, vector6, p2, vector7, new Vector2(vector8.Z, vector8.W), new Vector2(vector8.X, vector8.W), new Vector2(vector8.X, vector8.Y), new Vector2(vector8.Z, vector8.Y), color2);
			vector8 = BlocksManager.m_slotTexCoords[block.GetFaceTextureSlot(5, value)];
			color2 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(-matrix.Up));
			texturedBatch3D.QueueQuad(p, vector2, vector6, vector5, new Vector2(vector8.X, vector8.Y), new Vector2(vector8.Z, vector8.Y), new Vector2(vector8.Z, vector8.W), new Vector2(vector8.X, vector8.W), color2);
			vector8 = BlocksManager.m_slotTexCoords[block.GetFaceTextureSlot(4, value)];
			color2 = Color.MultiplyColorOnly(topColor, LightingManager.CalculateLighting(matrix.Up));
			texturedBatch3D.QueueQuad(vector3, vector7, p2, vector4, new Vector2(vector8.X, vector8.W), new Vector2(vector8.X, vector8.Y), new Vector2(vector8.Z, vector8.Y), new Vector2(vector8.Z, vector8.W), color2);
			vector8 = BlocksManager.m_slotTexCoords[block.GetFaceTextureSlot(1, value)];
			color2 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(-matrix.Right));
			texturedBatch3D.QueueQuad(p, vector5, vector7, vector3, new Vector2(vector8.Z, vector8.W), new Vector2(vector8.X, vector8.W), new Vector2(vector8.X, vector8.Y), new Vector2(vector8.Z, vector8.Y), color2);
			vector8 = BlocksManager.m_slotTexCoords[block.GetFaceTextureSlot(3, value)];
			color2 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(matrix.Right));
			texturedBatch3D.QueueQuad(vector2, vector4, p2, vector6, new Vector2(vector8.X, vector8.W), new Vector2(vector8.X, vector8.Y), new Vector2(vector8.Z, vector8.Y), new Vector2(vector8.Z, vector8.W), color2);
		}

		// Token: 0x0600186B RID: 6251 RVA: 0x000C1F90 File Offset: 0x000C0190
		public static void DrawFlatBlock(PrimitivesRenderer3D primitivesRenderer, int value, float size, ref Matrix matrix, Texture2D texture, Color color, bool isEmissive, DrawBlockEnvironmentData environmentData)
		{
			environmentData = (environmentData ?? BlocksManager.m_defaultEnvironmentData);
			if (!isEmissive)
			{
				float s = LightingManager.LightIntensityByLightValue[environmentData.Light];
				color = Color.MultiplyColorOnly(color, s);
			}
			Vector3 translation = matrix.Translation;
			Vector3 vector;
			Vector3 v;
			if (environmentData.BillboardDirection != null)
			{
				vector = Vector3.Normalize(Vector3.Cross(environmentData.BillboardDirection.Value, Vector3.UnitY));
				v = -Vector3.Normalize(Vector3.Cross(environmentData.BillboardDirection.Value, vector));
			}
			else
			{
				vector = matrix.Right;
				v = matrix.Up;
			}
			Vector3 p = translation + 0.85f * size * (-vector - v);
			Vector3 vector2 = translation + 0.85f * size * (vector - v);
			Vector3 vector3 = translation + 0.85f * size * (-vector + v);
			Vector3 p2 = translation + 0.85f * size * (vector + v);
			if (environmentData.ViewProjectionMatrix != null)
			{
				Matrix value2 = environmentData.ViewProjectionMatrix.Value;
				Vector3.Transform(ref p, ref value2, out p);
				Vector3.Transform(ref vector2, ref value2, out vector2);
				Vector3.Transform(ref vector3, ref value2, out vector3);
				Vector3.Transform(ref p2, ref value2, out p2);
			}
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			Vector4 vector4;
			if (texture != null)
			{
				vector4 = new Vector4(0f, 0f, 1f, 1f);
			}
			else
			{
				texture = ((environmentData.SubsystemTerrain != null) ? environmentData.SubsystemTerrain.SubsystemAnimatedTextures.AnimatedBlocksTexture : BlocksTexturesManager.DefaultBlocksTexture);
				vector4 = BlocksManager.m_slotTexCoords[block.GetFaceTextureSlot(-1, value)];
			}
			TexturedBatch3D texturedBatch3D = primitivesRenderer.TexturedBatch(texture, true, 0, null, RasterizerState.CullCounterClockwiseScissor, null, SamplerState.PointClamp);
			texturedBatch3D.QueueQuad(p, vector3, p2, vector2, new Vector2(vector4.X, vector4.W), new Vector2(vector4.X, vector4.Y), new Vector2(vector4.Z, vector4.Y), new Vector2(vector4.Z, vector4.W), color);
			if (environmentData.BillboardDirection == null)
			{
				texturedBatch3D.QueueQuad(p, vector2, p2, vector3, new Vector2(vector4.X, vector4.W), new Vector2(vector4.Z, vector4.W), new Vector2(vector4.Z, vector4.Y), new Vector2(vector4.X, vector4.Y), color);
			}
		}

		// Token: 0x0600186C RID: 6252 RVA: 0x000C2228 File Offset: 0x000C0428
		public static void DrawMeshBlock(PrimitivesRenderer3D primitivesRenderer, BlockMesh blockMesh, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			environmentData = (environmentData ?? BlocksManager.m_defaultEnvironmentData);
			Texture2D texture = (environmentData.SubsystemTerrain != null) ? environmentData.SubsystemTerrain.SubsystemAnimatedTextures.AnimatedBlocksTexture : BlocksTexturesManager.DefaultBlocksTexture;
			BlocksManager.DrawMeshBlock(primitivesRenderer, blockMesh, texture, Color.White, size, ref matrix, environmentData);
		}

		// Token: 0x0600186D RID: 6253 RVA: 0x000C2278 File Offset: 0x000C0478
		public static void DrawMeshBlock(PrimitivesRenderer3D primitivesRenderer, BlockMesh blockMesh, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			environmentData = (environmentData ?? BlocksManager.m_defaultEnvironmentData);
			Texture2D texture = (environmentData.SubsystemTerrain != null) ? environmentData.SubsystemTerrain.SubsystemAnimatedTextures.AnimatedBlocksTexture : BlocksTexturesManager.DefaultBlocksTexture;
			BlocksManager.DrawMeshBlock(primitivesRenderer, blockMesh, texture, color, size, ref matrix, environmentData);
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x000C22C4 File Offset: 0x000C04C4
		public static void DrawMeshBlock(PrimitivesRenderer3D primitivesRenderer, BlockMesh blockMesh, Texture2D texture, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			environmentData = (environmentData ?? BlocksManager.m_defaultEnvironmentData);
			float num = LightingManager.LightIntensityByLightValue[environmentData.Light];
			Vector4 vector = new Vector4(color);
			vector.X *= num;
			vector.Y *= num;
			vector.Z *= num;
			bool flag = vector == Vector4.One;
			TexturedBatch3D texturedBatch3D = primitivesRenderer.TexturedBatch(texture, true, 0, null, RasterizerState.CullCounterClockwiseScissor, null, SamplerState.PointClamp);
			bool flag2 = false;
			Matrix matrix2 = (environmentData.ViewProjectionMatrix == null) ? matrix : (matrix * environmentData.ViewProjectionMatrix.Value);
			if (size != 1f)
			{
				matrix2 = Matrix.CreateScale(size) * matrix2;
			}
			if (matrix2.M14 != 0f || matrix2.M24 != 0f || matrix2.M34 != 0f || matrix2.M44 != 1f)
			{
				flag2 = true;
			}
			int count = blockMesh.Vertices.Count;
			BlockMeshVertex[] array = blockMesh.Vertices.Array;
			int count2 = blockMesh.Indices.Count;
			ushort[] array2 = blockMesh.Indices.Array;
			DynamicArray<VertexPositionColorTexture> triangleVertices = texturedBatch3D.TriangleVertices;
			int count3 = triangleVertices.Count;
			int count4 = triangleVertices.Count;
			triangleVertices.Count += count;
			for (int i = 0; i < count; i++)
			{
				BlockMeshVertex blockMeshVertex = array[i];
				if (flag2)
				{
					Vector4 vector2 = new Vector4(blockMeshVertex.Position, 1f);
					Vector4.Transform(ref vector2, ref matrix2, out vector2);
					float num2 = 1f / vector2.W;
					blockMeshVertex.Position = new Vector3(vector2.X * num2, vector2.Y * num2, vector2.Z * num2);
				}
				else
				{
					Vector3.Transform(ref blockMeshVertex.Position, ref matrix2, out blockMeshVertex.Position);
				}
				if (flag || blockMeshVertex.IsEmissive)
				{
					triangleVertices.Array[count4++] = new VertexPositionColorTexture(blockMeshVertex.Position, blockMeshVertex.Color, blockMeshVertex.TextureCoordinates);
				}
				else
				{
					Color color2 = new Color((byte)((float)blockMeshVertex.Color.R * vector.X), (byte)((float)blockMeshVertex.Color.G * vector.Y), (byte)((float)blockMeshVertex.Color.B * vector.Z), (byte)((float)blockMeshVertex.Color.A * vector.W));
					triangleVertices.Array[count4++] = new VertexPositionColorTexture(blockMeshVertex.Position, color2, blockMeshVertex.TextureCoordinates);
				}
			}
			DynamicArray<ushort> triangleIndices = texturedBatch3D.TriangleIndices;
			int count5 = triangleIndices.Count;
			triangleIndices.Count += count2;
			for (int j = 0; j < count2; j++)
			{
				triangleIndices.Array[count5++] = (ushort)(count3 + (int)array2[j]);
			}
		}

		// Token: 0x0600186F RID: 6255 RVA: 0x000C25C0 File Offset: 0x000C07C0
		public static int DamageItem(int value, int damageCount)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			if (block.Durability < 0)
			{
				return value;
			}
			int num2 = block.GetDamage(value) + damageCount;
			if (num2 <= block.Durability)
			{
				return block.SetDamage(value, num2);
			}
			return block.GetDamageDestructionValue(value);
		}

		// Token: 0x06001870 RID: 6256 RVA: 0x000C260C File Offset: 0x000C080C
		public static void LoadBlocksData(string data)
		{
			Dictionary<Block, bool> dictionary = new Dictionary<Block, bool>();
			data = data.Replace("\r", string.Empty);
			string[] array = data.Split(new char[]
			{
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = null;
			for (int i = 0; i < array.Length; i++)
			{
				string[] array3 = array[i].Split(new char[]
				{
					';'
				});
				if (i == 0)
				{
					array2 = new string[array3.Length - 1];
					Array.Copy(array3, 1, array2, 0, array3.Length - 1);
				}
				else
				{
					if (array3.Length != array2.Length + 1)
					{
						throw new InvalidOperationException(string.Format(LanguageControl.Get("BlocksManager", 2), (array3.Length != 0) ? array3[0] : LanguageControl.Get("Usual", "unknown")));
					}
					string typeName = array3[0];
					if (!string.IsNullOrEmpty(typeName))
					{
						Block block = BlocksManager.m_blocks.FirstOrDefault((Block v) => v.GetType().Name == typeName);
						if (block == null)
						{
							throw new InvalidOperationException(string.Format(LanguageControl.Get("BlocksManager", 3), typeName));
						}
						dictionary.Add(block, true);
						Dictionary<string, FieldInfo> dictionary2 = new Dictionary<string, FieldInfo>();
						foreach (FieldInfo fieldInfo in block.GetType().GetRuntimeFields())
						{
							if (fieldInfo.IsPublic && !fieldInfo.IsStatic)
							{
								dictionary2.Add(fieldInfo.Name, fieldInfo);
							}
						}
						for (int j = 1; j < array3.Length; j++)
						{
							string text = array2[j - 1];
							string text2 = array3[j];
							if (!string.IsNullOrEmpty(text2))
							{
								FieldInfo fieldInfo2;
								if (!dictionary2.TryGetValue(text, out fieldInfo2))
								{
									throw new InvalidOperationException(string.Format(LanguageControl.Get("BlocksManager", 5), text));
								}
								object value;
								if (text2.StartsWith("#"))
								{
									string refTypeName = text2.Substring(1);
									object obj;
									if (string.IsNullOrEmpty(refTypeName))
									{
										obj = block.BlockIndex;
									}
									else
									{
										Block block2 = BlocksManager.m_blocks.FirstOrDefault((Block v) => v.GetType().Name == refTypeName);
										if (block2 == null)
										{
											throw new InvalidOperationException(string.Format(LanguageControl.Get("BlocksManager", 6), refTypeName));
										}
										obj = block2.BlockIndex;
									}
									value = obj;
								}
								else
								{
									value = HumanReadableConverter.ConvertFromString(fieldInfo2.FieldType, text2);
								}
								fieldInfo2.SetValue(block, value);
							}
						}
					}
				}
			}
		}

		// Token: 0x06001871 RID: 6257 RVA: 0x000C28A0 File Offset: 0x000C0AA0
		public static void CalculateSlotTexCoordTables()
		{
			for (int i = 0; i < 256; i++)
			{
				BlocksManager.m_slotTexCoords[i] = BlocksManager.TextureSlotToTextureCoords(i);
			}
		}

		// Token: 0x06001872 RID: 6258 RVA: 0x000C28D0 File Offset: 0x000C0AD0
		public static Vector4 TextureSlotToTextureCoords(int slot)
		{
			int num = slot % 16;
			int num2 = slot / 16;
			float x = ((float)num + 0.001f) / 16f;
			float y = ((float)num2 + 0.001f) / 16f;
			float z = ((float)(num + 1) - 0.001f) / 16f;
			float w = ((float)(num2 + 1) - 0.001f) / 16f;
			return new Vector4(x, y, z, w);
		}

		// Token: 0x04001151 RID: 4433
		public static Block[] m_blocks;

		// Token: 0x04001152 RID: 4434
		public static FluidBlock[] m_fluidBlocks;

		// Token: 0x04001153 RID: 4435
		public static List<string> m_categories = new List<string>();

		// Token: 0x04001154 RID: 4436
		public static DrawBlockEnvironmentData m_defaultEnvironmentData = new DrawBlockEnvironmentData();

		// Token: 0x04001155 RID: 4437
		public static Vector4[] m_slotTexCoords = new Vector4[256];

		// Token: 0x04001156 RID: 4438
		public static Action Initialized;

		// Token: 0x04001157 RID: 4439
		public static Action Initialize1;

		// Token: 0x04001158 RID: 4440
		public static Func<IEnumerable<TypeInfo>> GetBlockTypes1;

		// Token: 0x0200050E RID: 1294
		public sealed class c
		{
			// Token: 0x06002110 RID: 8464 RVA: 0x000E62AD File Offset: 0x000E44AD
			public bool Initialize_b__11_0(FieldInfo fi)
			{
				return fi.Name == "Index" && fi.IsPublic && fi.IsStatic;
			}

			// Token: 0x040018BC RID: 6332
			public static readonly BlocksManager.c _ = new BlocksManager.c();

			// Token: 0x040018BD RID: 6333
			public static Func<FieldInfo, bool> __11_0;
		}
	}
}
