using UnityEngine;

public class Object_Shop : Object_NPC, IInteractable
{
    public void Interact()
    {
        Debug.Log("打开商店");
    }
}
