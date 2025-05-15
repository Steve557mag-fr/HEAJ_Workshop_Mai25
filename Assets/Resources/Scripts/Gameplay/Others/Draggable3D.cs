using UnityEngine;

public class Draggable3D : MonoBehaviour
{

    [SerializeField] float radiusSnap = 1;


    public void OnMouseDrag()
    {

        Vector3 inWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = inWorldPosition;

    }

    public void OnMouseUp()
    {
        Debug.Log("mouse up!");
        Physics.SphereCast(transform.position, radiusSnap, Vector3.up, out RaycastHit hit);
        //do stuff..

    }


}
