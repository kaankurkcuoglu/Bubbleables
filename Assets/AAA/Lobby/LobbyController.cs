using UnityEngine;
using UnityEngine.UI;

namespace AAA.Lobby
{
    public class LobbyHud : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        
        private void Start()
        {
            _hostButton.onClick.AddListener(OnHostClicked);
        }

        private void OnHostClicked()
        {
            Debug.Log("Host clicked");
        }
    }
}