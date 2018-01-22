using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutCharacter : MonoBehaviour
{


    private float speed = 1.5f;
    private float nowTime = 0.0f;
    private float StayTimingTime = 0;
    private float StayTime = 0;
    private bool isStay = false;
    private float cutSpeed = 40f;


    Vector2 stratPosition;

    Vector2 addVector = Vector2.zero;

    AnimationImage animation = null;

    public void SetCharacter(Vector2 strat, Vector2 end, float Speed, float stayTiming, float stayTime,AnimationImage zanEffect)
    {
        StayTimingTime = stayTiming;
        StayTime = stayTime;
        speed = Speed;
        stratPosition = strat;
        gameObject.GetComponent<RectTransform>().anchoredPosition = strat;
        addVector = (end + strat) / speed;
        this.transform.Rotate(new Vector3(0, 180, 0));
        animation = zanEffect;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (nowTime >=StayTimingTime && isStay == false)
        {
            StartCoroutine(StayProsess());
        }
        else if (nowTime <=   StayTimingTime)
        {
            nowTime += Time.deltaTime;
            gameObject.GetComponent<RectTransform>().anchoredPosition = stratPosition + addVector * nowTime;
            //EffectManager.I.PlayEffect(gameObject.GetComponent<RectTransform>().anchoredPosition,PeaceColor.Blue.DisplayName());
        }
        else if (isStay == true)//直線に進み、終了したら切る
        {
            Debug.Log(" isStay ");
            nowTime += Time.deltaTime;
            gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(cutSpeed, 0);
            if (nowTime>speed+StayTime+5)//5秒プラスしたら
            {

                Destroy(this.gameObject);
            }
        }

    }
    private IEnumerator StayProsess()
    {
        yield return new WaitForSeconds(StayTime);
        isStay = true;

        animation.StartAnimation();


    }
}
