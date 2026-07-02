using UnityEngine;

public class DoorLever : MonoBehaviour
{
    public Door[] targetDoors;// 要控制的多个门
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
            //遍历所有门使其开关
            if (targetDoors != null)
            {
                foreach (Door door in targetDoors)
                {
                    if (door != null)
                        door.Toggle();
                }
            }
            //切换拉杆动画
            Leverdown = !Leverdown;
            anim.SetBool("use", Leverdown);
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