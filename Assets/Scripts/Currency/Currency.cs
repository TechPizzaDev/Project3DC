using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a currency pickup in the game.
/// It holds the amount of dollars that will be acquired when the player collides with it.
/// </summary>
/// 
public class Currency : MonoBehaviour
{
    public int dollars;

    /// <summary>
    /// This method is called when another collider enters the trigger collider attached to this object.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Checks if the game object and the other game object have the required components
        // to handle currency management.
        if (gameObject != null && other.gameObject.TryGetComponent(out CurrencyManager manager))
        {
            // Calls the AquireDollars() method of the CurrencyManager to add the acquired dollars.
            // Then destroys the currency pickup game object.
            manager.AquireDollars(dollars);
            Destroy(gameObject);
        }
    }
}
