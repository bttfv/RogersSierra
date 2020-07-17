Imports GTA
Friend Class cSmokeScript
    Inherits Script

    Public Overloads Property Interval As Integer
        Get
            Return MyBase.Interval
        End Get
        Set(value As Integer)
            MyBase.Interval = value
        End Set
    End Property

    Private Sub SmokeScript_Tick(sender As Object, e As EventArgs) Handles Me.Tick

        On Error Resume Next

        If IsNothing(RogersSierra) = False Then

            With RogersSierra

                If .FunnelSmoke <> SmokeColor.Off Then

                    .pFunnelSmoke.CreateOnEntityBone(.Locomotive, Bones.sFunnel, New Math.Vector3(0, -1.6, -0.5), New Math.Vector3(90, 0, 0), 1)

                    Select Case .FunnelSmoke
                        Case SmokeColor.Default

                        '.sFunnelSmoke.Color(132 / 255, 144 / 255, 118 / 255)
                        Case SmokeColor.Green

                            .pFunnelSmoke.Color(132 / 255, 144 / 255, 118 / 255)
                        Case SmokeColor.Yellow

                            .pFunnelSmoke.Color(217 / 255, 194 / 255, 75 / 255)
                        Case SmokeColor.Red

                            .pFunnelSmoke.Color(184 / 255, 81 / 255, 94 / 255)
                    End Select
                End If
            End With
        Else

            Abort()
        End If
    End Sub
End Class
