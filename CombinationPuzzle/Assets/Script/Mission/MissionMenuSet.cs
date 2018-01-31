using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionMenuSet : MonoBehaviour {

    [SerializeField]
    Image MissionImage = null;

    [SerializeField]
    Image ClearImage = null;

    public void SetClearActive(bool isActive)
    {
        ClearImage.gameObject.SetActive(isActive);
    }

    public void ChangeMissionSprite(Sprite sprite)
    {
        MissionImage.sprite = sprite;
    }
}
