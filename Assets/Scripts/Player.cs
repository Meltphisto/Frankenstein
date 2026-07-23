using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour, IFusionMaterialHolder
{
    public static Player Instance { get; private set; }

    //GameInput Object
    private GameInput gameInput;
    private CharacterController controller;

    [Header("移动参数")]
    //Movement Related
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 8f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float baitingSpeed = 3f;
    [SerializeField] private float rotateSpeed = 10f;
    private bool isBaiting = false;

    private float currentSpeed;
    //private bool isWalking = false;

    [Header("交互参数")]
    //Interaction Related
    [SerializeField] private float interactDistance = 2f;
    private Vector3 lastInteractDir;
    [SerializeField] private LayerMask interactLayer;
    private IInteractable selectedTarget;

    public event EventHandler<OnSelectedTargetChangedEventArgs> OnSelectedTargetChanged;
    public class OnSelectedTargetChangedEventArgs : EventArgs
    {
        public IInteractable selectedTarget;
    }

    //Pick up and drop object
    [SerializeField] private Transform holdPoint;
    private FusionMaterialObject holdingFusionMaterialObject;

    
    private Vector2 moveInput;
    private Vector3 currentVelocity = Vector3.zero;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        controller = GetComponent<CharacterController>();
        currentSpeed = moveSpeed;
    }

    private void Start()
    {
        gameInput = GameInput.Instance;

        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlterAction += GameInput_OnInteractAlterAction;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleSelecting();
    }

    private void GameInput_OnInteractAlterAction(object sender, EventArgs e)
    {
        if (selectedTarget != null)
        {
            selectedTarget.InteractAlter(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if(selectedTarget != null)
        {
            selectedTarget.Interact(this);
        }
    }

    private void HandleMovement()
    {
        moveInput = gameInput.GetMovementVectorNormalized();
        //计算方向
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        Vector3 targetVelocity = moveDir * currentSpeed;

        //平滑插值（加/减速）
        float currentAccel = moveInput.sqrMagnitude > 0.01f ? acceleration : deceleration;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, currentAccel * Time.deltaTime);
        if (currentVelocity.magnitude > currentSpeed)
            currentVelocity = currentVelocity.normalized * currentSpeed;

        //CharacterController 处理移动
        controller.Move(currentVelocity * Time.deltaTime);

        //isWalking = moveDir != Vector3.zero;
        //朝向移动方向
        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateSpeed * Time.deltaTime);
    }

    private void HandleSelecting()
    {
        //Get face direction from input
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDic = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDic != Vector3.zero)
        {
            lastInteractDir = moveDic;
        }

        //Detect interactable object
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycasthit, interactDistance, interactLayer))
        {
            if (raycasthit.transform.TryGetComponent<IInteractable>(out IInteractable selectedTarget))
            {
                if (selectedTarget != this.selectedTarget)
                {
                    SetSelectedTarget(selectedTarget);
                }
            }
            else
            {
                SetSelectedTarget(null);
            }
        }
        else
        {
            SetSelectedTarget(null);
        }
    }

    private void SetSelectedTarget(IInteractable selectedTarget)
    {
        this.selectedTarget = selectedTarget;

        OnSelectedTargetChanged?.Invoke(this, new OnSelectedTargetChangedEventArgs
        {
            selectedTarget = selectedTarget
        });
    }

    public void SetBaiting()
    {
        isBaiting = !isBaiting;

        switch (isBaiting)
        {
            case true:
                currentSpeed = baitingSpeed;
                break;
            case false:
                currentSpeed = moveSpeed;
                break;
        }
    }

    public bool GetBaitingState()
    {
        return isBaiting;
    }

    public Transform GetHoldPointTransform()
    {
        return holdPoint;
    }

    public void SetHoldingObject(FusionMaterialObject fusionMaterialObject)
    {
        holdingFusionMaterialObject = fusionMaterialObject;
    }

    public FusionMaterialObject GetFusionMaterialObject()
    {
        return holdingFusionMaterialObject;
    }

    public void ClearHoldingObject()
    {
        holdingFusionMaterialObject = null;
    }

    public bool IsHoldingObject()
    {
        return holdingFusionMaterialObject != null;
    }
}
