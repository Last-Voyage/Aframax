/**********************************************************************************************************************
// File Name :         SteamAchievements.cs
// Author :            Andrew Stapay
// Creation Date :     7/9/2025
//
// Brief Description : Helper for achievements that require the player to shoot an object
**********************************************************************************************************************/
using UnityEngine;

/// <summary>
/// Helper script for achievements that require the player to shoot an object
/// </summary>
public class ShootingAchievement : MonoBehaviour
{
    /// <summary>
    /// An enum for the different types of shooting achievements
    /// </summary>
    public enum ShootingAchievementType
    {
        HEADSHOT,
        DONT_BREAK_IT,
        FISH
    }

    // The achievement that shooting this object will unlock
    [SerializeField] private ShootingAchievementType _achievementType;

    /// <summary>
    /// Called when a collider triggers this object
    /// </summary>
    /// <param name="other"> Information about the other collider in the collision </param>
    private void OnTriggerEnter(Collider other)
    {
        // See if this was hit by a harpoon
        if (other.TryGetComponent<HarpoonDamage>(out HarpoonDamage damage))
        {
            // if this is a statue, we need to do a special something
            if (_achievementType == ShootingAchievementType.HEADSHOT)
            {
                // So here's the deal
                // We want to be able to specifically hit the head of the statue
                // However, the prefab for the statue was set up a little weird, and I don't want to add another collider
                // So we need to do some math
                // Thankfully, all the information we need should be in the CapsuleCollider
                if (gameObject.TryGetComponent<CapsuleCollider>(out CapsuleCollider capsuleCollider))
                {
                    // To find the rough world position of the head, we need to take the position, located at the feet
                    // Since it is located at the feet, the height of the collider tells us where the top of the head is
                    // Finally, to get a lower bound, since the ends of a capsule are hemispheres,
                    // We can subtract the radius to find where the top hemisphere meets the cylinder of the capsule
                    var headYLimit = transform.position.y + capsuleCollider.height - capsuleCollider.radius;

                    // Next, we get the rough position of the collision
                    var collisionPoint = other.ClosestPoint(transform.position);

                    // Finally, compare the two, and see if we hit above the mark
                    if (collisionPoint.y >= headYLimit)
                    {
                        SteamAchievements.Instance.AchievementShot(_achievementType, this.gameObject);
                    }
                }  
            }
            else // otherwise, we can just send it away
            {
                SteamAchievements.Instance.AchievementShot(_achievementType, this.gameObject);
            }
        }
    }
}
