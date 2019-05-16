using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ManagerScript : MonoBehaviour
{
    // Hide these variables from Unity editor.
    [HideInInspector]
    public bool playerPlaced = false;
    [HideInInspector]
    public bool saveLoadMenuOpen = false;

    public Animator itemUIAnimation;
    public Animator optionUIAnimation;
    public Animator saveUIAnimation;
    public Animator loadUIAnimation;
    public MeshFilter mouseObject;
    public MouseScript user;
    public Mesh playerMarker;
    public Slider rotSlider;
    public GameObject rotUI;
    public InputField levelNameSave;
    public InputField levelNameLoad;
    public Text levelMessage;
    public Animator messageAnim;

    private bool itemPositionIn = true;
    private bool optionPositionIn = true;
    private bool saveLoadPositionIn = false;
    private LevelEditor level;


    // Start is called before the first frame update
    void Start()
    {
        rotSlider.onValueChanged.AddListener(delegate { RotationValueChange(); }); // set up listener for rotation slider value change
        CreateEditor(); // create new instance of level.
    }

    LevelEditor CreateEditor()
    {
        level = new LevelEditor();
        level.editorObjects = new List<EditorObject.Data>(); // make new list of editor object data.
        return level;
    }

    //Rotating an object and saving the info
    void RotationValueChange()
    {
        user.rotObject.transform.localEulerAngles = new Vector3(0, rotSlider.value, 0); // rotate the object.
        user.rotObject.GetComponent<EditorObject>().data.rot = user.rotObject.transform.rotation; // save rotation info to object's editor object data.
    }

    /// <summary>
    /// Selecting certain menus
    /// </summary>
    public void SlideItemMenu()
    {
        if (itemPositionIn == false)
        {
            itemUIAnimation.SetTrigger("ItemMenuIn"); // slide menu into screen
            itemPositionIn = true; // indicate menu in screen view.
        }
        else
        {
            itemUIAnimation.SetTrigger("ItemMenuOut"); // slide menu out of screen
            itemPositionIn = false; // indicate menu off screen
        }
    }

    public void SlideOptionMenu()
    {
        if (optionPositionIn == false)
        {
            optionUIAnimation.SetTrigger("OptionMenuIn"); // slide menu into screen
            optionPositionIn = true; // indicate menu in screen view.
        }
        else
        {
            optionUIAnimation.SetTrigger("OptionMenuOut"); // slide menu out of screen
            optionPositionIn = false; // indicate menu off screen
        }
    }

    public void ChooseSave()
    {
        if (saveLoadPositionIn == false)
        {
            saveUIAnimation.SetTrigger("SaveLoadIn"); // slide menu into screen
            saveLoadPositionIn = true; // indicate menu on screen
            saveLoadMenuOpen = true; // indicate save menu open to prevent camera movement
        }
        else
        {
            saveUIAnimation.SetTrigger("SaveLoadOut"); // slide menu off screen
            saveLoadPositionIn = false; // indicate menu off screen
            saveLoadMenuOpen = false; // indicate save menu off screen, allow camera movement
        }
    }

    public void ChooseLoad()
    {
        if (saveLoadPositionIn == false)
        {
            loadUIAnimation.SetTrigger("SaveLoadIn"); // slide menu into screen
            saveLoadPositionIn = true; // indicate menu on screen
            saveLoadMenuOpen = true; // indicate load menu open, prevent camera movement.
        }
        else
        {
            loadUIAnimation.SetTrigger("SaveLoadOut"); // slide menu off screen
            saveLoadPositionIn = false; // indicate menu off screen
            saveLoadMenuOpen = false; // indicate load menu off screen, allow camera movement.
        }
    }


    /// <summary>
    /// Choosing an object
    /// </summary>
    public void ChooseCylinder()
    {
        user.itemOption = MouseScript.ItemList.Cylinder; // set object to place as cylinder
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder); // create object, get new object's mesh and set mouse object's mesh to that, then destroy
        mouseObject.mesh = cylinder.GetComponent<MeshFilter>().mesh;
        Destroy(cylinder);
    }
    
    public void ChooseCube()
    {
        user.itemOption = MouseScript.ItemList.Cube; // set object to place as cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube); // create object, get new object's mesh and set mouse object's mesh to that, then destroy
        mouseObject.mesh = cube.GetComponent<MeshFilter>().mesh;
        Destroy(cube);
    }

    public void ChooseSphere()
    {
        user.itemOption = MouseScript.ItemList.Sphere; // set object to place as sphere
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere); // create object, get new object's mesh and set mouse object's mesh to that, then destroy
        mouseObject.mesh = sphere.GetComponent<MeshFilter>().mesh;
        Destroy(sphere);
    }

    public void ChoosePlayerStart()
    {
        user.itemOption = MouseScript.ItemList.Player; // set object to place as player marker
        mouseObject.mesh = playerMarker; // set mouse object's mesh to playerMarker (player object mesh).
    }


    /// <summary>
    /// Choosing an option for level manipulation
    /// </summary>
    public void ChooseCreate()
    {
        user.manipulateOption = MouseScript.LevelManipulation.Create; // set mode to create
        user.mr.enabled = true; // show mouse object mesh
        rotUI.SetActive(false); // disable rotation ui
    }

    public void ChooseRotate()
    {
        user.manipulateOption = MouseScript.LevelManipulation.Rotate; // set mode to rotate
        user.mr.enabled = false; // hide mouse mesh
        rotUI.SetActive(true); // enable rotation ui
    }

    public void ChooseDestroy()
    {
        user.manipulateOption = MouseScript.LevelManipulation.Destroy; // set mode to destroy
        user.mr.enabled = false; // hide mouse mesh
        rotUI.SetActive(false); // disable rotation ui
    }

    

    // Saving a level
    public void SaveLevel()
    {
        // Gather all objects with EditorObject component
        EditorObject[] foundObjects = FindObjectsOfType<EditorObject>();
        foreach (EditorObject obj in foundObjects)
            level.editorObjects.Add(obj.data); // add these objects to the list of editor objects

        string json = JsonUtility.ToJson(level); // write the level data to json
        string folder = Application.dataPath + "/LevelData/"; // create a folder
        string levelFile = "";

        //set a default file name if no name given
        if (levelNameSave.text == "")
            levelFile = "new_level.json";
        else
            levelFile = levelNameSave.text + ".json";

        //Create new directory if LevelData directory does not yet exist.
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, levelFile); // set filepath

        //Overwrite file with same name, if applicable
        if (File.Exists(path))
            File.Delete(path);

        // create and save file
        File.WriteAllText(path, json); 

        //Remove save menu
        saveUIAnimation.SetTrigger("SaveLoadOut");
        saveLoadPositionIn = false;
        saveLoadMenuOpen = false;
        levelNameSave.text = ""; // clear input field
        levelNameSave.DeactivateInputField(); // remove focus from input field.

        //Display message
        levelMessage.text = levelFile + " saved to LevelData folder.";
        messageAnim.Play("MessageFade", 0, 0);
    }


    // Loading a level
    public void LoadLevel()
    {
        string folder = Application.dataPath + "/LevelData/";
        string levelFile = "";

        //set a default file name if no name given
        if (levelNameLoad.text == "")
            levelFile = "new_level.json";
        else
            levelFile = levelNameLoad.text + ".json";

        string path = Path.Combine(folder, levelFile); // set filepath

        if (File.Exists(path)) // if the file could be found in LevelData
        {
            // The objects currently in the level will be deleted
            EditorObject[] foundObjects = FindObjectsOfType<EditorObject>();
            foreach (EditorObject obj in foundObjects)
                Destroy(obj.gameObject);

            playerPlaced = false; // since objects are being destroyed, go ahead and say player placed is false

            string json = File.ReadAllText(path); // provide text from json file
            level = JsonUtility.FromJson<LevelEditor>(json); // level information filled from json file
            CreateFromFile(); // create objects from level data.
        }
        else // if file could not be found.
        {
            loadUIAnimation.SetTrigger("SaveLoadOut"); // remove menu
            saveLoadPositionIn = false; // indicate menu not on screen
            saveLoadMenuOpen = false; // indicate camera can move.
            levelMessage.text = levelFile + " could not be found!"; // send message
            messageAnim.Play("MessageFade", 0, 0);
            levelNameLoad.DeactivateInputField(); // remove focus from input field
        }
    }

    // create objects based on data within level.
    void CreateFromFile()
    {
        GameObject newObj; // make a new object.

        for (int i = 0; i < level.editorObjects.Count; i++)
        {
            if (level.editorObjects[i].objectType == EditorObject.ObjectType.Cylinder) // if a cylinder object
            {
                // create cylinder
                newObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                newObj.transform.position = level.editorObjects[i].pos; // set position from data in level
                newObj.transform.rotation = level.editorObjects[i].rot; // set rotation from data in level.
                newObj.layer = 9; // assign to SpawnedObjects layer.

                //Add editor object component and feed data.
                EditorObject eo = newObj.AddComponent<EditorObject>();
                eo.data.pos = newObj.transform.position;
                eo.data.rot = newObj.transform.rotation;
                eo.data.objectType = EditorObject.ObjectType.Cylinder;
            }
            else if (level.editorObjects[i].objectType == EditorObject.ObjectType.Cube)
            {
                // create cube
                newObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newObj.transform.position = level.editorObjects[i].pos; // set position from data in level
                newObj.transform.rotation = level.editorObjects[i].rot; // set rotation from data in level.
                newObj.layer = 9; // assign to SpawnedObjects layer.

                //Add editor object component and feed data.
                EditorObject eo = newObj.AddComponent<EditorObject>();
                eo.data.pos = newObj.transform.position;
                eo.data.rot = newObj.transform.rotation;
                eo.data.objectType = EditorObject.ObjectType.Cube;
            }
            else if (level.editorObjects[i].objectType == EditorObject.ObjectType.Sphere)
            {
                // create sphere
                newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newObj.transform.position = level.editorObjects[i].pos; // set position from data in level
                newObj.transform.rotation = level.editorObjects[i].rot; // set rotation from data in level.
                newObj.layer = 9; // assign to SpawnedObjects layer.

                //Add editor object component and feed data.
                EditorObject eo = newObj.AddComponent<EditorObject>();
                eo.data.pos = newObj.transform.position;
                eo.data.rot = newObj.transform.rotation;
                eo.data.objectType = EditorObject.ObjectType.Sphere;
            }
            else if (level.editorObjects[i].objectType == EditorObject.ObjectType.Player)
            {
                // create player marker
                newObj = Instantiate(user.Player, transform.position, Quaternion.identity);
                newObj.layer = 9; // assign to SpawnedObjects layer.
                newObj.AddComponent<CapsuleCollider>(); // make capsule collider component
                newObj.GetComponent<CapsuleCollider>().center = new Vector3(0, 1, 0);
                newObj.GetComponent<CapsuleCollider>().height = 2;
                newObj.transform.position = level.editorObjects[i].pos; // set position from data in level
                newObj.transform.rotation = level.editorObjects[i].rot; // set rotation from data in level.
                playerPlaced = true;

                //Add editor object component and feed data.
                EditorObject eo = newObj.AddComponent<EditorObject>();
                eo.data.pos = newObj.transform.position;
                eo.data.rot = newObj.transform.rotation;
                eo.data.objectType = EditorObject.ObjectType.Player;
            }
        }

        //Clear level box
        levelNameLoad.text = "";
        levelNameLoad.DeactivateInputField(); // remove focus from input field

        loadUIAnimation.SetTrigger("SaveLoadOut"); // slide load menu off screen
        saveLoadPositionIn = false; // indicate load menu off screen
        saveLoadMenuOpen = false; // allow camera movement.

        //Display message
        levelMessage.text = "Level loading...done.";
        messageAnim.Play("MessageFade", 0, 0);
    }
}
