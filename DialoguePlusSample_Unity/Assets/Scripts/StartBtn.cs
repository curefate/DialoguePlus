using UnityEngine;

public class StartBtn : MonoBehaviour
{
    public async void OnStartButtonClicked()
    {
        Debug.Log("Start button clicked!");
        Destroy(gameObject);
        await DialoguePlusAdapter.Instance.ExecuteToEnd("Assets/DPScript/s1.dp");
    }
}   
