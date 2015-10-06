#pragma once

#include "IFilter.h"

#ifndef BUTTER_H
#define BUTTER_H

#define CURRENT_VALUE 0
#define OLD_VALUE 1

#define LP_B 0.0305
#define LP_A -0.9391
#define	SAMPLE_RATE	0.002100

namespace Filter
{
	/*******************************************************************
	*   ESTRUTURA DA CLASSE
	*******************************************************************/

	class Butter : public IFilter
	{	
	public:
		Butter(double lpA, double lpB, double sampleRate);

	protected:
		void OnEvaluate(double value);

	private:

		double m_value;
		double m_sampleRate;
		double m_lpA;
		double m_lpB;

		double m_position[2];
		double m_velocity[2];
		double m_filteredVelocity[2];
	};

	/*******************************************************************
	*   IMPLEMENTAÇÃO DA CLASSE
	*******************************************************************/

	inline Butter::Butter(double lpA = LP_A, double lpB = LP_B, double sampleRate = SAMPLE_RATE)
		: m_sampleRate(sampleRate),
			m_lpA(lpA),
			m_lpB(lpB)
	{
		m_position[OLD_VALUE] = 0.0;
		m_position[CURRENT_VALUE] = 0.0;
		m_velocity[OLD_VALUE] = 0.0;
		m_velocity[CURRENT_VALUE] = 0.0;
		m_filteredVelocity[OLD_VALUE] = 0.0;
		m_filteredVelocity[CURRENT_VALUE] = 0.0;
	}

	inline void Butter::OnEvaluate(double value)
	{
		// Atualiza a posição
		m_position[OLD_VALUE] = m_position[CURRENT_VALUE];
		m_position[CURRENT_VALUE] = value;

		// Calcula a velocidade
		m_velocity[OLD_VALUE] = m_velocity[CURRENT_VALUE];
		m_velocity[CURRENT_VALUE] = (m_position[CURRENT_VALUE] - m_position[OLD_VALUE]) / m_sampleRate;

		// Atualiza o valor do filtrado
		m_filteredVelocity[OLD_VALUE] = m_filteredVelocity[CURRENT_VALUE];
		m_filteredVelocity[CURRENT_VALUE] = 
			m_lpB * m_velocity[CURRENT_VALUE] + 
			m_lpB * m_velocity[OLD_VALUE] - 
			m_lpA * m_filteredVelocity[OLD_VALUE];

		m_value = m_filteredVelocity[CURRENT_VALUE];
	}
}

#endif