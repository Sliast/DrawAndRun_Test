using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    bool death;
    void Start()
    {
        
    }
   
    private void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "Enemy" && !death)
        {
            transform.GetChild(1).gameObject.GetComponent<Animator>().SetTrigger("end");
            death = true;
            Controll.Instance.Remove_players(gameObject);
            transform.parent = null;
            Destroy(gameObject, 2);
        }
        if (coll.gameObject.tag == "Add")
        {
            coll.gameObject.SetActive(false);
            Controll.Instance.Add_players();            
        }
        if (coll.gameObject.tag == "Finish")
        {
            Controll.Instance.Win();
        }
    }
}
