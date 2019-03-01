using System.Collections;
using TMPro;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    private readonly float stepAmt = 4.0f;
    private readonly float moveSpeed = 5.5f;

    private bool showPopup = false;

    private Vector3 startingPos;

    void Start()
    {
        startingPos = transform.position;
    }

    void Update()
    {
        // Do animation
        if (showPopup) transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.up, stepAmt * (moveSpeed * Time.deltaTime));
    }

    public void SetTextAndShow(string text, Color popupColor)
    {
        IEnumerator coroutine = ShowTextCoroutine(text, 1.5f, popupColor);
        StartCoroutine(coroutine);
    }

    private IEnumerator ShowTextCoroutine(string text, float showTime, Color popupColor)
    {
        TextMeshProUGUI popupText = GameObject.Find("PickupPopup").GetComponent<TextMeshProUGUI>();
        popupText.text = text;
        popupText.color = popupColor;
        showPopup = true;
        yield return new WaitForSeconds(showTime);
        popupText.text = "";
        transform.position = startingPos;
        showPopup = false;
    }

}
