using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField]
    private float time = 1f;

    private void OnEnable()
    {
        StartCoroutine(EndMe());
    }

    private IEnumerator EndMe()
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
