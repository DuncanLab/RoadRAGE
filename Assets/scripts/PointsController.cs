using System;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class PointsController : MonoBehaviour
{
    public ProgressBar hungerBar;
    public ProgressBar thirstBar;
    public TextMeshProUGUI pointsCounter;

    private bool isFoodCollision = false;
    private bool isDrinkCollision = false;

    private int foodPickupOffset = 0;
    private int drinkPickupOffset = 0;

    public GameData data;

    private void Start()
    {
        // Carry over data.
        data = Toolbox.Instance.data;

        if (data.currTrial.TrackResources)
        {
            hungerBar = GameObject.Find("HungerBar").gameObject.GetComponent<ProgressBar>();
            thirstBar = GameObject.Find("ThirstBar").gameObject.GetComponent<ProgressBar>();
        }

        if (data.currTrial.TrackPoints)
        {
            pointsCounter = GameObject.Find("PointsCounter").gameObject.GetComponent<TextMeshProUGUI>();
        }

        // We start with resources
        data.currTrial.resourcesRemain = true;
    }

    private void Update()
    {
        if (data.currTrial.TrackResources) UpdateProgressBars();
        if (data.currTrial.TrackPoints) UpdatePointsCounter();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.ToLower().EndsWith("food"))
        {
            isFoodCollision = true;
        }
        else if (other.name.ToLower().EndsWith("drink"))
        {
            isDrinkCollision = true;
        }
        else if (other.name.ToLower().Contains("dice"))
        {
            foreach (GameData.Pickup pickup in data.currTrial.Pickups)
            {
                if (other.name.ToLower().Equals(pickup.PickupName.ToLower()))
                {
                    // Run the probabilities
                    int newPoints = DeterminePointsUsingProbabilities(pickup.WinChance, pickup.LoseChance, pickup.WinPoints, pickup.LosePoints);
                    data.currTrial.PointsCollected += newPoints;
                    break;
                }

            }
        }
        Destroy(other.gameObject);
    }

    private int DeterminePointsUsingProbabilities(double WinProb, double LoseProb, int WinPoints, int LosePoints)
    {
        int points = 0;
        Random rand = new Random();
        bool isWin = rand.NextDouble() < WinProb;

        if (isWin)
        {
            points = WinPoints;
        }
        else
        {
            points = LosePoints;
        }
        return points;
    }

    private void UpdateProgressBars()
    {
        if (isFoodCollision)
        {
            foodPickupOffset += 25;
            isFoodCollision = false;
        }
        else if (isDrinkCollision)
        {
            drinkPickupOffset += 25;
            isDrinkCollision = false;
        }

        int newBarValue = (int)Math.Round(100f - (data.currTrial.Timer.ElapsedMilliseconds / 1000f));

        hungerBar.BarValue = newBarValue;
        hungerBar.BarValue += foodPickupOffset;

        thirstBar.BarValue = newBarValue;
        thirstBar.BarValue += drinkPickupOffset;

        // When either resource gets to zero, end the trial.
        if (hungerBar.BarValue == 0 || thirstBar.BarValue == 0)
        {
            data.currTrial.resourcesRemain = false;
        }
    }

    private void UpdatePointsCounter()
    {
        pointsCounter.text = "Points: " + data.currTrial.PointsCollected;
    }
}
