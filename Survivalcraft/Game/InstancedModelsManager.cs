using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000298 RID: 664
	public static class InstancedModelsManager
	{
		// Token: 0x0600135B RID: 4955 RVA: 0x00096C41 File Offset: 0x00094E41
		static InstancedModelsManager()
		{
			Display.DeviceReset += delegate()
			{
				foreach (InstancedModelData instancedModelData in InstancedModelsManager.m_cache.Values)
				{
					instancedModelData.VertexBuffer.Dispose();
					instancedModelData.IndexBuffer.Dispose();
				}
				InstancedModelsManager.m_cache.Clear();
			};
		}

		// Token: 0x0600135C RID: 4956 RVA: 0x00096C64 File Offset: 0x00094E64
		public static InstancedModelData GetInstancedModelData(Model model, int[] meshDrawOrders)
		{
			InstancedModelData instancedModelData;
			if (!InstancedModelsManager.m_cache.TryGetValue(model, out instancedModelData))
			{
				instancedModelData = InstancedModelsManager.CreateInstancedModelData(model, meshDrawOrders);
				InstancedModelsManager.m_cache.Add(model, instancedModelData);
			}
			return instancedModelData;
		}

		// Token: 0x0600135D RID: 4957 RVA: 0x00096C98 File Offset: 0x00094E98
		public static InstancedModelData CreateInstancedModelData(Model model, int[] meshDrawOrders)
		{
			DynamicArray<InstancedModelsManager.InstancedVertex> dynamicArray = new DynamicArray<InstancedModelsManager.InstancedVertex>();
			DynamicArray<ushort> dynamicArray2 = new DynamicArray<ushort>();
			for (int i = 0; i < meshDrawOrders.Length; i++)
			{
				ModelMesh modelMesh = model.Meshes[meshDrawOrders[i]];
				foreach (ModelMeshPart modelMeshPart in modelMesh.MeshParts)
				{
					int count = dynamicArray.Count;
					VertexBuffer vertexBuffer = modelMeshPart.VertexBuffer;
					IndexBuffer indexBuffer = modelMeshPart.IndexBuffer;
					ReadOnlyList<VertexElement> vertexElements = vertexBuffer.VertexDeclaration.VertexElements;
					ushort[] indexData = BlockMesh.GetIndexData<ushort>(indexBuffer);
					Dictionary<ushort, ushort> dictionary = new Dictionary<ushort, ushort>();
					if (vertexElements.Count != 3 || vertexElements[0].Offset != 0 || !(vertexElements[0].Semantic == VertexElementSemantic.Position.GetSemanticString()) || vertexElements[1].Offset != 12 || !(vertexElements[1].Semantic == VertexElementSemantic.Normal.GetSemanticString()) || vertexElements[2].Offset != 24 || !(vertexElements[2].Semantic == VertexElementSemantic.TextureCoordinate.GetSemanticString()))
					{
						throw new InvalidOperationException("Unsupported vertex format.");
					}
					InstancedModelsManager.SourceModelVertex[] vertexData = BlockMesh.GetVertexData<InstancedModelsManager.SourceModelVertex>(vertexBuffer);
					for (int j = modelMeshPart.StartIndex; j < modelMeshPart.StartIndex + modelMeshPart.IndicesCount; j++)
					{
						ushort num = indexData[j];
						if (!dictionary.ContainsKey(num))
						{
							dictionary.Add(num, (ushort)dynamicArray.Count);
							InstancedModelsManager.InstancedVertex item = default(InstancedModelsManager.InstancedVertex);
							InstancedModelsManager.SourceModelVertex sourceModelVertex = vertexData[(int)num];
							item.X = sourceModelVertex.X;
							item.Y = sourceModelVertex.Y;
							item.Z = sourceModelVertex.Z;
							item.Nx = sourceModelVertex.Nx;
							item.Ny = sourceModelVertex.Ny;
							item.Nz = sourceModelVertex.Nz;
							item.Tx = sourceModelVertex.Tx;
							item.Ty = sourceModelVertex.Ty;
							item.Instance = (float)modelMesh.ParentBone.Index;
							dynamicArray.Add(item);
						}
					}
					for (int k = 0; k < modelMeshPart.IndicesCount / 3; k++)
					{
						dynamicArray2.Add(dictionary[indexData[modelMeshPart.StartIndex + 3 * k]]);
						dynamicArray2.Add(dictionary[indexData[modelMeshPart.StartIndex + 3 * k + 1]]);
						dynamicArray2.Add(dictionary[indexData[modelMeshPart.StartIndex + 3 * k + 2]]);
					}
				}
			}
			InstancedModelData instancedModelData = new InstancedModelData();
			instancedModelData.VertexBuffer = new VertexBuffer(InstancedModelData.VertexDeclaration, dynamicArray.Count);
			instancedModelData.IndexBuffer = new IndexBuffer(IndexFormat.SixteenBits, dynamicArray2.Count);
			instancedModelData.VertexBuffer.SetData<InstancedModelsManager.InstancedVertex>(dynamicArray.Array, 0, dynamicArray.Count, 0);
			instancedModelData.IndexBuffer.SetData<ushort>(dynamicArray2.Array, 0, dynamicArray2.Count, 0);
			return instancedModelData;
		}

		// Token: 0x04000D46 RID: 3398
		public static Dictionary<Model, InstancedModelData> m_cache = new Dictionary<Model, InstancedModelData>();

		// Token: 0x020004AD RID: 1197
		public struct SourceModelVertex
		{
			// Token: 0x04001755 RID: 5973
			public float X;

			// Token: 0x04001756 RID: 5974
			public float Y;

			// Token: 0x04001757 RID: 5975
			public float Z;

			// Token: 0x04001758 RID: 5976
			public float Nx;

			// Token: 0x04001759 RID: 5977
			public float Ny;

			// Token: 0x0400175A RID: 5978
			public float Nz;

			// Token: 0x0400175B RID: 5979
			public float Tx;

			// Token: 0x0400175C RID: 5980
			public float Ty;
		}

		// Token: 0x020004AE RID: 1198
		public struct InstancedVertex
		{
			// Token: 0x0400175D RID: 5981
			public float X;

			// Token: 0x0400175E RID: 5982
			public float Y;

			// Token: 0x0400175F RID: 5983
			public float Z;

			// Token: 0x04001760 RID: 5984
			public float Nx;

			// Token: 0x04001761 RID: 5985
			public float Ny;

			// Token: 0x04001762 RID: 5986
			public float Nz;

			// Token: 0x04001763 RID: 5987
			public float Tx;

			// Token: 0x04001764 RID: 5988
			public float Ty;

			// Token: 0x04001765 RID: 5989
			public float Instance;
		}
	}
}
