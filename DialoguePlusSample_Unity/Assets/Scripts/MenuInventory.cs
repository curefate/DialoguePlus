using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuInventory : MonoBehaviour
{
    public GameObject optionPrefab;
    public float spacing = 10f;
    public float lerpSpeed = 5f;

    public void ClearOptions()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public Button AddOption(string option)
    {
        var buttonObject = Instantiate(optionPrefab, transform);
        var button = buttonObject.GetComponent<Button>();
        var text = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
        text.text = option;
        button.onClick.AddListener(() => OnOptionSelected(option));
        return button;
    }

    private void OnOptionSelected(string option)
    {
        Debug.Log($"Option selected: {option}");
    }

    private void ArrangeChildren()
    {
        int childCount = transform.childCount;

        float totalHeight = spacing * (childCount - 1);
        float startY = -totalHeight / 2f;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child == null) continue;

            Vector3 targetLocalPos = new(
                0f,
                startY + spacing * i,
                0f
            );

            child.localPosition = Vector3.Lerp(
                child.localPosition,
                targetLocalPos,
                Time.deltaTime * lerpSpeed
            );
        }
    }

    private void Update()
    {
        ArrangeChildren();
    }

    private void Awake()
    {
        if (optionPrefab == null)
        {
            Debug.LogError("Option Prefab is not assigned in MenuInventory.");
        }
    }

}
