/**
*	The MIT License (MIT)
*
*	Copyright (c) 2011-2014 DreanNet, EESC-USP.
*
*	Permission is hereby granted, free of charge, to any person obtaining a copy
*	of this software and associated documentation files (the "Software"), to deal
*	in the Software without restriction, including without limitation the rights
*	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*	copies of the Software, and to permit persons to whom the Software is
*	furnished to do so, subject to the following conditions:*
*
*	The above copyright notice and this permission notice shall be included in
*	all copies or substantial portions of the Software.
*
*	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
*	THE SOFTWARE.
*/

#include "ClientNetwork.h"

ClientNetwork::ClientNetwork()
	: m_tcpClient(SERVER_ADDRESS, SERVER_PORT, 0, 1)
{

}


ClientNetwork::ClientNetwork(const char *host, int address)
	: m_tcpClient(host, address, 0, 1)
{

}

ClientNetwork::~ClientNetwork(void)
{
	m_tcpClient.Close();
	m_tcpClient.ShutdownSocket();
	delete m_dispatcherData;
	delete m_requestData;
}

bool ClientNetwork::Open(void)
{
	m_tcpClient.InitializeSockets();
	m_tcpClient.Open();
	return true;
}

bool ClientNetwork::IsOpen(void)
{
	return m_tcpClient.IsOpen();
}

void ClientNetwork::Close(void)
{
	m_tcpClient.Close();
}

void ClientNetwork::Send(void)
{
	m_dispatcherData->Serialize(m_Encode);
	char buffer[BUFFER_SIZE];
	memcpy(&buffer, m_Encode.GetBuffer(), BUFFER_SIZE);
	m_tcpClient.Send(buffer, BUFFER_SIZE);
}

void ClientNetwork::Receive(void)
{
	char buffer[BUFFER_SIZE];
	m_tcpClient.Receiver(buffer, BUFFER_SIZE);
	m_Decode.SetBuffer(buffer);
	m_requestData->Deserialize(m_Decode);
}