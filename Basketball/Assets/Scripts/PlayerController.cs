using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] MovementController movementController;
    [SerializeField] BallController ballController;
    [SerializeField] new Rigidbody rigidbody;
    [SerializeField] Transform overheadPosition;
    [SerializeField] Transform holdingPosition;
    [SerializeField] Transform dribblePosition;
    [SerializeField] Transform armL;
    [SerializeField] Transform armR;
    [SerializeField] Transform springArm;
    [SerializeField] Transform cameraTarget;
    [SerializeField, Range(-5f, -1f)] float springArmLength;
    [SerializeField] Transform cameraTransform;
    [SerializeField] CapsuleCollider capsuleCollider;

    PlayerInput _playerInput;
    IEnumerator _throwBallCoroutine;

    public InputAction MoveAction { get; private set; }
    public InputAction LookAction { get; private set; }
    public InputAction ShootAction { get; private set; }
    public InputAction CancelShootAction { get; private set; }
    public InputAction JumpAction { get; private set; }
    public Transform CameraTransform => cameraTransform;
    public bool IsGrounded { get; private set; }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (TryGetComponent<PlayerInput>(out _playerInput))
        {
            MoveAction = _playerInput.actions.FindAction("Move");
            LookAction = _playerInput.actions.FindAction("Look");
            ShootAction = _playerInput.actions.FindAction("Shoot");
            CancelShootAction = _playerInput.actions.FindAction("CancelShoot");
            JumpAction = _playerInput.actions.FindAction("Jump");
        }

        movementController.Setup(this, rigidbody, springArm);
        ballController.Setup(this, overheadPosition, holdingPosition, dribblePosition, armL, armR, cameraTarget, springArmLength);

        _throwBallCoroutine = ColliderDisableCoroutine(0.5f);
    }

    void Update()
    {
        CheckGrounded();
    }
    
    public void ColliderDisableForThrow()
    {
        StartCoroutine(_throwBallCoroutine);
        _throwBallCoroutine = ColliderDisableCoroutine(0.5f);
    }

    IEnumerator ColliderDisableCoroutine(float waitTime)
    {
        capsuleCollider.enabled = false;

        yield return new WaitForSeconds(waitTime);

        capsuleCollider.enabled = true;
    }
    
    void CheckGrounded()
    {
        IsGrounded = Physics.Raycast(transform.position, -Vector3.up, 1.5f , LayerMask.GetMask("Ground"));
    }
}