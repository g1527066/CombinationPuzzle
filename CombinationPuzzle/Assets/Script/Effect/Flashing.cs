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

    // Update is called once per frame
    void Update()
    {
        totaleTime += Time.deltaTime;
        if(changeSpeed<totaleTime)
        {
            totaleTime = 0;
            Debug.Log("超えた");


            if (FlashingText[0].gameObject.activeSelf == true)
            {
                Debug.Log("falseにします");

                for (int i=0;i<FlashingText.Count;i++)
                {
                    FlashingText[i].gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.Log("trueにします");

                for (int i = 0; i < FlashingText.Count; i++)
                {
                    FlashingText[i].gameObject.SetActive(true);
                }
            }

        }





    }
}
