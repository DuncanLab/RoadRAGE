using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsController : MonoBehaviour
{

    public ProgressBar hungerBar;
    public ProgressBar thirstBar;

    private bool isFoodCollision = false;
    private bool isDrinkCollision = false;
    private int foodPickupOffset = 0;
    private int drinkPickupOffset = 0;

    public GameData data;

    private void Start()
    {
        // Carry over data.
        data = Toolbox.Instance.data;
    }

    private void Update()
    {
        UpdateProgressBars();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.EndsWith("food"))
        {
            isFoodCollision = true;
        }
        else if (other.name.EndsWith("drink"))
        {
            isDrinkCollision = true;
        }

        Destroy(other.gameObject);
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
}
