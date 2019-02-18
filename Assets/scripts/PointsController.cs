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
    private bool isOnGoingCollision = false;

    private int foodPickupOffset = 0;
    private int drinkPickupOffset = 0;

    public GameData data;

    private void Start()
    {
        // Carry over data.
        data = Toolbox.Instance.data;

        hungerBar = GameObject.Find("HungerBar").gameObject.GetComponent<ProgressBar>();
        thirstBar = GameObject.Find("ThirstBar").gameObject.GetComponent<ProgressBar>();
        pointsCounter = GameObject.Find("PointsCounter").gameObject.GetComponent<TextMeshProUGUI>();

        // We start with resources
        data.currTrial.resourcesRemain = true;
    }

    private void Update()
    {
        UpdateProgressBars();
        UpdatePointsCounter();

        // When either resource gets to zero, end the trial.
        if (hungerBar.BarValue == 0 || thirstBar.BarValue == 0)
        {
            data.currTrial.resourcesRemain = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If we don't do this, points could be counted more than once.
        if (isOnGoingCollision)
        {
            return;
        }
        else
        {
            isOnGoingCollision = true;
        }

        if (other.name.ToLower().EndsWith("food"))
        {
            isFoodCollision = true;
        }
        else if (other.name.ToLower().EndsWith("drink"))
        {
            isDrinkCollision = true;
        }
        else if (other.name.ToLower().Contains("money"))
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

    private void OnTriggerExit(Collider other)
    {
        isOnGoingCollision = false;
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
    }

    private void UpdatePointsCounter()
    {
        pointsCounter.text = "Points: " + data.currTrial.PointsCollected;
    }
}
