using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlusOneScript : MonoBehaviour
{
    [SerializeField]
    private float upwardVelocity;
    [SerializeField]
    private float timeToFade;

    private TextMeshProUGUI plusOneText;
    
    void OnEnable()
    {
        GetComponent<Rigidbody>().velocity = transform.up * upwardVelocity;
        plusOneText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(FadingCoroutine());
    }
    
    private IEnumerator FadingCoroutine()
    {
        float startTime = Time.time;
        float lerpingValue = 0;
        Color startColor = plusOneText.color;
        while(lerpingValue < 1)
        {
            lerpingValue = (Time.time - startTime) / timeToFade;
            startColor.a = 1 - lerpingValue;
            plusOneText.color = startColor;
            yield return null;
        }
        UIManager.Instance.LoadIntoPool(gameObject);
    }
}
