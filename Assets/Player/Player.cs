using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    public IReadOnlyReactiveProperty<int> CursorPosX => cursorPosX;
    public IReadOnlyReactiveProperty<int> CursorPosY => cursorPosY;
    public IReadOnlyReactiveProperty<GameInfo.PlayerId> Id => id;
    
    private readonly ReactiveProperty<int> cursorPosX = new ReactiveProperty<int>(4);
    private readonly ReactiveProperty<int> cursorPosY = new ReactiveProperty<int>(4);

    private GameManager gameManager;
    private ReactiveProperty<GameInfo.PlayerId> id = new ReactiveProperty<GameInfo.PlayerId>();
    private GameInfo.InputConfig inputConfig;

    private void Awake()
    {
        // TODO:Injectionしたい
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Init(GameInfo.PlayerId id, GameInfo.InputConfig inputConfig)
    {
        this.id.Value = id;
        this.inputConfig = inputConfig;
        
        InitInput(inputConfig);
    }

    public void InitInput(GameInfo.InputConfig inputConfig)
    {
        // カーソル
        this.UpdateAsObservable().Where(_ => Input.GetKeyDown(inputConfig.Up)).Subscribe(_ =>
        {
            cursorPosY.Value = Mathf.Clamp(cursorPosY.Value + 1, 0, GameInfo.Length - 1);
        }).AddTo(this);
        this.UpdateAsObservable().Where(_ => Input.GetKeyDown(inputConfig.Down)).Subscribe(_ =>
        {
            cursorPosY.Value = Mathf.Clamp(cursorPosY.Value - 1, 0, GameInfo.Length - 1);
        }).AddTo(this);
        this.UpdateAsObservable().Where(_ => Input.GetKeyDown(inputConfig.Left)).Subscribe(_ =>
        {
            cursorPosX.Value = Mathf.Clamp(cursorPosX.Value - 1, 0, GameInfo.Length - 1);
        }).AddTo(this);
        this.UpdateAsObservable().Where(_ => Input.GetKeyDown(inputConfig.Right)).Subscribe(_ =>
        {
            cursorPosX.Value = Mathf.Clamp(cursorPosX.Value + 1, 0, GameInfo.Length - 1);
        }).AddTo(this);
    }

    public void UpdateInputPut()
    {
        // 置く
        if (Input.GetKeyDown(inputConfig.Put))
        {
            this.Put(cursorPosX.Value, cursorPosY.Value);
        }
    }

    public void Put(int x, int y)
    {
        var put = gameManager.GetBoard().Put(new Vector2Int(x, y), id.Value);
        if(put)
        {
            gameManager.GetSequencer().SwitchPlayer.SetValueAndForceNotify(id.Value);
        }
    }
}