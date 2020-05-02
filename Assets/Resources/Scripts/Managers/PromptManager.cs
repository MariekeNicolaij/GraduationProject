using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptManager : MonoBehaviour
{
    public static PromptManager instance;

    string characterName = "";
    
    public List<Prompt> promptList = new List<Prompt>();

    public GameObject promptDeck;
    public Transform promptParent;

    public GameObject promptGameObject;
    public Text promptText, option1Text, option2Text, feedbackText;
    public Image promptImage;
    PromptObject promptObjectScript;

    // Prompt
    Prompt currentPrompt;

    public Vector3 promptScale = new Vector3(3000, 3000, 3000);
    [Range(1, 2f)]
    public float animationTime = 1;


    void Awake()
    {
        instance = this;

        promptObjectScript = promptGameObject.GetComponent<PromptObject>();

        SetRandomCharacterName();
        GeneratePrompts();

        currentPrompt = promptList[Random.Range(0, promptList.Count)];
        SetPromptValues();

        StartCoroutine(AnimatePromptToCenter(animationTime));
    }


    void SetRandomCharacterName()
    {
        TextAsset file = (TextAsset)Resources.Load("Names", typeof(TextAsset));

        string[] row = file.text.Trim().Split('\n');

        characterName = row[UnityEngine.Random.Range(0, row.Length)];
    }

    void GeneratePrompts()
    {
        TextAsset file = (TextAsset)Resources.Load("Prompts", typeof(TextAsset));

        string[] row = file.text.Trim().Split('\n');

        for (int i = 1; i < row.Length; i++)
        {
            var data = row[i].Split(';');

            Prompt prompt = new Prompt();

            // strings
            prompt.promptText = data[0];
            prompt.option1 = data[1];
            prompt.option2 = data[2];
            prompt.feedbackOption1 = data[11];
            prompt.feedbackOption2 = data[12];
            prompt.sprite = (Sprite)Resources.Load<Sprite>("Sprites/Prompts/Prompt"+ int.Parse(data[13])) as Sprite;

            if (prompt.promptText.Contains("Name"))
                prompt.promptText = prompt.promptText.Replace("Name", characterName);
            if (prompt.option1.Contains("Name"))
                prompt.option1 = prompt.option1.Replace("Name", characterName);
            if (prompt.option2.Contains("Name"))
                prompt.option2 = prompt.option2.Replace("Name", characterName);
            if (prompt.feedbackOption1.Contains("Name"))
                prompt.feedbackOption1 = prompt.feedbackOption1.Replace("Name", characterName);
            if (prompt.feedbackOption2.Contains("Name"))
                prompt.feedbackOption2 = prompt.feedbackOption2.Replace("Name", characterName);

            // floats
            prompt.attributesOption1.Clear();
            prompt.attributesOption1.Add(int.Parse(data[3]));
            prompt.attributesOption1.Add(int.Parse(data[5]));
            prompt.attributesOption1.Add(int.Parse(data[7]));
            prompt.attributesOption1.Add(int.Parse(data[9]));

            prompt.attributesOption2.Clear();
            prompt.attributesOption2.Add(int.Parse(data[4]));
            prompt.attributesOption2.Add(int.Parse(data[6]));
            prompt.attributesOption2.Add(int.Parse(data[8]));
            prompt.attributesOption2.Add(int.Parse(data[10]));

            promptList.Add(prompt);
        }
    }

    void SetPromptValues()
    {
        promptText.text = currentPrompt.promptText;
        option1Text.text = currentPrompt.option1;
        option2Text.text = currentPrompt.option2;
        promptImage.sprite = currentPrompt.sprite;

        option1Text.enabled = false;
        option2Text.enabled = false;
        feedbackText.enabled = false;
        promptImage.enabled = false;
    }

    public void TurnPrompt(bool swipedLeft)
    {
        feedbackText.text = (swipedLeft) ? currentPrompt.feedbackOption1 : currentPrompt.feedbackOption2;
        feedbackText.enabled = true;
        promptImage.enabled = true;
        StartCoroutine(AnimatePromptTurn(animationTime));
        GameManager.instance.ApplyChanges(currentPrompt, swipedLeft);

        UnlockableManager.instance.RandomUnlock();
    }

    public void NextPrompt(bool methodCalledFromIEnumerator = false)
    {
        promptObjectScript.CustomEnd();
        currentPrompt = promptList[Random.Range(0, promptList.Count)];

        SetPromptValues();

        if (!methodCalledFromIEnumerator)
            StartCoroutine(AnimatePromptBackToDeck(animationTime, true));
    }

    IEnumerator AnimatePromptToCenter(float animationTime)
    {
        float startTime = 0;
        Vector3 startPos = promptDeck.transform.localPosition;
        startPos.z = 100;

        while (startTime < animationTime)
        {
            float progress = startTime / animationTime;

            promptGameObject.transform.localPosition = Vector3.Lerp(startPos, Vector3.zero, progress);
            promptGameObject.transform.localScale = Vector3.Lerp(Vector3.zero, promptScale, progress);

            // Animate rotation
            float angle = Mathf.LerpAngle(90, -90, progress);
            promptGameObject.transform.localEulerAngles = new Vector3(angle, transform.localEulerAngles.y, transform.localEulerAngles.z);

            startTime += Time.deltaTime;

            yield return null;
        }

        promptGameObject.transform.localPosition = Vector3.zero;
        promptGameObject.transform.localScale = promptScale;
        promptGameObject.transform.localEulerAngles = new Vector3(-90, 0, 0);

        promptObjectScript.CustomStart();
    }

    IEnumerator AnimatePromptTurn(float animationTime)
    {
        float startTime = 0;
        float startAngle = promptGameObject.transform.localEulerAngles.y;
        float angle = 0;

        while (startTime < animationTime)
        {
            float progress = startTime / animationTime;

            // Animate rotation
            angle = Mathf.LerpAngle(startAngle, 180, progress);
            promptGameObject.transform.localEulerAngles = new Vector3(promptGameObject.transform.localEulerAngles.x, angle, promptGameObject.transform.localEulerAngles.z);

            startTime += Time.deltaTime;

            yield return null;
        }

        promptGameObject.transform.localPosition = new Vector3(0, 0, -1);
        promptGameObject.transform.localEulerAngles = new Vector3(promptGameObject.transform.localEulerAngles.x, 180, promptGameObject.transform.localEulerAngles.z);
    }

    IEnumerator AnimatePromptBackToDeck(float animationTime, bool nextPrompt)
    {
        float startTime = 0;
        Vector3 startPos = promptGameObject.transform.localPosition;

        while (startTime < animationTime)
        {
            float progress = startTime / animationTime;

            promptGameObject.transform.localPosition = Vector3.Lerp(startPos, promptDeck.transform.localPosition, progress);
            promptGameObject.transform.localScale = Vector3.Lerp(promptScale, Vector3.zero, progress);

            // Animate rotation
            float angle = Mathf.LerpAngle(-90, 0, progress);
            promptGameObject.transform.localEulerAngles = new Vector3(angle, promptGameObject.transform.localEulerAngles.y, promptGameObject.transform.localEulerAngles.z);

            startTime += Time.deltaTime;

            yield return null;
        }

        promptGameObject.transform.localPosition = promptDeck.transform.localPosition;
        promptGameObject.transform.localScale = Vector3.zero;
        promptGameObject.transform.localEulerAngles = new Vector3(-90, 0, 0);

        if (nextPrompt)
            yield return StartCoroutine(AnimatePromptToCenter(animationTime));
    }
}

[System.Serializable]
public class Prompt
{
    public Sprite sprite;
    public string promptText, option1, option2, feedbackOption1, feedbackOption2, imagePath;
    public List<int> attributesOption1 = new List<int>(), attributesOption2 = new List<int>();
}
