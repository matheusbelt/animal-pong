using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpitter : MonoBehaviour
{
    [SerializeField] private obstacleManager _oM;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    // Update is called once per frame
    void Update()
    {
        UpdateSpriteColor();
    }
    private void UpdateSpriteColor()
    {
        // Calculate the percentage of time that has elapsed
        float elapsedPercentage = Mathf.Clamp01(_oM._timer / _oM._moveInterval);

        // Calculate the red percentage based on the elapsed time
        float redPercentage = 1f - elapsedPercentage;

        // Create a new Color object with the calculated red percentage
        Color spriteColor = new Color(redPercentage, 1f - redPercentage, 1f - redPercentage, 1f);

        // Set the color of the sprite to the new color
        _spriteRenderer.color = spriteColor;
    }
}
