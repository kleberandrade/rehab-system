#pragma once

/**
* @file  RobotDispatcherMessage.h
* @brief Classe que mantem as informações enviadas para o servidor
*
* @copyright DreanNet 2011-2014, EESC-USP.
*
*/

#include <stdio.h>

#include "Net/NetworkDispatcherData.h"

/*******************************************************************
*   ESTRUTURA DA CLASSE
*******************************************************************/

/**
* @brief Classe que mantem as informações enviadas para o servidor
*/
class DispatcherData : public Net::NetworkDispatcherData
{
public:

	/**
	* Configura a posição do robô
	* @param position - Posição (ângulo) do robô
	*/
	inline void SetPosition(double position)
	{
		m_dPosition = position;
	}

	/**
	* Configura o status do robô
	* @param status - Status atual do robô
	*/
	inline void SetStatus(int status)
	{
		m_iStatus = status;
	}

protected:

	/**
	* Método de serialização das mensagens para binário
	* @param encode - buffer utilizado para codificar a mensagem em binário
	*/
	void OnSerialize(Net::BufferEncode &encode);

private:
	double m_dPosition;			/**< m_dPosition - posição (ângulo) da junta do robô */
	int m_iStatus;				/**< m_iStatus - status do robô */
};

/*******************************************************************
*   IMPLEMENTAÇÃO DA CLASSE
*******************************************************************/

/**
* Método de serialização das mensagens para binário
* @param encode - buffer utilizado para codificar a mensagem em binário
*/
inline void DispatcherData::OnSerialize(Net::BufferEncode &encode)
{
	encode.ToInt(m_iStatus);			
	encode.ToDouble(m_dPosition);
}