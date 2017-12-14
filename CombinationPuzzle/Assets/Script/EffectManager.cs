using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{

    public static EffectManager I = null;

    private const float FromtY = -5;

    [SerializeField]
    List<GameObject> deletePeaceEffect = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        I = this;
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void StartEffect(Vector3 generationPotision, string effectName)
    {
        GameObject obj = Instantiate(deletePeaceEffect[0]);
        obj.transform.position = new Vector3(generationPotision.x,generationPotision.y,FromtY);

    }

}
