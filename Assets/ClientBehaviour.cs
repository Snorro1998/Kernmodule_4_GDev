using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using UnityEngine.Assertions;
using Unity.Collections;

public class ClientBehaviour : MonoBehaviour
{
    public NetworkDriver driver;
    public NetworkConnection connection;
    public bool done;

    // Start is called before the first frame update
    void Start()
    {
        driver = NetworkDriver.Create();
        connection = default(NetworkConnection);
        var endPoint = NetworkEndPoint.LoopbackIpv4;
        endPoint.Port = 9000;
        connection = driver.Connect(endPoint);
    }

    private void OnDestroy()
    {
        driver.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        driver.ScheduleUpdate().Complete();

        if (!connection.IsCreated)
        {
            if (!done)
            {
                Debug.Log("Er is iets fout gegaan met het verbinden");
            }

            return;
        }
        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We zijn nu verbonden met de server");

                uint value = 1;
                var writer = driver.BeginSend(connection);
                writer.WriteUInt(value);
                driver.EndSend(writer);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                uint value = stream.ReadUInt();
                Debug.Log("Ontving het getal " + value + " van de server");
                done = true;
                connection.Disconnect(driver);
                connection = default(NetworkConnection);
            }
        }
    }
}
