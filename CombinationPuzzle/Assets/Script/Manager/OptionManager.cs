using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{

    // Use this for initialization

    [SerializeField]
    List<Button> SEButton = new List<Button>();

    [SerializeField]
    List<Button> BGMButton = new List<Button>();

    [SerializeField]
    List<Text> bgmList = new List<Text>();

    [SerializeField]
    List<Text> seList = new List<Text>();


    private void Start()
    {
        Debug.Log("on option manager start");
        //0~3 小～大
        SEButton[0].onClick.AddListener(() =>
        {
            Debug.Log("on click");
            AudioManager.Instance.SetSEVolume(VolumeType.None);
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
            ChengeButtoneColor(0, true);
        });
        SEButton[1].onClick.AddListener(() =>
        {
            AudioManager.Instance.SetSEVolume(VolumeType.Small);
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
            ChengeButtoneColor(1, true);
        });
        SEButton[2].onClick.AddListener(() =>
        {
            AudioManager.Instance.SetSEVolume(VolumeType.Middle);
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
            ChengeButtoneColor(2, true);
        });

        SEButton[3].onClick.AddListener(() =>
        {
            AudioManager.Instance.SetSEVolume(VolumeType.Big);
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
            ChengeButtoneColor(3, true);
        });


        BGMButton[0].onClick.AddListener(() =>
        {
            AudioManager.Instance.SetBGMVolume(VolumeType.None);
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
            ChengeButtoneColor(0, false);
        });
        BGMButton[1].onClick.AddListener(() =>
        {
            AudioManager.Instance.SetBGMVolume(VolumeType.Small);
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
            ChengeButtoneColor(1, false);
        });
        BGMButton[2].onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
            AudioManager.Instance.SetBGMVolume(VolumeType.Middle);
            ChengeButtoneColor(2, false);
        });
        BGMButton[3].onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
            ChengeButtoneColor(3, false);
            AudioManager.Instance.SetBGMVolume(VolumeType.Big);
        });

        VolumeType se = AudioManager.Instance.GetSeVolume();




        VolumeType bgm = AudioManager.Instance.GetBgmVolume();

        Debug.Log(se);
        Debug.Log(bgm);
        if((int)se!=1)
        {
            switch(se)
            {
                case VolumeType.None:
                    ChengeButtoneColor(0, true);
                    break;
                case VolumeType.Small:
                    ChengeButtoneColor(1, true);
                    break;
                case VolumeType.Middle:
                    ChengeButtoneColor(2, true);
                    break;
                case VolumeType.Big:
                    ChengeButtoneColor(3, true);
                    break;
            }

        }
        if ((int)bgm != 1)
        {
            switch (bgm)
            {
                case VolumeType.None:
                    ChengeButtoneColor(0, false);
                    break;
                case VolumeType.Small:
                    ChengeButtoneColor(1, false);
                    break;
                case VolumeType.Middle:
                    ChengeButtoneColor(2, false);
                    break;
                case VolumeType.Big:
                    ChengeButtoneColor(3, false);
                    break;
            }

        }

        //switch(se)
        //{
        //    case VolumeType.:
        //        break;

        //}


    }

    private void ChengeButtoneColor(int num, bool isSe)
    {
        if (isSe == true)
        {
            for (int i = 0; i < seList.Count; i++)
            {
                seList[i].color = Color.black;
            }

            seList[num].color = Color.red;
        }
        else
        {
            for (int i = 0; i < bgmList.Count; i++)
            {
                bgmList[i].color = Color.black;
            }

            bgmList[num].color = Color.red;
        }

    }




}
