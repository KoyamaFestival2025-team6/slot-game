using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReelManager : MonoBehaviour
{
    bool _isRotating = false;
    
    [SerializeField] private Button startButton;
    
    [SerializeField] private const int RealCount = 3;
    
    [SerializeField] private Button[] stopReelButtons;
    [SerializeField] private Reel[] stopReelObjs;
    
    private readonly Dictionary<Button, Reel> _reels = new Dictionary<Button, Reel>();
    
    void Start()
    {
        startButton.onClick.AddListener(StartRotating);

        if (stopReelButtons.Length != RealCount || stopReelObjs.Length != RealCount)
        {
            Debug.LogError("設定したリールの個数とボタンもしくはリール本体の個数が違います");
            return;
        }
        for (int i = 0; i < RealCount; i++)
        {
            _reels.Add(stopReelButtons[i], stopReelObjs[i]);
        }
        
        foreach (var reel in _reels)
        {
            reel.Key.onClick.AddListener(() => StopRotating(reel.Value));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void StartRotating()
    {
        foreach (var reel in _reels)
        {
            Reel tmp = reel.Value;
            tmp.StartRotating();
        }
        
        _isRotating = true;
    }
    
    public void StopRotating(Reel reel)
    {
        GameManager.Instance.AddScore(reel.StopRotating());

        foreach (var reelObj in _reels)
        {
            if (reelObj.Value.IsRotating)
            {
                return; // まだリールがどれか一つでも回転していたらreturn
            }
        }
        
        _isRotating = false; // リールがすべて回転を停止しているのでfalseにする
    }
}
