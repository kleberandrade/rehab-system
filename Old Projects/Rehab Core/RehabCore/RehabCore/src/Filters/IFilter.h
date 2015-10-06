#pragma once

namespace Filter
{
	/*******************************************************************
	*   ESTRUTURA DA CLASSE
	*******************************************************************/

	class IFilter
	{
	public:
		
		void Evaluate(double setpoint);
		const double GetValue();

	protected:

		virtual void OnEvaluate(double setpoin) = 0;
		double m_value;

	};

	/*******************************************************************
	*   IMPLEMENTAÇÃO DA CLASSE
	*******************************************************************/

	inline void IFilter::Evaluate(double setpoint)
	{
		OnEvaluate(setpoint);
	}

	inline const double IFilter::GetValue()
	{
		return m_value;
	}

}