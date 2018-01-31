using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MissionType
{
    Delete,
    SameDelete,//同時
    CountDelete,//回数
    MakePeace,
    None,
}

[CreateAssetMenu(menuName = "CreateData/MissionData", fileName = "MissionData")]
public class MissionData : ScriptableObject {

    [System.Serializable]
    public class MissionParams
    {
        public string MissionName;
        public int LmiteTime;
        public float  FallFrequency;//初期落下頻度
        public float SpeedUpTime;//何秒で早くなるか
        public float minusTime;//何秒引くか

        public List<MissionDetails> MissionList = new List<MissionDetails>();
    }

    [System.Serializable]
    public class MissionDetails
    {
        public MissionType missionType;
        public PeaceColor peaceColor;
        public PeaceForm peaceForm;
        public int number;
    }

    public List<MissionParams> Elements = new List<MissionParams>();
}
