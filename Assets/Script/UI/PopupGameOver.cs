using UnityEngine;
using UnityEngine.SceneManagement;

namespace SB
{
    public class PopupGameOver : MonoBehaviour
    {
        #region UI

        public void RetryGame()
        {
            SceneManager.LoadScene("GameScene");
        }

        #endregion
    }
}
