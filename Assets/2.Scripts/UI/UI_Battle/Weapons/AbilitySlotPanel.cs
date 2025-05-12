using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySlotPanel : MonoBehaviour
{
    [SerializeField] private AbilitySlot[] abilitySlots;

    public void Reset()
    {
        for (int i = 0; i < abilitySlots.Length; i++)
        {
            abilitySlots[i].SetSprite(null);
        }
    }

    public void SetSprites(List<Sprite> abilitySprites)
    {
        if (abilitySprites == null)
        {
            for (int i = 0; i < abilitySlots.Length; i++)
            {
                abilitySlots[i].SetSprite(null);
            }
        }
        else
        {
            for (int i = 0; i < abilitySprites.Count; i++)
            {
                abilitySlots[i].SetSprite(abilitySprites[i]);
            }
        }
    }
}
