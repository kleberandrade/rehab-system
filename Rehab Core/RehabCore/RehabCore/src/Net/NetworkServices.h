#pragma once

/**
* @file  NetworkServices.h
* @brief Wrapper para as funções de send e recv do SOCKET
*
* @copyright DreanNet 2011-2014, EESC-USP.
*
*/

#include <winsock2.h>

namespace Net
	{
	/*******************************************************************
	*   ESTRUTURA DA CLASSE
	*******************************************************************/

	/**
	* @brief Wrapper para as funções de send e recv do SOCKET
	*/
	class NetworkServices
	{
	public:
		
		/**
		*	@brief	Função send encapsulada
		*	@param	socket - socket para comunicação
		*	@param	message - mensagem a ser enviada
		*	@param	messageSize - tamanho da mensagem a ser enviada
		*	@return informações do envio da mensagem
		*/
		static int SendMessage(SOCKET socket, char *message, int messageSize);

		/**
		*	@brief	Função recv encapsulada
		*	@param	socket - socket para comunicação 
		*	@param	buffer - buffer recebido
		*	@param	bufferSize - tamaho do buffer recebido
		*	@return informações do buffer recebido
		*/
		static int ReceiveMessage(SOCKET socket, char *buffer, int messageSize);
	};


	/*******************************************************************
	*   IMPLEMENTAÇÃO DA CLASSE
	*******************************************************************/

	inline int NetworkServices::SendMessage(SOCKET socket, char *message, int messageSize)
	{
		return send(socket, message, messageSize, 0);
	}

	inline int NetworkServices::ReceiveMessage(SOCKET socket, char *buffer, int bufferSize)
	{
		return recv(socket, buffer, bufferSize, 0);
	}
}