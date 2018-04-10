//Author: James Murphy
//Purpose: Check if the drone boss is dead
//Requirements: A drone to check

using UnityEngine;

public class CheckIfBossDead : MonoBehaviour
{
    [SerializeField]
    private GameObject droneBoss, doorToTurnOn, doorToTurnOff;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (droneBoss == null)
        {
            if (doorToTurnOn != null)
            {
                doorToTurnOn.SetActive(true);
            }
            if (doorToTurnOff != null)
            {
                doorToTurnOff.SetActive(false);
                Destroy(this);
            }
        }
    }
}
