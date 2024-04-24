using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using JetBrains.Annotations;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using UnityEngine;

public class HighscorePanel : MonoBehaviour
{
    private CloudClient CloudClient
    {
        get
        {
            return ManagerRepository.Get<CloudClient>();
        }
    }

    public void Initialize(LevelProxy level)
    {
        this.level = level;
        this.inviteOthersPrefab.SetActive(false);
        this.loadingPivot.SetActive(false);
        this.connectingErrorPivot.SetActive(false);
        this.playWithFriendsPivot.SetActive(false);
        this.noScoresYet.SetActive(false);
        if (level.LevelCollection is MainLevelDatabase)
        {
            this.loadingPivot.SetActive(this.CloudClient.HasValidUser);
            this.playWithFriendsPivot.SetActive(!this.CloudClient.HasValidUser);
            if (this.CloudClient.HasValidUser)
            {
                this.fiber.Start(this.FetchScores());
            }
        }
    }

    private void OnDestroy()
    {
        this.fiber.Terminate();
    }

    private void Update()
    {
        this.fiber.Step();
    }

    [UsedImplicitly]
    private void FacebookPlayWithFriendsClicked(UIEvent e)
    {
        
    }
    

    private IEnumerator FetchScores()
    {
        List<CloudScore> scores = new List<CloudScore>();
        if (this.level.LevelCollection is MainLevelDatabase)
        {
            scores = LeaderboardManager.Instance.GetCachedCloudScores(this.level.Index);
        }
        
        if (scores != null && scores.Count > 0)
        {
            this.UpdateUI(scores);
            this.loadingPivot.SetActive(false);
        }
        else
        {
            this.loadingPivot.SetActive(true);
            this.connectingErrorPivot.SetActive(false);
            this.noScoresYet.SetActive(false);
        }
        if (this.level.LevelCollection is MainLevelDatabase)
        {
            yield return LeaderboardManager.Instance.UpdateCachedCloudScoresCr(this.level.Index);
            scores = LeaderboardManager.Instance.GetCachedCloudScores(this.level.Index);
            this.loadingPivot.SetActive(false);
            this.UpdateUI(scores);
        }
      
        yield break;
    }

    private void UpdateUI(List<CloudScore> scores)
    {
        if (scores == null)
        {
            this.connectingErrorPivot.SetActive(true);
            this.noScoresYet.SetActive(false);
            this.connectionErrorLabel.text = L._("No Scores available");
            base.gameObject.SetActive(false);
            return;
        }
        UIListPanel component = base.GetComponent<UIListPanel>();
        component.DestroyAllContent();
        if (scores.Count == 0)
        {
            this.noScoresYet.SetActive(true);
            this.CreateAskFriendsCell(component);
        }
        else
        {
            this.connectingErrorPivot.SetActive(false);
            this.noScoresYet.SetActive(false);
            Vector2 vector = new Vector2(0f, 0f);
            Vector2 elementSize = new Vector2(0f, 0f);
            component.BeginAdding();
            int num = 0;
            for (int i = 0; i < scores.Count; i++)
            {
                num++;
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.elementPrefab);
                gameObject.SetActive(true);
                component.AddToContent(gameObject.GetComponent<UIElement>());
                HighscoreCell component2 = gameObject.GetComponent<HighscoreCell>();
                component2.Initialize(scores[i], i + 1);
                if ((this.CloudClient.CachedMe != null && this.CloudClient.CachedMe.CloudId == scores[i].UserId) || i == 0)
                {
                    vector = new Vector2(component2.GetElementSize().x * (float)i, gameObject.transform.localPosition.y);
                    elementSize = component2.GetElementSize();
                }
                if (this.CloudClient.HasValidUser)
                {
                    int num2 = 4;
                    if (num % num2 == 0 || (i == scores.Count - 1 && num < num2))
                    {
                        this.CreateAskFriendsCell(component);
                    }
                }
                HighscoreCell component3 = gameObject.GetComponent<HighscoreCell>();
                component3.AppearInPanel();
            }
            component.EndAdding();
            component.SetScroll(new Vector2
            {
                x = -Mathf.Max(0f, component.TotalContentSize.x - component.Size.x) - component.Size.x * 0.5f,
                y = -component.TotalContentSize.y * 0.5f
            });
            component.SetScroll(new Vector2(-vector.x - elementSize.x * 0.5f, 0f));
            if (component.TotalContentSize.x < component.Size.x)
            {
                component.SetScroll(new Vector2(-0.5f * component.Size.x, component.ScrollRoot.localPosition.y));
            }
        }
        if (this.UIUpdated != null)
        {
            this.UIUpdated();
        }
    }

    private void CreateAskFriendsCell(UIListPanel listPanel)
    {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.inviteOthersPrefab);
        gameObject.GetComponentInChildren<UIButton>().receiver = base.gameObject;
        gameObject.GetComponentInChildren<UIButton>().methodName = "ButtonAskOther";
        listPanel.AddToContent(gameObject.GetComponent<UIElement>());
    }

    [UsedImplicitly]
    private void ButtonAskOther(UIEvent e)
    {
        this.fiber.Start(this.HandleLevelScoreListInviteFriendsClickedCr());
    }

    private IEnumerator HandleLevelScoreListInviteFriendsClickedCr()
    {
        if (this.processingInviteRequest)
        {
            yield break;
        }
        yield return new Fiber.OnExit(delegate ()
        {
            this.processingInviteRequest = false;
        });
        this.processingInviteRequest = true;
        yield break;
    }

    public GameObject elementPrefab;

    public GameObject inviteOthersPrefab;

    public GameObject loadingPivot;

    public GameObject connectingErrorPivot;

    public GameObject playWithFriendsPivot;

    public GameObject noScoresYet;

    public UILabel connectionErrorLabel;

    public Action UIUpdated;

    private LevelProxy level;

    private Fiber fiber = new Fiber(FiberBucket.Manual);

    private bool processingInviteRequest;
}
