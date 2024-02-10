namespace DistanceMeasure.Utils
{
    public enum MessagesEnum
    {
        // ESP-NOW
        NOW_REQUEST,
        NOW_CALCULATION,
        NOW_KEEP_ALIVE,
        NOW_ACKNOWLEDGE,
        // ESP-WIFI-MESH
        MESH_PARENT_CONNECTED,
        MESH_ROOT_UPDATED,
        MESH_NODE_CONNECTED,
        MESH_GET_NODES,
        // NETWORK
        NETWORK_GOT_ADDRESS,
        NETWORK_LOST_ADDRESS,
        // UDP
        UDP_DISCOVER_REQUEST,
        UDP_DISCOVER_RESPONSE,
        // TCP
        TCP_NODE_CONNECTED,
        TCP_NODE_DISCONNECTED, // TODO: Implement
        TCP_GET_NODES_REQUEST,
        TCP_SERVER_OPTIONS_REQUEST,
        TCP_SERVER_OPTIONS_RESPONSE,
    }
}
