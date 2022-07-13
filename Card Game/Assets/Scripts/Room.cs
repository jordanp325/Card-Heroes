using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public char roomType;
    public int x;
    public int y;
    public int roomNumber;
    public int votes;
    public bool visible;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleVisible(bool v)
    {
        visible = v;
    }
}
