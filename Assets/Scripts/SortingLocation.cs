using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingLocation : ClearLocation
{
    [SerializeField] private MaterialProcessingFormulaSO[] formulas;

    [SerializeField] private ClearLocation[] outputLocations;
    private enum SortingState
    {
        Idle,
        Sorting,
        Finished
    }

    private SortingState currentSortingState;
    private float sortingTimer;
    private MaterialProcessingFormulaSO processingFormulaSO;

    private void Start()
    {
        currentSortingState = SortingState.Idle;
        processingFormulaSO = null;
    }

    private void Update()
    {
        switch (currentSortingState)
        {
            case SortingState.Idle:
                break;
            case SortingState.Sorting:
                sortingTimer += Time.deltaTime;
                if(sortingTimer > processingFormulaSO.maxProcessingStep)
                {
                    currentSortingState = SortingState.Finished;
                    //Finished sort
                    FusionMaterialObjectSO[] outputSO = processingFormulaSO.outputs;

                    GetFusionMaterialObject().DestroySelf();

                    for (int i = 0; i < outputSO.Length; i++)
                    {
                        FusionMaterialObject.SpawnFusionMaterialObject(outputSO[i], outputLocations[i]);
                    }
                }
                break;
            case SortingState.Finished:
                //sortingTimer = 0;
                currentSortingState = SortingState.Idle;
                break;
        }
    }
    public override void Interact(Player player)
    {
        if (!IsHoldingObject())
        {
            if (player.IsHoldingObject() && !IsAnyOutputLocationOccupied())
            {
                if (MaterialCanBeProcessed(player.GetFusionMaterialObject().GetFusionMaterialObjectSO()))
                {
                    //Player put object that can be processed
                    player.GetFusionMaterialObject().SetFusionMaterialObjectParent(this);
                    //Start to sort
                    sortingTimer = 0;
                    processingFormulaSO = GetOutPutFromInput(GetFusionMaterialObject().GetFusionMaterialObjectSO());
                    currentSortingState = SortingState.Sorting;
                }
                else
                {
                    Debug.LogError("This type of material cannot placed here.");
                }
            }
        }
        /*
         * Player cannot take object away after sorting start
        else
        {
            if (!player.IsHoldingObject())
            {
                //Player
                GetFusionMaterialObject().SetFusionMaterialObjectParent(player);

                currentSortingState = SortingState.Idle;
            }
        }
        */
    }

    private MaterialProcessingFormulaSO GetOutPutFromInput(FusionMaterialObjectSO inputSO)
    {
        if (inputSO != null)
        {
            foreach (MaterialProcessingFormulaSO formula in formulas)
            {
                if (formula.input == inputSO)
                {
                    return formula;
                }
            }
        }

        return null;
    }

    private bool MaterialCanBeProcessed(FusionMaterialObjectSO fusionMaterialObjectSO)
    {
        foreach (MaterialProcessingFormulaSO formula in formulas)
        {
            if (formula.input == fusionMaterialObjectSO)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsAnyOutputLocationOccupied()
    {
        foreach(ClearLocation outputLocation in outputLocations)
        {
            if (outputLocation.IsHoldingObject())
            {
                return true;
            }
        }

        return false;
    } 
}
