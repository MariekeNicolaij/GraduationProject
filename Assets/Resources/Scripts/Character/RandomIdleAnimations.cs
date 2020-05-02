using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class RandomIdleAnimations : MonoBehaviour
{
    Animator animator;
    public AnimationState test;

    AnimationClip[] randomIdleAnimations;
    int oldAnimationIndex = 99, currentAnimationIndex = 99;

    float animationTime = 0;
    [Range(1, 5)]
    public int loopMultiplier = 3;


    void Start()
    {
        animator = GetComponent<Animator>();
        randomIdleAnimations = animator.runtimeAnimatorController.animationClips;
        NextAnimation();
    }

    void Update()
    {
        // So that it returns to its initial state before it can enter a new state
        animationTime -= Time.deltaTime;
        animator.SetFloat("animationTime", animationTime);
    }

    void NextAnimation()
    {
        oldAnimationIndex = currentAnimationIndex;

        while (currentAnimationIndex == oldAnimationIndex)
            currentAnimationIndex = Random.Range(0, randomIdleAnimations.Length);

        AnimationClip clip = randomIdleAnimations[currentAnimationIndex];
        animationTime = (clip.isLooping) ? clip.length * loopMultiplier : clip.length; // If animation has loop play it a bit longer

        animator.SetInteger("randomIndex", currentAnimationIndex);

        Invoke("NextAnimation", animationTime);
    }
}
