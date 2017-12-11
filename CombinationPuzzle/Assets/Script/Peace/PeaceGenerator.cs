using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaceGenerator : MonoBehaviour
{

    [SerializeField]
    GameObject peacePrefab = null;

    [SerializeField]
    GameObject PeacePool = null;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void AllGeneration(Dictionary<POINT, Peace> dictionary,PeaceJudger peaceJudger)
    {
        PeaceColor addPeaceType;
        for (int i = 0; i < PeaceManager.BoardSizeY; i++)
        {
            for (int j = 0; j < PeaceManager.BoardSizeX; j++)
            {
                 addPeaceType = (PeaceColor)UnityEngine.Random.Range(0, (int)PeaceColor.None);
                Peace peace = Instantiate(peacePrefab).AddComponent<TrianglePeace>();
                peace.peaceType = addPeaceType;
                peace.point = new POINT(j, i);
                if(peaceJudger.CurrentDeletable(dictionary,peace))
                {
                    continue;
                    j--;
                }
                peace.transform.SetParent(PeacePool.transform, false);


            }
        }
    }

    //    //newPeace.GetComponent<RectTransform>().anchoredPosition = new Vector2(stratPosition.X + j * onePeaceSize, stratPosition.Y - i * onePeaceSize);
    //    //newPeace.SetSprite(PeaceSprites[(int)newPeace.peaceType]);
    //    //peaceTable.Add(newPeace.point, newPeace);
    //}
    //else
    //    j--;



}
