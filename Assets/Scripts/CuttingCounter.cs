using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CuttingCounter : BaseCounter
{

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // counter does not have kitchen object
            if (player.HasKitchenObject())
            {
                // if there are no cutting recipes for this kitchen object, return
                if (!HasCuttableKitchenObject(player.GetKitchenObject().GetKitchenObjectSO())) return;

                // player has kitchen object. Give to counter
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
        else
        {
            // counter has kitchen object
            if (player.HasKitchenObject())
            {
                // player has kitchen object and counter has kitchen object.
            }
            else
            {
                // player does not have kitchen object, but counter does. Give to player
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if(HasKitchenObject() && HasCuttableKitchenObject(GetKitchenObject().GetKitchenObjectSO()))
        {
            // Find a matching recipe
            var matchingRecipeSO = cuttingRecipeSOArray.FirstOrDefault(a => a.input == GetKitchenObject().GetKitchenObjectSO());

            // destroy current kitchen object
            GetKitchenObject().DestroySelf();

            // spawn cut kitchen object
            if(matchingRecipeSO != null)
            {
                KitchenObject.SpawnKitchenObject(matchingRecipeSO.output, this);
            }
        }
    }

    private bool HasCuttableKitchenObject(KitchenObjectSO kitchenObjectSO) => cuttingRecipeSOArray.Any(a => a.input == kitchenObjectSO);
}
