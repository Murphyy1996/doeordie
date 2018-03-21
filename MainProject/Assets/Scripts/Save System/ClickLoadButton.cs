using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickLoadButton : MonoBehaviour
{
    public void ContinueGameButton()
    {
        SaveSystemManager.inst.LoadGame();
    }
}
