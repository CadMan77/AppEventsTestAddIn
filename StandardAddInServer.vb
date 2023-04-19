Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports Inventor
'Imports Microsoft.Win32

Namespace AppEventsTestAddIn
    <ProgIdAttribute("AppEventsTestAddIn.StandardAddInServer"),
    GuidAttribute("e426ed51-0435-49a6-82ba-e5963dfc8c0d")>
    Public Class StandardAddInServer
        Implements Inventor.ApplicationAddInServer

        'Application events object
        Private WithEvents oAppEvents As ApplicationEvents

        Private WithEvents m_uiEvents As UserInterfaceEvents
        'Private WithEvents m_sampleButton As ButtonDefinition

        Friend myForm As Form1
#Region "ApplicationAddInServer Members"
        Private Sub oAppEvents_OnActivateDocument(ByVal DocumentObject As Document, ByVal BeforeOrAfter As EventTimingEnum, ByVal Context As NameValueMap, ByRef HandlingCode As HandlingCodeEnum) Handles oAppEvents.OnActivateDocument

            If BeforeOrAfter = EventTimingEnum.kAfter Then

                'MessageBox.Show("AFTER!")

                myForm = New Form1
                myForm.Show(New WindowWrapper(g_inventorApplication.MainFrameHWND))

            ElseIf BeforeOrAfter = EventTimingEnum.kBefore Then

                'MessageBox.Show("BEFORE!")

                Try
                    myForm.Close()
                Catch ex As Exception

                End Try
            End If
        End Sub

        ' This method is called by Inventor when it loads the AddIn. The AddInSiteObject provides access  
        ' to the Inventor Application object. The FirstTime flag indicates if the AddIn is loaded for
        ' the first time. However, with the introduction of the ribbon this argument is always true.
        Public Sub Activate(ByVal addInSiteObject As Inventor.ApplicationAddInSite, ByVal firstTime As Boolean) Implements Inventor.ApplicationAddInServer.Activate
            ' Initialize AddIn members.
            g_inventorApplication = addInSiteObject.Application

            ' Connect to the user-interface events to handle a ribbon reset.
            m_uiEvents = g_inventorApplication.UserInterfaceManager.UserInterfaceEvents

            oAppEvents = g_inventorApplication.ApplicationEvents()

            ' TODO: Add button definitions.

            ' Sample to illustrate creating a button definition.
            'Dim largeIcon As stdole.IPictureDisp = PictureDispConverter.ToIPictureDisp(My.Resources.YourBigImage)
            'Dim smallIcon As stdole.IPictureDisp = PictureDispConverter.ToIPictureDisp(My.Resources.YourSmallImage)
            'Dim controlDefs As Inventor.ControlDefinitions = g_inventorApplication.CommandManager.ControlDefinitions
            'm_sampleButton = controlDefs.AddButtonDefinition("Command Name", "Internal Name", CommandTypesEnum.kShapeEditCmdType, AddInClientID)

            ' Add to the user interface, if it's the first time.
            If firstTime Then
                AddToUserInterface()
            End If
        End Sub

        ' This method is called by Inventor when the AddIn is unloaded. The AddIn will be
        ' unloaded either manually by the user or when the Inventor session is terminated.
        Public Sub Deactivate() Implements Inventor.ApplicationAddInServer.Deactivate

            ' TODO:  Add ApplicationAddInServer.Deactivate implementation

            ' Release objects.
            m_uiEvents = Nothing
            g_inventorApplication = Nothing

            System.GC.Collect()
            System.GC.WaitForPendingFinalizers()
        End Sub

        ' This property is provided to allow the AddIn to expose an API of its own to other 
        ' programs. Typically, this  would be done by implementing the AddIn's API
        ' interface in a class and returning that class object through this property.
        Public ReadOnly Property Automation() As Object Implements Inventor.ApplicationAddInServer.Automation
            Get
                Return Nothing
            End Get
        End Property

        ' Note:this method is now obsolete, you should use the 
        ' ControlDefinition functionality for implementing commands.
        Public Sub ExecuteCommand(ByVal commandID As Integer) Implements Inventor.ApplicationAddInServer.ExecuteCommand
        End Sub

#End Region

#Region "User interface definition"
        ' Sub where the user-interface creation is done.  This is called when
        ' the add-in loaded and also if the user interface is reset.
        Private Sub AddToUserInterface()
            ' This is where you'll add code to add buttons to the ribbon.

            '** Sample to illustrate creating a button on a new panel of the Tools tab of the Part ribbon.

            '' Get the part ribbon.
            'Dim partRibbon As Ribbon = g_inventorApplication.UserInterfaceManager.Ribbons.Item("Part")

            '' Get the "Tools" tab.
            'Dim toolsTab As RibbonTab = partRibbon.RibbonTabs.Item("id_TabTools")

            '' Create a new panel.
            'Dim customPanel As RibbonPanel = toolsTab.RibbonPanels.Add("Sample", "MysSample", AddInClientID)

            '' Add a button.
            'customPanel.CommandControls.AddButton(m_sampleButton)
        End Sub

        Private Sub m_uiEvents_OnResetRibbonInterface(Context As NameValueMap) Handles m_uiEvents.OnResetRibbonInterface
            ' The ribbon was reset, so add back the add-ins user-interface.
            AddToUserInterface()
        End Sub

        ' Sample handler for the button.
        'Private Sub m_sampleButton_OnExecute(Context As NameValueMap) Handles m_sampleButton.OnExecute
        '    MsgBox("Button was clicked.")
        'End Sub
#End Region

    End Class
End Namespace


Public Module Globals
    ' Inventor application object.
    Public g_inventorApplication As Inventor.Application

#Region "Function to get the add-in client ID."
    ' This function uses reflection to get the GuidAttribute associated with the add-in.
    Public Function AddInClientID() As String
        Dim guid As String = ""
        Try
            Dim t As Type = GetType(AppEventsTestAddIn.StandardAddInServer)
            Dim customAttributes() As Object = t.GetCustomAttributes(GetType(GuidAttribute), False)
            Dim guidAttribute As GuidAttribute = CType(customAttributes(0), GuidAttribute)
            guid = "{" + guidAttribute.Value.ToString() + "}"
        Catch
        End Try

        Return guid
    End Function
#End Region

#Region "hWnd Wrapper Class"
    ' This class is used to wrap a Win32 hWnd as a .Net IWind32Window class.
    ' This is primarily used for parenting a dialog to the Inventor window.
    '
    ' For example:
    ' myForm.Show(New WindowWrapper(g_inventorApplication.MainFrameHWND))
    '
    Public Class WindowWrapper
        Implements System.Windows.Forms.IWin32Window
        Public Sub New(ByVal handle As IntPtr)
            _hwnd = handle
        End Sub

        Public ReadOnly Property Handle() As IntPtr _
          Implements System.Windows.Forms.IWin32Window.Handle
            Get
                Return _hwnd
            End Get
        End Property

        Private _hwnd As IntPtr
    End Class
#End Region

End Module
