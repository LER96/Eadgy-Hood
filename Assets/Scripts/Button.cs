using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    Renderer render;
    protected ContactFilter2D _contactFilter2D;
    protected const int ButtonLayer = 8;
    protected BoxCollider2D colliderButton;
    public bool pressed = false;
    // Start is called before the first frame update
    void Start()
    {
        colliderButton = GetComponent<BoxCollider2D>();
        render = GetComponent<Renderer>();
        render.material.color = Color.red;

        _contactFilter2D.useTriggers = true;
        _contactFilter2D.SetLayerMask(1 << ButtonLayer);
        _contactFilter2D.useLayerMask = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool found = false;
        List<Collider2D> pushers = new List<Collider2D>();
        Debug.Log(colliderButton);
        var count = colliderButton.OverlapCollider(_contactFilter2D, pushers);
        for (int i = 0; i < count; i++)
        {
            var cc2d = pushers[i].GetComponentInParent<CharacterController2D>();
            if (cc2d != null && !cc2d.HoodieOn)
            {
                found = true;
                break;
            }
            var box = pushers[i].GetComponentInParent<BoxScript>();
            if (box != null && box.GetComponent<Collider2D>().enabled)
            {
                found = true;
                break;
            }
        }
        pressed = found;
        if (found)
        {
            Debug.Log("found!");
            render.material.color = Color.green;
        }
        else
        {
            render.material.color = Color.red;
        }
    }



    //private void OnTriggerEnter(Collider other)
    //{

    //    if(other.CompareTag("Player"))
    //    {
    //        Debug.Log("Player enter");
    //        render.material.color = Color.green;
    //    }
    //}
}
