using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody _rb;
    private Vector2 _moveInput;
    private Vector3 _moveDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (Camera.main)
        {
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = Camera.main.transform.right;
            right.y = 0;
            right.Normalize();

            _moveDirection = (right * _moveInput.x + forward * _moveInput.y).normalized;
        }
        else
        {
            _moveDirection = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;
        }
    }

    private void FixedUpdate()
    {
        Vector3 move = _moveDirection * moveSpeed;
        Vector3 velocity = new Vector3(move.x, _rb.linearVelocity.y, move.z);

        _rb.linearVelocity = velocity;
    }

}