using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;

namespace Game
{
	// Token: 0x02000162 RID: 354
	public class SubsystemBodies : Subsystem, IUpdateable
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060006F1 RID: 1777 RVA: 0x0002BDF0 File Offset: 0x00029FF0
		public Dictionary<ComponentBody, Point2>.KeyCollection Bodies
		{
			get
			{
				return this.m_areaByComponentBody.Keys;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060006F2 RID: 1778 RVA: 0x0002BDFD File Offset: 0x00029FFD
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x0002BE00 File Offset: 0x0002A000
		public void FindBodiesAroundPoint(Vector2 point, float radius, DynamicArray<ComponentBody> result)
		{
			int num = (int)MathUtils.Floor((point.X - radius) / 8f);
			int num2 = (int)MathUtils.Floor((point.Y - radius) / 8f);
			int num3 = (int)MathUtils.Floor((point.X + radius) / 8f);
			int num4 = (int)MathUtils.Floor((point.Y + radius) / 8f);
			for (int i = num; i <= num3; i++)
			{
				for (int j = num2; j <= num4; j++)
				{
					DynamicArray<ComponentBody> dynamicArray;
					if (this.m_componentBodiesByArea.TryGetValue(new Point2(i, j), out dynamicArray))
					{
						for (int k = 0; k < dynamicArray.Count; k++)
						{
							result.Add(dynamicArray.Array[k]);
						}
					}
				}
			}
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x0002BEB8 File Offset: 0x0002A0B8
		public void FindBodiesInArea(Vector2 corner1, Vector2 corner2, DynamicArray<ComponentBody> result)
		{
			Point2 point = new Point2((int)MathUtils.Floor(corner1.X / 8f), (int)MathUtils.Floor(corner1.Y / 8f));
			Point2 point2 = new Point2((int)MathUtils.Floor(corner2.X / 8f), (int)MathUtils.Floor(corner2.Y / 8f));
			int num = MathUtils.Min(point.X, point2.X) - 1;
			int num2 = MathUtils.Min(point.Y, point2.Y) - 1;
			int num3 = MathUtils.Max(point.X, point2.X) + 1;
			int num4 = MathUtils.Max(point.Y, point2.Y) + 1;
			for (int i = num; i <= num3; i++)
			{
				for (int j = num2; j <= num4; j++)
				{
					DynamicArray<ComponentBody> dynamicArray;
					if (this.m_componentBodiesByArea.TryGetValue(new Point2(i, j), out dynamicArray))
					{
						for (int k = 0; k < dynamicArray.Count; k++)
						{
							result.Add(dynamicArray.Array[k]);
						}
					}
				}
			}
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x0002BFC8 File Offset: 0x0002A1C8
		public BodyRaycastResult? Raycast(Vector3 start, Vector3 end, float inflateAmount, Func<ComponentBody, float, bool> action)
		{
			float num = Vector3.Distance(start, end);
			Ray3 ray = new Ray3(start, (num > 0f) ? ((end - start) / num) : Vector3.UnitX);
			Vector2 corner = new Vector2(start.X, start.Z);
			Vector2 corner2 = new Vector2(end.X, end.Z);
			BodyRaycastResult bodyRaycastResult = new BodyRaycastResult
			{
				Ray = ray,
				Distance = float.MaxValue
			};
			this.m_componentBodies.Clear();
			this.FindBodiesInArea(corner, corner2, this.m_componentBodies);
			for (int i = 0; i < this.m_componentBodies.Count; i++)
			{
				ComponentBody componentBody = this.m_componentBodies.Array[i];
				float? num2;
				if (inflateAmount > 0f)
				{
					BoundingBox boundingBox = componentBody.BoundingBox;
					boundingBox.Min -= new Vector3(inflateAmount);
					boundingBox.Max += new Vector3(inflateAmount);
					num2 = ray.Intersection(boundingBox);
				}
				else
				{
					num2 = ray.Intersection(componentBody.BoundingBox);
				}
				if (num2 != null && num2.Value <= num && num2.Value < bodyRaycastResult.Distance && action(componentBody, num2.Value))
				{
					bodyRaycastResult.Distance = num2.Value;
					bodyRaycastResult.ComponentBody = componentBody;
				}
			}
			if (bodyRaycastResult.ComponentBody == null)
			{
				return null;
			}
			return new BodyRaycastResult?(bodyRaycastResult);
		}

        // Token: 0x060006F6 RID: 1782 RVA: 0x0002C160 File Offset: 0x0002A360
        public override void OnEntityAdded(Entity entity)
		{
			foreach (ComponentBody componentBody in entity.FindComponents<ComponentBody>())
			{
				this.AddBody(componentBody);
			}
		}

        // Token: 0x060006F7 RID: 1783 RVA: 0x0002C1B8 File Offset: 0x0002A3B8
        public override void OnEntityRemoved(Entity entity)
		{
			foreach (ComponentBody componentBody in entity.FindComponents<ComponentBody>())
			{
				this.RemoveBody(componentBody);
			}
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x0002C210 File Offset: 0x0002A410
		public void Update(float dt)
		{
			foreach (ComponentBody componentBody in this.Bodies)
			{
				this.UpdateBody(componentBody);
			}
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x0002C264 File Offset: 0x0002A464
		public void AddBody(ComponentBody componentBody)
		{
			Vector3 position = componentBody.Position;
			Point2 point = new Point2((int)MathUtils.Floor(position.X / 8f), (int)MathUtils.Floor(position.Z / 8f));
			this.m_areaByComponentBody.Add(componentBody, point);
			DynamicArray<ComponentBody> dynamicArray;
			if (!this.m_componentBodiesByArea.TryGetValue(point, out dynamicArray))
			{
				dynamicArray = new DynamicArray<ComponentBody>();
				this.m_componentBodiesByArea.Add(point, dynamicArray);
			}
			dynamicArray.Add(componentBody);
			componentBody.PositionChanged += this.ComponentBody_PositionChanged;
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x0002C2EC File Offset: 0x0002A4EC
		public void RemoveBody(ComponentBody componentBody)
		{
			Point2 key = this.m_areaByComponentBody[componentBody];
			this.m_areaByComponentBody.Remove(componentBody);
			this.m_componentBodiesByArea[key].Remove(componentBody);
			componentBody.PositionChanged -= this.ComponentBody_PositionChanged;
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x0002C338 File Offset: 0x0002A538
		public void UpdateBody(ComponentBody componentBody)
		{
			Vector3 position = componentBody.Position;
			Point2 point = new Point2((int)MathUtils.Floor(position.X / 8f), (int)MathUtils.Floor(position.Z / 8f));
			Point2 point2 = this.m_areaByComponentBody[componentBody];
			if (point != point2)
			{
				this.m_areaByComponentBody[componentBody] = point;
				this.m_componentBodiesByArea[point2].Remove(componentBody);
				DynamicArray<ComponentBody> dynamicArray;
				if (!this.m_componentBodiesByArea.TryGetValue(point, out dynamicArray))
				{
					dynamicArray = new DynamicArray<ComponentBody>();
					this.m_componentBodiesByArea.Add(point, dynamicArray);
				}
				dynamicArray.Add(componentBody);
			}
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x0002C3D7 File Offset: 0x0002A5D7
		public void ComponentBody_PositionChanged(ComponentFrame componentFrame)
		{
			this.UpdateBody((ComponentBody)componentFrame);
		}

		// Token: 0x040003DC RID: 988
		public const float m_areaSize = 8f;

		// Token: 0x040003DD RID: 989
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x040003DE RID: 990
		public Dictionary<ComponentBody, Point2> m_areaByComponentBody = new Dictionary<ComponentBody, Point2>();

		// Token: 0x040003DF RID: 991
		public Dictionary<Point2, DynamicArray<ComponentBody>> m_componentBodiesByArea = new Dictionary<Point2, DynamicArray<ComponentBody>>();
	}
}
