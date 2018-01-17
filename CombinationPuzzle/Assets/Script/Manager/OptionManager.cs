using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour {

    // Use this for initialization

    [SerializeField]
    List<Button> SEButton = new List<Button>();

    [SerializeField]
    List<Button> BGMButton = new List<Button>();


    private void Start()
    {
        Debug.Log("on option manager start");
        //0~3 小～大
        SEButton[0].onClick.AddListener(() =>
        {
            Debug.Log("on click");
            AudioManager.Instance.SetSEVolume(VolumeType.None);
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
        });
        SEButton[1].onClick.AddListener(() =>
        {
            AudioManager.Instance.SetSEVolume(VolumeType.Small);
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
        });
        SEButton[2].onClick.AddListener(() =>
        {
            AudioManager.Instance.SetSEVolume(VolumeType.Middle);
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
        });

        SEButton[3].onClick.AddListener(() =>
        {
            AudioManager.Instance.SetSEVolume(VolumeType.Big);
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
        });


        BGMButton[0].onClick.AddListener(() =>
        {
            AudioManager.Instance.SetBGMVolume(VolumeType.None);
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
        });
        BGMButton[1].onClick.AddListener(() =>
        {
            AudioManager.Instance.SetBGMVolume(VolumeType.Small);
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
        });
        BGMButton[2].onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
            AudioManager.Instance.SetBGMVolume(VolumeType.Middle);
        });
        BGMButton[3].onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySE("PAZ_SE_OK");
            AudioManager.Instance.SetBGMVolume(VolumeType.Big);
        });

    }

  

}
