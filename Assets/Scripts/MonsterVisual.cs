using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterVisual : MonoBehaviour
{
    [SerializeField] private MonsterBlock headVisual;
    [SerializeField] private MonsterBlock torsoVisual;
    [SerializeField] private MonsterBlock legVisual;
    [SerializeField] private MonsterBlock extraVisual;

    public void SetMonsterPartVisual(BlockType blockType, FusionMaterialObject fusionMaterialObject)
    {
        MonsterBlock targetVisual = GetVisualObjectFromBlockType(blockType);
        if (fusionMaterialObject != null && targetVisual != null)
        {
            fusionMaterialObject.SetFusionMaterialObjectParent(targetVisual);
        }
    }

    private MonsterBlock GetVisualObjectFromBlockType(BlockType blockType)
    {
        switch(blockType)
        {
            case BlockType.Head:
                return headVisual;
            case BlockType.Torso:
                return torsoVisual;
            case BlockType.Leg:
                return legVisual;
            case BlockType.Extra:
                return extraVisual;
        }
        return null;
    }


}
