using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IFusionMaterialHolder
{
    public static Player Instance { get; private set; }

    //GameInput Object
    private GameInput gameInput;

    //Movement Related
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotateSpeed = 10f;
    private bool canMove = false;
    private bool isWalking = false;

    //Collision Related
    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;

    //Interaction Related
    [SerializeField] private float interactDistance = 2f;
    private Vector3 lastInteractDir;
    [SerializeField] private LayerMask interactLayer;
    private BaseLocation selectedLocation;

    public event EventHandler<OnSelectedTableChangedEventArgs> OnSelectedTableChanged;
    public class OnSelectedTableChangedEventArgs : EventArgs
    {
        public BaseLocation selectedLocation;
    }

    //Pick up and drop object
    [SerializeField] private Transform holdPoint;
    private FusionMaterialObject holdingFusionMaterialObject;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        gameInput = GameInput.Instance;
        //Subscribe game input events
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if(selectedLocation != null)
        {
            selectedLocation.Interact(this);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleSelecting();
    }

    private void HandleMovement()
    {
        //Get move direction from player input
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        float moveDistance = moveSpeed * Time.deltaTime;
        
        canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
        
        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;
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

        //Detect table object
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycasthit, interactDistance, interactLayer))
        {
            if (raycasthit.transform.TryGetComponent<BaseLocation>(out BaseLocation selectedLocation))
            {
                if (selectedLocation != this.selectedLocation)
                {
                    SetSelectedTable(selectedLocation);
                }
            }
            else
            {
                SetSelectedTable(null);
            }
        }
        else
        {
            SetSelectedTable(null);
        }
    }

    private void SetSelectedTable(BaseLocation selectedLocation)
    {
        this.selectedLocation = selectedLocation;

        OnSelectedTableChanged?.Invoke(this, new OnSelectedTableChangedEventArgs
        {
            selectedLocation = selectedLocation
        });
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
