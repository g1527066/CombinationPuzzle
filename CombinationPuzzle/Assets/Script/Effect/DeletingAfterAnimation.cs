using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeletingAfterAnimation : MonoBehaviour {


    [SerializeField]
    Animator animator = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            Destroy(this.gameObject);

        Debug.Log(GetComponent<RectTransform>().anchoredPosition);
	}
}
