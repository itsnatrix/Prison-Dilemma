using UnityEngine;
using TMPro;

[System.Serializable]
public class Question
{
    public string questionText;
    public string answer;
    public string bodyLanguage;
    public bool isLie;
    public string reactionIfCalledLiar;
    public string reactionIfCalledTruth;
}

[System.Serializable]
public class Suspect
{
    public string name;
    public Question[] questions;
}

public class InterrogationManager : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public TMP_Text bodyLanguageText;
    public TMP_Text[] questionButtons;

    public GameObject questionPanel;
    public GameObject judgePanel;
    public GameObject continuePanel;

    public Suspect[] suspects;
    public int questionsPerSuspect = 2;

    int currentSuspectIndex = 0;
    int currentQuestionIndex = 0;
    int totalQuestionsAsked = 0;
    int score = 0;

    void Start()
    {
        ShowQuestionMenu("What is your question?");
    }

    public void ShowQuestionMenu(string question)
    {
        questionPanel.SetActive(true);
        judgePanel.SetActive(false);
        continuePanel.SetActive(false);

        dialogueText.text = question;
        bodyLanguageText.text = "";

        Suspect currentSuspect = suspects[currentSuspectIndex];
        nameText.text = currentSuspect.name;

        for (int i = 0; i < questionButtons.Length; i++)
        {
            if (i < currentSuspect.questions.Length)
            {
                questionButtons[i].text = currentSuspect.questions[i].questionText;
            }
        }
    }

    public void AskQuestion(int questionIndex)
    {
        currentQuestionIndex = questionIndex;
        totalQuestionsAsked = totalQuestionsAsked + 1;

        Question q = suspects[currentSuspectIndex].questions[questionIndex];

        questionPanel.SetActive(false);
        judgePanel.SetActive(true);

        dialogueText.text = q.answer;
        bodyLanguageText.text = q.bodyLanguage;
    }

    public void AccuseLiar()
    {
        Judge(true);
    }

    public void AccuseTruthTeller()
    {
        Judge(false);
    }

    void Judge(bool playerSaysLying)
    {
        Question q = suspects[currentSuspectIndex].questions[currentQuestionIndex];

        // Did the player guess correctly?
        if (playerSaysLying == q.isLie)
        {
            score = score + 1;
        }

        judgePanel.SetActive(false);
        bodyLanguageText.text = "";

        if (playerSaysLying)
        {
            dialogueText.text = q.reactionIfCalledLiar;
        }
        else
        {
            dialogueText.text = q.reactionIfCalledTruth;
        }

        continuePanel.SetActive(true);
    }

    public void Continue()
    {
        continuePanel.SetActive(false);

        if (totalQuestionsAsked >= questionsPerSuspect)
        {
            EndInterrogation();
        }
        else
        {
            ShowQuestionMenu("Anything else?");
        }
    }

    void EndInterrogation()
    {
        questionPanel.SetActive(false);
        judgePanel.SetActive(false);
        continuePanel.SetActive(false);
        bodyLanguageText.text = "";

        dialogueText.text = "The interrogation is done. You made " + score + " correct calls out of " + totalQuestionsAsked;
        Debug.Log("Score: " + score);
    }
}