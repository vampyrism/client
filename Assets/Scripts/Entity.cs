﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private UInt32 _id = 0;
    public UInt32 ID { get => _id; set {
            if (_id == 0) {
                _id = value;
            } 
            else
            {
                throw new Exception("Tried to update Entity ID of Entity with ID " + _id);
            }
        }
    }
    public float X { get; protected set; } = 0f;
    public float Y { get; protected set; } = 0f;
    public float DX { get; protected set; } = 0f;
    public float DY { get; protected set; } = 0f;
    public float Rotation { get; protected set; } = 0f;
}
