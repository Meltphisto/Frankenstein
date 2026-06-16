using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedTableVisual : MonoBehaviour
{
    [SerializeField] private BaseLocation baseLocation;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start()
    {
        Player.Instance.OnSelectedTableChanged += Player_OnSelectedTableChanged;
        Hide();
    }

    private void Player_OnSelectedTableChanged(object sender, Player.OnSelectedTableChangedEventArgs e)
    {
        if (e.selectedLocation == baseLocation)
        {
            Show();
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
