using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StoryUIController : MonoBehaviour
{
    [System.Serializable]
    public class StoryPage
    {
        [TextArea(3, 10)]
        public string storyText;
        public Sprite backgroundImage;
    }

    [Header("UI References")]
    public Image bgImg;
    public TextMeshProUGUI storyTextUI;

    [Header("Buttons")]
    public GameObject nextButton;
    public GameObject backButton;
    public GameObject returnButton;

    [Header("Story Data")]
    public StoryPage[] pages;

    [Header("Scene Name")]
    public string mainMenuSceneName = "MainMenu";

    private int currentPage = 0;

    void Start()
    {
        ShowPage(currentPage);
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void ShowPage(int index)
    {
        if (pages == null || pages.Length == 0)
        {
            Debug.LogWarning("No story pages assigned.");
            return;
        }

        storyTextUI.text = pages[index].storyText;
        bgImg.sprite = pages[index].backgroundImage;

        // Back button only shows if not on first page
        backButton.SetActive(index > 0);

        // Next button only shows if not on last page
        nextButton.SetActive(index < pages.Length - 1);

        // Return button only shows on last page
        returnButton.SetActive(index == pages.Length - 1);
    }
}
