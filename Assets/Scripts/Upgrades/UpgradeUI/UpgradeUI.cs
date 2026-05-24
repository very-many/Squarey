using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class UpgradeUI : MonoBehaviour
{
    public StyleSheet styleSheet;
    public PlayerMainCoordinator playerMainCoordinator;
    
    private List<Upgrade> _upgradeChoices = new List<Upgrade>();
    private VisualElement _root;

    public void Start()
    {
        GetRandomizedUpgrades();

        InitializeUI();
    }

    private void GetRandomizedUpgrades()
    {
        UpgradeLibrary upgradeLibrary = new UpgradeLibrary();
        for (int i=0; i < 5; i++)
        {
            _upgradeChoices.Add(upgradeLibrary.RandomUpgrade());
        }
        
    }

    private void InitializeUI()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        if (styleSheet != null) _root.styleSheets.Add(styleSheet);

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
        for (int i = 0; i < _upgradeChoices.Count; i++)
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

            // Hierarchy
            upgradeChoiceCard.Add(upgradeIcon);
            upgradeChoiceCard.Add(upgradeText);
            currentRow.Add(upgradeChoiceCard);
        }
    }

    private void ChooseUpgrade(Button upgradeChoiceCard)
    {
        Upgrade selectedUpgrade = upgradeChoiceCard.userData as Upgrade;

        playerMainCoordinator.Upgrades.Add(selectedUpgrade);

        playerMainCoordinator.ApplyUpgrades();

        // Code to start SpellPickerUI here
    }
}
