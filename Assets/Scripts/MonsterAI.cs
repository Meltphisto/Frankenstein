using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 怪物 AI 状态机 - 支持空闲、跟随引导、冲刺吃饵、提交传送
/// </summary>
public class MonsterAI : MonoBehaviour, IInteractable
{
    // ---------- 状态定义 ----------
    public enum MonsterState
    {
        Idle,           // 在缝合台附近小范围游荡
        Following,      // 被玩家手持诱饵引导，慢速跟随
        Charging,       // 响应投掷诱饵，高速冲刺
        Submitting       // 已传送提交，销毁自身
    }

    private MonsterState m_State = MonsterState.Idle;

    [Header("速度配置")]
    [SerializeField] private float idleSpeed = 6f;          // 游荡速度
    [SerializeField] private float followSpeed = 4f;   // 跟随速度为玩家速度的80%
    [SerializeField] private float chargeSpeed = 15f;   // 冲刺速度为玩家速度的1.5倍
    [SerializeField] private float rotateSpeed = 5f;
    private bool canMove = true;

    [Header("交互范围")]
    //[SerializeField] private float interactRange = 1.5f;      // 玩家按E引导的有效距离
    //[SerializeField] private float followBreakDistance = 8f;  // 跟随脱钩距离

    [SerializeField] private float monsterRadius = 1.0f;     // 交付判定半径
    [SerializeField] private float monsterHeight = 1.0f;
    [SerializeField] private LayerMask SubmissionLayer;
    private Collider[] colliderResults = new Collider[10];

    [Header("游荡相关")]
    private bool isIdling = false;
    private Vector3 idleTarget;

    [Header("诱饵相关")]
    [SerializeField] private Transform baitTarget;
    [SerializeField] private float followDistance = 1f; //玩家吸引时和玩家的相隔距离
    //[SerializeField] private float baitDistance = 0.5f; // 诱饵吸引力场半径
    //[SerializeField] private float baitLifetime = 3f;         // 诱饵力场持续时间

    //private Transform deliveryPoint;
    private void Update()
    {
        // 状态机更新
        switch (m_State)
        {
            case MonsterState.Idle:
                UpdateIdle();
                break;
            case MonsterState.Following:
                UpdateFollowing();
                break;
            case MonsterState.Charging:
                //UpdateCharging();
                break;
            case MonsterState.Submitting:
                // 无行为，等待销毁
                break;
        }

        CheckSubmission();

    }

    // ---------- 状态切换 ----------
    private void SwitchState(MonsterState newState)
    {
        m_State = newState;

        // 进入新状态（初始化）
        switch (newState)
        {
            case MonsterState.Idle:
                //SetNewIdleTarget();
                break;
            case MonsterState.Following:
                baitTarget = Player.Instance.transform;
                break;
            case MonsterState.Charging:
                // 设置诱饵计时器
                //baitTimer = baitLifetime;
                break;
            case MonsterState.Submitting:
                // 开始提交动画或销毁
                OnSubmitted();
                break;
        }
    }

    private void OnSubmitted()
    {
        transform.LookAt(-Vector3.forward);
        Destroy(gameObject, 3);
    }

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

    public void InteractAlter(Player player)
    {
        //throw new System.NotImplementedException();
    }

    private void UpdateFollowing()
    {
        if (baitTarget == null) return;

        //Get move direction from player position
        float distToPlayer = Vector3.Distance(transform.position, baitTarget.position);

        // 计算目标位置：以玩家为圆心，沿怪物→玩家的方向，向外推 followDistance 距离
        // 即：玩家位置 + (怪物位置 - 玩家位置).normalized * followDistance
        Vector3 directionFromPlayer = (transform.position - baitTarget.position).normalized;
        Vector3 targetPosition = baitTarget.position + directionFromPlayer * followDistance;

        //1.是否可以移动
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float moveDistance = followSpeed * Time.deltaTime;
        canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * monsterHeight, monsterRadius, moveDirection, moveDistance,-5,QueryTriggerInteraction.Ignore);

        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDirection.x, 0, 0).normalized;
            canMove = moveDirection.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * monsterHeight, monsterRadius, moveDirX, moveDistance);
            if (canMove)
            {
                moveDirection = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDirection.z).normalized;
                canMove = moveDirection.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * monsterHeight, monsterRadius, moveDirZ, moveDistance);
                if (canMove)
                {
                    moveDirection = moveDirZ;
                }
            }
        }

        if (canMove)
        {
            //仅在“怪物与玩家的距离 > 期望距离”时才移动
            if (distToPlayer > followDistance)
            {
                transform.position += moveDirection * moveDistance;
            }

        }

        // 3. 转向玩家（始终执行，无论距离远近）
        // 怪物永远面向玩家，保持“被注视”的交互感
        Vector3 lookDirection = baitTarget.position - transform.position;
        if (lookDirection != Vector3.zero)
        {
            // 只绕Y轴旋转（保持怪物直立）
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * rotateSpeed  // 平滑转向速度
            );
        }
    }

    private void SetNewIdleTarget()
    {
        idleTarget =transform.position;
    }

    private void UpdateIdle()
    {
    }

    private void HandleMovement()
    {

    }

    private void CheckSubmission()
    {
        int counts = Physics.OverlapCapsuleNonAlloc(transform.position, transform.position + Vector3.up * monsterHeight, monsterRadius, colliderResults, SubmissionLayer);

        for (int i = 0; i < counts; i++)
        {
            if (colliderResults[i].transform.GetComponent<SubmissionLocation>())
            {
                SwitchState(MonsterState.Submitting);
                break;
            }
        }
    }
}
