#pragma once

/**
* @file  Buffer.h
* @brief Classe que representa um buffer, criando métodos para pegar e configurar um buffer
*
* @copyright RehabCore::Net 2011-2015, EESC-USP.
*
*/

#include <string.h>

/*******************************************************************
*   DEFINES
*******************************************************************/

/** @brief Tamanho do buffer */
#define BUFFER_SIZE		64

namespace Net
{
	/*******************************************************************
	*   ESTRUTURA DA CLASSE
	*******************************************************************/

	/**
	*	@brief  Classe que representa um buffer, criando métodos para pegar e configurar um buffer
	*/
	class Buffer
	{
	public:
		/**
		* Construtor da classe
		*/
		Buffer(void);

		/**
		* Construtor da classe
		* @param buffer - Conteúdo para o buffer
		*/
		Buffer(char buffer[]);

		/**
		* Limpa o buffer
		*/
		void Clear(void);

		/**
		* Pega o buffer atual
		* @return m_strBuffer - retorna um buffer constante
		*/
		inline const char *GetBuffer(void)
		{
			return m_strBuffer;
		}

		/**
		* Modifica o buffer atual
		*/
		inline void SetBuffer(char buffer[])
		{
			memcpy(m_strBuffer, buffer, BUFFER_SIZE);
		}

		/**
		* Tamanho do buffer atual
		* @return m_iBufferSize - tamanho do buffer atual
		*/
		inline int GetSize(void)
		{
			return BUFFER_SIZE;
		}

	protected:
		char m_strBuffer[BUFFER_SIZE];			/**< m_strBuffer - representar um buffer de tamanho estático */
	};

	/*******************************************************************
	*   IMPLEMENTAÇÃO DA CLASSE
	*******************************************************************/

	inline Buffer::Buffer()
	{
		Clear();
	}

	inline Buffer::Buffer(char buffer[])
	{
		SetBuffer(buffer);
	}

	inline void Buffer::Clear(void)
	{
		memset(m_strBuffer, 0, BUFFER_SIZE);
	}
}