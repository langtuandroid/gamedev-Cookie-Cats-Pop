using System;
using System.Collections.Generic;

public class VoiceAssigner
{
    public VoiceAssigner(MultiTrack track)
    {
        for (int i = 0; i < track.voices.Count; i++)
        {
            if (track.voices[i] != null && track.voices[i].clip != null)
            {
                this.available.Add((VoiceType)i, track.voices[i]);
            }
        }
    }

    public MultiTrack.Voice GrabVoice(VoiceType preferred)
    {
        MultiTrack.Voice result = null;
        while (this.available.Count > 0)
        {
            if (this.available.ContainsKey(preferred))
            {
                result = this.available[preferred];
                this.available.Remove(preferred);
                break;
            }
            preferred = (VoiceType)(((int)preferred + 1) % 6);
        }
        return result;
    }

    private Dictionary<VoiceType, MultiTrack.Voice> available = new Dictionary<VoiceType, MultiTrack.Voice>();
}
