using System;
using UnityEngine;

namespace Multiplayer
{
    public class DDL : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}