using UnityEngine;

public class StartBtn : MonoBehaviour
{
    public async void OnStartButtonClicked()
    {
        Debug.Log("Start button clicked!");
        Destroy(gameObject);
        await DialoguePlusAdapter.Instance.ExecuteToEnd("Assets/Samples/DialoguePlus/1.0.0/DialoguePlus Sample Scene/DPScript/s1.dp");
    }
}   
