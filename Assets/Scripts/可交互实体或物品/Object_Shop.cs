using UnityEngine;

public class Object_Shop : Object_NPC, IInteractable
{
    private Bag_Player bag;
    private Bag_Shop shop;

    protected override void Awake()
    {
        base.Awake();
        shop = GetComponent<Bag_Shop>();
    }
    protected override void Update()
    {
        base.Update();
        if(Input.GetKeyDown(KeyCode.Z))
            shop.FillShopList();
    }
    public void Interact()//互动方法
    {
        ui.shopUI.SetupShopUI(shop, bag);
        ui.shopUI.gameObject.SetActive(true);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        bag = playerTransform.GetComponent<Bag_Player>();
        shop.SetBag(bag);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        ui.SwitchOffAlltooltips();
        ui.shopUI.gameObject.SetActive(false);
    }
}
