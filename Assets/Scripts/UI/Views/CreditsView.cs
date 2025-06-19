using UnityEngine;
using UnityEngine.UI;

public class CreditsView : View
{
    [SerializeField] private Button backButton;

    public override void Initialize()
    {
        backButton.onClick.AddListener(OnBackButtonClicked);

        base.Initialize();
    }

    private void OnBackButtonClicked()
    {
        ViewManager.Instance.Show<MainMenuView>();
    }
}
