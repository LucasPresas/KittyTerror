using UnityEngine;

[CreateAssetMenu(fileName = "ThoughtRegistry", menuName = "KittyTerror/ThoughtRegistry")]
public class ThoughtRegistry : ScriptableObject
{
    public ThoughtEntry[] entries;

    public string GetThought(string thoughtId)
    {
        foreach (var e in entries)
            if (e.thoughtId == thoughtId) return e.text;
        return null;
    }
}

[System.Serializable]
public class ThoughtEntry
{
    public string thoughtId;
    [TextArea(2, 4)] public string text;
}
