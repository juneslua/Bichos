
using UnityEngine;

public class Interactable : MonoBehaviour
{

    public Transform player;

    public float radius = 3f;

    public virtual void Interact () {
        Debug.Log("Interacting with" + transform.name);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    void Update() {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= radius){
            Interact();
        }    
    }
}
