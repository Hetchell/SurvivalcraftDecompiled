using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
	// Token: 0x02000228 RID: 552
	public class BlockMesh
	{
		// Token: 0x06001105 RID: 4357 RVA: 0x00084254 File Offset: 0x00082454
		public BoundingBox CalculateBoundingBox()
		{
			return new BoundingBox(from v in this.Vertices
			select v.Position);
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x00084288 File Offset: 0x00082488
		public BoundingBox CalculateBoundingBox(Matrix matrix)
		{
			return new BoundingBox(from v in this.Vertices
			select Vector3.Transform(v.Position, matrix));
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x000842BE File Offset: 0x000824BE
		public static Matrix GetBoneAbsoluteTransform(ModelBone modelBone)
		{
			if (modelBone.ParentBone != null)
			{
				return BlockMesh.GetBoneAbsoluteTransform(modelBone.ParentBone) * modelBone.Transform;
			}
			return modelBone.Transform;
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x000842E8 File Offset: 0x000824E8
		public void AppendImageExtrusion(Image image, Rectangle bounds, Vector3 size, Color color)
		{
			BlockMesh blockMesh = new BlockMesh();
			DynamicArray<BlockMeshVertex> vertices = blockMesh.Vertices;
			DynamicArray<ushort> indices = blockMesh.Indices;
			BlockMeshVertex item = new BlockMeshVertex
			{
				Position = new Vector3((float)bounds.Left, (float)bounds.Top, -1f),
				TextureCoordinates = new Vector2((float)bounds.Left, (float)bounds.Top)
			};
			vertices.Add(item);
			item = new BlockMeshVertex
			{
				Position = new Vector3((float)bounds.Right, (float)bounds.Top, -1f),
				TextureCoordinates = new Vector2((float)bounds.Right, (float)bounds.Top)
			};
			vertices.Add(item);
			item = new BlockMeshVertex
			{
				Position = new Vector3((float)bounds.Left, (float)bounds.Bottom, -1f),
				TextureCoordinates = new Vector2((float)bounds.Left, (float)bounds.Bottom)
			};
			vertices.Add(item);
			item = new BlockMeshVertex
			{
				Position = new Vector3((float)bounds.Right, (float)bounds.Bottom, -1f),
				TextureCoordinates = new Vector2((float)bounds.Right, (float)bounds.Bottom)
			};
			vertices.Add(item);
			indices.Add((ushort)(vertices.Count - 4));
			indices.Add((ushort)(vertices.Count - 1));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 1));
			indices.Add((ushort)(vertices.Count - 4));
			indices.Add((ushort)(vertices.Count - 2));
			item = new BlockMeshVertex
			{
				Position = new Vector3((float)bounds.Left, (float)bounds.Top, 1f),
				TextureCoordinates = new Vector2((float)bounds.Left, (float)bounds.Top)
			};
			vertices.Add(item);
			item = new BlockMeshVertex
			{
				Position = new Vector3((float)bounds.Right, (float)bounds.Top, 1f),
				TextureCoordinates = new Vector2((float)bounds.Right, (float)bounds.Top)
			};
			vertices.Add(item);
			item = new BlockMeshVertex
			{
				Position = new Vector3((float)bounds.Left, (float)bounds.Bottom, 1f),
				TextureCoordinates = new Vector2((float)bounds.Left, (float)bounds.Bottom)
			};
			vertices.Add(item);
			item = new BlockMeshVertex
			{
				Position = new Vector3((float)bounds.Right, (float)bounds.Bottom, 1f),
				TextureCoordinates = new Vector2((float)bounds.Right, (float)bounds.Bottom)
			};
			vertices.Add(item);
			indices.Add((ushort)(vertices.Count - 4));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 1));
			indices.Add((ushort)(vertices.Count - 1));
			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 4));
			for (int i = bounds.Left - 1; i <= bounds.Right; i++)
			{
				int num = -1;
				for (int j = bounds.Top - 1; j <= bounds.Bottom; j++)
				{
					bool flag = !bounds.Contains(new Point2(i, j)) || image.GetPixel(i, j) == Color.Transparent;
					bool flag2 = bounds.Contains(new Point2(i - 1, j)) && image.GetPixel(i - 1, j) != Color.Transparent;
					if (flag && flag2)
					{
						if (num < 0)
						{
							num = j;
						}
					}
					else if (num >= 0)
					{
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)i - 0.01f, (float)num - 0.01f, -1.01f),
							TextureCoordinates = new Vector2((float)(i - 1) + 0.01f, (float)num + 0.01f)
						};
						vertices.Add(item);
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)i - 0.01f, (float)num - 0.01f, 1.01f),
							TextureCoordinates = new Vector2((float)i - 0.01f, (float)num + 0.01f)
						};
						vertices.Add(item);
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)i - 0.01f, (float)j + 0.01f, -1.01f),
							TextureCoordinates = new Vector2((float)(i - 1) + 0.01f, (float)j - 0.01f)
						};
						vertices.Add(item);
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)i - 0.01f, (float)j + 0.01f, 1.01f),
							TextureCoordinates = new Vector2((float)i - 0.01f, (float)j - 0.01f)
						};
						vertices.Add(item);
						indices.Add((ushort)(vertices.Count - 4));
						indices.Add((ushort)(vertices.Count - 1));
						indices.Add((ushort)(vertices.Count - 3));
						indices.Add((ushort)(vertices.Count - 1));
						indices.Add((ushort)(vertices.Count - 4));
						indices.Add((ushort)(vertices.Count - 2));
						num = -1;
					}
				}
			}
			for (int k = bounds.Left - 1; k <= bounds.Right; k++)
			{
				int num2 = -1;
				for (int l = bounds.Top - 1; l <= bounds.Bottom; l++)
				{
					bool flag3 = !bounds.Contains(new Point2(k, l)) || image.GetPixel(k, l) == Color.Transparent;
					bool flag4 = bounds.Contains(new Point2(k + 1, l)) && image.GetPixel(k + 1, l) != Color.Transparent;
					if (flag3 && flag4)
					{
						if (num2 < 0)
						{
							num2 = l;
						}
					}
					else if (num2 >= 0)
					{
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)(k + 1) + 0.01f, (float)num2 - 0.01f, -1.01f),
							TextureCoordinates = new Vector2((float)(k + 1) + 0.01f, (float)num2 + 0.01f)
						};
						vertices.Add(item);
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)(k + 1) + 0.01f, (float)num2 - 0.01f, 1.01f),
							TextureCoordinates = new Vector2((float)(k + 2) - 0.01f, (float)num2 + 0.01f)
						};
						vertices.Add(item);
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)(k + 1) + 0.01f, (float)l + 0.01f, -1.01f),
							TextureCoordinates = new Vector2((float)(k + 1) + 0.01f, (float)l - 0.01f)
						};
						vertices.Add(item);
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)(k + 1) + 0.01f, (float)l + 0.01f, 1.01f),
							TextureCoordinates = new Vector2((float)(k + 2) - 0.01f, (float)l - 0.01f)
						};
						vertices.Add(item);
						indices.Add((ushort)(vertices.Count - 4));
						indices.Add((ushort)(vertices.Count - 3));
						indices.Add((ushort)(vertices.Count - 1));
						indices.Add((ushort)(vertices.Count - 1));
						indices.Add((ushort)(vertices.Count - 2));
						indices.Add((ushort)(vertices.Count - 4));
						num2 = -1;
					}
				}
			}
			for (int m = bounds.Top - 1; m <= bounds.Bottom; m++)
			{
				int num3 = -1;
				for (int n = bounds.Left - 1; n <= bounds.Right; n++)
				{
					bool flag5 = !bounds.Contains(new Point2(n, m)) || image.GetPixel(n, m) == Color.Transparent;
					bool flag6 = bounds.Contains(new Point2(n, m - 1)) && image.GetPixel(n, m - 1) != Color.Transparent;
					if (flag5 && flag6)
					{
						if (num3 < 0)
						{
							num3 = n;
						}
					}
					else if (num3 >= 0)
					{
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)num3 - 0.01f, (float)m - 0.01f, -1.01f),
							TextureCoordinates = new Vector2((float)num3 + 0.01f, (float)(m - 1) + 0.01f)
						};
						vertices.Add(item);
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)num3 - 0.01f, (float)m - 0.01f, 1.01f),
							TextureCoordinates = new Vector2((float)num3 + 0.01f, (float)m - 0.01f)
						};
						vertices.Add(item);
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)n + 0.01f, (float)m - 0.01f, -1.01f),
							TextureCoordinates = new Vector2((float)n - 0.01f, (float)(m - 1) + 0.01f)
						};
						vertices.Add(item);
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)n + 0.01f, (float)m - 0.01f, 1.01f),
							TextureCoordinates = new Vector2((float)n - 0.01f, (float)m - 0.01f)
						};
						vertices.Add(item);
						indices.Add((ushort)(vertices.Count - 4));
						indices.Add((ushort)(vertices.Count - 3));
						indices.Add((ushort)(vertices.Count - 1));
						indices.Add((ushort)(vertices.Count - 1));
						indices.Add((ushort)(vertices.Count - 2));
						indices.Add((ushort)(vertices.Count - 4));
						num3 = -1;
					}
				}
			}
			for (int num4 = bounds.Top - 1; num4 <= bounds.Bottom; num4++)
			{
				int num5 = -1;
				for (int num6 = bounds.Left - 1; num6 <= bounds.Right; num6++)
				{
					bool flag7 = !bounds.Contains(new Point2(num6, num4)) || image.GetPixel(num6, num4) == Color.Transparent;
					bool flag8 = bounds.Contains(new Point2(num6, num4 + 1)) && image.GetPixel(num6, num4 + 1) != Color.Transparent;
					if (flag7 && flag8)
					{
						if (num5 < 0)
						{
							num5 = num6;
						}
					}
					else if (num5 >= 0)
					{
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)num5 - 0.01f, (float)(num4 + 1) + 0.01f, -1.01f),
							TextureCoordinates = new Vector2((float)num5 + 0.01f, (float)(num4 + 1) + 0.01f)
						};
						vertices.Add(item);
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)num5 - 0.01f, (float)(num4 + 1) + 0.01f, 1.01f),
							TextureCoordinates = new Vector2((float)num5 + 0.01f, (float)(num4 + 2) - 0.01f)
						};
						vertices.Add(item);
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)num6 + 0.01f, (float)(num4 + 1) + 0.01f, -1.01f),
							TextureCoordinates = new Vector2((float)num6 - 0.01f, (float)(num4 + 1) + 0.01f)
						};
						vertices.Add(item);
						item = new BlockMeshVertex
						{
							Position = new Vector3((float)num6 + 0.01f, (float)(num4 + 1) + 0.01f, 1.01f),
							TextureCoordinates = new Vector2((float)num6 - 0.01f, (float)(num4 + 2) - 0.01f)
						};
						vertices.Add(item);
						indices.Add((ushort)(vertices.Count - 4));
						indices.Add((ushort)(vertices.Count - 1));
						indices.Add((ushort)(vertices.Count - 3));
						indices.Add((ushort)(vertices.Count - 1));
						indices.Add((ushort)(vertices.Count - 4));
						indices.Add((ushort)(vertices.Count - 2));
						num5 = -1;
					}
				}
			}
			for (int num7 = 0; num7 < vertices.Count; num7++)
			{
				BlockMeshVertex[] array = vertices.Array;
				int num8 = num7;
				array[num8].Position.X = array[num8].Position.X - ((float)bounds.Left + (float)bounds.Width / 2f);
				vertices.Array[num7].Position.Y = (float)bounds.Bottom - vertices.Array[num7].Position.Y - (float)bounds.Height / 2f;
				BlockMeshVertex[] array2 = vertices.Array;
				int num9 = num7;
				array2[num9].Position.X = array2[num9].Position.X * (size.X / (float)bounds.Width);
				BlockMeshVertex[] array3 = vertices.Array;
				int num10 = num7;
				array3[num10].Position.Y = array3[num10].Position.Y * (size.Y / (float)bounds.Height);
				BlockMeshVertex[] array4 = vertices.Array;
				int num11 = num7;
				array4[num11].Position.Z = array4[num11].Position.Z * (size.Z / 2f);
				BlockMeshVertex[] array5 = vertices.Array;
				int num12 = num7;
				array5[num12].TextureCoordinates.X = array5[num12].TextureCoordinates.X / (float)image.Width;
				BlockMeshVertex[] array6 = vertices.Array;
				int num13 = num7;
				array6[num13].TextureCoordinates.Y = array6[num13].TextureCoordinates.Y / (float)image.Height;
				vertices.Array[num7].Color = color;
			}
			this.AppendBlockMesh(blockMesh);
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x00085158 File Offset: 0x00083358
		public void AppendModelMeshPart(ModelMeshPart meshPart, Matrix matrix, bool makeEmissive, bool flipWindingOrder, bool doubleSided, bool flipNormals, Color color)
		{
			VertexBuffer vertexBuffer = meshPart.VertexBuffer;
			IndexBuffer indexBuffer = meshPart.IndexBuffer;
			ReadOnlyList<VertexElement> vertexElements = vertexBuffer.VertexDeclaration.VertexElements;
			if (vertexElements.Count != 3 || vertexElements[0].Offset != 0 || vertexElements[0].Semantic != VertexElementSemantic.Position.GetSemanticString() || vertexElements[1].Offset != 12 || vertexElements[1].Semantic != VertexElementSemantic.Normal.GetSemanticString() || vertexElements[2].Offset != 24 || vertexElements[2].Semantic != VertexElementSemantic.TextureCoordinate.GetSemanticString())
			{
				throw new InvalidOperationException("Wrong vertex format for a block mesh.");
			}
			BlockMesh.InternalVertex[] vertexData = BlockMesh.GetVertexData<BlockMesh.InternalVertex>(vertexBuffer);
			ushort[] indexData = BlockMesh.GetIndexData<ushort>(indexBuffer);
			Dictionary<ushort, ushort> dictionary = new Dictionary<ushort, ushort>();
			for (int i = meshPart.StartIndex; i < meshPart.StartIndex + meshPart.IndicesCount; i++)
			{
				ushort num = indexData[i];
				if (!dictionary.ContainsKey(num))
				{
					dictionary.Add(num, (ushort)this.Vertices.Count);
					BlockMeshVertex item = default(BlockMeshVertex);
					item.Position = Vector3.Transform(vertexData[(int)num].Position, matrix);
					item.TextureCoordinates = vertexData[(int)num].TextureCoordinate;
					Vector3 vector = Vector3.Normalize(Vector3.TransformNormal(flipNormals ? (-vertexData[(int)num].Normal) : vertexData[(int)num].Normal, matrix));
					if (makeEmissive)
					{
						item.IsEmissive = true;
						item.Color = color;
					}
					else
					{
						item.Color = color * LightingManager.CalculateLighting(vector);
						item.Color.A = color.A;
					}
					item.Face = (byte)CellFace.Vector3ToFace(vector, 5);
					this.Vertices.Add(item);
				}
			}
			for (int j = 0; j < meshPart.IndicesCount / 3; j++)
			{
				if (doubleSided)
				{
					this.Indices.Add(dictionary[indexData[meshPart.StartIndex + 3 * j]]);
					this.Indices.Add(dictionary[indexData[meshPart.StartIndex + 3 * j + 1]]);
					this.Indices.Add(dictionary[indexData[meshPart.StartIndex + 3 * j + 2]]);
					this.Indices.Add(dictionary[indexData[meshPart.StartIndex + 3 * j]]);
					this.Indices.Add(dictionary[indexData[meshPart.StartIndex + 3 * j + 2]]);
					this.Indices.Add(dictionary[indexData[meshPart.StartIndex + 3 * j + 1]]);
				}
				else if (flipWindingOrder)
				{
					this.Indices.Add(dictionary[indexData[meshPart.StartIndex + 3 * j]]);
					this.Indices.Add(dictionary[indexData[meshPart.StartIndex + 3 * j + 2]]);
					this.Indices.Add(dictionary[indexData[meshPart.StartIndex + 3 * j + 1]]);
				}
				else
				{
					this.Indices.Add(dictionary[indexData[meshPart.StartIndex + 3 * j]]);
					this.Indices.Add(dictionary[indexData[meshPart.StartIndex + 3 * j + 1]]);
					this.Indices.Add(dictionary[indexData[meshPart.StartIndex + 3 * j + 2]]);
				}
			}
			this.Trim();
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x000854F4 File Offset: 0x000836F4
		public void AppendBlockMesh(BlockMesh blockMesh)
		{
			int count = this.Vertices.Count;
			for (int i = 0; i < blockMesh.Vertices.Count; i++)
			{
				this.Vertices.Add(blockMesh.Vertices.Array[i]);
			}
			for (int j = 0; j < blockMesh.Indices.Count; j++)
			{
				this.Indices.Add((ushort)((int)blockMesh.Indices.Array[j] + count));
			}
			this.Trim();
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x00085578 File Offset: 0x00083778
		public void BlendBlockMesh(BlockMesh blockMesh, float factor)
		{
			if (blockMesh.Vertices.Count != this.Vertices.Count)
			{
				throw new InvalidOperationException("Meshes do not match.");
			}
			for (int i = 0; i < this.Vertices.Count; i++)
			{
				Vector3 position = this.Vertices.Array[i].Position;
				Vector3 position2 = blockMesh.Vertices.Array[i].Position;
				this.Vertices.Array[i].Position = Vector3.Lerp(position, position2, factor);
			}
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x0008560C File Offset: 0x0008380C
		public void TransformPositions(Matrix matrix, int facesMask = -1)
		{
			for (int i = 0; i < this.Vertices.Count; i++)
			{
				if ((1 << (int)this.Vertices.Array[i].Face & facesMask) != 0)
				{
					this.Vertices.Array[i].Position = Vector3.Transform(this.Vertices.Array[i].Position, matrix);
				}
			}
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x00085680 File Offset: 0x00083880
		public void TransformTextureCoordinates(Matrix matrix, int facesMask = -1)
		{
			for (int i = 0; i < this.Vertices.Count; i++)
			{
				if ((1 << (int)this.Vertices.Array[i].Face & facesMask) != 0)
				{
					this.Vertices.Array[i].TextureCoordinates = Vector2.Transform(this.Vertices.Array[i].TextureCoordinates, matrix);
				}
			}
		}

		// Token: 0x0600110E RID: 4366 RVA: 0x000856F4 File Offset: 0x000838F4
		public void SetColor(Color color, int facesMask = -1)
		{
			for (int i = 0; i < this.Vertices.Count; i++)
			{
				if ((1 << (int)this.Vertices.Array[i].Face & facesMask) != 0)
				{
					this.Vertices.Array[i].Color = color;
				}
			}
		}

		// Token: 0x0600110F RID: 4367 RVA: 0x00085750 File Offset: 0x00083950
		public void ModulateColor(Color color, int facesMask = -1)
		{
			for (int i = 0; i < this.Vertices.Count; i++)
			{
				if ((1 << (int)this.Vertices.Array[i].Face & facesMask) != 0)
				{
					BlockMeshVertex[] array = this.Vertices.Array;
					int num = i;
					array[num].Color = array[num].Color * color;
				}
			}
		}

		// Token: 0x06001110 RID: 4368 RVA: 0x000857BC File Offset: 0x000839BC
		public void GenerateSidesData()
		{
			this.Sides = new DynamicArray<sbyte>();
			this.Sides.Count = this.Indices.Count / 3;
			for (int i = 0; i < this.Sides.Count; i++)
			{
				int num = (int)this.Indices.Array[3 * i];
				int num2 = (int)this.Indices.Array[3 * i + 1];
				int num3 = (int)this.Indices.Array[3 * i + 2];
				Vector3 position = this.Vertices.Array[num].Position;
				Vector3 position2 = this.Vertices.Array[num2].Position;
				Vector3 position3 = this.Vertices.Array[num3].Position;
				if (BlockMesh.IsNear(position.Z, position2.Z, position3.Z, 1f))
				{
					this.Sides.Array[i] = 0;
				}
				else if (BlockMesh.IsNear(position.X, position2.X, position3.X, 1f))
				{
					this.Sides.Array[i] = 1;
				}
				else if (BlockMesh.IsNear(position.Z, position2.Z, position3.Z, 0f))
				{
					this.Sides.Array[i] = 2;
				}
				else if (BlockMesh.IsNear(position.X, position2.X, position3.X, 0f))
				{
					this.Sides.Array[i] = 3;
				}
				else if (BlockMesh.IsNear(position.Y, position2.Y, position3.Y, 1f))
				{
					this.Sides.Array[i] = 4;
				}
				else if (BlockMesh.IsNear(position.Y, position2.Y, position3.Y, 0f))
				{
					this.Sides.Array[i] = 5;
				}
				else
				{
					this.Sides.Array[i] = -1;
				}
			}
		}

		// Token: 0x06001111 RID: 4369 RVA: 0x000859C4 File Offset: 0x00083BC4
		public void Trim()
		{
			this.Vertices.Capacity = this.Vertices.Count;
			this.Indices.Capacity = this.Indices.Count;
			if (this.Sides != null)
			{
				this.Sides.Capacity = this.Sides.Count;
			}
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x00085A1C File Offset: 0x00083C1C
		public static T[] GetVertexData<T>(VertexBuffer vertexBuffer)
		{
			byte[] array = vertexBuffer.Tag as byte[];
			if (array == null)
			{
				throw new InvalidOperationException("VertexBuffer does not contain source data in Tag.");
			}
			if (array.Length % Utilities.SizeOf<T>() != 0)
			{
				throw new InvalidOperationException("VertexBuffer data size is not a whole multiply of target type size.");
			}
			T[] array2 = new T[array.Length / Utilities.SizeOf<T>()];
			GCHandle gchandle = GCHandle.Alloc(array2, GCHandleType.Pinned);
			T[] result;
			try
			{
				Marshal.Copy(array, 0, gchandle.AddrOfPinnedObject(), Utilities.SizeOf<T>() * array2.Length);
				result = array2;
			}
			finally
			{
				gchandle.Free();
			}
			return result;
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x00085AA4 File Offset: 0x00083CA4
		public static T[] GetIndexData<T>(IndexBuffer indexBuffer)
		{
			byte[] array = indexBuffer.Tag as byte[];
			if (array == null)
			{
				throw new InvalidOperationException("IndexBuffer does not contain source data in Tag.");
			}
			if (array.Length % Utilities.SizeOf<T>() != 0)
			{
				throw new InvalidOperationException("IndexBuffer data size is not a whole multiply of target type size.");
			}
			T[] array2 = new T[array.Length / Utilities.SizeOf<T>()];
			GCHandle gchandle = GCHandle.Alloc(array2, GCHandleType.Pinned);
			T[] result;
			try
			{
				Marshal.Copy(array, 0, gchandle.AddrOfPinnedObject(), Utilities.SizeOf<T>() * array2.Length);
				result = array2;
			}
			finally
			{
				gchandle.Free();
			}
			return result;
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x00085B2C File Offset: 0x00083D2C
		public static bool IsNear(float v1, float v2, float v3, float t)
		{
			return v1 - t >= -0.001f && v1 - t <= 0.001f && v2 - t >= -0.001f && v2 - t <= 0.001f && v3 - t >= -0.001f && v3 - t <= 0.001f;
		}

		// Token: 0x04000B66 RID: 2918
		public DynamicArray<BlockMeshVertex> Vertices = new DynamicArray<BlockMeshVertex>();

		// Token: 0x04000B67 RID: 2919
		public DynamicArray<ushort> Indices = new DynamicArray<ushort>();

		// Token: 0x04000B68 RID: 2920
		public DynamicArray<sbyte> Sides;

		// Token: 0x02000473 RID: 1139
		public struct InternalVertex
		{
			// Token: 0x04001693 RID: 5779
			public Vector3 Position;

			// Token: 0x04001694 RID: 5780
			public Vector3 Normal;

			// Token: 0x04001695 RID: 5781
			public Vector2 TextureCoordinate;
		}
	}
}
