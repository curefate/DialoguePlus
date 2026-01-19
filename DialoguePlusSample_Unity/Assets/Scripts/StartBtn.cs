using UnityEngine;

public class StartBtn : MonoBehaviour
{
    public async void OnStartButtonClicked()
    {
        Debug.Log("Start button clicked!");
        await DialoguePlusAdapter.Instance.ExecuteToEnd("Assets/DPScript/s1.dp");
        Destroy(gameObject);
    }
}
