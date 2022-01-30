using UnityEngine;

namespace Dogger.Spawning
{
	/// <summary>
	/// Collideable object that spawns in the traversable player area.
	/// </summary>
	[RequireComponent(typeof(BoxCollider))]
	public class Obstacle : Scroller
	{
		/// <summary>
		/// Global maximum constraint for the traversable player area in the x
		/// direction.
		/// </summary>
		public static float BoundXMax = 3.0f;

		/// <summary>
		/// Global minimum constraint for the traversable player area in the x
		/// direction.
		/// </summary>
		public static float BoundXMin = -3.0f;

		public BoxCollider ObstacleCollider;

		protected override void Awake()
		{
			if (ObstacleCollider != null)
			{
				ObstacleCollider.isTrigger = true;
			}
		}
	}
}