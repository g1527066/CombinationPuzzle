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
    void Start()
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
                if (peaceJudger.CurrentDeletable(dictionary, peace))
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

    public void ChangeForm(Dictionary<POINT, Peace> peaceTable, Peace changePeace)
    {
        peaceTable.Remove(changePeace.point);
        Peace newPeace = null;
        changePeace.SetSprite(PeaceSprites[(int)changePeace.nextPeaceForm + (int)PeaceColor.None - 2]);
        if (changePeace.nextPeaceForm == PeaceForm.Square)
            newPeace= changePeace.gameObject.AddComponent<SquarePeace>();
        else
            newPeace = changePeace.gameObject.AddComponent<PentagonPeace>();
        newPeace.point = changePeace.point;

        peaceTable.Add(newPeace.point, newPeace);
        Destroy(changePeace);
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
}
