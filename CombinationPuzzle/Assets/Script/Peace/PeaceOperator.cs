using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct POINT
{
    public int X;
    public int Y;
    public POINT(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class PeaceOperator : SingletonMonoBehaviour<PeaceOperator>
{

    [SerializeField]
    POINT stratPosition = new POINT(-718 + 53, 290 + 77);
    public const int onePeaceSize = 148;

    [SerializeField]
    public float downSpeed = 1.5f;


    public void ReSetAllPosition(Dictionary<POINT, Peace> dictionary)
    {
        foreach (var value in dictionary.Values)
        {
            ResetPosition(value);
        }
    }

    public void ResetPosition(Peace peace)
    {
        peace.GetComponent<RectTransform>().anchoredPosition = PeacePosition(peace.point);
    }

    public Vector2 PeacePosition(POINT p)
    {
        return new Vector2(stratPosition.X + p.X * onePeaceSize, stratPosition.Y - p.Y * onePeaceSize);
    }

    public void MovePeace(Vector2 difference, Peace peace)
    {
        peace.GetComponent<RectTransform>().anchoredPosition += difference;
    }

    public void TradeDictionaryPeace(Dictionary<POINT, Peace> dictionary, Peace p1, Peace p2)
    {
        POINT savePoint = p1.point;
        p1.point = p2.point;
        p2.point = savePoint;

        //保存
        Peace addPeace1 = p1;
        Peace addPeace2 = p2;
        dictionary.Remove(p1.point);
        dictionary.Remove(p2.point);

        try
        {
            dictionary.Add(addPeace1.point, addPeace1);
            dictionary.Add(addPeace2.point, addPeace2);
        }
        catch
        {
            MyDebug.DrawError("dictionary.Add Error");
            Debug.LogError("dictionary.Add Error");
        }

    }


    public void AddDrop(Peace peace)
    {
        peace.SetDown(DownPosition(new POINT(peace.point.X, peace.point.Y + 1)));
    }

    public float DownPosition(POINT targetPoint)
    {
        return stratPosition.Y - targetPoint.Y * onePeaceSize;
    }


    public void HidePeace(Peace peace)
    {
        int HideX = -999, HideY = -999;
        peace.point = new POINT(HideX, HideY);
        ResetPosition(peace);
    }

    [SerializeField]
    List<GameObject> cutGameObject = new List<GameObject>();
    public void LineCut(int num)
    {
        Debug.Log("LineCut");
        int[] cutLine = { 0, 2, 4 };

        List<PeaceColor> peaceColorList = new List<PeaceColor>();
        List<PeaceForm> peaceFormList = new List<PeaceForm>();


        for (int i = 0; i < PeaceManager.BoardSizeX; i++)
        {
            if (PeaceManager.Instance.GetPeaceTabel.ContainsKey(new POINT(i, cutLine[num])))
            {
                if (PeaceManager.Instance.GetPeaceTabel[new POINT(i, cutLine[num])].GetPeaceForm==PeaceForm.Triangle)
                {
                    peaceColorList.Add(PeaceManager.Instance.GetPeaceTabel[new POINT(i, cutLine[num])].GetPeaceColor);
                }
                else
                {
                    peaceFormList.Add(PeaceManager.Instance.GetPeaceTabel[new POINT(i, cutLine[num])].GetPeaceForm);
                }


                if (PeaceManager.Instance.GetPeaceTabel[new POINT(i, cutLine[num])].isMatching == true)
                {
                    PeaceJudger.Instance.DeleteTartgetPeace(PeaceManager.Instance.GetPeaceTabel[new POINT(i, cutLine[num])]);
                }

                cutGameObject[num * PeaceManager.BoardSizeX + i].SetActive(true);
                cutGameObject[num * PeaceManager.BoardSizeX + i].GetComponent<SpriteRenderer>().sprite = PeaceManager.Instance.GetPeaceTabel[new POINT(i, cutLine[num])].GetComponent<UnityEngine.UI.Image>().sprite;


                //元を審査）
                PeaceManager.Instance.stockPeaceList.Add(PeaceGenerator.Instance.ChangeForm(PeaceManager.Instance.GetPeaceTabel[new POINT(i, cutLine[num])]));


                //元の位置の上にピースがあったら落ちるように指示する
                for (int countY = cutLine[num] - 1; countY >= 0; countY--)
                {
                    if (PeaceManager.Instance.GetPeaceTabel.ContainsKey(new POINT(i, countY)) == true)
                    {
                        Instance.AddDrop(PeaceManager.Instance.GetPeaceTabel[new POINT(i, countY)]);
                    }
                    else break;
                }


              List<SpriteSlicer2DSliceInfo> sliced = new List<SpriteSlicer2DSliceInfo>();
                SpriteSlicer2D.ExplodeSprite(cutGameObject[num * PeaceManager.BoardSizeX + i], 5, 30
                    , false, ref sliced);
                //何秒で消える的なスクリプト付与
                for (int sliceCount = 0; sliceCount < sliced.Count; sliceCount++)
                {
                    for (int childCount = 0; childCount < sliced[sliceCount].ChildObjects.Count; childCount++)
                    {
                        sliced[sliceCount].ChildObjects[childCount].AddComponent<DestroyObject>();
                    }
                }
                cutGameObject[num * PeaceManager.BoardSizeX + i].SetActive(false);

                PeaceManager.Instance.GetPeaceTabel.Remove(new POINT(i, cutLine[num]));
                HidePeace(PeaceManager.Instance.stockPeaceList[PeaceManager.Instance.stockPeaceList.Count - 1]);//見えない位置に移動
                PeaceManager.Instance.stockPeaceList[PeaceManager.Instance.stockPeaceList.Count - 1].gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            }
        }

        PeaceJudger.Instance.mission.SameDeleteAll(peaceColorList, peaceFormList);
    }

}
