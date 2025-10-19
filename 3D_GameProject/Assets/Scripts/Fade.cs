using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    private Animator eyeFadeAnim;
    public int blinkCount = 2;
    public float blinkDelay = 0.8f;

    private bool hasBlinked = false;

    private void Start()
    {
        eyeFadeAnim = GetComponent<Animator>();

        if (!hasBlinked)
        {
            hasBlinked = true;
            StartCoroutine(BlinkSequence());
        }
    }


    private IEnumerator BlinkSequence()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            yield return new WaitForSeconds(blinkDelay);
            // Fade out (close eyes)
            eyeFadeAnim.SetBool("fadeout", true);
            yield return new WaitForSeconds(blinkDelay - 1f);

            // Fade in (open eyes)
            eyeFadeAnim.SetBool("fadeout", false);
        }
    }
}
