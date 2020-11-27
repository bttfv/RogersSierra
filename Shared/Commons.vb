Imports System.Runtime.InteropServices
Imports BTTFVLibrary.Extensions
Imports GTA
Imports GTA.Math

Public Module Commons

    Public Enum SmokeColor
        Off
        [Default]
        Green
        Yellow
        Red
    End Enum

    Public Enum TrainCamera
        Off = -1
        TowardsRail
        Pilot
        Front
        RightFunnel
        RightWheels
        RightFrontWheels
        RightFront2Wheels
        RightSide
        TopCabin
        LeftSide
        LeftFunnel
        LeftWheels
        LeftFrontWheels
        LeftFront2Wheels
        Inside
        WheelieUp
        WheelieDown
    End Enum

    Friend Structure CoordinateSetting

        Dim Update As Boolean
        Dim isIncreasing As Boolean
        Dim Minimum As Single
        Dim Maximum As Single
        Dim MaxMinRatio As Single
        Dim [Step] As Single
        Dim StepRatio As Single
        Dim isFullCircle As Boolean
        Dim [Stop] As Boolean
    End Structure

    Public Enum AnimationStep
        Off
        First
        Second
        Third
        Fourth
        Fifth
    End Enum
End Module
