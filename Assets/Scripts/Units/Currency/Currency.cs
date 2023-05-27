using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency : MonoBehaviour
{
    public int dollars;

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject != null && other.gameObject.TryGetComponent(out CurrencyManager manager))
        {
            manager.AquireDollars(dollars);
            Destroy(gameObject);
        }
    }
}
