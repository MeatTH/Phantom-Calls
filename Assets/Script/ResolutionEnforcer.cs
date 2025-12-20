using UnityEngine;

public class ResolutionEnforcer : MonoBehaviour
{
    // กำหนดขนาดเป้าหมาย
    private const int TargetWidth = 800;
    private const int TargetHeight = 1340;

    void Awake()
    {
        // ตั้งค่าความละเอียดจอ
        // FullScreenMode.Windowed จะช่วยให้ได้ขนาดหน้าต่างตามที่ต้องการบน PC
        Screen.SetResolution(TargetWidth, TargetHeight, FullScreenMode.Windowed);
        
        Debug.Log($"Resolution enforced to {TargetWidth}x{TargetHeight}");
    }

    void Start()
    {
        // เรียกซ้ำอีกครั้งใน Start เพื่อความชัวร์ (บางครั้ง Driver จออาจจะ override ตอน Awake)
        Screen.SetResolution(TargetWidth, TargetHeight, FullScreenMode.Windowed);
    }
}
