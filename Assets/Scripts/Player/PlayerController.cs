using UnityEngine;

[RequireComponent(typeof(InputWrapper))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");

    [Header("Settings")]
    [field: SerializeField] public float MaxSpeed { get; private set; } = 10f;
    [field: SerializeField] public float Acceleration { get; private set; } = 10f;
    [field: SerializeField] public float Deceleration { get; private set; } = 10f;
    [field: SerializeField] public bool ConserveMomentum { get; private set; } = true;
    [field: SerializeField] public LayerMask Ground { get; private set; }
    [field: SerializeField] public float DistanceFromGround { get; private set; } = 0.1f;
    [field: SerializeField] public Transform PlayerFeet { get; private set; }

    public bool Active { get; set; } = true;
    public bool UsingAnimations { get; private set; } = true;
    
    private Ps4Controller input;
    private Rigidbody rb;
    private Animator animator;
    private void Awake()
    {
        input = GetComponent<InputWrapper>().CurrentController;
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        if (animator == null) UsingAnimations = false;
    }

    private void FixedUpdate()
    {
        if (Active)
        {
            if (input.LeftStick.magnitude > 0.1f)
            {
                var forward = new Vector3(input.LeftStick.x, 0, input.LeftStick.y).normalized;
                var rotationGoal = Quaternion.LookRotation(forward, Vector3.up);
                transform.rotation = Quaternion.Euler(0,rotationGoal.eulerAngles.y, 0);
            }
            Vector3 targetSpeed = new Vector3(input.LeftStick.x, 0, input.LeftStick.y) * MaxSpeed;
            Vector3 speedDifference = targetSpeed - new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);
        
            float accelerationRate;
            accelerationRate = (targetSpeed.sqrMagnitude > 0.01f) ? Acceleration : Deceleration;
            
            if (ConserveMomentum && Mathf.Abs(rb.linearVelocity.x) > targetSpeed.magnitude &&
                (Mathf.Approximately(Mathf.Sign(rb.linearVelocity.x), Mathf.Sign(targetSpeed.x)) ||
                 Mathf.Approximately(Mathf.Sign(rb.linearVelocity.z), Mathf.Sign(targetSpeed.y))) &&
                targetSpeed.sqrMagnitude > 0.01f) accelerationRate = 0f;
        
            Vector3 finalMovement = speedDifference * accelerationRate;
            rb.AddForce(new Vector3(finalMovement.x, finalMovement.y, finalMovement.z), ForceMode.Force);
            
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit hit, Mathf.Infinity, Ground))
            {
                var endPos = hit.point;
                endPos.y += DistanceFromGround;
                endPos.y += transform.position.y - PlayerFeet.position.y;
                transform.position = new Vector3(transform.position.x, endPos.y, transform.position.z);
            }

            rb.angularVelocity = Vector3.zero;
            if(UsingAnimations) animator.SetFloat(Speed, rb.linearVelocity.magnitude / MaxSpeed);
        }
    }
}
