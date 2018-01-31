using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flashing : MonoBehaviour
{
    [SerializeField]
    List<Text> FlashingText = new List<Text>();
    [SerializeField]
    float changeSpeed = 1;

    float totaleTime = 0;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Destroy(this.gameObject);
        }

        totaleTime += Time.deltaTime;
        if(changeSpeed<totaleTime)
        {
            totaleTime = 0;

            if (FlashingText[0].gameObject.activeSelf == true)
            {
                for (int i=0;i<FlashingText.Count;i++)
                {
                    FlashingText[i].gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < FlashingText.Count; i++)
                {
                    FlashingText[i].gameObject.SetActive(true);
                }
            }
        }
    }
}
