using System.Collections;
using System.Collections.Generic;
using UnityEngine;



static class PeaceColorExt
{
    // Gender に対する拡張メソッドの定義
    public static string DisplayName(this PeaceColor peaceColor)
    {
        string[] names = { "赤", "青", "黄", "緑", "紫", "橙" };
        return names[(int)peaceColor];
    }
}


public class PeaceGenerator : SingletonMonoBehaviour<PeaceGenerator>
{

    [SerializeField]
    public GameObject peacePrefab = null;

    [SerializeField]
    GameObject PeacePool = null;


    //カラーの後は形
    [SerializeField]
    public List<Sprite> PeaceSprites = new List<Sprite>();

    
    public void Start()
    {
        generationFrequencyTime = InitialGenerationSpeed;
    }

    //トローゼ方式初期化
    public void AllGeneration()
    {
        const int StartY = 3;//下からこの番号まで入れておく、被り無し

        PeaceColor addPeaceType;
        for (int countY = PeaceManager.BoardSizeY - StartY; countY < PeaceManager.BoardSizeY; countY++)
        {
            for (int countX = 0; countX < PeaceManager.BoardSizeX; countX++)
            {
                addPeaceType = (PeaceColor)UnityEngine.Random.Range(0, (int)PeaceColor.None);
                Peace peace = Instantiate(peacePrefab).AddComponent<TrianglePeace>();
                peace.peaceColor = addPeaceType;
                peace.point = new POINT(countX, countY);
                if (PeaceJudger.Instance.CurrentDeletable(PeaceManager.Instance.GetPeaceTabel, peace))
                {
                    Destroy(peace.gameObject);
                    countX--;
                    continue;
                }
                peace.transform.SetParent(PeacePool.transform, false);
                peace.SetSprite(PeaceSprites[(int)peace.peaceColor]);
                PeaceManager.Instance.GetPeaceTabel.Add(peace.point, peace);

            }

        }


    }


    //public void AllGeneration(Dictionary<POINT, Peace> dictionary, PeaceJudger peaceJudger)
    //{
    //    PeaceColor addPeaceType;
    //    for (int i = 0; i < PeaceManager.BoardSizeY; i++)
    //    {
    //        for (int j = 0; j < PeaceManager.BoardSizeX; j++)
    //        {
    //            addPeaceType = (PeaceColor)UnityEngine.Random.Range(0, (int)PeaceColor.None);
    //            Peace peace = Instantiate(peacePrefab).AddComponent<TrianglePeace>();
    //            peace.peaceColor = addPeaceType;
    //            peace.point = new POINT(j, i);
    //            if (PeaceJudger.Instance.CurrentDeletable(dictionary, peace))
    //            {
    //                Destroy(peace.gameObject);
    //                j--;
    //                continue;
    //            }
    //            peace.transform.SetParent(PeacePool.transform, false);
    //            peace.SetSprite(PeaceSprites[(int)peace.peaceColor]);
    //            dictionary.Add(peace.point, peace);
    //        }
    //    }
    //}

    //TODO:直す
    public Peace ChangeNextForm(Dictionary<POINT, Peace> peaceTable, Peace changePeace)
    {

        peaceTable.Remove(changePeace.point);
        Peace newPeace = null;
        changePeace.SetSprite(PeaceSprites[(int)changePeace.GetNextPeaceForm + (int)PeaceColor.None - 1]);
        if (changePeace.GetNextPeaceForm == PeaceForm.Square)
            newPeace = changePeace.gameObject.AddComponent<SquarePeace>();
        else if (changePeace.GetNextPeaceForm == PeaceForm.Pentagon)
            newPeace = changePeace.gameObject.AddComponent<PentagonPeace>();
        newPeace.point = changePeace.point;

        peaceTable.Add(newPeace.point, newPeace);
        Destroy(changePeace);
        return newPeace;
    }

    //TODO:いったん三角のみにする
    public Peace ChangeForm(Peace changePeace)
    {
        PeaceManager.Instance.GetPeaceTabel.Remove(changePeace.point);
        Peace newPeace = null;
        newPeace = changePeace.gameObject.AddComponent<TrianglePeace>();
        changePeace.SetSprite(PeaceSprites[(int)changePeace.peaceColor]);
        newPeace.point = changePeace.point;

        PeaceManager.Instance.GetPeaceTabel.Add(newPeace.point, newPeace);
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

    public bool SetPeaceList(Peace peace, POINT newPoint)
    {
        //Debug.Log("次は X=" + newPoint.X + " Y=" + newPoint.Y + "に代入");
        //Debug.Log("現在 X=" + peace.point.X + " Y=" + peace.point.Y + "に代入");

        if (PeaceManager.Instance.GetPeaceTabel.ContainsKey(newPoint)) return false;
        //ポイント更新
        Peace p = peace;
        PeaceManager.Instance.GetPeaceTabel.Remove(peace.point);
        p.point = newPoint;
        //リストにセットしなおす
        PeaceManager.Instance.GetPeaceTabel.Add(newPoint, p);
        //    Debug.Log("セットしなおし"+ "次は X=" + newPoint.X + " Y=" + newPoint.Y + "に代入しました");

        return true;
    }


    [SerializeField, Header("初期生成 速度")]
    float InitialGenerationSpeed = 3.0f;
    //行ったん確認用に早く
    [SerializeField, Header("何秒で生成頻度が早くなるか")]
    float SpeedUpInterval = 5.0f;
    [SerializeField, Header("何秒早くなるか")]
    float SpeedUpTime = 0.5f;

    float generationFrequencyTime = 2;//初期化する

    float generationTotalTime = 0f;
    float speedUpTotalTime = 0f; 

    private void GenerationDropPeace()
    {
        generationTotalTime += Time.deltaTime;
        speedUpTotalTime += Time.deltaTime;

        if (generationTotalTime > generationFrequencyTime)
        {
            //
            //生成
            AddToTopPeace(PeaceManager.Instance.GetPeaceTabel, Peace )
            //ランダムに落下させる
            generationTotalTime = 0;
        }
        if(speedUpTotalTime>SpeedUpInterval)
        {
            speedUpTotalTime = 0;
            generationFrequencyTime -= SpeedUpTime;
           
        }
    }

    void Update()
    {
        GenerationDropPeace();
    }
}
