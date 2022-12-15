using System;
using Engine;

namespace Game
{
	// Token: 0x0200029B RID: 667
	public interface ITerrainContentsGenerator
	{
		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x0600136C RID: 4972
		int OceanLevel { get; }

		// Token: 0x0600136D RID: 4973
		Vector3 FindCoarseSpawnPosition();

		// Token: 0x0600136E RID: 4974
		float CalculateOceanShoreDistance(float x, float z);

		// Token: 0x0600136F RID: 4975
		float CalculateHeight(float x, float z);

		// Token: 0x06001370 RID: 4976
		int CalculateTemperature(float x, float z);

		// Token: 0x06001371 RID: 4977
		int CalculateHumidity(float x, float z);

		// Token: 0x06001372 RID: 4978
		float CalculateMountainRangeFactor(float x, float z);

		// Token: 0x06001373 RID: 4979
		void GenerateChunkContentsPass1(TerrainChunk chunk);

		// Token: 0x06001374 RID: 4980
		void GenerateChunkContentsPass2(TerrainChunk chunk);

		// Token: 0x06001375 RID: 4981
		void GenerateChunkContentsPass3(TerrainChunk chunk);

		// Token: 0x06001376 RID: 4982
		void GenerateChunkContentsPass4(TerrainChunk chunk);
	}
}
