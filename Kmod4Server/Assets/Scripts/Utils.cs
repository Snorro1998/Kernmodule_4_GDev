using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using Unity.Networking.Transport;
using System;
using System.Linq;
using System.Text;

public static class Utils
{
    public static void WriteElementToStream<T>(T item, ref DataStreamWriter writer)
    {
        if (item.GetType() == typeof(Player))
        {
            var p = item as Player;
            writer.WriteString(p.charName);
            writer.WriteInt(p.statMaxHealth);
        }
        if (item.GetType() == typeof(Monster))
        {
            var m = item as Monster;
            writer.WriteString(m.charName);
            writer.WriteInt(m.statMaxHealth);
        }
    }


    public static void WriteListToStream<T>(ref DataStreamWriter writer, ref List<T> list)
    {
        writer.WriteInt(list.Count);

        foreach (var i in list)
        {
            WriteElementToStream(i, ref writer);
        }
    }

    public static T ReadElementFromStream<T>(ref DataStreamReader reader)
    {
        if (typeof(T) == typeof(Player))
        {
            string name = reader.ReadString().ToString();
            int hp = reader.ReadInt();
            return (T)Convert.ChangeType(new Player(name, default, hp), typeof(T));
        }
        if (typeof(T) == typeof(Monster))
        {
            string name = reader.ReadString().ToString();
            int hp = reader.ReadInt();
            return (T)Convert.ChangeType(new Monster(name, hp), typeof(T));
        }
        return default;
    }

    
    public static List<T> ReadListFromStream<T>(ref DataStreamReader reader)
    {
        List<T> list = new List<T>();
        int nItems = reader.ReadInt();
        for (int i = 0; i < nItems; i++)
        {
            T val = ReadElementFromStream<T>(ref reader);
            list.Add(val);
        }
        return list;
    }

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public static string CalculateMD5(string filename)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filename))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    public static string CalculateMD5ForString(string filename)
    {
        using (var md5 = MD5.Create())
        {
            //return md5.ComputeHash(System.BitConverter.GetBytes(filename));
            return "";
            /*
            using (var stream = File.OpenRead(filename))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }*/
        }
    }

    public static byte[] FloatArrayToByteArray(float[] data)
    {
        var byteArray = new byte[data.Length * 4];
        System.Buffer.BlockCopy(data, 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }

    public static float[] ByteArrayToFloatArray(byte[] data)
    {
        var floatArray = new float[data.Length / 4];
        System.Buffer.BlockCopy(data, 0, floatArray, 0, data.Length);
        return floatArray;
    }
}
