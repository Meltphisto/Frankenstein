using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour, IInteractable
{
    // ---------- 状态定义 ----------
    public enum MonsterState
    {
        Idle, //游荡，默认状态
        Following, // 跟随玩家
        Charging, //冲向诱饵
        Submitted //提交中
    }

    [Header("速度配置")]
    [SerializeField] private float idleSpeed = 5f;  //游荡时的移动速度
    [SerializeField] private float followSpeed = 3f;   //跟随时的移动速度
    [SerializeField] private float chargeSpeed = 10f;   //冲刺时的速度

    [Header("跟随参数")]
    [SerializeField] private float followDistance = 1.8f;          // 与玩家保持的距离
    [SerializeField] private float followBreakDistance = 8f;       // 脱钩距离

    [Header("游荡相关")]
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float sampleDistance = 2.0f;
    [SerializeField] private float wanderingGap = 5f;

    [Header("交互范围")]
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private float deliveryRadius = 1.0f;

    [Header("诱饵相关")]
    [SerializeField] private float baitAttractRadius = 1.2f;
    [SerializeField] private float baitLifetime = 3f;

    // ---------- 组件引用 ----------
    private Player player;
    private Transform deliveryPoint;
    private NavMeshAgent agent;
    private NavMeshPath path;

    // ---------- 运行时状态 ----------
    public MonsterState CurrentState { get; private set; } = MonsterState.Idle;
    private Vector3 moveTarget; //移动的目标点

    private Vector3 wanderCenter; //游荡时的中心点
    private bool isWaitingWandering = true;
    private float waitTimer;
    private bool isTargetCalculated = false;   // 是否已计算出有效目标点
    private int maxSampleAttempts = 10;        // 每次尝试采样的最大次数

    private void Start()
    {
        player = Player.Instance;
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        moveTarget = transform.position;
        SetNewIdleTarget();
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case MonsterState.Idle:
                UpdateIdle();
                break;
            case MonsterState.Following:
                UpdateFollowing();
                break;
            case MonsterState.Charging:
                UpdateCharging();
                break;
            case MonsterState.Submitted:
                break;
        }
    }

    // ---------- 状态更新方法 ----------

    private void UpdateIdle()
    {
        if (isWaitingWandering)
        {
            waitTimer -= Time.deltaTime;

            // 如果还未计算出有效目标，则持续尝试计算
            if (!isTargetCalculated)
            {
                if (TryCalculateNextWanderTarget(out moveTarget))
                {
                    isTargetCalculated = true;  // 获得有效目标，停止计算
                }
            }

            if (waitTimer <= 0 && isTargetCalculated)
            {
                // 等待结束，将预计算的目标点设为 NavMesh 目标
                agent.SetDestination(moveTarget);
                isWaitingWandering = false;
                isTargetCalculated = false;
            }
            return;
        }

        // 非等待状态：检查是否到达目标
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartWaiting();
        }
    }

    /// <summary>
    /// 跟随逻辑
    /// </summary>
    private void UpdateFollowing()
    {
        // 脱钩检测
        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distToPlayer > followBreakDistance)
        {
            SwitchState(MonsterState.Idle);
            return;
        }

        // 移动:在距离大于期望值时追赶
        if (distToPlayer > followDistance)
        {
            // 计算目标位置：玩家位置 + (怪物→玩家方向) * followDistance
            Vector3 dirFromPlayer = (transform.position - player.transform.position).normalized;
            moveTarget = player.transform.position + dirFromPlayer * followDistance;
            // 使用NavMesh移动
            agent.SetDestination(moveTarget);
        }

        // 面向玩家
        Vector3 lookDir = player.transform.position - transform.position;
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    private void UpdateCharging()
    {
        /*
        // 冲刺方向
        Vector3 dir = (followTransform - transform.position).normalized;
        float playerSpeed = playerMovement != null ? playerMovement.CurrentSpeed : 5f;
        float chargeSpeed = playerSpeed * chargeSpeedRatio;
        transform.position += dir * chargeSpeed * Time.deltaTime;

        // 检查是否到达诱饵落点
        if (Vector3.Distance(transform.position, followTransform) < baitAttractRadius)
        {
            SwitchState(MonsterState.Idle);
            return;
        }

        baitTimer -= Time.deltaTime;
        if (baitTimer <= 0)
            SwitchState(MonsterState.Idle);
        */
    }

    // ---------- 状态切换 ----------
    private void SwitchState(MonsterState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case MonsterState.Idle:
                SetNewIdleTarget();
                break;
            case MonsterState.Following:
                agent.speed = followSpeed;
                break;
            case MonsterState.Charging:
                break;
            case MonsterState.Submitted:
                OnSubmitted();
                break;
        }
    }

    // ---------- 外部交互接口 ----------
    public void Interact(Player player)
    {
        player.SetBaiting();

        switch (player.GetBaitingState())
        {
            case true:
                SwitchState(MonsterState.Following);
                break;
            case false:
                SwitchState(MonsterState.Idle);
                break;
        }
    }

    public void OnBaitThrown(Vector3 targetPosition)
    {
        moveTarget = targetPosition;
        SwitchState(MonsterState.Charging);
    }

    // ---------- 辅助方法 ----------
    private void SetNewIdleTarget()
    {
        agent.speed = idleSpeed;
        wanderCenter = transform.position;
        StartWaiting();
    }

    /// <summary>
    /// 尝试在游荡半径内采样一个可达的目标点
    /// </summary>
    private bool TryCalculateNextWanderTarget(out Vector3 target)
    {
        Vector3 center = wanderCenter;
        for (int i = 0; i < maxSampleAttempts; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection.y = 0;
            Vector3 randomPos = center + randomDirection;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, sampleDistance, NavMesh.AllAreas))
            {
                // 确保目标点与当前位置有一定距离，避免原地打转
                if (Vector3.Distance(hit.position, transform.position) > 0.5f)
                {
                    target = hit.position;
                    return true;
                }
            }
        }
        // 若所有尝试都失败，返回一个简单的偏移作为兜底（但标记为失败）
        target = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        return false;
    }

    private void StartWaiting()
    {
        isWaitingWandering = true;
        waitTimer = wanderingGap;
        isTargetCalculated = false;// 重置计算标记
        agent.ResetPath();// 停止 Agent 移动
    }

    public void SubmitMonster()
    {
        if (CurrentState == MonsterState.Submitted)
            return;
        SwitchState(MonsterState.Submitted);

        if (player.GetBaitingState())
        {
            player.SetBaiting();
        }
    }

    private void OnSubmitted()
    {
        // 通知关卡管理器（如有）
        // 播放特效，然后销毁
        Destroy(gameObject, 3.0f);
    }

    public void InteractAlter(Player player)
    {
    }
}