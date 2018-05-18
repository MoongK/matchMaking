using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitreaction : MonoBehaviour {

    Animator anim;

	void Start () {
        anim = GetComponent<Animator>();
	}


    public void React(string reactionType)
    {
        print(reactionType);
        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Invincible"))
        {
            anim.SetTrigger("Reaction" + reactionType);
        }
    }
}
