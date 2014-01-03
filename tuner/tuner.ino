// Code for tuner


#include <Servo.h> 
 
Servo myservo;  // create servo object to control a servo 
Servo myservo2; // a maximum of eight servo objects can be created 
 
int pos = 0;    // variable to store the servo position 
int j = 0;
int k = 0;
int inData;
 
void setup() 
{ 
  myservo.attach(9);  // attaches the servo on pin 9 to the servo object 
  myservo2.attach(10);
  Serial.begin(9600);
  myservo.write(j);
  myservo2.write(k);
} 
 
 
void loop() 
{ 
   if (Serial.available() > 0)   // see if incoming serial data:
  { 
    inData = Serial.read();  // read oldest byte in serial buffer:
  }
  
    if (inData == 'J') {  // J to increase cap by 1 deg. 
        j = j + 1; 
       if (j > 179){      // this code allows the tuner to just receive j to increase the tuning steps
         k = k + 1;       // may need to disable this if it gives issues with the control SW
         j = 0;
       } 
        myservo.write(j);
        inData = 0;
  
  }
  
   if (inData == 'K') {   // increase inductance (need to change step size)
        k = k + 14;      
        myservo2.write(180 - k);
        inData = 0;
  
  }
  
  if (inData == 'L') {      // zero capacitor
        j = 0;      
        myservo.write(j);
        inData = 0;
  
  }
  
  if (inData == 'M') {      // Zero inductor. 
  
        k = 180;      
        myservo2.write(k);
        inData = 0;
  
  }
  
 
} 
