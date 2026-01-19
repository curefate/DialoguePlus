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

    public Image chatBackground;
    public TextMeshProUGUI chatText;
    public TextMeshProUGUI talkerText;
    public MenuInventory inventory;

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
        Debug.Log($"[D+] Handling dialogue: {dialogue.Text}");
        await PushText((string)dialogue.Text.Evaluate(runtime), dialogue.Speaker);
        await WaitForClick();
    }

    private async Task<int> HandleMenu(Runtime runtime, SIR_Menu menu)
    {
        Debug.Log($"[D+] Handling menu: {menu.Options}");
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

    void Start()
    {
        HideUI();
        DialoguePlusAdapter.Instance.Executer.OnDialogueAsync = HandleDialogue;
        DialoguePlusAdapter.Instance.Executer.OnMenuAsync = HandleMenu;
        DialoguePlusAdapter.Instance.Runtime.Functions.AddFunction(HideUI);
        DialoguePlusAdapter.Instance.Runtime.Functions.AddFunction(ShowUI);
        Debug.Log("ChatManager initialized");
    }
}
