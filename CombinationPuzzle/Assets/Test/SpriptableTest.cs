using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//スクリプタブルオブジェクトのテスト
[CreateAssetMenu(menuName = "MyGame/Create ParameterTable", fileName = "ParameterTable")]
public class SpriptableTest : ScriptableObject {

    [System.Serializable]
    public class Params
    {
        public PeaceColor peace;
        public string ContentsName;
        public string ContentsText;
        public List<MissionDetails> MissionList = new List<MissionDetails>();
    }
    public List<Params> Elements = new List<Params>();

    [System.Serializable]
    public class MissionDetails
    {
        public PeaceColor peace;
        public string ContentsName;
    }
}
