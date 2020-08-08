using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using UnityEngine.Assertions;
using Unity.Collections;

public class ServerBehaviour : MonoBehaviour
{
    public NetworkDriver driver;
    private NativeList<NetworkConnection> connections;

    // Start is called before the first frame update
    private void Start()
    {
        driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.AnyIpv4;
        endPoint.Port = 9000;
        if (driver.Bind(endPoint) != 0)
        {
            Debug.LogWarning("Server kon niet gebonden worden aan poort " + endPoint);
        }
        else
        {
            driver.Listen();
        }
        connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    private void OnDestroy()
    {
        driver.Dispose();
        connections.Dispose();
    }

    private void CleanUpConnections()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
            Debug.Log("Nieuwe verbinding gemaakt");
        }
    }

    // Update is called once per frame
    void Update()
    {
        driver.ScheduleUpdate().Complete();
        CleanUpConnections();
        AcceptNewConnections();

        DataStreamReader stream;
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                continue;
            }

            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    uint number = stream.ReadUInt();
                    Debug.Log("Ontving het getal " + number + " van de client. Voegt er nu 2 aan toe");
                    number += 2;
                    var writer = driver.BeginSend(NetworkPipeline.Null, connections[i]);
                    writer.WriteUInt(number);
                    driver.EndSend(writer);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client heeft de server verlaten");
                    connections[i] = default(NetworkConnection);
                }
            }
        }
    }
}
