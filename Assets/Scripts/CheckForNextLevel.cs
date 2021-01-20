using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Assets.Scripts {
public class CheckForNextLevel : MonoBehaviour {
    [SerializeField] private Color _disabledColor = Color.black;

    private void OnEnable()
    {
        bool nextLevelExists = SceneManager.sceneCount < SceneManager.GetActiveScene().buildIndex + 1;
        if (GetComponent<Image>() && GetComponent<Button>()) {
            GetComponent<Button>().interactable = nextLevelExists;
            GetComponent<Image>().color = _disabledColor;
        } else {
            gameObject.SetActive(nextLevelExists);
        }
    }
}
}