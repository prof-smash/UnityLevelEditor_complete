using UnityEngine;
using UnityEngine.EventSystems;

public class MouseScript : MonoBehaviour
{
    public enum LevelManipulation { Create, Rotate, Destroy }; // the possible level manipulation types
    public enum ItemList { Cylinder, Cube, Sphere, Player }; // the list of items

    [HideInInspector] // we hide these to make them known to the rest of the project without them appearing in the Unity editor.
    public ItemList itemOption = ItemList.Cylinder; // setting the cylinder object as the default object
    [HideInInspector]
    public LevelManipulation manipulateOption = LevelManipulation.Create; // create is the default manipulation type.
    [HideInInspector]
    public MeshRenderer mr;
    [HideInInspector]
    public GameObject rotObject;

    public Material goodPlace;
    public Material badPlace;
    public GameObject Player;
    public ManagerScript ms;

    private Vector3 mousePos;
    private bool colliding;
    private Ray ray;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>(); // get the mesh renderer component and store it in mr.
    }

    // Update is called once per frame
    void Update()
    {
        // Have the object follow the mouse cursor by getting mouse coordinates and converting them to world point.
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = new Vector3(
            Mathf.Clamp(mousePos.x, -20, 20),
            0.75f,
            Mathf.Clamp(mousePos.z, -20, 20)); // limit object movement to minimum -20 and maximum 20 for both x and z coordinates. Y alwasy remains 0.75.

        ray = Camera.main.ScreenPointToRay(Input.mousePosition); // send out raycast to detect objects
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.layer == 9) // check if raycast hitting user created object.
            {
                colliding = true; // Unity now knows it cannot create any new object until collision is false.
                mr.material = badPlace; // change the material to red, indicating that the user cannot place the object there.
            }
            else
            {
                colliding = false;
                mr.material = goodPlace;
            }
        }

        // after pressing the left mouse button...
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject()) // check if mouse over UI object.
            {
                if (colliding == false && manipulateOption == LevelManipulation.Create) // create an object if not colliding with anything.
                    CreateObject();
                else if (colliding == true && manipulateOption == LevelManipulation.Rotate) // Select object under mouse to be rotated.
                    SetRotateObject();
                else if (colliding == true && manipulateOption == LevelManipulation.Destroy) // select object under mouse to be destroyed.
                {
                    if (hit.collider.gameObject.name.Contains("PlayerModel")) // if player object, set ms.playerPlaced to false indicating no player object in level.
                        ms.playerPlaced = false;

                    Destroy(hit.collider.gameObject); // remove from game.
                }

            }
        }
    }


    /// <summary>
    /// Object creation
    /// </summary>
    void CreateObject()
    {
        GameObject newObj;

        if (itemOption == ItemList.Cylinder) // cylinder
        {
            //Create object
            newObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            newObj.transform.position = transform.position;
            newObj.layer = 9; // set to Spawned Objects layer

            //Add editor object component and feed it data.
            EditorObject eo = newObj.AddComponent<EditorObject>();
            eo.data.pos = newObj.transform.position;
            eo.data.rot = newObj.transform.rotation;
            eo.data.objectType = EditorObject.ObjectType.Cylinder;
        }
        else if (itemOption == ItemList.Cube) // cube
        {
            //Create object
            newObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newObj.transform.position = transform.position;
            newObj.layer = 9; // set to Spawned Objects layer

            //Add editor object component and feed it data.
            EditorObject eo = newObj.AddComponent<EditorObject>();
            eo.data.pos = newObj.transform.position;
            eo.data.rot = newObj.transform.rotation;
            eo.data.objectType = EditorObject.ObjectType.Cube;
        }
        else if (itemOption == ItemList.Sphere) // sphere
        {
            //Create object
            newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newObj.transform.position = transform.position;
            newObj.layer = 9; // set to Spawned Objects layer

            //Add editor object component and feed it data.
            EditorObject eo = newObj.AddComponent<EditorObject>();
            eo.data.pos = newObj.transform.position;
            eo.data.rot = newObj.transform.rotation;
            eo.data.objectType = EditorObject.ObjectType.Sphere;
        }
        else if (itemOption == ItemList.Player) // player start
        {
            if (ms.playerPlaced == false) // only perform next actions if player not yet placed.
            {
                //Create object and give it capsule collider component.
                newObj = Instantiate(Player, transform.position, Quaternion.identity);
                newObj.layer = 9; // set to Spawned Objects layer
                newObj.AddComponent<CapsuleCollider>();
                newObj.GetComponent<CapsuleCollider>().center = new Vector3(0, 1, 0);
                newObj.GetComponent<CapsuleCollider>().height = 2;
                ms.playerPlaced = true;

                //Add editor object component and feed it data.
                EditorObject eo = newObj.AddComponent<EditorObject>();
                eo.data.pos = newObj.transform.position;
                eo.data.rot = newObj.transform.rotation;
                eo.data.objectType = EditorObject.ObjectType.Player;
            }
        }
    }

    /// <summary>
    /// Object rotation
    /// </summary>
    void SetRotateObject()
    {
        rotObject = hit.collider.gameObject; // object to be rotated
        ms.rotSlider.value = rotObject.transform.rotation.y; // set slider to current object's rotation.
    }
}
