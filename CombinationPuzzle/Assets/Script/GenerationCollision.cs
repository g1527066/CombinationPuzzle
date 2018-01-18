using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationCollision : MonoBehaviour {

    //poolのサイズは表示するサイズにする（それを元に判定作成
    //[SerializeField]
    //RectTransform CollisionPool = null;

    [SerializeField]
    PointCollision PointCollisionPrefab = null;

    List<PointCollision> collistionList = new List<PointCollision>();

  //  public void GenerationCollistion()
    public void GenerationCol()
    {
        //XYが同じサイズであること前提
        float size = GetComponent<RectTransform>().sizeDelta.x / PeaceManager.BoardSizeX;
        for (int Ycount=0;Ycount<PeaceManager.BoardSizeY;Ycount++)
        {
            for (int Xcount = 0; Xcount < PeaceManager.BoardSizeX; Xcount++)
            {
                PointCollision pointCollition = Instantiate(PointCollisionPrefab);
                pointCollition.SettingCollistion(new POINT(Xcount,Ycount),size);
                //位置設定
                pointCollition.transform.SetParent(transform, false);
                pointCollition.GetComponent<RectTransform>().anchoredPosition =
                    PeaceOperator.Instance.PeacePosition(pointCollition.point);

            //    pointCollition.GetComponent<BoxCollider2D>().enabled = false;
                collistionList.Add(pointCollition);

            }
        }
    }
}
