using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public enum TItemType
    {
        LIFE,
        SHIELD,
        AMMO
    }

    public Animation itemAnimation;

    GameController m_GameController;
    public TItemType m_ItemType;

    public float m_LifePoints;
    //public int m_AmmoCount;
    //public GameObject m_Weapon;

    private void Start()
    {
        m_GameController = GameObject.Find("GameController").GetComponent<GameController>();
        //itemAnimation.CrossFade("Item1_Rotation");
    }

    private void Update()
    {
        itemAnimation.CrossFade("Item1_Rotation");
    }

    public void TakeItem()
    {
        switch (m_ItemType)
        {
            case TItemType.LIFE:
                TakeHealth();
                break;

            case TItemType.AMMO:
                TakeAmmo();
                break;

            case TItemType.SHIELD:
                TakeShield();
                break;
        }

        DestroyItem();
    }

    void TakeAmmo()
    {
        m_GameController.Player.GetComponent<FPSShooter>();
    }

    void TakeHealth()
    {
        m_GameController.Player.GetComponent<PlayerController>();
    }

    void TakeShield()
    {
        m_GameController.Player.GetComponent<PlayerController>();
    }

    void DestroyItem()
    {
        GameController.Destroy(gameObject);
    }

}
