using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutObject : MonoBehaviour
{
    Vector3 startPosition;
    Vector3 endPosition;
    float totalTime = 0;
    float cutTime = 0;

    //斬撃用（カット後に）
    protected List<GameObject> cutedObject = new List<GameObject>();
    protected Vector3 SavePosition;
    protected Vector3[] syosokudo;
    protected float[] kakudoRadian;
    //   protected float totalTime;
    protected bool isHit;



    // Use this for initialization
    void Start()
    {

    }
    bool cutFlag = false;
    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
        if (cutFlag == false && totalTime >= cutTime)
        {
            Debug.Log("カットされた、、はず、、" + startPosition + "   e=" + endPosition);
            List<SpriteSlicer2DSliceInfo> cutObject = new List<SpriteSlicer2DSliceInfo>();
            // SpriteSlicer2D.SliceSprite(startPosition, endPosition, this.gameObject, false, ref cutObject);
            SpriteSlicer2D.ShatterSprite(this.gameObject,30,false,ref cutObject);

            cutFlag = true;
            //飛ばす処理もいれる
            totalTime = 0;
            //TODO;error直す
            try
            {
                for (int i = 0; i < cutObject[0].ChildObjects.Count; i++)
                    cutedObject.Add(cutObject[0].ChildObjects[i]);
            }
            catch
            {
                //エラーがあったら削除する
                Destroy(gameObject);
                MyDebug.DrawError("CutObject Out of Range Error");
            }
            SavePosition = cutObject[0].SlicedObject.transform.position;
            syosokudo = new Vector3[cutedObject.Count];
            syosokudo[0] = Vector3.zero;
            kakudoRadian = new float[cutedObject.Count];

            gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
            gameObject.SetActive(true);
            this.enabled = true;
            Destroy(gameObject.GetComponent<BoxCollider2D>());
        }
        else if (cutFlag == true)
        {
            if (SavePosition != cutedObject[0].transform.position)
            {
                //初回一回目に初速度、角度求める
                if (syosokudo[0] == Vector3.zero)
                {
                    float moveSpeed = 10;// 4.5f;
                    for (int i = 0; i < cutedObject.Count; i++)
                    {
                        Vector3 selectPosition = cutedObject[i].transform.position;
                        //初速度求める
                        Vector3 add = selectPosition - SavePosition;
                        syosokudo[i] = add.normalized * moveSpeed;
                        //角度
                        kakudoRadian[i] = Mathf.Atan2(
                            selectPosition.y - SavePosition.y,
                            selectPosition.x - SavePosition.x);
                    }
                }
                //斜方投射、今回ー前回で移動文加算
                float zyuryoku = 0.5f;
                for (int i = 0; i < cutedObject.Count; i++)
                {
                    cutedObject[i].transform.position += new Vector3(
                        syosokudo[i].x * Mathf.Cos(kakudoRadian[i]) * (totalTime + Time.deltaTime)
                        - syosokudo[i].x * Mathf.Cos(kakudoRadian[i]) * totalTime,
                syosokudo[i].y * Mathf.Sin(kakudoRadian[i]) * (totalTime + Time.deltaTime) - 0.5f * zyuryoku * Mathf.Pow(totalTime + Time.deltaTime, 2)
                - syosokudo[i].y * Mathf.Sin(kakudoRadian[i]) * totalTime - 0.5f * zyuryoku * Mathf.Pow(totalTime, 2)
                        , 0);
                }
                totalTime += Time.deltaTime;
                //5秒たったら削除（適当、変更？）
                if (totalTime > 5f)
                {
                    for (int i = 0; i < cutedObject.Count; i++)
                    {
                        Destroy(cutedObject[i]);
                    }
                    Destroy(this.gameObject);
                }
            }
        }



        // Debug.DrawLine(new Vector3(-4.11f, 3.43f, 0), new Vector3(-10.16f, 0.33f, 0), Color.red);
        //Debug.DrawLine(startPosition, endPosition,Color.red);
    }

    //スプライト
    public void SetCut(Vector3 position, Sprite sprite, float time, Vector2 start, Vector2 end)
    {
        gameObject.transform.localPosition = position;
        startPosition = start;
        endPosition = start + end;
        cutTime = time;
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
