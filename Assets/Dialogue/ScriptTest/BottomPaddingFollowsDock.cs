using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class BottomPaddingFollowsDock : MonoBehaviour
{
    public RectTransform inputDock;     // ลากแถบ New Text มาวาง
    public int extraPadding = 8;        // เว้นเพิ่มนิดนึง

    private VerticalLayoutGroup _vg;
    private float _last;

    void Awake() { _vg = GetComponent<VerticalLayoutGroup>(); }

    void LateUpdate()
    {
        if (inputDock == null || _vg == null) return;

        // เอาความสูงจริงของ dock (รวม scale)
        float dockH = inputDock.rect.height * inputDock.lossyScale.y;

        // ถ้าใช้ SafeArea ล่าง:
        float safeBottom = 0f;
#if UNITY_IOS || UNITY_ANDROID
        var sa = Screen.safeArea;
        safeBottom = Mathf.Max(0, (float)Screen.height - (sa.y + sa.height));
#endif

        int target = Mathf.RoundToInt(dockH + safeBottom) + extraPadding;
        if (Mathf.Abs(target - _last) > 0.5f)
        {
            _vg.padding.bottom = target;
            LayoutRebuilder.MarkLayoutForRebuild((RectTransform)transform);
            _last = target;
        }
    }
}
