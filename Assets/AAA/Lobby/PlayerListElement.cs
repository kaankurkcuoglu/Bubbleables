using TMPro;
using UnityEngine;

public class PlayerListElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _connectionIdText;
    [SerializeField] private TextMeshProUGUI _ipAddressText;
    
    public void SetData(int connectionId, string ipAddress)
    {
        _connectionIdText.text = connectionId.ToString();
        _ipAddressText.text = ipAddress;
    }
}
