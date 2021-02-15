using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Assets.Scripts {
public class CheckForNextLevel : MonoBehaviour {
    [SerializeField] private Color _disabledColor = Color.black;

    private void OnEnable()
    {
        bool nextLevelExists = SceneManager.sceneCountInBuildSettings > SceneManager.GetActiveScene().buildIndex + 1;
        if (GetComponent<Image>() && GetComponent<Button>()) {
            GetComponent<Button>().interactable = nextLevelExists;
            if (!nextLevelExists) {
                GetComponent<Image>().color = _disabledColor;
            }
        } else {
            gameObject.SetActive(nextLevelExists);
        }
    }
}
}