using System;
using System.Runtime.InteropServices;


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct NetEventInfo{
    public byte cmd;
    public int oid;
    public byte event_id;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
    public char[] data;

    public NetEventInfo(byte _cmd, int _oid, byte _event_id, string _data){

        cmd = _cmd;
        oid = _oid;
        event_id = _event_id;
        data = new char[256];
        Array.Copy(_data.ToCharArray(), data, Math.Min(_data.Length, 256));
    }
}
