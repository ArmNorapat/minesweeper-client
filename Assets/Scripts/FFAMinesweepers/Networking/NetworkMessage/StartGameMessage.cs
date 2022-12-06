using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.Networking.NetworkMessage
{
    public class StartGameMessage
    {
        public bool IsGameStart => isGameStart == 1 ? true : false;

        private int isGameStart;

        public static StartGameMessage Parse(string[] parameters)
        {
            return new StartGameMessage()
            {
                isGameStart = int.Parse(parameters[0])
            };
        }

        public string Serialize()
        {
            return $"{NetworkAction.Instantiate}|{isGameStart}";
        }
    }
}