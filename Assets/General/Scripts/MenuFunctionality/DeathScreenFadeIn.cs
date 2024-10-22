using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathScreenFadeIn : MonoBehaviour
{
    private CanvasGroup thestuff;

    public float FadeTime;

    private void Awake()
    {
        thestuff = this.GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        thestuff.alpha = 0;
        StartCoroutine(UiFadeIn(thestuff));
    }

    private IEnumerator UiFadeIn(CanvasGroup thethings)
    {
        float elapsedtime = 0;

        while (thestuff.alpha < 1)
        {
            elapsedtime += Time.deltaTime;
            thethings.alpha = Mathf.Lerp(0, 1, elapsedtime / FadeTime);

            yield return null;
        }
    }

}
