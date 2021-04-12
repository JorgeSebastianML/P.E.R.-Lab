# P.E.R. Lab

<img src="https://github.com/JorgeSebastianML/Unity_simulation_pepper_robot/blob/main/Img/LOGO_PERLAB-03.png " width="300" height="300">

Simulator in Unity 5 of a laboratory to carry out reinforcement learning for the Pepper robot.



## Contains
* Pepper robot model with the following sensors:
  * Infrared sensors.
  * 2D Cameras. 
  * 3D Camera.
* The environment has the following 3D models:
  * Table.
  * Chair.
  * Monitor.
  * Keyboard.
  * Oscilloscope.
  * Multimeter.
  * DC source.
  * Signal generator.
  * Apple.
  * Water bottle.
  * Pringles. 
  * Coca cola bottle.
  * Cell phone. 
* 3D models of people obtained from Mixamo, which can make the following animations:
  * Walk. 
  * Type on a pc.
  * Ask.
  * Ask to stop.

## Reinforcement Learning

### Supported algorithms
* Proximal Police Optimization (PPO).
* Soft Actor-Critic (SAC).
* Q-Learning.

### Environmental observations
* Linear speed of the robot in the X axis.
* Linear speed of the robot in the Z axis.
* Angular speed of rotation of the robot about its own axis.
* Head angle.
* Distance information from the robot's laser sensors.
* Depth information from the robot's 3D camera.
* Images captured by the robot's 2D cameras.

### Default rewards

|       Reward       |                                            Description                                            |
|:------------------:|:-------------------------------------------------------------------------------------------------:|
|         -1         | When the robot detects with the lasers objects less than 40 cm away.                              |
|     1-(DRS/MDS)    | Se da dependiendo de que tanto se acerque el robot a  una persona que esta preguntando.           |
| -0.9*(1-(DRS/MDS)) | Se da dependiendo de que tanto se acerque el robot a una persona que no quiere que se le acerque. |
|          1         | Cuando detecta un objeto prohibido y manda la señal de advertencia.                               |
|        -0.1        | Cuando no detecta un objeto prohibido y manda la señal de advertencia.                            |
|        0.01        | Se da cada 10 seg. que el robot permanezca sin chocar.                                            |
|         -1         | Si el robot no se mueve mas de 35cm en un minuto.                                                 |
|       -0.005       | Si el robot no encuentra una persona que pregunta en 10 seg.                                      |

## Requirements
* Unity 5 Version >=2019.
* ml-agents unity Release 6 https://github.com/Unity-Technologies/ml-agents.
* S.O. Windows 10, Linux Ubuntu. 
