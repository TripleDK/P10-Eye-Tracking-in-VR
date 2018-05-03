﻿using System.Collections;
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

    void Awake()
    {
        foreach (LikertButton butt in buttons)
        {
            butt.OnSelected.AddListener(Reply);
        }
        continueButton.OnSelected.AddListener(Continue);
        questionTextField.text = "";
    }

    public void Initialize(int condition)
    {
        foreach (LikertButton butt in buttons)
        {
            butt.gameObject.SetActive(true);
        }
        questionIndex = -1;
        TaskContext.singleton.likertAnswers += "\n";

        NextQuestion();
    }

    public void Reply(int value)
    {
        TaskContext.singleton.likertAnswers += "\nCondition: " + currentCondition + ", question: " + questionIndex + ", answer: " + value;
        NextQuestion();
    }

    void NextQuestion()
    {
        questionIndex++;
        if (questionIndex >= questions.Count)
        {
            FinishQuestions();
            return;
        }
        questionTextField.text = questions[questionIndex];
    }

    void FinishQuestions()
    {
        continueButton.gameObject.SetActive(true);
        questionTextField.text = "";
        foreach (LikertButton butt in buttons)
        {
            butt.gameObject.SetActive(false);
        }
    }

    void Continue(int value)
    {
        continueButton.gameObject.SetActive(false);
        CalibrationContext.singleton.EndMirroring();

        gameObject.SetActive(false);
    }

}
