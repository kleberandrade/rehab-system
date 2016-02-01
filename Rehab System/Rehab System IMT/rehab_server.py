#!/usr/bin/python

from socket import *
import struct
import asyncio
import time

BUFFER_SIZE = 256

class AsyncServer:
  clientsList = []
  clientAddressesList = []

  def AcceptCallback( self ):
    while self.isListening:
      ( data, address ) = self.workSocket.recvfrom( BUFFER_SIZE, MSG_PEEK )
      if address not in self.clientAddressesList:
        print( 'New client from ' + str(address) )
        self.clientAddressesList.append( address )
        newClient = AsyncClient( address, self.workSocket )
        self.clientsList.append( newClient )
      time.sleep( 0.02 )

  def __init__( self, port ):
    self.workSocket = socket( type=SOCK_DGRAM )
    self.workSocket.setsockopt( SOL_SOCKET, SO_REUSEADDR, 1 )
    self.workSocket.bind( ( '', port ) )
    self.isListening = True
    self.updateLoop = asyncio.new_event_loop()
    self.updateLoop.run_in_executor( None, self.AcceptCallback )

  def AcceptClient( self ):
    if len(self.clientsList) == 0: return None

    return self.clientsList.pop()

  def StopListening( self ):
    self.isListening = False
    self.updateLoop.stop()
    self.updateLoop.close()
    self.workSocket.close()


class AsyncClient:
  messageQueue = []

  def ReadCallback( self ):
    while self.isReading:
      print( 'Reading' )
      address = self.workSocket.recvfrom( BUFFER_SIZE, MSG_PEEK )[ 1 ]
      if address == self.address:
        print( 'New message from ' + str(address) )
        self.messageQueue.append( self.workSocket.recvfrom( BUFFER_SIZE )[ 0 ] )
      time.sleep( 0.02 )

  def __init__( self, remoteAddress, sock=None ):
    self.address = remoteAddress
    self.workSocket = sock
    if sock is None: self.workSocket = socket( type=SOCK_DGRAM )
    self.isReading = True
    self.updateLoop = asyncio.new_event_loop()
    self.updateLoop.run_in_executor( None, self.ReadCallback )

  def ReceiveData( self ):
    if len(self.messageQueue) == 0: return None

    return self.messageQueue.pop()

  def SendData( self, data ):
    self.workSocket.sendto( data, self.address )

  def Disconnect( self ):
    self.isReading = False
    self.updateLoop.stop()
    self.updateLoop.close()



messageBuffer = bytearray( BUFFER_SIZE )

clientMessagesList = {}

server = AsyncServer( 50004 )

while True:

  try:
    newClient = server.AcceptClient()
    if newClient is not None: clientMessagesList[ newClient ] = None

    for client in clientMessagesList:
      data = client.ReceiveData()
      if data is not None:
        print( '{0} bytes: ({1},{2}) position: {3}\r'.format( data[0], data[1], data[2], struct.unpack( 'f', data[3:7] ) ) )
        clientMessagesList[ client ] = data

    for sendClient in clientMessagesList:
      messageLength = 1
      for client in clientMessagesList:
        if client is not sendClient and clientMessagesList[ client ] is not None:
          data = clientMessagesList[ client ]
          dataLength = data[ 0 ]
          messageBuffer[ messageLength:messageLength + dataLength - 1 ] = data[ 1:dataLength ]
          messageLength += dataLength
      messageBuffer[ 0 ] = messageLength
      sendClient.SendData( messageBuffer )

  except ( KeyboardInterrupt, SystemExit ):
    break

  time.sleep( 0.01 )

asyncio.get_event_loop().close()
server.StopListening()
