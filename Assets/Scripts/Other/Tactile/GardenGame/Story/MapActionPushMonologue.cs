using System;
using System.Collections;
using Tactile.GardenGame.Story.Monologue;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
    public class MapActionPushMonologue : MapAction, IMapActionLocalizable
    {
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        public override IEnumerator Logic(IStoryMapController map)
        {
            map.Monologues.Push(new Tactile.GardenGame.Story.Monologue.Monologue
            {
                Text = this.text
            });
            yield break;
        }

        public override bool IsAllowedWhenSkipping
        {
            get
            {
                return true;
            }
        }

        string IMapActionLocalizable.GetLocalizableText()
        {
            return this.Text;
        }

        string IMapActionLocalizable.GetContext()
        {
            return "Monologue";
        }

        [SerializeField]
        private string text;
    }
}
