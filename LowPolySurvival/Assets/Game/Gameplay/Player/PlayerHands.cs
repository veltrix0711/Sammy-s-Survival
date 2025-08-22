using UnityEngine;

namespace LowPolySurvival.Game.Gameplay.Player
{
	public sealed class PlayerHands : MonoBehaviour
	{
		[SerializeField] private Transform holdPoint;
		[SerializeField] private float holdDistance = 0.8f;
		[SerializeField] private GameObject carried;

		private void Awake()
		{
			if (holdPoint == null)
			{
				var t = new GameObject("HoldPoint").transform;
				t.SetParent(transform);
				t.localPosition = new Vector3(0, 1.2f, 0.5f);
				t.localRotation = Quaternion.identity;
				holdPoint = t;
			}
		}

		public bool IsCarrying => carried != null;

		public bool TryPickup(GameObject target)
		{
			if (target == null || carried != null) return false;
			carried = target;
			var rb = carried.GetComponent<Rigidbody>();
			if (rb != null) rb.isKinematic = true;
			foreach (var c in carried.GetComponentsInChildren<Collider>()) c.enabled = false;
			carried.transform.SetParent(holdPoint, worldPositionFalse: false);
			carried.transform.localPosition = new Vector3(0, 0, holdDistance);
			carried.transform.localRotation = Quaternion.identity;
			return true;
		}

		public bool Drop()
		{
			if (carried == null) return false;
			var rb = carried.GetComponent<Rigidbody>();
			carried.transform.SetParent(null, true);
			foreach (var c in carried.GetComponentsInChildren<Collider>()) c.enabled = true;
			if (rb != null) rb.isKinematic = false;
			carried = null;
			return true;
		}
	}
}


