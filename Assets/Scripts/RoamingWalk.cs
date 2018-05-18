using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoamingWalk : StateMachineBehaviour {
    public int currentPoint = 0;

    List<Transform> wayPoints;
    Vector3 nextPoint;

    NavMeshAgent nm;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        wayPoints = new List<Transform>();
        foreach (Transform t in GameObject.Find("WayPoints").transform)
            wayPoints.Add(t);

        if (currentPoint >= wayPoints.Count)
            currentPoint = 0;
        if (wayPoints.Count >= 2 && currentPoint < wayPoints.Count)
            nextPoint = wayPoints[currentPoint].transform.position;

        nm = animator.GetComponent<NavMeshAgent>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (wayPoints.Count >= 2)
        {
            float dist = Vector3.Distance(animator.transform.position, nextPoint);
            if (dist < 0.1f)
            {
                currentPoint++;
                if (currentPoint >= wayPoints.Count)
                    currentPoint = 0;
                nextPoint = wayPoints[currentPoint].transform.position;
            }
            nm.destination = nextPoint;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
