using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPlayer : MonoBehaviour 
{

    private GameObject player;
    private float pushDistance = 2f;
    private Vector3 offset = new Vector3(1f, 0f, 0f);

	void Start () 
    {
        player = GameObject.Find("Player");
	}

    //Check collision with player
    void OnTriggerEnter(Collider other)
    {
        
        if (other.GetComponent<Collider>().tag == "Player")
        {
            Vector3 enemyPos = this.gameObject.transform.position;
            Vector3 directionToPush = enemyPos - player.transform.position;
   
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Collider>().tag == "Player")
        {
            Debug.Log("stop jumping on head");
            Vector3 enemyPos = this.gameObject.transform.position;
            Vector3 directionToPush = enemyPos - player.transform.position;
            directionToPush.y = 0f;
            player.transform.position += offset * pushDistance * Time.deltaTime;
            //MovePlayer();
        }

    }

    //Prevent movement while not in the trigger
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Collider>().tag == "Player")
        {
            return;
        }
    }

    //Push player off AI's head
//    void MovePlayer()
//    {
//        player.GetComponent<CharacterController>().Move(transform.forward + offset * pushDistance);
//    }

}
