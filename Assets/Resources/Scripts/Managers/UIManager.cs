using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startPanel, progressionPanel;

    public LineRenderer graphLineRenderer;
    float lineHorizontalOffset = 100, lineVerticalOffset = 50;

    public Font font;


    void Awake()
    {
        instance = this;

        DrawGraph();
    }

    void DrawGraph()
    {
        int totalDeaths = PlayerPrefs.GetInt("TotalDeaths");
        if (totalDeaths == 0)
            return;

        int months = PlayerPrefs.GetInt("Months");
        int years = PlayerPrefs.GetInt("Years");

        Vector3[] positions = new Vector3[years];
        Dictionary<int, int> yearsAndDeaths = new Dictionary<int, int>();

        for (int i = 1; i <= years; i++)
            yearsAndDeaths.Add(i, 0);

        for (int i = 1; i <= totalDeaths; i++)
        {
            int yearOfDeath = PlayerPrefs.GetInt("DeathInYear" + i);

            yearsAndDeaths[yearOfDeath] += 1;
        }

        for (int i = 1; i <= years; i++)
        {
            positions[i - 1] = new Vector3(i + (lineHorizontalOffset * i) - (i * 1), yearsAndDeaths[i] * lineVerticalOffset, 0);

            // Horizontal text
            GameObject deathYearTextObject = Instantiate(new GameObject("GraphText"));
            deathYearTextObject.transform.parent = graphLineRenderer.transform;
            deathYearTextObject.transform.localScale = Vector3.one;
            deathYearTextObject.transform.localPosition = new Vector3(lineHorizontalOffset * i, 0, 0);

            Text t1 = deathYearTextObject.AddComponent<Text>();
            t1.color = Color.yellow;
            t1.font = font;
            t1.fontSize = 20;
            t1.fontStyle = FontStyle.Bold;
            t1.text = "Year " + i;
            deathYearTextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 100);

            // Vertical text
            GameObject totalDeathTextObject = Instantiate(new GameObject("GraphText"));
            totalDeathTextObject.transform.parent = graphLineRenderer.transform;
            totalDeathTextObject.transform.localScale = Vector3.one;
            totalDeathTextObject.transform.localPosition = new Vector3(0, yearsAndDeaths[i] * lineVerticalOffset, 0);

            Text t2 = totalDeathTextObject.AddComponent<Text>();
            t2.color = Color.yellow;
            t2.font = font;
            t2.fontSize = 20;
            t2.fontStyle = FontStyle.Bold;
            t2.text = yearsAndDeaths[i] + " deaths";
            deathYearTextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 100);
        }

        graphLineRenderer.positionCount = positions.Length;
        graphLineRenderer.SetPositions(positions);
    }

    public void DeleteAllSaveData()
    {
        PlayerPrefs.DeleteAll();
    }

    void DisableAllPanels()
    {
        startPanel.SetActive(false);
        progressionPanel.SetActive(false);
    }

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowStartPanel()
    {
        DisableAllPanels();
        startPanel.SetActive(true);
    }
    public void ShowProgressionPanel()
    {
        DisableAllPanels();
        progressionPanel.SetActive(true);
    }
}
