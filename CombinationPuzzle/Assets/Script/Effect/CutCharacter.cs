using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutCharacter : MonoBehaviour {


    private float speed = 1.5f;//
    private float nowTime = 0.0f;

    Vector2 stratPosition;

    Vector2 addVector = Vector2.zero;

    public void SetCharacter(Vector2 strat,Vector2 end,float Speed)
    {
        speed = Speed;
        stratPosition = strat;
        gameObject.GetComponent<RectTransform>().anchoredPosition = strat;
        addVector = (end + strat) / speed;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (nowTime < speed)
        {
            nowTime += Time.deltaTime;
            gameObject.GetComponent<RectTransform>().anchoredPosition =stratPosition + addVector * nowTime;
            //EffectManager.I.PlayEffect(gameObject.GetComponent<RectTransform>().anchoredPosition,PeaceColor.Blue.DisplayName());
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
