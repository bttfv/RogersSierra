Imports System.Windows.Forms
Imports GTA
Public Class TrainMission
    Inherits Script

    Private SceneTimeTable As New List(Of MovieAnimation)

    Private currentTime As TimeSpan = TimeSpan.Zero

    Private aTrainScene As New AudioPlayer(".\scripts\BackToTheFutureV\sounds\TrainMission.wav", False, 0.8)

    Private whistleCount As AnimationStep = AnimationStep.Off

    Private funnelExpl As New PTFX(Particles.sFunnelExpl)
    Private funnelExplSound As AudioPlayer

    Public Shared Property timeMultiplier As Single = 1.0
    Public Shared Property playMusic As Boolean = True

    Private Shared _togglemission As Boolean = False

    Private Shared _startexploding As Boolean = False
    Private TrainExplosionCounter As Single = 0

    Public Shared ReadOnly Property NoStop As Boolean
        Get
            Try
                Return isTrainMissionRunning AndAlso RogersSierra.Locomotive.SpeedMPH >= 55
            Catch ex As Exception
                Return False
            End Try
        End Get
    End Property

    Public Shared ReadOnly Property isTrainMissionRunning As Boolean
        Get
            Try
                Return RogersSierra.isOnTrainMission
            Catch ex As Exception
                Return False
            End Try
        End Get
    End Property

    Public Shared Sub StartExplodingTrainScene()

        _startexploding = True
    End Sub

    Public Shared Sub ToggleMission()

        _togglemission = True
    End Sub

    Private Sub SetTrainSpeed(sDuration As TimeSpan, prevSpeedMPH As Integer, maxSpeedMPH As Integer)

        With RogersSierra

            If .Locomotive.SpeedMPH <= maxSpeedMPH Then

                .LocomotiveSpeed += (MphToMs(maxSpeedMPH - prevSpeedMPH + 1) / sDuration.TotalSeconds) * Game.LastFrameTime
            End If
        End With
    End Sub

    Private Sub StartStopMission()

        With RogersSierra

            If NoStop Then

                ShowSubtitle("You have passed the point of no return! Now it's the future or bust!")

                Exit Sub
            End If

            .isOnTrainMission = Not .isOnTrainMission

            If .isOnTrainMission Then

                With SceneTimeTable

                    .Add(New MovieAnimation(0, 0, 0, 0, 0, 25, 0, timeMultiplier)) '25
                    .Add(New MovieAnimation(1, 1, 29, 386, 1, 40, 137, timeMultiplier)) 'prima esplosione e 35
                    .Add(New MovieAnimation(2, 2, 16, 266, 2, 25, 364, timeMultiplier)) ' seconda esplosione e 40
                    .Add(New MovieAnimation(3, 2, 25, 364, 3, 3, 406, timeMultiplier)) ' > 45
                    .Add(New MovieAnimation(4, 3, 3, 406, 3, 16, 710, timeMultiplier)) '50
                    .Add(New MovieAnimation(5, 3, 20, 321, 3, 21, 587, timeMultiplier)) 'fischio
                    .Add(New MovieAnimation(6, 3, 21, 587, 3, 23, 315, timeMultiplier)) 'fischio
                    .Add(New MovieAnimation(7, 3, 23, 315, 3, 24, 511, timeMultiplier)) 'fischio
                    .Add(New MovieAnimation(8, 3, 24, 511, 3, 25, 529, timeMultiplier)) 'fischio
                    .Add(New MovieAnimation(9, 3, 25, 529, 3, 47, 111, timeMultiplier)) '> 50
                    .Add(New MovieAnimation(10, 3, 47, 111, 4, 26, 239, timeMultiplier)) '60
                    .Add(New MovieAnimation(11, 4, 26, 239, 4, 46, 395, timeMultiplier)) '70
                    .Add(New MovieAnimation(12, 5, 10, 603, 5, 13, 603, timeMultiplier)) '72 e terza esplosione
                    .Add(New MovieAnimation(13, 5, 19, 0, 5, 21, 0, timeMultiplier)) 'su e 75
                    .Add(New MovieAnimation(14, 5, 26, 0, 5, 27, 0, timeMultiplier)) 'giù
                    .Add(New MovieAnimation(15, 5, 27, 500, 5, 28, 500, timeMultiplier)) 'effetti ruote davanti
                    .Add(New MovieAnimation(16, 5, 29, 500, 5, 30, 500, timeMultiplier)) 'rimuovi effetti
                    .Add(New MovieAnimation(17, 5, 19, 0, 5, 29, 500, timeMultiplier)) 'sparks
                    .Add(New MovieAnimation(18, 5, 25, 0, 5, 40, 137, timeMultiplier)) '80
                    .Add(New MovieAnimation(19, 5, 56, 450, 7, 12, 627, timeMultiplier)) '88

                    .Add(New MovieAnimation(20, 3, 21, 87, 3, 21, 587, timeMultiplier)) 'fischio
                    .Add(New MovieAnimation(21, 3, 22, 815, 3, 23, 315, timeMultiplier)) 'fischio
                    .Add(New MovieAnimation(22, 3, 24, 11, 3, 24, 511, timeMultiplier)) 'fischio            
                End With

                .isCruiseControlOn = True

                If playMusic Then

                    aTrainScene.Play()
                End If
            Else

                .isCruiseControlOn = False
                .FunnelSmoke = SmokeColor.Default

                ResetMission()
            End If
        End With
    End Sub

    Private Sub ShonashRavine_Tick(sender As Object, e As EventArgs) Handles Me.Tick

        If IsNothing(RogersSierra) = False Then

            If _togglemission Then

                StartStopMission()
                _togglemission = False
            End If

            If RogersSierra.isOnTrainMission Then

                currentTime = currentTime.Add(TimeSpan.FromSeconds(Game.LastFrameTime))

                Dim tmpList = SceneTimeTable.Where(Function(x)
                                                       Return x.ShouldRun(currentTime)
                                                   End Function).ToList

                With RogersSierra
                    tmpList.ForEach(Sub(x)
                                        Select Case SceneTimeTable.IndexOf(x)
                                            Case 0

                                                SetTrainSpeed(x.Duration, 0, 25)
                                            Case 1
                                                If x.Executed = False Then

                                                    .FunnelSmoke = SmokeColor.Green
                                                    funnelExpl.RequestAsset()
                                                    funnelExplSound = New AudioPlayer(.Locomotive, My.Resources.FunnelExpl, "funnelexpl", False)
                                                    funnelExplSound.Play()
                                                    funnelExpl.CreateOnEntityBone(.Locomotive, Bones.sFunnel, Math.Vector3.Zero, Math.Vector3.Zero)
                                                End If

                                                SetTrainSpeed(x.Duration, 25, 35)
                                            Case 2
                                                If x.Executed = False Then

                                                    .FunnelSmoke = SmokeColor.Yellow
                                                    funnelExpl.RequestAsset()
                                                    funnelExplSound.Play()
                                                    funnelExpl.CreateOnEntityBone(.Locomotive, Bones.sFunnel, Math.Vector3.Zero, Math.Vector3.Zero)
                                                End If

                                                SetTrainSpeed(x.Duration, 35, 40)
                                            Case 3

                                                SetTrainSpeed(x.Duration, 40, 45)
                                            Case 4

                                                SetTrainSpeed(x.Duration, 45, 50)
                                            Case 5
                                                If whistleCount = AnimationStep.Off Then

                                                    .Whistle = True
                                                    whistleCount = AnimationStep.First
                                                End If
                                            Case 20
                                                If x.Executed = False Then

                                                    .Whistle = False
                                                End If
                                            Case 6
                                                If whistleCount = AnimationStep.First Then

                                                    .Whistle = True
                                                    whistleCount = AnimationStep.Second
                                                End If
                                            Case 21
                                                If x.Executed = False Then

                                                    .Whistle = False
                                                End If
                                            Case 7
                                                If whistleCount = AnimationStep.Second Then

                                                    .Whistle = True
                                                    whistleCount = AnimationStep.Third
                                                End If
                                            Case 22
                                                If x.Executed = False Then

                                                    .Whistle = False
                                                End If
                                            Case 8
                                                If whistleCount = AnimationStep.Third Then

                                                    .Whistle = True
                                                    whistleCount = AnimationStep.Off
                                                End If
                                            Case 9
                                                If x.Executed = False Then

                                                    .Whistle = False
                                                End If

                                                SetTrainSpeed(x.Duration, 50, 55)
                                            Case 10

                                                SetTrainSpeed(x.Duration, 55, 60)
                                            Case 11

                                                SetTrainSpeed(x.Duration, 60, 70)
                                            Case 12
                                                If x.Executed = False Then

                                                    .FunnelSmoke = SmokeColor.Red
                                                    .FunnelFire = True
                                                    funnelExpl.RequestAsset()
                                                    funnelExplSound.Play()
                                                    funnelExpl.CreateOnEntityBone(.Locomotive, Bones.sFunnel, Math.Vector3.Zero, Math.Vector3.Zero)
                                                End If

                                                SetTrainSpeed(x.Duration, 70, 72)
                                            Case 13
                                                If x.Executed = False Then

                                                    .DeLoreanWheelie = AnimationStep.First
                                                    .GlowingWheels = AnimationStep.First
                                                End If

                                                SetTrainSpeed(x.Duration, 72, 75)
                                            Case 14
                                                If x.Executed = False Then

                                                    .DeLoreanWheelie = AnimationStep.Second
                                                End If
                                            Case 15
                                                If x.Executed = False Then

                                                    .GlowingWheels = AnimationStep.Second
                                                End If
                                            Case 16
                                                If x.Executed = False Then

                                                    .GlowingWheels = AnimationStep.Third
                                                End If
                                            Case 17
                                                .CreateSparksWheelsDeLorean()
                                            Case 18
                                                SetTrainSpeed(x.Duration, 75, 80)
                                            Case 19
                                                SetTrainSpeed(x.Duration, 80, 88)
                                        End Select
                                    End Sub)

                    'ShowSubtitle(currentTime.TotalSeconds & " " & .tTrain.SpeedMPH & " " & .FunnelInterval)

                    If _startexploding Then

                        If aTrainScene.isPlaying Then

                            aTrainScene.Stop()
                        End If

                        TrainExplosionCounter += Game.LastFrameTime

                        If TrainExplosionCounter >= 2 Then

                            RogersSierra.Explode()

                            _startexploding = False
                            RogersSierra.isOnTrainMission = False

                            ResetMission()
                        End If
                    End If
                End With
            End If
        Else

            If SceneTimeTable.Count > 0 Then

                ResetMission()
            End If
        End If
    End Sub

    Private Sub ResetMission()

        On Error Resume Next

        SceneTimeTable.Clear()

        whistleCount = AnimationStep.Off

        currentTime = TimeSpan.Zero

        funnelExplSound.Dispose()

        If aTrainScene.isPlaying Then

            aTrainScene.Stop()
        End If
    End Sub

    Private Sub ShonashRavine_Aborted(sender As Object, e As EventArgs) Handles Me.Aborted

        If aTrainScene.isPlaying Then

            aTrainScene.Stop()
        End If
    End Sub
End Class
