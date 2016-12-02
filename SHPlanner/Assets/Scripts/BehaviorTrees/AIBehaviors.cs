using UnityEngine;
using System.Collections;

public class AIBehaviors : MonoBehaviour
{
    [HideInInspector]
    public Vector3 Target = new Vector3();
    [HideInInspector]
    public Vector3 MovementTarget = new Vector3();

    public float Speed = 0.05f;
    
	// Use this for initialization
	void Start () {
	
	}
	
	
}
