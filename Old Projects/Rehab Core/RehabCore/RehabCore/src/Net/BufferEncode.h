#pragma once

/**
* @file  BufferEncode.h
* @brief Classe auxiliar para codificar um buffer binario
*
* @copyright DreanNet 2011 - 2014, EESC - USP.
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
	* @brief Classe auxiliar para codificar um buffer binario
	*/
	class BufferEncode : public Buffer
	{
	public:

		/**
		* Construtor da classe
		*/
		BufferEncode(void);

		/**
		* Construtor da classe
		* @param buffer - Conte�do para o buffer
		*/
		BufferEncode(char buffer[]);

		/**
		* Seta o pr�ximo double
		* @param dValue - double que ser� convertido para bin�rio e alocado no buffer
		*/
		void ToDouble(double dValue);

		/**
		* Seta o pr�ximo float
		* @param fValue - float que ser� convertido para bin�rio e alocado no buffer
		*/
		void ToFloat(float fValue);

		/**
		* Seta o pr�ximo int
		* @param iValue - int que ser� convertido para bin�rio e alocado no buffer
		*/
		void ToInt(int iValue);

		/**
		* Limpa o buffer e zera m_iPosition
		*/
		void Clear(void);

	private:
		int m_iPosition;			/**< m_iPosition - posi��o (indice) do buffer */
	};

	/*******************************************************************
	*   IMPLEMENTA��O DA CLASSE
	*******************************************************************/

	/**
	* Construtor da classe
	*/
	inline BufferEncode::BufferEncode(void)
		: Buffer(),
		m_iPosition(0)
	{
		Clear();
	}

	inline BufferEncode::BufferEncode(char buffer[])
		: Buffer(buffer),
		m_iPosition(0)
	{

	}

	inline void BufferEncode::ToDouble(double dValue)
	{
		double value = Encoding::HostToNetworkDouble(dValue);
		memcpy(&m_strBuffer[m_iPosition], &value, sizeof(double));
		m_iPosition += sizeof(double);
	}

	inline void BufferEncode::ToFloat(float fValue)
	{
		float value = Encoding::HostToNetworkFloat(fValue);
		memcpy(&m_strBuffer[m_iPosition], &value, sizeof(float));
		m_iPosition += sizeof(float);
	}

	inline void BufferEncode::ToInt(int iValue)
	{
		int value = Encoding::HostToNetworkInt(iValue);
		memcpy(&m_strBuffer[m_iPosition], &value, sizeof(int));
		m_iPosition += sizeof(int);
	}

	inline void BufferEncode::Clear(void)
	{
		Buffer::Clear();
		m_iPosition = 0;
	}
}
