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
        if (Input.GetKeyDown(KeyCode.Z))
            shop.FillShopList();
    }
    public void Interact()//F互动方法
    {
        if (shop == null || bag == null)
            return;
        ui.shopUI.SetupShopUI(shop, bag);
        if (ui.shopUI.gameObject.activeSelf)//二次交互关闭
        {
            ui.ToggleShopUI(false);
            return;
        }
        ui.ToggleShopUI(true);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & WhatIsPlayer) != 0)
        {
            base.OnTriggerEnter2D(collision);
            bag = playerTransform.GetComponent<Bag_Player>();
            shop.SetBag(bag);
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & WhatIsPlayer) != 0)
        {
            base.OnTriggerExit2D(collision);
            try
            {
                ui.ToggleShopUI(false);
            }
            catch
            {
                // 忽略退出运行模式时的 MissingReferenceException
                // 因为对象可能在场景卸载时已被销毁 
            }
        }
    }
}