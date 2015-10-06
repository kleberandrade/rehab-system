#pragma once

/**
* @file  ClientNetwork.h
* @brief Cliente TCP para conectar com um servidor e receber informações do jogo
*
* @copyright DreanNet 2011-2014, EESC-USP.
*
*/

#include "TCPClient.h"
#include "NetworkDispatcherData.h"
#include "NetworkRequestData.h"

/*******************************************************************
*   DEFINES
*******************************************************************/

/** @brief define que o endianness é do tipo big endian*/
// #define __BIG_ENDIAN

/** @brief define que o endianness é do tipo little endian*/
#define __LITTLE_ENDIAN

//#ifdef __WCE_SECTION
#pragma comment(lib,"ws2.lib")
//#else
//#pragma comment (lib, "Ws2_32.lib")
//#endif

namespace Net
	{
	/*******************************************************************
	*   ESTRUTURA DA CLASSE
	*******************************************************************/

	/**
	* @brief Cliente TCP para conectar com um servidor e receber informações do jogo
	*/
	class ClientNetwork
	{
	public:

		/**
		* Construtor da classe
		* @param host - ip do servidor
		* @param port - porta do servidor
		* @param dispatcherData - tipo de objeto de dados a ser enviado para o servidor
		* @param requestData - tipo de objeto de dados a ser recebido do servidor
		*/
		ClientNetwork(const char *host, int port, NetworkDispatcherData *dispatcherData, NetworkRequestData *requestData);

		/**
		* Destrutor da classe
		*/
		~ClientNetwork(void);

		/**
		* Abre a conecão com o servidor
		* @return se a conexão for estabelecida (true), caso contrário (false)
		*/
		bool Open(void);

		/**
		* Verifica se a conexão esta aberta
		* @return se a conexão estiver estabelecida (true), caso contrário (false)
		*/
		bool IsOpen(void);

		/**
		* Encerra a conexão com o servidor
		*/
		void Close(void);

		/**
		* Envia o buffer para o servidor
		*/
		void Send(void);

		/**
		* Recebe o buffer do servidor
		*/
		void Receive(void);

		/**
		* Pega o objeto de envio para o servidor
		* @return m_dispatcherData
		*/
		inline NetworkDispatcherData *GetDispatcherData(void)
		{
			return m_dispatcherData;
		}

		/**
		* Pega o objeto de recebimento do servidor
		* @return m_requestData
		*/
		inline NetworkRequestData *GetRequestData(void)
		{
			return m_requestData;
		}

		/**
		* Configura o objeto de envio de dados
		* @param dispatcherData - tipo de objeto de dados a ser enviado para o servidor
		*/
		inline void SetDispatcher(NetworkDispatcherData *dispatcherData)
		{
			m_dispatcherData = dispatcherData;
		}

		/**
		* Configura o objeto de recebimento de dados
		* @param requestData - tipo de objeto de dados a ser recebido do servidor
		*/
		inline void SetRequest(NetworkRequestData *requestData)
		{
			m_requestData = requestData;
		}

	private:
		TCPClient m_tcpClient;						/**< m_tcpClient - cliente TCP */
		BufferDecode m_Decode;						/**< m_Decode - decodificador de buffer */
		BufferEncode m_Encode;						/**< m_Encode - codificador de buffer */
		NetworkRequestData *m_requestData;			/**< m_requestData - ponteiro para mensagen de recebida do servidor */
		NetworkDispatcherData *m_dispatcherData;	/**< m_dispatcherData - ponteiro para mensagem a ser enviada para o servidor */
	};	

	/*******************************************************************
	*   IMPLEMENTAÇÃO DA CLASSE
	*******************************************************************/

	inline ClientNetwork::ClientNetwork(const char *host, int port, NetworkDispatcherData *dispatcherData = NULL, NetworkRequestData *requestData = NULL)
		: m_tcpClient(host, port, 0),
	      m_dispatcherData(dispatcherData),
		  m_requestData(requestData)
	{

	}

	inline ClientNetwork::~ClientNetwork(void)
	{
		m_tcpClient.Close();
		m_tcpClient.ShutdownSocket();
		delete m_dispatcherData;
		delete m_requestData;
	}

	inline bool ClientNetwork::Open(void)
	{
		m_tcpClient.InitializeSockets();
		m_tcpClient.Open();
		return true;
	}

	inline bool ClientNetwork::IsOpen(void)
	{
		return m_tcpClient.IsOpen();
	}

	inline void ClientNetwork::Close(void)
	{
		m_tcpClient.Close();
	}

	inline void ClientNetwork::Send(void)
	{
		m_dispatcherData->Serialize(m_Encode);
		char buffer[BUFFER_SIZE];
		memcpy(&buffer, m_Encode.GetBuffer(), BUFFER_SIZE);
		m_tcpClient.Send(buffer, BUFFER_SIZE);
	}

	inline void ClientNetwork::Receive(void)
	{
		char buffer[BUFFER_SIZE];
		m_tcpClient.Receiver(buffer, BUFFER_SIZE);
		m_Decode.SetBuffer(buffer);
		m_requestData->Deserialize(m_Decode);
	}
}