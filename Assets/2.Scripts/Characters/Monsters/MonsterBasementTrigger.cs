using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBasementTrigger : MonoBehaviour
{
    public void EncounterMonsterBasement()
    {
        BoxManager.instance.boxMoveController.UpdateMovingState(false);
        BoxManager.instance.boxMoveController.UpdateMonsterBasementEncounterState(true);
    }
}
