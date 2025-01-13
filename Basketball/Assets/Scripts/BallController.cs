using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    [SerializeField, Min(0.1f)] float forwardThrowForceScale = 2f;
    [SerializeField, Min(0.1f)] float forwardThrowForceBase = 5;
    [SerializeField, Range(1, 10)] float dribbleSpeed = 5;
    [SerializeField, Range(1, 10)] float chargeSpeed = 10f;
    
    bool _isAiming = false;
    bool _hasBall = false;
    float _wave = 0f;
    float _aimCharge = 0f;
    float _defaultSpringArmLength;
    float _springArmLength;
    float _targetSpringArmLength;
    PlayerController _controller;
    Transform _ball;
    Rigidbody _ballBody;
    Transform _dribblePosition;
    Transform _overheadPosition;
    Transform _holdingPosition;
    Transform _armL;
    Transform _armR;
    Transform _cameraTarget;

    const float ArmDribble = 50f;
    const float ArmDribbleMin = 10f;
    const float MaxCharge = 10f;
    const float ArmHeight = 160f;

    public void Setup(PlayerController controller, 
                        Transform overheadPosition, 
                        Transform holdingPosition, 
                        Transform dribblePosition, 
                        Transform armL, 
                        Transform armR, 
                        Transform cameraTarget, 
                        float springArmLength)
    {
        _controller = controller;
        _overheadPosition = overheadPosition;
        _holdingPosition = holdingPosition;
        _dribblePosition = dribblePosition;
        _armL = armL;
        _armR = armR;
        _cameraTarget = cameraTarget;
        _defaultSpringArmLength = springArmLength;
        _springArmLength = _defaultSpringArmLength;
        _cameraTarget.localPosition = new Vector3(0f, 0f, _springArmLength);

        _controller.ShootAction.started += StartShot;
        _controller.ShootAction.canceled += Shoot;
        _controller.CancelShootAction.started += CancelShot;
    }

    void OnDestroy()
    {
        _controller.ShootAction.started -= StartShot;
        _controller.ShootAction.canceled -= Shoot;
        _controller.CancelShootAction.started -= CancelShot;
    }

    void StartShot(InputAction.CallbackContext ctx)
    {
        if (!_hasBall) return;
        
        _isAiming = true;
        _aimCharge = 0f;
        
        _armL.localEulerAngles = Vector3.right * -ArmHeight;
        _armR.localEulerAngles = Vector3.right * -ArmHeight;
        _ball.position = _overheadPosition.position;
    }

    void CancelShot(InputAction.CallbackContext ctx)
    {
        if (!_hasBall) return;
        
        _isAiming = false;
        _armL.localEulerAngles = Vector3.right * 0f;
        _springArmLength = _defaultSpringArmLength;
    }

    void Shoot(InputAction.CallbackContext ctx)
    {
        if (!_hasBall) return;
        if (!_isAiming) return;

        _isAiming = false;
        _hasBall = false;
        _springArmLength = _defaultSpringArmLength;
        
        _controller.ColliderDisableForThrow();
        
        _armL.localEulerAngles = Vector3.right * 0f;
        _armR.localEulerAngles = Vector3.right * 0f;
        _ballBody.isKinematic = false;
        _ballBody.AddForce(_controller.CameraTransform.forward * (_aimCharge * forwardThrowForceScale + forwardThrowForceBase), ForceMode.Impulse);
        _ballBody.AddForce(transform.up * 8, ForceMode.Impulse);
        _ballBody.AddForce(_controller.CameraTransform.right * (_aimCharge/10 + 1), ForceMode.Impulse);
    }

    void LateUpdate()
    {
        _cameraTarget.localPosition = new Vector3(0f, 0f, _springArmLength);
        if (!_hasBall) return;
        
        if (_isAiming)
        {
            _springArmLength = _defaultSpringArmLength - (_aimCharge / MaxCharge) * 2;
            _ball.position = _overheadPosition.position;
            if (Mathf.Approximately(MaxCharge, _aimCharge)) return;

            _aimCharge += Time.deltaTime * chargeSpeed;
            if (_aimCharge >= MaxCharge)
            {
                _aimCharge = MaxCharge;
            }
            
            _armL.localEulerAngles = Vector3.right * (-ArmHeight - 3 * _aimCharge);
            _armR.localEulerAngles = Vector3.right * (-ArmHeight - 3 * _aimCharge);

            return;
        }
        
        _springArmLength = Mathf.Lerp(_springArmLength, _defaultSpringArmLength, 0.2f);

        if (!_controller.IsGrounded)
        {
            _armL.localEulerAngles = Vector3.right * -90;
            _armR.localEulerAngles = Vector3.right * -90;
            _ball.position = _holdingPosition.position;
            return;
        }
        
        _wave = Mathf.Abs(Mathf.Sin(Time.time * dribbleSpeed));

        _ball.position = _dribblePosition.position + Vector3.up * _wave;
        _armR.localEulerAngles = Vector3.right * (_wave * (-ArmDribble / Mathf.Pow(transform.localScale.y,2)) - ArmDribbleMin);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() == null) return;
        if (_hasBall) return;

        _hasBall = true;
        _ball = other.transform;
        _ballBody = _ball.GetComponent<Rigidbody>();
        _ballBody.isKinematic = true;
        _armL.localEulerAngles = Vector3.right * 0;
        _armR.localEulerAngles = Vector3.right * 0;
    }
}