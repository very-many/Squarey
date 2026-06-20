using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class StaffDragAndDrop : MonoBehaviour
{
    public StyleSheet styleSheet;
    public MultiStaffObject staffMulti;
    public PlayerMainCoordinator playerMainCoordinator;

    private VisualElement _root;
    private PlayerMenuCaller _caller;

    public void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _root.visible = false;

    }

    public void OpenUI(PlayerMenuCaller caller)
    {
        _caller = caller;
        _root.visible = true;
        InitializeUI();
        LoadStaff();
    }

    public void CloseUI()
    {
        _root.visible = false;
        _root.Clear();
    }

    private void InitializeUI()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Staff/DragAndDrop/StaffDragAndDrop.uss");
        if (styleSheet != null) _root.styleSheets.Add(styleSheet);

        VisualElement mainContainer = new VisualElement() { name = "slots-container" };
        _root.Add(mainContainer);

        // Create 5 Rows (3 Staff, 2 for storage)
        for (int i = 0; i < 5; i++)
        {
            VisualElement row = new VisualElement();
            row.AddToClassList("staff-row");

            // Row Label (The "Staff" Image or Icon)
            VisualElement staffIcon = new VisualElement();
            staffIcon.AddToClassList(i < 3 ? "staff-icon" : "storage-icon");
            staffIcon.Add(new Label(i < 3 ? $"Staff {i + 1}" : $"Storage {i - 2}")); //!! The names in this label are relevant for the code in OnDisable
            row.Add(staffIcon);

            // The actual drop zone for this row
            VisualElement slotContainer = new VisualElement() { name = "slots" };
            slotContainer.AddToClassList("slot-container");

            // Create some empty slots for each row
            for (int s = 0; s < 10; s++)
            {
                VisualElement slot = new VisualElement();
                slot.AddToClassList("slot");
                slotContainer.Add(slot);
            }

            row.Add(slotContainer);
            mainContainer.Add(row);
        }

        VisualElement finalRow = new VisualElement();
        finalRow.AddToClassList("staff-row");
        mainContainer.Add(finalRow);

        VisualElement spellTrashCan = new VisualElement();
        spellTrashCan.AddToClassList("spellTrashCan-icon");
        finalRow.Add(spellTrashCan);

        Button acceptChangesButton = new Button(() => AcceptChanges()) { text = "Done!" };
        acceptChangesButton.AddToClassList("acceptChanges-button");
        finalRow.Add(acceptChangesButton);

        if (staffMulti == null)
        {
            Button spawnButton = new Button(() => SpawnRandomSpell()) { text = "Add Spell to Storage" };
            mainContainer.Add(spawnButton);
        }

        CreateDragLayer();
    }

    private void AcceptChanges()
    {
        StoreSpells();
        _caller.CloseDragAndDrop();
        //set local player ready in GameOrchestrator
        if (GameOrchestrator.Instance.CurrentGameState == GameOrchestrator.GameState.Upgrade)
        {
            PlayerObjectController player = playerMainCoordinator.GetComponent<PlayerObjectController>();
            if (player != null)
            {
                player.SetUpgradeReady();
            }
        }
    }

    private void CreateDragLayer()
    {
        VisualElement dragLayer = new VisualElement();
        dragLayer.name = "drag-layer";
        dragLayer.style.position = Position.Absolute;
        dragLayer.style.left = 0;
        dragLayer.style.top = 0;
        dragLayer.style.width = Length.Percent(100);
        dragLayer.style.height = Length.Percent(100);
        dragLayer.pickingMode = PickingMode.Ignore;

        _root.Add(dragLayer);
    }

    private void LoadStaff()
    {
        List<VisualElement> rows = _root.Query<VisualElement>(className: "staff-row").ToList();

        List<Spell> spellPool = playerMainCoordinator.SpellPool;

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

            if (row.Q<Label>().text == "Storage 1")
            {
                LoadSpellsToUI(row, spellPool);
                remove10(spellPool);
            }
            if (row.Q<Label>().text == "Storage 2")
            {
                LoadSpellsToUI(row, spellPool);
                remove10(spellPool);
            }

        }

    }

    private void remove10(List<Spell> spellPool)
    {
        for (int i = 0; i < 10; i++)
        {
            if (spellPool.Count > 0)
            {
                spellPool.RemoveAt(0);
            }
        }
    }


    void SpawnRandomSpell()
    {
        var rows = _root.Query(className: "slot-container").ToList();
        VisualElement storage = rows[3];
        VisualElement storage2 = rows[4];

        var allStorageSlots = rows[3].Children().Concat(rows[4].Children()).ToList();

        SpellLibrary spellLibrary = new SpellLibrary();

        Spell spell = spellLibrary.RandomSpell();

        // Find the first empty slot in storage
        foreach (var slot in allStorageSlots)
        {
            if (slot.childCount == 0)
            {
                AddSpellToSlot(slot, spell);
                return;
            }
        }
    }

    private void StoreSpells()
    {
        //parse the UI and move the spells from the UI into the staff object
        List<VisualElement> rows = _root.Query<VisualElement>(className: "staff-row").ToList();

        List<Spell> staff1Spells = new List<Spell>();
        List<Spell> staff2Spells = new List<Spell>();
        List<Spell> staff3Spells = new List<Spell>();

        List<Spell> spellPool1 = new List<Spell>();
        List<Spell> spellPool2 = new List<Spell>();

        foreach (VisualElement row in rows) {
            if (row.Q<Label>() == null) { break; }
            if (row.Q<Label>().text == "Staff 1")
            {
                staff1Spells = WriteSpellsToStaff(row);
            }
            if (row.Q<Label>().text == "Staff 2")
            {
                staff2Spells = WriteSpellsToStaff(row);
            }
            if (row.Q<Label>().text == "Staff 3")
            {
                staff3Spells = WriteSpellsToStaff(row);
            }
            if (row.Q<Label>().text == "Storage 1")
            {
                spellPool1 = WriteSpellsToStaff(row);
            }
            if (row.Q<Label>().text == "Storage 2")
            {
                spellPool2 = WriteSpellsToStaff(row);
            }
        }

        playerMainCoordinator.SpellPool = spellPool1.Concat(spellPool2).ToList();

        staffMulti.UpdateSpells(staff1Spells, staff2Spells, staff3Spells);
    }

    private List<Spell> WriteSpellsToStaff(VisualElement staffRow)
    {
        List<Spell> spellList = new List<Spell>();
        List <VisualElement> slots = staffRow.Query<VisualElement>(className: "slot").ToList();
        foreach (VisualElement slot in slots)
        {
            if (slot.childCount > 0)
            {
                VisualElement newSpellElement = slot.Children().First();
                Spell spell = newSpellElement.userData as Spell;
                if (spell != null)
                {
                    spellList.Add(spell);
                }
            }
        }
        return spellList;
    }

    private void LoadSpellsToUI(VisualElement row, List<Spell> spellList)
    {
        List<VisualElement> slots = row.Query<VisualElement>(className: "slot").ToList();

        int slotIndex = 0;

        foreach (Spell spell in spellList)
        {
            if (slotIndex >= slots.Count)
            {
                Debug.LogWarning($"Not enough slots in row to fit all {spellList.Count} spells!");
                break;
            }

            VisualElement targetSlot = slots[slotIndex];

            AddSpellToSlot(targetSlot, spell);

            slotIndex++;
        }
    }

    private void AddSpellToSlot(VisualElement slot, Spell spell)
    {
        slot.Clear();
        VisualElement draggableSpell = new VisualElement();
        draggableSpell.AddToClassList("draggable-spell");

        // Attach the manipulator
        draggableSpell.AddManipulator(new DragAndDropManipulator(draggableSpell));

        // Add link to Spell to the userData of the VisualElement
        draggableSpell.userData = spell;

        Debug.Log($"Attempting to load: {spell.spellImagePath}");

        // Add image from spell object
        Sprite spellSprite = Resources.Load<Sprite>(spell.spellImagePath);
        draggableSpell.style.backgroundImage = new StyleBackground(spellSprite);
        draggableSpell.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

        slot.Add(draggableSpell);
    }
}




