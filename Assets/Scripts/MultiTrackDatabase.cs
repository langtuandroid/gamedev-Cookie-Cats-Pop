using System;
using System.Collections.Generic;

[SingletonAssetPath("Assets/[Database]/Resources/MultiTrackDatabase.asset")]
public class MultiTrackDatabase : SingletonAsset<MultiTrackDatabase>
{
	public MultiTrack GetRandomVictorySong()
	{
		if (this.victorySongPool.Count <= 0)
		{
			foreach (MultiTrackDatabase.MultiTrackEntry multiTrackEntry in this.victoryMultiTracks)
			{
				this.victorySongPool.Add(multiTrackEntry.probability, multiTrackEntry.track);
			}
		}
		MultiTrack multiTrack = this.victorySongPool.PickRandomItem(false);
		this.victorySongPool.Remove(multiTrack);
		return multiTrack;
	}

	public int lowBand = 2;

	public int highBand = 12;

	public MultiTrack title;

	public List<MultiTrackDatabase.MultiTrackEntry> victoryMultiTracks = new List<MultiTrackDatabase.MultiTrackEntry>();

	private Lottery<MultiTrack> victorySongPool = new Lottery<MultiTrack>();

	[Serializable]
	public class MultiTrackEntry
	{
		public MultiTrack track;

		public float probability = 1f;
	}
}
