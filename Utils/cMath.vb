Imports GTA
Friend Module cMath

    Public Function MsToMph(ms As Single) As Single

        Return ms * 2.23694
    End Function

    Public Function MphToMs(mph As Single) As Single

        Return mph * 0.44704
    End Function

    Public Function ArcCos(X As Double) As Double

        Return System.Math.Atan(-X / System.Math.Sqrt(-X * X + 1)) + 2 * System.Math.Atan(1)
    End Function

    Public Function DirectionToRotation(dir As Math.Vector3, roll As Single) As Math.Vector3

        dir = Math.Vector3.Normalize(dir)

        Dim vector3_1 As Math.Vector3

        vector3_1.Z = -RadToDeg(System.Math.Atan2(dir.X, dir.Y))

        Dim vector3_2 = Math.Vector3.Normalize(New Math.Vector3(dir.Z, New Math.Vector3(dir.X, dir.Y, 0.0F).Length(), 0.0F))

        vector3_1.X = RadToDeg(System.Math.Atan2(vector3_2.X, vector3_2.Y))

        vector3_1.Y = roll
        Return vector3_1
    End Function

    Public Function GetAngularSpeedRotation(linearSpeed As Single, wheelRadius As Single, currentRotation As Single, vehDirection As Boolean, Optional modifier As Single = 1) As Single

        Dim angVel As Single = (RadToDeg(linearSpeed / System.Math.Abs(wheelRadius)) / Game.FPS) / modifier

        If vehDirection Then

            currentRotation -= angVel
        Else

            currentRotation += angVel
        End If

        Return GetWrapAngle(currentRotation)
    End Function

    Public Function GetWrapAngle(angle As Single)

        angle = angle Mod 360

        If angle < 0 Then

            angle += 360
        End If

        Return angle
    End Function

    Public Function PositiveAngle(angle As Single) As Single

        If angle < 0 Then

            angle = 360 - System.Math.Abs(angle)
        End If

        Return angle
    End Function

    Public Function DegToRad(deg As Single) As Single

        Return deg * (System.Math.PI / 180)
    End Function

    Public Function RadToDeg(rad As Single) As Single

        Return rad * (180 / System.Math.PI)
    End Function

    <Runtime.CompilerServices.Extension>
    Public Function ToVector3D(vect3 As Math.Vector3) As IrrKlang.Vector3D

        Return New IrrKlang.Vector3D(vect3.X, vect3.Y, vect3.Z)
    End Function
End Module
