#pragma once

/**
* @file  NetworkDispatcherData.h
* @brief Classe abstrata que serializa o buffer da rede
*
* @copyright DreanNet 2011-2014, EESC-USP.
*
*/

#include "BufferEncode.h"

namespace Net
	{
	/*******************************************************************
	*   ESTRUTURA DA CLASSE
	*******************************************************************/

	/**
	* @brief Classe abstrata que serializa o buffer para rede
	*/
	class NetworkDispatcherData
	{
	public:
		/**
		* Método de serialização da mensagen em binário
		* @param encode - buffer utilizado para codificar a mensagem em binário
		*/
		void Serialize(BufferEncode &encode);

	protected:
		/**
		* Método virtual de serialização da mensagen em binário
		* @param encode - buffer utilizado para serialização a mensagem em binário
		*/
		virtual void OnSerialize(BufferEncode &encode) = 0;
	};

	/*******************************************************************
	*   IMPLEMENTAÇÃO DA CLASSE
	*******************************************************************/

	inline void NetworkDispatcherData::Serialize(BufferEncode &encode)
	{
		encode.Clear();
		OnSerialize(encode);
	}
}