using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Talker : MonoBehaviour
{
    private int floatSpeed = 1;
    private int floatHeight = 10;

    private RectTransform rect;
    [HideInInspector]
    public RawImage img;
    private bool isFloating = false;
    private Vector3 originalPos;
    private float floatTimer = 0f;
    private bool _isVisible = false;
    private int fadeSpeed = 10;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;
        img = GetComponent<RawImage>();
    }

    private void Update()
    {
        if (isFloating)
        {
            floatTimer += Time.deltaTime * floatSpeed;

            float offsetY = Mathf.Sin(floatTimer) * floatHeight;
            rect.anchoredPosition = originalPos + new Vector3(0, offsetY, 0);
        }
        else
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, originalPos, Time.deltaTime * floatSpeed);
        }

        if (_isVisible)
        {
            Color color = img.color;
            color.a = Mathf.Min(color.a + Time.deltaTime * fadeSpeed, 1f);
            img.color = color;
        }
        else
        {
            Color color = img.color;
            color.a = Mathf.Max(color.a - Time.deltaTime * fadeSpeed, 0f);
            img.color = color;
        }
    }

    [DPFunction]
    public void StartFloat(int speed, int height)
    {
        originalPos = rect.anchoredPosition;
        floatSpeed = speed;
        floatHeight = height;

        isFloating = true;
        floatTimer = 0f;
    }

    [DPFunction]
    public void StopFloat()
    {
        isFloating = false;
        rect.anchoredPosition = originalPos;
    }

    [DPFunction]
    public void Rotate(float angleX, float angleY, float angleZ)
    {
        rect.Rotate(new Vector3(angleX, angleY, angleZ));
    }

    [DPFunction]
    public void Fade(bool isVisible, int speed = 10)
    {
        _isVisible = isVisible;
        fadeSpeed = speed;
    }

    public void MoveToPos(Vector2 position)
    {
        originalPos = position;
    }
}
