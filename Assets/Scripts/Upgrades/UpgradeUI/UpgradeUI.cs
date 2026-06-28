using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeUI : MonoBehaviour
{
    public StyleSheet styleSheet;
    public PlayerMainCoordinator playerMainCoordinator;
    
    private List<Upgrade> _upgradeChoices = new List<Upgrade>();
    private VisualElement _root;
    private PlayerMenuCaller _caller;

    public void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
            
        _root.visible = false;

        _root.RegisterCallback<PointerDownEvent>(PointerDownHandler);
    }

    public void OpenUI(PlayerMenuCaller caller)
    {
        _caller = caller;
        _root.visible = true;
        GetRandomizedUpgrades();
        InitializeUI();
    }

    public void CloseUI()
    {
        _root.visible = false;
        _upgradeChoices.Clear();
        _root.Clear();
    }

    private void GetRandomizedUpgrades()
    {
        for (int i=0; i < 5; i++)
        {
            _upgradeChoices.Add(UpgradeLibrary.instance.RandomUpgrade());
        }
        
    }

    private void InitializeUI()
    {
        if (styleSheet != null) { _root.styleSheets.Add(styleSheet); }      

        // Main Container
        VisualElement mainContainer = new VisualElement();
        mainContainer.AddToClassList("slots-container");
        _root.Add(mainContainer);

        // Rows
        VisualElement row1 = new VisualElement();
        row1.AddToClassList("upgrade-row");
        mainContainer.Add(row1);

        VisualElement row2 = new VisualElement();
        row2.AddToClassList("upgrade-row");
        mainContainer.Add(row2);

        // Loop through choices
        for (int i = 0; i < _upgradeChoices.Count;  i++)
        {
            var upgradeChoice = _upgradeChoices[i];
            VisualElement currentRow = i < 2 ? row1 : row2;

            // Card Container
            Button upgradeChoiceCard = new Button();
            upgradeChoiceCard.clicked += () => ChooseUpgrade(upgradeChoiceCard);
            upgradeChoiceCard.AddToClassList("upgrade-card");
            upgradeChoiceCard.userData = upgradeChoice;

            // Icon
            VisualElement upgradeIcon = new VisualElement();
            upgradeIcon.AddToClassList("upgrade-icon");

            Sprite spellSprite = Resources.Load<Sprite>(upgradeChoice.upgradeImagePath);
            if (spellSprite != null)
            {
                upgradeIcon.style.backgroundImage = new StyleBackground(spellSprite);
            }
            upgradeIcon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

            Label upgradeText = new Label();
            upgradeText.AddToClassList("upgrade-text");
            upgradeText.text = upgradeChoice.upgradeDescription;

            Label upgradeTitle = new Label();
            upgradeTitle.AddToClassList("upgrade-title");
            upgradeTitle.text = upgradeChoice.upgradeTitle;

            // Hierarchy
            upgradeChoiceCard.Add(upgradeTitle);
            upgradeChoiceCard.Add(upgradeIcon);
            upgradeChoiceCard.Add(upgradeText);
            currentRow.Add(upgradeChoiceCard);
        }
    }

    private void PointerDownHandler(PointerDownEvent evt)
    {
        VisualElement selectedUpgrade = evt.target as VisualElement;

        if (!(selectedUpgrade is Button)) { selectedUpgrade = selectedUpgrade.parent;  }

        ChooseUpgrade(selectedUpgrade as Button);
    }

    private void ChooseUpgrade(Button upgradeChoiceCard)
    {
        if (upgradeChoiceCard == null) { return; }
        Upgrade selectedUpgrade = upgradeChoiceCard.userData as Upgrade;

        playerMainCoordinator.Upgrades.Add(selectedUpgrade);

        playerMainCoordinator.ApplyUpgrades();

        CloseUI();

        _caller.OpenDragAndDrop();
    }
}
