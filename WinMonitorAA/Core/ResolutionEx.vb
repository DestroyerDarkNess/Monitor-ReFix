Imports System
Imports System.Windows.Forms
Imports System.Runtime.InteropServices

Namespace Core
    Public Class ResolutionEx

        <StructLayout(LayoutKind.Sequential)>
        Public Structure DEVMODE1
            <MarshalAs(UnmanagedType.ByValTStr,
               SizeConst:=32)>
            Public dmDeviceName As String
            Public dmSpecVersion As Short
            Public dmDriverVersion As Short
            Public dmSize As Short
            Public dmDriverExtra As Short
            Public dmFields As Integer
            Public dmOrientation As Short
            Public dmPaperSize As Short
            Public dmPaperLength As Short
            Public dmPaperWidth As Short
            Public dmScale As Short
            Public dmCopies As Short
            Public dmDefaultSource As Short
            Public dmPrintQuality As Short
            Public dmColor As Short
            Public dmDuplex As Short
            Public dmYResolution As Short
            Public dmTTOption As Short
            Public dmCollate As Short
            <MarshalAs(UnmanagedType.ByValTStr,
               SizeConst:=32)>
            Public dmFormName As String
            Public dmLogPixels As Short
            Public dmBitsPerPel As Short
            Public dmPelsWidth As Integer
            Public dmPelsHeight As Integer
            Public dmDisplayFlags As Integer
            Public dmDisplayFrequency As Integer
            Public dmICMMethod As Integer
            Public dmICMIntent As Integer
            Public dmMediaType As Integer
            Public dmDitherType As Integer
            Public dmReserved1 As Integer
            Public dmReserved2 As Integer
            Public dmPanningWidth As Integer
            Public dmPanningHeight As Integer
        End Structure

        Class User_32

            <DllImport("user32.dll")>
            Public Shared Function EnumDisplaySettings(ByVal _
               deviceName As String, ByVal modeNum As Integer,
               ByRef devMode As DEVMODE1) As Integer
            End Function

            <DllImport("user32.dll")>
            Public Shared Function ChangeDisplaySettings(ByRef _
               devMode As DEVMODE1,
               ByVal flags As Integer) As Integer
            End Function
            Public Const ENUM_CURRENT_SETTINGS As Integer = -1
            Public Const CDS_UPDATEREGISTRY As Integer = 1
            Public Const CDS_TEST As Integer = 2
            Public Const DISP_CHANGE_SUCCESSFUL As Integer = 0
            Public Const DISP_CHANGE_RESTART As Integer = 1
            Public Const DISP_CHANGE_FAILED As Integer = -1
        End Class

        Class CResolution

            Public Shared ExpData As String = String.Empty

            Public Shared Function SetResolution(ByVal a As Integer, ByVal b As Integer, Optional ByVal MsgInfo As Boolean = False) As Boolean
                Dim screen As Screen = Screen.PrimaryScreen
                Dim iWidth As Integer = a
                Dim iHeight As Integer = b
                Dim dm As DEVMODE1 = New DEVMODE1
                dm.dmDeviceName = New String(New Char(32) {})
                dm.dmFormName = New String(New Char(32) {})
                dm.dmSize = CType(Marshal.SizeOf(dm), Short)
                If Not (0 = User_32.EnumDisplaySettings(Nothing,
                      User_32.ENUM_CURRENT_SETTINGS, dm)) Then
                    dm.dmPelsWidth = iWidth
                    dm.dmPelsHeight = iHeight
                    Dim iRet As Integer = User_32.ChangeDisplaySettings(dm,
                       User_32.CDS_TEST)
                    If iRet = User_32.DISP_CHANGE_FAILED Then
                        '  MessageBox.Show("Unable to process   your request")
                        '  MessageBox.Show("Description: Unable To Process  Your Request. Sorry For This Inconvenience.",
                        '      "Information", MessageBoxButtons.OK,
                        '      MessageBoxIcon.Information)
                        Return False
                    Else
                        iRet = User_32.ChangeDisplaySettings(dm,
                           User_32.CDS_UPDATEREGISTRY)
                        Select Case iRet
                            Case User_32.DISP_CHANGE_SUCCESSFUL
                                Return True
                            Case User_32.DISP_CHANGE_RESTART
                                If MsgInfo = True Then
                                    MessageBox.Show("Description: You Need To _
                        Reboot For The Change To Happen." &
                                 Microsoft.VisualBasic.Chr(10) &
                                 " If You Feel Any Problem After Rebooting _
                        Your Machine" & Microsoft.VisualBasic.Chr(10) _
                                 & "Then Try To Change Resolution _
                        In Safe Mode.",
                                 "Information", MessageBoxButtons.OK,
                                 MessageBoxIcon.Information)
                                End If
                                ExpData = "Error : DISP_CHANGE_RESTART"
                                Return False

                                ' break
                            Case Else
                                If MsgInfo = True Then
                                    MessageBox.Show("Description: Failed To Change The _
                        Resolution.", "Information",
                                 MessageBoxButtons.OK, MessageBoxIcon.Information)
                                End If
                                ExpData = "Error : DISP_CHANGE_UNDEFINED"
                                Return False
                                ' break
                        End Select
                    End If
                End If
                Return False
            End Function

            Public Shared Function GetScreenResolutions() As List(Of Size)
                Dim LocalList As New List(Of Size)

                Dim vDevMode As DEVMODE1 = New DEVMODE1
                ' vDevMode.dmDeviceName = New String(New Char(32) {})
                '  vDevMode.dmFormName = New String(New Char(32) {})
                '   vDevMode.dmSize = CType(Marshal.SizeOf(vDevMode), Short)

                Dim i As Integer = 0

                While User_32.EnumDisplaySettings(Nothing, i, vDevMode)

                    Dim ReSize As New Size(vDevMode.dmPelsWidth, vDevMode.dmPelsHeight)
                    LocalList.Add(ReSize)

                    i += 1
                End While

                Return RemoveDuplicate(LocalList)
            End Function

            Public Shared Function RemoveDuplicate(ByVal TheList As List(Of Size)) As List(Of Size)
                Dim Result As New List(Of Size)

                Dim Exist As Boolean = False
                For Each ElementString As Size In TheList
                    Exist = False
                    For Each ElementStringInResult As Size In Result
                        If ElementString = ElementStringInResult Then
                            Exist = True
                            Exit For
                        End If
                    Next
                    If Not Exist Then
                        Result.Add(ElementString)
                    End If
                Next

                Return Result
            End Function

        End Class
    End Class
End Namespace

