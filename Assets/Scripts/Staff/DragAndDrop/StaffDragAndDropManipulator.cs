using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DragAndDropManipulator : PointerManipulator
{
    private VisualElement _dummyIcon;
    // Write a constructor to set target and store a reference to the
    // root of the visual tree.
    public DragAndDropManipulator(VisualElement target)
    {
        this.target = target;
        root = target.parent;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        // Register the four callbacks on target.
        target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
        target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
        target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        // Un-register the four callbacks from target.
        target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
        target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
        target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
    }

    private Vector2 targetStartPosition { get; set; }

    private Vector3 pointerStartPosition { get; set; }

    private bool enabled { get; set; }

    private VisualElement root { get; }

    // This method stores the starting position of target and the pointer,
    // makes target capture the pointer, and denotes that a drag is now in progress.
    private void PointerDownHandler(PointerDownEvent evt)
    {
        target.CapturePointer(evt.pointerId);
        pointerStartPosition = evt.position;
        enabled = true;

        //create copy
        _dummyIcon = new VisualElement();
        foreach (var className in target.GetClasses())
        {
            _dummyIcon.AddToClassList(className);
        }
        _dummyIcon.style.backgroundImage = target.resolvedStyle.backgroundImage;
        _dummyIcon.style.unityBackgroundImageTintColor = target.resolvedStyle.unityBackgroundImageTintColor;
        _dummyIcon.style.position = Position.Absolute;

        //align copy
        _dummyIcon.style.left = evt.position.x - (target.resolvedStyle.width / 2);
        _dummyIcon.style.top = evt.position.y - (target.resolvedStyle.height / 2);

        // add to drag layer
        VisualElement dragLayer = target.panel.visualTree.Q("drag-layer");
        dragLayer.Add(_dummyIcon);

        // hide originaö
        target.style.visibility = Visibility.Hidden;
    }

    private void PointerMoveHandler(PointerMoveEvent evt)
    {
        if (enabled && target.HasPointerCapture(evt.pointerId) && _dummyIcon != null)
        {
            // Move the dummy directly to the mouse
            _dummyIcon.style.left = evt.position.x - (_dummyIcon.resolvedStyle.width / 2);
            _dummyIcon.style.top = evt.position.y - (_dummyIcon.resolvedStyle.height / 2);
        }
    }

    // This method checks whether a drag is in progress and whether target has captured the pointer.
    // If both are true, makes target release the pointer.
    private void PointerUpHandler(PointerUpEvent evt)
    {
        if (enabled && target.HasPointerCapture(evt.pointerId))
        {
            target.ReleasePointer(evt.pointerId);
        }
    }

    private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
    {
        if (enabled)
        {
            VisualElement rootVisualTree = target.panel.visualTree;
            UQueryBuilder<VisualElement> allSlots = rootVisualTree.Query<VisualElement>(className: "slot");

            VisualElement closestOverlappingSlot = null;
            float bestDistance = float.MaxValue;

            foreach (var slot in allSlots.ToList())
            {
                // IMPORTANT: Check overlap against the DUMMY, as the target is hidden/static
                if (_dummyIcon.worldBound.Overlaps(slot.worldBound))
                {
                    float dist = Vector2.Distance(_dummyIcon.worldBound.center, slot.worldBound.center);
                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        closestOverlappingSlot = slot;
                    }
                }
            }

            if (closestOverlappingSlot != null && closestOverlappingSlot.childCount == 0)
            {
                closestOverlappingSlot.Add(target);
            }

            // Cleanup
            _dummyIcon?.RemoveFromHierarchy();
            _dummyIcon = null;

            target.style.visibility = Visibility.Visible;
            target.transform.position = Vector3.zero;

            enabled = false;
        }
    }

    private Vector3 RootSpaceOfSlot(VisualElement slot)
    {
        Vector2 slotWorldSpace = slot.parent.LocalToWorld(slot.layout.position);
        return root.WorldToLocal(slotWorldSpace);
    }
}

