using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class SculptingLocation : ClearLocation
{
    [SerializeField] private MaterialProcessingFormulaSO[] formulas;

    //private float sculptingProgress;

    public override void Interact(Player player)
    {
        if (!IsHoldingObject())
        {
            if (!player.IsHoldingObject())
            {
                //Do nothing
            }
            else
            {
                if (MaterialCanBeProcessed(player.GetFusionMaterialObject().GetFusionMaterialObjectSO()))
                {
                    player.GetFusionMaterialObject().SetFusionMaterialObjectParent(this);
                }
                else
                {
                    Debug.LogError("This type of material cannot placed here.");
                }
            }
        }
        else
        {
            if (player.IsHoldingObject())
            {
                //Do nothing
            }
            else
            {
                GetFusionMaterialObject().SetFusionMaterialObjectParent(player);
            }
        }
    }

    public override void InteractAlter(Player player)
    {
        if (IsHoldingObject() && MaterialCanBeProcessed(GetFusionMaterialObject().GetFusionMaterialObjectSO()))
        {
            FusionMaterialObjectSO[] outputSO = GetOutPutFromInput(GetFusionMaterialObject().GetFusionMaterialObjectSO());

            GetFusionMaterialObject().DestroySelf();

            FusionMaterialObject.SpawnFusionMaterialObject(outputSO[0], this);
        }
    }

    private FusionMaterialObjectSO[] GetOutPutFromInput(FusionMaterialObjectSO inputSO)
    {
        if (inputSO != null)
        {
            foreach (MaterialProcessingFormulaSO formula in formulas)
            {
                if (formula.input == inputSO)
                {
                    return formula.outputs;
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

}
