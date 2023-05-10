using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endReload : MonoBehaviour
{
    private PlayerAction playerAction;

    private void Start()
    {
        playerAction = GetComponentInParent<PlayerAction>();
    }
    private void EndReload()
    {
        playerAction.EndReload();
    }
}
