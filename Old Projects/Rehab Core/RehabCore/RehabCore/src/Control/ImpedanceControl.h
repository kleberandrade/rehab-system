#pragma once

#define	PERIOD					0.002100
#define KIMP 					200.0   
#define BIMP 					20.0	 
#define MOTOR_GAIN 				0.5  
#define CURRENT_MAX 			5000
#define CURRENT_MIN 			-5000

#ifndef IMPEDANCE_CONTROL_H
#define IMPEDANCE_CONTROL_H

#include "Filters/IFilter.h"
#include "Filters/Butter.h"

namespace Control
{
	/*******************************************************************
	*   ESTRUTURA DA CLASSE
	*******************************************************************/

	class ImpedanceControl
	{
		public:
			ImpedanceControl(Filter::IFilter &filter, double bimp, double kimp, double motorGain, double currentMin, double currentMax);
			void Update(double);
			const double GetCurrent();
			void SetFilter(Filter::IFilter &filter);

		private:
			double m_bimp;
			double m_kimp;
			double m_motorGain;
			double m_currentMax;
			double m_currentMin;
			double m_positionError;
			double m_setpointPosition;
			double m_velocityError;
			double m_zero;
			double m_actionTorque;
			double m_actionCurrent;
			Filter::IFilter *m_filter;
	};

	/*******************************************************************
	*   IMPLEMENTAÇÃO DA CLASSE
	*******************************************************************/

	inline ImpedanceControl::ImpedanceControl(Filter::IFilter &filter, double bimp = BIMP, double kimp = KIMP, double motorGain = MOTOR_GAIN, double currentMin = CURRENT_MIN, double currentMax = CURRENT_MAX)
		: m_filter(&filter),
		  m_bimp(bimp),
		  m_kimp(kimp),
		  m_motorGain(motorGain),
		  m_currentMin(currentMin),
		  m_currentMax(currentMax)
	{

	}

	inline void ImpedanceControl::Update(double setpoint)
	{
		m_filter->Evaluate(setpoint);

		m_positionError = m_setpointPosition - setpoint;
		m_velocityError = 0.0 - m_filter->GetValue();
		m_actionTorque = (m_kimp * m_positionError) + (m_bimp * m_velocityError);
		m_actionCurrent = m_motorGain * m_actionTorque;

		if (m_actionCurrent > m_currentMax) 
			m_actionCurrent = m_currentMax;

		if (m_actionCurrent < m_currentMin) 
			m_actionCurrent = m_currentMin;
	}

	inline const double ImpedanceControl::GetCurrent()
	{
		return m_actionCurrent;
	}

	inline void ImpedanceControl::SetFilter(Filter::IFilter &filter)
	{
		m_filter = &filter;
	}
}

#endif