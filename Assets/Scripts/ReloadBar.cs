using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadBar : MonoBehaviour
{
    public Image reloadBarBackground;
    public Image nodeImage;
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;
    public float reloadDuration = 2f;
    public AnimationCurve reloadCurve;

    private Transform playerTransform;
    private Vector3 nodeOriginalPosition;
    private CanvasGroup reloadBar;
    private CanvasGroup node;
    private Vector3 velocity = Vector3.zero; // For SmoothDamp
    private Vector3 offset = new Vector3(0, 0.9f, 0); // Adjust this to position the bar above the player

    void Start()
    {

        nodeOriginalPosition = nodeImage.transform.localPosition;  // Store the original position

        reloadBar = reloadBarBackground.GetComponent<CanvasGroup>();
        node = nodeImage.GetComponent<CanvasGroup>();

        // Ensure both CanvasGroups are properly initialized
        if (reloadBar == null)
        {
            reloadBar = reloadBarBackground.gameObject.AddComponent<CanvasGroup>();
        }
        if (node == null)
        {
            node = nodeImage.gameObject.AddComponent<CanvasGroup>();
        }

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;


        node.alpha = 0;
        reloadBar.alpha = 0;
        reloadBarBackground.fillAmount = 0;
    }

    void Update()
    {
        // Calculate the world position offset relative to the player's position
        Vector3 targetWorldPosition = playerTransform.position + offset;

        // Convert the target world position to screen position
        Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(targetWorldPosition);

        // Smoothly interpolate the reload bar's position
        transform.position = Vector3.SmoothDamp(transform.position, targetScreenPosition, ref velocity, 0.1f);
    }

    public void StartReloading()
    {
        StartCoroutine(ReloadSequence());
    }

    private IEnumerator ReloadSequence()
    {
       yield return StartCoroutine(FadeIn());

        float elapsedTime = 0f;
        while (elapsedTime < reloadDuration)
        {
            float progress = elapsedTime / reloadDuration;
            float curvedProgress = reloadCurve.Evaluate(progress);//Using a curve to adjust reload slider speed
            nodeImage.transform.localPosition = new Vector3(Mathf.Lerp(reloadBarBackground.rectTransform.rect.width / 2, -reloadBarBackground.rectTransform.rect.width / 2, curvedProgress), 0, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure node reaches the end position
        nodeImage.transform.localPosition = new Vector3(reloadBarBackground.rectTransform.rect.width / 2, 0, 0);

        yield return StartCoroutine(FadeOut());
        // Reset node position to original after fading out
        nodeImage.transform.localPosition = nodeOriginalPosition;
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            reloadBar.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeInDuration);
            node.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        reloadBar.alpha = 1;
        node.alpha = 1;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            reloadBar.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration);
            node.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        reloadBar.alpha = 0;
        node.alpha = 0;
    }
}
