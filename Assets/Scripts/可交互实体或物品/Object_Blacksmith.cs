using UnityEngine;

public class Object_Blacksmith : Object_NPC, IInteractable
{
    private Animator anim;
    private Bag_Player bag;
    private Bag_Storage storage;
    protected override void Awake()
    {
        base.Awake();
        storage = GetComponent<Bag_Storage>();
        //因为NPC使用的都是同一个控制器，所以需要改下默认的待机动画
        anim = GetComponentInChildren<Animator>();
        anim.SetBool("isBlacksmith", true);
    }
    public void Interact()//互动方法
    {
        ui.storageUI.SetupStorage(bag, storage);//将本类获取的背包和仓库实例传递进去
        ui.storageUI.gameObject.SetActive(true);
    }
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);//父类获取了playerTransform
        bag = playerTransform.GetComponent<Bag_Player>();
        storage.SetBag(bag);
    }
    protected override void OnTriggerExit2D(Collider2D collider)
    {
        base.OnTriggerExit2D(collider);
        ui.SwitchOffAlltooltips();
        ui.storageUI.gameObject.SetActive(false);
    }
}
