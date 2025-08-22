using UnityEngine;
using LowPolySurvival.Game.Gameplay.UI;

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
		[SerializeField] private bool useLegacyInput = true; // set false if using new Input System only

		private CharacterController controller;
		private float pitch;
		private Vector3 velocity;
		private Vector3 groundNormal = Vector3.up;

		private void Awake()
		{
			controller = GetComponent<CharacterController>();
			if (controller.height < 1.5f) controller.height = 1.8f;
			if (controller.radius < 0.3f) controller.radius = 0.35f;
			if (Mathf.Abs(controller.center.y) < 0.01f) controller.center = new Vector3(0, 0.9f, 0);
			controller.slopeLimit = Mathf.Clamp(controller.slopeLimit, 60f, 89f);
			controller.stepOffset = Mathf.Max(controller.stepOffset, 0.45f);
			controller.skinWidth = Mathf.Max(controller.skinWidth, 0.08f);
			controller.minMoveDistance = 0f;
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
			Cursor.visible = false;
		}

		private void Update()
		{
			if (UIInputLock.IsLocked) return;
			// Mouse look
			float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
			float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;
			transform.Rotate(Vector3.up * mouseX);
			pitch = Mathf.Clamp(pitch - mouseY, -85f, 85f);
			playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);

			// Movement
			float h = 0f, v = 0f;
			if (useLegacyInput)
			{
				h = Input.GetAxisRaw("Horizontal");
				v = Input.GetAxisRaw("Vertical");
			}
			else
			{
#if ENABLE_INPUT_SYSTEM
				var kb = UnityEngine.InputSystem.Keyboard.current;
				if (kb != null)
				{
					if (kb.wKey.isPressed) v += 1f;
					if (kb.sKey.isPressed) v -= 1f;
					if (kb.dKey.isPressed) h += 1f;
					if (kb.aKey.isPressed) h -= 1f;
				}
#else
				// Fallback to legacy if new input system not enabled
				h = Input.GetAxisRaw("Horizontal");
				v = Input.GetAxisRaw("Vertical");
#endif
			}
			Vector3 input = new Vector3(h, 0, v);
			float inputMag = Mathf.Clamp01(input.magnitude);
			Vector3 wishDirWorld = transform.TransformDirection(input.normalized);
			Vector3 alongGround = Vector3.ProjectOnPlane(wishDirWorld, groundNormal).normalized;
			Vector3 planar = alongGround * (moveSpeed * inputMag);

			if (controller.isGrounded)
			{
				// SimpleMove applies gravity internally and handles ground sliding nicely
				controller.SimpleMove(planar);
				velocity.y = -2f; // keep grounded
			}
			else
			{
				velocity.x = planar.x;
				velocity.z = planar.z;
				velocity.y += gravity * Time.deltaTime;
				var move = velocity * Time.deltaTime;
				controller.Move(move);
			}
		}

		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (hit.normal.y > 0.1f)
			{
				groundNormal = hit.normal;
			}
		}
	}
}
