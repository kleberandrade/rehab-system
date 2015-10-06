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
		* M�todo de serializa��o da mensagen em bin�rio
		* @param encode - buffer utilizado para codificar a mensagem em bin�rio
		*/
		void Serialize(BufferEncode &encode);

	protected:
		/**
		* M�todo virtual de serializa��o da mensagen em bin�rio
		* @param encode - buffer utilizado para serializa��o a mensagem em bin�rio
		*/
		virtual void OnSerialize(BufferEncode &encode) = 0;
	};

	/*******************************************************************
	*   IMPLEMENTA��O DA CLASSE
	*******************************************************************/

	inline void NetworkDispatcherData::Serialize(BufferEncode &encode)
	{
		encode.Clear();
		OnSerialize(encode);
	}
}