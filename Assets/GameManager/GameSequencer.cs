using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameSequencer : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private GameInfo.InputConfig firstPlayerInputConfig;
    [SerializeField] private GameInfo.InputConfig secondPlayerInputConfig;
    [SerializeField] private bool auto = false;
    [SerializeField] private float autoPutTime;
    
    public ReactiveProperty<GameInfo.PlayerId> SwitchPlayer => switchPlayer;
    
    private Player playerFirst = default;
    private Player playerSecond = default;

    private IPlayer currentPlayer;
    private readonly ReactiveProperty<GameInfo.PlayerId> switchPlayer = new ReactiveProperty<GameInfo.PlayerId>();

    private Board board;

    private float timer;
    
    private void Awake()
    {
        board = FindObjectOfType<Board>();
        
        playerFirst = Instantiate(playerPrefab, transform);
        playerFirst.Init(GameInfo.PlayerId.First, firstPlayerInputConfig);
        playerSecond = Instantiate(playerPrefab, transform);
        playerSecond.Init(GameInfo.PlayerId.Second, secondPlayerInputConfig);

        currentPlayer = playerFirst;
        
        switchPlayer.SkipLatestValueOnSubscribe().Subscribe(id =>
        {
            var (firstNum, secondNum) = board.GetStonesNum();

            // もう置ける場所がない
            if (IsAllPut())
            {
                // 勝ち負けに移動
                GameEnd(firstNum, secondNum);
                return;
            }
            else
            {
                var canFirst = board.CanPut(GameInfo.PlayerId.First);
                var canSecond = board.CanPut(GameInfo.PlayerId.Second);

                if (!canFirst && !canSecond)
                {
                    // どっちも置く場所がない
                    // 勝ち負けに移動
                    GameEnd(firstNum, secondNum);
                    return;
                }

                if (id == GameInfo.PlayerId.First && !canSecond)
                {
                    // Firstが置いたけどSecondが置けない -> Secondはスキップ
                    Debug.Log("Sequence:Skip->Second");
                    return;
                }
                else if (id == GameInfo.PlayerId.Second && !canFirst)
                {
                    // Secondが置いたけどFirstが置けない -> Firstはスキップ
                    Debug.Log("Sequence:Skip->First");
                    return;
                }
            }
            
            currentPlayer = id == GameInfo.PlayerId.First ? playerSecond : playerFirst;
        }).AddTo(this);
    }

    private void Update()
    {
        if (!auto)
        {
            currentPlayer.UpdateInputPut();   
        }
        else
        {
            timer += Time.deltaTime;

            if (!(timer > autoPutTime)) return;
            timer -= autoPutTime;
            
            AutoPut(currentPlayer);
        }
    }

    private void AutoPut(IPlayer player)
    {
        for (int y = 0; y < GameInfo.Length; ++y)
        {
            for (int x = 0; x < GameInfo.Length; ++x)
            {
                var stone = board.GetStone(x, y);
                
                // Noneではない
                if (stone.State.Value != GameInfo.PlayerId.None)
                {
                    continue;
                }
                
                // 自分が置けるか
                if (!board.CanPut(x, y, player.Id.Value))
                {
                    continue;   
                }
                
                player.Put(stone.Pos.x, stone.Pos.y);
                return;
            }
        }
    }

    private bool IsAllPut()
    {
        // 空きマスがない
        var isNoEmptyStones = board.IsNoEmptyStones();
        return isNoEmptyStones;
    }

    private void GameEnd(int firstNum, int secondNum)
    {
        Debug.Log("Sequence:GameEnd");
        Debug.Log($"Sequence:First->{firstNum}, Second->{secondNum}");
    }
}
