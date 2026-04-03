using UnityEngine;
//以商人为模板制作，但作为所有NPC的基类，包含靠近触发的悬浮提示以及NPC转向
public class Object_NPC : MonoBehaviour
{
    protected Transform playerTransform;
    protected UI ui;//供子类通过ui实例访问其他UI相关脚本
    [SerializeField] protected LayerMask WhatIsPlayer;
    [SerializeField] private Transform npc;
    [SerializeField] private GameObject npcToolTip;
    private bool facingRight = true;

    [Header("漂浮按键细节")]
    [SerializeField] private float floatSpeed = 5f;
    [SerializeField] private float floatRange = 0.1f;
    private Vector3 startPosition;
    protected virtual void Awake()
    {
        ui = FindFirstObjectByType<UI>();
        startPosition = npcToolTip.transform.position;
        npcToolTip.SetActive(false);
    }
    protected virtual void Update()
    {
        HandleNpcFlip();
        HandleToolTipFloat();
    }
    protected void HandleToolTipFloat()
    {
        if (npcToolTip.activeSelf)
        {
            float yOffect = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            npcToolTip.transform.position = startPosition + new Vector3(0, yOffect);
        }
    }
    private void HandleNpcFlip()
    {
        if (playerTransform == null || npc == null)
            return;

        if (npc.position.x > playerTransform.position.x && facingRight)
        {
            npc.transform.Rotate(0, 180, 0);
            facingRight = false;
        }
        else if (npc.position.x < playerTransform.position.x && facingRight == false)
        {
            npc.transform.Rotate(0, 180, 0);
            facingRight = true;
        }
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & WhatIsPlayer) != 0)
        {
            playerTransform = collision.transform;
            npcToolTip.SetActive(true);
        }
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & WhatIsPlayer) != 0)
            npcToolTip.SetActive(false);
    }

}
