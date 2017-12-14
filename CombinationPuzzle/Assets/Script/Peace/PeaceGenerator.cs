using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaceGenerator : MonoBehaviour
{

    public static PeaceGenerator I = null;


    [SerializeField]
    public GameObject peacePrefab = null;

    [SerializeField]
    GameObject PeacePool = null;


    //カラーの後は形
    [SerializeField]
    List<Sprite> PeaceSprites = new List<Sprite>();




    // Use this for initialization
    void Awake()
    {
        I = this;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void AllGeneration(Dictionary<POINT, Peace> dictionary, PeaceJudger peaceJudger)
    {
        PeaceColor addPeaceType;
        for (int i = 0; i < PeaceManager.BoardSizeY; i++)
        {
            for (int j = 0; j < PeaceManager.BoardSizeX; j++)
            {
                addPeaceType = (PeaceColor)UnityEngine.Random.Range(0, (int)PeaceColor.None);
                Peace peace = Instantiate(peacePrefab).AddComponent<TrianglePeace>();
                peace.peaceColor = addPeaceType;
                peace.point = new POINT(j, i);
                if (PeaceJudger.I.CurrentDeletable(dictionary, peace))
                {
                    j--;
                    continue;
                }
                peace.transform.SetParent(PeacePool.transform, false);
                peace.SetSprite(PeaceSprites[(int)peace.peaceColor]);
                dictionary.Add(peace.point, peace);
            }
        }
    }

    //TODO:直す
    public Peace ChangeNextForm(Dictionary<POINT, Peace> peaceTable, Peace changePeace)
    {

        peaceTable.Remove(changePeace.point);
        Peace newPeace = null;
        changePeace.SetSprite(PeaceSprites[(int)changePeace.GetNextPeaceForm + (int)PeaceColor.None - 1]);
        if (changePeace.GetNextPeaceForm == PeaceForm.Square)
            newPeace= changePeace.gameObject.AddComponent<SquarePeace>();
        else if(changePeace.GetNextPeaceForm == PeaceForm.Pentagon)
            newPeace = changePeace.gameObject.AddComponent<PentagonPeace>();
        newPeace.point = changePeace.point;

        peaceTable.Add(newPeace.point, newPeace);
        Destroy(changePeace);
        return newPeace;
    }

    //TODO:いったん三角のみにする
    public Peace ChangeForm(Peace changePeace)
    {
        PeaceManager.I.GetPeaceTabel.Remove(changePeace.point);
        Peace newPeace = null;
        newPeace = changePeace.gameObject.AddComponent<TrianglePeace>();
        changePeace.SetSprite(PeaceSprites[(int)changePeace.peaceColor]);
        newPeace.point = changePeace.point;
        
        PeaceManager.I.GetPeaceTabel.Add(newPeace.point, newPeace);
        Destroy(changePeace);
        return newPeace;
    }

    public void AddToTopPeace(Dictionary<POINT, Peace> peaceTable, Peace deletePeace)
    {
        for (int i = -1; i >= -PeaceManager.BoardSizeY; i--)
        {
            if (peaceTable.ContainsKey(new POINT(deletePeace.point.X, i)) == false)
            {
                //初期化
                deletePeace.Initialization();
                deletePeace.SetNewType();
                deletePeace.SetSprite(PeaceSprites[(int)deletePeace.peaceColor]);
                //point
                deletePeace.point = new POINT(deletePeace.point.X, i);
                //テーブルに追加
                peaceTable.Add(deletePeace.point, deletePeace);

                return;
            }
        }
        Debug.LogError("上に追加できません");
    }

    public bool SetPeaceList(Peace peace,POINT newPoint)
    {
        //Debug.Log("次は X=" + newPoint.X + " Y=" + newPoint.Y + "に代入");
        //Debug.Log("現在 X=" + peace.point.X + " Y=" + peace.point.Y + "に代入");

        if (PeaceManager.I.GetPeaceTabel.ContainsKey(newPoint)) return false;
        //ポイント更新
        Peace p = peace;
        PeaceManager.I.GetPeaceTabel.Remove(peace.point);
        p.point = newPoint;
        //リストにセットしなおす
        PeaceManager.I.GetPeaceTabel.Add(newPoint,p);
        Debug.Log("セットしなおし"+ "次は X=" + newPoint.X + " Y=" + newPoint.Y + "に代入しました");

        return true;
    }
}
