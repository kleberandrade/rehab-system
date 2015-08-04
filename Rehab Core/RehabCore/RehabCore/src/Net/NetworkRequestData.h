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
		* Método de deserialização da mensagen em binário
		* @param decode - buffer utilizado para decodificar a mensagem em binário
		*/
		void Deserialize(BufferDecode &decode);

	protected:
		/**
		* Método virtual de deserialização da mensagen em binário
		* @param decode - buffer utilizado para decodificar a mensagem em binário
		*/
		virtual void OnDeserialize(BufferDecode &decode) = 0;
	};


	/*******************************************************************
	*   IMPLEMENTAÇÃO DA CLASSE
	*******************************************************************/

	inline void NetworkRequestData::Deserialize(BufferDecode &decode)
	{
		OnDeserialize(decode);
		decode.Clear();
	}
}