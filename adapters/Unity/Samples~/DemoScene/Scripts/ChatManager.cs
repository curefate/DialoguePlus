using DialoguePlus.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class ChatManager : MonoBehaviour
{
    public int typingDelay;
    public bool autoSkip;

    public Image chatBackground;
    public TextMeshProUGUI chatText;
    public TextMeshProUGUI talkerText;
    public MenuInventory inventory;
    public GameObject talkerPrefab;
    public ImageDictionary imageDictionary;
    public RectTransform left;
    public RectTransform right;
    public RectTransform mid;

    private Dictionary<string, Talker> talkerDict = new();

    private bool isTyping = false;
    private async Task PushText(string text, string talker, CancellationToken ct = default)
    {
        while (isTyping)
        {
            await Task.Delay(100, ct);
        }
        isTyping = true;

        talkerText.text = talker;
        chatText.text = "";
        foreach (char c in text)
        {
            ct.ThrowIfCancellationRequested();
            chatText.text += c;
            await Task.Delay(Mathf.RoundToInt(typingDelay * Time.deltaTime), ct);
        }
        isTyping = false;
    }

    private bool _isClicked = false;
    private async Task WaitForClick(CancellationToken ct = default)
    {
        if (autoSkip) return;
        _isClicked = false;
        while (!_isClicked)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                _isClicked = true;
            }
            await Task.Yield();
        }
    }

    private async Task<int> CreateOptions(List<string> options, CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<int>();
        inventory.ClearOptions();

        for (int i = 0; i < options.Count; i++)
        {
            int index = i;
            var button = inventory.AddOption(options[index]);
            button.onClick.AddListener(() =>
            {
                tcs.TrySetResult(index);
                inventory.ClearOptions();
            });
        }

        return await tcs.Task;
    }

    private async Task HandleDialogue(Runtime runtime, SIR_Dialogue dialogue)
    {
        var talker = GetTalkerByName(dialogue.Speaker);
        if (talker != null) talker.StartFloat(20, 15);
        await PushText((string)dialogue.Text.Evaluate(runtime), dialogue.Speaker);
        if (talker != null) talker.StopFloat();
        await WaitForClick();
    }

    private async Task<int> HandleMenu(Runtime runtime, SIR_Menu menu)
    {
        var options = menu.Options.Select(option => (string)option.Evaluate(runtime)).ToList();
        int selectedIndex = await CreateOptions(options);
        return selectedIndex;
    }

    private void ShowUI()
    {
        chatBackground.gameObject.SetActive(true);
        talkerText.gameObject.SetActive(true);
        chatText.gameObject.SetActive(true);
    }

    private void HideUI()
    {
        chatBackground.gameObject.SetActive(false);
        talkerText.gameObject.SetActive(false);
        chatText.gameObject.SetActive(false);
    }

    private Vector2 GetPosByName(string position)
    {
        switch (position.ToLower())
        {
            case "left":
                return left.anchoredPosition;
            case "right":
                return right.anchoredPosition;
            case "mid":
            case "center":
                return mid.anchoredPosition;
            default:
                return mid.anchoredPosition;
        }
    }

    private Talker? GetTalkerByName(string talker)
    {
        if (talkerDict.ContainsKey(talker))
        {
            return talkerDict[talker];
        }
        var talkerObjs = FindObjectsByType<Talker>(FindObjectsSortMode.None);
        foreach (var obj in talkerObjs)
        {
            if (obj.name == talker)
            {
                talkerDict[talker] = obj;
                return obj;
            }
        }
        return null;
    }

    private void ShowTalker(string talker, string position)
    {
        var talkerObj = GetTalkerByName(talker);
        var pos = GetPosByName(position);
        if (talkerObj == null)
        {
            var img = imageDictionary.GetSprite(talker);
            if (img == null)
            {
                Debug.LogWarning($"Talker image '{talker}' not found in ImageDictionary.");
                return;
            }
            talkerObj = Instantiate(talkerPrefab, transform.parent).GetComponent<Talker>();
            talkerObj.name = talker;
            talkerObj.transform.SetSiblingIndex(0);
            talkerObj.img.texture = img;
            talkerDict[talker] = talkerObj;
        }
        talkerObj.MoveToPos(pos);
        talkerObj.Fade(true);
    }

    private void HideTalker(string talker)
    {
        var talkerObj = GetTalkerByName(talker);
        if (talkerObj != null)
        {
            talkerObj.Fade(false);
        }
    }

    private void MoveTalker(string talker, string position)
    {
        var talkerObj = GetTalkerByName(talker);
        var pos = GetPosByName(position);
        if (talkerObj != null)
        {
            talkerObj.MoveToPos(pos);
        }
    }

    void Start()
    {
        HideUI();
        DialoguePlusAdapter.Instance.Executer.OnDialogueAsync = HandleDialogue;
        DialoguePlusAdapter.Instance.Executer.OnMenuAsync = HandleMenu;
        DialoguePlusAdapter.Instance.Runtime.Functions.AddFunction(HideUI);
        DialoguePlusAdapter.Instance.Runtime.Functions.AddFunction(ShowUI);
        DialoguePlusAdapter.Instance.Runtime.Functions.AddFunction<string, string>(ShowTalker);
        DialoguePlusAdapter.Instance.Runtime.Functions.AddFunction<string>(HideTalker);
        DialoguePlusAdapter.Instance.Runtime.Functions.AddFunction<string, string>(MoveTalker);
        Debug.Log("ChatManager initialized");
    }
}
