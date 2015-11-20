using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NutBurn : MonoBehaviour 
{
    public delegate void BurnHandler();
    public static event BurnHandler OnBegin;
    public static event BurnHandler OnEnd;

	void BurnBegin () 
    {
        if (OnBegin != null)
            OnBegin();
	}
	
	void BurnEnd () 
    {
        if (OnEnd != null)
            OnEnd();

        SendMessageUpwards("ToFall");
    }
}
