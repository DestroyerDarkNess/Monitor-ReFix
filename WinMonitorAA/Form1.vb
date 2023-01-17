
Imports System.Security.Principal

Public Class Form1

    Private WithEvents Hotkey As GlobalHotkey = Nothing
    Private WithEvents HotkeyReturn As GlobalHotkey = Nothing
    Private ResolutionHistory As New List(Of Size)
    Private CurrentScreenSize As Size = Get_Screen_Resolution(False)
    Private FullLoaded As Boolean = False
    Private ExeptionData As String = String.Empty
    Private WLimitSize As Integer = 800
    Private IsElevateAdming As Boolean = IsAdmin()

    '  Dim IsAdminBool As Boolean = New WindowsPrincipal(WindowsIdentity.GetCurrent).IsInRole(WindowsBuiltInRole.Administrator)

    Public Sub New()

        If ApplicationIsInstalled() = False Then

            If IsElevateAdming = False Then

                Dim MssageResult As DialogResult = MessageBox.Show("The application is not Installed at Windows startup." & vbNewLine &
"This is necessary for when you change monitors. " & vbNewLine &
"If your monitor is lower resolution than settings, this will help Setup." & vbNewLine & vbNewLine &
"Do you want to Add it to Windows Startup?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                If MssageResult = DialogResult.Yes Then

                    StartAsAdmin(Application.ExecutablePath)

                End If

            Else

                Add_Application_To_Startup(Startup_User.All_Users)

            End If

        End If

        InitializeComponent()

        Hotkey = New GlobalHotkey(GlobalHotkey.KeyModifier.Ctrl Or GlobalHotkey.KeyModifier.Alt, Keys.A)
        Hotkey.Tag = "[MonitorReFix] Set minimum Screen Resolution"

        HotkeyReturn = New GlobalHotkey(GlobalHotkey.KeyModifier.Ctrl Or GlobalHotkey.KeyModifier.Alt, Keys.R)
        HotkeyReturn.Tag = "[MonitorReFix] Go back to the previous resolution"



    End Sub

    Private Sub StartAsAdmin(ByVal AppDir As String)
        Dim procStartInfo As New ProcessStartInfo
        Dim procExecuting As New Process

        With procStartInfo
            .UseShellExecute = True
            .FileName = AppDir
            .WindowStyle = ProcessWindowStyle.Normal
            .Verb = "runas"
        End With

        procExecuting = Process.Start(procStartInfo)
        End
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Dim arguments As String() = Environment.GetCommandLineArgs()
        If arguments.Length >= 2 Then

            Select Case LCase(arguments(1))
                Case "-silent" : Me.WindowState = FormWindowState.Minimized
                Case Else
                    Me.Show()
            End Select

        End If
        NotifyIcon1.Icon = My.Resources.descarga
        NotifyIcon1.Visible = True
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        CheckBox1.Checked = My.Settings.Msginfo
        ResolutionHistory.Add(CurrentScreenSize)

        ComboBox1.Items.Clear()
        Dim SizeRes As List(Of Size) = Core.ResolutionEx.CResolution.GetScreenResolutions

        For Each Sizes As Size In SizeRes
            If Sizes.Width >= WLimitSize Then
                ComboBox1.Items.Add(Sizes.Width & "x" & Sizes.Height)
            End If
        Next

        SetCurrentResolutionToComboBox(CurrentScreenSize)

        FullLoaded = True
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If FullLoaded = True Then
            Dim ItemIndex As Integer = ComboBox1.SelectedIndex
            Dim ItemSelected As String = ComboBox1.Items.Item(ItemIndex)
            Dim CurrentSize As String() = ItemSelected.Split("x")
            Dim NewDefSize As New Size(CurrentSize(0), CurrentSize(1))
            SetResolution(NewDefSize)
        End If
    End Sub

    Private Sub HotKey_Press(ByVal sender As GlobalHotkey, ByVal e As GlobalHotkey.HotKeyEventArgs) Handles Hotkey.Press

        For i As Integer = 1 To ComboBox1.Items.Count
            Dim ItemSelected As String = ComboBox1.Items.Item(i - 1)
            Dim CurrentSize As String() = ItemSelected.Split("x")
            Dim NewSize As New Size(CurrentSize(0), CurrentSize(1))


            Dim ChangeResEx As Boolean = SetResolution(NewSize)

            If ChangeResEx = True Then
                SetCurrentResolutionToComboBox(NewSize)
                WriteStatus("Everything ended correctly", Color.Lime)
                Exit For
            Else
                WriteStatus(ExeptionData, Color.Red)
            End If

        Next

    End Sub

    Private Sub HotkeyReturn_Press(ByVal sender As GlobalHotkey, ByVal e As GlobalHotkey.HotKeyEventArgs) Handles HotkeyReturn.Press
        SetOldResolution()
    End Sub

    Private Sub Panel2_Click(sender As Object, e As EventArgs) Handles Panel2.Click
        Process.Start("https://www.paypal.me/SalvadorKrilewski")
    End Sub

    Private Sub Panel3_Click(sender As Object, e As EventArgs) Handles Panel3.Click
        Clipboard.SetText("0xDEB8efD8E2D069DB0e3e112CDbd589089844c258")
        WriteStatus("My address has been copied to the clipboard, Thank you!", Color.FromArgb(0, 122, 204))
    End Sub

    Public Function SetResolution(ByVal SizeTarget As Size) As Boolean
        Dim ChangeRes As Boolean = Core.ResolutionEx.CResolution.SetResolution(SizeTarget.Width, SizeTarget.Height, CheckBox1.Checked)
        If ChangeRes = False Then
            ExeptionData = Core.ResolutionEx.CResolution.ExpData
        End If
        ResolutionHistory.Add(SizeTarget)
        My.Settings.DefSize = SizeTarget
        My.Settings.Save()
        Return ChangeRes
    End Function

    Public Sub SetOldResolution()
        Dim TargetSize As Size = Nothing
        Dim CurrentRe As Size = Get_Screen_Resolution(False)

        For Each ResolutionL As Size In Core.ResolutionEx.CResolution.RemoveDuplicate(ResolutionHistory)
            If CurrentRe = ResolutionL Then
                Exit For
            End If
            TargetSize = ResolutionL
        Next

        If Not TargetSize = Nothing Then
            Dim ChangeResEx As Boolean = SetResolution(TargetSize)
            If ChangeResEx = True Then
                SetCurrentResolutionToComboBox(TargetSize)
                WriteStatus("Everything ended correctly", Color.Lime)
            Else
                WriteStatus(ExeptionData, Color.Red)
            End If
        End If
    End Sub

    Public Sub SetCurrentResolutionToComboBox(ByVal SizeTarget As Size)
        For i As Integer = 1 To ComboBox1.Items.Count
            Dim ItemSelected As String = ComboBox1.Items.Item(i - 1)
            Dim CurrentSize As String() = ItemSelected.Split("x")
            Dim NewSize As New Size(CurrentSize(0), CurrentSize(1))

            If NewSize = SizeTarget Then

                ComboBox1.SelectedIndex = (i - 1)

            End If

        Next
    End Sub

    Private Function Get_Screen_Resolution(ByVal Get_Extended_Screen_Resolution As Boolean) As Size

        If Not Get_Extended_Screen_Resolution Then
            Return New Size(My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height)
        Else
            Dim X As Integer, Y As Integer

            For Each screen As Screen In Screen.AllScreens
                X += screen.Bounds.Width
                Y += screen.Bounds.Height
            Next

            Return New Size(X, Y)
        End If

    End Function

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If FullLoaded = True Then
            My.Settings.Msginfo = CheckBox1.Checked
            My.Settings.Save()
        End If
    End Sub

    Private Sub WriteStatus(ByVal Message As String, ByVal ColorC As Color)
        Panel4.BackColor = ColorC
        Label8.Text = Message
    End Sub

    Private Sub Form1_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        If Me.WindowState = FormWindowState.Minimized Then
            Me.Hide()
        End If
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Me.Show()
        Me.TopMost = True
        Me.TopMost = False
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        End
    End Sub

#Region " Add Application To Startup "

    ' [ Add Application To Startup Function ]
    '
    ' // By Elektro H@cker
    '
    ' Examples :
    ' Add_Application_To_Startup(Startup_User.All_Users)
    ' Add_Application_To_Startup(Startup_User.Current_User)
    ' Add_Application_To_Startup(Startup_User.Current_User, "Application Name", """C:\ApplicationPath.exe""" & " -Arguments")

    Public Enum Startup_User
        Current_User
        All_Users
    End Enum

    Private Function Add_Application_To_Startup(ByVal Startup_User As Startup_User,
                                            Optional ByVal Application_Name As String = Nothing,
                                            Optional ByVal Application_Path As String = Nothing) As Boolean

        If Application_Name Is Nothing Then Application_Name = Process.GetCurrentProcess().MainModule.ModuleName
        If Application_Path Is Nothing Then Application_Path = """" & Application.ExecutablePath & """" & " -silent"

        Try
            Select Case Startup_User
                Case Startup_User.All_Users

                    RegEdit.CreateValue(fullKeyPath:="HKLM\Software\Microsoft\Windows\CurrentVersion\Run\",
                       valueName:=Application_Name,
                       valueData:=Application_Path,
                       valueType:=Microsoft.Win32.RegistryValueKind.String)

                Case Startup_User.Current_User

                    RegEdit.CreateValue(fullKeyPath:="HKCU\Software\Microsoft\Windows\CurrentVersion\Run\",
                   valueName:=Application_Name,
                   valueData:=Application_Path,
                   valueType:=Microsoft.Win32.RegistryValueKind.String)

            End Select
        Catch ex As Exception
            ' Throw New Exception(ex.Message)
            Return False
        End Try
        Return True

    End Function

    Private Function ApplicationIsInstalled() As Boolean
        Try
            Dim Application_Name As String = Process.GetCurrentProcess().MainModule.ModuleName

            Dim exist3 As Boolean = RegEdit.ExistValue(fullKeyPath:="HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run\", valueName:=Application_Name)

            If exist3 = False Then

                Return False

            Else

                Dim dataObject As Object = RegEdit.GetValueData(rootKeyName:="HKLM",
                                                                    subKeyPath:="Software\Microsoft\Windows\CurrentVersion\Run\",
                                                                    valueName:=Application_Name)

                Return IO.File.Exists(GetList(dataObject)(0))

            End If

        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try
    End Function

    Public Function GetList(ByVal str As String) As List(Of String)
        Dim ar As String()
        Dim ar2 As List(Of String) = New List(Of String)
        ar = Split(str, Chr(34))

        ' making sure there is a matching closing quote with - (UBound(ar) And 1)
        For a As Integer = 1 To UBound(ar) - (UBound(ar) And 1) Step 2
            ar2.Add(ar(a))
        Next a

        Return ar2
    End Function

    Private Function IsAdmin() As Boolean
        Try
            Dim identity = WindowsIdentity.GetCurrent()
            Dim principal = New WindowsPrincipal(identity)
            Dim isElevated As Boolean = principal.IsInRole(WindowsBuiltInRole.Administrator)
            Return isElevated
        Catch ex As Exception
            Return False
        End Try
    End Function

#End Region

End Class
