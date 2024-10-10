using TMPro;
using UnityEngine;

public class ResourcesListener : MonoBehaviour
{
    [SerializeField] private ResourcesKey resource;
    [SerializeField] private TextMeshProUGUI txtValue;

    private void OnEnable()
    {
        NotificationService.Instance.Add(resource.ToString(), OnResourceChanged);
        OnResourceChanged();
    }

    private void OnDisable()
    {
        NotificationService.Instance.Remove(resource.ToString(), OnResourceChanged);
    }

    private void OnResourceChanged()
    {
        txtValue.text = resource.ToString() + ": " + GameManager.Instance.GetResources(resource).ToString();
    }
}
