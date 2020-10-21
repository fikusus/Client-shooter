using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAndSyncObject : MonoBehaviour
{
    private SocketIOComponent socket;



    // Start is called before the first frame update
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
