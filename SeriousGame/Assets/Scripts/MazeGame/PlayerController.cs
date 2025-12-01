using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	private CharacterController controller;
	private Transform cameraTransform;
	private PlayerInput playerInput;

	[Header("Look Settings")] [SerializeField]
	private float lookSensitivity = 2f;

	[SerializeField] private float lookClamp;

	[Header("Movement Settings")] [SerializeField]
	private float walkSpeed = 7.5f;

	[SerializeField] private float moveSmoothTime = 0.3f;

	private Vector3 velocity;
	private Vector3 movementDirection;
	private Vector2 currentDirection;
	private Vector2 currentVelocity;
	private Vector2 lookInput;
	private float lookXRotation;
	private bool playingSound;
	private AudioSource audioSource;

	public delegate void MoveInputEvent(Vector2 _direction);

	private event MoveInputEvent OnMove;

	public delegate void LookInputEvent(InputAction.CallbackContext _context);

	private event LookInputEvent OnLook;

	private void Awake()
	{
		controller = GetComponent<CharacterController>();
		cameraTransform = Camera.main.transform;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		playerInput = GetComponent<PlayerInput>();
		audioSource = GetComponent<AudioSource>();
		setInputActions();
	}

	private void OnEnable()
	{
		setInputActions();
	}

	private void OnDestroy()
	{
		removeInputActions();
	}

	private void setInputActions()
	{
		playerInput.actions["Move"].performed += _ctx => OnMove?.Invoke(_ctx.ReadValue<Vector2>());
		playerInput.actions["Move"].canceled += _ctx => OnMove?.Invoke(Vector2.zero);
		OnMove += movement;
		playerInput.actions["Look"].performed += _ctx => OnLook?.Invoke(_ctx);
		OnLook += look;
	}

	private void removeInputActions()
	{
		playerInput.actions["Move"].performed -= _ctx => OnMove?.Invoke(_ctx.ReadValue<Vector2>());
		playerInput.actions["Move"].canceled -= _ctx => OnMove?.Invoke(Vector2.zero);
		OnMove -= movement;
		playerInput.actions["Look"].performed -= _ctx => OnLook?.Invoke(_ctx);
		OnLook -= look;
	}

	private void Update()
	{
		if (controller.enabled) { controller.Move(velocity * Time.deltaTime); }
		setRotation();
		if (velocity.magnitude > 1 && !playingSound)
		{
			audioSource.Play();
			playingSound = true;
		}
		else if (velocity.magnitude < 1 && playingSound)
		{
			if (audioSource.time >= audioSource.clip.length - 0.1f)
			{
				playingSound = false;
				audioSource.Stop();
			}
		}
	}

	private void FixedUpdate()
	{
		move();
	}

	private void look(InputAction.CallbackContext _context)
	{
		lookInput = _context.ReadValue<Vector2>() * lookSensitivity;
	}

	private void setRotation()
	{
		if (lookInput.sqrMagnitude < 0.1f) { return; }
		Vector2 _lookInput = lookInput;
		transform.Rotate(Vector3.up, _lookInput.x);
		lookXRotation -= _lookInput.y;
		lookXRotation = Mathf.Clamp(lookXRotation, -lookClamp, lookClamp);
		cameraTransform.localRotation = Quaternion.Euler(lookXRotation, 0f, 0f);
	}

	private void movement(Vector2 _direction)
	{
		movementDirection = new Vector3(_direction.x, 0, _direction.y);
	}

	private void move()
	{
		currentDirection = Vector2.SmoothDamp(currentDirection, new(movementDirection.x, movementDirection.z), ref currentVelocity, moveSmoothTime);
		velocity = (transform.forward * currentDirection.y + transform.right * currentDirection.x) * walkSpeed + Vector3.up * velocity.y;
	}
}