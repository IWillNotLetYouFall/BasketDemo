using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float movementSpeed = 10f;
    [SerializeField, Min(0.01f)] private float cameraSpeedX = 0.1f;
    [SerializeField, Min(0.01f)] private float cameraSpeedY = 0.04f;
    [SerializeField, Min(0.01f)] private float jumpHeightScale = 10;
    [SerializeField, Min(0.01f)] private float jumpHeightBase = 12;

    PlayerController _controller;
    Rigidbody _rigidbody;
    float _xRotation;
    float _jumpCharge;
    bool _chargingJump;
    Transform _springArm;
    Vector3 _deltaMove;
    Vector3 _moveDirection;
    
    const float MaxCharge = 1f;

    public void Setup(PlayerController controller, Rigidbody rigidbody, Transform springArm)
    {
        _controller = controller;
        _springArm = springArm;
        _rigidbody = rigidbody;

        _controller.JumpAction.started += ChargeJump;
        _controller.JumpAction.canceled += Jump;
    }

    void OnDestroy()
    {
        _controller.JumpAction.started -= ChargeJump;
        _controller.JumpAction.canceled -= Jump;
    }
    
    void ChargeJump(InputAction.CallbackContext ctx)
    {
        if (!_controller.IsGrounded) return;
        
        _jumpCharge = 0.0f;
        _chargingJump = true;
    }
    
    void Jump(InputAction.CallbackContext ctx)
    {
        if (!_chargingJump) return;
        
        transform.localScale = Vector3.one;
        _chargingJump = false;
        
        _rigidbody.AddForce(transform.up * (_jumpCharge * jumpHeightScale + jumpHeightBase), ForceMode.Impulse);
    }

    void Update()
    {
        MoveCamera();

        if (!_chargingJump) return;
        if (Mathf.Approximately(MaxCharge, _jumpCharge)) return;

        _jumpCharge += Time.deltaTime;
        if (_jumpCharge >= MaxCharge)
        {
            _jumpCharge = MaxCharge;
        }

        transform.localScale = new Vector3(transform.localScale.x, 1 - (_jumpCharge/2), transform.localScale.z);
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 movement = _controller.MoveAction.ReadValue<Vector2>();
        _deltaMove = Vector3.Lerp(_deltaMove, transform.right * movement.x + transform.forward * movement.y, 0.1f);

        transform.position += _deltaMove * (movementSpeed * Time.fixedDeltaTime);
    }

    void MoveCamera()
    {
        Vector2 lookout = _controller.LookAction.ReadValue<Vector2>();
        _xRotation -= lookout.y * cameraSpeedY;
        _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);

        _springArm.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(0, lookout.x * cameraSpeedX, 0);
    }
}