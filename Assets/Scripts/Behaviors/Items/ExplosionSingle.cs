using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSingle : MonoBehaviour
{
    void Update()
    {
        if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("explosion"))
        {
            Destroy(gameObject);
        }
        
    }
}
