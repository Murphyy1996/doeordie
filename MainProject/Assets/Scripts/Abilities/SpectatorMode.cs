using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpectatorMode : MonoBehaviour
{

	//Author: Kate Georgiou 16/10/17 Handles the spectator mode stuff such as camera switch etc
	[SerializeField]
	[Header("Prefabs")]
	private GameObject spectatorPrefab;
	[SerializeField]
	private GameObject spectatorCanvasPrefab, eventSystemPrefab;
	private GameObject spawnedSpectator, foundPlayer, spawnedCamera;
	private Canvas preGameCanvas, inSpectatorCanvas;
	private Button exitSpectatorButton;
	[SerializeField]
	[Header("Optional Dialogue Settings")]
	private GameObject fungusDialogueObject;
	private GameObject spawnedEventSystem;
	private bool inSpectatorMode = false;

	private void Awake()
	{
         Time.timeScale = 1;
        //Unparent from the player
        transform.SetParent(null);
		//Turn the player off
		Invoke("TurnPlayerOff", 0.2f);
		//Time freezes
	}

	private void TurnPlayerOff()
	{
		foundPlayer = GameObject.FindGameObjectWithTag("Player");
		foundPlayer.SetActive(false);
		spawnedCamera = new GameObject();
		spawnedCamera.name = "Spawned Camera";
		spawnedCamera.transform.position = foundPlayer.transform.position;
		spawnedCamera.AddComponent<Camera>();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		//spawn an event system
		spawnedEventSystem = Instantiate(eventSystemPrefab);
		spawnedEventSystem.name = "Event System Spectator UI";
		//Get this canvas
		preGameCanvas = GetComponent<Canvas>();
		//Spawn the spectator canvas
		GameObject spawnedSpectatorCanvas = Instantiate(spectatorCanvasPrefab) as GameObject;
		inSpectatorCanvas = spawnedSpectatorCanvas.GetComponent<Canvas>();
		spawnedSpectatorCanvas.name = spectatorCanvasPrefab.name;
		//Get the spectator button
		exitSpectatorButton = spawnedSpectatorCanvas.GetComponentInChildren<Button>();
		//Start code
		preGameCanvas.enabled = true;
		inSpectatorCanvas.enabled = false;
		if (fungusDialogueObject != null)
		{
			fungusDialogueObject.SetActive(false);
		}
		Cursor.visible = true;
	}

	private void Update()
	{
		if (inSpectatorMode == true)
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}

	public void Spectate()
	{
		spawnedCamera.GetComponent<Camera>().enabled = false;
		Cursor.visible = true;
		spawnedSpectator = Instantiate(spectatorPrefab, foundPlayer.transform.position, foundPlayer.transform.rotation) as GameObject;
		spawnedSpectator.name = "Spawned Spectator";
		spawnedSpectator.SetActive(true);
		preGameCanvas.enabled = false;
		inSpectatorCanvas.enabled = true;
		Cursor.visible = true;
		inSpectatorMode = true;
	}

	public void GoToGame()
	{
		inSpectatorMode = false;
		Time.timeScale = 1;
		Destroy(spawnedCamera);
		Destroy(spawnedEventSystem);
		exitSpectatorButton.interactable = false;
		Destroy(spawnedSpectator);
		preGameCanvas.enabled = false;
		inSpectatorCanvas.enabled = false;
		foundPlayer.SetActive(true);
		//lock and hide cursor
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		//Turn Dialogue on if needed
		if (fungusDialogueObject != null)
		{
			fungusDialogueObject.SetActive(true);
			Destroy(inSpectatorCanvas.gameObject);
			Destroy(this.gameObject);
		}
	}

	public void GoBackToChoice()
	{
		GameObject foundPreGameCanvas = GameObject.Find("Pre-Game");
		GameObject foundSpawnedCamera = GameObject.Find("Spawned Camera");
		GameObject foundSpectatorCanvas = GameObject.Find("InSpec");
		GameObject foundSpectatorObj = GameObject.Find("Spawned Spectator");

		foundSpawnedCamera.GetComponent<Camera>().enabled = true;
		Destroy(foundSpectatorObj);
		foundPreGameCanvas.GetComponent<Canvas>().enabled = true;
		foundSpectatorCanvas.GetComponent<Canvas>().enabled = false;
		Cursor.visible = true;
	}

	public void MakeCursorVisible()
	{
		Cursor.visible = true;
	}

	public void CursorInvisble()
	{
		Cursor.visible = false;

	}
}
