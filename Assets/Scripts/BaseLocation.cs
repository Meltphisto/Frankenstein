using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLocation : MonoBehaviour, IInteractable
{
    public virtual void Interact(Player player)
    {
        Debug.Log("Interact");
    }
}
