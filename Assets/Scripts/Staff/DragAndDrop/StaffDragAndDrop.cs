using Mono.Cecil;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class StaffDragAndDrop : MonoBehaviour
{
    public StyleSheet styleSheet;
    public MultiStaffObject staff;

    private VisualElement _root;

    public void Start()
    {
        InitializeUI();

        // LoadStaff();
       
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
            staffIcon.Add(new Label(i < 3 ? $"Staff {i + 1}" : "Storage"));
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

        VisualElement spellTrashCan = new VisualElement();
        spellTrashCan.AddToClassList("spellTrashCan-icon");
        _root.Add(spellTrashCan);

        // Spawn Button for Spells
        Button spawnButton = new Button(() => SpawnRandomSpell()) { text = "Add Spell to Storage" };
        _root.Add(spawnButton);

        CreateDragLayer();
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
        throw new System.NotImplementedException();

        //move the spells from the spell object into the UI

    }

    void SpawnRandomSpell()
    {
        var rows = _root.Query(className: "slot-container").ToList();
        VisualElement storage = rows[3];
        VisualElement storage2 = rows[4];

        var allStorageSlots = rows[3].Children().Concat(rows[4].Children()).ToList();

        Spell spell = new Firebolt();

        // Find the first empty slot in storage
        foreach (var slot in allStorageSlots)
        {
            if (slot.childCount == 0)
            {
                VisualElement draggableSpell = new VisualElement();
                draggableSpell.AddToClassList("object");

                // Attach the manipulator
                draggableSpell.AddManipulator(new DragAndDropManipulator(draggableSpell));

                // Add link to Spell to the userData of the VisualElement
                draggableSpell.userData = spell;

                // Add image from spell object
                Sprite spellSprite = Resources.Load<Sprite>(spell.spellImagePath);
                draggableSpell.style.backgroundImage = new StyleBackground(spellSprite);
                draggableSpell.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

                slot.Add(draggableSpell);
                return;
            }
        }
    }

    private void OnDisable()
    {
        //write staffs to your player staffs

        //write storage rows to your storage

        //clean up
    }
}




