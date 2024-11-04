using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Progress: MonoBehaviour
{
    private Slider slider;
    private int currentCorrectAnswers;

    public Progress(Slider slider, int totalQuestions)
    {
        this.slider = slider;
        this.currentCorrectAnswers = 0;

        // Ensure the slider is set up correctly
        this.slider.minValue = 0;
        this.slider.maxValue = 1;
        this.slider.value = 0;
    }

    public void incrementProgress(int totalQuestions)
    {
        currentCorrectAnswers++;
        float newProgress = (float)currentCorrectAnswers / totalQuestions;
        slider.value = newProgress;
        Debug.Log($"Progress incremented. Current: {currentCorrectAnswers}, Total: {totalQuestions}, New Value: {newProgress}");
    }

    public void resetProgressBar()
    {
        currentCorrectAnswers = 0;
        slider.value = 0;
        Debug.Log("Progress bar reset");
    }
}