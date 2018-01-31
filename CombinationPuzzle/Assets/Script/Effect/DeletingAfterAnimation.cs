using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeletingAfterAnimation : MonoBehaviour {

    [SerializeField]
    Animator animator = null;

	void Update () {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)//1以上だとアニメは終了している
            Destroy(this.gameObject);
	}
}
