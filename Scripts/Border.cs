using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class Border : MonoBehaviour
{
    public float effectDistance = 5f;
    public Color effectColor = Color.black;

    private RectTransform rectTransform;
    private Image[] borders = new Image[4];
    private readonly string[] names = { "Top", "Bottom", "Left", "Right" };

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        CreateBorders();
        UpdateBorders();
    }

    void OnEnable()
    {
        SetBordersActive(true);
        UpdateBorders();
    }

    void OnDisable()
    {
        SetBordersActive(false);
    }

    void OnValidate()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        CreateBorders();
        UpdateBorders();
    }

    void CreateBorders()
    {
        for (int i = 0; i < 4; i++)
        {
            if (borders[i] == null)
            {
                Transform existing = transform.Find("Border_" + names[i]);
                if (existing != null)
                {
                    borders[i] = existing.GetComponent<Image>();
                }
                else
                {
                    GameObject borderObj = new GameObject("Border_" + names[i], typeof(RectTransform), typeof(Image));
                    borderObj.transform.SetParent(transform, false);
                    borders[i] = borderObj.GetComponent<Image>();
                }
            }
        }
    }

    void UpdateBorders()
    {
        Vector2 size = rectTransform.sizeDelta;
        float totalHeight = (effectDistance/2); // adds top+bottom thickness

        for (int i = 0; i < 4; i++)
        {
            var rt = borders[i].rectTransform;
            borders[i].color = effectColor;

            switch (i)
            {
                case 0: // Top
                    rt.anchorMin = new Vector2(0, 1);
                    rt.anchorMax = new Vector2(1, 1);
                    rt.sizeDelta = new Vector2(0, effectDistance);
                    rt.anchoredPosition = new Vector2(0, totalHeight);
                    break;

                case 1: // Bottom
                    rt.anchorMin = new Vector2(0, 0);
                    rt.anchorMax = new Vector2(1, 0);
                    rt.sizeDelta = new Vector2(0, effectDistance);
                    rt.anchoredPosition = new Vector2(0, -totalHeight);
                    break;

                case 2: // Left
                    rt.anchorMin = new Vector2(0, 0);
                    rt.anchorMax = new Vector2(0, 1);
                    rt.sizeDelta = new Vector2(effectDistance, (effectDistance * 2));
                    rt.anchoredPosition = new Vector2(-totalHeight, 0);
                    break;

                case 3: // Right
                    rt.anchorMin = new Vector2(1, 0);
                    rt.anchorMax = new Vector2(1, 1);
                    rt.sizeDelta = new Vector2(effectDistance, (effectDistance * 2));
                    rt.anchoredPosition = new Vector2(totalHeight, 0);
                    break;
            }
        }
    }

    void SetBordersActive(bool state)
    {
        foreach (var b in borders)
        {
            if (b != null && b.gameObject != null)
                b.gameObject.SetActive(state);
        }
    }
}
