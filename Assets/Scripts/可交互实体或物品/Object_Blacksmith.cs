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
    public void Interact()//F互动方法
    {
        ui.storageUI.SetupStorageUI(storage);//初始化库存界面
        ui.craftUI.SetUpCraftUI(storage);//初始化制作面版
        //第二次互动关闭
        if (ui.storageUI.gameObject.activeSelf || ui.craftUI.gameObject.activeSelf)
        {
            ui.ToggleStorageUI(false);//关闭库存UI和制作UI和提示板
            return;//直接退出
        }
        ui.ToggleStorageUI(true);
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
        try
        {
            ui.ToggleStorageUI(false);//关闭库存UI和制作UI和提示板
        }
        catch
        {
            // 忽略退出运行模式时的 MissingReferenceException
            // 因为对象可能在场景卸载时已被销毁 
        }
    }
}
