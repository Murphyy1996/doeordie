using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LerpingObjective : MonoBehaviour
{

    [SerializeField]
    private Text objective;
    [SerializeField]
    private Transform lerp1, lerp2, lerp3, target;
    QuestManager QM;
    private bool canrun = false;

    // Use this for initialization
    void Start()
    {
        //find the components
        objective = GameObject.Find("QuestText").GetComponent<Text>();
        objective.text = "Do it in style";
        lerp1 = GameObject.Find("LerpPoint1").transform;
        lerp2 = GameObject.Find("LerpPoint2").transform;
        lerp3 = GameObject.Find("LerpPoint3").transform;
        QM = GameObject.Find("Player").GetComponent<QuestManager>();
        target = lerp1;
        Invoke("Lerp", 3f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //if the objective text is activated = bounce down from point 1 to 2, then after some time boucne from 2 to 3

        if (canrun == true)
        {
            if (Vector3.Distance(transform.localPosition, target.localPosition) <= 1f)
            {
                if (target == lerp1)
                {
                    target = lerp2;
                }
                else if (target == lerp2)
                {
                    target = lerp3;
                }
            }
            float speed = 5 * Time.fixedDeltaTime;
            objective.transform.localPosition = Vector3.Lerp(objective.transform.localPosition, target.localPosition, speed);
            if (target == lerp3)
            {
                transform.rotation = Quaternion.Euler(0, 0, 7);

            }
        }

    }

    private void Lerp()
    {
        //leave some tome between going to each poitn and make it so that it stop s when it reaches point 3
        canrun = true;
    }
}
