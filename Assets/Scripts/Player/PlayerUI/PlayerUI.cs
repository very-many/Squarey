using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;
public class PlayerUI : MonoBehaviour
{
    public StyleSheet styleSheet;
    public MultiStaffObject staffMulti;

    private VisualElement _root;

    List<VisualElement> dynamicIcons = new List<VisualElement> { null, null, null };

    public void Start()
    {
        if (staffMulti == null || !staffMulti.isOwned) return;
        InitializeUI();
    }

    private void InitializeUI()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Player/PlayerUI/PlayerUI.uss");
        if (styleSheet != null) _root.styleSheets.Add(styleSheet);

        VisualElement mainContainer = new VisualElement() { name = "main-container" };
        mainContainer.AddToClassList("main-container");


        mainContainer.style.position = Position.Absolute;

        mainContainer.style.flexDirection = FlexDirection.Row;

        mainContainer.style.left = Length.Percent(50);
        mainContainer.style.bottom = 20;
        mainContainer.style.translate = new Translate(Length.Percent(-50), 0);

        _root.Add(mainContainer);

        // Create 3 rows
        for (int i = 0; i < 3; i++)
        {
            VisualElement row = new VisualElement();
            row.AddToClassList("staff-row");

            // Row Label (The "Staff" Image or Icon)
            VisualElement staffIcon = new VisualElement();
            staffIcon.AddToClassList("staff-icon");
            staffIcon.Add(new Label($"Staff {i + 1}"));

            if (i < dynamicIcons.Count) dynamicIcons[i] = staffIcon;

            // The actual drop zone for this row
            VisualElement slotContainer = new VisualElement() { name = "slots" };
            slotContainer.AddToClassList("slot-container");

            // Create some empty slots for each row
            for (int s = 0; s < 3; s++)
            {
                VisualElement slot = new VisualElement();
                slot.AddToClassList("slot");
                slotContainer.Add(slot);
            }

            row.Add(slotContainer);
            row.Add(staffIcon);
            mainContainer.Add(row);

            InsertSpells();
        }
    }

    public void InsertSpells()
    {
        List<VisualElement> rows = _root.Query<VisualElement>(className: "staff-row").ToList();

        foreach (VisualElement row in rows) {
            foreach (VisualElement slot in row.Query<VisualElement>(className: "slot").ToList())
            {
                slot.Clear();
            }
        }

        foreach (VisualElement row in rows)
        {
            if (row.Q<Label>() == null) { break; }

            if (row.Q<Label>().text == "Staff 1")
            {
                LoadSpellsToUI(row, staffMulti.Staff_1.SpellList);
            }
            if (row.Q<Label>().text == "Staff 2")
            {
                LoadSpellsToUI(row, staffMulti.Staff_2.SpellList);
            }
            if (row.Q<Label>().text == "Staff 3")
            {
                LoadSpellsToUI(row, staffMulti.Staff_3.SpellList);
            }

        }


    }

    private void LoadSpellsToUI(VisualElement row, List<Spell> spellList)
    {
        List<VisualElement> slots = row.Query<VisualElement>(className: "slot").ToList();

        int slotIndex = 2;

        foreach (Spell spell in spellList)
        {
            if (slotIndex < 0)
            {
                Debug.LogWarning($"Not enough slots in row to fit all {spellList.Count} spells!");
                break;
            }

            VisualElement targetSlot = slots[slotIndex];

            AddSpellToSlot(targetSlot, spell);

            slotIndex--;
        }
    }

    private void AddSpellToSlot(VisualElement slot, Spell spell)
    {
        slot.Clear();
        VisualElement spellIcon = new VisualElement();
        spellIcon.AddToClassList("spell-icon");

        // Add link to Spell to the userData of the VisualElement
        spellIcon.userData = spell;

        Debug.Log($"Attempting to load: {spell.spellImagePath}");

        // Add image from spell object
        Sprite spellSprite = Resources.Load<Sprite>(spell.spellImagePath);
        spellIcon.style.backgroundImage = new StyleBackground(spellSprite);
        spellIcon.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

        slot.Add(spellIcon);
    }

    private void Update()
    {
        if (staffMulti == null) return;

        if (staffMulti.Staff_1.spellCoolDownTimer > 0)
        {
            dynamicIcons[0].AddToClassList("on-Cooldown");
            dynamicIcons[0].Q<Label>().text = staffMulti.Staff_1.spellCoolDownTimer.ToString("F1");
        }
        else
        {
            dynamicIcons[0].RemoveFromClassList("on-Cooldown");
            dynamicIcons[0].Q<Label>().text = ""; // Clears the text
        }

        if (staffMulti.Staff_2.spellCoolDownTimer > 0)
        {
            dynamicIcons[1].AddToClassList("on-Cooldown");
            dynamicIcons[1].Q<Label>().text = staffMulti.Staff_2.spellCoolDownTimer.ToString("F1");
        }
        else
        {
            dynamicIcons[1].RemoveFromClassList("on-Cooldown");
            dynamicIcons[1].Q<Label>().text = ""; // Clears the text
        }

        if (staffMulti.Staff_3.spellCoolDownTimer > 0)
        {
            dynamicIcons[2].AddToClassList("on-Cooldown");
            dynamicIcons[2].Q<Label>().text = staffMulti.Staff_3.spellCoolDownTimer.ToString("F1");
        }
        else
        {
            dynamicIcons[2].RemoveFromClassList("on-Cooldown");
            dynamicIcons[2].Q<Label>().text = ""; // Clears the text
        }
    }
}
