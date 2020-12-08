using ProtoBuf;
using System;

[Serializable]
[ProtoContract]
public class Entity
{
    [ProtoMember(1)]
    public string color { get; set; }
    [ProtoMember(2)]
    public string Tiletype { get; set; }

    [ProtoMember(3)]
    public bool isAvaliable { get; set; }

}