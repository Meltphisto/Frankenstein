using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MaterialProcessingFormulaSO : ScriptableObject
{
    public FusionMaterialObjectSO input;
    public FusionMaterialObjectSO[] outputs;

    public int maxProcessingStep;
}
