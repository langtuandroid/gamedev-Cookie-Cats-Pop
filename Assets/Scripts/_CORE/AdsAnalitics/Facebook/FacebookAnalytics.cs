using System;
using System.Collections.Generic;


public class FacebookAnalytics
{
    public static void LogUserRegistered(bool detectedAtLogin, FacebookUser fbUser)
    {
        if (AnalyticsBase.Instance.GetOneTimeEventSent("UserRegistered"))
        {
            return;
        }
        AnalyticsBase.Instance.SetOneTimeEventSent("UserRegistered");
        TactileAnalytics.Instance.LogEvent(new FacebookAnalytics.UserRegisteredEvent(fbUser), -1.0, null);

        AdjustUtils.LogUserRegistered();
    }

    [TactileAnalytics.EventAttribute("userRegistered", true)]
    protected class UserRegisteredEvent : BasicEvent
    {
        public UserRegisteredEvent(FacebookUser fbUser)
        {
            if (fbUser != null)
            {
                this.FacebookId = fbUser.Id;
                if (!string.IsNullOrEmpty(fbUser.Email))
                {
                    this.FacebookEmail = fbUser.Email;
                }
                if (!string.IsNullOrEmpty(fbUser.Name))
                {
                    this.Name = fbUser.Name;
                }
                if (!string.IsNullOrEmpty(fbUser.FirstName))
                {
                    this.FirstName = fbUser.FirstName;
                }
                if (!string.IsNullOrEmpty(fbUser.LastName))
                {
                    this.LastName = fbUser.LastName;
                }
                if (!string.IsNullOrEmpty(fbUser.Username))
                {
                    this.Username = fbUser.Username;
                }
                if (!string.IsNullOrEmpty(fbUser.Gender))
                {
                    this.Gender = fbUser.Gender.ToUpper();
                }
            }
        }

        public TactileAnalytics.OptionalParam<string> FacebookId { get; set; }

        public TactileAnalytics.OptionalParam<string> FacebookEmail { get; set; }

        public TactileAnalytics.OptionalParam<string> Name { get; set; }

        public TactileAnalytics.OptionalParam<string> FirstName { get; set; }

        public TactileAnalytics.OptionalParam<string> LastName { get; set; }

        public TactileAnalytics.OptionalParam<string> Username { get; set; }

        public TactileAnalytics.OptionalParam<string> Gender { get; set; }
    }
}
