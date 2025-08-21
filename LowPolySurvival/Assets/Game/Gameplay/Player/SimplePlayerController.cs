using UnityEngine;

namespace LowPolySurvival.Game.Gameplay.Player
{
	[RequireComponent(typeof(CharacterController))]
	[DefaultExecutionOrder(100)]
	public sealed class SimplePlayerController : MonoBehaviour
	{
		[SerializeField] private float moveSpeed = 4f;
		[SerializeField] private float lookSensitivity = 2f;
		[SerializeField] private float gravity = -9.81f;
		[SerializeField] private Camera playerCamera;

		private CharacterController controller;
		private float pitch;
		private Vector3 velocity;

		private void Awake()
		{
			controller = GetComponent<CharacterController>();
			if (controller.height < 1.5f) controller.height = 1.8f;
			if (controller.radius < 0.3f) controller.radius = 0.35f;
			if (Mathf.Abs(controller.center.y) < 0.01f) controller.center = new Vector3(0, 0.9f, 0);
			if (playerCamera == null)
			{
				var camGo = new GameObject("PlayerCamera");
				playerCamera = camGo.AddComponent<Camera>();
				playerCamera.tag = "MainCamera";
				camGo.transform.SetParent(transform);
				camGo.transform.localPosition = new Vector3(0, 1.6f, 0);
				camGo.transform.localRotation = Quaternion.identity;
			}
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update()
		{
			// Mouse look
			float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
			float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;
			transform.Rotate(Vector3.up * mouseX);
			pitch = Mathf.Clamp(pitch - mouseY, -85f, 85f);
			playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);

			// Movement
			float h = Input.GetAxisRaw("Horizontal");
			float v = Input.GetAxisRaw("Vertical");
			Vector3 input = new Vector3(h, 0, v);
			Vector3 world = transform.TransformDirection(input.normalized) * moveSpeed;
			velocity.x = world.x;
			velocity.z = world.z;
			velocity.y += gravity * Time.deltaTime;
			controller.Move(velocity * Time.deltaTime);
			if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
		}
	}
}
