using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text chatContent;
    public Button senBtn;

    [System.Obsolete]
    void Start()
    {
        senBtn.onClick.AddListener(OnSendClicked);
        inputField.onEndEdit.AddListener(OnEndEdit);
    }

    [System.Obsolete]
    void OnEndEdit(string text)
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnSendClicked();
        }
    }

    [System.Obsolete]
    public void OnSendClicked()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            ChatManager.Intance.SendChatMessage(inputField.text);
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }
}