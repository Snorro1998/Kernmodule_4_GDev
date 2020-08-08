using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Jobs;

struct ClientUpdateJob : IJob
{
    public NetworkDriver driver;
    public NativeArray<NetworkConnection> connection;
    public NativeArray<byte> done;

    public void Execute()
    {
        driver.ScheduleUpdate().Complete();

        if (!connection.IsCreated)
        {
            if (done[0] != 1)
            {
                Debug.Log("Er is iets fout gegaan met het verbinden");
            }

            return;
        }
        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = connection[0].PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We zijn nu verbonden met de server");

                uint value = 1;
                var writer = driver.BeginSend(connection[0]);
                writer.WriteUInt(value);
                driver.EndSend(writer);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                uint value = stream.ReadUInt();
                Debug.Log("Ontving het getal " + value + " van de server");
                done[0] = 1;
                connection[0].Disconnect(driver);
                connection[0] = default(NetworkConnection);
            }
        }
    }
}
