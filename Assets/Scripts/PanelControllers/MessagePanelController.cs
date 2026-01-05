using System.Collections;
using TMPro;
using UnityEngine;

public class MessagePanelController : MonoBehaviour
{
    public static MessagePanelController Instance;

    public GameObject MessagePanel;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Show(string message, float duration = 1.5f)
    {
        // якщо вже щось показується — скидаємо
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ShowRoutine(message, duration));
    }

    private IEnumerator ShowRoutine(string message, float duration)
    {
        MessagePanel.SetActive(true);
        MessagePanel.transform.SetAsLastSibling();
        var text = MessagePanel.transform.Find("Message").GetComponent<TextMeshProUGUI>();

        text.text = message;

        yield return new WaitForSeconds(duration);

        MessagePanel.SetActive(false);
        currentCoroutine = null;
    }
}
