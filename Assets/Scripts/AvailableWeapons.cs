using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvailableWeapons : MonoBehaviour
{
    [SerializeField] private Image[] weaponImages;
    [SerializeField] private Image[] backgroundImages;


    public void SetActive(int weaponIndex) {
        var tempColor = weaponImages[weaponIndex].color;
        tempColor.a = 1f;
        weaponImages[weaponIndex].color = tempColor;
    }

    public void ChooseWeapon(int weaponIndex) {
        foreach (Image backgroundImage in backgroundImages) {
            var tempColor = backgroundImage.color;
            tempColor.a = 0f;
            backgroundImage.color = tempColor;
        }

        var tempColor2 = backgroundImages[weaponIndex].color;
        tempColor2.a = 1f;
        backgroundImages[weaponIndex].color = tempColor2;

    }
}
