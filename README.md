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
* Trained model.
* Option to select between conventional reinforcement learning and learning options.

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
|     1-(DRS/MSD)    | It occurs depending on how close the robot is to a person who is asking.                          |
| -0.9*(1-(DRS/MSD)) | It occurs depending on how close the robot is to a person that it does not want to be approached. |
|          1         | When it detects a prohibited object and sends the warning signal.                                 |
|        -0.1        | When it does not detect a prohibited object and sends the warning signal.                         |
|        0.01        | It occurs every 10 sec. that the robot remains without colliding.                                 |
|         -1         | If the robot does not move more than 35cm in one minute.                                          |
|       -0.005       | If the robot does not find a questioner within 10 sec.                                            |

DRS: Distance Returned by Sensor.

MSD: Maximum Sensor Distance.

### Actions

#### Actions for conventional reinforcement learning
* Move forward.
* Move right.
* Move Left. 
* Rotate on its own axis clockwise.
* Rotate on its own axis counterclockwise.

#### Actions with Learning options
* Only move forward by one meter.
* Only move right by one meter.
* Only move left by one meter.
* Rotate only on its own axis clockwise.
* Rotate only on its own axis counterclockwise.

## Instructions for training a new model
1. Clone the repository.
2. Download release 6 from the Unity ML-Agents Toolkit repository.
3. In Unity open the project, select the robot and in the options bar on the right and change the behavior type to default.
4. Open a terminal at the address where the Unity ML-Agents Toolkit repository was downloaded and run the following command: ```mlagents-learn config/ppo/Tesis.yaml --run-id=Tesis```
5. run the simulation in Unity.

## Instructions for running the simulation with the trained model
1. Clone the repository.
2. In Unity open the project, select the robot and in the options bar on the right and change the behavior type to default.
3. run the simulation in Unity.

## Requirements
* Unity 5 Version >=2019.
* Unity ML-Agents Toolkit Release 6 https://github.com/Unity-Technologies/ml-agents.
* S.O. Windows 10, Linux Ubuntu. 
