[System.Serializable]
public class ServerSettings
{
    public ushort NETWORK_PORT = 9000;
    //Moet in elk geval kleiner dan 30 zijn om te voorkomen dat de verbinding tussen client en server automatisch verbroken wordt.
    public float PING_INTERVAL = 5;
    public int MAX_BYTES_PER_MESSAGE_FOR_IMAGES = 1300;
    public int MAX_CONNECTIONS = 32;

    public bool CONNECT_TO_PLAYER_DB = false;
    public bool DOWNLOAD_IMAGES = false;

    public string SESSION_ID;
}