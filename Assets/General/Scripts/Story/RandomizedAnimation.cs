/******************************************************************************
// File Name:       RandomizedAnimation.cs
// Author:          Ryan Swanson
// Creation Date:   January 30th, 2025
//
// Description:     Allows for animations to be randomly played in
******************************************************************************/

using PrimeTween;
using UnityEngine;

/// <summary>
/// Provides the different types of ways of determining randomized animations to be played.
/// </summary>
public enum ERandomizedAnimationType
{
    Random,
    Order
};

/// <summary>
/// Allows for animations to be played with an element of randomness
/// System is created to avoid needing duplicated functionality
/// </summary>
public class RandomizedAnimation : MonoBehaviour
{
    [Tooltip("The minimum amount of time for it to work before plays the next animation")]
    [SerializeField] private float _minAnimationDelay;
    [Tooltip("The maximum amount of time for it to work before plays the next animation")]
    [SerializeField] private float _maxAnimationDelay;

    [Space]
    [Tooltip("The array of animation names to play")]
    [SerializeField] private string[] _animationNameList;
    private int _currentAnimationPosition = 0;

    [Space]
    [Tooltip("Determines if the animation to play is random from the array or in the order of the array")]
    [SerializeField] private ERandomizedAnimationType _randomAnimType;
    
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        StartRandomDelay();
    }

    /// <summary>
    /// Starts the random delay before playing the animation
    /// </summary>
    private void StartRandomDelay()
    {
        if (!gameObject.activeInHierarchy) return;

        Tween.Delay(this, Random.Range(_minAnimationDelay, _maxAnimationDelay), DetermineNextAnimation);
    }

    /// <summary>
    /// Determines which animation from the array should be played next
    /// </summary>
    private void DetermineNextAnimation()
    {
        //Checks if the animation is determined randomly
        if(_randomAnimType == ERandomizedAnimationType.Random)
        {
            PlayAnimationTrigger(Random.Range(0, _animationNameList.Length));
        }
        //Determines if the animations are played in order
        //Could be an else but I prefer this in case another enum state is added
        else if (_randomAnimType == ERandomizedAnimationType.Order)
        {
            PlayAnimationTrigger(_currentAnimationPosition);
            //Progresses in the array
            _currentAnimationPosition++;
            //Resets when out of bounds
            if(_currentAnimationPosition >= _animationNameList.Length)
            {
                _currentAnimationPosition = 0;
            }
        }
    }

    /// <summary>
    /// Plays an animation from the array and reset the loop
    /// </summary>
    /// <param name="animPos"></param>
    private void PlayAnimationTrigger(int animPos)
    {
        _animator.SetTrigger(_animationNameList[animPos]);
        StartRandomDelay();
    }
}
