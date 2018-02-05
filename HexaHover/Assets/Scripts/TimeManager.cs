using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    public float ElapsedSeconds = 0;

	// Update is called once per frame
	void Update ()
	{
        ElapsedSeconds += Time.deltaTime;
	}
}
