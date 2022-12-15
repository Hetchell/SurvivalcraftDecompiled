using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000176 RID: 374
	public class SubsystemElectricity : Subsystem, IUpdateable
	{
		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000816 RID: 2070 RVA: 0x00034B34 File Offset: 0x00032D34
		// (set) Token: 0x06000817 RID: 2071 RVA: 0x00034B3C File Offset: 0x00032D3C
		public SubsystemTime SubsystemTime { get; set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000818 RID: 2072 RVA: 0x00034B45 File Offset: 0x00032D45
		// (set) Token: 0x06000819 RID: 2073 RVA: 0x00034B4D File Offset: 0x00032D4D
		public SubsystemTerrain SubsystemTerrain { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600081A RID: 2074 RVA: 0x00034B56 File Offset: 0x00032D56
		// (set) Token: 0x0600081B RID: 2075 RVA: 0x00034B5E File Offset: 0x00032D5E
		public SubsystemAudio SubsystemAudio { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600081C RID: 2076 RVA: 0x00034B67 File Offset: 0x00032D67
		// (set) Token: 0x0600081D RID: 2077 RVA: 0x00034B6F File Offset: 0x00032D6F
		public int FrameStartCircuitStep { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600081E RID: 2078 RVA: 0x00034B78 File Offset: 0x00032D78
		// (set) Token: 0x0600081F RID: 2079 RVA: 0x00034B80 File Offset: 0x00032D80
		public int CircuitStep { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000820 RID: 2080 RVA: 0x00034B89 File Offset: 0x00032D89
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x00034B8C File Offset: 0x00032D8C
		public void OnElectricElementBlockGenerated(int x, int y, int z)
		{
			this.m_pointsToUpdate[new Point3(x, y, z)] = false;
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x00034BA2 File Offset: 0x00032DA2
		public void OnElectricElementBlockAdded(int x, int y, int z)
		{
			this.m_pointsToUpdate[new Point3(x, y, z)] = true;
		}

		// Token: 0x06000823 RID: 2083 RVA: 0x00034BB8 File Offset: 0x00032DB8
		public void OnElectricElementBlockRemoved(int x, int y, int z)
		{
			this.m_pointsToUpdate[new Point3(x, y, z)] = true;
		}

		// Token: 0x06000824 RID: 2084 RVA: 0x00034BCE File Offset: 0x00032DCE
		public void OnElectricElementBlockModified(int x, int y, int z)
		{
			this.m_pointsToUpdate[new Point3(x, y, z)] = true;
		}

		// Token: 0x06000825 RID: 2085 RVA: 0x00034BE4 File Offset: 0x00032DE4
		public void OnChunkDiscarding(TerrainChunk chunk)
		{
			foreach (CellFace cellFace in this.m_electricElementsByCellFace.Keys)
			{
				if (cellFace.X >= chunk.Origin.X && cellFace.X < chunk.Origin.X + 16 && cellFace.Z >= chunk.Origin.Y && cellFace.Z < chunk.Origin.Y + 16)
				{
					this.m_pointsToUpdate[new Point3(cellFace.X, cellFace.Y, cellFace.Z)] = false;
				}
			}
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x00034CB0 File Offset: 0x00032EB0
		public static ElectricConnectorDirection? GetConnectorDirection(int mountingFace, int rotation, int connectorFace)
		{
			ElectricConnectorDirection? result = SubsystemElectricity.m_connectorDirectionsTable[6 * mountingFace + connectorFace];
			if (result == null)
			{
				return null;
			}
			if (result.Value < ElectricConnectorDirection.In)
			{
				int u = (int)ElectricConnectorDirection.In;
				int t = (int)result.Value;
				return new ElectricConnectorDirection?((ElectricConnectorDirection)((t + rotation) % u));
			}
			return result;
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x00034CFC File Offset: 0x00032EFC
		public static int GetConnectorFace(int mountingFace, ElectricConnectorDirection connectionDirection)
		{
			return SubsystemElectricity.m_connectorFacesTable[(int)(5 * mountingFace + connectionDirection)];
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x00034D0C File Offset: 0x00032F0C
		public void GetAllConnectedNeighbors(int x, int y, int z, int mountingFace, DynamicArray<ElectricConnectionPath> list)
		{
			int cellValue = this.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			IElectricElementBlock electricElementBlock = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)] as IElectricElementBlock;
			if (electricElementBlock == null)
			{
				return;
			}
			for (ElectricConnectorDirection electricConnectorDirection = ElectricConnectorDirection.Top; electricConnectorDirection < (ElectricConnectorDirection)5; electricConnectorDirection++)
			{
				for (int i = 0; i < 4; i++)
				{
					ElectricConnectionPath electricConnectionPath = SubsystemElectricity.m_connectionPathsTable[(int)(20 * mountingFace + (int)(ElectricConnectorDirection.In) * (int)(electricConnectorDirection) + i)];
					if (electricConnectionPath == null)
					{
						break;
					}
					ElectricConnectorType? connectorType = electricElementBlock.GetConnectorType(this.SubsystemTerrain, cellValue, mountingFace, electricConnectionPath.ConnectorFace, x, y, z);
					if (connectorType == null)
					{
						break;
					}
					int x2 = x + electricConnectionPath.NeighborOffsetX;
					int y2 = y + electricConnectionPath.NeighborOffsetY;
					int z2 = z + electricConnectionPath.NeighborOffsetZ;
					int cellValue2 = this.SubsystemTerrain.Terrain.GetCellValue(x2, y2, z2);
					IElectricElementBlock electricElementBlock2 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)] as IElectricElementBlock;
					if (electricElementBlock2 != null)
					{
						ElectricConnectorType? connectorType2 = electricElementBlock2.GetConnectorType(this.SubsystemTerrain, cellValue2, electricConnectionPath.NeighborFace, electricConnectionPath.NeighborConnectorFace, x2, y2, z2);
						if (connectorType2 != null && ((connectorType.Value != ElectricConnectorType.Input && connectorType2.Value != ElectricConnectorType.Output) || (connectorType.Value != ElectricConnectorType.Output && connectorType2.Value != ElectricConnectorType.Input)))
						{
							bool connectionMask = electricElementBlock.GetConnectionMask(cellValue) != 0;
							int connectionMask2 = electricElementBlock2.GetConnectionMask(cellValue2);
							if (((connectionMask ? 1 : 0) & connectionMask2) != 0)
							{
								list.Add(electricConnectionPath);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x00034E70 File Offset: 0x00033070
		public ElectricElement GetElectricElement(int x, int y, int z, int mountingFace)
		{
			ElectricElement result;
			this.m_electricElementsByCellFace.TryGetValue(new CellFace(x, y, z, mountingFace), out result);
			return result;
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x00034E98 File Offset: 0x00033098
		public void QueueElectricElementForSimulation(ElectricElement electricElement, int circuitStep)
		{
			if (circuitStep == this.CircuitStep + 1)
			{
				if (this.m_nextStepSimulateList == null && !this.m_futureSimulateLists.TryGetValue(this.CircuitStep + 1, out this.m_nextStepSimulateList))
				{
					this.m_nextStepSimulateList = this.GetListFromCache();
					this.m_futureSimulateLists.Add(this.CircuitStep + 1, this.m_nextStepSimulateList);
				}
				this.m_nextStepSimulateList[electricElement] = true;
				return;
			}
			if (circuitStep > this.CircuitStep + 1)
			{
				Dictionary<ElectricElement, bool> listFromCache;
				if (!this.m_futureSimulateLists.TryGetValue(circuitStep, out listFromCache))
				{
					listFromCache = this.GetListFromCache();
					this.m_futureSimulateLists.Add(circuitStep, listFromCache);
				}
				listFromCache[electricElement] = true;
			}
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x00034F40 File Offset: 0x00033140
		public void QueueElectricElementConnectionsForSimulation(ElectricElement electricElement, int circuitStep)
		{
			foreach (ElectricConnection electricConnection in electricElement.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Input && electricConnection.NeighborConnectorType != ElectricConnectorType.Output)
				{
					this.QueueElectricElementForSimulation(electricConnection.NeighborElectricElement, circuitStep);
				}
			}
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x00034FAC File Offset: 0x000331AC
		public float? ReadPersistentVoltage(Point3 point)
		{
			float value;
			if (this.m_persistentElementsVoltages.TryGetValue(point, out value))
			{
				return new float?(value);
			}
			return null;
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x00034FD9 File Offset: 0x000331D9
		public void WritePersistentVoltage(Point3 point, float voltage)
		{
			this.m_persistentElementsVoltages[point] = voltage;
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x00034FE8 File Offset: 0x000331E8
		public void Update(float dt)
		{
			this.FrameStartCircuitStep = this.CircuitStep;
			SubsystemElectricity.SimulatedElectricElements = 0;
			this.m_remainingSimulationTime = MathUtils.Min(this.m_remainingSimulationTime + dt, 0.1f);
			while (this.m_remainingSimulationTime >= 0.01f)
			{
				this.UpdateElectricElements();
				int circuitStep = this.CircuitStep + 1;
				this.CircuitStep = circuitStep;
				this.m_remainingSimulationTime -= 0.01f;
				this.m_nextStepSimulateList = null;
				Dictionary<ElectricElement, bool> dictionary;
				if (this.m_futureSimulateLists.TryGetValue(this.CircuitStep, out dictionary))
				{
					this.m_futureSimulateLists.Remove(this.CircuitStep);
					SubsystemElectricity.SimulatedElectricElements += dictionary.Count;
					foreach (ElectricElement electricElement in dictionary.Keys)
					{
						if (this.m_electricElements.ContainsKey(electricElement))
						{
							this.SimulateElectricElement(electricElement);
						}
					}
					this.ReturnListToCache(dictionary);
				}
			}
			if (SubsystemElectricity.DebugDrawElectrics)
			{
				this.DebugDraw();
			}
		}

        // Token: 0x0600082F RID: 2095 RVA: 0x00035104 File Offset: 0x00033304
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.SubsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.SubsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.SubsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			string[] array = valuesDictionary.GetValue<string>("VoltagesByCell").Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new string[]
				{
					","
				}, StringSplitOptions.None);
				if (array2.Length != 4)
				{
					throw new InvalidOperationException("Invalid number of tokens.");
				}
				int x = int.Parse(array2[0], CultureInfo.InvariantCulture);
				int y = int.Parse(array2[1], CultureInfo.InvariantCulture);
				int z = int.Parse(array2[2], CultureInfo.InvariantCulture);
				float value = float.Parse(array2[3], CultureInfo.InvariantCulture);
				this.m_persistentElementsVoltages[new Point3(x, y, z)] = value;
			}
		}

        // Token: 0x06000830 RID: 2096 RVA: 0x000351F0 File Offset: 0x000333F0
        public override void Save(ValuesDictionary valuesDictionary)
		{
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<Point3, float> keyValuePair in this.m_persistentElementsVoltages)
			{
				if (num > 500)
				{
					break;
				}
				StringBuilder stringBuilder2 = stringBuilder;
				Point3 key = keyValuePair.Key;
				stringBuilder2.Append(key.X.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				StringBuilder stringBuilder3 = stringBuilder;
				key = keyValuePair.Key;
				stringBuilder3.Append(key.Y.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				StringBuilder stringBuilder4 = stringBuilder;
				key = keyValuePair.Key;
				stringBuilder4.Append(key.Z.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(',');
				stringBuilder.Append(keyValuePair.Value.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(';');
				num++;
			}
			valuesDictionary.SetValue<string>("VoltagesByCell", stringBuilder.ToString());
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x00035310 File Offset: 0x00033510
		public static ElectricConnectionPath GetConnectionPath(int mountingFace, ElectricConnectorDirection localConnector, int neighborIndex)
		{
			return SubsystemElectricity.m_connectionPathsTable[(int)(16 * mountingFace + (int)ElectricConnectorDirection.In * (int)localConnector + neighborIndex)];
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x00035322 File Offset: 0x00033522
		public void SimulateElectricElement(ElectricElement electricElement)
		{
			if (electricElement.Simulate())
			{
				this.QueueElectricElementConnectionsForSimulation(electricElement, this.CircuitStep + 1);
			}
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x0003533C File Offset: 0x0003353C
		public void AddElectricElement(ElectricElement electricElement)
		{
			this.m_electricElements.Add(electricElement, true);
			foreach (CellFace cellFace in electricElement.CellFaces)
			{
				this.m_electricElementsByCellFace.Add(cellFace, electricElement);
				this.m_tmpConnectionPaths.Clear();
				this.GetAllConnectedNeighbors(cellFace.X, cellFace.Y, cellFace.Z, cellFace.Face, this.m_tmpConnectionPaths);
				foreach (ElectricConnectionPath electricConnectionPath in this.m_tmpConnectionPaths)
				{
					CellFace cellFace2 = new CellFace(cellFace.X + electricConnectionPath.NeighborOffsetX, cellFace.Y + electricConnectionPath.NeighborOffsetY, cellFace.Z + electricConnectionPath.NeighborOffsetZ, electricConnectionPath.NeighborFace);
					ElectricElement electricElement2;
					if (this.m_electricElementsByCellFace.TryGetValue(cellFace2, out electricElement2) && electricElement2 != electricElement)
					{
						int cellValue = this.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
						int num = Terrain.ExtractContents(cellValue);
						ElectricConnectorType value = ((IElectricElementBlock)BlocksManager.Blocks[num]).GetConnectorType(this.SubsystemTerrain, cellValue, cellFace.Face, electricConnectionPath.ConnectorFace, cellFace.X, cellFace.Y, cellFace.Z).Value;
						int cellValue2 = this.SubsystemTerrain.Terrain.GetCellValue(cellFace2.X, cellFace2.Y, cellFace2.Z);
						int num2 = Terrain.ExtractContents(cellValue2);
						ElectricConnectorType value2 = ((IElectricElementBlock)BlocksManager.Blocks[num2]).GetConnectorType(this.SubsystemTerrain, cellValue2, cellFace2.Face, electricConnectionPath.NeighborConnectorFace, cellFace2.X, cellFace2.Y, cellFace2.Z).Value;
						electricElement.Connections.Add(new ElectricConnection
						{
							CellFace = cellFace,
							ConnectorFace = electricConnectionPath.ConnectorFace,
							ConnectorType = value,
							NeighborElectricElement = electricElement2,
							NeighborCellFace = cellFace2,
							NeighborConnectorFace = electricConnectionPath.NeighborConnectorFace,
							NeighborConnectorType = value2
						});
						electricElement2.Connections.Add(new ElectricConnection
						{
							CellFace = cellFace2,
							ConnectorFace = electricConnectionPath.NeighborConnectorFace,
							ConnectorType = value2,
							NeighborElectricElement = electricElement,
							NeighborCellFace = cellFace,
							NeighborConnectorFace = electricConnectionPath.ConnectorFace,
							NeighborConnectorType = value
						});
					}
				}
			}
			this.QueueElectricElementForSimulation(electricElement, this.CircuitStep + 1);
			this.QueueElectricElementConnectionsForSimulation(electricElement, this.CircuitStep + 2);
			electricElement.OnAdded();
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x00035638 File Offset: 0x00033838
		public void RemoveElectricElement(ElectricElement electricElement)
		{
			electricElement.OnRemoved();
			this.QueueElectricElementConnectionsForSimulation(electricElement, this.CircuitStep + 1);
			this.m_electricElements.Remove(electricElement);
			foreach (CellFace key in electricElement.CellFaces)
			{
				this.m_electricElementsByCellFace.Remove(key);
			}
			Func<ElectricConnection, bool> predicate0 = null;
			foreach (ElectricConnection electricConnection in electricElement.Connections)
			{
				IEnumerable<ElectricConnection> connections = electricConnection.NeighborElectricElement.Connections;
				Func<ElectricConnection, bool> predicate;
				if ((predicate = predicate0) == null)
				{
					predicate = (predicate0 = ((ElectricConnection c) => c.NeighborElectricElement == electricElement));
				}
				int num = connections.FirstIndex(predicate);
				if (num >= 0)
				{
					electricConnection.NeighborElectricElement.Connections.RemoveAt(num);
				}
			}
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x00035768 File Offset: 0x00033968
		public void UpdateElectricElements()
		{
			foreach (KeyValuePair<Point3, bool> keyValuePair in this.m_pointsToUpdate)
			{
				Point3 key = keyValuePair.Key;
				int cellValue = this.SubsystemTerrain.Terrain.GetCellValue(key.X, key.Y, key.Z);
				for (int i = 0; i < 6; i++)
				{
					ElectricElement electricElement = this.GetElectricElement(key.X, key.Y, key.Z, i);
					if (electricElement != null)
					{
						if (electricElement is WireDomainElectricElement)
						{
							this.m_wiresToUpdate[key] = true;
						}
						else
						{
							this.m_electricElementsToRemove[electricElement] = true;
						}
					}
				}
				if (keyValuePair.Value)
				{
					this.m_persistentElementsVoltages.Remove(key);
				}
				int num = Terrain.ExtractContents(cellValue);
				if (BlocksManager.Blocks[num] is IElectricWireElementBlock)
				{
					this.m_wiresToUpdate[key] = true;
				}
				else
				{
					IElectricElementBlock electricElementBlock = BlocksManager.Blocks[num] as IElectricElementBlock;
					if (electricElementBlock != null)
					{
						ElectricElement electricElement2 = electricElementBlock.CreateElectricElement(this, cellValue, key.X, key.Y, key.Z);
						if (electricElement2 != null)
						{
							this.m_electricElementsToAdd[key] = electricElement2;
						}
					}
				}
			}
			this.RemoveWireDomains();
			foreach (KeyValuePair<ElectricElement, bool> keyValuePair2 in this.m_electricElementsToRemove)
			{
				this.RemoveElectricElement(keyValuePair2.Key);
			}
			this.AddWireDomains();
			foreach (ElectricElement electricElement3 in this.m_electricElementsToAdd.Values)
			{
				this.AddElectricElement(electricElement3);
			}
			this.m_pointsToUpdate.Clear();
			this.m_wiresToUpdate.Clear();
			this.m_electricElementsToAdd.Clear();
			this.m_electricElementsToRemove.Clear();
		}

		// Token: 0x06000836 RID: 2102 RVA: 0x000359AC File Offset: 0x00033BAC
		public void AddWireDomains()
		{
			this.m_tmpVisited.Clear();
			foreach (Point3 point in this.m_wiresToUpdate.Keys)
			{
				for (int i = point.X - 1; i <= point.X + 1; i++)
				{
					for (int j = point.Y - 1; j <= point.Y + 1; j++)
					{
						for (int k = point.Z - 1; k <= point.Z + 1; k++)
						{
							for (int l = 0; l < 6; l++)
							{
								this.m_tmpResult.Clear();
								this.ScanWireDomain(new CellFace(i, j, k, l), this.m_tmpVisited, this.m_tmpResult);
								if (this.m_tmpResult.Count > 0)
								{
									WireDomainElectricElement electricElement = new WireDomainElectricElement(this, this.m_tmpResult.Keys);
									this.AddElectricElement(electricElement);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x00035AD0 File Offset: 0x00033CD0
		public void RemoveWireDomains()
		{
			foreach (Point3 point in this.m_wiresToUpdate.Keys)
			{
				for (int i = point.X - 1; i <= point.X + 1; i++)
				{
					for (int j = point.Y - 1; j <= point.Y + 1; j++)
					{
						for (int k = point.Z - 1; k <= point.Z + 1; k++)
						{
							for (int l = 0; l < 6; l++)
							{
								ElectricElement electricElement;
								if (this.m_electricElementsByCellFace.TryGetValue(new CellFace(i, j, k, l), out electricElement) && electricElement is WireDomainElectricElement)
								{
									this.RemoveElectricElement(electricElement);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x00035BB8 File Offset: 0x00033DB8
		public void ScanWireDomain(CellFace startCellFace, Dictionary<CellFace, bool> visited, Dictionary<CellFace, bool> result)
		{
			DynamicArray<CellFace> dynamicArray = new DynamicArray<CellFace>();
			dynamicArray.Add(startCellFace);
			while (dynamicArray.Count > 0)
			{
				CellFace[] array = dynamicArray.Array;
				DynamicArray<CellFace> dynamicArray2 = dynamicArray;
				int num = dynamicArray2.Count - 1;
				dynamicArray2.Count = num;
				CellFace cellFace = array[num];
				if (!visited.ContainsKey(cellFace))
				{
					TerrainChunk chunkAtCell = this.SubsystemTerrain.Terrain.GetChunkAtCell(cellFace.X, cellFace.Z);
					if (chunkAtCell != null && chunkAtCell.AreBehaviorsNotified)
					{
						int cellValue = this.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
						int num2 = Terrain.ExtractContents(cellValue);
						IElectricWireElementBlock electricWireElementBlock = BlocksManager.Blocks[num2] as IElectricWireElementBlock;
						if (electricWireElementBlock != null)
						{
							int connectedWireFacesMask = electricWireElementBlock.GetConnectedWireFacesMask(cellValue, cellFace.Face);
							if (connectedWireFacesMask != 0)
							{
								for (int i = 0; i < 6; i++)
								{
									if ((connectedWireFacesMask & 1 << i) != 0)
									{
										CellFace cellFace2 = new CellFace(cellFace.X, cellFace.Y, cellFace.Z, i);
										visited.Add(cellFace2, true);
										result.Add(cellFace2, true);
										this.m_tmpConnectionPaths.Clear();
										this.GetAllConnectedNeighbors(cellFace2.X, cellFace2.Y, cellFace2.Z, cellFace2.Face, this.m_tmpConnectionPaths);
										foreach (ElectricConnectionPath electricConnectionPath in this.m_tmpConnectionPaths)
										{
											int x = cellFace2.X + electricConnectionPath.NeighborOffsetX;
											int y = cellFace2.Y + electricConnectionPath.NeighborOffsetY;
											int z = cellFace2.Z + electricConnectionPath.NeighborOffsetZ;
											dynamicArray.Add(new CellFace(x, y, z, electricConnectionPath.NeighborFace));
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x00035DA8 File Offset: 0x00033FA8
		public Dictionary<ElectricElement, bool> GetListFromCache()
		{
			if (this.m_listsCache.Count > 0)
			{
				Dictionary<ElectricElement, bool> result = this.m_listsCache[this.m_listsCache.Count - 1];
				this.m_listsCache.RemoveAt(this.m_listsCache.Count - 1);
				return result;
			}
			return new Dictionary<ElectricElement, bool>();
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x00035DF9 File Offset: 0x00033FF9
		public void ReturnListToCache(Dictionary<ElectricElement, bool> list)
		{
			list.Clear();
			this.m_listsCache.Add(list);
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x00035E0D File Offset: 0x0003400D
		public void DebugDraw()
		{
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x00035EA8 File Offset: 0x000340A8
		// Note: this type is marked as 'beforefieldinit'.
		static SubsystemElectricity()
		{
			ElectricConnectionPath[] array = new ElectricConnectionPath[120];
			array[0] = new ElectricConnectionPath(0, 1, -1, 4, 4, 0);
			array[1] = new ElectricConnectionPath(0, 1, 0, 0, 4, 5);
			array[2] = new ElectricConnectionPath(0, 1, -1, 2, 4, 5);
			array[3] = new ElectricConnectionPath(0, 0, 0, 5, 4, 2);
			array[4] = new ElectricConnectionPath(-1, 0, -1, 3, 3, 0);
			array[5] = new ElectricConnectionPath(-1, 0, 0, 0, 3, 1);
			array[6] = new ElectricConnectionPath(-1, 0, -1, 2, 3, 1);
			array[7] = new ElectricConnectionPath(0, 0, 0, 1, 3, 2);
			array[8] = new ElectricConnectionPath(0, -1, -1, 5, 5, 0);
			array[9] = new ElectricConnectionPath(0, -1, 0, 0, 5, 4);
			array[10] = new ElectricConnectionPath(0, -1, -1, 2, 5, 4);
			array[11] = new ElectricConnectionPath(0, 0, 0, 4, 5, 2);
			array[12] = new ElectricConnectionPath(1, 0, -1, 1, 1, 0);
			array[13] = new ElectricConnectionPath(1, 0, 0, 0, 1, 3);
			array[14] = new ElectricConnectionPath(1, 0, -1, 2, 1, 3);
			array[15] = new ElectricConnectionPath(0, 0, 0, 3, 1, 2);
			array[16] = new ElectricConnectionPath(0, 0, -1, 2, 2, 0);
			array[20] = new ElectricConnectionPath(-1, 1, 0, 4, 4, 1);
			array[21] = new ElectricConnectionPath(0, 1, 0, 1, 4, 5);
			array[22] = new ElectricConnectionPath(-1, 1, 0, 3, 4, 5);
			array[23] = new ElectricConnectionPath(0, 0, 0, 5, 4, 3);
			array[24] = new ElectricConnectionPath(-1, 0, 1, 0, 0, 1);
			array[25] = new ElectricConnectionPath(0, 0, 1, 1, 0, 2);
			array[26] = new ElectricConnectionPath(-1, 0, 1, 3, 0, 2);
			array[27] = new ElectricConnectionPath(0, 0, 0, 2, 0, 3);
			array[28] = new ElectricConnectionPath(-1, -1, 0, 5, 5, 1);
			array[29] = new ElectricConnectionPath(0, -1, 0, 1, 5, 4);
			array[30] = new ElectricConnectionPath(-1, -1, 0, 3, 5, 4);
			array[31] = new ElectricConnectionPath(0, 0, 0, 4, 5, 3);
			array[32] = new ElectricConnectionPath(-1, 0, -1, 2, 2, 1);
			array[33] = new ElectricConnectionPath(0, 0, -1, 1, 2, 0);
			array[34] = new ElectricConnectionPath(-1, 0, -1, 3, 2, 0);
			array[35] = new ElectricConnectionPath(0, 0, 0, 0, 2, 3);
			array[36] = new ElectricConnectionPath(-1, 0, 0, 3, 3, 1);
			array[40] = new ElectricConnectionPath(0, 1, 1, 4, 4, 2);
			array[41] = new ElectricConnectionPath(0, 1, 0, 2, 4, 5);
			array[42] = new ElectricConnectionPath(0, 1, 1, 0, 4, 5);
			array[43] = new ElectricConnectionPath(0, 0, 0, 5, 4, 0);
			array[44] = new ElectricConnectionPath(1, 0, 1, 1, 1, 2);
			array[45] = new ElectricConnectionPath(1, 0, 0, 2, 1, 3);
			array[46] = new ElectricConnectionPath(1, 0, 1, 0, 1, 3);
			array[47] = new ElectricConnectionPath(0, 0, 0, 3, 1, 0);
			array[48] = new ElectricConnectionPath(0, -1, 1, 5, 5, 2);
			array[49] = new ElectricConnectionPath(0, -1, 0, 2, 5, 4);
			array[50] = new ElectricConnectionPath(0, -1, 1, 0, 5, 4);
			array[51] = new ElectricConnectionPath(0, 0, 0, 4, 5, 0);
			array[52] = new ElectricConnectionPath(-1, 0, 1, 3, 3, 2);
			array[53] = new ElectricConnectionPath(-1, 0, 0, 2, 3, 1);
			array[54] = new ElectricConnectionPath(-1, 0, 1, 0, 3, 1);
			array[55] = new ElectricConnectionPath(0, 0, 0, 1, 3, 0);
			array[56] = new ElectricConnectionPath(0, 0, 1, 0, 0, 2);
			array[60] = new ElectricConnectionPath(1, 1, 0, 4, 4, 3);
			array[61] = new ElectricConnectionPath(0, 1, 0, 3, 4, 5);
			array[62] = new ElectricConnectionPath(1, 1, 0, 1, 4, 5);
			array[63] = new ElectricConnectionPath(0, 0, 0, 5, 4, 1);
			array[64] = new ElectricConnectionPath(1, 0, -1, 2, 2, 3);
			array[65] = new ElectricConnectionPath(0, 0, -1, 3, 2, 0);
			array[66] = new ElectricConnectionPath(1, 0, -1, 1, 2, 0);
			array[67] = new ElectricConnectionPath(0, 0, 0, 0, 2, 1);
			array[68] = new ElectricConnectionPath(1, -1, 0, 5, 5, 3);
			array[69] = new ElectricConnectionPath(0, -1, 0, 3, 5, 4);
			array[70] = new ElectricConnectionPath(1, -1, 0, 1, 5, 4);
			array[71] = new ElectricConnectionPath(0, 0, 0, 4, 5, 1);
			array[72] = new ElectricConnectionPath(1, 0, 1, 0, 0, 3);
			array[73] = new ElectricConnectionPath(0, 0, 1, 3, 0, 2);
			array[74] = new ElectricConnectionPath(1, 0, 1, 1, 0, 2);
			array[75] = new ElectricConnectionPath(0, 0, 0, 2, 0, 1);
			array[76] = new ElectricConnectionPath(1, 0, 0, 1, 1, 3);
			array[80] = new ElectricConnectionPath(0, -1, -1, 2, 2, 4);
			array[81] = new ElectricConnectionPath(0, 0, -1, 4, 2, 0);
			array[82] = new ElectricConnectionPath(0, -1, -1, 5, 2, 0);
			array[83] = new ElectricConnectionPath(0, 0, 0, 0, 2, 5);
			array[84] = new ElectricConnectionPath(-1, -1, 0, 3, 3, 4);
			array[85] = new ElectricConnectionPath(-1, 0, 0, 4, 3, 1);
			array[86] = new ElectricConnectionPath(-1, -1, 0, 5, 3, 1);
			array[87] = new ElectricConnectionPath(0, 0, 0, 1, 3, 5);
			array[88] = new ElectricConnectionPath(0, -1, 1, 0, 0, 4);
			array[89] = new ElectricConnectionPath(0, 0, 1, 4, 0, 2);
			array[90] = new ElectricConnectionPath(0, -1, 1, 5, 0, 2);
			array[91] = new ElectricConnectionPath(0, 0, 0, 2, 0, 5);
			array[92] = new ElectricConnectionPath(1, -1, 0, 1, 1, 4);
			array[93] = new ElectricConnectionPath(1, 0, 0, 4, 1, 3);
			array[94] = new ElectricConnectionPath(1, -1, 0, 5, 1, 3);
			array[95] = new ElectricConnectionPath(0, 0, 0, 3, 1, 5);
			array[96] = new ElectricConnectionPath(0, -1, 0, 5, 5, 4);
			array[100] = new ElectricConnectionPath(0, 1, -1, 2, 2, 5);
			array[101] = new ElectricConnectionPath(0, 0, -1, 5, 2, 0);
			array[102] = new ElectricConnectionPath(0, 1, -1, 4, 2, 0);
			array[103] = new ElectricConnectionPath(0, 0, 0, 0, 2, 4);
			array[104] = new ElectricConnectionPath(1, 1, 0, 1, 1, 5);
			array[105] = new ElectricConnectionPath(1, 0, 0, 5, 1, 3);
			array[106] = new ElectricConnectionPath(1, 1, 0, 4, 1, 3);
			array[107] = new ElectricConnectionPath(0, 0, 0, 3, 1, 4);
			array[108] = new ElectricConnectionPath(0, 1, 1, 0, 0, 5);
			array[109] = new ElectricConnectionPath(0, 0, 1, 5, 0, 2);
			array[110] = new ElectricConnectionPath(0, 1, 1, 4, 0, 2);
			array[111] = new ElectricConnectionPath(0, 0, 0, 2, 0, 4);
			array[112] = new ElectricConnectionPath(-1, 1, 0, 3, 3, 5);
			array[113] = new ElectricConnectionPath(-1, 0, 0, 5, 3, 1);
			array[114] = new ElectricConnectionPath(-1, 1, 0, 4, 3, 1);
			array[115] = new ElectricConnectionPath(0, 0, 0, 1, 3, 4);
			array[116] = new ElectricConnectionPath(0, 1, 0, 4, 4, 5);
			SubsystemElectricity.m_connectionPathsTable = array;
			ElectricConnectorDirection?[] array2 = new ElectricConnectorDirection?[36];
			array2[1] = new ElectricConnectorDirection?(ElectricConnectorDirection.Right);
			array2[2] = new ElectricConnectorDirection?(ElectricConnectorDirection.In);
			array2[3] = new ElectricConnectorDirection?(ElectricConnectorDirection.Left);
			array2[4] = new ElectricConnectorDirection?(ElectricConnectorDirection.Top);
			array2[5] = new ElectricConnectorDirection?(ElectricConnectorDirection.Bottom);
			array2[6] = new ElectricConnectorDirection?(ElectricConnectorDirection.Left);
			array2[8] = new ElectricConnectorDirection?(ElectricConnectorDirection.Right);
			array2[9] = new ElectricConnectorDirection?(ElectricConnectorDirection.In);
			array2[10] = new ElectricConnectorDirection?(ElectricConnectorDirection.Top);
			array2[11] = new ElectricConnectorDirection?(ElectricConnectorDirection.Bottom);
			array2[12] = new ElectricConnectorDirection?(ElectricConnectorDirection.In);
			array2[13] = new ElectricConnectorDirection?(ElectricConnectorDirection.Left);
			array2[15] = new ElectricConnectorDirection?(ElectricConnectorDirection.Right);
			array2[16] = new ElectricConnectorDirection?(ElectricConnectorDirection.Top);
			array2[17] = new ElectricConnectorDirection?(ElectricConnectorDirection.Bottom);
			array2[18] = new ElectricConnectorDirection?(ElectricConnectorDirection.Right);
			array2[19] = new ElectricConnectorDirection?(ElectricConnectorDirection.In);
			array2[20] = new ElectricConnectorDirection?(ElectricConnectorDirection.Left);
			array2[22] = new ElectricConnectorDirection?(ElectricConnectorDirection.Top);
			array2[23] = new ElectricConnectorDirection?(ElectricConnectorDirection.Bottom);
			array2[24] = new ElectricConnectorDirection?(ElectricConnectorDirection.Bottom);
			array2[25] = new ElectricConnectorDirection?(ElectricConnectorDirection.Right);
			array2[26] = new ElectricConnectorDirection?(ElectricConnectorDirection.Top);
			array2[27] = new ElectricConnectorDirection?(ElectricConnectorDirection.Left);
			array2[29] = new ElectricConnectorDirection?(ElectricConnectorDirection.In);
			array2[30] = new ElectricConnectorDirection?(ElectricConnectorDirection.Top);
			array2[31] = new ElectricConnectorDirection?(ElectricConnectorDirection.Right);
			array2[32] = new ElectricConnectorDirection?(ElectricConnectorDirection.Bottom);
			array2[33] = new ElectricConnectorDirection?(ElectricConnectorDirection.Left);
			array2[34] = new ElectricConnectorDirection?(ElectricConnectorDirection.In);
			SubsystemElectricity.m_connectorDirectionsTable = array2;
			SubsystemElectricity.m_connectorFacesTable = new int[]
			{
				4,
				3,
				5,
				1,
				2,
				4,
				0,
				5,
				2,
				3,
				4,
				1,
				5,
				3,
				0,
				4,
				2,
				5,
				0,
				1,
				2,
				1,
				0,
				3,
				5,
				0,
				1,
				2,
				3,
				4
			};
			SubsystemElectricity.DebugDrawElectrics = false;
		}

		// Token: 0x04000436 RID: 1078
		public static ElectricConnectionPath[] m_connectionPathsTable;

		// Token: 0x04000437 RID: 1079
		public static ElectricConnectorDirection?[] m_connectorDirectionsTable;

		// Token: 0x04000438 RID: 1080
		public static int[] m_connectorFacesTable;

		// Token: 0x04000439 RID: 1081
		public float m_remainingSimulationTime;

		// Token: 0x0400043A RID: 1082
		public Dictionary<Point3, float> m_persistentElementsVoltages = new Dictionary<Point3, float>();

		// Token: 0x0400043B RID: 1083
		public Dictionary<ElectricElement, bool> m_electricElements = new Dictionary<ElectricElement, bool>();

		// Token: 0x0400043C RID: 1084
		public Dictionary<CellFace, ElectricElement> m_electricElementsByCellFace = new Dictionary<CellFace, ElectricElement>();

		// Token: 0x0400043D RID: 1085
		public Dictionary<Point3, bool> m_pointsToUpdate = new Dictionary<Point3, bool>();

		// Token: 0x0400043E RID: 1086
		public Dictionary<Point3, ElectricElement> m_electricElementsToAdd = new Dictionary<Point3, ElectricElement>();

		// Token: 0x0400043F RID: 1087
		public Dictionary<ElectricElement, bool> m_electricElementsToRemove = new Dictionary<ElectricElement, bool>();

		// Token: 0x04000440 RID: 1088
		public Dictionary<Point3, bool> m_wiresToUpdate = new Dictionary<Point3, bool>();

		// Token: 0x04000441 RID: 1089
		public List<Dictionary<ElectricElement, bool>> m_listsCache = new List<Dictionary<ElectricElement, bool>>();

		// Token: 0x04000442 RID: 1090
		public Dictionary<int, Dictionary<ElectricElement, bool>> m_futureSimulateLists = new Dictionary<int, Dictionary<ElectricElement, bool>>();

		// Token: 0x04000443 RID: 1091
		public Dictionary<ElectricElement, bool> m_nextStepSimulateList;

		// Token: 0x04000444 RID: 1092
		public DynamicArray<ElectricConnectionPath> m_tmpConnectionPaths = new DynamicArray<ElectricConnectionPath>();

		// Token: 0x04000445 RID: 1093
		public Dictionary<CellFace, bool> m_tmpVisited = new Dictionary<CellFace, bool>();

		// Token: 0x04000446 RID: 1094
		public Dictionary<CellFace, bool> m_tmpResult = new Dictionary<CellFace, bool>();

		// Token: 0x04000447 RID: 1095
		public static bool DebugDrawElectrics;

		// Token: 0x04000448 RID: 1096
		public static int SimulatedElectricElements;

		// Token: 0x04000449 RID: 1097
		public const float CircuitStepDuration = 0.01f;
	}
}
