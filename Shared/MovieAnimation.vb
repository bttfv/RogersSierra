Friend Class MovieAnimation


    Public [Step] As Integer
    Public StartTime As TimeSpan
    Public EndTime As TimeSpan
    Public Duration As TimeSpan
    Private count As Integer = 0

    Public Sub New(tStep As Integer, startMinute As Integer, startSecond As Integer, startMs As Integer, endMinute As Integer, endSecond As Integer, endMs As Integer, Optional timeRate As Single = 1)

        Me.Step = tStep

        StartTime = New TimeSpan(0, 0, startMinute, startSecond, startMs)

        EndTime = New TimeSpan(0, 0, endMinute, endSecond, endMs)

        If timeRate <> 1 Then

            StartTime = New TimeSpan(StartTime.Ticks * timeRate)

            EndTime = New TimeSpan(EndTime.Ticks * timeRate)
        End If

        Duration = EndTime - StartTime

        count = 0
    End Sub

    Public ReadOnly Property Executed As Boolean
        Get
            Return count = 2
        End Get
    End Property

    Public Function ShouldRun(tCurrentTime As TimeSpan) As Boolean

        Dim ret = StartTime.TotalMilliseconds <= tCurrentTime.TotalMilliseconds AndAlso tCurrentTime.TotalMilliseconds <= EndTime.TotalMilliseconds

        If ret Then

            If count < 2 Then

                count += 1
            End If
        End If

        Return ret
    End Function
End Class
