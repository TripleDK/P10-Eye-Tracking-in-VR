using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LikertManager : MonoBehaviour
{

    [HideInInspector] public int currentCondition = 0;
    [SerializeField] LikertButton[] buttons = new LikertButton[5];
    [SerializeField] LikertButton continueButton;
    [SerializeField] TextMeshPro questionTextField;
    [SerializeField] List<string> questions = new List<string>();

    int questionIndex = 0;
    List<int> questionsUsed = new List<int>();

    void Awake()
    {
        foreach (LikertButton butt in buttons)
        {
            butt.OnSelected.AddListener(Reply);
            butt.gameObject.SetActive(false);
        }
        continueButton.OnSelected.AddListener(Continue);
        questionTextField.text = "";
    }

    public void EnableContinueButton()
    {
        continueButton.gameObject.SetActive(true);
    }


    public void Initialize(int condition)
    {
        foreach (LikertButton butt in buttons)
        {
            butt.gameObject.SetActive(true);
        }
        questionsUsed.Clear();
        TaskContext.singleton.likertAnswers = new string[questions.Count + 1];
        TaskContext.singleton.likertAnswers[0] = "\n";
        currentCondition = condition;
        NextQuestion();
    }

    public void Reply(int value)
    {
        //        Debug.Log("Likert reply with value: " + value);
        if (value == -1)
        {
            Continue(-1);
            return;
        }
        TaskContext.singleton.likertAnswers[questionIndex + 1] = "\nCondition: " + currentCondition + ", question: " + questionIndex + ", answer: " + value;
        NextQuestion();
    }

    void NextQuestion()
    {
        if (questionsUsed.Count >= questions.Count)
        {
            FinishQuestions();
            return;
        }
        questionIndex = Random.Range(0, questions.Count);
        while (questionsUsed.Contains(questionIndex))
        {
            questionIndex++;
            if (questionIndex >= questions.Count)
            {
                questionIndex = 0;
            }
        }
        questionsUsed.Add(questionIndex);
        questionTextField.text = questions[questionIndex];
    }

    void FinishQuestions()
    {
        continueButton.gameObject.SetActive(true);
        questionTextField.text = "";
        foreach (string s in TaskContext.singleton.likertAnswers)
        {
            TaskContext.singleton.allLikertStuff += s;
        }

        foreach (LikertButton butt in buttons)
        {
            butt.gameObject.SetActive(false);
        }
    }

    void Continue(int value)
    {
        //      Debug.Log("Continue with value: " + value);
        continueButton.gameObject.SetActive(false);
        CalibrationContext.singleton.EndMirroring();

        gameObject.SetActive(false);
    }

}
