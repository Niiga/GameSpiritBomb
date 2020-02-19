using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixRatio : MonoBehaviour {
	protected Camera AspectCamera;
	public Vector2Int ReferenceRatio = new Vector2Int(16,9);
	// Use this for initialization
	void Update () {
		AspectCamera = GetComponent<Camera>();

		float x = 1;
		if(Screen.width/ReferenceRatio.x != Screen.height/ReferenceRatio.y){
			x = ((float)Screen.height/(float)Screen.width)/((float)ReferenceRatio.y/(float)ReferenceRatio.x);
		}

		AspectCamera.rect = new Rect((1-x)/2,0,x,1);
	}
}
