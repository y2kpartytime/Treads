using UnityEngine;
using System.Collections;

public class DoorState : StateMachineBehaviour
{
    private AudioSource[] audioSources;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioSources = animator.GetComponents<AudioSource>();
        audioSources[0].Play();
        audioSources[0].pitch = 1.25f;
        
    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
    
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioSources[0].Play();
        audioSources[0].pitch = 0.7f;
    }

}

