using UnityEngine;
using System;

public class EditorObject : MonoBehaviour // inherit from monobehaviour to use as component in Unity.
{
    public enum ObjectType { Cylinder, Cube, Sphere, Player }; // the different objects this could be attached to.

    [Serializable] // serialize the Data struct
    public struct Data
    {
        public Vector3 pos; // the object's position
        public Quaternion rot; // the object's rotation
        public ObjectType objectType; // the type of object.
    }

    public Data data; // public reference to Data
}
