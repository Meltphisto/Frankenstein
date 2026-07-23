using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 8f;

    private CharacterController controller;
    private GameInput gameInput;
    private Vector2 moveInput;
    private Vector3 currentVelocity = Vector3.zero;

    public float CurrentSpeed { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        gameInput = GameInput.Instance;
    }

    private void FixedUpdate()
    {
        moveInput = gameInput.GetMovementVectorNormalized();
        // 1. 计算目标速度
        Vector3 worldMoveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        float targetSpeed = moveSpeed;
        Vector3 targetVelocity = worldMoveDirection * targetSpeed;

        // 2. 平滑插值当前速度
        float currentAccel = moveInput.sqrMagnitude > 0.01f ? acceleration : deceleration;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, currentAccel * Time.deltaTime);
        if (currentVelocity.magnitude > targetSpeed && targetSpeed > 0)
            currentVelocity = currentVelocity.normalized * targetSpeed;

        // 3. 应用移动（CharacterController 自动处理碰撞和滑动）
        controller.Move(currentVelocity * Time.deltaTime);

        // 4. 更新当前速度（实际移动速度）
        CurrentSpeed = controller.velocity.magnitude;
    }
}