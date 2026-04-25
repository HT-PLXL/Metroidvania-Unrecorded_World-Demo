using UnityEngine;

public class DoorLever : MonoBehaviour
{
    public Door targetDoor;                 // 要控制的门
    private bool playerInRange = false;
    private Animator anim;
    private bool Leverdown;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (targetDoor != null)
                targetDoor.Toggle();

            anim.SetBool("down", Leverdown);
            Leverdown = !Leverdown;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}