using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInteractVisual : MonoBehaviour
{
    [SerializeField] private MonsterAI monster;
    [SerializeField] private GameObject monsterSelectedVisual;

    private void Start()
    {
        Player.Instance.OnSelectedTargetChanged += Player_OnSelectedMonsterChanged;
        Hide(monsterSelectedVisual);
    }

    private void Player_OnSelectedMonsterChanged(object sender, Player.OnSelectedTargetChangedEventArgs e)
    {
        if (e.selectedTarget is MonsterAI == monster)
        {
            Show(monsterSelectedVisual);
        }
        else Hide(monsterSelectedVisual);
    }

    private void OnDestroy()
    {
        Player.Instance.OnSelectedTargetChanged -= Player_OnSelectedMonsterChanged;
    }

    private void Show(GameObject target)
    {
        target.SetActive(true);
    }

    private void Hide(GameObject target)
    {
        target.SetActive(false);
    }
}
