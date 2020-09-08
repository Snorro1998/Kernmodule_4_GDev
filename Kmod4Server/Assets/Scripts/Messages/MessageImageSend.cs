using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageImageSend : Message
{
    public override MessageType Type => MessageType.imageSend;
    public uint imageSize = 0;
    public byte[] imageData;
    //public int lastMessage = 0;
    public DataType dataType = DataType.singlething;

    public MessageImageSend()
    {

    }

    public enum DataType
    {
        singlething,
        start,
        middle,
        end
    }

    public MessageImageSend(byte[] _imageData, uint _imageSize, DataType _dataType)
    {
        imageData = _imageData;
        imageSize = _imageSize;
        dataType = _dataType;
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        dataType = (DataType)reader.ReadUInt();
        imageSize = reader.ReadUInt();
        imageData = new byte[imageSize];
        for (int i = 0; i < imageSize; i++)
        {
            imageData[i] = reader.ReadByte();
        }
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteUInt((uint)dataType);
        writer.WriteUInt(imageSize);
        int i = 0;
        foreach (byte b in imageData)
        {
            //Debug.Log("i = " + i + ", val = " + b.ToString());
            writer.WriteByte(b);
            i++;
        }
    }
}
