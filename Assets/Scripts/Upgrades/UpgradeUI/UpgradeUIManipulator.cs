using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UpgradeUIManipulator : PointerManipulator
{
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

    private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
    {
        throw new NotImplementedException();
    }

    private void PointerUpHandler(PointerUpEvent evt)
    {
        throw new NotImplementedException();
    }

    private void PointerMoveHandler(PointerMoveEvent evt)
    {
        throw new NotImplementedException();
    }

    private void PointerDownHandler(PointerDownEvent evt)
    {
        throw new NotImplementedException();
    }
}
