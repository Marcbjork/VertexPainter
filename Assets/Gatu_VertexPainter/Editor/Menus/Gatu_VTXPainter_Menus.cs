using UnityEngine;
using UnityEditor;
using System.Collections;

public class Gatu_VTX_Menus : MonoBehaviour 
{
	[MenuItem("Gatu/Tools/Vertex Painter", false, 10)]
	static void LaunchVertexPainter()
	{
		Gatu_VTXPainter_Window.LaunchVertexPainter ();
	}
}
