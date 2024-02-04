﻿namespace DistanceMeasure.Utils
{
    public enum MessagesEnum
    {
        // 0-99: ESP-NOW
        NOW_REQUEST = 0,
        NOW_CALCULATION = 1,
        NOW_KEEP_ALIVE = 2,
        NOW_ACKNOWLEDGE = 3,
        // 100-199: ESP-WIFI-MESH
        MESH_ROOT_UPDATED = 100,
        MESH_EXTERNAL_IP_ACCESS_UPDATED = 101,
        // 200-299: NETWORK
        NETWORK_GOT_ADDRESS = 200,
        NETWORK_LOST_ADDRESS = 201,
        // 300-399: UDP
        UDP_DISCOVER_REQUEST = 300,
        UDP_DISCOVER_RESPONSE = 301,
        // 400-499: TCP
    }
}
