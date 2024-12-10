using UnityEngine;
using TMPro;

public class PointsManager : MonoBehaviour
{
    public int points = 0;
    public TMP_Text pointsText;
    public void AddPoints(int value)
    {
        points += value;
        if (pointsText != null)
        {
            pointsText.text = "Points: " + points;
        }
        Debug.Log("Points: " + points);
    }
}