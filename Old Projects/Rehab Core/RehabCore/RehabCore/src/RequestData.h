#pragma once

/**
* @file  GameRequestMessage.h
* @brief Classe que mantem as informações recebidas do servidor (Game)
*
* @copyright DreanNet 2011-2014, EESC-USP.
*
*/

#include "Net/NetworkRequestData.h"


/*******************************************************************
*   ESTRUTURA DA CLASSE
*******************************************************************/

/**
* @brief Classe que mantem as informações recebidas do servidor (Game)
*/
class RequestData : public Net::NetworkRequestData
{
public:

	/**
	* Pega o setpoint de posição do controle de impedância
	* @return m_dPosition
	*/
	inline double GetPosition() const
	{
		return m_dPosition;
	}

	/**
	* Pega a rigidez do controle de impedância
	* @return m_dStiffness
	*/
	inline double GetStiffness() const
	{
		return m_dStiffness;
	}

	/**
	* Pega o setpoint de velocidade do controle de impedância
	* @return m_dVelocity
	*/
	inline double GetVelocity() const
	{
		return m_dVelocity;
	}

	/**
	* Pega o setpoint de aceleração do controle de impedância
	* @return m_dAcceleration 
	*/
	inline double GetAcceleration() const
	{
		return m_dAcceleration;
	}

	/**
	* Pega o controle enviado pelo jogo
	* @return m_iControl
	*/
	inline int GetControl() const
	{
		return m_iControl;
	}

protected:
	/**
	* Método de deserialização da mensagen em binário
	* @param decode - buffer utilizado para decodificar a mensagem em binário
	*/
	void OnDeserialize(Net::BufferDecode &decode);

private:
	double m_dPosition;				/**< m_dPosition - setpoint de posição para o controle de impedância */
	double m_dStiffness;			/**< m_dStiffness - rigidez do controle de impedância */
	double m_dVelocity;				/**< m_dVelocity - setpoint de velocidade para o controle de impedância */
	double m_dAcceleration;			/**< m_dAcceleration - setpoint de aceleração para o controle de impedância */
	int m_iControl;					/**< m_iControl - controle enviado do jogo para o robô */
};

/*******************************************************************
*   IMPLEMENTAÇÃO DA CLASSE
*******************************************************************/

inline void RequestData::OnDeserialize(Net::BufferDecode &decode)
{
	m_iControl = decode.GetInt();
	m_dPosition = decode.GetDouble();
	m_dStiffness = decode.GetDouble();
	m_dVelocity = decode.GetDouble();
	m_dAcceleration = decode.GetDouble();
}