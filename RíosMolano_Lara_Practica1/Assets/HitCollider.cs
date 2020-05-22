using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour 
    
    {
        public enum THitColliderType
        {
            HEAD,
            HELIX,
            BODY
        }
        public THitColliderType m_HitColliderType;
    public EnemyController m_DroneEnemy;
    /*public BoxCollider head;
    public BoxCollider helix1;*/
    }

