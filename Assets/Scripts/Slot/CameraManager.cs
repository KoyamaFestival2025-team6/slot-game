using Unity.Cinemachine; // Cinemachineの名前空間を忘れずに
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // インスペクターで設定する
    [SerializeField] CinemachineCamera titleCamera;
    [SerializeField] CinemachineCamera gameCamera;

    // カメラAをアクティブにする（Priorityを高くする）
    public void ChangeTitleCamera()
    {
        titleCamera.Priority = 20; // 高い優先度
        gameCamera.Priority = 10; // 低い優先度
    }

    // カメラBをアクティブにする（Priorityを高くする）
    public void ChangeGameCamera()
    {
        titleCamera.Priority = 10; // 低い優先度
        gameCamera.Priority = 20; // 高い優先度
    }
}