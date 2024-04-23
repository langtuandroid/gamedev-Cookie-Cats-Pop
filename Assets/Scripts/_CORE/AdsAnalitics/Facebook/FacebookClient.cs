using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FacebookClient : SingleInstance<FacebookClient>
{
    public FacebookClient(string facebookAppId, string facebookAppNamespace, string facebookSuffix, string openGraphVersion)
    {
        this.facebookAppId = facebookAppId;
        this.facebookAppNamespace = facebookAppNamespace;
        this.facebookSuffix = facebookSuffix;
        this.openGraphVersion = openGraphVersion;
        this.portraitManager = new FacebookPortraitManager(this);
        this.localScores = FacebookClient.PersistedLocalScores;

    }

    private void OnInitComplete()
    {
        if (this.IsSessionValid)
        {
            this.cachedPermissions = FacebookClient.PersistedCachedPermissions;
            this.cachedMe = FacebookClient.PersistedCachedMe;
            this.cachedFriends = FacebookClient.PersistedCachedFriends;
        }
        else
        {
            this.ClearCachedData();
        }
        if (this.FacebookInitComplete != null)
        {
            this.FacebookInitComplete();
        }
    }

    public IEnumerator Login(List<string> readPermissions, Action<object> callback)
    {
        if (!this.IsInitialized)
        {
            callback("Facebook Not Initialized");
            yield break;
        }
        if (this.loginInProgress)
        {
            callback("Login is already in progress");
            yield break;
        }
        this.loginInProgress = true;
        yield return this.LoginInternal(readPermissions, callback);
        this.loginInProgress = false;
        yield break;
    }

    private IEnumerator LoginInternal(List<string> readPermissions, Action<object> callback)
    {
        ActivityManager.PushIgnoreNextOnResumeEventIfNoNewIntent();
        object loginError = null;
        bool facebookCompleted = false;
        this.cancelLogin = false;

        while (!this.cancelLogin && !facebookCompleted)
        {
            yield return null;
        }
        ActivityManager.PopIgnoreNextOnResumeEventIfNoNewIntent();
        callback(loginError);
        if (this.IsSessionValid)
        {
            this.OnFacebookLogin();
        }
        yield break;
    }

    public IEnumerator ReauthorizeWithPublishPermissions(List<string> publishPermissions, Action<object> callback)
    {
        bool facebookCompleted = false;
        object callbackInfo = null;
        this.cancelLogin = false;
        ActivityManager.PushIgnoreNextOnResumeEventIfNoNewIntent();

        while (!this.cancelLogin && !facebookCompleted)
        {
            yield return null;
        }
        ActivityManager.PopIgnoreNextOnResumeEventIfNoNewIntent();
        yield return this.UpdateCachedPermissions(delegate (object err)
        {
        });
        callback(callbackInfo);
        yield break;
    }

    public IEnumerator ReauthorizeWithReadPermissions(List<string> readPermissions, Action<object> callback)
    {
        bool facebookCompleted = false;
        object callbackInfo = null;
        this.cancelLogin = false;
        ActivityManager.PushIgnoreNextOnResumeEventIfNoNewIntent();

        while (!this.cancelLogin && !facebookCompleted)
        {
            yield return null;
        }
        ActivityManager.PopIgnoreNextOnResumeEventIfNoNewIntent();
        yield return this.UpdateCachedPermissions(delegate (object err)
        {
        });
        callback(callbackInfo);
        yield break;
    }



    public void Logout()
    {

        this.ClearCachedData();
        this.OnFacebookLogout();
    }

    private void ClearCachedData()
    {
        FacebookClient.PersistedCachedPermissions = null;
        FacebookClient.PersistedCachedMe = null;
        FacebookClient.PersistedCachedFriends = null;
        this.cachedPermissions = FacebookClient.PersistedCachedPermissions;
        this.cachedMe = FacebookClient.PersistedCachedMe;
        this.cachedFriends = FacebookClient.PersistedCachedFriends;
    }

    private void OnFacebookLogin()
    {
        if (this.FacebookLogin != null)
        {
            this.FacebookLogin();
        }
    }

    private void OnFacebookLogout()
    {
        if (this.FacebookLogout != null)
        {
            this.FacebookLogout();
        }
    }

    private IEnumerator DoRequest(string url, Action<object, WWW> callback)
    {
        yield return this.DoRequest(url, null, callback);
        yield break;
    }

    private IEnumerator RunRequest(string url, WWWForm form, Action<WWW> callback)
    {
        if (form != null)
        {
            yield return TactileRequest.Run(url, form, callback, TactileRequest.RequestPriority.Default);
        }
        else
        {
            yield return TactileRequest.Run(url, callback, TactileRequest.RequestPriority.Default);
        }
        yield break;
    }

    private IEnumerator DoRequest(string url, WWWForm form, Action<object, WWW> callback)
    {
        yield return this.RunRequest(url, form, delegate (WWW request)
        {
            if (request == null)
            {
                callback("Request timed out", null);
            }
            else if (request.error != null)
            {
                Dictionary<string, string> responseHeaders = request.responseHeaders;
                string text = (!responseHeaders.ContainsKey("WWW-AUTHENTICATE")) ? string.Empty : responseHeaders["WWW-AUTHENTICATE"];
                if (text.IndexOf("invalid_token", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    this.Logout();
                }
                callback(request.error, null);
            }
            else if (request.bytes.Length == 0)
            {
                callback("Invalid byte array in request.", null);
            }
            else if (request.bytes[0] != 123 && request.bytes[0] != 91)
            {
                callback(null, request);
            }
            else
            {
                List<Hashtable> list = new List<Hashtable>();
                bool flag = false;
                if (request.bytes[0] == 91)
                {
                    flag = true;
                    ArrayList arrayList = request.text.arrayListFromJson();
                    IEnumerator enumerator = arrayList.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            object obj = enumerator.Current;
                            Hashtable hashtable = (Hashtable)obj;
                            if (hashtable != null && hashtable.ContainsKey("body"))
                            {
                                list.Add(((string)hashtable["body"]).hashtableFromJson());
                            }
                        }
                    }
                    finally
                    {
                        IDisposable disposable;
                        if ((disposable = (enumerator as IDisposable)) != null)
                        {
                            disposable.Dispose();
                        }
                    }
                }
                else
                {
                    list.Add(request.text.hashtableFromJson());
                }
                bool flag2 = false;
                foreach (Hashtable hashtable2 in list)
                {
                    if (hashtable2.Contains("error"))
                    {
                        Hashtable hashtable3 = hashtable2["error"] as Hashtable;
                        string arg = "No error message specified by facebook";
                        if (hashtable3.Contains("message"))
                        {
                            arg = (hashtable3["message"] as string);
                        }
                        if (hashtable3.Contains("code"))
                        {
                            int num = (int)((double)hashtable3["code"]);
                            if (num == 190 || num == 102)
                            {
                                this.Logout();
                            }
                            else if (num == 10 || (num >= 200 && num <= 299))
                            {
                                FiberCtrl.Pool.Run(this.UpdateCachedPermissions(delegate (object err)
                                {
                                }), false);
                            }
                        }
                        if (!flag)
                        {
                            callback(arg, null);
                            flag2 = true;
                        }
                        break;
                    }
                }
                if (!flag2)
                {
                    callback(null, request);
                }
            }
        });
        yield break;
    }

    public IEnumerator QueryPendingRequests(Action<object, List<FacebookRequest>> callback)
    {
        if (this.IsSessionValid)
        {
            string url = this.OpenGraphBaseUrl + "me/apprequests?access_token=" + this.AccessToken;
            yield return this.DoJsonRequest(url, delegate (object err, Hashtable result)
            {
                if (err == null)
                {
                    List<FacebookRequest> list = new List<FacebookRequest>();
                    if (result != null && result.ContainsKey("data") && result["data"].GetType() == typeof(ArrayList))
                    {
                        ArrayList arrayList = (ArrayList)result["data"];
                        IEnumerator enumerator = arrayList.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                object obj = enumerator.Current;
                                if (obj != null && obj.GetType() == typeof(Hashtable))
                                {
                                    Hashtable hashtable = (Hashtable)obj;
                                    FacebookRequest facebookRequest = JsonSerializer.HashtableToObject<FacebookRequest>(hashtable);
                                    if (facebookRequest != null)
                                    {
                                        if (hashtable.ContainsKey("data") && hashtable["data"].GetType() == typeof(string))
                                        {
                                            facebookRequest.Data = (MiniJSON.jsonDecode(hashtable["data"] as string) as Hashtable);
                                        }
                                        list.Add(facebookRequest);
                                    }
                                }
                            }
                        }
                        finally
                        {
                            IDisposable disposable;
                            if ((disposable = (enumerator as IDisposable)) != null)
                            {
                                disposable.Dispose();
                            }
                        }
                    }
                    callback(null, list);
                }
                else
                {
                    callback(err, null);
                }
            });
        }
        else
        {
            callback("Not logged in to Facebook", null);
        }
        yield break;
    }

    public IEnumerator ConsumeRequest(string requestId, Action<object> callback)
    {
        string url = string.Format(this.OpenGraphBaseUrl + "{0}?method=DELETE&access_token={1}", requestId, this.AccessToken);
        yield return this.DoJsonRequest(url, delegate (object error, Hashtable result)
        {
            callback(error);
        });
        yield break;
    }

    public IEnumerator DoBatchRequest(string requests, Action<object, ArrayList> callback)
    {
        if (this.IsSessionValid)
        {
            WWWForm form = new WWWForm();
            form.AddField("batch", requests);
            form.AddField("access_token", this.AccessToken);
            yield return this.DoRequest(this.OpenGraphBaseUrl, form, delegate (object err, WWW request)
            {
                if (err != null)
                {
                    callback(err, null);
                }
                else
                {
                    callback(null, request.text.arrayListFromJson());
                }
            });
        }
        else
        {
            callback("Not logged in to Facebook", null);
        }
        yield break;
    }

    public IEnumerator DoJsonRequest(string url, Action<object, Hashtable> callback)
    {
        yield return this.DoJsonRequest(url, null, callback);
        yield break;
    }

    public IEnumerator DoJsonRequest(string url, WWWForm form, Action<object, Hashtable> callback)
    {
        yield return this.DoRequest(url, form, delegate (object err, WWW request)
        {
            if (err != null)
            {
                callback(err, null);
            }
            else
            {
                callback(null, request.text.hashtableFromJson());
            }
        });
        yield break;
    }

    public IEnumerator DoImageRequest(string url, Action<object, Texture2D> callback)
    {
        yield return this.DoRequest(url, delegate (object err, WWW request)
        {
            if (err != null)
            {
                callback(err, null);
            }
            else
            {
                Texture2D texture2D = new Texture2D(64, 64, TextureFormat.RGB24, false);
                request.LoadImageIntoTexture(texture2D);
                callback(null, texture2D);
            }
        });
        yield break;
    }

    public IEnumerator Update()
    {
        if (this.IsSessionValid)
        {
            yield return this.UpdateCachedData();
        }
        else
        {
            this.ClearCachedData();
        }
        yield break;
    }

    public IEnumerator DeletePermissions(Action<object> callback)
    {
        object error = null;
        if (this.IsSessionValid)
        {
            string url = this.OpenGraphBaseUrl + "me/permissions?method=DELETE&access_token=" + this.AccessToken;
            yield return this.DoJsonRequest(url, delegate (object err, Hashtable result)
            {
                error = err;
                if (err == null)
                {
                    this.cachedPermissions.Clear();
                    this.Logout();
                }
            });
        }
        else
        {
            this.ClearCachedData();
        }
        if (callback != null)
        {
            callback(error);
        }
        yield break;
    }

    private IEnumerator UpdateCachedData()
    {
        yield return this.DoBatchRequest(MiniJSON.jsonEncode(new ArrayList
        {
            new Hashtable
            {
                {
                    "method",
                    "GET"
                },
                {
                    "relative_url",
                    this.openGraphVersion + "/me/permissions"
                }
            },
            new Hashtable
            {
                {
                    "method",
                    "GET"
                },
                {
                    "relative_url",
                    this.openGraphVersion + "/me?fields=id,name,first_name,last_name,gender,email"
                }
            },
            new Hashtable
            {
                {
                    "method",
                    "GET"
                },
                {
                    "relative_url",
                    this.openGraphVersion + "/me/friends?fields=id,name,first_name,last_name,gender,installed,email"
                }
            }
        }, false, 0), delegate (object err, ArrayList results)
        {
            if (err == null)
            {
                this.cachedPermissions.Clear();
                Hashtable hashtable = results[0] as Hashtable;
                if (hashtable != null && hashtable.ContainsKey("code") && (int)((double)hashtable["code"]) == 200 && hashtable.ContainsKey("body"))
                {
                    Hashtable hashtable2 = ((string)hashtable["body"]).hashtableFromJson();
                    ArrayList arrayList = hashtable2["data"] as ArrayList;
                    if (arrayList != null && arrayList.Count >= 1)
                    {
                        IEnumerator enumerator = arrayList.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                object obj = enumerator.Current;
                                Hashtable hashtable3 = (Hashtable)obj;
                                if (hashtable3.ContainsKey("permission") && hashtable3.ContainsKey("status") && (string)hashtable3["status"] == "granted")
                                {
                                    this.cachedPermissions.Add((string)hashtable3["permission"]);
                                }
                            }
                        }
                        finally
                        {
                            IDisposable disposable;
                            if ((disposable = (enumerator as IDisposable)) != null)
                            {
                                disposable.Dispose();
                            }
                        }
                    }
                }
                FacebookClient.PersistedCachedPermissions = this.cachedPermissions;
                hashtable = (results[1] as Hashtable);
                if (hashtable != null && hashtable.ContainsKey("code") && (int)((double)hashtable["code"]) == 200 && hashtable.ContainsKey("body"))
                {
                    Hashtable table = ((string)hashtable["body"]).hashtableFromJson();
                    this.cachedMe = JsonSerializer.HashtableToObject<FacebookUser>(table);
                    FacebookClient.PersistedCachedMe = this.cachedMe;
                }
                hashtable = (results[2] as Hashtable);
                if (hashtable != null && hashtable.ContainsKey("code") && (int)((double)hashtable["code"]) == 200 && hashtable.ContainsKey("body"))
                {
                    Hashtable hashtable4 = ((string)hashtable["body"]).hashtableFromJson();
                    if (hashtable4 != null && hashtable4.ContainsKey("data") && hashtable4["data"] is ArrayList)
                    {
                        this.cachedFriends = JsonSerializer.ArrayListToGenericList<FacebookUser>((ArrayList)hashtable4["data"]);
                        FacebookClient.PersistedCachedFriends = this.cachedFriends;
                    }
                }
            }
        });
        yield break;
    }

    private IEnumerator UpdateCachedPermissions(Action<object> callback)
    {
        object error = null;
        if (this.IsSessionValid)
        {
            string url = this.OpenGraphBaseUrl + "me/permissions?access_token=" + this.AccessToken;
            yield return this.DoJsonRequest(url, delegate (object err, Hashtable result)
            {
                error = err;
                if (err == null)
                {
                    this.cachedPermissions.Clear();
                    if (result != null && result.ContainsKey("data"))
                    {
                        ArrayList arrayList = result["data"] as ArrayList;
                        if (arrayList != null && arrayList.Count >= 1)
                        {
                            IEnumerator enumerator = arrayList.GetEnumerator();
                            try
                            {
                                while (enumerator.MoveNext())
                                {
                                    object obj = enumerator.Current;
                                    Hashtable hashtable = (Hashtable)obj;
                                    if (hashtable.ContainsKey("permission") && hashtable.ContainsKey("status") && (string)hashtable["status"] == "granted")
                                    {
                                        this.cachedPermissions.Add((string)hashtable["permission"]);
                                    }
                                }
                            }
                            finally
                            {
                                IDisposable disposable;
                                if ((disposable = (enumerator as IDisposable)) != null)
                                {
                                    disposable.Dispose();
                                }
                            }
                        }
                    }
                    FacebookClient.PersistedCachedPermissions = this.cachedPermissions;
                }
            });
        }
        else
        {
            error = "Not possible to get facebook permissions. Session is not valid";
        }
        callback(error);
        yield break;
    }

    public IEnumerator UpdateCachedMe(Action<object> callback)
    {
        object error = null;
        if (this.IsSessionValid)
        {
            string url = this.OpenGraphBaseUrl + "me?fields=id,name,first_name,last_name,gender,email&access_token=" + this.AccessToken;
            yield return this.DoJsonRequest(url, delegate (object err, Hashtable result)
            {
                error = err;
                if (error == null)
                {
                    this.cachedMe = JsonSerializer.HashtableToObject<FacebookUser>(result);
                    FacebookClient.PersistedCachedMe = this.cachedMe;
                }
            });
        }
        else
        {
            error = "Not possible to get facebook me. Session is not valid";
        }
        callback(error);
        yield break;
    }

    public IEnumerator UpdateCachedFriends(Action<object> callback)
    {
        object error = null;
        if (this.IsSessionValid)
        {
            string url = this.OpenGraphBaseUrl + "me/friends?fields=id,name,first_name,last_name,gender,installed,email&access_token=" + this.AccessToken;
            yield return this.DoJsonRequest(url, delegate (object err, Hashtable result)
            {
                error = err;
                if (err == null)
                {
                    if (result != null && result.ContainsKey("data") && result["data"] is ArrayList)
                    {
                        this.cachedFriends = JsonSerializer.ArrayListToGenericList<FacebookUser>((ArrayList)result["data"]);
                        FacebookClient.PersistedCachedFriends = this.cachedFriends;
                    }
                    else
                    {
                        error = "Facebook UpdateCachedFriends failed - unexpected data";
                    }
                }
            });
        }
        else
        {
            error = "Not possible to get facebook friends. Session is not valid";
        }
        callback(error);
        yield break;
    }



    public IEnumerator LoadPortrait(string facebookId, Action<object, Texture2D> callback)
    {
        return this.portraitManager.GetPortrait(facebookId, callback);
    }

    public IEnumerator LoadPortraitNoCache(string facebookId, Action<object, Texture2D> callback)
    {
        string url = this.OpenGraphBaseUrl + facebookId + "/picture?redirect=false&width=128&height=128";
        if (this.IsSessionValid)
        {
            url = url + "&access_token=" + this.AccessToken;
        }
        Hashtable imageDescription = null;
        object error = null;
        yield return this.DoJsonRequest(url, delegate (object err, Hashtable result)
        {
            error = err;
            if (error == null)
            {
                if (result != null && result.ContainsKey("data") && result["data"].GetType() == typeof(Hashtable))
                {
                    imageDescription = (Hashtable)result["data"];
                }
                else
                {
                    error = "Unexpected data received when trying to get picture data";
                }
            }
        });
        if (error == null)
        {
            if (imageDescription != null && imageDescription.ContainsKey("is_silhouette") && imageDescription.ContainsKey("url"))
            {
                if (!(bool)imageDescription["is_silhouette"])
                {
                    yield return this.DoImageRequest((string)imageDescription["url"], callback);
                }
                else
                {
                    callback(null, null);
                }
            }
            else
            {
                callback("Invalid image description received from facebook", null);
            }
        }
        else
        {
            callback(error, null);
        }
        yield break;
    }

    public void PostOpenGraphStory(string action, string objectType, string objectId, Dictionary<string, string> properties)
    {
        FiberCtrl.Pool.Run(this.PostOpenGraphStoryCr(action, objectType, objectId, properties, delegate (object err)
        {
        }), false);
    }

    public IEnumerator PostOpenGraphStoryCr(string action, string objectType, string objectId, Dictionary<string, string> properties, Action<object> callback)
    {
        object error = null;
        if (this.IsSessionValid && this.HasPublishPermissions)
        {
            string url = string.Concat(new string[]
            {
                this.OpenGraphBaseUrl,
                "me/",
                this.facebookAppNamespace,
                ":",
                action
            });
            WWWForm form = new WWWForm();
            form.AddField(objectType, objectId);
            form.AddField("access_token", this.AccessToken);
            foreach (KeyValuePair<string, string> keyValuePair in properties)
            {
                form.AddField(keyValuePair.Key, keyValuePair.Value);
            }
            yield return this.DoRequest(url, form, delegate (object err, WWW request)
            {
                if (err != null)
                {
                    error = err;
                }
            });
        }
        else if (!this.IsSessionValid)
        {
            error = "Not possible to post open graph story. Session is not valid";
        }
        else
        {
            error = "Not possible to post open graph story. Publish permissions not available";
        }
        callback(error);
        yield break;
    }

    public IEnumerator PostLinkCr(string message, string link, Action<object> callback)
    {
        object error = null;
        if (this.IsSessionValid && this.HasPublishPermissions)
        {
            string url = this.OpenGraphBaseUrl + "me/feed";
            WWWForm form = new WWWForm();
            form.AddField("message", message);
            form.AddField("link", link);
            form.AddField("access_token", this.AccessToken);
            yield return this.DoRequest(url, form, delegate (object err, WWW request)
            {
                if (err != null)
                {
                    error = err;
                }
            });
        }
        else if (!this.IsSessionValid)
        {
            error = "Not possible to post link. Session is not valid";
        }
        else
        {
            error = "Not possible to post link. Publish permissions not available";
        }
        callback(error);
        yield break;
    }

    public void SubmitScore(int score)
    {
        this.localScores.Add(new FacebookClient.FacebookLocalScore
        {
            score = (long)score,
            submitted = false
        });
        FacebookClient.PersistedLocalScores = this.localScores;
        FiberCtrl.Pool.Run(this.SubmitUnsubmittedScores(), false);
    }

    private FacebookClient.FacebookLocalScore GetUnsubmittedScore()
    {
        foreach (FacebookClient.FacebookLocalScore facebookLocalScore in this.localScores)
        {
            if (!facebookLocalScore.submitted)
            {
                return facebookLocalScore;
            }
        }
        return null;
    }

    public IEnumerator SubmitUnsubmittedScores()
    {
        if (this.IsSessionValid && this.HasPublishPermissions)
        {
            string url = this.OpenGraphBaseUrl + "me/scores";
            FacebookClient.FacebookLocalScore unsubmittedScore;
            for (unsubmittedScore = this.GetUnsubmittedScore(); unsubmittedScore != null; unsubmittedScore = this.GetUnsubmittedScore())
            {
                WWWForm form = new WWWForm();
                form.AddField("score", unsubmittedScore.score.ToString());
                form.AddField("access_token", this.AccessToken);
                object error = null;
                yield return this.DoRequest(url, form, delegate (object err, WWW request)
                {
                    if (err == null)
                    {
                        unsubmittedScore.submitted = true;
                        FacebookClient.PersistedLocalScores = this.localScores;
                    }
                    else
                    {
                        error = err;
                    }
                });
                if (error != null)
                {
                    break;
                }
            }
        }
        int scoresToRemove = this.localScores.Count - 10;
        if (scoresToRemove > 0)
        {
            this.localScores.RemoveRange(0, scoresToRemove);
        }
        FacebookClient.PersistedLocalScores = this.localScores;
        yield break;
    }

    private static FacebookUser PersistedCachedMe
    {
        get
        {
            string securedString = TactilePlayerPrefs.GetSecuredString("FacebookClientCachedFacebookMe", string.Empty);
            if (securedString.Length > 0)
            {
                return JsonSerializer.HashtableToObject<FacebookUser>(securedString.hashtableFromJson());
            }
            return null;
        }
        set
        {
            if (value != null)
            {
                TactilePlayerPrefs.SetSecuredString("FacebookClientCachedFacebookMe", JsonSerializer.ObjectToHashtable(value).toJson());
            }
            else
            {
                TactilePlayerPrefs.SetSecuredString("FacebookClientCachedFacebookMe", string.Empty);
            }
        }
    }

    private static List<FacebookUser> PersistedCachedFriends
    {
        get
        {
            string securedString = TactilePlayerPrefs.GetSecuredString("FacebookClientCachedFacebookFriends", string.Empty);
            if (securedString.Length > 0)
            {
                return JsonSerializer.ArrayListToGenericList<FacebookUser>(securedString.arrayListFromJson());
            }
            return new List<FacebookUser>();
        }
        set
        {
            if (value != null)
            {
                TactilePlayerPrefs.SetSecuredString("FacebookClientCachedFacebookFriends", JsonSerializer.GenericListToArrayList<FacebookUser>(value).toJson());
            }
            else
            {
                TactilePlayerPrefs.SetSecuredString("FacebookClientCachedFacebookFriends", string.Empty);
            }
        }
    }

    private static List<FacebookClient.FacebookLocalScore> PersistedLocalScores
    {
        get
        {
            string securedString = TactilePlayerPrefs.GetSecuredString("FacebookClientLocalScores", string.Empty);
            if (securedString.Length > 0)
            {
                return JsonSerializer.ArrayListToGenericList<FacebookClient.FacebookLocalScore>(securedString.arrayListFromJson());
            }
            return new List<FacebookClient.FacebookLocalScore>();
        }
        set
        {
            if (value != null)
            {
                TactilePlayerPrefs.SetSecuredString("FacebookClientLocalScores", JsonSerializer.GenericListToArrayList<FacebookClient.FacebookLocalScore>(value).toJson());
            }
            else
            {
                TactilePlayerPrefs.SetSecuredString("FacebookClientLocalScores", string.Empty);
            }
        }
    }

    private static List<string> PersistedCachedPermissions
    {
        get
        {
            string securedString = TactilePlayerPrefs.GetSecuredString("FacebookClientCachedPermissions", string.Empty);
            if (securedString.Length > 0)
            {
                return JsonSerializer.ArrayListToGenericList<string>(securedString.arrayListFromJson());
            }
            return new List<string>();
        }
        set
        {
            if (value != null)
            {
                TactilePlayerPrefs.SetSecuredString("FacebookClientCachedPermissions", JsonSerializer.GenericListToArrayList<string>(value).toJson());
            }
            else
            {
                TactilePlayerPrefs.SetSecuredString("FacebookClientCachedPermissions", string.Empty);
            }
        }
    }

    public bool IsInitialized
    {
        get
        {
            return false;
        }
    }

    public bool IsSessionValid
    {
        get
        {
            return false;
        }
    }

    public bool HasPublishPermissions
    {
        get
        {
            foreach (string text in this.cachedPermissions)
            {
                if (text.ToLower() == "publish_actions")
                {
                    return true;
                }
            }
            return false;
        }
    }

    public bool HasFriendListPermissions
    {
        get
        {
            foreach (string text in this.cachedPermissions)
            {
                if (text.ToLower() == "user_friends")
                {
                    return true;
                }
            }
            return false;
        }
    }

    public string AccessToken
    {
        get
        {
            return string.Empty;
        }
    }

    public string OpenGraphBaseUrl
    {
        get
        {
            return "https://graph.facebook.com/" + this.openGraphVersion + "/";
        }
    }

    public bool LoginInProgress
    {
        get
        {
            return this.loginInProgress;
        }
    }

    public FacebookUser CachedMe
    {
        get
        {
            return this.cachedMe;
        }
    }

    public List<FacebookUser> CachedFriends
    {
        get
        {
            return this.cachedFriends;
        }
    }

    private string _FacebookSuffix
    {
        get
        {
            return this.facebookSuffix;
        }
    }

    private string _FacebookAppId
    {
        get
        {
            return this.facebookAppId;
        }
    }

    private bool _CancelLogin
    {
        get
        {
            return this.cancelLogin;
        }
    }

    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action FacebookLogout;

    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action FacebookLogin;

    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action FacebookInitComplete;

    private bool cancelLogin;

    private float lastTimeLogin;

    private bool loginInProgress;

    private string facebookSuffix;

    private string facebookAppId;

    private string facebookAppNamespace;

    private string openGraphVersion;

    private FacebookPortraitManager portraitManager;

    private FacebookUser cachedMe;

    private List<string> cachedPermissions;

    private List<FacebookUser> cachedFriends;

    private List<FacebookClient.FacebookLocalScore> localScores;

    private const string PREFS_CACHED_FACEBOOK_ME = "FacebookClientCachedFacebookMe";

    private const string PREFS_CACHED_FACEBOOK_FRIENDS = "FacebookClientCachedFacebookFriends";

    private const string PREFS_LOCAL_SCORES = "FacebookClientLocalScores";

    private const string PREFS_CACHED_PERMISSIONS = "FacebookClientCachedPermissions";

    public enum AppRequestFilter
    {
        AppUsers,
        AppNonUsers,
        None
    }

    public class FacebookLocalScore
    {
        [JsonSerializable("sc", null)]
        public long score { get; set; }

        [JsonSerializable("su", null)]
        public bool submitted { get; set; }
    }
}
