using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class SpellTreeUI : UIBase, IPointerClickHandler
{
    [SerializeField] private GameObject Content;
    [SerializeField] private GameObject nodeInfoPanel;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoImage;   // Spell_Video 아래 RawImage
    [SerializeField] private TextMeshProUGUI nodeNameText;
    [SerializeField] private TextMeshProUGUI nodeDescriptionText;
    [SerializeField]private Button unlockButton;
    [SerializeField] private TextMeshProUGUI unlockButtonText;
    [SerializeField] private Button equipButton;
    [SerializeField] private TextMeshProUGUI equipButtonText;
    public event Action OnEquipClicked;
    public event Action OnUnlockClicked;

    private void Awake()
    {
        if (videoPlayer != null)
            videoPlayer.prepareCompleted += OnVideoPrepared;
        if (equipButton != null)
        {
            equipButton.onClick.AddListener(() =>
            {
                OnEquipClicked?.Invoke();
            });
        }
        if (unlockButton != null)
            unlockButton.onClick.AddListener(() => OnUnlockClicked?.Invoke());
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.prepareCompleted -= OnVideoPrepared;
    }
    // 스펠 노드: 해금됐을 때만 보이게. 스탯 노드/미해금 노드는 visible=false
    public void SetEquipButton(bool visible, string label = "장착")
    {
        if (equipButton == null) return;

        equipButton.gameObject.SetActive(visible);
        if (visible && equipButtonText != null) equipButtonText.text = label;
    }
    public void SetUnlockButton(bool visible, bool interactable = true, string label = "해금")
    {
        if (unlockButton == null) return;

        unlockButton.gameObject.SetActive(visible);
        if (!visible) return;

        unlockButton.interactable = interactable;
        if (unlockButtonText != null) unlockButtonText.text = label;
    }
    public void SetNodeInfo(string nodeName, string nodeDescription, VideoClip videoClip)
    {
        nodeNameText.text = nodeName;
        nodeDescriptionText.text = nodeDescription;

        if (videoPlayer == null) return;

        // 이전 영상과 잔상을 먼저 정리
        StopVideo();

        if (videoClip == null)
        {
            videoPlayer.gameObject.SetActive(false);
            return;
        }

        videoPlayer.gameObject.SetActive(true);
        videoPlayer.clip = videoClip;
        videoPlayer.Prepare();   // 준비가 끝나면 OnVideoPrepared에서 재생
    }

    // 영상은 그대로 두고 설명 텍스트만 갱신 (스탯 변경 시 영상이 처음부터 다시 재생되는 것 방지)
    public void UpdateDescription(string nodeDescription)
    {
        nodeDescriptionText.text = nodeDescription;
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        // 준비되는 사이 패널이 닫혔다면 무시
        if (!nodeInfoPanel.activeSelf) return;

        if (videoImage != null) videoImage.enabled = true;
        vp.Play();
    }

    private void StopVideo()
    {
        if (videoPlayer == null) return;

        videoPlayer.Stop();
        videoPlayer.clip = null;

        if (videoImage != null) videoImage.enabled = false;
        ClearRenderTexture();
    }

    private void ClearRenderTexture()
    {
        RenderTexture rt = videoPlayer.targetTexture;
        if (rt == null) return;

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = previous;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 패널 내부(자식)를 눌러도 이벤트가 올라오므로, 맞은 오브젝트가 나 자신일 때만 닫기
        if (eventData.pointerCurrentRaycast.gameObject != Content) return;
        SpellTreeManager.Instance.CloseNodeInfoUI();
        SpellTreeManager.Instance.CloseSpecialNodeInfoUI();
    }

    public void ToggleNodeInfoPanel(bool isActive)
    {
        if (!isActive) StopVideo();   // 비활성화되기 전에 정리해야 텍스처가 지워짐
        nodeInfoPanel.SetActive(isActive);
    }

    public bool isPanelActive()
    {
        return nodeInfoPanel.activeSelf;
    }
    public void RefreshCurrentSpellSlot()
    {
    }
    public override void Close()
    {
        base.Close();
        SpellTreeManager.Instance.CloseNodeInfoUI();
        SpellTreeManager.Instance.CloseSpecialNodeInfoUI();
    }

}