using UnityEngine;

using Unity.Networking.Transport;

public class V2ClientBehaviour : MonoBehaviour
{
    public NetworkDriver driver;
    public NetworkConnection connection;
    public bool done;

    void Start()
    {
        driver = NetworkDriver.Create();
        connection = default(NetworkConnection);

        var endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = 9000;
        connection = driver.Connect(endpoint);
    }

    public void OnDestroy()
    {
        driver.Dispose();
    }

    void Update()
    {
        driver.ScheduleUpdate().Complete();

        if (!connection.IsCreated)
        {
            if (!done)
            {
                Debug.Log("Er ging iets mis terwijl ik met de server probeerde te verbinden");
            }              
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("Ik ben nu verbonden met de server");

                uint value = 1;
                var writer = driver.BeginSend(connection);
                writer.WriteUInt(value);
                driver.EndSend(writer);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                uint value = stream.ReadUInt();
                Debug.Log("Ik ontvang het getal " + value + " van de server");
                done = true;
                connection.Disconnect(driver);
                connection = default(NetworkConnection);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Mijn verbinding met de server is verbroken");
                connection = default(NetworkConnection);
            }
        }
    }
}