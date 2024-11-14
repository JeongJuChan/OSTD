using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_HeroEquipmentViewer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    public void UpdateEquipment(int index, Sprite sprite)
    {
        if (index == Consts.GRANADE_INDEX || index == Consts.BACKPACK_INDEX)
        {
            return;
        }

        spriteRenderers[index].sprite = sprite;
    }
}
