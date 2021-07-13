using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Board board;
    private GameSequencer sequencer;

    private void Awake()
    {
        board = FindObjectOfType<Board>();
        sequencer = GetComponent<GameSequencer>();
    }

    public void GameStart()
    {
        board.Reset();
    }

    public void GameEnd()
    {
        
    }

    public Board GetBoard()
    {
        return board;
    }

    public GameSequencer GetSequencer()
    {
        return sequencer;
    }
}
