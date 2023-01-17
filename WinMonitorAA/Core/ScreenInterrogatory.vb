Imports System.ComponentModel
Imports System.Runtime.InteropServices

Namespace Core
    Public Class ScreenInterrogatory

        Public Const ERROR_SUCCESS As Integer = 0

#Region " Enums "

        Public Enum QUERY_DEVICE_CONFIG_FLAGS As UInteger
            QDC_ALL_PATHS = &H1
            QDC_ONLY_ACTIVE_PATHS = &H2
            QDC_DATABASE_CURRENT = &H4
        End Enum

        Public Enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY As UInteger
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = &HFFFFFFFFUI
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL = &H80000000UI
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32 = &HFFFFFFFFUI
        End Enum

        Public Enum DISPLAYCONFIG_SCANLINE_ORDERING As UInteger
            DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0
            DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3
            DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32 = &HFFFFFFFFUI
        End Enum

        Public Enum DISPLAYCONFIG_ROTATION As UInteger
            DISPLAYCONFIG_ROTATION_IDENTITY = 1
            DISPLAYCONFIG_ROTATION_ROTATE90 = 2
            DISPLAYCONFIG_ROTATION_ROTATE180 = 3
            DISPLAYCONFIG_ROTATION_ROTATE270 = 4
            DISPLAYCONFIG_ROTATION_FORCE_UINT32 = &HFFFFFFFFUI
        End Enum

        Public Enum DISPLAYCONFIG_SCALING As UInteger
            DISPLAYCONFIG_SCALING_IDENTITY = 1
            DISPLAYCONFIG_SCALING_CENTERED = 2
            DISPLAYCONFIG_SCALING_STRETCHED = 3
            DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4
            DISPLAYCONFIG_SCALING_CUSTOM = 5
            DISPLAYCONFIG_SCALING_PREFERRED = 128
            DISPLAYCONFIG_SCALING_FORCE_UINT32 = &HFFFFFFFFUI
        End Enum

        Public Enum DISPLAYCONFIG_PIXELFORMAT As UInteger
            DISPLAYCONFIG_PIXELFORMAT_8BPP = 1
            DISPLAYCONFIG_PIXELFORMAT_16BPP = 2
            DISPLAYCONFIG_PIXELFORMAT_24BPP = 3
            DISPLAYCONFIG_PIXELFORMAT_32BPP = 4
            DISPLAYCONFIG_PIXELFORMAT_NONGDI = 5
            DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32 = &HFFFFFFFFUI
        End Enum

        Public Enum DISPLAYCONFIG_MODE_INFO_TYPE As UInteger
            DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1
            DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2
            DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32 = &HFFFFFFFFUI
        End Enum

        Public Enum DISPLAYCONFIG_DEVICE_INFO_TYPE As UInteger
            DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3
            DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4
            DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6
            DISPLAYCONFIG_DEVICE_INFO_FORCE_UINT32 = &HFFFFFFFFUI
        End Enum

#End Region

#Region " Structs "

        <StructLayout(LayoutKind.Sequential)>
        Public Structure LUID
            Public LowPart As UInteger
            Public HighPart As Integer
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure DISPLAYCONFIG_PATH_SOURCE_INFO
            Public adapterId As LUID
            Public id As UInteger
            Public modeInfoIdx As UInteger
            Public statusFlags As UInteger
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure DISPLAYCONFIG_PATH_TARGET_INFO
            Public adapterId As LUID
            Public id As UInteger
            Public modeInfoIdx As UInteger
            Private outputTechnology As DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY
            Private rotation As DISPLAYCONFIG_ROTATION
            Private scaling As DISPLAYCONFIG_SCALING
            Private refreshRate As DISPLAYCONFIG_RATIONAL
            Private scanLineOrdering As DISPLAYCONFIG_SCANLINE_ORDERING
            Public targetAvailable As Boolean
            Public statusFlags As UInteger
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure DISPLAYCONFIG_RATIONAL
            Public Numerator As UInteger
            Public Denominator As UInteger
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure DISPLAYCONFIG_PATH_INFO
            Public sourceInfo As DISPLAYCONFIG_PATH_SOURCE_INFO
            Public targetInfo As DISPLAYCONFIG_PATH_TARGET_INFO
            Public flags As UInteger
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure DISPLAYCONFIG_2DREGION
            Public cx As UInteger
            Public cy As UInteger
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure DISPLAYCONFIG_VIDEO_SIGNAL_INFO
            Public pixelRate As ULong
            Public hSyncFreq As DISPLAYCONFIG_RATIONAL
            Public vSyncFreq As DISPLAYCONFIG_RATIONAL
            Public activeSize As DISPLAYCONFIG_2DREGION
            Public totalSize As DISPLAYCONFIG_2DREGION
            Public videoStandard As UInteger
            Public scanLineOrdering As DISPLAYCONFIG_SCANLINE_ORDERING
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure DISPLAYCONFIG_TARGET_MODE
            Public targetVideoSignalInfo As DISPLAYCONFIG_VIDEO_SIGNAL_INFO
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure POINTL
            Private x As Integer
            Private y As Integer
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure DISPLAYCONFIG_SOURCE_MODE
            Public width As UInteger
            Public height As UInteger
            Public pixelFormat As DISPLAYCONFIG_PIXELFORMAT
            Public position As POINTL
        End Structure

        <StructLayout(LayoutKind.Explicit)>
        Public Structure DISPLAYCONFIG_MODE_INFO_UNION
            <FieldOffset(0)>
            Public targetMode As DISPLAYCONFIG_TARGET_MODE
            <FieldOffset(0)>
            Public sourceMode As DISPLAYCONFIG_SOURCE_MODE
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure DISPLAYCONFIG_MODE_INFO
            Public infoType As DISPLAYCONFIG_MODE_INFO_TYPE
            Public id As UInteger
            Public adapterId As LUID
            Public modeInfo As DISPLAYCONFIG_MODE_INFO_UNION
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
            Public value As UInteger
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure DISPLAYCONFIG_DEVICE_INFO_HEADER
            Public type As DISPLAYCONFIG_DEVICE_INFO_TYPE
            Public size As UInteger
            Public adapterId As LUID
            Public id As UInteger
        End Structure

        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
        Public Structure DISPLAYCONFIG_TARGET_DEVICE_NAME
            Public header As DISPLAYCONFIG_DEVICE_INFO_HEADER
            Public flags As DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
            Public outputTechnology As DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY
            Public edidManufactureId As UShort
            Public edidProductCodeId As UShort
            Public connectorInstance As UInteger
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
            Public monitorFriendlyDeviceName As String
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
            Public monitorDevicePath As String
        End Structure

#End Region

#Region " PInvokes "

        <DllImport("user32.dll")>
        Public Shared Function GetDisplayConfigBufferSizes(ByVal flags As QUERY_DEVICE_CONFIG_FLAGS,
                                                           <Out> ByRef numPathArrayElements As UInteger,
                                                           <Out> ByRef numModeInfoArrayElements As UInteger) As Integer
        End Function

        <DllImport("user32.dll")>
        Public Shared Function QueryDisplayConfig(ByVal flags As QUERY_DEVICE_CONFIG_FLAGS, ByRef numPathArrayElements As UInteger,
    <Out> ByVal PathInfoArray As DISPLAYCONFIG_PATH_INFO(), ByRef numModeInfoArrayElements As UInteger,
    <Out> ByVal ModeInfoArray As DISPLAYCONFIG_MODE_INFO(), ByVal currentTopologyId As IntPtr) As Integer
        End Function

        <DllImport("user32.dll")>
        Public Shared Function DisplayConfigGetDeviceInfo(ByRef deviceName As DISPLAYCONFIG_TARGET_DEVICE_NAME) As Integer
        End Function

#End Region

        Private Shared Function MonitorFriendlyName(ByVal adapterId As LUID, ByVal targetId As UInteger) As String
            Dim deviceName = New DISPLAYCONFIG_TARGET_DEVICE_NAME With {
                  .header = New DISPLAYCONFIG_DEVICE_INFO_HEADER With {
                                 .size = CUInt(Marshal.SizeOf(Of DISPLAYCONFIG_TARGET_DEVICE_NAME)),
                                 .adapterId = adapterId,
                                 .id = targetId,
                                 .type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME}
 }

            Dim [error] = DisplayConfigGetDeviceInfo(deviceName)
            If [error] <> ERROR_SUCCESS Then Throw New Win32Exception([error])
            Return deviceName.monitorFriendlyDeviceName
        End Function

        Private Shared Iterator Function GetAllMonitorsFriendlyNames() As IEnumerable(Of String)
            Dim pathCount, modeCount As UInteger
            Dim [error] = GetDisplayConfigBufferSizes(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, pathCount, modeCount)
            If [error] <> ERROR_SUCCESS Then Throw New Win32Exception([error])
            Dim displayPaths = New DISPLAYCONFIG_PATH_INFO(pathCount - 1) {}
            Dim displayModes = New DISPLAYCONFIG_MODE_INFO(modeCount - 1) {}
            [error] = QueryDisplayConfig(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, pathCount, displayPaths, modeCount, displayModes, IntPtr.Zero)
            If [error] <> ERROR_SUCCESS Then Throw New Win32Exception([error])

            For i = 0 To modeCount - 1
                MsgBox(MonitorFriendlyName(displayModes(i).adapterId, displayModes(i).id))
                If displayModes(i).infoType = DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET Then Yield MonitorFriendlyName(displayModes(i).adapterId, displayModes(i).id)
            Next
        End Function

        Public Shared Function DeviceFriendlyName(ByVal screen As Screen) As String
            Dim allFriendlyNames = GetAllMonitorsFriendlyNames()

            For index = 0 To Screen.AllScreens.Length - 1
                If Equals(screen, Screen.AllScreens(index)) Then Return allFriendlyNames.ToArray()(index)
            Next

            Return Nothing
        End Function

    End Class
End Namespace