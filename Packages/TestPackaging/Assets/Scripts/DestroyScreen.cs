using UnityEngine;

public class DestroyOffscreen : MonoBehaviour
{
    private Camera mainCamera; // 카메라 저장 변수
    private float margin = 0.5f; // 기본 0.5f로 설정

    void Start()
    {
        // 게임 시작 시 메인 카메라 할당
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 오브젝트의 월드 좌표를 뷰포트 좌표로 변환
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        // 뷰포트 좌표가 화면 영역에서 margin 만큼 벗어나면 오브젝트 파괴
        if (viewportPos.y > 1.0f + margin || viewportPos.y < 0.0f - margin ||
            viewportPos.x > 1.0f + margin || viewportPos.x < 0.0f - margin)
        {
            Destroy(gameObject); // 화면 밖으로 나간 오브젝트 삭제
        }
    }
}
