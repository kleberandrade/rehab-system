#pragma once

/**
* @file  BufferDecode.h
* @brief Classe auxiliar para decodificar um buffer binario
*
* @copyright RehabCore::Net 2011-2014, EESC-USP.
*
*/

#include "Buffer.h"
#include "Encoding.h"

namespace Net
	{
	/*******************************************************************
	*   ESTRUTURA DA CLASSE
	*******************************************************************/

	/**
	* @brief Classe auxiliar para decodificar um buffer binario
	*/
	class BufferDecode : public Buffer
	{
	public:

		/**
		* Construtor da classe
		*/
		BufferDecode(void);

		/**
		* Construtor da classe
		* @param buffer - Conteúdo para o buffer
		*/
		BufferDecode(char buffer[]);

		/**
		* Pega o próximo double
		* @return double - retorna o próximo double do buffer
		*/
		double GetDouble(void);

		/**
		* Pega o próximo float
		* @return float - retorna o próximo float do buffer
		*/
		float GetFloat(void);

		/**
		* Pega o próximo int
		* @return int - retorna o próximo int do buffer
		*/
		int GetInt(void);

		/**
		* Limpa o buffer e zera m_iPosition
		*/
		void Clear(void);

	private:
		int m_iPosition;		/**< m_iPosition - posição (indice) do buffer */
	};

		
	/*******************************************************************
	*   IMPLEMENTAÇÃO DA CLASSE
	*******************************************************************/

	/**
	* Construtor da classe
	*/
	inline BufferDecode::BufferDecode(void)
		: Buffer(),
		m_iPosition(0)
	{
		Clear();
	}

	/**
	* Construtor da classe
	* @param buffer - Conteúdo para o buffer
	*/
	inline BufferDecode::BufferDecode(char buffer[])
		: Buffer(buffer),
		m_iPosition(0)
	{

	}

	/**
	* Pega o próximo double
	* @return double - retorna o próximo double do buffer
	*/
	inline double BufferDecode::GetDouble()
	{
		double value;
		memcpy(&value, &m_strBuffer[m_iPosition], sizeof(double));
		m_iPosition += sizeof(double);
		return Encoding::NetworkToHostDouble(value);
	}

	/**
	* Pega o próximo float
	* @return float - retorna o próximo float do buffer
	*/
	inline float BufferDecode::GetFloat()
	{
		float value;
		memcpy(&value, &m_strBuffer[m_iPosition], sizeof(float));
		m_iPosition += sizeof(float);
		return Encoding::NetworkToHostFloat(value);
	}

	/**
	* Pega o próximo int
	* @return int - retorna o próximo int do buffer
	*/
	inline int BufferDecode::GetInt()
	{
		int value;
		memcpy(&value, &m_strBuffer[m_iPosition], sizeof(int));
		m_iPosition += sizeof(int);
		return Encoding::NetworkToHostInt(value);
	}

	/**
	* Limpa o buffer e zera m_iPosition
	*/
	inline void BufferDecode::Clear(void)
	{
		Buffer::Clear();
		m_iPosition = 0;
	}
}