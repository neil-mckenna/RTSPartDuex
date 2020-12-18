using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject healthBarParent = null;
    [SerializeField] private Image healthBarImage = null;

    private void Awake()
    {
        // subscribe to the event on start of game
        health.ClientOnHealthUpdated += HandleHealthUpdated;

    }

    private void OnDestroy()
    {
        // on application destruction unsubscribe to the event
        health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    private void OnMouseEnter()
    {
        // show the healthbars when moved over
        healthBarParent.SetActive(true);
    }

    private void OnMouseExit()
    {
        // hide the healthbars when moved over
        healthBarParent.SetActive(false);
    }

    private void HandleHealthUpdated(int currentHealth, int maxhealth)
    {
        // the float cast change from decimal to int
        healthBarImage.fillAmount = (float)currentHealth / maxhealth;
    }


}
