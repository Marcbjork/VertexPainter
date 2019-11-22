using UnityEngine;
using UnityEditor;
using System.Collections;

public class Gatu_VTXPainter_Window :  EditorWindow
{
	#region Variables
	GUIStyle boxStyle;

	public bool allowPainting = false;
	public bool changingBrushValue = false;
	public bool isPainting = false;

	public Vector2 mousePos;
	public RaycastHit curHit;

	public float brushSize = 1.0f;
	public float brushOpacity = 1.0f;
	public float brushFalloff = 1.0f;

	public GameObject curGO;
	public Mesh curMesh;
	public GameObject lastGO;

	public Color foregroundColor;
	#endregion

	#region Main Method
	public static void LaunchVertexPainter()
	{
		var win = EditorWindow.GetWindow<Gatu_VTXPainter_Window> (false, "VTX Painter", true);
		win.GenerateStyles ();
	}

	void OnEnable()
	{
		SceneView.duringSceneGui -= this.OnSceneGUI;
		SceneView.duringSceneGui += this.OnSceneGUI;
	}

	void OnDestroy()
	{
		SceneView.duringSceneGui -= this.OnSceneGUI;
	}

	void Update()
	{
		//Debug.Log ("We are updating in the Update Method!");
		if(allowPainting)
		{
			Selection.activeGameObject = null;

			if(curHit.transform != null)
			{
				if(curHit.transform.gameObject != lastGO)
				{
					curGO = curHit.transform.gameObject;
					curMesh = Gatu_VTXPainter_Utils.GetMesh(curGO);
					lastGO = curGO;
					
					if(curGO != null && curMesh != null)
					{
						Debug.Log(curGO.name + " : " + curMesh.name);
					}
				}
			}
		}
		else
		{
			curGO = null;
			curMesh = null;
			lastGO = null; 
		}
	}
	#endregion

	#region GUI Methods
	void OnGUI()
	{
		//Header
		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Vertex Painter", boxStyle, GUILayout.Height(60), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		//Body
		GUILayout.BeginVertical (boxStyle);
		GUILayout.Space (10);

		allowPainting = GUILayout.Toggle (allowPainting, "Allow Painting", GUI.skin.button, GUILayout.Height (60));

		if(GUILayout.Button("Update Styles", GUILayout.Height(30)))
		{
			GenerateStyles();
		}

		GUILayout.Space (10f);

		foregroundColor = EditorGUILayout.ColorField ("Foreground Color: ", foregroundColor);

		GUILayout.FlexibleSpace ();
		GUILayout.EndVertical ();


		//Footer
		GUILayout.Box ("", boxStyle, GUILayout.Height(60), GUILayout.ExpandWidth(true));

		//Updates and Repaints the UI in real time.
		Repaint ();
	}

	void OnSceneGUI(SceneView sceneView)
	{
		/*
		Handles.BeginGUI ();
		GUILayout.BeginArea (new Rect (10, 10, 200, 150), boxStyle);
		GUILayout.Button ("Button", GUILayout.Height (25));
		GUILayout.Button ("Button", GUILayout.Height (25));
		GUILayout.EndArea ();
		Handles.EndGUI ();
		*/

		if(allowPainting)
		{
			//Debug.Log ("We have access to the scene view!");
			if (curHit.transform != null) 
			{
				Handles.color = new Color (foregroundColor.r, foregroundColor.g, foregroundColor.b, brushOpacity);
				Handles.DrawSolidDisc (curHit.point, curHit.normal, brushSize);

				Handles.color = Color.red;
				Handles.DrawWireDisc (curHit.point, curHit.normal, brushSize);
				Handles.DrawWireDisc (curHit.point, curHit.normal, brushFalloff);
			}

			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			Ray worldRay = HandleUtility.GUIPointToWorldRay (mousePos);
			if(!changingBrushValue)
			{
				if(Physics.Raycast(worldRay, out curHit, 500f))
				{
					//Begin Vertex Painting here
					if(isPainting)
					{
						PaintVertexColor();
					}
				}
			}
		}


		//Get user inputs
		ProcessInputs ();

		//Update ad repaint our Scene View GUI
		sceneView.Repaint ();
	}
	#endregion



	#region TempPainter Method
	void PaintVertexColor()
	{
		if(curMesh)
		{
			Vector3[] verts = curMesh.vertices;
			Color[] colors = new Color[0];

			if(curMesh.colors.Length > 0)
			{
				colors = curMesh.colors;
			}
			else
			{
				colors = new Color[verts.Length];
			}

			for(int i = 0; i < verts.Length; i++)
			{
				Vector3 vertPos = curGO.transform.TransformPoint(verts[i]);
				float sqrMag = (vertPos - curHit.point).sqrMagnitude;
				if(sqrMag > brushSize)
				{
					continue;
				}

				float falloff = Gatu_VTXPainter_Utils.LinearFalloff(sqrMag, brushSize);
				falloff = Mathf.Pow(falloff, brushFalloff * 3f) * brushOpacity;
				colors[i] = Gatu_VTXPainter_Utils.VtxColorLerp(colors[i], foregroundColor, falloff);
			}

			curMesh.colors = colors;
		}
		else
		{
			Debug.LogWarning("Cant paint vertex color because there is now mesh available...");
		}
	}
	#endregion



	#region Utility Methods
	void ProcessInputs()
	{
		Event e = Event.current;

		mousePos = e.mousePosition;

		//Key Pressed down
		if(e.type == EventType.KeyDown)
		{
			if(e.isKey)
			{
				if(e.keyCode == KeyCode.Y)
				{
					//Debug.Log("Pressed the T Key!!!");
					allowPainting = !allowPainting;
					if(!allowPainting)
					{
						Tools.current = Tool.View;
					}
					else
					{
						Tools.current = Tool.None;
					}
				}
			}
		}

		//Mouse button
		if(e.type == EventType.MouseDown)
		{
			if(e.button == 0)
			{
				//Debug.Log("Pressed the Left Mouse Button!");
			}

			if(e.button == 1)
			{
				//Debug.Log("Pressed the Right Mouse Button!");
			}

			if(e.button == 2)
			{
				//Debug.Log("Pressed the Middle Mouse Button!");
			}
		}

		if(e.type == EventType.MouseUp)
		{
			changingBrushValue = false;
			isPainting = false;
		}

		//brush key combinaations
		if(allowPainting)
		{
			if(e.type == EventType.MouseDrag && e.control && e.button == 0 && !e.shift)
			{
				brushSize -= e.delta.x * 0.005f;
				brushSize = Mathf.Clamp(brushSize, 0.1f, 10f);
				if(brushFalloff > brushSize)
				{
					brushFalloff = brushSize;
				}
				changingBrushValue = true;
			}

			if(e.type == EventType.MouseDrag && !e.control && e.button == 0 && e.shift)
			{
				brushOpacity += e.delta.x * 0.005f;
				brushOpacity = Mathf.Clamp01(brushOpacity);
				changingBrushValue = true;
			}

			if(e.type == EventType.MouseDrag && e.control && e.button == 0 && e.shift)
			{
				brushFalloff += e.delta.x * 0.005f;
				brushFalloff = Mathf.Clamp(brushFalloff, 0.1f, brushSize);
				changingBrushValue = true;
			}

			if(e.type == EventType.MouseDrag && e.button == 0 && !e.control && !e.shift && !e.alt)
			{
				isPainting = true;
			}
		}
	}

	void GenerateStyles()
	{
		boxStyle = new GUIStyle ();
		boxStyle.normal.background = (Texture2D)Resources.Load ("Textures/default_box_bg");
		boxStyle.normal.textColor = Color.white;
		boxStyle.border = new RectOffset (3, 3, 3, 3);
		boxStyle.margin = new RectOffset (2, 2, 2, 2);
		boxStyle.fontSize = 30;
		boxStyle.fontStyle = FontStyle.Bold;
		boxStyle.font = (Font)Resources.Load ("Fonts/OPENSANS-SEMIBOLD");
		boxStyle.alignment = TextAnchor.MiddleCenter;


	}
	#endregion
}
