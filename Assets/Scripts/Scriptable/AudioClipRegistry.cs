using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipRegistry", menuName = "KittyTerror/AudioClipRegistry")]
public class AudioClipRegistry : ScriptableObject
{
    public AudioEntry[] entries;

    public AudioClip GetClip(string clipId)
    {
        foreach (var e in entries)
            if (e.clipId == clipId) return e.clip;
        return null;
    }
}

[System.Serializable]
public class AudioEntry
{
    public string clipId;
    public AudioClip clip;
}
