using UniRx;
using UnityEngine;
using Color = UnityEngine.Color;

public class Stone : MonoBehaviour
{
    [SerializeField] private Color firstColor = default;
    [SerializeField] private Color secondColor = default;
    [SerializeField] private Color noneColor = default;
    
    public ReactiveProperty<GameInfo.PlayerId> State { get; set; } = new ReactiveProperty<GameInfo.PlayerId>();
    public Vector2Int Pos { get; set; }
    
    private Material mat;

    private void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
        State.Value = GameInfo.PlayerId.None;
        
        State.Subscribe(status =>
            {
                if (status == GameInfo.PlayerId.First)
                {
                    mat.color = firstColor;
                }
                else if(status == GameInfo.PlayerId.Second)
                {
                    mat.color = secondColor;
                }
                else
                {
                    mat.color = noneColor;
                }
            }
        ).AddTo(this);
    }
}