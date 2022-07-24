using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterController2D : MonoBehaviour
{
    public float MovementSpeed = 1;
    public float BoxFloatSpeed = 1;
    public bool HoodieOn = false;
    public float gravity = 1;
    public float max_gravity = 10;
    public bool isOnFan = false;
    public float safetyMargin = 0.001f;

    private Rigidbody2D _playerRigiB;
    private SpriteRenderer _playerSprite;
    private Animator _playerAnimator;
    private float velocity_y;

    protected ContactFilter2D _contactFilter2D;
    protected ContactFilter2D _contactFilter2DBoxLayer;
    protected ContactFilter2D _contactFilter2DDeathLayer;
    protected ContactFilter2D _contactFilter2DPassLayer;

    protected List<BoxScript> liftedboxes = new List<BoxScript>();
    protected const float size = 5f;
    protected const int BoxLayer = 7;
    protected const int DeathLayer = 9;
    protected const int PassLayer = 10;

    public AudioSource flow;
    public AudioSource drag;
    public bool dragplaying = false;
    public bool flowplaying = false;


    // Start is called before the first frame update
    void Start()
    {
        _playerRigiB = GetComponent<Rigidbody2D>();
        _playerSprite = GetComponent<SpriteRenderer>();
        _playerAnimator = GetComponent<Animator>();
        velocity_y = 0.0f;

        //ContactFilter
        _contactFilter2D.useTriggers = false;
        _contactFilter2D.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        _contactFilter2D.useLayerMask = true;

        _contactFilter2DBoxLayer.useTriggers = true;
        _contactFilter2DBoxLayer.SetLayerMask(1 << BoxLayer);
        _contactFilter2DBoxLayer.useLayerMask = true;

        _contactFilter2DDeathLayer.useTriggers = true;
        _contactFilter2DDeathLayer.SetLayerMask(1 << DeathLayer);
        _contactFilter2DDeathLayer.useLayerMask = true;

        _contactFilter2DPassLayer.useTriggers = true;
        _contactFilter2DPassLayer.SetLayerMask(1 << PassLayer);
        _contactFilter2DPassLayer.useLayerMask = true;
    }

    // Update is called once per frame
    void Update()
    {
        ToggleHoodie();
    }

    private void FixedUpdate()
    {
        if (_playerAnimator.GetBool("Reset") == false)
        {
            Move();
        }
        else
        {
            if (dragplaying)
                drag.Stop();
            dragplaying = false;
            if (flowplaying)
                flow.Stop();
            flowplaying = false;
        }
    }

    void Move()
    {
        if (_playerRigiB.OverlapCollider(_contactFilter2DDeathLayer, new List<Collider2D>())> 0)
        {
            _playerAnimator.SetBool("Reset", true);
            return;
        }
        Pass();
        // Takes Pressed Button and transforms Player Pos accordingly
        var movement = Input.GetAxis("Horizontal");
        MovePlayer(new Vector2(movement, 0) * Time.fixedDeltaTime * MovementSpeed);
        if (HoodieOn == false)
        {
            // Hood off - gravity
            //velocity_y -= gravity * Time.fixedDeltaTime;
            //if (velocity_y < -max_gravity)
            {
                velocity_y = -max_gravity;
            }
            MovePlayer(new Vector2(0, 1) * Time.fixedDeltaTime * velocity_y);
        }
        else if (isOnFan == true && HoodieOn == true)
        {
            // On fan - anti-gravity
            //velocity_y += gravity * Time.fixedDeltaTime;
            //if (velocity_y > max_gravity)
            {
                velocity_y = max_gravity;
            }
            MovePlayer(new Vector2(0, 1) * Time.fixedDeltaTime * velocity_y);
        }
        else 
        {
            // Hood on, no fan - float
            velocity_y = 0.0f;
        }

        // Flips Player According to Movement Direction
        if (movement < 0)
        {
            _playerSprite.flipX = true;
            if (!HoodieOn)
            {
                if (!dragplaying)
                    drag.Play();
                dragplaying = true;
            }
        }
        else if (movement > 0)
        {
            _playerSprite.flipX = false;
            if (!HoodieOn)
            {
                if (!dragplaying)
                    drag.Play();
                dragplaying = true;
            }
        }
        else
        {
            if (dragplaying)
                drag.Stop();
            dragplaying = false;

        }
        if (HoodieOn)
        {
            if (dragplaying)
                drag.Stop();
            dragplaying = false;
            if (!flowplaying)
                flow.Play();
            flowplaying = true;
        }
        else
        {
            if (flowplaying)
                flow.Stop();
            flowplaying = false;
        }

        foreach (BoxScript box in liftedboxes)
        {
            var delta = this.gameObject.transform.position.y - box.gameObject.transform.position.y;
            if (delta < -BoxFloatSpeed * Time.fixedDeltaTime)
                delta = -BoxFloatSpeed * Time.fixedDeltaTime;
            if (delta > BoxFloatSpeed * Time.fixedDeltaTime)
                delta = BoxFloatSpeed * Time.fixedDeltaTime;
            var results = new List<RaycastHit2D>();
            var rescount = box.GetComponent<Rigidbody2D>().Cast(new Vector2(delta, 0), _contactFilter2D, results, Mathf.Abs(delta)+ safetyMargin);
            if (rescount < 1)
                box.gameObject.transform.position += new Vector3(0,delta,0);
        }
    }

    void MovePlayer(Vector2 movement)
    {
        var results = new List<RaycastHit2D>();
        var rescount = _playerRigiB.Cast(movement, _contactFilter2D, results, movement.magnitude+ safetyMargin);
        if (rescount > 0)
        {
            return;
        }
        foreach (BoxScript box in liftedboxes)
        {
            results.Clear();
            rescount = box.GetComponent<Rigidbody2D>().Cast(movement, _contactFilter2D, results, movement.magnitude+ safetyMargin);
            for (int i = 0; i < rescount;i++)
            {
                if(results[i].collider.gameObject != this.gameObject) return;
            }
        }
        _playerRigiB.position += movement;
        foreach (BoxScript box in liftedboxes)
        {
            //box.gameObject.GetComponent<Rigidbody2D>() += movement;
            box.gameObject.transform.position += new Vector3(movement.x, movement.y, 0);
        }

    }

    void ToggleHoodie()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (HoodieOn == true)
            {
                GameObject.FindGameObjectWithTag("AudioManager").GetComponent<BGAudio>().ChangeMusicStyle(false);
                HoodieOn = false;
                UnForce();
            }
            else
            {
                GameObject.FindGameObjectWithTag("AudioManager").GetComponent<BGAudio>().ChangeMusicStyle(true);
                HoodieOn = true;
                Force();   
            }
        }
        AnimateHoodie();
    }

    void AnimateHoodie()
    {
        if (HoodieOn == true)
        {
            _playerAnimator.SetBool("HoodieOn", true);
        }
        else
        {
            _playerAnimator.SetBool("HoodieOn", false);
        }
    }


    void Force()
    {
        List<Collider2D> boxes = new List<Collider2D>();
        var count = Physics2D.OverlapBox(_playerRigiB.position, new Vector2(size, transform.localScale.y*1.5f), 0, _contactFilter2DBoxLayer, boxes);
        liftedboxes.Clear();
        Debug.Log(count);
        for (int i = 0; i < count; i++)
        {
            var box = boxes[i].GetComponentInParent<BoxScript>();
            if (box == null) continue;
            box.GetComponent<Collider2D>().enabled = false;
            liftedboxes.Add(box);
        }
    }

    void UnForce()
    {
        for (int i = 0; i < liftedboxes.Count; i++)
        {
            liftedboxes[i].GetComponent<Collider2D>().enabled = true;
        }
        liftedboxes.Clear();
    }

    void Pass()
    {
        if (_playerRigiB.OverlapCollider(_contactFilter2DPassLayer, new List<Collider2D>()) > 0)
        {
            SceneManager.LoadScene(2);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wind"))
        {
            isOnFan = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wind"))
        {
            isOnFan = false;
        }
    }

}
