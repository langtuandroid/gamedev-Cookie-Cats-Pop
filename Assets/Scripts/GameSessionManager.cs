using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

public class GameSessionManager : IGameSessionManager
{
	private GameSessionManager([NotNull] GameSessionManager.IGameSessionManagerProvider gameSessionManagerProvider)
	{
		if (gameSessionManagerProvider == null)
		{
			throw new ArgumentNullException("gameSessionManagerProvider");
		}
		UnityEngine.Object.DontDestroyOnLoad(new GameObject("GameSessionManagerLifecycle", new Type[]
		{
			typeof(GameSessionManager.GameSessionManagerLifecycle)
		}));
		this.provider = gameSessionManagerProvider;
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action NewSessionStarted;

	private GameSessionConfig Config
	{
		get
		{
			return this.provider.Configuration;
		}
	}

	public static GameSessionManager Instance { get; private set; }

	private static int SecondsSinceLastMinimise
	{
		get
		{
			return (int)(DateTime.UtcNow - GameSessionManager.LastMinimise).TotalSeconds;
		}
	}

	private static DateTime LastMinimise
	{
		get
		{
			string name = "DirectorLastMinimise";
			DateTime dateTime = new DateTime(2000, 1, 1);
			string @string = TactilePlayerPrefs.GetString(name, dateTime.ToString("0:d/M/yyyy HH:mm:ss"));
			return DateTime.ParseExact(@string, "0:d/M/yyyy HH:mm:ss", null);
		}
		set
		{
			TactilePlayerPrefs.SetString("DirectorLastMinimise", value.ToString("0:d/M/yyyy HH:mm:ss"));
		}
	}

	private static string LastVersion
	{
		get
		{
			return TactilePlayerPrefs.GetString("GameSessionManagerLastMinorVersion", string.Empty);
		}
		set
		{
			TactilePlayerPrefs.SetString("GameSessionManagerLastMinorVersion", value);
		}
	}

	public static GameSessionManager CreateInstance(GameSessionManager.IGameSessionManagerProvider provider)
	{
		if (GameSessionManager.Instance != null)
		{
			throw new Exception("Instance of GameSessionManager already exsists!");
		}
		GameSessionManager.Instance = new GameSessionManager(provider);
		return GameSessionManager.Instance;
	}

	public void StartNewSession()
	{
		this.hasGottenNewSessionAfterBoot = true;
		this.consumedSessionsByRecipient.Clear();
		if (this.NewSessionStarted != null)
		{
			this.NewSessionStarted();
		}
	}

	public bool HasPendingSessionForRecipient(object recipient)
	{
		bool result;
		if (this.consumedSessionsByRecipient.TryGetValue(recipient, out result))
		{
			return result;
		}
		return this.hasGottenNewSessionAfterBoot;
	}

	public void ConsumeSessionForRecipient(object recipient)
	{
		this.consumedSessionsByRecipient[recipient] = false;
	}

	private void CheckNewSession()
	{
		if (GameSessionManager.SecondsSinceLastMinimise > this.Config.PlayPauseForNewSession || !SystemInfoHelper.IsMinorVersionMatch(GameSessionManager.LastVersion) || !SystemInfoHelper.IsPatchVersionMatch(GameSessionManager.LastVersion))
		{
			this.StartNewSession();
		}
		GameSessionManager.LastMinimise = DateTime.UtcNow;
		GameSessionManager.LastVersion = SystemInfoHelper.BundleShortVersion;
	}

	private readonly Dictionary<object, bool> consumedSessionsByRecipient = new Dictionary<object, bool>();

	private bool hasGottenNewSessionAfterBoot;

	private const string DATE_TIME_FORMAT = "0:d/M/yyyy HH:mm:ss";

	private const string PLAYER_PREFS_KEY = "DirectorLastMinimise";

	private const string LAST_VERSION_KEY = "GameSessionManagerLastMinorVersion";

	private readonly GameSessionManager.IGameSessionManagerProvider provider;

	private class GameSessionManagerLifecycle : MonoBehaviour
	{
		private void OnApplicationFocus(bool hasFocus)
		{
			if (hasFocus)
			{
				if (GameSessionManager.Instance.provider.IsOnAMap)
				{
					GameSessionManager.Instance.CheckNewSession();
				}
			}
			else
			{
				GameSessionManager.LastMinimise = DateTime.UtcNow;
			}
		}

		private void OnApplicationQuit()
		{
			GameSessionManager.LastMinimise = DateTime.UtcNow;
			GameSessionManager.LastVersion = SystemInfoHelper.BundleShortVersion;
		}

		private void Start()
		{
			GameSessionManager.Instance.CheckNewSession();
		}
	}

	public interface IGameSessionManagerProvider
	{
		GameSessionConfig Configuration { get; }

		bool IsOnAMap { get; }
	}
}
