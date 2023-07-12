using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }

    public event EventHandler onCut;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;

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
                cuttingProgress = 0;
                OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs { progressNormalized = 0 });
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
            cuttingProgress++;
            onCut?.Invoke(this, EventArgs.Empty);
            var cuttingRecipe = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
            OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs { progressNormalized = (float)cuttingProgress / cuttingRecipe.cuttingProgressMax });

            if (cuttingProgress >= cuttingRecipe.cuttingProgressMax)
            {
               // var matchingRecipeSO = cuttingRecipeSOArray.FirstOrDefault(a => a.input == GetKitchenObject().GetKitchenObjectSO());

                // destroy current kitchen object
                GetKitchenObject().DestroySelf();

                // spawn cut kitchen object
                if (cuttingRecipe != null)
                {
                    KitchenObject.SpawnKitchenObject(cuttingRecipe.output, this);
                }
            }
        }
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO kitchenObjectSO) => cuttingRecipeSOArray.FirstOrDefault(a => a.input == kitchenObjectSO);

    private bool HasCuttableKitchenObject(KitchenObjectSO kitchenObjectSO) => GetCuttingRecipeSOWithInput(kitchenObjectSO) != null;
}
