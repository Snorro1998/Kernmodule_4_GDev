using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Jobs;

public class JobifiedClientBehaviour : MonoBehaviour
{
    public NetworkDriver driver;
    public NativeArray<NetworkConnection> connection;
    public NativeArray<byte> done;
    public JobHandle clientJobHandle;
    //public NativeArray<NetworkConnection> m_Connection;
    //public NativeArray<byte> m_Done;


    // Start is called before the first frame update
    void Start()
    {
        driver = NetworkDriver.Create();
        connection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);
        done = new NativeArray<byte>(1, Allocator.Persistent);

        var endPoint = NetworkEndPoint.LoopbackIpv4;
        endPoint.Port = 9000;

        connection[0] = driver.Connect(endPoint);
    }

    private void OnDestroy()
    {
        clientJobHandle.Complete();
        connection.Dispose();
        driver.Dispose();
        done.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        clientJobHandle.Complete();

        var job = new ClientUpdateJob
        {
            driver = driver,
            connection = connection,
            done = done
        };

        clientJobHandle = driver.ScheduleUpdate();
        clientJobHandle = job.Schedule(clientJobHandle);

        //driver.ScheduleUpdate().Complete();
        /*
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
        */
    }
}
