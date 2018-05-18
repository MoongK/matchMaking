using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControl : MonoBehaviour {
    public bool ikActive = true;
    public Transform rightHandTarget;
    public Transform leftHandTarget;
    public Transform lookTarget;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (ikActive)
        {
            anim.SetLookAtWeight(1);
            anim.SetLookAtPosition(lookTarget.position);

            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);

            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
        }
        else
        {
            anim.SetLookAtWeight(0);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }
    }
}
