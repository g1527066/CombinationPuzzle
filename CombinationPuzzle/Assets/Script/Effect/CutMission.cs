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
    float StayTimingTime = 1f;
    [SerializeField]
    float StayTime = 1f;

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
    List<AnimationImage> BigZanEffectList = new List<AnimationImage>();
    [SerializeField]
    List<GameObject> zanAnimator = new List<GameObject>();

    private void SetCutObject(Sprite AllSprite, int num, PeaceColor peaceColor, PeaceForm peaceForm)
    {
        CutObject cutObject = Instantiate(CutPrefab);
        cutObject.gameObject.transform.SetParent(cutPeacePool.transform, false);
        int a = (int)PeaceColor.None + (int)peaceForm - 1;


        if (peaceColor != PeaceColor.None)
        {
            cutObject.SetCut(new Vector3(CutPeacePosition.x, CutPeacePosition.y - (num * CutPeacePositionShiftY), 0), PeaceGenerator.Instance.PeaceSprites[(int)peaceColor],
                cutTime, new Vector2(cutStartPostion.x, cutStartPostion.y - (CutShiftY * num)), CutEndVector);

        }
        else if (peaceForm != PeaceForm.None)
        {
            cutObject.SetCut(new Vector3(CutPeacePosition.x, CutPeacePosition.y - (num * CutPeacePositionShiftY), 0), PeaceGenerator.Instance.PeaceSprites[(int)PeaceColor.None + (int)peaceForm - 1],
                cutTime, new Vector2(cutStartPostion.x, cutStartPostion.y - (CutShiftY * num)), CutEndVector);

        }
        else
        {
            cutObject.SetCut(new Vector3(CutPeacePosition.x, CutPeacePosition.y - (num * CutPeacePositionShiftY), 0), AllSprite,
                cutTime, new Vector2(cutStartPostion.x, cutStartPostion.y - (CutShiftY * num)), CutEndVector);
        }
    }

    private void SetCutCharacter(int num)
    {
        StartCoroutine(StayProsess(num));
        CutCharacter chara = Instantiate(SDcharacterPrefab, EffctPool.transform);
        chara.SetCharacter(new Vector2(StartPos.x, StartPos.y - (num * ShiftY)), EndPos, CharacterSpeed, StayTimingTime, StayTime, BigZanEffectList[num], num);

    }

    public void SetCutEffect(Sprite AllSprite, int num, PeaceColor peaceColor, PeaceForm peaceForm)
    {
        SetCutCharacter(num);
        SetCutObject(AllSprite, num, peaceColor, peaceForm);
    }
    public void Test(int num)
    {
        SetCutCharacter(num);
        SetCutObject(null, num, PeaceColor.Blue, PeaceForm.None);
    }

    private IEnumerator StayProsess(int num)
    {
        Debug.Log("StayProsess");
        yield return new WaitForSeconds(0.3f);
        zanAnimator[num].gameObject.SetActive(true);
        zanAnimator[num].GetComponent<Animator>(). Play("Mission_Slise_Animation");
        yield return new WaitForSeconds(0.5f);
        zanAnimator[num].gameObject.SetActive(false);
    }
}
