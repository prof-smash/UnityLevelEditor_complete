using UnityEngine;
using UnityEngine.UI;


public class CameraMove : MonoBehaviour
{
    public Slider cameraSpeedSlide;
    public ManagerScript ms;

    private float xAxis;
    private float yAxis;
    private float zoom;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>(); // get the camera component for later use
    }

    // Update is called once per frame
    void Update()
    {
        if (ms.saveLoadMenuOpen == false) // if no save or load menus are open.
        {
            xAxis = Input.GetAxis("Horizontal"); // get user input
            yAxis = Input.GetAxis("Vertical");

            zoom = Input.GetAxis("Mouse ScrollWheel") * 10;

            // move camera based on info from xAxis and yAxis
            transform.Translate(new Vector3(xAxis * -cameraSpeedSlide.value, yAxis * -cameraSpeedSlide.value, 0.0f));
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -20, 20),
                Mathf.Clamp(transform.position.y, 20, 20),
                Mathf.Clamp(transform.position.z, -20, 20)); // limit camera movement to -20 min, 20 max. Y value remains 20.

            //change camera's orthographic size to create zooming in and out. Can only be between -25 and -5.
            if (zoom < 0 && cam.orthographicSize >= -25)
                cam.orthographicSize -= zoom * -cameraSpeedSlide.value;

            if (zoom > 0 && cam.orthographicSize <= -5)
                cam.orthographicSize += zoom * cameraSpeedSlide.value;
        }
    }
}
