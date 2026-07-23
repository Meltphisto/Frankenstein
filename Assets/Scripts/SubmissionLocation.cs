using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmissionLocation : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<MonsterAI>(out MonsterAI monster))
        {
            monster.SubmitMonster();
        }
    }
}
