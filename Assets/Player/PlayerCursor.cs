using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerCursor : MonoBehaviour
{
    [SerializeField] private Transform cursor;
    [SerializeField] private GameObject First;
    [SerializeField] private GameObject Second;
    
    private Player player = null;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        player.CursorPosX.Subscribe(x =>
        {
            float drawPosX = x - GameInfo.Half + GameInfo.HalfAdjust;
            var pos = cursor.position;
            cursor.position = new Vector3(drawPosX, pos.y, pos.z);
        }).AddTo(this);
        
        player.CursorPosY.Subscribe(y =>
        {
            float drawPosY = y - GameInfo.Half + GameInfo.HalfAdjust;
            var pos = cursor.position;
            cursor.position = new Vector3(pos.x, pos.y, drawPosY);
        }).AddTo(this);

        player.Id.Subscribe(id =>
        {
            if (id == GameInfo.PlayerId.First)
            {
                First.SetActive(true);
            }
            else
            {
                Second.SetActive(true);
            }
        }).AddTo(this);
    }
}