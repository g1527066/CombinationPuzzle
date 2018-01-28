using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCharacter : MonoBehaviour
{
    [SerializeField]
    Animator animator = null;




    // Use this for initialization
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //http://rikoubou.hatenablog.com/entry/2016/01/29/163518
            Vector2 tapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collition2d = Physics2D.OverlapPoint(tapPoint);
            if (collition2d)
            {
                GameObject result = collition2d.transform.gameObject;
                Debug.Log("clickCharacter");
                animator.CrossFade("Wink_Animation", 0);

            }
        }
    }
}
