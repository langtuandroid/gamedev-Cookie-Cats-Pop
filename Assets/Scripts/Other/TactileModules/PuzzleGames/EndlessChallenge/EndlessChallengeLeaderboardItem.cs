using System;
using System.Collections.Generic;
using TactileModules.FeatureManager;
using TactileModules.Foundation;
using TactileModules.PuzzleGames.EndlessChallenge.Data;
using UnityEngine;

namespace TactileModules.PuzzleGames.EndlessChallenge
{
    public class EndlessChallengeLeaderboardItem : MonoBehaviour
    {
        private FacebookClient FacebookClient
        {
            get
            {
                return ManagerRepository.Get<FacebookClient>();
            }
        }

        private EndlessChallengeHandler Handler
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<EndlessChallengeHandler>();
            }
        }

        public void Initialize(int position, Entry entry, bool isMe)
        {
            this.bestRow.text = string.Format(L.Get("{0} rows"), entry.Score.MaxRows);
            this.allTimeBestPivot.gameObject.SetActive(isMe);
            if (isMe)
            {
                this.bestRow.Color = this.myNameLabelColor;
                this.backgroundSprite.Color = this.myBackgroundColor;
                this.allTimeBestRow.text = string.Format(L.Get("Your best: {0}"), this.Handler.AllTimeHighestRow);
            }
            this.SetPosition(position);
            CloudUser cloudUser = this.Handler.GetCloudUser(entry.UserId);
            this.SetName(isMe, entry, cloudUser);
            this.SetPortrait(isMe, entry, cloudUser);
        }

        private void SetPosition(int rank)
        {
            this.positionNumber.text = rank.ToString();
            if (rank - 1 < this.leaderSpriteNames.Count)
            {
                this.leaderIcon.SpriteName = this.leaderSpriteNames[rank - 1];
            }
            else
            {
                this.leaderIcon.gameObject.SetActive(false);
            }
        }

        private void SetName(bool isMe, Entry entry, CloudUser user)
        {
            this.firstName.text = this.GetFirstName(isMe, user);
            if (string.IsNullOrEmpty(this.firstName.text))
            {
                if (isMe)
                {
                    this.firstName.text = L.Get("You");
                    this.firstName.Color = this.myNameLabelColor;
                }
                else
                {
                    this.firstName.text = this.GetRandomName(entry.DeviceId);
                }
            }
        }

        private void SetPortrait(bool isMe, Entry entry, CloudUser user)
        {
            string text = string.Empty;
            if (user != null && !string.IsNullOrEmpty(user.FacebookId))
            {
                text = user.FacebookId;
            }
            else if (isMe)
            {
                text = ((!this.Handler.CloudClient.HasValidUser) ? string.Empty : this.Handler.CloudClient.CachedMe.ExternalId);
            }
            if (string.IsNullOrEmpty(text))
            {
                bool flag = false;
                if (isMe && this.FacebookClient.IsSessionValid && this.FacebookClient.CachedMe != null)
                {
                    this.portrait.Load(this.FacebookClient, this.FacebookClient.CachedMe.Id, null);
                    this.nonFBPortraitTexture.gameObject.SetActive(false);
                    flag = true;
                }
                if (!flag)
                {
                    this.portrait.gameObject.SetActive(false);
                    this.nonFBPortraitTexture.SetTexture(this.GetRandomPortrait(entry.DeviceId));
                }
            }
            else
            {
                this.portrait.Load(this.FacebookClient, text, null);
            }
        }

        private string GetFirstName(bool isMe, CloudUser user)
        {
            string text = string.Empty;
            if (user != null)
            {
                text = user.DisplayName;
            }
            else if (isMe)
            {
                text = ((!this.Handler.CloudClient.HasValidUser) ? string.Empty : this.Handler.CloudClient.CachedMe.DisplayName);
            }
            if (!string.IsNullOrEmpty(text))
            {
                string[] array = text.Split(new char[]
                {
                    ' '
                });
                if (array.Length > 0)
                {
                    return array[0];
                }
            }
            return string.Empty;
        }

        private string GetRandomName(string deviceId)
        {
            int hashCode = deviceId.GetHashCode();
            int value = hashCode % this.nameList.Length;
            return this.nameList[Mathf.Abs(value)];
        }

        private Texture2D GetRandomPortrait(string deviceId)
        {
            int hashCode = deviceId.GetHashCode();
            int value = hashCode % this.randomPortraitTextures.Count;
            return this.randomPortraitTextures[Mathf.Abs(value)];
        }

        [SerializeField]
        private FacebookPortraitWithProgress portrait;

        [SerializeField]
        private UILabel firstName;

        [SerializeField]
        private UILabel bestRow;

        [SerializeField]
        private UIElement allTimeBestPivot;

        [SerializeField]
        private UILabel allTimeBestRow;

        [SerializeField]
        private UISprite leaderIcon;

        [SerializeField]
        private UISprite backgroundSprite;

        [SerializeField]
        private UILabel positionNumber;

        [SerializeField]
        private UITextureQuad nonFBPortraitTexture;

        [SerializeField]
        private List<string> leaderSpriteNames;

        [SerializeField]
        private Color myNameLabelColor;

        [SerializeField]
        private Color myBackgroundColor;

        [SerializeField]
        private List<Texture2D> randomPortraitTextures;

        private string[] nameList = new string[]
        {
            "Sarah",
            "Jacob",
            "Jennifer",
            "Keith",
            "Sally",
            "Eric",
            "Connor",
            "Madeleine",
            "Sam",
            "Una",
            "Steven",
            "Lillian",
            "Claire",
            "Luke",
            "Heather",
            "Ruth",
            "Harry",
            "Austin",
            "Steven",
            "Joan",
            "Luke",
            "Diana",
            "Carl",
            "Adam",
            "Brian",
            "Richard",
            "Cameron",
            "Jane",
            "Christian",
            "Trevor",
            "Sebastian",
            "Frank",
            "Liam",
            "Diana",
            "Arya",
            "Sally",
            "Julian",
            "Natalie",
            "Molly",
            "Ruth",
            "Jason",
            "Grace",
            "Faith",
            "Max",
            "Fiona",
            "Vladimir",
            "Diana",
            "Stephen",
            "Colin",
            "Keith",
            "Emma",
            "David",
            "Sally",
            "Colin",
            "Warren",
            "Alexander",
            "Lucas",
            "Wendy",
            "Nathan",
            "Lillian",
            "Anne",
            "Neil",
            "Sally",
            "Brandon",
            "Caroline",
            "Amelia",
            "Ruth",
            "Ryan",
            "Wanda",
            "Leah",
            "Jason",
            "Alexandra",
            "Jasmine",
            "Bella",
            "Nathan",
            "Sam",
            "Anna",
            "Matt",
            "Brandon",
            "Warren",
            "Oliver",
            "Sue",
            "Karen",
            "Dharma",
            "Rebecca",
            "Rachel",
            "Sean",
            "Suzy",
            "Amelia",
            "Oliver",
            "Benjamin",
            "Elizabeth",
            "Diana",
            "Eleanor",
            "Madonna",
            "Lisa",
            "Dylan",
            "John",
            "Gordon",
            "Jason",
            "Dorothy",
            "Amanda",
            "Dorothy",
            "Bella",
            "Christian",
            "Wanda",
            "Sarah",
            "Wanda",
            "Pippa",
            "Karen",
            "Matt",
            "Angela",
            "Amelia",
            "Michelle",
            "Karen",
            "Pippa",
            "Penelope",
            "Max",
            "Benjamin",
            "Joseph",
            "Line",
            "Alexander",
            "Michael",
            "Heather",
            "Stewart",
            "Michael",
            "Boris",
            "Dorothy",
            "Sam",
            "Wen",
            "Faith",
            "Jennifer",
            "Henrik",
            "Sofie",
            "Tracey",
            "Lauren",
            "Yvonne",
            "Chloe",
            "Alison",
            "Lucas",
            "Chloe",
            "Stewart",
            "Nicola",
            "Jane",
            "John",
            "Thomas",
            "Cameron",
            "Claire",
            "Joan",
            "Claire",
            "Jan",
            "Hannah",
            "Connor",
            "Abigail",
            "Heather",
            "Owen",
            "Emma",
            "Joshua",
            "Wanda",
            "Rachel",
            "Amanda",
            "Alexander",
            "Eric",
            "Dylan",
            "Andrew",
            "Trevor",
            "Robert",
            "Eric",
            "Brian",
            "Jan",
            "Pippa",
            "Brandon",
            "Dan",
            "Amelia",
            "Lily",
            "Nathan",
            "Rebecca",
            "Sam",
            "Hannah",
            "Carol",
            "Bernadette",
            "Jake",
            "Jennifer",
            "Stephen",
            "Olivia",
            "Nathan",
            "David",
            "Jack",
            "Colin",
            "Dan",
            "Mary",
            "Sue",
            "Dan",
            "Michael",
            "Chloe",
            "Lillian",
            "Maria",
            "Leonard",
            "William"
        };
    }
}
