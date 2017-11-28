using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peace : MonoBehaviour
{

    public PeaceType peaceType;
    public POINT point;

    float deleteTime = 0f;
    float flashingTime = 0f;
    bool isNoColor = false;
    public PeaceType nextPeaceType = PeaceType.None;
  
    // Update is called once per frame
    void Update()
    {
        if(peaceType==PeaceType.None)
        {
            deleteTime += Time.deltaTime;
            flashingTime += Time.deltaTime;
            if(flashingTime>GameSystem.I.flashingTime)
            {
                flashingTime = 0;

                if (isNoColor)
                    this.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                else
                    this.GetComponent<UnityEngine.UI.Image>().color = Color.clear;

                isNoColor = !isNoColor;
            }

            if (deleteTime>GameSystem.I.DeleteTime)
            {
                if(nextPeaceType!=PeaceType.None)
                {
                    peaceType = nextPeaceType;
                    nextPeaceType = PeaceType.None;
                    this.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                    flashingTime = deleteTime = 0;
                    SetSprite(PeaceManager.I.ReturnSprite(peaceType));
                    this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
                    PeaceManager.I.JudgeChangeNextPiece(this);
                    return;
                }
                PeaceManager.I.DeletePeace(this);
                AudioManager.I.PlaySound("DeletePeace");
                Destroy(this.gameObject);
            }
        }
    }
    public void SetSprite(Sprite setSptrite)
    {
        this.GetComponent<UnityEngine.UI.Image>().sprite = setSptrite;
    }
}
