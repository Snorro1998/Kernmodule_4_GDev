﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameEvent
{
    PING,
    SERVER_QUIT,
    LOGIN_REQUEST,
    LOGIN_RESPONSE,
    LOGOUT_REQUEST,
    LOGOUT_RESPONSE,
    REGISTER_REQUEST,
    GAME_MAZE_CREATE_NEW,
    GAME_MAZE_REVEAL_TILE,
    GAME_BATTLE_START,
    GAME_BATTLE_LOSE,
    GAME_BATTLE_VICTORY,
    GAME_BATTLE_PERFORM_ACTION,
    GAME_BATTLE_GIVE_TURN,
    GAME_GET_ITEM,
    GAME_USE_ITEM,
    PLAYERMANAGER_UPDATE_PLAYERS,
    PLAYER_DIED,
}