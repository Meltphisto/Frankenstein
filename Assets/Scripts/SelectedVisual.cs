using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedVisual : MonoBehaviour
{
    [SerializeField] private BaseLocation baseLocation;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start()
    {
        Player.Instance.OnSelectedTargetChanged += Player_OnSelectedTableChanged;
        Hide();
    }

    private void Player_OnSelectedTableChanged(object sender, Player.OnSelectedTargetChangedEventArgs e)
    {
        if (e.selectedTarget is BaseLocation)
        {
            BaseLocation selectedLocation = e.selectedTarget as BaseLocation;
            if (selectedLocation == baseLocation)
            {
                Show();
            }
            else Hide();
        }
        else Hide();
    }

    private void Show()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }
}
