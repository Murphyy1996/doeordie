//Author: James Murphy
//Purpose: To move the player in the selected local direction at the selected speed
//Requirements: A trigger zone

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerInLocalDirection : MonoBehaviour
{
	[SerializeField]
	[Range(0, 15f)]
	private float speedToMoveObjects = 2f;

	private enum localDirection
	{
		forward,
		backwards,
		left,
		right}

	;

	[SerializeField]
	private localDirection localDirectionToMovePlayer;
	private GameObject playerMoveZone, childObj;
	private CharacterControllerMovement movementScript;
	private Grapple grappleScript;
	private List<GameObject> objectsOnThis = new List<GameObject>();

	private void Start()
	{
		movementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterControllerMovement>();
		grappleScript = movementScript.gameObject.GetComponent<Grapple>();
		childObj = this.gameObject.transform.GetChild(0).gameObject;
	}

	//Move the player in the fixed update
	private void FixedUpdate()
	{
		//Move the required objects in update
		if (playerMoveZone != null || objectsOnThis.Count > 0)
		{
			//Create the direction variable
			Vector3 direction = transform.forward;

			//Set the direction variable based on used input
			switch (localDirectionToMovePlayer)
			{
				case localDirection.forward:
					direction = transform.forward;
					break;
				case localDirection.backwards:
					direction = -transform.forward;
					break;
				case localDirection.left:
					direction = -transform.right;
					break;
				case localDirection.right:
					direction = transform.right;
					break;
			}

			if (grappleScript.IsCurrentlyGrappling() == false && grappleScript.ReturnIfGrappleSliding() == false)
			{
				if (playerMoveZone != null)
				{
					//Perform the actual code moving the player move zone
					playerMoveZone.transform.position += direction * (Time.fixedDeltaTime * speedToMoveObjects);
				}
			}
			//Move other objects too
			foreach (GameObject obj in objectsOnThis)
			{
				obj.transform.position += direction * (Time.fixedDeltaTime * speedToMoveObjects);
			}
			
		}

		if (grappleScript.IsCurrentlyGrappling() == false && grappleScript.ReturnIfGrappleSliding() == false && movementScript.ReturnObjectPlayerIsStandingOnAccurate() != null)
		{
			//Also only allow this glitch fix to run if an object rotation script isn't present
			if (movementScript.ReturnObjectPlayerIsStandingOnAccurate().GetComponent<ObjectRotation>() == null)
			{
				//This unfortuantly affects all objects and upsets me
				if (movementScript.ReturnObjectPlayerIsStandingOnAccurate() != childObj || objectsOnThis.Contains(movementScript.ReturnObjectPlayerIsStandingOnAccurate()) == false)
				{
					movementScript.transform.SetParent(null);
				}
			}
		}
	}

	private void OnTriggerStay(Collider otherObject)
	{
		if (otherObject.tag == "Player" && grappleScript.IsCurrentlyGrappling() == false && grappleScript.isMomentumSliding() == false)
		{
			//If the player is not on this object any more then stop moving them
			if (objectsOnThis.Contains(movementScript.ReturnObjectPlayerIsStandingOnAccurate()) == false && movementScript.ReturnObjectPlayerIsStandingOnAccurate() != childObj || movementScript.ReturnObjectPlayerIsStandingOnAccurate() != childObj || movementScript.ReturnObjectPlayerIsStandingOnAccurate() == null)
			{
				//this is an alternative if statement for making the player move with objects (It didnt work before but works in here???)
				if (objectsOnThis.Contains(movementScript.ReturnObjectPlayerIsStandingOnAccurate()) == true)
				{
					if (playerMoveZone == null)
					{
						playerMoveZone = new GameObject();
						playerMoveZone.name = "Player move zone for object: " + this.gameObject.name;
						playerMoveZone.transform.SetPositionAndRotation(otherObject.transform.position, transform.rotation);
					}
					otherObject.transform.SetParent(playerMoveZone.transform);
				}
				else
				{
					//Unparent as the player is no longer on the object
					movementScript.transform.SetParent(null);
				}
			}
			else
			{
				if (playerMoveZone == null)
				{
					playerMoveZone = new GameObject();
					playerMoveZone.name = "Player move zone for object: " + this.gameObject.name;
					playerMoveZone.transform.SetPositionAndRotation(otherObject.transform.position, transform.rotation);
				}
				otherObject.transform.SetParent(playerMoveZone.transform);
			}
		}
		else //If there is another object than the player on this then mark it down
		{
			if (otherObject.tag == "MoveableObjects" && otherObject.transform.parent != movementScript.transform)
			{
				if (!objectsOnThis.Contains(otherObject.gameObject))
				{
					objectsOnThis.Add(otherObject.gameObject);
				}
			}
		}
	}

	private void OnTriggerExit(Collider otherObject) //Unmark objects from this trigger zone
	{
		if (objectsOnThis.Contains(otherObject.gameObject))
		{
			objectsOnThis.Remove(otherObject.gameObject);
		}
	}
}
