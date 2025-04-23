using UnityEngine;

public class ColliderCheck : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log(this.transform.parent.name+"is colliding with "+collision.transform.parent.name);
    }
}
