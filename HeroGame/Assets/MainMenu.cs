using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	public Texture2D menu;
	public bool menumode = true;
	
	
	void OnGUI ()
	{
		if (menumode == true)
		{
			if(GUI.Button(new Rect(500,275, 150, 50), ""))
			{
			//start button
				menumode = false;
				
			}
			if(GUI.Button(new Rect(500,390,150,50), ""))
			{
			//exit game
				Application.Quit();
			}
		
		//draw the skin second so its on top
			
			GUI.Box(new Rect(0,0, Screen.width, Screen.height), menu);
		}
		
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
