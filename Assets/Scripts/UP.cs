using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UP : MonoBehaviour
{
    public GameObject cube1;
    public GameObject cube2;
    private Button  button1;
    private Button button2;
    private Vector2 pos1,pos2;
    // Start is called before the first frame update
    void Start()
    {
        pos1 = transform.position;
        pos2 = new Vector2(transform.position.x, transform.position.y + 8);
        button1 = cube1.GetComponent<Button>();
        button2 = cube2.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (button1.pressed && button2.pressed)
        {

            transform.position = pos2;
        }
        else
            transform.position = pos1;
    } 
}
