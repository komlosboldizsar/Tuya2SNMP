namespace Tuya2SNMP.Tuya
{
    public enum TuyaCommandV34
    {
        UDP = 0,
        SESS_KEY_NEG_START = 3,
        SESS_KEY_NEG_RES = 4,
        SESS_KEY_NEG_FINISH = 5,
        STATUS = 8,
        HEART_BEAT = 9,
        CONTROL_NEW = 13,
        DP_QUERY_NEW = 16,
        UPDATE_DPS = 18,
        UDP_NEW = 19
    }
}
