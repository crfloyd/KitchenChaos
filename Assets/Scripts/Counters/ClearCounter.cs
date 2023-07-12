using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if(!HasKitchenObject())
        {
            // counter does not have kitchen object
            if(player.HasKitchenObject())
            {
                // player has kitchen object. Give to counter
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        } else
        {
            // counter has kitchen object
            if(player.HasKitchenObject())
            {
                // player has kitchen object and counter has kitchen object.
            } else
            {
                // player does not have kitchen object, but counter does. Give to player
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
