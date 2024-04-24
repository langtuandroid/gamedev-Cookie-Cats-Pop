using System;
using System.Collections;
using TactileModules.Ads;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

namespace TactileModules.SagaGame.AdsForSagaGame
{
    public class InterstitialRequester
    {


        private IEnumerator HandleFullscreenPreChange(ChangeInfo info)
        {
            if (info.NextOwner is MainMapFlow)
            {
                
            }
            yield break;
        }

        
    }
}
