using System;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

public static class MusicSystemBuilder
{
	public static void Build(AudioManager audioManager, IFlowStack flowStack)
	{
		new MusicSystemBuilder.MusicManager(audioManager, flowStack);
	}

	private class MusicManager
	{
		public MusicManager(AudioManager audioManager, IFlowStack flowStack)
		{
			this.audioManager = audioManager;
			flowStack.Changed += this.HandleFlowStackChanged;
		}

		private void HandleFlowStackChanged(IFlow newFlow, IFlow oldFlow)
		{
			if (newFlow is MapFlow)
			{
				this.audioManager.SetMusic(SingletonAsset<SoundDatabase>.Instance.mapMusic, true);
			}
		}

		private readonly AudioManager audioManager;
	}
}
