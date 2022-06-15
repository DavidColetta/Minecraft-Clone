using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private Gradient gradient;
    private Image fill;
    private void Awake() {
        slider = GetComponent<Slider>();
        fill = transform.Find("Fill").GetComponent<Image>();
    }
    ///<summary>
    ///Sets the value of the bar as a percentage from 0 to 1.
    ///</summary>
    public void SetPercent(float percent){
        slider.value = percent;
        fill.color = gradient.Evaluate(percent);
    }
}
