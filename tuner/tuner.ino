// Code for tuner


#include <Servo.h> 
 
Servo myservo;  // create servo object to control a servo 
Servo myservo2; // a maximum of eight servo objects can be created 
 
int pos = 0;    // variable to store the servo position 
int j = 0;
int k = 0;
 
void setup() 
{ 
  myservo.attach(9);  // attaches the servo on pin 9 to the servo object 
  myservo2.attach(10);
  Serial.begin(9600);
  myservo.write(j)
  myservo2.write(k)
} 
 
 
void loop() 
{ 
   if (Serial.available() > 0)   // see if incoming serial data:
  { 
    inData = Serial.read();  // read oldest byte in serial buffer:
  }
  
    if (inData == 'J') {
        j = j + 1;      
        myservo.write(j)
        inData = 0;
  
  }
  
   if (inData == 'K') {
        k = k + 1;      
        myservo1.write(k)
        inData = 0;
  
  }
  
  if (inData == 'L') {
        j = 0;      
        myservo.write(j)
        inData = 0;
  
  }
  
  if (inData == 'M') {
        k = 0;      
        myservo1.write(k)
        inData = 0;
  
  }
  
 
} 
