using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutMission : MonoBehaviour
{

    [SerializeField]
    GameObject EffctPool = null;

    //SDキャラの動き
    [SerializeField, Header("SDキャラの設定")]
    Vector2 StartPos;
    [SerializeField]
    Vector2 EndPos;
    [SerializeField]
    float ShiftY = 252;
    [SerializeField]
    float CharacterSpeed = 1.0f;

    [SerializeField]
    CutCharacter SDcharacterPrefab = null;
    [SerializeField]
    GameObject CharacterMoveParticle = null;


    //ピースカットの座標
    [SerializeField, Header("カットされるピースの設定")]
    CutObject CutPrefab = null;

    [SerializeField]
    Vector2 cutStartPostion;
    [SerializeField]
    Vector2 CutEndVector;
    [SerializeField]
    float CutShiftY = 250;
    [SerializeField]//何秒後にカットするか
    float cutTime = 1;
    [SerializeField]
    Vector2 CutPeacePosition;
    [SerializeField]
    float CutPeacePositionShiftY = 250;

    [SerializeField]
    GameObject cutPeacePool = null;
    [SerializeField]
    float PurpuseSize = 9.75f;
    [SerializeField]
    float ColorPeaceSize = 9.75f;

    private void SetCutObject(Sprite AllSprite, int num, PeaceColor peaceColor, PeaceForm peaceForm)
    {
        CutObject cutObject = Instantiate(CutPrefab);
        cutObject.gameObject.transform.SetParent(cutPeacePool.transform, false);

        if (peaceColor != PeaceColor.None)
        {
            cutObject.transform.localScale = new Vector3(ColorPeaceSize, ColorPeaceSize, 1);
            cutObject.SetCut(new Vector3(CutPeacePosition.x, CutPeacePosition.y - (num * CutPeacePositionShiftY), 0), PeaceGenerator.I.PeaceSprites[(int)peaceColor],
                cutTime, new Vector2(cutStartPostion.x, cutStartPostion.y - (CutShiftY * num)), CutEndVector);

        }
        else if (peaceForm != PeaceForm.None)
        {
            cutObject.transform.localScale = new Vector3(ColorPeaceSize, ColorPeaceSize, 1);
            cutObject.SetCut(new Vector3(CutPeacePosition.x, CutPeacePosition.y - (num * CutPeacePositionShiftY), 0), PeaceGenerator.I.PeaceSprites[(int)PeaceColor.None + (int)peaceForm - 1],
                cutTime, new Vector2(cutStartPostion.x, cutStartPostion.y - (CutShiftY * num)), CutEndVector);

        }
        else//おっきいのだけサイズちがう
        {
            cutObject.transform.localScale = new Vector3(PurpuseSize, PurpuseSize, 1);
            cutObject.SetCut(new Vector3(CutPeacePosition.x, CutPeacePosition.y - (num * CutPeacePositionShiftY), 0), AllSprite,
                cutTime, new Vector2(cutStartPostion.x, cutStartPostion.y - (CutShiftY * num)), CutEndVector);
        }
    }

    private void SetCutCharacter(int num)
    {
        CutCharacter chara = Instantiate(SDcharacterPrefab, EffctPool.transform);
        chara.SetCharacter(new Vector2(StartPos.x, StartPos.y - (num * ShiftY)), EndPos, CharacterSpeed);
    }

    public void SetCutEffect(Sprite AllSprite, int num, PeaceColor peaceColor, PeaceForm peaceForm)
    {
        SetCutCharacter(num);
        SetCutObject(AllSprite, num, peaceColor, peaceForm);
    }
    public void Test(int num)
    {
        SetCutCharacter(num);
        SetCutObject(null, num, PeaceColor.Blue,PeaceForm.None);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //if (nowTime < speed)
        //{
        //    nowTime += Time.deltaTime;
        //    gameObject.GetComponent<RectTransform>().anchoredPosition = StartPos[deleteNum] + addVector * nowTime;
        //    //EffectManager.I.PlayEffect(gameObject.GetComponent<RectTransform>().anchoredPosition,PeaceColor.Blue.DisplayName());
        //}
        //else
        //{
        //    Destroy(this.gameObject);
        //}

    }
}
