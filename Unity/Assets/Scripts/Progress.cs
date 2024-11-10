using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    private Slider slider; // משתנה מסוג סליידר לשמירת מד ההתקדמות
    private int currentCorrectAnswers; // משתנה לספירת התשובות הנכונות שהשחקן ענה עד כה

    public Progress(Slider slider, int totalQuestions) // קבלת הסליידר ומספר השאלות הכולל
    {
        this.slider = slider;
        this.currentCorrectAnswers = 0; // אתחול ספירת התשובות הנכונות לאפס

        // הגדרה של תכונות הסליידר
        this.slider.minValue = 0; // ערך מינימלי של הסליידר
        this.slider.maxValue = 1; // ערך מקסימלי של הסליידר (באחוזים, הסליידר מוגדר מ-0 עד 1)
        this.slider.value = 0; // אתחול הסליידר לאפס (התקדמות התחלתית)
    }

    public void incrementProgress(int totalQuestions) // פונקציה להעלאת ההתקדמות עם כל תשובה נכונה
    {
        currentCorrectAnswers++; // העלאה באחד של כמות התשובות הנכונות
        float newProgress = (float)currentCorrectAnswers / totalQuestions; // חישוב ההתקדמות באחוזים
        slider.value = newProgress; // עדכון ערך הסליידר בהתאם להתקדמות החדשה
        Debug.Log($"Progress incremented. Current: {currentCorrectAnswers}, Total: {totalQuestions}, New Value: {newProgress}");
    }

    public void resetProgressBar() // פונקציה לאיפוס הסליידר וההתקדמות
    {
        currentCorrectAnswers = 0; // איפוס כמות התשובות הנכונות
        slider.value = 0; // איפוס הסליידר להתחלה (אפס)
        Debug.Log("Progress bar reset");
    }
}