using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockableManager : MonoBehaviour
{
    public static UnlockableManager instance;

    public GameObject unlockablePrefab;
    public Transform parent;
    [Range(0.1f, 2)]
    public float animationTime = 1;

    int unlockChance = 20;


    void Awake()
    {
        instance = this;
    }

    public void RandomUnlock()
    {
        int a = Random.Range(0, 101);
        int c = Random.Range(0, 101);
        int u = Random.Range(0, 101);

        if (a <= unlockChance)
        {
            Unlock("Achievement unlocked!");
        }
        else if (c <= unlockChance)
        {
            Unlock("Challenge completed!");
        }
        else if (u <= unlockChance)
        {
            Unlock("Upgrade unlocked!");
        }
    }

    public void Unlock(string unlockText)
    {
        GameObject unlockableObject = Instantiate(unlockablePrefab);
        unlockableObject.GetComponentInChildren<Text>().text = unlockText;
        unlockableObject.transform.SetParent(parent);
        unlockableObject.transform.localPosition = unlockablePrefab.transform.position;
        unlockableObject.transform.localRotation = unlockablePrefab.transform.rotation;
        unlockableObject.transform.localScale = unlockablePrefab.transform.localScale;

        StartCoroutine(UnlockedStartAnimation(unlockableObject, animationTime));
    }

    IEnumerator UnlockedStartAnimation(GameObject unlockableObject, float animationTime)
    {
        float startTime = 0;
        Vector3 targetScale = unlockableObject.transform.localScale;
        Vector3 startScale = new Vector3(0, targetScale.y, targetScale.z);

        while (startTime < animationTime)
        {
            float progress = startTime / animationTime;

            unlockableObject.transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

            startTime += Time.deltaTime;

            yield return null;
        }

        unlockableObject.transform.localScale = targetScale;

        // Next animation
        yield return StartCoroutine(UnlockedMiddleAnimation(unlockableObject, animationTime));
    }

    IEnumerator UnlockedMiddleAnimation(GameObject unlockableObject, float animationTime)
    {
        float startTime = 0;

        while (startTime < animationTime)
        {
            startTime += Time.deltaTime;

            yield return null;
        }

        // Next animation
        yield return StartCoroutine(UnlockedEndAnimation(unlockableObject, animationTime));
    }

    IEnumerator UnlockedEndAnimation(GameObject unlockableObject, float animationTime)
    {
        float startTime = 0;
        Vector3 startScale = unlockableObject.transform.localScale;
        Vector3 targetScale = new Vector3(0, unlockableObject.transform.localScale.y, unlockableObject.transform.localScale.z);

        while (startTime < animationTime)
        {
            float progress = startTime / animationTime;

            unlockableObject.transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

            startTime += Time.deltaTime;

            yield return null;
        }

        unlockableObject.transform.localScale = targetScale;
        Destroy(unlockableObject);
    }
}
