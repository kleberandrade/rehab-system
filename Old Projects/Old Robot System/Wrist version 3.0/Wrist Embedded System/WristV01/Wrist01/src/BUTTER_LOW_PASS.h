/* 
 * File:   Butter.h
 * Author: guilhermefernandes
 *
 * Created on 28 de Agosto de 2012, 22:40
 */

#ifndef BUTTER_LOW_PASS_H
#define BUTTER_LOW_PASS_H

class BUTTER_LOW_PASS {

private:
               
    double data, data_old;
	double vf, vf_k;
    double sampleRate, a, b;               
	double W;
    
    
public:
    
BUTTER_LOW_PASS() {
        
        data = data_old = vf = vf_k = sampleRate = W = a = b = 0;
               
}
  
    

~BUTTER_LOW_PASS() {

}

/*Thsi function initializes the class to data history and time configurations*/
void init(double timePeriod, DWORD W){
    
	sampleRate = timePeriod;

	 switch (W){
        case 1:
            a = -0.9691;
			b = 0.0155;
            break;
        case 2:
            a = -0.9391;
			b = 0.0305;
            break;
        case 3:
            a = -0.9099;
			b = 0.045;
            break;
        case 4:
            a = -0.8816;
			b = 0.0592;
            break;
		case 5:
            a = -0.8541;
			b = 0.073;
            break;

		case 10:
			a = -0.7265;
			b = 0.1367;
			break;

		case 15:
			a = -0.6128;
			b = 0.1936;
			break;

		case 20:
			a = -0.5095;
			b = 0.2452;
			break;

		default:
			a = -0.8541;
			b = 0.073;
            break;
    }

    return;
            
}


void uploadData(double value){
    
	// Atualiza a posição
	data_old = data;
	data = value;

	// Atualiza o valor filtrado
	vf_k = vf;
	vf = (b*data) + (b*data_old) - (a*vf_k);

}



/* This function returns the actual derivated value*/
double getValue(void){
    
   return vf;
    
};





};

#endif	/* DERIVADOR_H */


