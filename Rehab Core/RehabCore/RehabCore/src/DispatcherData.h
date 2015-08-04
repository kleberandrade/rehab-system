#pragma once

/**
* @file  RobotDispatcherMessage.h
* @brief Classe que mantem as informa��es enviadas para o servidor
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
* @brief Classe que mantem as informa��es enviadas para o servidor
*/
class DispatcherData : public Net::NetworkDispatcherData
{
public:

	/**
	* Configura a posi��o do rob�
	* @param position - Posi��o (�ngulo) do rob�
	*/
	inline void SetPosition(double position)
	{
		m_dPosition = position;
	}

	/**
	* Configura o status do rob�
	* @param status - Status atual do rob�
	*/
	inline void SetStatus(int status)
	{
		m_iStatus = status;
	}

protected:

	/**
	* M�todo de serializa��o das mensagens para bin�rio
	* @param encode - buffer utilizado para codificar a mensagem em bin�rio
	*/
	void OnSerialize(Net::BufferEncode &encode);

private:
	double m_dPosition;			/**< m_dPosition - posi��o (�ngulo) da junta do rob� */
	int m_iStatus;				/**< m_iStatus - status do rob� */
};

/*******************************************************************
*   IMPLEMENTA��O DA CLASSE
*******************************************************************/

/**
* M�todo de serializa��o das mensagens para bin�rio
* @param encode - buffer utilizado para codificar a mensagem em bin�rio
*/
inline void DispatcherData::OnSerialize(Net::BufferEncode &encode)
{
	encode.ToInt(m_iStatus);			
	encode.ToDouble(m_dPosition);
}