using System;

[Serializable]
public class LobbyResponse
{
    public string application_id;
    public string id;
    public int capacity;
    public bool locked;
    public MetadataResponse metadata;
    public string owner_id;
    public string region;
    public string secret;
    public int type;
}
