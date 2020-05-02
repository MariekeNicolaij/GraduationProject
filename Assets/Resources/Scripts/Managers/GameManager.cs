using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject tutorialObject;
    public List<GameObject> environmentPresets;
    public List<GameObject> characterPrefabs;
    public Transform environmentParent;

    Material characterMaterial;
    List<Material> environmentMaterials = new List<Material>();

    public List<Attribute> attributeList;
    public Color badAttributeColor = Color.red, goodAttributeColor = Color.green;
    public Color badEnvironmentHealthColor = Color.red, goodEnvironmentHealthColor = Color.white;

    Gradient environmentGradient = new Gradient(), characterGradient = new Gradient();
    List<GradientColorKey> colorKey = new List<GradientColorKey>();
    List<GradientAlphaKey> alphaKey = new List<GradientAlphaKey>();

    public int maxAttributeValue = 100;

    [Range(1, 2)]
    public float attributeScaleMultiplier = 1.25f;
    [Range(0.1f, 1)]
    public float attributeAnimationTime = 0.5f;

    public Text dateText;
    int years = 1, months = 1;


    void Awake()
    {
        instance = this;

        if (environmentPresets.Count == 0 || characterPrefabs.Count == 0)
        {
            Debug.LogError("Not all prefabs have been set!");
            Application.Quit();
        }

        SetRandomEnvironment();
        SetMaterials();
        SetGradients();

        years = PlayerPrefs.GetInt("Years", 1);
        months = PlayerPrefs.GetInt("Months", 1);
        dateText.text = "Year " + years + " Month " + months;
    }

    void SetMaterials()
    {
        // Character
        characterMaterial = GameObject.FindGameObjectWithTag("Character").GetComponentInChildren<SkinnedMeshRenderer>().material;

        // Environment

        List<Material> materials = new List<Material>();

        // Wil geen duplicates natuurlijk, dat maakt dit stukje code nog meer cringe-ier
        foreach (MeshRenderer mr in environmentParent.GetComponentsInChildren<MeshRenderer>())
        {
            Material em = mr.material;
            bool duplicate = false;

            foreach (Material temp in materials)
                if (em == temp)
                    duplicate = true;

            if (!duplicate)
                materials.Add(em);
        }
        environmentMaterials = materials;
    }

    void SetGradients()
    {
        colorKey.Add(new GradientColorKey(badEnvironmentHealthColor, 0.2f));
        colorKey.Add(new GradientColorKey(goodEnvironmentHealthColor, 0.5f));
        colorKey.Add(new GradientColorKey(badEnvironmentHealthColor, 0.8f));

        alphaKey.Add(new GradientAlphaKey(1f, 0f));
        alphaKey.Add(new GradientAlphaKey(1f, 0f));
        alphaKey.Add(new GradientAlphaKey(1f, 0f));

        environmentGradient.SetKeys(colorKey.ToArray(), alphaKey.ToArray());
        characterGradient.SetKeys(colorKey.ToArray(), alphaKey.ToArray());
    }

    void SetRandomEnvironment()
    {
        GameObject go = environmentPresets[UnityEngine.Random.Range(0, environmentPresets.Count)];
        Instantiate(go, environmentParent);

        Transform characterSlot = GameObject.FindGameObjectWithTag("CharacterSlot").transform;

        SetRandomCharacter(characterSlot);
    }

    public void SetRandomCharacter(Transform characterSlot)
    {
        GameObject go = characterPrefabs[UnityEngine.Random.Range(0, characterPrefabs.Count)];
        Instantiate(go, characterSlot);
    }

    public void ApplyChanges(Prompt currentPrompt, bool swipedLeft)
    {
        for (int i = 0; i < attributeList.Count; i++)
        {
            attributeList[i].attributeValue += (swipedLeft) ? currentPrompt.attributesOption1[i] : currentPrompt.attributesOption2[i];
            attributeList[i].AnimateAttribute(attributeAnimationTime);
        }

        if (IsGameOver())
            GameOver();

        UpdateDate();
        ApplyChangesToCharacter();
        ApplyChangesToEnvironment();
    }

    void ApplyChangesToCharacter()
    {
        float averageValue = 0;

        // First half of attributesList
        for (int i = 0; i < Mathf.FloorToInt(attributeList.Count / 2); i++)
            averageValue += attributeList[i].attributeValue;

        averageValue /= (attributeList.Count / 2);

        characterMaterial.color = (characterGradient.Evaluate(averageValue / (float)maxAttributeValue));
    }

    void ApplyChangesToEnvironment()
    {
        float averageValue = 0;

        // Second half of attributesList
        for (int i = Mathf.FloorToInt(attributeList.Count / 2); i < attributeList.Count; i++)
            averageValue += attributeList[i].attributeValue;

        averageValue /= (attributeList.Count / 2);

        foreach (Material m in environmentMaterials)
            m.color = (environmentGradient.Evaluate(averageValue / (float)maxAttributeValue));
    }

    public void UpdateDate()
    {
        months += 1;

        if (months == 12)
        {
            years += 1;
            months = 1;
        }

        dateText.text = "Year " + years + " Month " + months;
        PlayerPrefs.SetInt("Years", years);
        PlayerPrefs.SetInt("Months", months);
    }

    bool IsGameOver()
    {
        foreach (Attribute attribute in attributeList)
            if (attribute.attributeValue < 0 || attribute.attributeValue > maxAttributeValue)
                return true;
        return false;
    }

    public void GameOver()
    {
        int totalDeaths = PlayerPrefs.GetInt("TotalDeaths", 0);
        totalDeaths += 1;
        Debug.Log(totalDeaths);
        PlayerPrefs.SetInt("DeathInYear" + totalDeaths, years);
        PlayerPrefs.SetInt("TotalDeaths", totalDeaths);
        Debug.Log("Game over");
    }
}
