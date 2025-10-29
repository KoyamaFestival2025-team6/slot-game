using System;
using UnityEngine;
using UnityEngine.EventSystems; // EventSystemsの名前空間をインポート

// IPointerClickHandlerインターフェースを実装
public class Clickable3DObject : MonoBehaviour, IPointerClickHandler
{
    public int ColumnIndex; // 自分が何番目のリールか
    public event Action OnClicked;
    // このメソッドがクリック時に呼び出されます
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("3Dモデルがクリックされました！: " + gameObject.name);
        
        OnClicked?.Invoke();
    }
}