using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxScript : MonoBehaviour
{
    private float _gravityModifier = -5;
    private Rigidbody2D _boxRBody2D;
    private BoxCollider2D _boxCollider2D;
    protected ContactFilter2D _contactFilter2D;
    public bool isGround = false;
    public AudioSource fall;


    // Start is called before the first frame update
    void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _boxRBody2D = GetComponent<Rigidbody2D>();

        _contactFilter2D.useTriggers = false;
        _contactFilter2D.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        _contactFilter2D.useLayerMask = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_boxCollider2D.enabled == true)
        {
            BoxFall();
        }
    }

    void BoxFall()
    {
        var results = new List<RaycastHit2D>();
        var rescount = _boxRBody2D.Cast(new Vector2(0, 1), _contactFilter2D, results, _gravityModifier * Time.fixedDeltaTime);
        if (rescount < 1)
        {
            _boxRBody2D.position += new Vector2(0, 1) * Time.fixedDeltaTime * _gravityModifier;
            if (isGround == false)
            {
                isGround = true;
                fall.Play();
            }
            else
            {
                isGround = true;
                //fall.Stop();
            }
        }
        else
        {
            isGround = false;
        }
    }
}
