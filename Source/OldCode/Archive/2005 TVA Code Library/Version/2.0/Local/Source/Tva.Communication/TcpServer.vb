'*******************************************************************************************************
'  Tva.Data.Transport.TcpServer.vb - Server for transporting data using TCP
'  Copyright � 2006 - TVA, all rights reserved - Gbtc
'
'  Build Environment: VB.NET, Visual Studio 2005
'  Primary Developer: Pinal C. Patel, Operations Data Architecture [TVA]
'      Office: COO - TRNS/PWR ELEC SYS O, CHATTANOOGA, TN - MR 2W-C
'       Phone: 423/751-2250
'       Email: pcpatel@tva.gov
'
'  Code Modification History:
'  -----------------------------------------------------------------------------------------------------
'  06/02/2006 - Pinal C. Patel
'       Original version of source code generated
'
'*******************************************************************************************************

Option Strict On

Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.ComponentModel
Imports Tva.Common
Imports Tva.IO.Common
Imports Tva.Serialization
Imports Tva.Threading
Imports Tva.Communication.SocketHelper
Imports Tva.Security.Cryptography.Common

''' <summary>
''' Represents a server involved in the transportation of data over the network using TCP.
''' </summary>
Public Class TcpServer

    Private m_packetAware As Boolean
    Private m_tcpServer As Socket
    Private m_tcpClients As Dictionary(Of Guid, SocketState)
    Private m_pendingTcpClients As List(Of SocketState)
    Private m_configurationStringData As Dictionary(Of String, String)

    ''' <summary>
    ''' Initializes a instance of Tva.Data.Transport.TcpServer with the specified data.
    ''' </summary>
    ''' <param name="configurationString">The data that is required by the server to initialize.</param>
    Public Sub New(ByVal configurationString As String)
        MyClass.New()
        MyBase.ConfigurationString = configurationString    ' Override the default configuration string value.
    End Sub

    ''' <summary>
    ''' Gets or sets a boolean value indicating whether the server will send the size of the packet before 
    ''' sending the actual packet.
    ''' </summary>
    ''' <value></value>
    ''' <returns>
    ''' True if the server will send the size of the packet before sending the actual packet; otherwise False.
    ''' </returns>
    ''' <remarks>This property must be set to True is wither Encryption or Compression is enabled.</remarks>
    <Description("Indicates whether the server will send the size of the packet before sending the actual packet."), Category("Data"), DefaultValue(GetType(Boolean), "True")> _
    Public Property PacketAware() As Boolean
        Get
            Return m_packetAware
        End Get
        Set(ByVal value As Boolean)
            m_packetAware = value
        End Set
    End Property

    ''' <summary>
    ''' Starts the server.
    ''' </summary>
    Public Overrides Sub Start()

        If Enabled() AndAlso Not IsRunning() AndAlso ValidConfigurationString(ConfigurationString()) Then
            ' Start the thread that will be listening for client connections.
            Dim listenerThread As New Thread(AddressOf ListenForConnections)
            listenerThread.Start()
        End If

    End Sub

    ''' <summary>
    ''' Stops the server.
    ''' </summary>
    Public Overrides Sub [Stop]()

        If Enabled() AndAlso IsRunning() Then
            ' NOTE: Closing the socket for server and all of the connected clients will cause a SocketException
            ' in the thread that is using the socket and result in the thread to exit gracefully.

            ' *** Stop accepting incoming connections ***
            If m_tcpServer IsNot Nothing Then m_tcpServer.Close()
            ' *** Diconnect all of the connected clients ***
            SyncLock m_tcpClients
                For Each tcpClient As SocketState In m_tcpClients.Values()
                    If tcpClient IsNot Nothing AndAlso tcpClient.Socket() IsNot Nothing Then
                        tcpClient.Socket.Close()
                    End If
                Next
            End SyncLock
            SyncLock m_pendingTcpClients
                For Each pendingTcpClient As SocketState In m_pendingTcpClients
                    If pendingTcpClient IsNot Nothing AndAlso pendingTcpClient.Socket() IsNot Nothing Then
                        pendingTcpClient.Socket.Close()
                    End If
                Next
            End SyncLock
        End If

    End Sub

    ''' <summary>
    ''' Sends prepared data to the specified client.
    ''' </summary>
    ''' <param name="clientID">ID of the client to which the data is to be sent.</param>
    ''' <param name="data">The prepared data that is to be sent to the client.</param>
    Protected Overrides Sub SendPreparedDataTo(ByVal clientID As Guid, ByVal data As Byte())

        If Enabled() AndAlso IsRunning() Then
            If data IsNot Nothing AndAlso data.Length() > 0 Then
                ' We don't want to synclock 'm_tcpClients' over here because doing so will block all
                ' all incoming connections (in ListenForConnections) while sending data to client(s). 
                Dim tcpClient As SocketState = Nothing
                If m_tcpClients.TryGetValue(clientID, tcpClient) Then
                    If SecureSession() Then data = EncryptData(data, tcpClient.Passphrase(), Encryption())
                    ' We'll send data over the wire asynchronously for improved performance.
                    If m_packetAware Then
                        Dim packetHeader As Byte() = BitConverter.GetBytes(data.Length())
                        tcpClient.Socket.BeginSend(packetHeader, 0, packetHeader.Length(), SocketFlags.None, Nothing, Nothing)
                    End If
                    tcpClient.Socket.BeginSend(data, 0, data.Length(), SocketFlags.None, Nothing, Nothing)
                Else
                    Throw New ArgumentException("Client ID '" & clientID.ToString() & "' is invalid.")
                End If
            Else
                Throw New ArgumentNullException("data")
            End If
        End If

    End Sub

    ''' <summary>
    ''' Determines whether specified configuration string, required for the server to initialize, is valid.
    ''' </summary>
    ''' <param name="configurationString">The configuration string to be validated.</param>
    ''' <returns>True if the configuration string is valid.</returns>
    Protected Overrides Function ValidConfigurationString(ByVal configurationString As String) As Boolean

        If Not String.IsNullOrEmpty(configurationString) Then
            m_configurationStringData = Tva.Text.Common.ParseKeyValuePairs(configurationString)
            If m_configurationStringData.ContainsKey("port") AndAlso _
                    ValidPortNumber(Convert.ToString(m_configurationStringData("port"))) Then
                Return True
            Else
                ' Configuration string is not in the expected format.
                With New StringBuilder()
                    .Append("Configuration string must be in the following format:")
                    .Append(Environment.NewLine())
                    .Append("   Port=<Port Number>")
                    Throw New ArgumentException(.ToString())
                End With
            End If
        Else
            Throw New ArgumentNullException()
        End If

    End Function

    ''' <summary>
    ''' Size of the packet that will contain the size of the acutal packet.
    ''' </summary>
    Private PacketHeaderSize As Integer = 4

    ''' <summary>
    ''' Listens for incoming client connections.
    ''' </summary>
    ''' <remarks>This method is meant to be executed on a seperate thread.</remarks>
    Private Sub ListenForConnections()

        Try
            ' Create a socket for the server.
            m_tcpServer = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            ' Bind the server socket to a local endpoint.
            m_tcpServer.Bind(New IPEndPoint(IPAddress.Any, Convert.ToInt32(m_configurationStringData("port"))))
            ' Start listening for connections and keep a maximum of 0 pending connection in the queue.
            m_tcpServer.Listen(0)
            OnServerStarted(EventArgs.Empty) ' Notify that the server has started.

            Do While True
                If MaximumClients() = -1 OrElse ClientIDs.Count() < MaximumClients() Then
                    ' We can accept incoming client connection requests.
                    Dim tcpClient As New SocketState()
                    tcpClient.Socket = m_tcpServer.Accept()  ' Accept client connection.
                    tcpClient.Socket.LingerState = New LingerOption(True, 10)

                    ' Start the client on a seperate thread so all the connected clients run independently.
                    RunThread.ExecuteNonPublicMethod(Me, "ReceiveClientData", tcpClient)
                    Thread.Sleep(1000)   ' Wait enough for the client thread to kick-off.
                End If
            Loop
        Catch ex As Exception
            ' We will gracefully exit when an exception occurs.
        Finally
            If m_tcpServer IsNot Nothing Then
                m_tcpServer.Close()
                m_tcpServer = Nothing
            End If
            OnServerStopped(EventArgs.Empty) ' Notify that the server has stopped.
        End Try

    End Sub

    ''' <summary>
    ''' Receives any data sent by a client that is connected to the server.
    ''' </summary>
    ''' <param name="tcpClient">Tva.Data.Transport.SocketState of the the connected client.</param>
    ''' <remarks>This method is meant to be executed on seperate threads.</remarks>
    Private Sub ReceiveClientData(ByVal tcpClient As SocketState)

        Try
            If Handshake() Then
                ' Handshaking is to be performed to authenticate the client.
                tcpClient.Socket.ReceiveTimeout = 5000
                SyncLock m_pendingTcpClients
                    m_pendingTcpClients.Add(tcpClient)
                End SyncLock
            Else
                ' No handshaking is to be performed for authenicating the client.
                tcpClient.SocketID = Guid.NewGuid()
                SyncLock m_tcpClients
                    m_tcpClients.Add(tcpClient.SocketID(), tcpClient)
                End SyncLock

                OnClientConnected(tcpClient.SocketID())    ' Notify that the client is connected.
            End If

            Do While True   ' Wait for data from the client.
                If tcpClient.DataBuffer() Is Nothing Then
                    Dim bufferSize As Integer = PacketHeaderSize
                    If Not m_packetAware Then bufferSize = ReceiveBufferSize()
                    tcpClient.DataBuffer = CreateArray(Of Byte)(bufferSize)
                End If

                Dim dataLength As Integer = tcpClient.Socket.Receive(tcpClient.DataBuffer(), _
                    tcpClient.BytesReceived(), tcpClient.DataBuffer.Length(), SocketFlags.None)
                tcpClient.BytesReceived += dataLength

                If dataLength > 0 Then
                    If m_packetAware Then
                        If tcpClient.PacketSize() = -1 AndAlso tcpClient.BytesReceived() = PacketHeaderSize Then
                            ' Size of the packet has been received.
                            tcpClient.PacketSize = BitConverter.ToInt32(tcpClient.DataBuffer(), 0)
                            If tcpClient.PacketSize() <= MaximumDataSize Then
                                tcpClient.DataBuffer = CreateArray(Of Byte)(tcpClient.PacketSize())
                                Continue Do
                            Else
                                Exit Do ' Packet size is not valid.
                            End If
                        ElseIf tcpClient.PacketSize() = -1 AndAlso tcpClient.BytesReceived() < PacketHeaderSize Then
                            ' Size of the packet is yet to be received.
                            Continue Do
                        ElseIf tcpClient.BytesReceived() < tcpClient.DataBuffer.Length() Then
                            ' We have not yet received the entire packet.
                            Continue Do
                        End If
                    Else
                        tcpClient.DataBuffer = CopyBuffer(tcpClient.DataBuffer(), 0, tcpClient.BytesReceived())
                    End If

                    If tcpClient.SocketID() = Guid.Empty AndAlso Handshake() Then
                        ' Authentication is required, but not performed yet. When authentication is required
                        ' the first message from the client must be information about itself.
                        Dim clientInfo As HandshakeMessage = _
                            DirectCast(GetObject(GetActualData(tcpClient.DataBuffer())), HandshakeMessage)
                        If clientInfo IsNot Nothing AndAlso clientInfo.ID() <> Guid.Empty AndAlso _
                                clientInfo.Passphrase() = HandshakePassphrase() Then
                            If SecureSession() Then tcpClient.Passphrase = GenerateKey()
                            Dim myInfo As Byte() = GetPreparedData(GetBytes(New HandshakeMessage(ServerID(), tcpClient.Passphrase())))
                            If m_packetAware Then tcpClient.Socket.Send(BitConverter.GetBytes(myInfo.Length()))
                            tcpClient.Socket.ReceiveTimeout = 0
                            tcpClient.Socket.Send(myInfo)   ' Send server info to the client.
                            tcpClient.SocketID = clientInfo.ID()

                            SyncLock m_pendingTcpClients
                                m_pendingTcpClients.Remove(tcpClient)
                            End SyncLock
                            SyncLock m_tcpClients
                                m_tcpClients.Add(tcpClient.SocketID(), tcpClient)
                            End SyncLock
                            OnClientConnected(tcpClient.SocketID())    ' Notify that the client is connected.
                        Else
                            ' The first response from the client is either not information about itself, or
                            ' the information provided by the client is invalid.
                            Exit Do
                        End If
                    Else
                        If SecureSession() Then
                            tcpClient.DataBuffer = DecryptData(tcpClient.DataBuffer(), tcpClient.Passphrase(), Encryption())
                        End If
                        ' Notify of data received from the client.
                        OnReceivedClientData(tcpClient.SocketID(), tcpClient.DataBuffer())
                    End If
                    tcpClient.PacketSize = -1
                    tcpClient.DataBuffer = Nothing
                Else
                    ' Connection is forcibly closed by the client.
                    Exit Do
                End If
            Loop
        Catch ex As Exception
            ' We will exit gracefully in case of any exception.
        Finally
            ' We are now done with the client.
            If tcpClient IsNot Nothing AndAlso tcpClient.Socket() IsNot Nothing Then
                tcpClient.Socket.Close()
                tcpClient.Socket = Nothing
            End If
            SyncLock m_pendingTcpClients
                m_pendingTcpClients.Remove(tcpClient)
            End SyncLock
            SyncLock m_tcpClients
                If m_tcpClients.ContainsKey(tcpClient.SocketID()) Then
                    m_tcpClients.Remove(tcpClient.SocketID())
                    OnClientDisconnected(tcpClient.SocketID())    ' Notify that the client is disconnected.
                End If
            End SyncLock
        End Try

    End Sub

End Class