using UnityEngine;
using UnityEditor;

public class VertexPaint : MonoBehaviour
{
    public bool allowPainting = true;
    public bool isPainting = true;

    public RaycastHit curHit;
    private Camera cam;

    public float brushSize = 10.0f;
    public float brushOpacity = 1.0f;
    public float brushFalloff = 1.0f;

    public GameObject curGO;
    public Mesh curMesh;
    public GameObject lastGO;

    public Color foregroundColor;
    private Color activeColor;
    MouseIndicator mouseIndicator;

    void Start()
    {
        cam = Camera.main;
        mouseIndicator = GetComponent<MouseIndicator>();
        
        //Vector3 point = new Vector3();
        //Event currentEvent = Event.current;
        //Vector2 mousePos = new Vector2();
        //mousePos.x = currentEvent.mousePosition.x;
        //mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;
        //point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

    }
   
    void Update()
    {
        Ray worldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButton(0) && Physics.Raycast(worldRay, out curHit, 500f))
        {
            //Begin Vertex Painting here
            if (isPainting)
            {
                Debug.Log("Träffar Objects");
                PaintVertexColors();
                
            }
            

            if (allowPainting)
            {
                Debug.Log(curHit);
                if (curHit.transform.gameObject != lastGO)
                {
                    curGO = curHit.transform.gameObject;
                    curMesh = FindMesh.GetMesh(curGO);
                    lastGO = curGO;

                    if (curGO != null && curMesh != null)
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
        // save the mesh
        if (Input.GetKeyDown("s"))
        {
            SavePrefab();
        }

    }

    void PaintVertexColors()
    {
        if (curMesh)
        {
            Vector3[] verts = curMesh.vertices;
            Color[] colors = new Color[0];

            if (curMesh.colors.Length > 0)
            {
                colors = curMesh.colors;
            }
            else
            {
                colors = new Color[verts.Length];
            }

            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 vertPos = curGO.transform.TransformPoint(verts[i]);
                float sqrMag = (vertPos - curHit.point).sqrMagnitude;
                if(sqrMag > brushSize)
                { continue; }
               
                float falloff = FindMesh.LinearFalloff(sqrMag, brushSize);
                falloff = Mathf.Pow(falloff, brushFalloff * 3f) * brushOpacity;
                colors[i] = foregroundColor;
                //Falloff på paintin
                //colors[i] = FindMesh.VtxColorLerp(colors[i], foregroundColor, falloff);
            }

            curMesh.colors = colors;
        }
    }

    void SavePrefab()
    {
        GameObject[] objectarray = GameObject.FindGameObjectsWithTag("VertexCube");
        foreach(GameObject gameObject in objectarray)
        {
            string localPath = "Assets/" + gameObject.name + ".prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction);
        }

    }
    public void RedColor()
    {
        foregroundColor = Color.red;
        
        //foregroundColor = colors[index];
    }
    public void BlueColor()
    {
        foregroundColor = Color.blue;
        mouseIndicator.BlueColor();
        //foregroundColor = colors[index];
    }
    public void GreenColor()
    {
        foregroundColor = Color.green;
        mouseIndicator.GreenColor();
        //foregroundColor = colors[index];
    }
    public void YellowColor()
    {
        foregroundColor = Color.yellow;
        //foregroundColor = colors[index];
    }
    public void EraserColor()
    {
        foregroundColor = Color.white;
        //foregroundColor = colors[index];
    }
}
