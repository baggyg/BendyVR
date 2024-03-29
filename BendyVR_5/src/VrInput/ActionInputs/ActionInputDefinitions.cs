﻿using static Valve.VR.SteamVR_Actions;

namespace BendyVR_5.VrInput.ActionInputs;

public static class ActionInputDefinitions
{
    //Non-Dominant Hand
    public static readonly BooleanActionInput Pause = new(NonDominantHand.Pause); //Default: Y
    public static readonly BooleanActionInput Interact = new(NonDominantHand.Interact); //Default: B        

    //Dominant Hand
    public static readonly BooleanActionInput SeeingTool = new(DominantHand.SeeingTool); //Default: B
    public static readonly BooleanActionInput Weapon = new(DominantHand.Weapon); //Default: RightTrigger    

    //Movement Hand
    public static readonly BooleanActionInput Run = new(MovementHand.Run); //JoystickTrigger
    public static readonly Vector2ActionInput MoveX = new(MovementHand.Move); //Joystick
    public static readonly Vector2ActionInput MoveY = new(MovementHand.Move, true); //Joystick    

    //Rotation Hand
    public static readonly BooleanActionInput UiUp = new(RotationHand.UiUp); //JoystickUp
    public static readonly BooleanActionInput UiDown = new(RotationHand.UiDown); //JoystickDown
    public static readonly BooleanActionInput Next = new(RotationHand.Next); //JoystickRight
    public static readonly BooleanActionInput Previous = new(RotationHand.Previous); //JoystickLeft
    public static readonly Vector2ActionInput RotateX = new(RotationHand.Rotate); //JoystickX
    public static readonly BooleanActionInput SnapTurnLeft = new(RotationHand.SnapTurnLeft); //JoystickLeft
    public static readonly BooleanActionInput SnapTurnRight = new(RotationHand.SnapTurnRight); //JoystickRight
    public static readonly BooleanActionInput Jump = new(RotationHand.Jump); //Default: A
    public static readonly BooleanActionInput Cancel = new(RotationHand.Cancel); //Default: B
}