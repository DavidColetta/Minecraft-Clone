using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    private static Tooltip instance;
    private TextMeshProUGUI tooltipText;
    private RectTransform backgroundRectTransform;
    private RectTransform canvasRectTransform;
    private RectTransform thisRect;
    private void Awake() {
        instance = this;
        thisRect = transform.GetComponent<RectTransform>();
        backgroundRectTransform = transform.Find("Tooltip Background").GetComponent<RectTransform>();
        tooltipText = transform.Find("Tooltip Text").GetComponent<TextMeshProUGUI>();
        canvasRectTransform = transform.root.GetComponent<RectTransform>();

        HideTooltip();
    }
    private void Update() {
        transform.position = Input.mousePosition;

        Vector2 anchoredPosition = thisRect.anchoredPosition;
        if (anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width){
            anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
        }
        if (anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height){
            anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height;
        }
        thisRect.anchoredPosition = anchoredPosition;
    }
    public void ShowTooltip(string tooltipString){
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        tooltipText.SetText(tooltipString);
        tooltipText.ForceMeshUpdate();
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth, tooltipText.preferredHeight);
        backgroundRectTransform.sizeDelta = backgroundSize;

        Update();
    }
    public void HideTooltip(){
        gameObject.SetActive(false);
    }

    public static void ShowTooltip_Static(string tooltipString){
        instance.ShowTooltip(tooltipString);
    }
    public static void HideTooltip_Static(){
        instance.HideTooltip();
    }
}
