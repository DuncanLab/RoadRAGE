using System;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class PointsController : MonoBehaviour
{
    public GameData data;

    public ProgressBar hungerBar;
    public ProgressBar thirstBar;
    public TextMeshProUGUI pointsCounter;

    private int foodPickupOffset = 0;
    private int drinkPickupOffset = 0;

    private enum CollisionType
    {
        Food, Drink, Dice, None
    }

    private CollisionType colType = CollisionType.None;

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
        int newPoints = 0;

        if (other.name.ToLower().Contains("food"))
        {
            colType = CollisionType.Food;
            GameData.Pickup p = GetMatchingPickup(other.name);
            newPoints = p.WinPoints;
        }
        else if (other.name.ToLower().Contains("drink"))
        {
            colType = CollisionType.Drink;
            GameData.Pickup p = GetMatchingPickup(other.name);
            newPoints = p.WinPoints;
        }
        else if (other.name.ToLower().Contains("dice"))
        {
            colType = CollisionType.Dice;
            GameData.Pickup p = GetMatchingPickup(other.name);
            newPoints = DeterminePointsUsingProbabilities(p);
            data.currTrial.PointsCollected += newPoints;
        }
        else
        {
            colType = CollisionType.None;
        }

        if (data.currTrial.TrackPickups)
        {
            ShowPickupPopup(newPoints);
        }

        Destroy(other.gameObject);
    }

    private int DeterminePointsUsingProbabilities(GameData.Pickup pickup)
    {
        int points = 0;
        Random rand = new Random();
        bool isWin = rand.NextDouble() < pickup.WinChance;

        if (isWin)
        {
            points = pickup.WinPoints;
        }
        else
        {
            points = pickup.LosePoints;
        }
        return points;
    }

    private void UpdateProgressBars()
    {
        switch (colType)
        {
            case CollisionType.Food:
                foodPickupOffset += 25;
                break;
            case CollisionType.Drink:
                drinkPickupOffset += 25;
                break;
            case CollisionType.Dice:
                break;
            default:
                break;
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
            if (!data.currTrial.KeepPointsAfterResourceDepletion) data.currTrial.PointsCollected = 0;
        }

        // Reset Collision
        colType = CollisionType.None;
    }

    private void UpdatePointsCounter()
    {
        pointsCounter.text = "Points: " + data.currTrial.PointsCollected;
    }

    private void ShowPickupPopup(int points)
    {
        string popupText = "";
        string pointsStr = points.ToString();

        // Display '+' if points are positive
        if (points >= 0)
        {
            pointsStr = "+" + pointsStr;
        }

        Color textColor = new Color(0f, 0f, 0f);

        switch (colType)
        {
            case CollisionType.Food:
                popupText = pointsStr + " food";
                textColor = Color.green;
                break;
            case CollisionType.Drink:
                popupText = pointsStr + " drink";
                textColor = new Color(0, 140, 255);
                break;
            case CollisionType.Dice:
                popupText = pointsStr + " points";
                textColor = Color.yellow;
                break;
            default:
                // No proper collision
                return;
        }

        GameObject.Find("PickupPopup").GetComponent<PopupController>().SetTextAndShow(popupText, textColor);
    }

    private GameData.Pickup GetMatchingPickup(string pickupName)
    {
        GameData.Pickup p = null;
        foreach (GameData.Pickup pickup in data.currTrial.Pickups)
        {
            if (pickupName.ToLower().Equals(pickup.PickupName.ToLower()))
            {
                p = pickup;
                break;
            }
        }

        return p;
    }
}
