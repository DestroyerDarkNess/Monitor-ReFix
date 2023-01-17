' ***********************************************************************
' Author   : Elektro
' Created  : 01-09-2014
' Modified : 01-11-2014
' ***********************************************************************
' <copyright file="GlobalHotkeys.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Usage Examples "

'Public Class Form1

'    ''' <summary>
'    ''' Define the system-wide hotkey object.
'    ''' </summary>
'    Private WithEvents Hotkey As GlobalHotkey = Nothing

'    ''' <summary>
'    ''' Initializes a new instance of this class.
'    ''' </summary>
'    Public Sub New()

'        InitializeComponent()

'        ' Registers a new global hotkey on the system. (Alt + Ctrl + A) 
'        Hotkey = New GlobalHotkey(GlobalHotkey.KeyModifier.Alt Or GlobalHotkey.KeyModifier.Ctrl, Keys.A) 

'        ' Replaces the current registered hotkey with a new one. (Alt + Escape)
'        Hotkey = New GlobalHotkey([Enum].Parse(GetType(GlobalHotkey.KeyModifier), "Alt", True),
'                                  [Enum].Parse(GetType(Keys), "Escape", True))

'        ' Set the tag property.
'        Hotkey.Tag = "I'm an example tag"

'    End Sub

'    ''' <summary>
'    ''' Handles the Press event of the HotKey object.
'    ''' </summary>
'    Private Sub HotKey_Press(ByVal sender As GlobalHotkey, ByVal e As GlobalHotkey.HotKeyEventArgs) _
'    Handles Hotkey.Press

'        MsgBox(e.Count) ' The times that the hotkey was pressed.
'        MsgBox(e.ID) ' The unique hotkey identifier.
'        MsgBox(e.Key.ToString) ' The assigned key.
'        MsgBox(e.Modifier.ToString) ' The assigned key-modifier.

'        MsgBox(sender.Tag) ' The hotkey tag object.

'        ' Unregister the hotkey.
'        Hotkey.Unregister()

'        ' Register it again.
'        Hotkey.Register()

'        ' Is Registered?
'        MsgBox(Hotkey.IsRegistered)

'    End Sub

'End Class

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Runtime.InteropServices

#End Region

#Region " Global Hotkey "

''' <summary>
''' Class to perform system-wide hotkey operations.
''' </summary>
Friend NotInheritable Class GlobalHotkey : Inherits NativeWindow : Implements IDisposable

#Region " API "

    ''' <summary>
    ''' Native API Methods.
    ''' </summary>
    Private Class NativeMethods

        ''' <summary>
        ''' Defines a system-wide hotkey.
        ''' </summary>
        ''' <param name="hWnd">The hWND.</param>
        ''' <param name="id">The identifier of the hotkey.
        ''' If the hWnd parameter is NULL, then the hotkey is associated with the current thread rather than with a particular window.
        ''' If a hotkey already exists with the same hWnd and id parameters.</param>
        ''' <param name="fsModifiers">The keys that must be pressed in combination with the key specified by the uVirtKey parameter
        ''' in order to generate the WM_HOTKEY message.
        ''' The fsModifiers parameter can be a combination of the following values.</param>
        ''' <param name="vk">The virtual-key code of the hotkey.</param>
        ''' <returns>
        ''' <c>true</c> if the function succeeds, otherwise <c>false</c>
        ''' </returns>
        <DllImport("user32.dll", SetLastError:=True)>
        Public Shared Function RegisterHotKey(
                      ByVal hWnd As IntPtr,
                      ByVal id As Integer,
                      ByVal fsModifiers As UInteger,
                      ByVal vk As UInteger
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' <summary>
        ''' Unregisters a hotkey previously registered.
        ''' </summary>
        ''' <param name="hWnd">The hWND.</param>
        ''' <param name="id">The identifier of the hotkey to be unregistered.</param>
        ''' <returns>
        ''' <c>true</c> if the function succeeds, otherwise <c>false</c>
        ''' </returns>
        <DllImport("user32.dll", SetLastError:=True)>
        Public Shared Function UnregisterHotKey(
                      ByVal hWnd As IntPtr,
                      ByVal id As Integer
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

    End Class

#End Region

#Region " Members "

#Region " Properties "

    ''' <summary>
    ''' Indicates the key assigned to the hotkey.
    ''' </summary>
    Public ReadOnly Property Key As Keys
        Get
            Return Me.PressEventArgs.Key
        End Get
    End Property

    ''' <summary>
    ''' Indicates the Key-Modifier assigned to the hotkey.
    ''' </summary>
    Public ReadOnly Property Modifier As KeyModifier
        Get
            Return Me.PressEventArgs.Modifier
        End Get
    End Property

    ''' <summary>
    ''' Indicates the unique identifier assigned to the hotkey.
    ''' </summary>
    Public ReadOnly Property ID As Integer
        Get
            Return Me.PressEventArgs.ID
        End Get
    End Property

    ''' <summary>
    ''' Indicates user-defined data associated with this object.
    ''' </summary>
    Public Property Tag As Object = Nothing

    ''' <summary>
    ''' Indicates how many times was pressed the hotkey.
    ''' </summary>
    Public ReadOnly Property Count As Integer
        Get
            Return _Count
        End Get
    End Property

#End Region

#Region " Enumerations "

    ''' <summary>
    ''' Key-modifiers to assign to a hotkey.
    ''' </summary>
    <Flags>
    Public Enum KeyModifier As Integer

        ''' <summary>
        ''' Any modifier.
        ''' </summary>
        None = &H0

        ''' <summary>
        ''' The Alt key.
        ''' </summary>
        Alt = &H1

        ''' <summary>
        ''' The Control key.
        ''' </summary>
        Ctrl = &H2

        ''' <summary>
        ''' The Shift key.
        ''' </summary>
        Shift = &H4

        ''' <summary>
        ''' The Windows key.
        ''' </summary>
        Win = &H8

    End Enum

    ''' <summary>
    ''' Known Windows Message Identifiers.
    ''' </summary>
    <Description("Messages to process in WndProc")>
    Public Enum KnownMessages As Integer

        ''' <summary>
        ''' Posted when the user presses a hot key registered by the RegisterHotKey function. 
        ''' The message is placed at the top of the message queue associated with the thread that registered the hot key.
        ''' <paramref name="WParam"/>
        ''' The identifier of the hot key that generated the message.
        ''' If the message was generated by a system-defined hot key.
        ''' <paramref name="LParam"/>
        ''' The low-order word specifies the keys that were to be pressed in 
        ''' combination with the key specified by the high-order word to generate the WM_HOTKEY message.
        ''' </summary>
        WM_HOTKEY = &H312

    End Enum

#End Region

#Region " Events "

    ''' <summary>
    ''' Event that is raised when a hotkey is pressed.
    ''' </summary>
    Public Event Press As EventHandler(Of HotKeyEventArgs)

    ''' <summary>
    ''' Event arguments for the Press event.
    ''' </summary>
    Public Class HotKeyEventArgs : Inherits EventArgs

        ''' <summary>
        ''' Indicates the Key assigned to the hotkey.
        ''' </summary>
        ''' <value>The key.</value>
        Friend Property Key As Keys

        ''' <summary>
        ''' Indicates the Key-Modifier assigned to the hotkey.
        ''' </summary>
        ''' <value>The modifier.</value>
        Friend Property Modifier As KeyModifier

        ''' <summary>
        ''' Indicates the unique identifier assigned to the hotkey.
        ''' </summary>
        ''' <value>The identifier.</value>
        Friend Property ID As Integer

        ''' <summary>
        ''' Indicates how many times was pressed the hotkey.
        ''' </summary>
        Friend Property Count As Integer

    End Class

#End Region

#Region " Exceptions "

    ''' <summary>
    ''' Exception that is thrown when a hotkey tries to register but is already registered.
    ''' </summary>
    <Serializable>
    Private Class IsRegisteredException : Inherits Exception

        ''' <summary>
        ''' Initializes a new instance of the <see cref="IsRegisteredException"/> class.
        ''' </summary>
        Sub New()
            MyBase.New("Unable to register. Hotkey is already registered.")
        End Sub

    End Class

    ''' <summary>
    ''' Exception that is thrown when a hotkey tries to unregister but is not registered.
    ''' </summary>
    <Serializable>
    Private Class IsNotRegisteredException : Inherits Exception

        ''' <summary>
        ''' Initializes a new instance of the <see cref="IsNotRegisteredException"/> class.
        ''' </summary>
        Sub New()
            MyBase.New("Unable to unregister. Hotkey is not registered.")
        End Sub

    End Class

#End Region

#Region " Other "

    ''' <summary>
    ''' Stores an counter indicating how many times was pressed the hotkey.
    ''' </summary>
    Private _Count As Integer = 0

    ''' <summary>
    ''' Stores the Press Event Arguments.
    ''' </summary>
    Protected PressEventArgs As New HotKeyEventArgs

#End Region

#End Region

#Region " Constructor "

    ''' <summary>
    ''' Creates a new system-wide hotkey.
    ''' </summary>
    ''' <param name="Modifier">
    ''' Indicates the key-modifier to assign to the hotkey.
    ''' ( Can use one or more modifiers )
    ''' </param>
    ''' <param name="Key">
    ''' Indicates the key to assign to the hotkey.
    ''' </param>
    ''' <exception cref="IsRegisteredException"></exception>
    <DebuggerStepperBoundary()>
    Public Sub New(ByVal Modifier As KeyModifier, ByVal Key As Keys)

        MyBase.CreateHandle(New CreateParams)

        Me.PressEventArgs.ID = MyBase.GetHashCode()
        Me.PressEventArgs.Key = Key
        Me.PressEventArgs.Modifier = Modifier
        Me.PressEventArgs.Count = 0

        If Not NativeMethods.RegisterHotKey(MyBase.Handle,
                                            Me.ID,
                                            Me.Modifier,
                                            Me.Key) Then

            Throw New IsRegisteredException

        End If

    End Sub

#End Region

#Region " Event Handlers "

    ''' <summary>
    ''' Occurs when a hotkey is pressed.
    ''' </summary>
    Private Sub OnHotkeyPress() Handles Me.Press
        _Count += 1
    End Sub

#End Region

#Region "Public Methods "

    ''' <summary>
    ''' Determines whether this hotkey is registered on the system.
    ''' </summary>
    ''' <returns>
    ''' <c>true</c> if this hotkey is registered; otherwise, <c>false</c>.
    ''' </returns>
    Public Function IsRegistered() As Boolean

        DisposedCheck()

        ' Try to unregister the hotkey.
        Select Case NativeMethods.UnregisterHotKey(MyBase.Handle, Me.ID)

            Case False ' Unregistration failed.
                Return False ' Hotkey is not registered.

            Case Else ' Unregistration succeeds.
                Register() ' Re-Register the hotkey before return.
                Return True ' Hotkey is registeres.

        End Select

    End Function

    ''' <summary>
    ''' Registers this hotkey on the system.
    ''' </summary>
    ''' <exception cref="IsRegisteredException"></exception>
    Public Sub Register()

        DisposedCheck()

        If Not NativeMethods.RegisterHotKey(MyBase.Handle,
                                            Me.ID,
                                            Me.Modifier,
                                            Me.Key) Then

            Throw New IsRegisteredException

        End If

    End Sub

    ''' <summary>
    ''' Unregisters this hotkey from the system.
    ''' After calling this method the hotkey turns unavaliable.
    ''' </summary>
    ''' <returns>
    ''' <c>true</c> if unregistration succeeds, <c>false</c> otherwise.
    ''' </returns>
    Public Function Unregister() As Boolean

        DisposedCheck()

        If Not NativeMethods.UnregisterHotKey(MyBase.Handle, Me.ID) Then

            Throw New IsNotRegisteredException

        End If

    End Function

#End Region

#Region " Hidden methods "

    ' These methods and properties are purposely hidden from Intellisense just to look better without unneeded methods.
    ' NOTE: The methods can be re-enabled at any-time if needed.

    ''' <summary>
    ''' Assigns the handle.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Shadows Sub AssignHandle()
    End Sub

    ''' <summary>
    ''' Creates the handle.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Shadows Sub CreateHandle()
    End Sub

    ''' <summary>
    ''' Creates the object reference.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Shadows Sub CreateObjRef()
    End Sub

    ''' <summary>
    ''' Definitions the WND proc.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Shadows Sub DefWndProc()
    End Sub

    ''' <summary>
    ''' Destroys the window and its handle.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Shadows Sub DestroyHandle()
    End Sub

    ''' <summary>
    ''' Equalses this instance.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Shadows Sub Equals()
    End Sub

    ''' <summary>
    ''' Gets the hash code.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Shadows Sub GetHashCode()
    End Sub

    ''' <summary>
    ''' Gets the lifetime service.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Shadows Sub GetLifetimeService()
    End Sub

    ''' <summary>
    ''' Initializes the lifetime service.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Shadows Sub InitializeLifetimeService()
    End Sub

    ''' <summary>
    ''' Releases the handle associated with this window.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Shadows Sub ReleaseHandle()
    End Sub

    ''' <summary>
    ''' Gets the handle for this window.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Shadows Property Handle()

#End Region

#Region " WndProc "

    ''' <summary>
    ''' Invokes the default window procedure associated with this window to process messages for this Window.
    ''' </summary>
    ''' <param name="m">
    ''' A <see cref="T:System.Windows.Forms.Message" /> that is associated with the current Windows message.
    ''' </param>
    Protected Overrides Sub WndProc(ByRef m As Message)

        Select Case m.Msg

            Case KnownMessages.WM_HOTKEY  ' A hotkey is pressed.

                ' Update the pressed counter.
                Me.PressEventArgs.Count += 1

                ' Raise the Event
                RaiseEvent Press(Me, Me.PressEventArgs)

            Case Else
                MyBase.WndProc(m)

        End Select

    End Sub

#End Region

#Region " IDisposable "

    ''' <summary>
    ''' To detect redundant calls when disposing.
    ''' </summary>
    Private IsDisposed As Boolean = False

    ''' <summary>
    ''' Prevent calls to methods after disposing.
    ''' </summary>
    ''' <exception cref="System.ObjectDisposedException"></exception>
    Private Sub DisposedCheck()

        If Me.IsDisposed Then
            Throw New ObjectDisposedException(Me.GetType().FullName)
        End If

    End Sub

    ''' <summary>
    ''' Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    ''' <summary>
    ''' Releases unmanaged and - optionally - managed resources.
    ''' </summary>
    ''' <param name="IsDisposing">
    ''' <c>true</c> to release both managed and unmanaged resources; 
    ''' <c>false</c> to release only unmanaged resources.
    ''' </param>
    Protected Sub Dispose(IsDisposing As Boolean)

        If Not Me.IsDisposed Then

            If IsDisposing Then
                NativeMethods.UnregisterHotKey(MyBase.Handle, Me.ID)
            End If

        End If

        Me.IsDisposed = True

    End Sub

#End Region

End Class

#End Region