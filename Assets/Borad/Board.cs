using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;

public class Board : MonoBehaviour
{
    [SerializeField] private Stone originalStone = default;

    #region reverseUtil
    private class ReverseData
    {
        public Vector2Int Pos;
        public Vector2Int Sub;
        public GameInfo.PlayerId Id;

        public void Reset()
        {
            Id = GameInfo.PlayerId.None;
        }
    }

    private class ReverseDataDir
    {
        public bool Enable;
        public ReverseData[] Data = new ReverseData[GameInfo.Length];

        public ReverseDataDir()
        {
            for (int i = 0; i < GameInfo.Length; ++i)
            {
                Data[i] = new ReverseData();
            }
        }
        
        public void Reset()
        {
            Enable = false;
            foreach (var d in Data)
            {
                d.Reset();
            }
        }
    }
    #endregion

    private readonly Stone[,] stones = new Stone[GameInfo.Length, GameInfo.Length];
    private readonly ReverseDataDir[] reverseData = new ReverseDataDir[(int) GameInfo.Dir.Max];

    private void Awake()
    {
        for (int i = 0; i < (int)GameInfo.Dir.Max; ++i)
        {
            reverseData[i] = new ReverseDataDir();
        }
        
        // 石を生成
        CreateStone();
    }

    private void OnDestroy()
    {
        foreach (var stone in stones)
        {
            Destroy(stone.gameObject);
        }
    }

    private void Start()
    {
        // 初期状態を作成
        Reset();
    }

    #region public
    public void Reset()
    {
        foreach (var stone in stones)
        {
            stone.State.Value = GameInfo.PlayerId.None;
        }

        stones[GameInfo.Half, GameInfo.Half].State.Value = GameInfo.PlayerId.Second;
        stones[GameInfo.Half - 1, GameInfo.Half].State.Value = GameInfo.PlayerId.First;
        stones[GameInfo.Half, GameInfo.Half - 1].State.Value = GameInfo.PlayerId.First;
        stones[GameInfo.Half - 1, GameInfo.Half - 1].State.Value = GameInfo.PlayerId.Second;
    }

    public bool Put(Vector2Int pos, GameInfo.PlayerId id)
    {
        // 置いてある場所に置こうとした
        if (stones[pos.x, pos.y].State.Value != GameInfo.PlayerId.None)
        {
            return false;
        }
        
        // 判定用のキャッシュを初期化
        ResetReverseData();
        
        // 8方向の石のデータを取得
        SetReverseData(pos, reverseData);

        // 8方向の石の有効・無効を判定
        SetupReverseData(reverseData, id);
        
        // 置いたところと最短の石の間
        var done = Reverse(pos, reverseData, id);
        if (done)
        {
            // 問題なく置けるということは有効な場所だったので石を置く
            stones[pos.x, pos.y].State.Value = id;
            return true;
        }

        return false;
    }
    
    public bool IsNoEmptyStones()
    {
        return stones.Cast<Stone>().All(stone => stone.State.Value != GameInfo.PlayerId.None);
    }

    public bool CanPut(GameInfo.PlayerId id)
    {
        foreach (var stone in stones)
        {
            // 置いてある場所に置こうとした
            if (stone.State.Value != GameInfo.PlayerId.None)
            {
                continue;
            }
            
            // 判定用のキャッシュを初期化
            ResetReverseData();
            
            // 8方向のコマのデータを取得
            SetReverseData(stone.Pos, reverseData);

            // 8方向のコマの有効・無効を判定
            SetupReverseData(reverseData, id);

            for (int dir = 0; dir < (int)GameInfo.Dir.Max; ++dir)
            {
                if (!reverseData[dir].Enable)
                {
                    continue;
                }

                return true;
            }
        }

        return false;
    }

    public bool CanPut(int x, int y, GameInfo.PlayerId id)
    {
        // 判定用のキャッシュを初期化
        ResetReverseData();
            
        // 8方向のコマのデータを取得
        SetReverseData(new Vector2Int(x, y), reverseData);

        // 8方向のコマの有効・無効を判定
        SetupReverseData(reverseData, id);

        for (int dir = 0; dir < (int)GameInfo.Dir.Max; ++dir)
        {
            if (!reverseData[dir].Enable)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    public (int,int) GetStonesNum()
    {
        int firstNum = 0, secondNum = 0;
        foreach (var stone in stones)
        {
            if (stone.State.Value == GameInfo.PlayerId.First) ++firstNum;
            else if (stone.State.Value == GameInfo.PlayerId.Second) ++secondNum;
        }

        return (firstNum, secondNum);
    }

    public Stone GetStone(int x, int y)
    {
        return stones[x, y];
    }
    #endregion
    
    #region private
    private void CreateStone()
    {
        int y = 0;
        for (int h = -GameInfo.Half; h < GameInfo.Half; ++h)
        {
            int x = 0;
            for (int w = -GameInfo.Half; w < GameInfo.Half; ++w)
            {
                float posX = w + GameInfo.HalfAdjust;
                float posZ = h + GameInfo.HalfAdjust;
                stones[x, y] = Instantiate(originalStone, gameObject.transform);
                stones[x, y].gameObject.transform.position = new Vector3(posX, 0.0f, posZ);
                stones[x, y].Pos = new Vector2Int(x, y);
                ++x;
            }

            ++y;
        }
    }

    private void ResetReverseData()
    {
        foreach (var data in reverseData)
        {
            data.Reset();
        }
    }

    private void SetupReverseData(ReverseDataDir[] existStoneArray, GameInfo.PlayerId id)
    {
        for (int i = 0; i < (int) GameInfo.Dir.Max; ++i)
        {
            var dirData = existStoneArray[i];
            bool existEnemyStone = false;

            for (int j = 0; j < GameInfo.Length; ++j)
            {
                var data = dirData.Data[j];

                // 置いた位置から同じ色の石までの間にNoneがあったらダメ
                if (data.Id == GameInfo.PlayerId.None)
                {
                    break;
                }

                // 相手の石があった
                if (data.Id == GetEnemyId(id))
                {
                    existEnemyStone = true;
                    continue;
                }

                // 自分の石があった
                if (data.Id == id)
                {
                    dirData.Enable = existEnemyStone;
                    break;
                }
            }
        }
    }

    private void SetReverseData(Vector2Int pos, ReverseDataDir[] stoneArray)
    {
        // 置かれた場所から全方向の石の情報を詰める
        for(var dir = 0; dir < (int) GameInfo.Dir.Max; ++dir)
        {
            var grad = DirToGradient((GameInfo.Dir) dir);
            var tmp = pos;

            var dirData = stoneArray[dir];
            
            for (var i = 0; i < GameInfo.Length; ++i)
            {
                tmp += grad;

                // ボード外
                if (tmp.x < 0 || GameInfo.Length <= tmp.x ||
                    tmp.y < 0 || GameInfo.Length <= tmp.y)
                {
                    break;
                }

                // 石がないならひっくり返せない
                if (stones[tmp.x, tmp.y].State.Value == GameInfo.PlayerId.None)
                {
                    break;
                }

                dirData.Data[i].Pos = tmp;
                dirData.Data[i].Sub = (i + 1) * grad;
                dirData.Data[i].Id = stones[tmp.x, tmp.y].State.Value;
            }
        }
    }

    private bool Reverse(Vector2Int pos, ReverseDataDir[] data, GameInfo.PlayerId id)
    {
        var ret = false;

        foreach (var dirData in data)
        {
            // その方向にはひっくり返す石がない
            if (!dirData.Enable)
            {
                continue;
            }

            foreach (var d in dirData.Data)
            {
                // 同じ色の石があるまでひっくり返す
                if (d.Id == id)
                {
                    break;
                }

                Reverse(d.Pos.x, d.Pos.y, id);
                ret = true;
            }
        }
        
        return ret;
    }

    private void Reverse(int x, int y, GameInfo.PlayerId id)
    {
        stones[x, y].State.Value = id;
    }

    private Vector2Int DirToGradient(GameInfo.Dir dir)
    {
        switch (dir)
        {
            case GameInfo.Dir.U: return new Vector2Int(0, 1);
            case GameInfo.Dir.R: return new Vector2Int(1, 0);
            case GameInfo.Dir.D: return new Vector2Int(0, -1);
            case GameInfo.Dir.L: return new Vector2Int(-1, 0);
            case GameInfo.Dir.UR: return new Vector2Int(1, 1);
            case GameInfo.Dir.UL: return new Vector2Int(-1, 1);
            case GameInfo.Dir.DR: return new Vector2Int(1, -1);
            case GameInfo.Dir.DL: return new Vector2Int(-1, -1);
            default: return new Vector2Int();
        }
    }

    private GameInfo.PlayerId GetEnemyId(GameInfo.PlayerId id)
    {
        if (id == GameInfo.PlayerId.None) return GameInfo.PlayerId.None;
        return id == GameInfo.PlayerId.First ? GameInfo.PlayerId.Second : GameInfo.PlayerId.First;
    }
    #endregion
}