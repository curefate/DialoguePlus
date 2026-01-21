
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ImageDictionary", menuName = "Custom/Image Dictionary")]
public class ImageDictionary : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public string key = "";
        public Texture texture = null;
    }

    public List<Entry> entries = new();

    private Dictionary<string, Texture> dict = null;

    public Texture GetSprite(string key)
    {
        if (dict == null)
        {
            dict = new Dictionary<string, Texture>();
            foreach (var e in entries)
            {
                if (!dict.ContainsKey(e.key))
                    dict.Add(e.key, e.texture);
            }
        }

        if (dict.TryGetValue(key, out Texture tex))
            return tex;

        Debug.LogWarning($"ImageDictionary: Don't have image for key = {key}");
        return null;
    }
}
