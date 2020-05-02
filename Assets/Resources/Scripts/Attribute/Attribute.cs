using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attribute : MonoBehaviour
{
    public int attributeValue = 0;

    Gradient gradient = new Gradient();
    List<GradientColorKey> colorKey = new List<GradientColorKey>();
    List<GradientAlphaKey> alphaKey = new List<GradientAlphaKey>();

    void Start()
    {
        attributeValue = GameManager.instance.maxAttributeValue / 2;
        AnimateAttribute(GameManager.instance.attributeAnimationTime);

        SetGradient();
    }

    void SetGradient()
    {
        colorKey.Add(new GradientColorKey(GameManager.instance.badAttributeColor, 0.2f));
        colorKey.Add(new GradientColorKey(GameManager.instance.goodAttributeColor, 0.5f));
        colorKey.Add(new GradientColorKey(GameManager.instance.badAttributeColor, 0.8f));

        alphaKey.Add(new GradientAlphaKey(1f, 0f));
        alphaKey.Add(new GradientAlphaKey(1f, 0f));
        alphaKey.Add(new GradientAlphaKey(1f, 0f));

        gradient.SetKeys(colorKey.ToArray(), alphaKey.ToArray());
        GetComponent<Renderer>().material.color = (gradient.Evaluate((float)attributeValue / (float)GameManager.instance.maxAttributeValue));
    }

    public void AnimateAttribute(float animationTime)
    {
        // Animate color
        GetComponent<Renderer>().material.color = (gradient.Evaluate((float)attributeValue / (float)GameManager.instance.maxAttributeValue));
        
        // Scale animation
        StartCoroutine(AttributeChangesAnimationPart1(animationTime));
    }

    IEnumerator AttributeChangesAnimationPart1(float animationTime)
    {
        float startTime = 0;

        Vector3 currentScale = transform.localScale;
        Vector3 targetScale = currentScale * GameManager.instance.attributeScaleMultiplier;

        while (startTime < animationTime)
        {
            transform.localScale = Vector3.Lerp(currentScale, targetScale, (startTime / animationTime));

            startTime += Time.deltaTime;

            yield return null;
        }

        transform.localScale = targetScale;

        yield return StartCoroutine(AttributeChangesAnimationPart2(targetScale, currentScale, animationTime));
    }

    IEnumerator AttributeChangesAnimationPart2(Vector3 currentScale, Vector3 targetScale, float animationTime)
    {
        float startTime = 0;

        while (startTime < animationTime)
        {
            transform.localScale = Vector3.Lerp(currentScale, targetScale, (startTime / animationTime));

            startTime += Time.deltaTime;

            yield return null;
        }

        transform.localScale = targetScale;
    }
}
