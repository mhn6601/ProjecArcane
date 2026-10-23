using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public enum UIType
{
    Inventory,
    SpecialNodeInventory,
    Stat,
    Spell,
    Setting,
    Popup,
    Menu,
    Debug
}
public class UIManager : Singleton<UIManager>
{
    Player player;
    private UIBase focusedUI;
    [SerializeField]
    private List<UIBase> uiList = new List<UIBase>();
    private List<UIBase> openedUIs = new List<UIBase>();


    private Dictionary<UIType, UIBase> uiDictionary
        = new Dictionary<UIType, UIBase>();

    public Action<bool> OnPlayerInputChanged;


    protected override void Awake()
    {
        base.Awake();
        player = Player.Instance;
    }
    private void Start()
    {
        RegisterAllUI();
    }


    private void OnEnable()
    {
        player.Input.OnUIToggleInput += ToggleUI;
    }

    private void OnDisable()
    {
        player.Input.OnUIToggleInput -= ToggleUI;
    }


    // -------------------------
    // UI 등록
    // -------------------------
    private void RegisterAllUI()
    {
        
        foreach (UIBase ui in uiList)
        {
            RegisterUI(ui);
        }
    }
    public void RegisterUI(UIBase ui)
    {
        //Debug.Log($" Regist:{ui.uIType}");
        if (ui == null)
        {
            Debug.Log($" ui==null");
            return;
        }
            

        if (uiDictionary.ContainsKey(ui.uIType))
        {
           // Debug.LogWarning($"Already Registed : {ui.uIType}");
            return;
        }

        uiDictionary.Add(ui.uIType, ui);
        //Debug.Log($"UI Registed:{ui.uIType}");
    }


    private UIBase GetUI(UIType uiType)
    {
        if (uiDictionary.TryGetValue(uiType, out UIBase ui))
        {
            return ui;
        }

        Debug.LogWarning($"Can't find UI: {uiType}");

        return null;
    }


    // ESC 처리
    // - 열린 UI가 없으면 퍼즈 메뉴를 연다
    // - 열린 UI가 있으면 포커스된 UI를 닫는다 (퍼즈 메뉴가 열려 있을 때는 메뉴가 닫혀 토글이 됨)
    public void HandleEscape()
    {
        if (player.ResourceSystem.IsDead) return;   // 사망 중에는 메뉴 금지

        if (!IsAnyUIOpen())
        {
            OpenUI(UIType.Menu);
            return;
        }
        if (!IsAnyUIOpen())
        {
            OpenUI(UIType.Menu);
            return;
        }

        UIBase target = focusedUI != null
            ? focusedUI
            : openedUIs[openedUIs.Count - 1];   // 포커스가 비어 있어도 가장 위의 UI를 닫음

        CloseUI(target);
    }
    // -------------------------
    // UI 열기
    // -------------------------

    public void OpenUI(UIBase ui)
    {
        if (ui == null)
            return;

        if (openedUIs.Contains(ui))
        {
            FocusUI(ui);
            return;
        }
        CloseOtherUIs(ui);
        openedUIs.Add(ui);

        ui.Open();

        FocusUI(ui);

        UpdatePlayerInput();
    }


    public void OpenUI(UIType uiType)
    {
        UIBase ui = GetUI(uiType);

        if (ui == null)
            return;

        OpenUI(ui);
    }


    // -------------------------
    // UI 닫기
    // -------------------------

    public void CloseUI(UIBase ui)
    {
        if (ui == null)
            return;

        if (!openedUIs.Contains(ui))
            return;

        openedUIs.Remove(ui);

        ui.Close();


        if (focusedUI == ui)
        {
            if (openedUIs.Count > 0)
            {
                focusedUI = openedUIs[openedUIs.Count - 1];

                focusedUI.Focus();
            }
            else
            {
                focusedUI = null;
            }
        }

        UpdatePlayerInput();
    }


    public void CloseUI(UIType uiType)
    {
        UIBase ui = GetUI(uiType);

        if (ui == null)
            return;

        CloseUI(ui);
    }
    private void CloseOtherUIs(UIBase except)
    {
        // 순회 중 리스트가 바뀌므로 뒤에서부터 돈다
        for (int i = openedUIs.Count - 1; i >= 0; i--)
        {
            UIBase other = openedUIs[i];
            if (other == except) continue;

            openedUIs.RemoveAt(i);
            other.Close();
        }

        focusedUI = null;
    }
    public void CloseAllUI()
    {
        for (int i = openedUIs.Count - 1; i >= 0; i--)
        {
            UIBase ui = openedUIs[i];
            openedUIs.RemoveAt(i);
            ui.Close();
        }
        focusedUI = null;
    }
    // -------------------------
    // Focus
    // -------------------------

    public void FocusUI(UIBase ui)
    {
        if (ui == null)
            return;

        if (!openedUIs.Contains(ui))
            return;


        openedUIs.Remove(ui);
        openedUIs.Add(ui);

        focusedUI = ui;

        ui.Focus();
    }


    public void CloseFocusUI()
    {
        if (focusedUI == null)
            return;

        CloseUI(focusedUI);
    }


    // -------------------------
    // UI 상태 확인
    // -------------------------

    public bool IsUIOpen(UIBase ui)
    {
        return openedUIs.Contains(ui);
    }


    public bool IsUIOpen(UIType uiType)
    {
        UIBase ui = GetUI(uiType);

        if (ui == null)
            return false;

        return openedUIs.Contains(ui);
    }


    public bool IsAnyUIOpen()
    {
        return openedUIs.Count > 0;
    }


    // -------------------------
    // 플레이어 입력
    // -------------------------

    private void UpdatePlayerInput()
    {
        bool canInput = openedUIs.Count == 0;

        OnPlayerInputChanged?.Invoke(canInput);
    }


    // -------------------------
    // Inventory
    // -------------------------

    public void ToggleUI(UIType uiType)
    {
        if (player.ResourceSystem.IsDead) return;   // 사망 중 인벤토리/스펠 금지
        if (IsUIOpen(uiType))
            CloseUI(uiType);
        else
            OpenUI(uiType);
    }
}