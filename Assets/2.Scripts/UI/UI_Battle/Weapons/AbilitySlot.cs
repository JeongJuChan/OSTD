using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour
{
    [SerializeField] private Image abilityImage;

    public void SetSprite(Sprite abilitySprite)
    {
        abilityImage.sprite = abilitySprite;
        abilityImage.gameObject.SetActive(abilitySprite != null);
    }
}
