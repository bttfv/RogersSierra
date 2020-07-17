Friend Class ExternalThread

    Private Declare Function GetForegroundWindow Lib "user32.dll" () As Int32
    Private Declare Function GetWindowThreadProcessId Lib "user32.dll" (ByVal hwnd As Int32, ByRef lpdwProcessId As Int32) As Int32

    Private Function GetActiveAppProcess() As Process
        Dim activeProcessID As IntPtr

        GetWindowThreadProcessId(GetForegroundWindow(), activeProcessID)

        Return Process.GetProcessById(activeProcessID)
    End Function

    Public Interval As Integer = 1
    Public PauseAll As Boolean = False

    Private mThread As Threading.Thread

    Sub New()

        mThread = New Threading.Thread(AddressOf CheckWindow)

        mThread.IsBackground = True
    End Sub

    Public Sub Start()
        mThread.Start()
    End Sub

    Private Sub CheckWindow()
        Do
            AudioPlayer.PauseAll(PauseAll)

            AudioPlayer.MuteAll(GetActiveAppProcess.ProcessName <> "GTA5")

            PauseAll = True

            Threading.Thread.Sleep(Interval)
        Loop
    End Sub

    Public Sub Dispose()

        mThread.Abort()
        mThread = Nothing
    End Sub
End Class
