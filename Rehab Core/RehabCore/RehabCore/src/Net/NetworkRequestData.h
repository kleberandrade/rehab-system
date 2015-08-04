#pragma once

/**
* @file  NetworkRequestData.h
* @brief Classe abstrata que deserializa o buffer da rede
*
* @copyright DreanNet 2011-2014, EESC-USP.
*
*/

#include "BufferDecode.h"

namespace Net
	{
	/*******************************************************************
	*   ESTRUTURA DA CLASSE
	*******************************************************************/

	/**
	* @brief Classe abstrata que deserializa o buffer da rede
	*/
	class NetworkRequestData
	{
	public:
		/**
		* M�todo de deserializa��o da mensagen em bin�rio
		* @param decode - buffer utilizado para decodificar a mensagem em bin�rio
		*/
		void Deserialize(BufferDecode &decode);

	protected:
		/**
		* M�todo virtual de deserializa��o da mensagen em bin�rio
		* @param decode - buffer utilizado para decodificar a mensagem em bin�rio
		*/
		virtual void OnDeserialize(BufferDecode &decode) = 0;
	};


	/*******************************************************************
	*   IMPLEMENTA��O DA CLASSE
	*******************************************************************/

	inline void NetworkRequestData::Deserialize(BufferDecode &decode)
	{
		OnDeserialize(decode);
		decode.Clear();
	}
}