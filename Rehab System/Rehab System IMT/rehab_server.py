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

    return self.clientsList.pop( 0 )

  def StopListening( self ):
    self.isListening = False
    self.updateLoop.stop()
    self.updateLoop.close()
    for client in clientAddressesList:
      client.Disconnect()
    self.workSocket.close()


class AsyncClient:

  def ReadCallback( self ):
    while self.isReading:
      address = self.workSocket.recvfrom( BUFFER_SIZE, MSG_PEEK )[ 1 ]
      if address == self.address:
        self.message = self.workSocket.recvfrom( BUFFER_SIZE )[ 0 ]
      time.sleep( 0.02 )

  def __init__( self, remoteAddress, sock=None ):
    self.address = remoteAddress
    self.message = b''
    self.workSocket = sock
    if sock is None: self.workSocket = socket( type=SOCK_DGRAM )
    self.isReading = True
    self.updateLoop = asyncio.new_event_loop()
    self.updateLoop.run_in_executor( None, self.ReadCallback )

  def ReceiveData( self ):
    lastMessage = self.message
    self.message = None
    return lastMessage

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
        dataLength = data[ 0 ]
#        print( 'Received {} from {}'.format( data[ 0:dataLength ], client.address ) )
        for id in range( 1, dataLength, 14 ):
          print( 'Received <({},{}): {},{}> from {}'.format( data[ id ], data[ id + 1 ], struct.unpack( 'f', data[ id + 2:id + 6 ] )[ 0 ], struct.unpack( 'f', data[ id + 6:id + 10 ] )[ 0 ], str(client.address) ) )
        clientMessagesList[ client ] = data

    for sendClient in clientMessagesList:
      messageLength = 1
      messageBuffer = bytearray( BUFFER_SIZE )
      for client in clientMessagesList:
#        print( 'comparing {} to {}: {}'.format( sendClient.address[0], client.address[0], str(client.address[0] != sendClient.address[0]) ) )
        if client.address != sendClient.address and clientMessagesList[ client ] is not None:
          data = clientMessagesList[ client ]
          if len(data) > 0:
            dataLength = data[ 0 ]
            messageBuffer[ messageLength:messageLength + dataLength - 1 ] = data[ 1:dataLength ]
            messageLength += ( dataLength - 1 )
  #          for id in range( 0, dataLength, 7 ):
  #            print( 'Sending <({},{}): {}> from {} to {}'.format( data[ id + 1 ], data[ id + 2 ],
  #                                                                 struct.unpack( 'f', data[ id + 3:id + 7 ] )[ 0 ],
  #                                                                 str(client.address), str(sendClient.address) ) )
      if messageLength > 1:
        messageBuffer[ 0 ] = messageLength
        #print( 'Sending {} bytes to {}'.format( messageBuffer[0], sendClient.address ) )
        sendClient.SendData( messageBuffer )

  except ( KeyboardInterrupt, SystemExit ):
    break

  time.sleep( 0.01 )

#asyncio.get_event_loop().close()
server.StopListening()
exit()
