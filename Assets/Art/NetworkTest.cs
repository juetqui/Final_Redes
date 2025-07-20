using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Linq;
public class NetworkTest : MonoBehaviour
{
    [SerializeField] private NetworkRunner _networkRunnerHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        print(_networkRunnerHandler.ActivePlayers.Count());
    }
}
