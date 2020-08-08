using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;

public class V2ServerBehaviour : MonoBehaviour
{
    public NetworkDriver driver;
    private NativeList<NetworkConnection> connections;
    private ushort port = 9000;

    Dictionary<string, string> message = new Dictionary<string, string>
    {
        { "stopServer", "Server wordt afgesloten" },
        { "startServer", "Server wordt gestart" }
    };

    private void StopServer()
    {
        Debug.Log("Server wordt afgesloten");
        driver.Dispose();
        connections.Dispose();
    }

    void Start()
    {
        Debug.Log("Server wordt gestart");
        driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = port;
        if (driver.Bind(endpoint) != 0)
        {
            Debug.LogWarning("De server kon niet gebonden worden aan de poort " + port);
        }     
        else
        {
            Debug.Log("De server is zonder problemen opgestart");
            driver.Listen();
        }           

        connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    public void OnDestroy()
    {
        StopServer();
    }

    void Update()
    {
        driver.ScheduleUpdate().Complete();

        // CleanUpConnections
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }
        // AcceptNewConnections
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
            Debug.Log("Nieuwe client is met de server verbonden");
        }

        DataStreamReader stream;
        for (int i = 0; i < connections.Length; i++)
        {
            Assert.IsTrue(connections[i].IsCreated);

            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    uint number = stream.ReadUInt();

                    Debug.Log("Server ontvangt het getal " + number + " van een client, en voegt er nu 2 aan toe.");
                    number += 2;

                    Debug.Log("Server stuurt het getal " + number + " terug naar de client");
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