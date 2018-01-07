using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectManager : SingletonMonoBehaviour<EffectManager>
{

    private const float FromtY = -5;

    [SerializeField]
    List<GameObject> deletePeaceEffect = new List<GameObject>();
    [SerializeField]
    GameObject generationEffectPrefab = null;
    [SerializeField]
    GameObject effectPool = null;

    public void PlayEffect(Vector3 generationPotision, string effectName)
    {
        GameObject obj = null; 
       
        switch (effectName)
        {
            case "赤":
                obj=Instantiate(deletePeaceEffect[(int)PeaceColor.Red]);
                break;
            case "青":
                obj = Instantiate(deletePeaceEffect[(int)PeaceColor.Blue]);
                break;
            case "黄":
                obj = Instantiate(deletePeaceEffect[(int)PeaceColor.Yellow]);
                break;
            case "緑":
                obj = Instantiate(deletePeaceEffect[(int)PeaceColor.Green]);
                break;
            case "紫":
                obj = Instantiate(deletePeaceEffect[(int)PeaceColor.Perple]);
                break;
            case "橙":
                obj = Instantiate(deletePeaceEffect[(int)PeaceColor.Orange]);
                break;
            case "黒":
                obj = Instantiate(deletePeaceEffect[(int)PeaceColor.None]);
                break;
            case "白":
                obj = Instantiate(deletePeaceEffect[(int)PeaceColor.None+1]);
                break;
            case "生成":
                obj = Instantiate(generationEffectPrefab);
                break;

            default:
                Debug.LogError("エフェクト名が指定されていません");
                return;
                break;

        }
        //obj.transform.SetParent(effectPool.transform, false);
        obj.transform.position = new Vector3(generationPotision.x, generationPotision.y, FromtY);
    
    }

}
