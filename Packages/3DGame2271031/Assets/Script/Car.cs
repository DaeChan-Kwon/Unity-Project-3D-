using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Car : MonoBehaviour
{
    // 필요 아이템
    public bool needsOil = true;
    public bool needsKey = true;

    // 문 설정
    public bool isDoorOpen = false;
    public Animator doorAnimator;
    public string doorOpenTrigger = "Open";
    public AudioClip doorOpenSound;
    public string openDoorMessage = "E - 문 열기";

    // UI
    public GameObject interactUI;
    public Text messageText;

    // 게임 종료
    public GameObject gameEndPanel;
    public float endDelay = 3f;

    // 메시지
    public string needOilMessage = "기름이 필요합니다";
    public string needKeyMessage = "열쇠를 찾으세요";
    public string canInteractMessage = "E - 자동차 시동";

    // 사운드
    public AudioClip startEngineSound;

    private bool playerInRange = false;
    private PlayerInventory playerInventory;
    private AudioSource audioSource;

    void Start()
    {
        if (interactUI != null) interactUI.SetActive(false);
        if (gameEndPanel != null) gameEndPanel.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInventory>();

            if (interactUI != null) interactUI.SetActive(true);
            UpdateMessage();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;

            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    // 상호작용 시도 (단계별 처리)
    void TryInteract()
    {
        if (playerInventory == null) return;

        // 1단계: 기름 필요
        if (needsOil)
        {
            if (playerInventory.hasOil)
            {
                playerInventory.UseOil();
                needsOil = false;
                UpdateMessage();
            }
            return;
        }

        // 2단계: 열쇠 필요
        if (needsKey)
        {
            if (playerInventory.hasKey)
            {
                needsKey = false;
                UpdateMessage();
            }
            return;
        }

        // 3단계: 문 열기
        if (!isDoorOpen)
        {
            OpenDoor();
            return;
        }

        // 4단계: 시동 걸기
        if (isDoorOpen)
        {
            StartCar();
        }
    }

    // 차문 열기
    void OpenDoor()
    {
        isDoorOpen = true;

        if (doorAnimator != null) doorAnimator.SetTrigger(doorOpenTrigger);
        if (audioSource != null && doorOpenSound != null) audioSource.PlayOneShot(doorOpenSound);

        UpdateMessage();
    }

    // 메시지 업데이트
    void UpdateMessage()
    {
        if (messageText == null) return;

        if (needsOil)
            messageText.text = needOilMessage;
        else if (needsKey)
            messageText.text = needKeyMessage;
        else if (!isDoorOpen)
            messageText.text = openDoorMessage;
        else
            messageText.text = canInteractMessage;
    }

    // 자동차 시동 및 게임 종료
    void StartCar()
    {
        if (interactUI != null) interactUI.SetActive(false);

        // BGM 페이드아웃
        if (BGMManager.instance != null) BGMManager.instance.StopBGMWithFade();

        // 시동 사운드 재생
        if (audioSource != null && startEngineSound != null) audioSource.PlayOneShot(startEngineSound);

        if (gameEndPanel != null) gameEndPanel.SetActive(true);

        Time.timeScale = 0f;

        StartCoroutine(EndGameProcess());
    }

    // 게임 종료 처리
    IEnumerator EndGameProcess()
    {
        yield return new WaitForSecondsRealtime(endDelay);

        Time.timeScale = 1f;
        AudioListener.volume = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}